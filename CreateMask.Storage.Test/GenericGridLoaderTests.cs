using System;
using System.IO;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Storage.Test
{
    [TestFixture]
    public class GenericGridLoaderTests
    {
        [Test, Category(Categories.Unit)]
        public void CanLoadData()
        {
            //Given
            const int nrOfRows = 3;
            const int nrOfColumns = 4;
            const string testDataFileName = "IntGrid.csv";
            var fullpath = FileManager.GetFullFilePath(testDataFileName);
            var gridLoader = GetGridLoader<int>();
            var expectedData = new [,]
            {
                {160, 7420, 560, 1598},
                {176, 6620, 3664, 1234},
                {2938, 4522, 1367, 3235}
            };

            //When
            var actualData = gridLoader.GetFromCsvFile(fullpath, nrOfRows, nrOfColumns);

            //Then
            actualData.Should().BeEquivalentTo(expectedData);
        }

        [Test, Category(Categories.Unit)]
        public void ThrowsExceptionWhenCsvDoesNotContainEnoughRows()
        {
            //Given
            const string expectedExceptionMessage = "Excepted a row at zero based index 0.";
            var genericGridLoader = GetGridLoader<int>();
            const int rows = 1;
            const int columns = 5;
            var filePath = FileManager.GetFullFilePath("empty.csv");

            //When
            var action = new Action(() =>
            {
                genericGridLoader.GetFromCsvFile(filePath, rows, columns);
            });

            //Then
            AssertExt.ThrowsException<InvalidDataException>(action, expectedExceptionMessage);
        }

        [Test, Category(Categories.Unit)]
        public void ThrowsExceptionWhenDataIsOfUnexpectedType()
        {
            //Given
            var expectedExceptionMessage = $"Expected data of type {typeof(int).FullName} at zero based row:column index 0:0";
            var genericGridLoader = GetGridLoader<int>();
            const int rows = 1;
            const int columns = 5;
            var filePath = FileManager.GetFullFilePath("stringgrid.csv");

            //When
            var action = new Action(() =>
            {
                genericGridLoader.GetFromCsvFile(filePath, rows, columns);
            });

            //Then
            AssertExt.ThrowsException<InvalidDataException>(action, expectedExceptionMessage);
        }

        private IGenericGridLoader<T> GetGridLoader<T>()
        {
            return new GenericGridLoader<T>();
        } 
    }
}
