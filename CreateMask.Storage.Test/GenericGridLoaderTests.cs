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
            var expectedData = new int[,]
            {
                {160, 7420, 560, 1598},
                {176, 6620, 3664, 1234},
                {2938, 4522, 1367, 3235}
            };

            //When
            var grid = gridLoader.GetFromCsvFile(fullpath, nrOfRows, nrOfColumns);
            var actualData = grid.GetData();

            //Then
            actualData.Should().BeEquivalentTo(expectedData);
        }

        private IGenericGridLoader<T> GetGridLoader<T>()
        {
            return new GenericGridLoader<T>();
        } 
    }
}
