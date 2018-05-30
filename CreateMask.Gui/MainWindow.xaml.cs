using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CreateMask.Containers;
using CreateMask.Contracts.Enums;
using CreateMask.Contracts.Helpers;
using CreateMask.Contracts.Interfaces;
using CreateMask.Gui.Annotations;
using CreateMask.Gui.Components;
using CreateMask.Gui.Controls;
using CreateMask.Main;
using Ninject;

namespace CreateMask.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly Main.Main _main;
        private bool _exposureTimeRecalculated;
        private int ? _recalculatedExposureTime;
        private string _fileFilter;

        public bool ExposureTimeRecalculated
        {
            get { return _exposureTimeRecalculated; }
            set
            {
                _exposureTimeRecalculated = value;
                OnPropertyChanged();
            }
        }

        public int? RecalculatedExposureTime
        {
            get { return _recalculatedExposureTime; }
            set
            {
                _recalculatedExposureTime = value;
                OnPropertyChanged();
            }
        }

        public ApplicationArguments Arguments { get; set; }

        public CheckForUpdate CheckForUpdateComponent { get; private set; }

        public IEnumerable<string> SupportedFileTypes => _main.SupportedFileTypes;

        public string FileFilter
        {
            get
            {
                if (_fileFilter != null) return _fileFilter;
                foreach (var supportedFileType in SupportedFileTypes)
                {
                    _fileFilter += $"{supportedFileType}|*.{supportedFileType}|";
                }
                _fileFilter = _fileFilter.Substring(0, _fileFilter.Length - 1);
                return _fileFilter;
            }
        }
        
        public string Version => "v" +Assembly.GetExecutingAssembly().GetName().Version;

        public MainWindow()
        {
            var kernel = KernelConstructor.GetKernel();
            _main = kernel.Get<Main.Main>();
            _main.Output += Main_Output;
            _main.ExposureTimeCalculated += _main_ExposureTimeCalculated;

            Arguments = new ApplicationArguments
            {
                MeasurementsNrOfRows = 7,
                MeasurementsNrOfColumns = 12,
                High = byte.MaxValue,
                Low = 175,
                LcdWidth = 2560,
                LcdHeight = 1440,
                DesiredResistance = 8820,
                FileType = ImageFileType.Png.ToString(),
                OriginalExposureTime = 8000
            };

            InitializeComponent();

            var releaseManager = kernel.Get<IReleaseManager>();
            CheckForUpdateComponent = new CheckForUpdate(releaseManager, this);
            CheckForUpdateComponent.PropertyChanged += CheckForUpdateComponent_PropertyChanged;
      
            Loaded += (sender, args) => CheckForUpdateComponent.Check();
        }

        private void _main_ExposureTimeCalculated(object sender, int recalculatedExposureTime)
        {
            RecalculatedExposureTime = recalculatedExposureTime;
            ExposureTimeRecalculated = true;
        }

        private void CheckForUpdateComponent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CheckForUpdateComponent));
        }

        private void CmbFileTypeOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (string.IsNullOrWhiteSpace(sfOutputPath?.SelectedFilePath)) return;
            sfOutputPath.SelectedFilePath = Path.ChangeExtension(sfOutputPath.SelectedFilePath, (string) cmbFileType.SelectedItem);
        }

        private void BtnCreateMask_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                tbxOutput.Clear();
                ExposureTimeRecalculated = false;
                _main.CreateMask(Arguments);
            }
            catch (Exception exception)
            {
                Output(exception.Message);
                Output(exception.StackTrace);
            }
        }

        private void Main_Output(object sender, string e)
        {
            Output(e);
        }

        private void Output(string output)
        {
            tbxOutput.AppendText(output + Environment.NewLine);
        }

        private void SelectFile_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectFile.SelectedFilePath)) return; 

            var selectFile = (SelectFile) sender;
            var extension = Path.GetExtension(selectFile.SelectedFilePath);
            extension = ImageFileTypeHelper.FromString(extension).ToString();
            cmbFileType.SelectedItem = extension;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
