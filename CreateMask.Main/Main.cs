using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using CreateMask.Containers;
using CreateMask.Contracts.Helpers;
using CreateMask.Contracts.Interfaces;
using CreateMask.Utilities;

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

        public IEnumerable<string> SupportedFileTypes => ImageFileTypeHelper.ImageFileTypes;

        public Main(IGenericLoader<Measurement> measurementsLoader, 
                    IMaskIntensityResistanceInterpolatorFactory maskIntensityInterpolatorFactory,
                    IGenericGridLoader<int> measurementGridLoader,
                    IMeasurementGridProcessor measurementGridProcessor,
                    IExposureTimeCalculator exposureTimeCalculator,
                    IOutputWriter outputWriter)
        {
            _measurementsLoader = measurementsLoader;
            _maskIntensityInterpolatorFactory = maskIntensityInterpolatorFactory;
            _measurementGridLoader = measurementGridLoader;
            _measurementGridProcessor = measurementGridProcessor;
            _exposureTimeCalculator = exposureTimeCalculator;
            _outputWriter = outputWriter;
        }

        public void CreateMask(ApplicationArguments arguments)
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
                using (var mask = bitmap.Resize(arguments.LcdWidth, arguments.LcdHeight))
                {
                    mask.Save(arguments.MaskFilePath, imageFormat);
                }
            }
            _outputWriter.MaskSavedTo(arguments.MaskFilePath); 
            
            if (arguments.OriginalExposureTime > 0)
            {
                var exposureTime = _exposureTimeCalculator.CalculateExposure(arguments.High, localMaskIntensityGrid,
                    arguments.OriginalExposureTime);
                _outputWriter.NewAdvisedExposureTime(exposureTime);
            }
        }

        private void OnOutput(string e)
        {
            Output?.Invoke(this, e);
        }
    }
}
