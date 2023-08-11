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

        public Form1()
        {
            InitializeComponent();

            this.FormClosing += Form1_FormClosing;
            cameraController = new CameraController();
            cameraController.TextReceivedData = txtReceivedData;
            robotController = new RobotController();
            robotController.TextReceivedData = txtReceivedData;
            robotController.TextReceivedData = txtReceiveDataRobot;
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
            await cameraController.ReceiveData();

        }
        private void btnClearData_Click(object sender, EventArgs e)
        {
            txtReceivedData.Clear();
            btnHE.Enabled = true;
            btnAutoCalib.Enabled = true;
            btnTrainPickPlace.Enabled = true;
            btnTrainVisionPoint.Enabled = true;
        }

      

        private async void btnSendRobot_Click(object sender, EventArgs e)
        {
            string command = txtSendRobot.Text.Trim() + ",";
            await robotController.SendCommand(command);
            await robotController.ReceiveData();
        }

        private void btnCleanDataRobot_Click(object sender, EventArgs e)
        {
            txtReceiveDataRobot.Clear();
            btnTrainPickPlace.Enabled = true;
            btnTrainVisionPoint.Enabled = true;
        }

       

        private async void btnGetCurPos_Click(object sender, EventArgs e) // Lấy current pos của robot
        {
           await robotController.SendCommand("CRP,");
           await UpdateCurrentPos();

            txtMinX.Text = robotController.x;
            txtMinY.Text = robotController.y;
            txtMinT.Text = robotController.rz;


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


        private async void btnGetCurPos2_Click(object sender, EventArgs e)
        {
            await robotController.SendCommand("CRP,");
            await UpdateCurrentPos();
            UpdateUIComponents();
        }

        private async void btnMoveRobot_Click(object sender, EventArgs e)
        {
            await robotController.MoveRobot(txtX.Text, txtY.Text, txtZ2.Text, txtRx2.Text, txtRy2.Text, txtRz.Text, txtFig2.Text);
            await UpdateCurrentPos();
            UpdateUIComponents();
        }

       private string ChangeDataFromCamToPosRobot(string DataReiceved)
        {
            string[] part = DataReiceved.Split(',');
            if (part.Length >= 6)
            {
                var temp = part[3];
                part[3] = part[5];
                part[5] = temp;
                DataReiceved = string.Join(",", part);
            }
            return DataReiceved;
        }


    }

}
