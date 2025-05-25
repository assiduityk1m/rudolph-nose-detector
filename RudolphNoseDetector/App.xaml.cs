using System;
using RudolphNoseDetector.ViewModels;
using System.Windows;

namespace RudolphNoseDetector;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
        
    /// <summary>
    /// Application Entry for RudolphNoseDetector
    /// </summary>
    public App()
    {
        var view = new MainWindow
        {
            DataContext = Activator.CreateInstance<MainViewModel>()
        };
            
        view.Show();
    }
        
}