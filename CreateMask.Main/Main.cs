using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Reflection;
using CreateMask.Containers;
using CreateMask.Contracts.Helpers;
using CreateMask.Contracts.Interfaces;

namespace CreateMask.Main
{
    public class Main
    {
        public event EventHandler<string> Output;

        private readonly IGenericLoader<Measurement> _measurementsLoader;
        private readonly IMaskIntensityResistanceInterpolatorFactory _maskIntensityInterpolatorFactory;
        private readonly IGenericGridLoader<int> _measurementGridLoader;
        private readonly IMeasurementGridProcessor _measurementGridProcessor;
        private readonly IExposureTimeCalculator _exposureTimeCalculator;
        private readonly IOutputWriter _outputWriter;
        private readonly IBitmapProcessor _bitmapProcessor;
        private readonly IErrorReportCreator _errorReportCreator;
        private readonly ErrorReportConfiguration _errorReportConfiguration;

        public IEnumerable<string> SupportedFileTypes => ImageFileTypeHelper.ImageFileTypes;

        public event EventHandler<int> ExposureTimeCalculated;

        public Main(IGenericLoader<Measurement> measurementsLoader,
                    IMaskIntensityResistanceInterpolatorFactory maskIntensityInterpolatorFactory,
                    IGenericGridLoader<int> measurementGridLoader,
                    IMeasurementGridProcessor measurementGridProcessor,
                    IExposureTimeCalculator exposureTimeCalculator,
                    IOutputWriter outputWriter,
                    IBitmapProcessor bitmapProcessor,
                    IErrorReportCreator errorReportCreator,
                    ErrorReportConfiguration errorReportConfiguration,
                    IErrorReportReporter errorReportReporter)
        {
            _measurementsLoader = measurementsLoader;
            _maskIntensityInterpolatorFactory = maskIntensityInterpolatorFactory;
            _measurementGridLoader = measurementGridLoader;
            _measurementGridProcessor = measurementGridProcessor;
            _exposureTimeCalculator = exposureTimeCalculator;
            _outputWriter = outputWriter;
            _bitmapProcessor = bitmapProcessor;
            _errorReportCreator = errorReportCreator;
            _errorReportConfiguration = errorReportConfiguration;
            
            errorReportReporter.Start();
        }

        public void CreateMask(ApplicationArguments arguments)
        {
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));

            try
            {
                var imageFormat = ImageFormat.Png;
                if (!string.IsNullOrEmpty(arguments.FileType))
                {
                    imageFormat = ImageFileTypeHelper.FromString(arguments.FileType).ToImageFormat();
                }

                _outputWriter.SetOutputMethod(OnOutput);

                _outputWriter.LoadingFile(arguments.LdrCalibrationFilePath);
                var ldrCalibrationMeasurements = _measurementsLoader.GetFromCsvFile(arguments.LdrCalibrationFilePath);

                _outputWriter.ConstructionLdPolynomialCurveFit();
                var maskIntensityInterpolator = _maskIntensityInterpolatorFactory.Create(ldrCalibrationMeasurements);

                _outputWriter.LoadingFile(arguments.LcdMeasurementsFilePathHigh);
                var measurementsHigh = _measurementGridLoader.GetFromCsvFile(
                    arguments.LcdMeasurementsFilePathHigh,
                    arguments.MeasurementsNrOfRows,
                    arguments.MeasurementsNrOfColumns);

                _outputWriter.LoadingFile(arguments.LcdMeasurementsFilePathLow);
                var measurementsLow = _measurementGridLoader.GetFromCsvFile(
                    arguments.LcdMeasurementsFilePathLow,
                    arguments.MeasurementsNrOfRows,
                    arguments.MeasurementsNrOfColumns);

                _outputWriter.ConstructingGridOfLowHighMeasurements();
                var minMaxResistanceGrid = _measurementGridProcessor.CreateMinMaxMeasurementGrid(arguments.Low, arguments.High, measurementsLow, measurementsHigh);
                _outputWriter.CreatingGridOfLocalMaskIntensities();
                var localMaskIntensityGrid = _measurementGridProcessor.CreateLocalMaskIntensityGrid(maskIntensityInterpolator, minMaxResistanceGrid, arguments.DesiredResistance);
                _outputWriter.ConvertingLocalMaskIntensitiesToBitmap();
                using (var bitmap = _measurementGridProcessor.CreateBitMap(localMaskIntensityGrid))
                {
                    _outputWriter.ResizingBitmap(bitmap, arguments.LcdWidth, arguments.LcdHeight);
                    using (var mask = _bitmapProcessor.Resize(bitmap, arguments.LcdWidth, arguments.LcdHeight))
                    {
                        _bitmapProcessor.Save(mask, arguments.MaskFilePath, imageFormat);
                        _outputWriter.MaskSavedTo(arguments.MaskFilePath);
                    }
                }

                if (arguments.OriginalExposureTime > 0)
                {
                    var exposureTime = _exposureTimeCalculator.CalculateExposure(arguments.High, localMaskIntensityGrid,
                        arguments.OriginalExposureTime);
                    _outputWriter.NewAdvisedExposureTime(exposureTime);
                    OnExposureTimeCalculated(exposureTime);
                }
            }
            catch (Exception exception)
            {
                CreateErrorReport(exception, arguments);
                throw;
            }
        }

        private void CreateErrorReport(Exception exception, ApplicationArguments arguments)
        {
            try
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                var reportName = DateTime.Now.ToString("yyyyMMddHHmmss");
                _errorReportCreator.CreateReport(version, exception, arguments, _errorReportConfiguration.MainDirectory, reportName);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                //Creating an error erport shouldn't cause further issues.
            }
        }

        private void OnOutput(string e)
        {
            Output?.Invoke(this, e);
        }

        protected virtual void OnExposureTimeCalculated(int exposureTime)
        {
            ExposureTimeCalculated?.Invoke(this, exposureTime);
        }
    }
}
