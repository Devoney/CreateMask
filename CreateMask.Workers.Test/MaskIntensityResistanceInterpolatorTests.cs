using System;
using System.Collections.Generic;
using System.Linq;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using CreateMask.Workers.Factories;
using FluentAssertions;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Workers.Test
{
    [TestFixture]
    public class MaskIntensityResistanceInterpolatorTests
    {
        [Test, Category(Categories.Unit)]
        public void ReturnsCorrectFitMethodForMaskIntensity()
        {
            //Given
            var measurements = GetDefeaultSetOfMeasurements();
            var expectedIntensities = measurements.Select(m => m.MaskIntensity).ToList();
            var maskIntensityInterpolator = GetMaskIntensityInterpolator(measurements);

            //When
            var actualIntensities = measurements.Select(m => maskIntensityInterpolator.GetMaskIntensity(m.Resistance)).ToList();

            //Then
            actualIntensities.Should().BeEquivalentTo(expectedIntensities);
        }

        [Test, Category(Categories.Unit)]
        public void ReturnsCorrectFitMethodForResistance()
        {
            //Given
            var measurements = GetDefeaultSetOfMeasurements();
            var expectedResistance = measurements.Select(m => m.Resistance).ToList();
            var maskIntensityInterpolator = GetMaskIntensityInterpolator(measurements);

            //When
            var actualResistance = measurements.Select(m => maskIntensityInterpolator.GetResistance(m.MaskIntensity)).ToList();

            //Then
            actualResistance.Should().BeEquivalentTo(expectedResistance);
        }

        [Test, Category(Categories.Unit)]
        public void LocalMaskIntensityCalculationYieldsCorrectValue()
        {
            //Given
            var measurements = GetDefeaultSetOfMeasurements();
            var maskIntensityInterpolator = GetMaskIntensityInterpolator(measurements);
            var relativePointMinMax = new MinMax<Measurement>
            {
                Min = new Measurement(192, 6867),
                Max = new Measurement(byte.MaxValue, 4623)
            };
            const int desiredLocalResistance = 5000;
            const byte expectedRelativeMaskIntensity = 240;

            //When
            var actualRelativeMaskIntensity = maskIntensityInterpolator.GetLocalMaskIntensity(desiredLocalResistance, relativePointMinMax);

            //Then
            actualRelativeMaskIntensity.Should().Be(expectedRelativeMaskIntensity);
        }

        [Test, Category(Categories.Unit)]
        [TestCase(UseMinOrMax.Min)]
        [TestCase(UseMinOrMax.Max)]
        public void LocalMaskIntensityCalculationThrowsExceptionWhenDesiredResistanceValueIsOutOfRangeOfLoadedMeasurements(UseMinOrMax minOrMax)
        {
            //Given
            var measurements = GetDefeaultSetOfMeasurements();
            var maskIntensityInterpolator = GetMaskIntensityInterpolator(measurements);
            var relativePointMinMax = new MinMax<Measurement>
            {
                Min = new Measurement(192, 6867),
                Max = new Measurement(byte.MaxValue, 4623)
            };
            var minResistance = measurements.Min(m => m.Resistance);
            var maxResistance = measurements.Max(m => m.Resistance);
            int desiredLocalResistance;
            switch (minOrMax)
            {
                case UseMinOrMax.Min:
                    desiredLocalResistance = measurements.Min(m => m.Resistance) - 1;
                    break;
                case UseMinOrMax.Max:
                    desiredLocalResistance = measurements.Max(m => m.Resistance) + 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(minOrMax), minOrMax, null);
            }
            
            var expectedPartialExceptionMessage =
                $"Cannot calculate mask intensity because given desired resistance value of '{desiredLocalResistance}'" +
                $" is not within range (Min:'{minResistance}' Max:'{maxResistance}') " +
                "of loaded measurements.";

            //When
            var action = new Action(() =>
            {
                maskIntensityInterpolator.GetLocalMaskIntensity(desiredLocalResistance, relativePointMinMax);
            });

            //Then
            AssertExt.ThrowsException<InvalidOperationException>(action, expectedPartialExceptionMessage);
        }

        [Test, Category(Categories.Unit)]
        [TestCase(UseMinOrMax.Min)]
        [TestCase(UseMinOrMax.Max)]
        public void LocalMaskIntensityCalculationThrowsExceptionWhenDesiredResistanceValueIsOutOfRangeOfLocalMeasurement(UseMinOrMax minOrMax)
        {
            //Given
            var measurements = GetDefeaultSetOfMeasurements();
            var maskIntensityInterpolator = GetMaskIntensityInterpolator(measurements);
            var relativePointMinMax = new MinMax<Measurement>
            {
                Min = new Measurement(192, 6867),
                Max = new Measurement(byte.MaxValue, 4623)
            };
            int desiredLocalResistance;
            switch (minOrMax)
            {
                case UseMinOrMax.Min:
                    desiredLocalResistance = relativePointMinMax.Min.Resistance + 1;
                    break;
                case UseMinOrMax.Max:
                    desiredLocalResistance = relativePointMinMax.Max.Resistance - 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(minOrMax), minOrMax, null);
            }

            var expectedPartialExceptionMessage =
                $"Cannot calculate mask intensity because given desired resistance value of '{desiredLocalResistance}'" +
                                                    $" is not within range (Min:'{relativePointMinMax.Max.Resistance}' Max:'{relativePointMinMax.Min.Resistance}') " +
                                                    "of local measurements.";

            //When
            var action = new Action(() =>
            {
                maskIntensityInterpolator.GetLocalMaskIntensity(desiredLocalResistance, relativePointMinMax);
            });

            //Then
            AssertExt.ThrowsException<InvalidOperationException>(action, expectedPartialExceptionMessage);
        }

        [Test, Category(Categories.Unit)]
        public void ConstructionThrowsOnNullArgument()
        {
            //Given
            IEnumerable<Measurement> measurements = null;

            //When
            var action = new Action(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                // ReSharper disable once ExpressionIsAlwaysNull
                new MaskIntensityResistanceInterpolator(measurements);
            });

            //Then
            AssertExt.ThrowsException<ArgumentNullException>(action, "");
        }

        private List<Measurement> GetDefeaultSetOfMeasurements()
        {
            return new List<Measurement>
            {
                new Measurement(160,7420),
                new Measurement(176,6620),
                new Measurement(192,5900),
                new Measurement(208,5290),
                new Measurement(224,4860),
                new Measurement(240,4500),
                new Measurement(255,4260)
            };
        } 

        private IMaskIntensityResistanceInterpolator GetMaskIntensityInterpolator(IEnumerable<Measurement> measurements)
        {
            var factory = new MaskIntensityResistanceInterpolatorFactory();
            return factory.Create(measurements);
        }

        public enum UseMinOrMax
        {
            Min,
            Max
        }
    }
}
