using Figgle;
using Figgle.Fonts;

namespace Mythral.Server.Application.Utils
{
    public static class ServerUtils
    {
        public static void PrintServerLogo()
        {
            Console.WriteLine(
                FiggleFonts.Standard.Render("Mythral - 1.0.0"));
        }
    }
}