using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form1
    {
        private string x0, y0, z0, rx0, ry0, rz0, fig0;
        private string x1, y1, z1, rx1, ry1, rz1, fig1;
        private string x2, y2, z2, rx2, ry2, rz2, fig2;
        private string x3, y3, z3, rx3, ry3, rz3, fig3;
        private string x4, y4, z4, rx4, ry4, rz4, fig4;

        public void SavePos()
        {
            var Pos = cbPos.Text;
            switch(Pos)
            {
                case "P0":
                    x0 = robotController.x;
                    y0 = robotController.y;
                    z0 = robotController.z;
                    rx0 = robotController.rx;
                    ry0 = robotController.ry;
                    rz0 = robotController.rz;
                    fig0 = robotController.fig;
                    lbP0.Text = $"{x0},{y0},{z0},{rx0},{ry0},{rz0},{fig0}";
                    break;
                case "P1":
                    x1 = robotController.x;
                    y1 = robotController.y;
                    z1 = robotController.z;
                    rx1 = robotController.rx;
                    ry1 = robotController.ry;
                    rz1 = robotController.rz;
                    fig1 = robotController.fig;
                    lbP1.Text = $"{x1},{y1},{z1},{rx1},{ry1},{rz1},{fig1}";
                    break;
                case "P2":
                    x2 = robotController.x;
                    y2 = robotController.y;
                    z2 = robotController.z;
                    rx2 = robotController.rx;
                    ry2 = robotController.ry;
                    rz2= robotController.rz;
                    fig2 = robotController.fig;
                    lbP2.Text = $"{x2},{y2},{z2},{rx2},{ry2},{rz2},{fig2}";
                    break ;
                case "P3":
                    x3 = robotController.x;
                    y3 = robotController.y;
                    z3 = robotController.z;
                    rx3 = robotController.rx;
                    ry3 = robotController.ry;
                    rz3 = robotController.rz;
                    fig3 = robotController.fig;
                    lbP3.Text = $"{x3},{y3},{z3},{rx3},{ry3},{rz3},{fig3}";
                    break;
                case "P4":
                    x4 = robotController.x;
                    y4 = robotController.y;
                    z4 = robotController.z;
                    rx4 = robotController.rx;
                    ry4 = robotController.ry;
                    rz4 = robotController.rz;
                    fig4 = robotController.fig;
                    lbP4.Text = $"{x4},{y4},{z4},{rx4},{ry4},{rz4},{fig4}";
                    break;
               

            }    
       
        }
        private void btnSavePos_Click(object sender, EventArgs e)
        {
            if(cbPos.SelectedIndex == -1)
            {
                MessageBox.Show("Chọn P cần lưu");
                return;
            }    
            SavePos();
        }
        private async void btnMoveP0_Click(object sender, EventArgs e)
        {
            if(x0==null)
            {
                MessageBox.Show("Chưa gán giá trị P");
                return;
                
            }    
            await robotController.MoveRobot(x0,y0,z0,rx0,ry0,rz0,fig0);
            await UpdateCurrentPos();
            UpdateUIComponents();

        }
        private async void btnMoveP1_Click(object sender, EventArgs e)
        {
            if (x1 == null)
            {
                MessageBox.Show("Chưa gán giá trị P");
                return;

            }
            await robotController.MoveRobot(x1, y1, z1, rx1, ry1, rz1, fig1);
            await UpdateCurrentPos();
            UpdateUIComponents();


        }
        private async void btnMoveP2_Click(object sender, EventArgs e)
        {
            if (x2 == null)
            {
                MessageBox.Show("Chưa gán giá trị P");
                return;
            }
            await robotController.MoveRobot(x2, y2, z2, rx2, ry2, rz2, fig2);
            await UpdateCurrentPos();
            UpdateUIComponents();


        }
        private async void btnMoveP3_Click(object sender, EventArgs e)
        {
            if (x3 == null)
            {
                MessageBox.Show("Chưa gán giá trị P");
                return;
            }
            await robotController.MoveRobot(x3, y3, z3, rx3, ry3, rz3, fig3);
            await UpdateCurrentPos();
            UpdateUIComponents();


        }
        private async void btnMoveP4_Click(object sender, EventArgs e)
        {
            if (x4 == null)
            {
                MessageBox.Show("Chưa gán giá trị P");
                return;
            }
            await robotController.MoveRobot(x4, y4, z4, rx4, ry4, rz4, fig4);
            await UpdateCurrentPos();
            UpdateUIComponents();


        }

    }
}
