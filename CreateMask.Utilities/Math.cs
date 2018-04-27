using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CreateMask.Utilities
{
    public static class Math
    {
        public static bool IsEven(this int number)
        {
            return number % 2 == 0;
        }

        public static IEnumerable<T> GetCenterItems<T>(this T[,] items)
        {
            if(items == null) throw new ArgumentNullException(nameof(items), "An initialized 2D array should have been provided, but null was instead.");

            var nrOfRows = items.GetLength(0);
            var nrOfColumns = items.GetLength(1);
            if (nrOfRows == 0)
                throw new ArgumentException("No items where present in the given 2D array.", nameof(items));

            var nrOfRowsHalved = (nrOfRows - 1) / 2.0;
            var nrOfColumnsHalved = (nrOfColumns - 1) / 2.0;
            var fRow = (int)System.Math.Floor(nrOfRowsHalved);
            var lRow = (int)System.Math.Ceiling(nrOfRowsHalved);
            var fCol = (int)System.Math.Floor(nrOfColumnsHalved);
            var lCol = (int)System.Math.Ceiling(nrOfColumnsHalved);

            var cells = new List<Tuple<int, int>> {
                new Tuple<int, int>(fRow, fCol),
                new Tuple<int, int>(fRow, lCol),
                new Tuple<int, int>(lRow, fCol),
                new Tuple<int, int>(lRow, lCol)
            };
            var resultCells = cells.Distinct();
            return resultCells.Select(t => items[t.Item1, t.Item2]);
        }
    }
}
