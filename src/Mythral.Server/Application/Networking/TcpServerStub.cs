using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Mythral.Server.Domain.Game.Messages;
using Mythral.Server.Domain.Networking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mythral.Server.Config;
using Mythral.Server.Serialization;
using Mythral.Shared.Messages;

namespace Mythral.Server.Application.Networking;

public sealed class TcpServerStub : ITcpServer, IAsyncDisposable
{
    private readonly ILogger<TcpServerStub> _logger;
    private readonly ServerOptions _options;
    private readonly IMessageSerializer _serializer;
    private TcpListener? _listener;
    private readonly CancellationTokenSource _cts = new();
    private readonly List<Task> _clientTasks = new();

    private readonly ConcurrentDictionary<Guid, ConnectionContext> _connections = new();
    private readonly ConcurrentQueue<ServerRequest> _requestInbox = new();

    private const ushort Magic = 0x4D54; // 'M''T'
    private const byte Version = 1;

    private static readonly Dictionary<ushort, Type> OpcodeToType = new()
    {
        [(ushort)Opcode.Ping] = typeof(PingPacket),
        [(ushort)Opcode.Pong] = typeof(PongPacket),
        [(ushort)Opcode.ChatMessage] = typeof(ChatMessage),
        [(ushort)Opcode.InventoryOp] = typeof(InventoryOp),
        [(ushort)Opcode.PlayerInput] = typeof(PlayerInputMsg),
        [(ushort)Opcode.CombatEvent] = typeof(CombatEvent),
        [(ushort)Opcode.StateSnapshot] = typeof(StateSnapshot),
        [(ushort)Opcode.LoginRequest] = typeof(LoginRequest),
        [(ushort)Opcode.LoginOk] = typeof(LoginOk),
        [(ushort)Opcode.UdpHello] = typeof(UdpHello)
    };

    private static readonly Dictionary<Type, ushort> TypeToOpcode = OpcodeToType.ToDictionary(kv => kv.Value, kv => kv.Key);

    public TcpServerStub(ILogger<TcpServerStub> logger, IOptions<ServerOptions> options, IMessageSerializer serializer)
    {
        _logger = logger;
        _options = options.Value;
        _serializer = serializer;
        _ = StartAsync();
    }

