using System.Windows;
using System.Windows.Threading;

namespace WpfApp2.Views
{
    public partial class NotificationWindow : Window
    {
        public NotificationWindow(string title, string desc)
        {
            InitializeComponent();
            TitleText.Text = title;
            DescText.Text = desc;
            Loaded += (s, e) =>
            {
                var desktopWorkingArea = SystemParameters.WorkArea;
                Left = desktopWorkingArea.Left + (desktopWorkingArea.Width - Width) / 2;
                Top = desktopWorkingArea.Top + (desktopWorkingArea.Height - Height) / 2;
            };
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            timer.Tick += (s, e) => { timer.Stop(); Close(); };
            timer.Start();
        }
    }
} 