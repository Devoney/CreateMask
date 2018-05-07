using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using CreateMask.Gui.Annotations;

namespace CreateMask.Gui.Components
{
    public class CheckForUpdate : INotifyPropertyChanged
    {
        private readonly IReleaseManager _releaseManager;
        private readonly Window _window;
        private bool _isChecking;
        private bool _updateAvailable;
        private ReleaseInfo _latestVersion;

        public bool IsChecking
        {
            get { return _isChecking; }
            private set
            {
                _isChecking = value;
                OnPropertyChanged();
            }
        }

        public bool UpdateAvailable
        {
            get { return _updateAvailable; }
            set
            {
                _updateAvailable = value;
                OnPropertyChanged();
            }
        }

        public ReleaseInfo LatestVersion
        {
            get { return _latestVersion; }
            set
            {
                _latestVersion = value;
                OnPropertyChanged();
            }
        }

        public CheckForUpdate(IReleaseManager releaseManager, Window window)
        {
            _releaseManager = releaseManager;
            _window = window;
        }

        public async void Check()
        {
            IsChecking = true;
            var args = new CheckForReleaseArgs
            {
                CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version,
                OnNewReleaseCallBack = AskToVisiteTheDownloadPage,
                Owner = "Devoney", // TODO: Should be defined somewhere else
                Repository = "CreateMask" // TODO: Should be defined somewhere else
            };
            await _releaseManager.CheckForNewReleaseAsync(args);
            IsChecking = false;
        }

        private void AskToVisiteTheDownloadPage(ReleaseInfo releaseInfo)
        {
            LatestVersion = releaseInfo;
            IsChecking = false;
            _window.Dispatcher.Invoke(() =>
            {
                var messageBoxText = $"A newer version is available ({releaseInfo.Version}).\r\nDo you wish to visit the download page?";
                var answer = MessageBox.Show(_window, messageBoxText, "CreateMask",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (answer != MessageBoxResult.Yes)
                {
                    UpdateAvailable = true;
                    return;
                }

                Process.Start(new ProcessStartInfo(releaseInfo.Uri.AbsoluteUri));
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
