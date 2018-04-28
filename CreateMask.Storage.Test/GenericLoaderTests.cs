using System;
using System.Collections.Generic;
using System.IO;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Storage.Test
{
    [TestFixture]
    public class GenericLoaderTests
    {
        [Test, Category(Categories.Unit)]
        public void CanLoadFromFile()
        {
            //Given
            const string measurementsDataFileName = "Measurements.csv";
            var filepath = FileManager.GetFullFilePath(measurementsDataFileName);
            var genericLoader = GetGenericLoader<Measurement>();
            var expectedMeasurements = new List<Measurement>
            {
                new Measurement(160, 7420),
                new Measurement(176, 6620)
            };

            //When
            var actualMeasurements = genericLoader.GetFromCsvFile(filepath);

            //Then
            actualMeasurements.Should().BeEquivalentTo(expectedMeasurements);
        }

        [Test, Category(Categories.Unit)]
        public void ThrowsExceptionIfFileNotFound()
        {
            //Given
            const string expectedExceptionMessage = "File could not be found.";
            var genericLoader = GetGenericLoader<object>();

            //When
            var action = new Action(() =>
            {
                genericLoader.GetFromCsvFile("");
            });

            //Then
            AssertExt.ThrowsException<FileNotFoundException>(action, expectedExceptionMessage);
        }

        private IGenericLoader<T> GetGenericLoader<T>()
        {
            return new GenericLoader<T>();
        }
    }
}
