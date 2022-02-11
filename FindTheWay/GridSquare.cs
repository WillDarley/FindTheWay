using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindTheWay
{
    class GridSquare
    {
        public int x;
        public int y;
        public bool obstacle;
        public bool startPoint;
        public bool endPoint;

        /// <summary>
        /// Create a new grid square
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="obstacle">True if this grid square is an obstacle</param>
        /// <param name="startPoint">True if this is the start of the route</param>
        /// <param name="endPoint">True if this is the end of the route</param>
        public GridSquare(int x, int y, bool obstacle = false, bool startPoint = false, bool endPoint = false)
        {
            this.x = x;
            this.y = y;
            this.obstacle = obstacle;
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }
    }
}
