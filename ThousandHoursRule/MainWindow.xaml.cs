using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ThousandHoursRule
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private ObservableCollection<TimerViewModel> _timers;
        private const int MaxTimers = 20;

        public MainWindow()
        {
            this.InitializeComponent();
            _timers = new ObservableCollection<TimerViewModel>();
            timersListView.ItemsSource = _timers;
        }

        private void addTimerButtonClick(object sender, RoutedEventArgs e)
        {
            if (_timers.Count < MaxTimers)
            {
                var newTimer = new TimerViewModel();

                newTimer.RemoveTimer += OnRemoveTimer;

                _timers.Add(newTimer); // Add a new timer of 1000 hours
            }
            else
            {
                // Show message when max limit is reached
                ShowMessage("Jack of all trades, master of none.", TimeSpan.FromSeconds(2));
            }
        }

        private void OnRemoveTimer(object sender, EventArgs e)
        {
            if (sender is TimerViewModel timer)
            {
                // Remove the timer from the collection
                _timers.Remove(timer);
            }
        }

        private void ShowMessage(string message, TimeSpan duration)
        {
            messageTextBlock.Text = message;
            messageTextBlock.Visibility = Visibility.Visible;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = duration;
            timer.Tick += (sender, e) =>
            {
                messageTextBlock.Visibility = Visibility.Collapsed;
                timer.Stop();
            };
            timer.Start();
        }
    }
}
