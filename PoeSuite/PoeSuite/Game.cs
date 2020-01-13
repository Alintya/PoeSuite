using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite
{
    class Game
    {
        public static readonly string[] ExecutableNames = { "PathOfExile.exe", "PathOfExile_x64.exe", "PathOfExileSteam.exe", "PathOfExile_x64Steam.exe", "PathOfExile_KG.exe", "PathOfExile_x64_KG.exe" };

        static Process[] GetRunningInstances()
        {
            var instances = new Process[] { };

            return instances;
        }

        public Game()
        {

        }
    }
}
