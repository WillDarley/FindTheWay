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

            DrawGrid(x, y);
        }


        public void DrawGrid(int x, int y)
        {

        }
    }
}
