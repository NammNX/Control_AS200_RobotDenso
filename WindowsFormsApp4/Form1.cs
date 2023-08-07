using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


//nxn
namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        private CameraController cameraController;
        private RobotController robotController;
        string x, y, z, rx, ry, rz, fig;
        string j1, j2, j3, j4, j5, j6;

        


        public Form1()
        {
            InitializeComponent();

            this.FormClosing += Form1_FormClosing;
            cameraController = new CameraController();
            cameraController.TextReceivedData = txtReceivedData;
            robotController = new RobotController();
            robotController.TextReceivedData = txtReceivedData;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!cameraController.IsConnected && !robotController.IsConnected) { return; }
            robotController.DisConnectRobot();
            cameraController.DisconnectCamera();
        }

        private bool IsconnectCam = false;
        private bool IsConnectRobot = false;

        private void btnConnectCamera_Click(object sender, EventArgs e)
        {
            if (!IsconnectCam)
            {
                string ipAddress = txtCamIp.Text;
                int port = int.Parse(txtCamPort.Text);
                cameraController.ConnectCamera(ipAddress, port);
                if (cameraController.IsConnected)
                {
                    btnConnectCamera.Text = "Disconnect";
                    btnConnectCamera.BackColor = Color.Red;
                    IsconnectCam = true;
                }
                else
                {
                    return;
                }
            }
            else
            {
                cameraController.DisconnectCamera();
                IsconnectCam = false;
                btnConnectCamera.Text = "Connect";
                btnConnectCamera.BackColor = Color.Green;
            }
        }

        private void btnRobotConnect_Click(object sender, EventArgs e)
        {
            if (!IsConnectRobot)
            {
                string ipAddress = txtRobotIP.Text;
                int port = int.Parse(txtRobotPort.Text);
                robotController.ConnectRobot(ipAddress, port);
                if (robotController.IsConnected)
                {
                    btnRobotConnect.Text = "Disconnect";
                    btnRobotConnect.BackColor = Color.Red;
                    IsConnectRobot = true;
                }
                else
                {
                    return;
                }
            }
            else
            {
                robotController.DisConnectRobot();
                IsConnectRobot = false;
                btnRobotConnect.Text = "Connect";
                btnRobotConnect.BackColor = Color.Green;
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private async void btnSend_Click(object sender, EventArgs e)
        {

            string command = txtCommand.Text.Trim();
            await cameraController.SendCommand(command);
            byte[] buffer = new byte[1024];
            await cameraController.ReceiveData(buffer);

        }

        

        private async Task RunHE(double minX, double maxX, int stepX, double minY, double maxY, int stepY,
                                   double minT, double maxT, int stepT, double z, double Rx, double Ry,
                                   int fig, StringBuilder commandBuilderCam, StringBuilder commandBuilderRobot)
        {
            for (double y = minY; y <= maxY; y += (maxY - minY) / (stepY - 1))
            {
                for (double x = minX; x <= maxX; x += (maxX - minX) / (stepX - 1))
                {
                    string entryRobot = $"{x},{y},{z},{Rx},{Ry},{minT},{fig},";
                    commandBuilderRobot.AppendLine(entryRobot);

                    string entryCam = $"HE,1,1,{x},{y},0,{minT},0,0";
                    commandBuilderCam.AppendLine(entryCam);
                }
            }

            for (double Rz = minT; Rz <= maxT; Rz += (maxT - minT) / (stepT - 1))
            {
                string entryRobot = $"{minX + 10},{minY + 10},{z},{Rx},{Ry},{Rz},{fig},";
                commandBuilderRobot.AppendLine(entryRobot);

                string entryCam = $"HE,1,1,{minX + 10},{minY + 10},0,{Rz},0,0";
                commandBuilderCam.AppendLine(entryCam);
            }
        }

        private void btnClearData_Click(object sender, EventArgs e)
        {
            txtReceivedData.Clear();
            btnHE.Enabled = true;
            btnAutoCalib.Enabled = true;
        }

        private async void btnGetCurPos_Click(object sender, EventArgs e) // Lấy current pos của robot
        {
            await robotController.SendCommand("CRP,");
            await UpdateCurrentPost();

            txtMinX.Text = robotController.x;
            txtMinY.Text = robotController.y;
            txtZ.Text = robotController.z;
            txtRx.Text = robotController.rx;
            txtRy.Text = robotController.ry;
            txtMinT.Text = robotController.rz;
            txtFig.Text = robotController.fig;
           
        }

        private async void btnAutoCalib_Click(object sender, EventArgs e)
        {

            btnAutoCalib.Enabled = false;

            //if (
            //!double.TryParse(txtMinX.Text, out double minX) ||
            //!double.TryParse(txtMinY.Text, out double minY) ||
            //!double.TryParse(txtMinT.Text, out double minT) ||
            //!double.TryParse(txtZ.Text, out double z) ||
            //!double.TryParse(txtRx.Text, out double Rx) ||
            //!double.TryParse(txtRy.Text, out double Ry) ||
            //!double.TryParse(txtFig.Text, out double Fig))
            //{
            //    MessageBox.Show("Nhập số cho các giá trị!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            

            if (!cameraController.IsConnected || !robotController.IsConnected)
            {
                MessageBox.Show("Kết nối Cam và Robot trước khi Calib");
                return;
            }
            await robotController.SendCommand("CRP,");
            await UpdateCurrentPost(); 
            var command = $"ACB,1,1,{x},{y},{z},{rz},{ry},{rx}"; // Lệnh Start HE
            byte[] buffer = new byte[1024];
            await robotController.SendCommand("HE,"); // gửi kí tự HE để robot nhảy vào phần HE trên WC3
            await cameraController.SendCommand(command);
            string receivedDataCam = await cameraController.ReceiveData(buffer);

            if (!receivedDataCam.Contains("ACB,2"))
            {
                MessageBox.Show("Không tìm thấy Feature", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnAutoCalib.Enabled = true;
                return;
            }
            //var index = receivedDataCam.IndexOf(',');
            var NextPosition = receivedDataCam.Substring(6); // Lấy kí tự thứ 6 trở đi (ACB,2,....)
            NextPosition = NextPosition.Replace("\r\n", "");
            string[] part = NextPosition.Split(',');
            var _NextPosition = "";
            if (part.Length >= 6)
            {
                var temp = part[3];
                part[3] = part[5];
                part[5] = temp;
                _NextPosition = string.Join(",", part);
            }
            var sendPostoRobot = $"{_NextPosition},{fig},";
            await robotController.SendCommand(sendPostoRobot);

            await robotController.ReceiveData(buffer);

            var commandNextPosition = "AC,1,1," + NextPosition;
            await cameraController.SendCommand(commandNextPosition);
            var CamResponse = await cameraController.ReceiveData(buffer);

            while (!CamResponse.Contains("AC,1"))
            {
                if (!CamResponse.Contains("AC,2"))
                {
                    MessageBox.Show("Không tìm thấy Feature", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnAutoCalib.Enabled = true;
                    break;
                }

                //var index2 = CamResponse.IndexOf(',');
                var NextPosition2 = CamResponse.Substring(5); // Lấy kí tự thứ 5 trở đi (AC,2,....)
                NextPosition2 = NextPosition2.Replace("\r\n", "");

                string[] parts = NextPosition2.Split(',');
                var _SendPosToRobot = "";

                if (parts.Length >= 6)
                {
                    // Đổi vị trí giữa phần tử Rz và Rx
                    string temp = parts[3];
                    parts[3] = parts[5];
                    parts[5] = temp;

                    _SendPosToRobot = string.Join(",", parts);

                }

                var SendPosToRobot = $"{_SendPosToRobot},{fig},";

                await robotController.SendCommand(SendPosToRobot);

                string receivedDataRobot = await robotController.ReceiveData(buffer);
                if (receivedDataRobot.Contains("OK"))
                {

                    var commandNextPosition2 = "AC,1,1," + NextPosition2;
                    await cameraController.SendCommand(commandNextPosition2);
                    CamResponse = await cameraController.ReceiveData(buffer);
                    await Task.Delay(500);
                }

            }

            btnAutoCalib.Enabled = true;
            MessageBox.Show("Calib Success");
        }

        /*
        private async Task<string> SendCameraCommandAndWaitResponse( string command)
        {
            byte[] buffer = new byte[1024];
            await cameraController.SendCommand(command);
            return await cameraController.ReceiveData(buffer);
        }

        private async Task<string> SendRobotCommandAndWaitResponse(string command)
        {
            byte[] buffer = new byte[1024];
            await robotController.SendCommand(command);
            return await robotController.ReceiveData(buffer);
        }

        private string SwapPositionElements(string position)
        {
            string[] parts = position.Split(',');
            if (parts.Length >= 6)
            {
                string temp = parts[3];
                parts[3] = parts[5];
                parts[5] = temp;
                return string.Join(",", parts);
            }
            return position;
        }

        private async Task<bool> PerformAutoCalibration(  double minX, double minY, double minT, double z, double Rx, double Ry, double Fig)
        {
            var command = $"ACB,1,1,{minX},{minY},{z},{minT},{Ry},{Rx}";
            byte[] buffer = new byte[1024];

            if (!cameraController.IsConnected || !robotController.IsConnected)
            {
                MessageBox.Show("Kết nối Cam và Robot trước khi Calib");
                return false;
            }

            await robotController.SendCommand("HE,");
            string receivedDataCam = await SendCameraCommandAndWaitResponse(command);

            if (!receivedDataCam.Contains("ACB,2"))
            {
                MessageBox.Show("Không tìm thấy Feature", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string NextPosition = receivedDataCam.Substring(6).Replace("\r\n", "");
            NextPosition = SwapPositionElements(NextPosition);
            string sendPostoRobot = $"{NextPosition},{Fig},";
            await robotController.SendCommand(sendPostoRobot);
            await robotController.ReceiveData(buffer);

            string commandNextPosition = "AC,1,1," + NextPosition;
            await SendCameraCommandAndWaitResponse( commandNextPosition);
            string CamResponse = await cameraController.ReceiveData(buffer);

            while (!CamResponse.Contains("AC,1"))
            {
                if (!CamResponse.Contains("AC,2"))
                {
                    MessageBox.Show("Không tìm thấy Feature", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string NextPosition2 = CamResponse.Substring(5).Replace("\r\n", "");
                NextPosition2 = SwapPositionElements(NextPosition2);

                string SendPosToRobot = $"{NextPosition2},{Fig},";
                await robotController.SendCommand(SendPosToRobot);
                string receivedDataRobot = await robotController.ReceiveData(buffer);
                if (receivedDataRobot.Contains("OK"))
                {
                    string commandNextPosition2 = "AC,1,1," + NextPosition2;
                    await SendCameraCommandAndWaitResponse( commandNextPosition2);
                    CamResponse = await cameraController.ReceiveData(buffer);
                    await Task.Delay(500);
                }
            }

            return true;
        }

        private async void btnAutoCalib_Click(object sender, EventArgs e)
        {
            btnAutoCalib.Enabled = false;

            if (
                !double.TryParse(txtMinX.Text, out double minX) ||
                !double.TryParse(txtMinY.Text, out double minY) ||
                !double.TryParse(txtMinT.Text, out double minT) ||
                !double.TryParse(txtZ.Text, out double z) ||
                !double.TryParse(txtRx.Text, out double Rx) ||
                !double.TryParse(txtRy.Text, out double Ry) ||
                !double.TryParse(txtFig.Text, out double Fig))
            {
                MessageBox.Show("Nhập số cho các giá trị!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnAutoCalib.Enabled = true;
                return;
            }

            bool success = await PerformAutoCalibration(minX, minY, minT, z, Rx, Ry, Fig);

            if (success)
            {
                MessageBox.Show("Calib Success");
            }

            btnAutoCalib.Enabled = true;
        }

        */

        private void UpdateUIComponents()
        {
            txtX.Text = x;
            txtY.Text = y;
            txtZ2.Text = z;
            txtRx2.Text = rx;
            txtRy2.Text = ry;
            txtRz.Text = rz;
            txtFig2.Text = fig;

        }
        private void UpdateUIComponentsJoint()
        {

            txtJ1.Text = j1;
            txtJ2.Text = j2;
            txtJ3.Text = j3;
            txtJ4.Text = j4;
            txtJ5.Text = j5;
            txtJ6.Text = j6;

        }
        private async Task UpdateCurrentPost()
        {
           
            await robotController.GetRobotCurrentPosition();
            x = robotController.x;
            y = robotController.y;
            z = robotController.z;
            rx = robotController.rx;
            ry = robotController.ry;
            rz = robotController.rz;
            fig = robotController.fig;

           
        }

        private async Task UpdateCurrentJoint()
        {
            await robotController.GetRobotCurrentJoint();
            j1 = robotController.j1;
            j2 = robotController.j2;
            j3 = robotController.j3;
            j4 = robotController.j4;
            j5 = robotController.j5;
            j6 = robotController.j6;
            UpdateUIComponentsJoint();
        }


        private async void btnGetCurPos2_Click(object sender, EventArgs e)
        {
            await robotController.SendCommand("CRP,");
            await UpdateCurrentPost();
            UpdateUIComponents();
        }

        private async void btnMoveRobot_Click(object sender, EventArgs e)
        {

            await robotController.MoveRobot(txtX.Text, txtY.Text, txtZ2.Text, txtRx2.Text, txtRy2.Text, txtRz.Text, txtFig2.Text);
            await Task.Delay(200);
            await robotController.SendCommand("End,");

        }
    }

}




