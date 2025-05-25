using System.Windows;
using System;



namespace RudolphNoseDetector
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"예외 발생: {ex.Message}\n자세한 예외: {ex.InnerException?.Message}");
                throw;
            }
            //Console.WriteLine("예외 발생: " + ex.Message);
            //Console.WriteLine("자세한 예외: " + ex.InnerException?.Message);      
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