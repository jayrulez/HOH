using SiliconStudio.Xenko.Engine;

namespace HOH
{
    class HOHApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
