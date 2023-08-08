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

        private async void btnSend_Click(object sender, EventArgs e)
        {

            string command = txtCommand.Text.Trim();
            await cameraController.SendCommand(command);
            byte[] buffer = new byte[1024];
            await cameraController.ReceiveData();

        }



        //private async Task RunHE(double minX, double maxX, int stepX, double minY, double maxY, int stepY,
        //                           double minT, double maxT, int stepT, double z, double Rx, double Ry,
        //                           int fig, StringBuilder commandBuilderCam, StringBuilder commandBuilderRobot)
        //{
        //    for (double y = minY; y <= maxY; y += (maxY - minY) / (stepY - 1))
        //    {
        //        for (double x = minX; x <= maxX; x += (maxX - minX) / (stepX - 1))
        //        {
        //            string entryRobot = $"{x},{y},{z},{Rx},{Ry},{minT},{fig},";
        //            commandBuilderRobot.AppendLine(entryRobot);

        //            string entryCam = $"HE,1,1,{x},{y},0,{minT},0,0";
        //            commandBuilderCam.AppendLine(entryCam);
        //        }
        //    }

        //    for (double Rz = minT; Rz <= maxT; Rz += (maxT - minT) / (stepT - 1))
        //    {
        //        string entryRobot = $"{minX + 10},{minY + 10},{z},{Rx},{Ry},{Rz},{fig},";
        //        commandBuilderRobot.AppendLine(entryRobot);

        //        string entryCam = $"HE,1,1,{minX + 10},{minY + 10},0,{Rz},0,0";
        //        commandBuilderCam.AppendLine(entryCam);
        //    }
        //}
        //private async void btnHE_Click(object sender, EventArgs e)
        //{
        //    btnHE.Enabled = false;

        //    if (
        //    !double.TryParse(txtMinX.Text, out double minX) ||
        //    !double.TryParse(txtMaxX.Text, out double maxX) ||
        //    !int.TryParse(txtStepX.Text, out int stepX) ||
        //    !double.TryParse(txtMinY.Text, out double minY) ||
        //    !double.TryParse(txtMaxY.Text, out double maxY) ||
        //    !int.TryParse(txtStepY.Text, out int stepY) ||
        //    !double.TryParse(txtMinT.Text, out double minT) ||
        //    !double.TryParse(txtMaxT.Text, out double maxT) ||
        //    !int.TryParse(txtStepT.Text, out int stepT) ||
        //    !double.TryParse(txtZ.Text, out double z) ||
        //    !double.TryParse(txtRx.Text, out double Rx) ||
        //    !double.TryParse(txtRy.Text, out double Ry) ||
        //    !int.TryParse(txtRx.Text, out int fig))
        //    {
        //        MessageBox.Show("Nhập số cho các giá trị!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }

        //    StringBuilder commandBuilderCam = new StringBuilder();
        //    StringBuilder commandBuilderRobot = new StringBuilder();


        //    await RunHE(minX, maxX, stepX, minY, maxY, stepY, minT, maxT, stepT, z, Rx, Ry, fig, commandBuilderCam, commandBuilderRobot);


        //    string commandsCam = commandBuilderCam.ToString();
        //    string commandsRobot = commandBuilderRobot.ToString();

        //    Console.WriteLine("commandsCam = " + commandsCam);

        //    commandsCam = commandsCam.Replace("\r\n", "$");

        //    if (cameraController.IsConnected && robotController.IsConnected)
        //    {
        //        try
        //        {
        //            byte[] buffer = new byte[1024];
        //            await cameraController.SendCommand("HEB,1");
        //            await cameraController.ReceiveData(buffer);

        //            StringBuilder dataBuilderCam = new StringBuilder();
        //            StringBuilder dataBuilderRobot = new StringBuilder();

        //            string[] commandLinesCam = commandsCam.Split('$');
        //            string[] commandLinesRobot = commandsRobot.Split('\n');

        //            for (int i = 0; i < commandLinesRobot.Length - 1; i++)
        //            {
        //                await robotController.SendCommand(commandLinesRobot[i]);
        //                string receivedDataRobot = await robotController.ReceiveData(buffer);

        //                if (receivedDataRobot.Contains("OK"))
        //                {
        //                    await cameraController.SendCommand(commandLinesCam[i]);
        //                    string receivedDataCam = await cameraController.ReceiveData(buffer);

        //                    if (receivedDataCam.Contains("HE,1"))
        //                    {
        //                        await Task.Delay(1000);
        //                    }
        //                    else
        //                    {
        //                        break;
        //                    }
        //                }
        //            }

        //            btnHE.Enabled = true;
        //            await cameraController.SendCommand("HEE,1");
        //            await cameraController.ReceiveData(buffer);
        //            MessageBox.Show("HE finish");

        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Lỗi khi gửi và nhận dữ liệu từ camera hoặc robot: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Vui lòng kết nối đến camera và robot trước khi gửi lệnh!");
        //    }
        //}

        private void btnClearData_Click(object sender, EventArgs e)
        {
            txtReceivedData.Clear();
            btnHE.Enabled = true;
            btnAutoCalib.Enabled = true;
        }

        private async void btnGetCurPos_Click(object sender, EventArgs e) // Lấy current pos của robot
        {
            await robotController.SendCommand("CRP,");
            await UpdateCurrentPos();

            txtMinX.Text = robotController.x;
            txtMinY.Text = robotController.y;
            txtMinT.Text = robotController.rz;


        }

        private async void btnAutoCalib_Click(object sender, EventArgs e)
        {

            if (!cameraController.IsConnected || !robotController.IsConnected)
            {
                MessageBox.Show("Kết nối Cam và Robot trước khi Calib");
                return;
            }
            btnAutoCalib.Enabled = false;
            await robotController.SendCommand("CRP,");
            await UpdateCurrentPos();
            var CommandHandEyeBegin = $"ACB,1,1,{x},{y},{z},{rz},{ry},{rx}"; // Lệnh Start HE
            byte[] buffer = new byte[1024];
            await robotController.SendCommand("HE,"); // gửi kí tự HE để robot nhảy vào phần HE trên WC3
            await cameraController.SendCommand(CommandHandEyeBegin);
            string receivedDataCam = await cameraController.ReceiveData();

            if (!receivedDataCam.Contains("ACB,2"))
            {
                MessageBox.Show("Không tìm thấy Feature", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnAutoCalib.Enabled = true;
                return;
            }
            //var index = receivedDataCam.IndexOf(',');
            var NextPosForCam = receivedDataCam.Substring(6); // Lấy kí tự thứ 6 trở đi (ACB,2,....)
            NextPosForCam = NextPosForCam.Replace("\r\n", "");
            string[] part = NextPosForCam.Split(',');
            var NextPosForRobot = "";
            if (part.Length >= 6)
            {
                var temp = part[3];
                part[3] = part[5];
                part[5] = temp;
                NextPosForRobot = string.Join(",", part);
            }
            var PosRobot = $"{NextPosForRobot},{fig},";
            await robotController.SendCommand(PosRobot);

            await robotController.ReceiveData();

            var CommandHandEyeStep = "AC,1,1," + NextPosForCam;
            await cameraController.SendCommand(CommandHandEyeStep);
            var CamResponse = await cameraController.ReceiveData();

            while (!CamResponse.Contains("AC,1"))
            {
                if (!CamResponse.Contains("AC,2"))
                {
                    MessageBox.Show("Không tìm thấy Feature", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnAutoCalib.Enabled = true;
                    return;
                }


                var NextPositionForCam = CamResponse.Substring(5); // Lấy kí tự thứ 5 trở đi (AC,2,....)
                NextPositionForCam = NextPositionForCam.Replace("\r\n", "");

                string[] parts = NextPositionForCam.Split(',');
                var NextPositionForRobot = "";

                if (parts.Length >= 6)
                {
                    // Đổi vị trí giữa phần tử Rz và Rx
                    string temp = parts[3];
                    parts[3] = parts[5];
                    parts[5] = temp;

                    NextPositionForRobot = string.Join(",", parts);

                }
                var SendPosToRobot = $"{NextPositionForRobot},{fig},";

                await robotController.SendCommand(SendPosToRobot);

                string receivedDataRobot = await robotController.ReceiveData();
                if (receivedDataRobot.Contains("OK"))
                {
                    var commandHeStep = "AC,1,1," + NextPositionForCam;
                    await cameraController.SendCommand(commandHeStep);
                    CamResponse = await cameraController.ReceiveData();
                    if (CamResponse.Contains("AC,1"))
                    { break; }
                    await Task.Delay(500);
                }

            }

            btnAutoCalib.Enabled = true;
            MessageBox.Show("Calib Success");
        }



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
       
        private async Task UpdateCurrentPos()
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

        //private async Task UpdateCurrentJoint()
        //{
        //    await robotController.GetRobotCurrentJoint();
        //    j1 = robotController.j1;
        //    j2 = robotController.j2;
        //    j3 = robotController.j3;
        //    j4 = robotController.j4;
        //    j5 = robotController.j5;
        //    j6 = robotController.j6;
            
        //}


        private async void btnGetCurPos2_Click(object sender, EventArgs e)
        {
            await robotController.SendCommand("CRP,");
            await UpdateCurrentPos();
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




