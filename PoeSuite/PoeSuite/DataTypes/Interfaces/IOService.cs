using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.DataTypes.Interfaces
{
    // Untestable IO functions
    public interface IOService
    {
        string OpenFileDialog(string defaultPath);
        string OpenFileDialog();

        Stream OpenFile(string path);
    }
}
