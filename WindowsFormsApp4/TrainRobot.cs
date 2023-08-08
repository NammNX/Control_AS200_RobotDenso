using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4;
using System.Drawing;

namespace WindowsFormsApp4
{
    public partial class Form1
    {

        #region Train Robot
        private async Task TrainVisionPoint()
        {

            double.TryParse(txtX.Text, out double x);
            double.TryParse(txtY.Text, out double y);
            double.TryParse(txtZ2.Text, out double z);
            double.TryParse(txtRx2.Text, out double rx);
            double.TryParse(txtRy2.Text, out double ry);
            double.TryParse(txtRz.Text, out double rz);

            var Feature = cbFeature.Text;

            var command = $"TT,{Feature},{x},{y},{z},{rz},{ry},{rx}";

            await cameraController.SendCommand(command);
        }

        private async Task TrainRobotPickPlace()
        {
            double.TryParse(txtX.Text, out double x);
            double.TryParse(txtY.Text, out double y);
            double.TryParse(txtZ2.Text, out double z);
            double.TryParse(txtRx2.Text, out double rx);
            double.TryParse(txtRy2.Text, out double ry);
            double.TryParse(txtRz.Text, out double rz);
            string Feature = cbFeature.Text;

            string command = $"TTR,{Feature},{x},{y},{z},{rz},{ry},{rx}";
            await cameraController.SendCommand(command);
        }

        private async void btnTrainVisionPoint_Click(object sender, EventArgs e)
        {
            if (!cameraController.IsConnected)
            {
                MessageBox.Show("Camera chưa kết nối");
                return;
            }
            if (cbFeature.SelectedIndex == -1)
            {
                MessageBox.Show("Chọn Feature trước khi Train", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            btnTrainVisionPoint.Enabled = false;
            await TrainVisionPoint();

            byte[] buffer = new byte[1024];
            string DataReceive = await cameraController.ReceiveData();
            if (DataReceive.Contains("TT,1"))
            {

                MessageBox.Show("Train Success", "Thông báo", MessageBoxButtons.OK);
            }
            else
            {

                MessageBox.Show("Train Fail", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnTrainVisionPoint.Enabled = true;
        }

        private async void btnTrainPickPlace_Click(object sender, EventArgs e)
        {

            if (!cameraController.IsConnected)
            {
                MessageBox.Show("Camera chưa kết nối");
                return;
            }

            if (cbFeature.SelectedIndex == -1)
            {
                MessageBox.Show("Chọn Feature trước khi Train", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnTrainPickPlace.Enabled = false;
            await TrainRobotPickPlace();

            byte[] buffer = new byte[1024];
            string DataReceive = await cameraController.ReceiveData();
            if (DataReceive.Contains("TTR,1"))
            {
                MessageBox.Show("Train Success", "Thông báo", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Train Fail", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnTrainPickPlace.Enabled = true;

        }

        #endregion
        private bool isToolOn = false;

        private async void btnOnOffTool_Click(object sender, EventArgs e)
        {
            if (!robotController.IsConnected)
            {
                MessageBox.Show("Robot chưa kết nối");
                return;
            }

            if (!isToolOn)
            {
                await robotController.SendCommand("ON,");
                isToolOn = true;
                btnOnOffTool.Text = "OFF Tool";
                btnOnOffTool.BackColor = Color.Red;
            }
            else
            {
                await robotController.SendCommand("OFF,");
                isToolOn = false;
                btnOnOffTool.Text = "ON Tool";
                btnOnOffTool.BackColor = Color.Green;

            }
        }
    }
}
