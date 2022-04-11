using System;

namespace AttackOnTitan
{
    public static class Program
    {
        public static void Main()
        {
            using (var manager = new SceneManager())
            {
                manager.Run();
            }
        }
    }
}
