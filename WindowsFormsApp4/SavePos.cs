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
        
        private string[] X = new string[10];
        private string[] Y = new string[10];
        private string[] Z = new string[10];
        private string[] RX = new string[10];
        private string[] RY = new string[10];
        private string[] RZ = new string[10];
        private string[] FIG = new string[10];


        public async Task SavePos()
        {
            
            var Pos = cbPos.Text;

            int posIndex = -1;
            if (int.TryParse(Pos.Substring(1), out posIndex) )
            {
                int arrayIndex = posIndex - 10;

                X[arrayIndex] = x;
                Y[arrayIndex] = y;
                Z[arrayIndex] = z;
                RX[arrayIndex] = rx;
                RY[arrayIndex] = ry;
                RZ[arrayIndex] = rz;
                FIG[arrayIndex] = fig;

                string labelText = $"{X[arrayIndex]},{Y[arrayIndex]},{Z[arrayIndex]},{RX[arrayIndex]},{RY[arrayIndex]},{RZ[arrayIndex]},{FIG[arrayIndex]},";
                Controls.Find($"lbP{posIndex}", true)[0].Text = labelText;
                await robotController.SendCommand(Pos + ",");
                await Task.Delay(30);
                await robotController.SendCommand(labelText);
;            }    
       
        }
        private async void MoveToPosition_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int index = int.Parse(button.Tag.ToString());

            if (X[index] == null)
            {
                MessageBox.Show("Chưa gán giá trị P");
                return;
            }

            await robotController.MoveRobot(X[index], Y[index], Z[index], RX[index], RY[index], RZ[index], FIG[index]);
            txtX.Text = X[index];
            txtY.Text = Y[index];
            txtZ2.Text = Z[index];
            txtRx2.Text = RX[index];
            txtRy2.Text = RY[index];
            txtRz.Text = RZ[index];
            txtFig2.Text = FIG[index];
        }

        private async void btnSavePos_Click(object sender, EventArgs e)
        {
            if (cbPos.SelectedIndex == -1)
            {
                MessageBox.Show("Chọn P cần lưu");
                return;
            }
            await SavePos();
        }


    }
}
