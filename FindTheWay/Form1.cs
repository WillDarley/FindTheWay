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


        public void DrawGrid(int xSize, int ySize)
        {
            Bitmap i = new Bitmap(panelVis.Width, panelVis.Height);
            Graphics g = Graphics.FromImage(i);
            int w = panelVis.Width / xSize;
            int h = panelVis.Height / ySize;
            Pen pen = new Pen(Color.Black);
            SolidBrush brushNormal = new SolidBrush(Color.Blue);
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

            //

            // run Dijkstra's algorithm
        }
    }
}
