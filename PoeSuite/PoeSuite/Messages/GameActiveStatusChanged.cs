using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.Messages
{
    class GameActiveStatusChanged
    {
        public bool IsInForeground { get; set; }
    }
}
