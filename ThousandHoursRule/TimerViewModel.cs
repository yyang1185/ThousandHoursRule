using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ThousandHoursRule
{
    public class TimerViewModel : INotifyPropertyChanged
    {
        private DispatcherTimer _dispatcherTimer;
        private TimeSpan _timeLeft;
        private string _displayTime;
        private string _title;
        private string _startButtonContent;
        private Visibility _startOverButtonVisibility;
        private Visibility _cancelButtonVisibility;
        private Visibility _editVisibility;
        private Visibility _timerVisibility;
        private bool _isInEditMode;
        private bool _isConfirmingCancel; // New property to control confirmation overlay
        private bool _isConfirmingReset;
        private Symbol _startIcon;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler RemoveTimer;

        public TimerViewModel()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimer.Tick += DispatcherTimer_Tick;

            _timeLeft = TimeSpan.FromHours(1000);
            _startButtonContent = "Start";
            _startIcon = Symbol.Play;
            _startOverButtonVisibility = Visibility.Collapsed;
            _cancelButtonVisibility = Visibility.Visible; // Always visible
            _editVisibility = Visibility.Visible; // Start in edit mode
            _timerVisibility = Visibility.Collapsed; // Hide timer initially
            UpdateDisplayTime();
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            if (_timeLeft > TimeSpan.Zero)
            {
                _timeLeft = _timeLeft.Subtract(TimeSpan.FromSeconds(1));
                UpdateDisplayTime();
            }
            else
            {
                _dispatcherTimer.Stop();
                StartButtonContent = "Start";
                StartIcon = Symbol.Play;
                StartOverButtonVisibility = Visibility.Collapsed;
            }
        }

        private void UpdateDisplayTime()
        {
            int totalHours = (int)_timeLeft.TotalHours;
            DisplayTime = string.Format("{0:D2}:{1:D2}:{2:D2}", totalHours, _timeLeft.Minutes, _timeLeft.Seconds);
        }

        public string CancelConfirmationMessage => $"Do you want to close the timer for '{Title}'?";
        public string ResetConfirmationMessage => $"Do you want to reset the timer for '{Title}'?";

        public string DisplayTime
        {
            get { return _displayTime; }
            set
            {
                _displayTime = value;
                OnPropertyChanged();
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CancelConfirmationMessage)); // Notify UI to update message when Title changes
                OnPropertyChanged(nameof(ResetConfirmationMessage)); // Notify UI to update message when Title changes
            }
        }

        public string StartButtonContent
        {
            get { return _startButtonContent; }
            set
            {
                _startButtonContent = value;
                OnPropertyChanged();
            }
        }

        public Symbol StartIcon
        {
            get { return _startIcon; }
            set
            {
                _startIcon = value;
                OnPropertyChanged();
            }
        }

        public Visibility StartOverButtonVisibility
        {
            get { return _startOverButtonVisibility; }
            set
            {
                _startOverButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility CancelButtonVisibility
        {
            get { return _cancelButtonVisibility; }
            set
            {
                _cancelButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility EditVisibility
        {
            get { return _editVisibility; }
            set
            {
                _editVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility TimerVisibility
        {
            get { return _timerVisibility; }
            set
            {
                _timerVisibility = value;
                OnPropertyChanged();
            }
        }

        public bool IsConfirmingCancel
        {
            get { return _isConfirmingCancel; }
            set
            {
                _isConfirmingCancel = value;
                OnPropertyChanged();
            }
        }

        public bool IsConfirmingReset
        {
            get { return _isConfirmingReset; }
            set
            {
                _isConfirmingReset = value;
                OnPropertyChanged();
            }
        }


        public ICommand ConfirmTitleCommand => new RelayCommand(o =>
        {
            if (!string.IsNullOrEmpty(Title))
            {
                _isInEditMode = false;
                EditVisibility = Visibility.Collapsed; // Hide the edit box
                TimerVisibility = Visibility.Visible;  // Show the timer and start button
            }
        });

        public ICommand StartButtonClick => new RelayCommand(o =>
        {
            if (!_isInEditMode && !_isConfirmingCancel) 
            {
                if (StartButtonContent == "Start")
                {
                    StartButtonContent = "Pause";
                    StartIcon = Symbol.Pause; // Change to Pause icon
                    StartOverButtonVisibility = Visibility.Visible;
                    _dispatcherTimer.Start();
                }
                else if (StartButtonContent == "Pause")
                {
                    StartButtonContent = "Continue";
                    StartIcon = Symbol.Play; // Change to Play icon for "Continue"
                    _dispatcherTimer.Stop();
                }
                else if (StartButtonContent == "Continue")
                {
                    StartButtonContent = "Pause";
                    StartIcon = Symbol.Pause; // Change back to Pause icon
                    _dispatcherTimer.Start();
                }
            }
        });

        public ICommand CancelButtonClick => new RelayCommand(o =>
        {
            if (StartButtonContent == "Pause")
            {
                StartButtonContent = "Continue";
                StartIcon = Symbol.Play;
                _dispatcherTimer.Stop();
                TimerVisibility = Visibility.Collapsed;
            }
            else if (StartButtonContent == "Start" || StartButtonContent == "Continue")
            {
                TimerVisibility = Visibility.Collapsed;
            }

            IsConfirmingCancel = true;
        });

        public ICommand ConfirmCancelCommand => new RelayCommand(o =>
        {
            // Trigger event to remove timer
            RemoveTimer?.Invoke(this, EventArgs.Empty);
        });

        public ICommand CancelConfirmationCommand => new RelayCommand(o =>
        {
            // Hide the confirmation overlay
            IsConfirmingCancel = false; 
            IsConfirmingReset = false;

            TimerVisibility = Visibility.Visible;
        });

        public ICommand StartOverButtonClick => new RelayCommand(o =>
        {
            if (StartButtonContent == "Pause")
            {
                StartButtonContent = "Continue";
                StartIcon = Symbol.Play;
                _dispatcherTimer.Stop();
                TimerVisibility = Visibility.Collapsed;

            }
            else
            {
                TimerVisibility = Visibility.Collapsed;
            }

            IsConfirmingReset = true; // Show reset confirmation message

        });

        public ICommand ConfirmResetCommand => new RelayCommand(o =>
        {
            StartButtonContent = "Start";
            StartIcon = Symbol.Play;
            StartOverButtonVisibility = Visibility.Collapsed;
            _dispatcherTimer.Stop();
            _timeLeft = TimeSpan.FromHours(1000);
            UpdateDisplayTime();
            TimerVisibility = Visibility.Visible;


            // Hide the reset confirmation message
            IsConfirmingReset = false;

        });

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
