using System;
using System.Drawing;
using CreateMask.Contracts.Constants;
using CreateMask.Contracts.Interfaces;

namespace CreateMask.Workers
{
    public class OutputWriter : IOutputWriter
    {
        private Action<string> _outputMethod;
         
        public void LoadingFile(string filePath)
        {
            OnOutput(OutputStrings.LoadingFile, filePath);
        }

        public void SetOutputMethod(Action<string> outputMethod)
        {
            _outputMethod = outputMethod;
        }

        public void ConstructionLdPolynomialCurveFit()
        {
            OnOutput(OutputStrings.ConstructionLdPolynomialCurveFit);
        }

        public void ConstructingGridOfLowHighMeasurements()
        {
            OnOutput(OutputStrings.ConstructingGridOfLowHighMeasurements);
        }

        public void CreatingGridOfLocalMaskIntensities()
        {
            OnOutput(OutputStrings.CreatingGridOfLocalMaskIntensities);
        }

        public void ConvertingLocalMaskIntensitiesToBitmap()
        {
            OnOutput(OutputStrings.ConvertingLocalMaskIntensitiesToBitmap);
        }

        public void ResizingBitmap(Bitmap bitmap, int width, int height)
        {
            OnOutput(OutputStrings.ResizingBitmap, bitmap.Width, bitmap.Height, width, height);
        }

        public void MaskSavedTo(string filePath)
        {
            OnOutput(OutputStrings.MaskSavedTo, filePath);
        }

        public void NewAdvisedExposureTime(int exposureTime)
        {
            OnOutput(OutputStrings.NewAdvisedExposureTime, exposureTime);
        }

        private void OnOutput(string format, params object[] arguments)
        {
            if (_outputMethod == null) return;
            var output = string.Format(format, arguments);
            _outputMethod.Invoke(output);
        }
    }
}
