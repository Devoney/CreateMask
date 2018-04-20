using System;
using System.Collections.Generic;
using System.Linq;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using MathNet.Numerics;

namespace CreateMask.Workers
{
    public class MaskIntensityResistanceInterpolator : IMaskIntensityResistanceInterpolator
    {
        private const int PolynomialOrder = 6;
        private bool _measurementsLoaded = false;
        private Func<double, double> _fitMethodMaskIntensity = null;
        private Func<double, double> _fitMethodResistance = null;

        private MinMax<int> _resistance = null; 
         
        public void LoadMeasurements(List<Measurement> measurements)
        {
            var resistanceData = measurements.Select(m => (double)m.Resistance).ToArray();
            _resistance = new MinMax<int>((int)resistanceData.Min(), (int)resistanceData.Max());
            var intensityData = measurements.Select(m => (double)m.MaskIntensity).ToArray();

            _fitMethodMaskIntensity = CreateFitMethod(resistanceData, intensityData);
            _fitMethodResistance = CreateFitMethod(intensityData, resistanceData);

            _measurementsLoaded = true;
        }

        public byte GetMaskIntensity(int resistance)
        {
            if (!_measurementsLoaded)
            {
                throw new InvalidOperationException("No measurements has been loaded yet, can not interpolate mask intensity.");
            }
            var maskIntensity = Math.Round(_fitMethodMaskIntensity(resistance));
            if (maskIntensity > byte.MaxValue) return byte.MaxValue;
            return (byte) maskIntensity;
        }

        public int GetResistance(byte maskIntensity)
        {
            if (!_measurementsLoaded)
            {
                throw new InvalidOperationException("No measurements has been loaded yet, can not interpolate resistance value.");
            }
            return (int)Math.Round(_fitMethodResistance(maskIntensity));
        }

        public byte GetLocalMaskIntensity(int desiredLocalResistance, MinMax<Measurement> localMeasurement)
        {
            #region Sanity check

            if (!_measurementsLoaded)
            {
                throw new InvalidOperationException("No measurements has been loaded yet.");
            }
            if (desiredLocalResistance < _resistance.Min || desiredLocalResistance > _resistance.Max)
            {
                throw new InvalidOperationException($"Cannot calculate mask intensity because given desired resistance value of '{desiredLocalResistance}'" +
                                                    $" is not within range (Min:'{_resistance.Min}' Max:'{_resistance.Max}') " +
                                                    $"of loaded measurements.");
            }
            if (desiredLocalResistance < localMeasurement.Max.Resistance ||
                desiredLocalResistance > localMeasurement.Min.Resistance)
            {
                throw new InvalidOperationException($"Cannot calculate mask intensity because given desired resistance value of '{desiredLocalResistance}'" +
                                                    $" is not within range (Min:'{localMeasurement.Max.Resistance}' Max:'{localMeasurement.Min.Resistance}') " +
                                                    $"of local measurements.");
            }
            #endregion

            // See explanation of calcution within LocalResistanceCalculationMethod.svg
#pragma warning disable 1587
            /// <image url="$(SolutionDir)\misc\LocalResistanceCalculationMethod.png"/>
#pragma warning restore 1587
            var standardMinMaskIntensity = _fitMethodMaskIntensity(localMeasurement.Min.Resistance); // 1
            var standardMaxMaskIntensity = _fitMethodMaskIntensity(localMeasurement.Max.Resistance); // 2
            var standardIntensityRange = standardMaxMaskIntensity - standardMinMaskIntensity; // 3

            var localIntensityRange = localMeasurement.Max.MaskIntensity - localMeasurement.Min.MaskIntensity; //4
            var factor = localIntensityRange/standardIntensityRange; // 5

            var standardMaskIntensity = _fitMethodMaskIntensity(desiredLocalResistance); // 6
            var standardDifference = standardMaskIntensity - standardMinMaskIntensity; // 7
            var localDifference = standardDifference*factor; //8
            var localMaskIntensity = localMeasurement.Min.MaskIntensity + localDifference; // 9

            var roundedValue = Math.Round(localMaskIntensity);
            var value = Math.Min(byte.MaxValue, roundedValue);
            return (byte)value;
        }

        private Func<double, double> CreateFitMethod(double[] xData, double[] yData)
        {
            var p = Fit.Polynomial(xData, yData, PolynomialOrder);

            var func = new Func<double, double>(x =>
            {
                var endResult = p[0];
                for (var i = 1; i <= PolynomialOrder; i++)
                {
                    endResult += (p[i]*Math.Pow(x, i));
                }
                return endResult;
            });

            return func;
        }
    }
}
