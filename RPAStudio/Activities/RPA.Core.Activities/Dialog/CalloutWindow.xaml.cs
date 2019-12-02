using System;
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
using System.Windows.Threading;

namespace RPA.Core.Activities.DialogActivity.Windows
{
    /// <summary>
    /// CalloutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CalloutWindow : Window
    {

        private Int32 closeTimer { get; set; }
        public CalloutWindow(Int32 _closeTimer)
        {
            InitializeComponent();
            closeTimer = _closeTimer;
        }

        public void setTitle(string strTitle)
        {
            TitleLabel.Content = strTitle;
        }

        public void setContent(string strContent)
        {
            ContentLabel.Content = strContent;
        }

        public double getCanvasHeight()
        {
            return canvas.Height;
        }

        public double getCanvasWidth()
        {
            return canvas.Width;
        }

        private void CloseButtonRectangle_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(closeTimer != 0)
                StartCloseTimer();

            TopMostWindows();
        }

        private void TopMostWindows()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timerTopMost;
            timer.Start();
        }

        private void timerTopMost(object sender, EventArgs e)
        {
            this.Topmost = true;
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= timerTopMost;
        }

        private void StartCloseTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(closeTimer); 
            timer.Tick += TimerTick; 
            timer.Start();
        }
        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick; 
            this.Close();
        }
    }
}
