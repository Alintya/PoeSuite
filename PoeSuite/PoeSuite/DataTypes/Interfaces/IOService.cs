using System.IO;

namespace PoeSuite.DataTypes.Interfaces
{
    // Untestable IO functions
    internal interface IOService
    {
        string OpenFileDialog(string defaultPath);
        string OpenFileDialog();
        Stream OpenFile(string path);
    }
}
