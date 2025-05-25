using System.Windows;
using System;

//using OpenCvSharp;


namespace RudolphNoseDetector
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //InitializeComponent();
            //DataContext = new MainWindow();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnClosed(e);
        }
    }
}