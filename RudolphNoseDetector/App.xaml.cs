using System;
using RudolphNoseDetector.ViewModels;
using System.Windows;

namespace RudolphNoseDetector;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{

    protected override void OnStartup(StartupEventArgs e)
    {
        // 중복 실행 방지
        var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
        var processes = System.Diagnostics.Process.GetProcessesByName(currentProcess.ProcessName);

        if (processes.Length == 1)
        {
            //MessageBox.Show("애플리케이션이 이미 실행 중입니다.", "알림",
            //    MessageBoxButton.OK, MessageBoxImage.Information);
            //Shutdown();
            return;
        }

        base.OnStartup(e);

        // 메인 윈도우를 명시적으로 생성하고 표시
        var mainWindow = new mainwindow();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // 리소스 정리
        base.OnExit(e);
    }

    /// <summary>
    /// Application Entry for RudolphNoseDetector
    /// </summary>
    public App()
    {
        var view = new mainwindow
        {
            DataContext = Activator.CreateInstance<MainViewModel>()
        };
            
        view.Show();
    }
        
}