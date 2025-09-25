namespace Mythral.Server.Config;

public sealed class ServerOptions
{
    public int TcpPort { get; set; } = 27016;
    public int UdpPort { get; set; } = 27015;
    public int TickRate { get; set; } = 60;
    public string UdpKey { get; set; } = "CHANGE_THIS_KEY"; // TODO: load from secret/env
}
