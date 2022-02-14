using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindTheWay
{
    public enum SquareType
    {
        Normal,
        Obstacle,
        StartPoint,
        EndPoint
    }

    public class GridSquare: IComparable<GridSquare>
    {
        public int x;
        public int y;
        public SquareType type;
        public bool visited = false;
        public int tentativeDistance = int.MaxValue;
        public GridSquare previous = null;

        /// <summary>
        /// Create a new grid square
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="type">TYpe of square (defaults to normal, which isn't an obstacle or start or end point)</param>
        public GridSquare(int x, int y, SquareType type = SquareType.Normal)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }

        public int CompareTo(GridSquare other)
        {
            return tentativeDistance - other.tentativeDistance;
        }
    }
}
