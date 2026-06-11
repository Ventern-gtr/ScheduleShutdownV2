using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;

namespace ScheduleShutdownV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            int saved = LoadDelay();
            DelayMinutes = saved;
            DelaySlider.Value = saved;
            loaded = true;
        }

        private int _delayMinutes;
        private bool loaded;
        public int DelayMinutes
        {
            get => _delayMinutes;
            set
            {
                _delayMinutes = value;
                DelayDisplay.Text = $"{_delayMinutes} Minutes";
                DelaySlider.Value = _delayMinutes;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = "/a",
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }

        private void ScheduleShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            int seconds = DelayMinutes * 60;
            Process.Start(new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = $"/s /t {seconds}",
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }

        private void DelaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!loaded) return;

            _delayMinutes = (int)e.NewValue;
            DelayDisplay.Text = $"{_delayMinutes} Minutes";
            SaveDelay(_delayMinutes);
        }

        private void SaveDelay(int minutes)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\ScheduleShutdownV2");
            key.SetValue("DelayMinutes", minutes, RegistryValueKind.DWord);
            key.Close();
        }

        private int LoadDelay()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\ScheduleShutdownV2");
            if (key == null)
                return 30;
            object value = key.GetValue("DelayMinutes");
            return value != null ? (int)value : 30;
        }
    }
}