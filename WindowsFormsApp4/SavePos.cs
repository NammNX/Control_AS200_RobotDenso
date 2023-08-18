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

           
            if (int.TryParse(Pos.Substring(1), out int posIndex) )
            {
                int arrayIndex = posIndex - 10;

                X[arrayIndex] = x;
                Y[arrayIndex] = y;
                Z[arrayIndex] = z;
                RX[arrayIndex] = rx;
                RY[arrayIndex] = ry;
                RZ[arrayIndex] = rz;
                FIG[arrayIndex] = fig;

                var labelText = $"{X[arrayIndex]},{Y[arrayIndex]},{Z[arrayIndex]},{RX[arrayIndex]},{RY[arrayIndex]},{RZ[arrayIndex]},{FIG[arrayIndex]}";
                Controls.Find($"lbP{posIndex}", true)[0].Text = labelText;
                await robotController.SendCommand(Pos);
                await Task.Delay(30);
                await robotController.SendCommand(labelText);
;            }    
       
        }
        private async void MoveToPosition_Click(object sender, EventArgs e, int index)
        {
            Button button = (Button)sender;
            index = int.Parse(button.Tag.ToString());

            if (X[index] == null)
            {
                MessageBox.Show("Chưa gán giá trị P");
                return;
            }

            await robotController.MoveRobot(X[index],Y[index],Z[index],RX[index],RY[index],RZ[index],FIG[index]);
            x = X[index];
            y = Y[index];
            z = Z[index];
            rx = RX[index];
            ry = RY[index];
            rz = RZ[index];
            fig = FIG[index];
            UpdateUIComponents();
        }

        private void RegisterSaveButton()
        {
            for (int i = 10; i <= 19; i++)
            {
                Button button = Controls.Find($"btnMoveP{i}", true).FirstOrDefault() as Button;
                if (button != null)
                {
                    button.Tag = i - 10;
                    button.Click += (sender, e) => MoveToPosition_Click(sender, e, i - 10);
                }
            }
            //btnMoveP10.Tag = 0;
            //btnMoveP11.Tag = 1;
            //btnMoveP12.Tag = 2;
            //btnMoveP13.Tag = 3;
            //btnMoveP14.Tag = 4;
            //btnMoveP15.Tag = 5;
            //btnMoveP16.Tag = 6;
            //btnMoveP17.Tag = 7;
            //btnMoveP18.Tag = 8;
            //btnMoveP19.Tag = 9;

            //btnMoveP10.Click += MoveToPosition_Click;
            //btnMoveP11.Click += MoveToPosition_Click;
            //btnMoveP12.Click += MoveToPosition_Click;
            //btnMoveP13.Click += MoveToPosition_Click;
            //btnMoveP14.Click += MoveToPosition_Click;
            //btnMoveP15.Click += MoveToPosition_Click;
            //btnMoveP16.Click += MoveToPosition_Click;
            //btnMoveP17.Click += MoveToPosition_Click;
            //btnMoveP18.Click += MoveToPosition_Click;
            //btnMoveP19.Click += MoveToPosition_Click;
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
