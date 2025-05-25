using System.Windows;
using System;



namespace RudolphNoseDetector
{
    public partial class mainwindow : Window
    {
        public mainwindow()
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