using System;

namespace CreateMask.Containers
{
    [Serializable]
    public class Grid<T>
    {
        private T[,] _data;

        public Grid()
        {
            
        }

        public Grid(int rows, int columns)
        {
            SetDimensions(rows, columns);
        }

        public T[,] GetData()
        {
            var data = new T[_data.GetLength(0), _data.GetLength(1)];
            Array.Copy(_data, data, data.Length);
            return data;
        }

        public void SetDimensions(int nrOfRows, int nrOfColumns)
        {
            _data = new T[nrOfRows,nrOfColumns];
        }

        public void Set(T value, int rowIndex, int columnIndex)
        {
            if(_data == null) throw new InvalidOperationException("Dimensions of data set not known, use non-default constructor or SetDimensions method.");
            _data[rowIndex, columnIndex] = value;
        }
    }
}
