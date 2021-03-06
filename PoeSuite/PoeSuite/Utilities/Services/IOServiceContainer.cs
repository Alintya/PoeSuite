﻿using PoeSuite.DataTypes.Interfaces;

using System.Windows.Forms;
using System.IO;
using System;

namespace PoeSuite.Utilities.Services
{
    internal class IOServiceContainer : IOService
    {
        public Stream OpenFile(string path)
        {
            throw new NotImplementedException();
        }

        public string OpenFileDialog(string defaultPath)
        {
            throw new NotImplementedException();
        }

        public string OpenFileDialog()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                //openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                //openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    return openFileDialog.FileName;

                return string.Empty;
            }
        }
    }
}
