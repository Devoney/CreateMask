using System;
using System.Drawing;

namespace CreateMask.Contracts.Interfaces
{
    public interface IOutputWriter
    {
        void LoadingFile(string filePath);

        void SetOutputMethod(Action<string> outputMethod);
        void ConstructionLdPolynomialCurveFit();

        void ConstructingGridOfLowHighMeasurements();

        void CreatingGridOfLocalMaskIntensities();

        void ConvertingLocalMaskIntensitiesToBitmap();

        void ResizingBitmap(Bitmap bitmap, int width, int height);
    
        void MaskSavedTo(string filePath);

        void NewAdvisedExposureTime(int exposureTime);
    }
}
