﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PoeSuite
{
    /// <summary>
    /// Interaction logic for TextBoxPrompt.xaml
    /// </summary>
    public partial class TextBoxPrompt : Window
    {
        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }


        public TextBoxPrompt(string title, string message)
        {
            InitializeComponent();

            Title = title;
            TextField.Text = message;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
