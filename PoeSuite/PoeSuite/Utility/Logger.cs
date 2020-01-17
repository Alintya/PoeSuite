using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

// TODO: implement proper logging, that does not write to stdout in release mode

namespace PoeSuite.Utility
{
    public static class Logger
    {
        public static void Log(string msg, ConsoleColor clr = ConsoleColor.Gray, bool wait = false)
        {
            Console.ForegroundColor = clr;
            Console.WriteLine($"[{DateTime.Now}] {msg}");
            Console.ResetColor();

            if (wait)
                Console.ReadLine();
        }
    }
}