    private async Task StartAsync()
    {
        try
        {
            _listener = new TcpListener(IPAddress.Any, _options.TcpPort);
            _listener.Start();
            _logger.LogInformation("TCP server escutando porta {Port}", _options.TcpPort);
            _ = AcceptLoopAsync(_cts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao iniciar TCP listener na porta {Port}", _options.TcpPort);
            throw;
        }
    }

    private async Task AcceptLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            TcpClient? rawClient = null;
            try
            {
                rawClient = await _listener!.AcceptTcpClientAsync(token).ConfigureAwait(false);
                rawClient.NoDelay = true;
                var ctx = new ConnectionContext(rawClient);
                _connections[ctx.Id] = ctx;
                _logger.LogInformation("Nova conexão TCP {Id} de {Endpoint}", ctx.Id, rawClient.Client.RemoteEndPoint);
                var t = HandleClientAsync(ctx, token);
                lock (_clientTasks) _clientTasks.Add(t);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no accept loop TCP");
                rawClient?.Close();
            }
        }
    }

    private async Task HandleClientAsync(ConnectionContext ctx, CancellationToken token)
    {
        using var c = ctx.Client;
        var stream = ctx.Client.GetStream();
        var headerBuffer = new byte[9]; // 2 magic +1 ver +2 opcode +4 length
        try
        {
            while (!token.IsCancellationRequested && ctx.Client.Connected)
            {
                if (!await ReadExactAsync(stream, headerBuffer, headerBuffer.Length, token).ConfigureAwait(false)) break;

                var magic = BinaryPrimitives.ReadUInt16BigEndian(headerBuffer.AsSpan(0, 2));
                if (magic != Magic)
                {
                    _logger.LogWarning("Conexão {Id} header inválido magic=0x{Magic:X4}", ctx.Id, magic);
                    break;
                }
                var version = headerBuffer[2];
                if (version != Version)
                {
                    _logger.LogWarning("Conexão {Id} versão não suportada {Version}", ctx.Id, version);
                    break;
                }
                var opcode = BinaryPrimitives.ReadUInt16BigEndian(headerBuffer.AsSpan(3, 2));
                var length = BinaryPrimitives.ReadUInt32BigEndian(headerBuffer.AsSpan(5, 4));
                if (length > 1024 * 1024) // 1MB hard limit
                {
                    _logger.LogWarning("Conexão {Id} payload muito grande {Length}", ctx.Id, length);
                    break;
                }
                var payloadBuffer = length > 0 ? ArrayPool<byte>.Shared.Rent((int)length) : Array.Empty<byte>();
                try
                {
                    if (length > 0 && !await ReadExactAsync(stream, payloadBuffer, (int)length, token).ConfigureAwait(false))
                        break;
                    await HandlePacketAsync(ctx, opcode, payloadBuffer, (int)length, stream, token).ConfigureAwait(false);
                }
                finally
                {
                    if (length > 0)
                        ArrayPool<byte>.Shared.Return(payloadBuffer);
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Conexão TCP encerrada por erro {Endpoint}", ctx.Client.Client.RemoteEndPoint);
        }
        finally
        {
            _connections.TryRemove(ctx.Id, out _);
            _logger.LogInformation("Conexão TCP fechada {Id} {Endpoint}", ctx.Id, ctx.Client.Client.RemoteEndPoint);
        }
    }

    private async Task<bool> ReadExactAsync(NetworkStream stream, byte[] buffer, int length, CancellationToken token)
    {
        int readTotal = 0;
        while (readTotal < length)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(readTotal, length - readTotal), token).ConfigureAwait(false);
            if (read == 0) return false; // disconnect
            readTotal += read;
        }
        return true;
    }

    private async Task HandlePacketAsync(ConnectionContext ctx, ushort opcode, byte[] buffer, int length, NetworkStream stream, CancellationToken token)
    {
        ctx.LastSeen = DateTimeOffset.UtcNow;
        if (!OpcodeToType.TryGetValue(opcode, out var type))
        {
            _logger.LogWarning("Opcode desconhecido {Opcode} (conexão {Conn})", opcode, ctx.Id);
            return;
        }

        object? model = null;
        if (length > 0)
        {
            try { model = _serializer.Read(new ReadOnlySpan<byte>(buffer, 0, length), type); }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao desserializar opcode {Opcode} len={Length}", opcode, length);
                return;
            }
        }

        // Handshake obrigatório: primeiro pacote deve ser LoginRequest
        if (!ctx.IsAuthenticated && opcode != (ushort)Opcode.LoginRequest)
        {
            _logger.LogWarning("Conexão {Id} enviou opcode {Opcode} antes do LoginRequest", ctx.Id, opcode);
            ctx.Client.Close();
            return;
        }

        if (opcode == (ushort)Opcode.LoginRequest)
        {
            var login = (LoginRequest?)model;
            if (login == null)
            {
                _logger.LogWarning("LoginRequest inválido (null) {Id}", ctx.Id);
                ctx.Client.Close();
                return;
            }
            // Stub de autenticação - aceitar tudo
            ctx.IsAuthenticated = true;
            ctx.AccountId = Random.Shared.Next(1, 1_000_000);
            var loginOk = new LoginOk
            {
                SessionId = Guid.NewGuid(),
                UdpToken = Guid.NewGuid().ToString("N"),
                UdpPort = _options.UdpPort,
                TickRate = _options.TickRate
            };
            await SendInlineAsync(stream, loginOk, token).ConfigureAwait(false);
            _logger.LogInformation("Conexão {Id} autenticada como account {Acc}", ctx.Id, ctx.AccountId);
            return;
        }

        if (opcode == (ushort)Opcode.Ping)
        {
            var ping = model as PingPacket ?? new PingPacket();
            var pong = new PongPacket { EchoTimestamp = ping.Timestamp, ServerTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
            await SendInlineAsync(stream, pong, token).ConfigureAwait(false);
            return;
        }

        // Outros pacotes autenticados vão para fila
        _requestInbox.Enqueue(new ServerRequest
        {
            ConnectionId = ctx.Id,
            Opcode = opcode,
            BinaryPayload = length > 0 ? buffer.Take(length).ToArray() : Array.Empty<byte>()
        });
    }

    private async Task SendInlineAsync(NetworkStream stream, object packet, CancellationToken token)
    {
        if (!TypeToOpcode.TryGetValue(packet.GetType(), out var opcode))
        {
            _logger.LogWarning("Tentativa de enviar tipo sem opcode registrado {Type}", packet.GetType().Name);
            return;
        }
        byte[] payload;
        try { payload = _serializer.Write(packet.GetType(), packet); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao serializar packet {Type}", packet.GetType().Name);
            return;
        }
        await SendRawAsync(stream, opcode, payload, token).ConfigureAwait(false);
    }

    private async Task SendRawAsync(NetworkStream stream, ushort opcode, ReadOnlyMemory<byte> payload, CancellationToken token)
    {
        var header = new byte[9];
        BinaryPrimitives.WriteUInt16BigEndian(header.AsSpan(0, 2), Magic);
        header[2] = Version;
        BinaryPrimitives.WriteUInt16BigEndian(header.AsSpan(3, 2), opcode);
        BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(5, 4), (uint)payload.Length);
        await stream.WriteAsync(header, token).ConfigureAwait(false);
        if (!payload.IsEmpty)
            await stream.WriteAsync(payload, token).ConfigureAwait(false);
    }

    public void DrainRequests(ConcurrentQueue<ServerRequest> requestQueue)
    {
        while (_requestInbox.TryDequeue(out var req))
            requestQueue.Enqueue(req);
    }

    public void Send<T>(T packet)
    {
        if (packet == null) return;
        if (!TypeToOpcode.TryGetValue(typeof(T), out var opcode))
        {
            _logger.LogWarning("Tipo não registrado para envio {Type}", typeof(T).Name);
            return;
        }
        byte[] payload;
        try { payload = _serializer.Write(packet); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha serializando {Type}", typeof(T).Name);
            return;
        }
        foreach (var kv in _connections)
        {
            if (!kv.Value.Client.Connected || !kv.Value.IsAuthenticated) continue;
            try { _ = SendRawAsync(kv.Value.Client.GetStream(), opcode, payload, _cts.Token); } catch { }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        try { _listener?.Stop(); } catch { }
        Task[] tasks;
        lock (_clientTasks) tasks = _clientTasks.ToArray();
        await Task.WhenAll(tasks);
        _cts.Dispose();
    }
}
