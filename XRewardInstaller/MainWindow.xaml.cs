﻿using System.Diagnostics;
using System.Windows;

namespace Spareio.Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Installation started --");
        }
    }
}
