using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindTheWay
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        GridSquare[,] grid;
        Point gridSize = new Point();

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            
            lblStatus.Text = "Generating grid";
            tabControl1.SelectTab(1);

            // set defaults
            int x = 10;
            int y = 10;

            if(
                int.TryParse(txtXSize.Text, out x) &&
                int.TryParse(txtYSize.Text, out y))
            {
                lblStatus.Text = "Drawing grid";
            } else
            {
                lblStatus.Text = "Invalid text: using default sizes";
            }

            grid = new GridSquare[x, y];
            gridSize.X = x;
            gridSize.Y = y;
            for (int xPos = 0; xPos < x; xPos++)
            {
                for (int yPos = 0; yPos < y; yPos++)
                {
                    GridSquare square = new GridSquare(xPos, yPos);
                    grid[xPos, yPos] = square;
                }
            }
            

            DrawGrid(x, y);
        }

        
        public void DrawGrid(int xSize, int ySize, GridSquare destination = null)
        {
            Bitmap i = new Bitmap(panelVis.Width, panelVis.Height);
            Graphics g = Graphics.FromImage(i);
            int w = panelVis.Width / xSize;
            int h = panelVis.Height / ySize;
            Pen pen = new Pen(Color.Black);
            SolidBrush brushNormal = new SolidBrush(Color.White);
            SolidBrush brushObstacle = new SolidBrush(Color.Red);
            SolidBrush brushStart = new SolidBrush(Color.Green);
            SolidBrush brushEnd = new SolidBrush(Color.Black);

            for (int x = 0; x < xSize; x++)
            {
                for(int y = 0; y < ySize; y++)
                {
                    GridSquare square = grid[x, y];
                    SolidBrush brush = brushNormal;
                    
                    // draw in red for an obstacle
                    if(square.type == SquareType.Obstacle)
                    {
                        brush = brushObstacle;
                    }

                    // change colour for the start
                    if (square.type == SquareType.StartPoint)
                    {
                        brush = brushStart;
                    }

                    // change colour for the end
                    if (square.type == SquareType.EndPoint)
                    {
                        brush = brushEnd;
                    }
                    g.FillRectangle(brush, x * w, y * h, w, h);
                    g.DrawRectangle(pen, x * w, y * h, w, h);
                }
            }
            Pen penPath = new Pen(Color.Blue, 5);
            while(destination != null)
            {
                if (destination.previous != null)
                {
                    
                    g.DrawLine(penPath, destination.x * w + w/2, destination.y * h + h/2,
                        destination.previous.x * w + w/2, destination.previous.y * h + h/2);
                    g.FillEllipse(brushStart, destination.x * w + w / 2 - 10, destination.y * h + h / 2 - 10, 20, 20);
                }
                destination = destination.previous;
            }
            panelVis.BackgroundImage = i;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            DrawGrid(gridSize.X, gridSize.Y);
        }

        private void panelVis_MouseMove(object sender, MouseEventArgs e)
        {
            Point gridCoordinates = ScreenToGrid(e.X, e.Y);
            lblStatus.Text = $"Mouse moved: ({e.X}, {e.Y}) which is grid coordinates: ({gridCoordinates.X},{gridCoordinates.Y})";
        }

        public Point ScreenToGrid(int screenX, int screenY)
        {
            
            Point p = new Point();
            p.X = screenX * gridSize.X / panelVis.Width;
            p.Y = screenY * gridSize.Y / panelVis.Height;
            return p;
        }

        private void panelVis_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = ScreenToGrid(e.X, e.Y);

            // click once for an obstacle, twice for start point, three times for end point

            switch(grid[p.X, p.Y].type)
            {
                case SquareType.Normal:
                    grid[p.X, p.Y].type = SquareType.Obstacle;
                    break;
                case SquareType.Obstacle:
                    grid[p.X, p.Y].type = SquareType.StartPoint;
                    break;
                case SquareType.StartPoint:
                    grid[p.X, p.Y].type = SquareType.EndPoint;
                    break;
                case SquareType.EndPoint:
                    grid[p.X, p.Y].type = SquareType.Normal;
                    break;
            }
            DrawGrid(gridSize.X, gridSize.Y);
        }

        private void btnFindPath_ButtonClick(object sender, EventArgs e)
        {
            // check if there's only one start point
            int startCount = 0;
            int endCount = 0;
            for (int x = 0; x < gridSize.X; x++)
            {
                for (int y = 0; y < gridSize.Y; y++) {
                    if (grid[x, y].type == SquareType.StartPoint)
                    {
                        startCount++;
                    }
                    if (grid[x, y].type == SquareType.EndPoint)
                    {
                        endCount++;
                    }
                }
            }

            if(startCount != 1)
            {
                lblStatus.Text = "You must have exactly one start point";
                return;
            }

            // check if there's only one end point
            if (endCount != 1)
            {
                lblStatus.Text = "You must have exactly one end point";
                return;
            }

            lblStatus.Text = "Finding shortest path";

            // run Dijkstra's algorithm

            // https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
            
            // Mark all nodes unvisited. Create a set of all the unvisited nodes called the unvisited set.
            List<GridSquare> unvisited = new List<GridSquare>();
            foreach(GridSquare s in grid)
            {
                s.visited = false;
                if(s.type != SquareType.Obstacle)
                {
                    unvisited.Add(s);
                } 
                    
            }

            /* Assign to every node a tentative distance value: 
             * set it to zero for our initial node and to infinity for all other nodes. 
             * The tentative distance of a node v is the length of the shortest path discovered so far 
             * between the node v and the starting node. 
             * Since initially no path is known to any other vertex than the source itself 
             * (which is a path of length zero), 
             * all other tentative distances are initially set to infinity. 
             * Set the initial node as current.
             */

            GridSquare current = null;
            GridSquare destination = null;
            foreach(GridSquare s in unvisited)
            {
                if(s.type == SquareType.StartPoint)
                {
                    s.tentativeDistance = 0;
                    current = s;
                } else
                {
                    s.tentativeDistance = int.MaxValue;
                }
                if(s.type == SquareType.EndPoint)
                {
                    destination = s;
                }
            }

            while (true)
            {
                /*
                 * For the current node, consider all of its unvisited neighbors 
                 * and calculate their tentative distances through the current node. 
                 * Compare the newly calculated tentative distance to the current assigned value and assign the smaller one. 
                 * For example, if the current node A is marked with a distance of 6, 
                 * and the edge connecting it with a neighbor B has length 2, then the distance to B through A will be 6 + 2 = 8. 
                 * If B was previously marked with a distance greater than 8 then change it to 8. 
                 * Otherwise, the current value will be kept.
                 */
                List<GridSquare> unvisitedNeighbors = GetUnvisitedNeighbours(current);
                
                foreach (GridSquare s in unvisitedNeighbors)
                {
                    int tentativeDistance = current.tentativeDistance + 1;
                    if (tentativeDistance < s.tentativeDistance)
                    {
                        s.tentativeDistance = tentativeDistance;
                        s.previous = current;
                    }
                }

                /*
                 * When we are done considering all of the unvisited neighbors of the current node, 
                 * mark the current node as visited and remove it from the unvisited set. 
                 * A visited node will never be checked again.
                 */
                current.visited = true;
                unvisited.Remove(current);

                /*
                 * If the destination node has been marked visited (when planning a route between two specific nodes) 
                 * or if the smallest tentative distance among the nodes in the unvisited set is infinity 
                 * (when planning a complete traversal; occurs when there is no connection between the initial node and remaining unvisited nodes), 
                 * then stop. The algorithm has finished.
                 */
                 if(destination.visited)
                {
                    lblStatus.Text = "Found a path!";
                    DrawGrid(gridSize.X, gridSize.Y, destination);
                    break;
                }

                // sort by tentative distance
                unvisited.Sort();
                if(unvisited[0].tentativeDistance == int.MaxValue)
                {
                    lblStatus.Text = "No path can be found";
                    break;
                }


                /* 
                 * Otherwise, select the unvisited node that is marked with the smallest tentative distance, 
                 * set it as the new current node, and go back to step 3.
                 */
                current = unvisited[0];
            }

        }

        private List<GridSquare> GetUnvisitedNeighbours(GridSquare s)
        {
            List<GridSquare> neighbours = new List<GridSquare>();
            if (s.x > 0 && !grid[s.x - 1, s.y].visited && grid[s.x - 1, s.y].type != SquareType.Obstacle)
                neighbours.Add(grid[s.x - 1, s.y]);
            if (s.x < gridSize.X - 1 && !grid[s.x + 1, s.y].visited && grid[s.x + 1, s.y].type != SquareType.Obstacle)
                neighbours.Add(grid[s.x + 1, s.y]);
            if (s.y > 0 && !grid[s.x, s.y - 1].visited && grid[s.x, s.y - 1].type != SquareType.Obstacle)
                neighbours.Add(grid[s.x, s.y - 1]);
            if (s.y < gridSize.Y - 1 && !grid[s.x, s.y + 1].visited && grid[s.x, s.y + 1].type != SquareType.Obstacle)
                neighbours.Add(grid[s.x, s.y + 1]);
            return neighbours;
        }

        private void addIfUnvisited(List<GridSquare> unvisitedNeighbors, int x, int y)
        {
            if (x >= 0 && x < gridSize.X && y >= 0 && y < gridSize.Y)
            {
                if (!grid[x, y].visited && grid[x, y].type != SquareType.Obstacle)
                {
                    unvisitedNeighbors.Add(grid[x, y]);
                }
            }
        }
    }
}
