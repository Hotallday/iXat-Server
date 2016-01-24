using System;
using System.Runtime.InteropServices;

namespace iXat_Server {
    sealed internal class Program {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        public static bool IsMono => Type.GetType("Mono.Runtime") != null;

        /// <summary>
        /// Server startup method
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args) {
            if (!IsMono) {
                Console.Title = "Xat Server";
                DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);
            }
            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) => {
                Server.PerformShutDown();
                e.Cancel = true;
            };
            Console.CursorVisible = false;
            Server.Initialize();
        }
    }
}