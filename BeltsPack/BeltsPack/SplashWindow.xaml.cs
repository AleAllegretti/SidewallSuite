
using System;
using System.Windows;
using System.Windows.Threading;

namespace BeltsPack
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
            //this.StartLoadingBar();
            this.StartCloseTimer();

        }
        private void StartCloseTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2d);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;

            // Nasconde lo splash screen
            this.Hide();

            // Apre la main window
            MainWindow mainWindow = new MainWindow();
            mainWindow.ShowDialog();

            // Chiude lo splash screen
            this.Close();
        }

        //private void StartLoadingBar()
        //{
        //    DispatcherTimer Progresstimer = new DispatcherTimer();
        //    Progresstimer.Interval = TimeSpan.FromSeconds(0.1d);
        //    Timer.Maximum = 100;
        //    Progresstimer.Tick += Timer_Tick;
        //    Progresstimer.Start();
        //}
        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    if (Timer.Progress <100)
        //    {
        //        Timer.Progress += 10;
        //    }
            
        //}
    }
}

