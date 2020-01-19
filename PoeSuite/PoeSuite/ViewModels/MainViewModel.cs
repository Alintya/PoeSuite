using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string WindowTitle { get; private set; }


        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                WindowTitle = "PoeSuite (Designmode)";
            }
            else
            {
                WindowTitle = "PoeSuite - Settings";
            }
        }
    }
}
