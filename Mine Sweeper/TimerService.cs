using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace Mine_Sweeper
{
    public class TimerService : INotifyPropertyChanged
    {
		public DispatcherTimer timer;
		private int _secondsCounter = 0;
		public int SecondsCounter {get => _secondsCounter ;set { _secondsCounter = value; OnPropertyChanged(); } }

		public void InitializeTimer()
		{
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
		}

		public void Timer_Tick(object sender, EventArgs e)
		{
			var mainWindow = Application.Current.MainWindow as MainWindow;
			SecondsCounter++;
			mainWindow.TimerDisplay.Text = SecondsCounter.ToString();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
