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

        private TcpClient robotClient;
        private TcpClient cameraClient;
       

        public Form1()
        {
            InitializeComponent();
            cameraController = new CameraController();
            cameraController.TextReceivedData = txtReceivedData;
            robotController = new RobotController();
            robotController.TextReceivedData = txtReceivedData;
        }




        
        private void btnConnectRobot_Click(object sender, EventArgs e)
        {
            string ipAddress = txtRobotIP.Text;
            int port = int.Parse(txtRobotPort.Text);
            robotController.ConnectRobot(ipAddress, port);
            lbStatusRobot.Text = "Connected";
            btnConnectRobot.Enabled = false;
        }

        private void btnDisconnectRobot_Click(object sender, EventArgs e)
        {
            robotController.DisConnectRobot();
            btnConnectRobot.Enabled = true;
        }

        private async void btnConnectCam_Click(object sender, EventArgs e)
        {
            string ipAddress = txtCamIp.Text;
            int port = int.Parse(txtCamPort.Text);
            cameraController.ConnectCamera(ipAddress, port);
            lbstatusCam.Text = "Connected";
            btnConnectCam.Enabled = false;

        }
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            cameraController.DisconnectCamera();
            btnConnectCam.Enabled = true;
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

        private async void btnHE_Click(object sender, EventArgs e)
        {
            btnHE.Enabled = false;

            if (
            !double.TryParse(txtMinX.Text, out double minX) ||
            !double.TryParse(txtMaxX.Text, out double maxX) ||
            !int.TryParse(txtStepX.Text, out int stepX) ||
            !double.TryParse(txtMinY.Text, out double minY) ||
            !double.TryParse(txtMaxY.Text, out double maxY) ||
            !int.TryParse(txtStepY.Text, out int stepY) ||
            !double.TryParse(txtMinT.Text, out double minT) ||
            !double.TryParse(txtMaxT.Text, out double maxT) ||
            !int.TryParse(txtStepT.Text, out int stepT) ||
            !double.TryParse(txtZ.Text, out double z) ||
            !double.TryParse(txtRx.Text, out double Rx) ||
            !double.TryParse(txtRy.Text, out double Ry))
            {
                MessageBox.Show("Nhập số cho các giá trị!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            StringBuilder commandBuilderCam = new StringBuilder();
            StringBuilder commandBuilderRobot = new StringBuilder();
            int fig = 5; // Gán tư thế robot = 5

            await RunHE(minX, maxX, stepX, minY, maxY, stepY, minT, maxT, stepT, z, Rx, Ry, fig, commandBuilderCam, commandBuilderRobot);


            string commandsCam = commandBuilderCam.ToString();
            string commandsRobot = commandBuilderRobot.ToString();

            Console.WriteLine("commandsCam = " + commandsCam);

            commandsCam = commandsCam.Replace("\r\n", "$");

            if (cameraClient != null && cameraClient.Connected && robotClient != null && robotClient.Connected)
            {
                try
                {
                    NetworkStream cameraStream = cameraClient.GetStream();
                    NetworkStream robotStream = robotClient.GetStream();
                    byte[] buffer = new byte[1024];
                    StringBuilder dataBuilderCam = new StringBuilder();
                    StringBuilder dataBuilderRobot = new StringBuilder();

                    string[] commandLinesCam = commandsCam.Split('$');
                    string[] commandLinesRobot = commandsRobot.Split('\n');

                    for (int i = 0; i < commandLinesRobot.Length - 1; i++)
                    {
                        await robotController.SendCommand(commandLinesRobot[i]);
                        string receivedDataRobot = await robotController.ReceiveData(buffer);

                        if (receivedDataRobot.Contains("OK"))
                        {
                            await cameraController.SendCommand(commandLinesCam[i]);
                            string receivedDataCam = await cameraController.ReceiveData(buffer);

                            if (receivedDataCam.Contains("HE,1"))
                            {
                                await Task.Delay(1000);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    btnHE.Enabled = true;
                    finishHE();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi gửi và nhận dữ liệu từ camera hoặc robot: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng kết nối đến camera và robot trước khi gửi lệnh!");
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
            await robotController.GetRobotCurrentPosition();

            txtMinX.Text = robotController.x;
            txtMinY.Text = robotController.y;
            txtZ.Text = robotController.z;
            txtRx.Text = robotController.rx;
            txtRy.Text = robotController.ry;
            txtMinT.Text = robotController.rz;
            txtFig.Text = robotController.fig;



        }

        async void finishHE()
        {
            string command = "HEE,1";
            byte[] buffer = new byte[1024];
            NetworkStream cameraStream = cameraClient.GetStream();
            await cameraController.SendCommand(command);
            string receivedDataCam = await cameraController.ReceiveData(buffer);
            MessageBox.Show("HE finish");
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
                return;
            }
            string command = $"ACB,1,1,{minX},{minY},{z},{minT},{Ry},{Rx}"; // Start HE
            //string currentPost = $"{minX},{minY},{z},{Rx},{Ry},{minT},{Fig},";
            byte[] buffer = new byte[1024];

            if (cameraClient != null && cameraClient.Connected && robotClient != null && robotClient.Connected)
            {
                //NetworkStream cameraStream = cameraClient.GetStream();
                NetworkStream robotStream = robotClient.GetStream();

                await robotController.SendCommand("HE,"); // gửi kí tự HE để robot nhảy vào phần HE trên WC3
                await cameraController.SendCommand(command);
                string receivedDataCam = await cameraController.ReceiveData(buffer);

                if (receivedDataCam.Contains("ACB,2"))
                {
                    int index = receivedDataCam.IndexOf(',');
                    string NextPosition = receivedDataCam.Substring(index + 3);
                    NextPosition = NextPosition.Replace("\r\n", "");

                    Console.WriteLine(NextPosition);
                    string[] part = NextPosition.Split(',');
                    string _NextPosition = "";
                    if (part.Length >= 6)
                    {
                        string temp = part[3];
                        part[3] = part[5];
                        part[5] = temp;
                        _NextPosition = string.Join(",", part);
                        Console.WriteLine(_NextPosition);
                    }
                    string sendPostoRobot = $"{_NextPosition},{Fig},";
                    await robotController.SendCommand(sendPostoRobot);

                    await robotController.ReceiveData(buffer);

                    string commandNextPosition = "AC,1,1," + NextPosition;
                    await cameraController.SendCommand(commandNextPosition);
                    string CamResponse = await cameraController.ReceiveData(buffer);

                    while (!CamResponse.Contains("AC,1"))
                    {
                        if (CamResponse.Contains("AC,2"))
                        {

                            int index2 = CamResponse.IndexOf(',');
                            string NextPosition2 = CamResponse.Substring(index2 + 3);
                            NextPosition2 = NextPosition2.Replace("\r\n", "");

                            string[] parts = NextPosition2.Split(',');
                            string _SendPosToRobot = "";

                            if (parts.Length >= 6)
                            {
                                // Đổi vị trí giữa phần tử Rz và Rx
                                string temp = parts[3];
                                parts[3] = parts[5];
                                parts[5] = temp;

                                _SendPosToRobot = string.Join(",", parts);

                                Console.WriteLine(_SendPosToRobot);

                            }
                            Console.WriteLine(Fig);
                            String SendPosToRobot = $"{_SendPosToRobot},{Fig},";
                            Console.WriteLine(SendPosToRobot);
                            await robotController.SendCommand(SendPosToRobot);

                            string receivedDataRobot = await robotController.ReceiveData(buffer);
                            if (receivedDataRobot.Contains("OK"))
                            {

                                string commandNextPosition2 = "AC,1,1," + NextPosition2;
                                await cameraController.SendCommand(commandNextPosition2);
                                CamResponse = await cameraController.ReceiveData(buffer);
                                Console.WriteLine(CamResponse);
                                await Task.Delay(500);
                            }

                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy Feature", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }

                    btnAutoCalib.Enabled = true;
                    MessageBox.Show("Calib Done");

                }
                else
                {
                    MessageBox.Show("Không tìm thấy Feature", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnAutoCalib.Enabled = true;
                }

            }
            else { MessageBox.Show("Kết nối Cam và Robot trước khi Calib"); }
        }
        private async void btnGetCurPos2_Click(object sender, EventArgs e)
        {
            await robotController.GetRobotCurrentPosition();
            txtX.Text = robotController.x;
            txtY.Text = robotController.y;
            txtZ2.Text = robotController.z;
            txtRx2.Text = robotController.rx;
            txtRy2.Text = robotController.ry;
            txtRz.Text = robotController.rz;
            txtFig2.Text = robotController.fig;

        }
        
        private async void btnMoveRobot_Click(object sender, EventArgs e)
        {
            if (
           double.TryParse(txtX.Text, out double x) &&
           double.TryParse(txtY.Text, out double y) &&
           double.TryParse(txtZ2.Text, out double z) &&
           double.TryParse(txtRx2.Text, out double rx) &&
           double.TryParse(txtRy2.Text, out double ry) &&
           double.TryParse(txtRz.Text, out double rz) &&
           double.TryParse(txtFig2.Text, out double fig))

            {
                await robotController.MoveRobot(x, y, z, rx, ry, rz, fig);
            }
            else
            {
                MessageBox.Show("Nhập số cho các giá trị!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private bool isXIncreasing = false;
        private bool isXDecreasing = false;
        private bool isYIncreasing = false;
        private bool isYDecreasing = false;
        private bool isZIncreasing = false;
        private bool isZDecreasing = false;
        private bool isRzIncreasing = false;
        private bool isRzDecreasing = false;

        //move X -----------------------------------------------------
        private async void btnXup_MouseDown(object sender, MouseEventArgs e)
        {
            isXIncreasing = true;
            
            while (isXIncreasing)
            {

                double.TryParse(txtX.Text, out double x);
                double.TryParse(txtY.Text, out double y);
                double.TryParse(txtZ2.Text, out double z);
                double.TryParse(txtRx2.Text, out double rx);
                double.TryParse(txtRy2.Text, out double ry);
                double.TryParse(txtRz.Text, out double rz);
                double.TryParse(txtFig2.Text, out double fig);
                x += 10;
                txtX.Text = x.ToString();
                await robotController.MoveRobot(x, y, z, rx, ry, rz, fig);
                byte[] buffer = new byte[1024];
                await robotController.ReceiveData(buffer);

            }
        }

        private void btnXup_MouseUp(object sender, MouseEventArgs e)
        {
            isXIncreasing = false;
        }

        private async void btnXdown_MouseDown(object sender, MouseEventArgs e)
        {
            isXDecreasing = true;
           
            while (isXDecreasing)
            {
                double.TryParse(txtX.Text, out double x);
                double.TryParse(txtY.Text, out double y);
                double.TryParse(txtZ2.Text, out double z);
                double.TryParse(txtRx2.Text, out double rx);
                double.TryParse(txtRy2.Text, out double ry);
                double.TryParse(txtRz.Text, out double rz);
                double.TryParse(txtFig2.Text, out double fig);
                x += -10;
                txtX.Text = x.ToString();
                await robotController.MoveRobot(x, y, z, rx, ry, rz, fig);
                byte[] buffer = new byte[1024];
                await robotController.ReceiveData(buffer);
            }
        }

        private void btnXdown_MouseUp(object sender, MouseEventArgs e)
        {
            isXDecreasing = false;
        }
        // move Y -------------------------------------------------
        private async void btnYup_MouseDown(object sender, MouseEventArgs e)
        {
            isYIncreasing = true;

            while (isYIncreasing)
            {

                double.TryParse(txtX.Text, out double x);
                double.TryParse(txtY.Text, out double y);
                double.TryParse(txtZ2.Text, out double z);
                double.TryParse(txtRx2.Text, out double rx);
                double.TryParse(txtRy2.Text, out double ry);
                double.TryParse(txtRz.Text, out double rz);
                double.TryParse(txtFig2.Text, out double fig);
                y += 10;
                txtY.Text = y.ToString();
                await robotController.MoveRobot(x, y, z, rx, ry, rz, fig);

                byte[] buffer = new byte[1024];
                await robotController.ReceiveData(buffer);
            }
        }

        private void btnYup_MouseUp(object sender, MouseEventArgs e)
        {
            isYIncreasing = false;
        }

        private async void btnYdown_MouseDown(object sender, MouseEventArgs e)
        {
            isYDecreasing = true;

            while (isYDecreasing)
            {
                double.TryParse(txtX.Text, out double x);
                double.TryParse(txtY.Text, out double y);
                double.TryParse(txtZ2.Text, out double z);
                double.TryParse(txtRx2.Text, out double rx);
                double.TryParse(txtRy2.Text, out double ry);
                double.TryParse(txtRz.Text, out double rz);
                double.TryParse(txtFig2.Text, out double fig);
                y += -10;
                txtY.Text = y.ToString();
                await robotController.MoveRobot(x, y, z, rx, ry, rz, fig);
                byte[] buffer = new byte[1024];
                await robotController.ReceiveData(buffer);
            }
        }

        private void btnYdown_MouseUp(object sender, MouseEventArgs e)
        {
            isYDecreasing = false;
        }

        //move Z -----------------------------------------------
        private async void btnZup_MouseDown(object sender, MouseEventArgs e)
        {
            isZIncreasing = true;

            while (isZIncreasing)
            {

                double.TryParse(txtX.Text, out double x);
                double.TryParse(txtY.Text, out double y);
                double.TryParse(txtZ2.Text, out double z);
                double.TryParse(txtRx2.Text, out double rx);
                double.TryParse(txtRy2.Text, out double ry);
                double.TryParse(txtRz.Text, out double rz);
                double.TryParse(txtFig2.Text, out double fig);
                z += 10;
                txtZ2.Text = z.ToString();
                await robotController.MoveRobot(x, y, z, rx, ry, rz, fig);

                byte[] buffer = new byte[1024];
                await robotController.ReceiveData(buffer);

            }
        }

        private void btnZup_MouseUp(object sender, MouseEventArgs e)
        {
            isZIncreasing = false;
        }

        private async void btnZdown_MouseDown(object sender, MouseEventArgs e)
        {
            isZDecreasing = true;

            while (isZDecreasing)
            {
                double.TryParse(txtX.Text, out double x);
                double.TryParse(txtY.Text, out double y);
                double.TryParse(txtZ2.Text, out double z);
                double.TryParse(txtRx2.Text, out double rx);
                double.TryParse(txtRy2.Text, out double ry);
                double.TryParse(txtRz.Text, out double rz);
                double.TryParse(txtFig2.Text, out double fig);
                z += -10;
                txtZ2.Text = z.ToString();
                await robotController.MoveRobot(x, y, z, rx, ry, rz, fig);
                byte[] buffer = new byte[1024];
                await robotController.ReceiveData(buffer);
            }
        }

        private void btnZdown_MouseUp(object sender, MouseEventArgs e)
        {
            isZDecreasing = false;
        }

        //move Rz ---------------------------------------------------------------
        private async void btnRzup_MouseDown(object sender, MouseEventArgs e)
        {
            isRzIncreasing = true;

            while (isRzIncreasing)
            {

                double.TryParse(txtX.Text, out double x);
                double.TryParse(txtY.Text, out double y);
                double.TryParse(txtZ2.Text, out double z);
                double.TryParse(txtRx2.Text, out double rx);
                double.TryParse(txtRy2.Text, out double ry);
                double.TryParse(txtRz.Text, out double rz);
                double.TryParse(txtFig2.Text, out double fig);
                rz += 2;
                txtRz.Text = rz.ToString();
                await robotController.MoveRobot(x, y, z, rx, ry, rz, fig);

                byte[] buffer = new byte[1024];
                await robotController.ReceiveData(buffer);

            }
        }

        private void btnRzup_MouseUp(object sender, MouseEventArgs e)
        {
            isRzIncreasing = false;
        }

        private async void btnRzdown_MouseDown(object sender, MouseEventArgs e)
        {
            isRzDecreasing = true;

            while (isRzDecreasing)
            {
                double.TryParse(txtX.Text, out double x);
                double.TryParse(txtY.Text, out double y);
                double.TryParse(txtZ2.Text, out double z);
                double.TryParse(txtRx2.Text, out double rx);
                double.TryParse(txtRy2.Text, out double ry);
                double.TryParse(txtRz.Text, out double rz);
                double.TryParse(txtFig2.Text, out double fig);
                rz += -2;
                txtRz.Text = rz.ToString();
                await robotController.MoveRobot(x, y, z, rx, ry, rz, fig);
                byte[] buffer = new byte[1024];
                await robotController.ReceiveData(buffer);
            }
        }

        private void btnRzdown_MouseUp(object sender, MouseEventArgs e)
        {
            isRzDecreasing = false;
        }

        
        private async Task TrainVisionPickPlace(int feature)
        {
            double.TryParse(txtX.Text, out double x);
            double.TryParse(txtY.Text, out double y);
            double.TryParse(txtZ2.Text, out double z);
            double.TryParse(txtRx2.Text, out double rx);
            double.TryParse(txtRy2.Text, out double ry);
            double.TryParse(txtRz.Text, out double rz);

            string command = $"TT,{feature},{x},{y},{z},{rz},{ry},{rx}";
            await cameraController.SendCommand(command);

            byte[] buffer = new byte[1024];
            await cameraController.ReceiveData(buffer);
        }

        private async Task TrainRobotPickPlace(int feature)
        {
            double.TryParse(txtX.Text, out double x);
            double.TryParse(txtY.Text, out double y);
            double.TryParse(txtZ2.Text, out double z);
            double.TryParse(txtRx2.Text, out double rx);
            double.TryParse(txtRy2.Text, out double ry);
            double.TryParse(txtRz.Text, out double rz);

            string command = $"TTR,{feature},{x},{y},{z},{rz},{ry},{rx}";
            await cameraController.SendCommand(command);

            byte[] buffer = new byte[1024];
            await cameraController.ReceiveData(buffer);
        }
        // Train điểm gắp, lấy điểm chụp là feature 1
        private async void btnTrainVisionPick_Click(object sender, EventArgs e)
        {

            int feature = 1;
            await TrainVisionPickPlace(feature);
        }
        
        private async void btnTrainRobotPick_Click(object sender, EventArgs e)
        {
            int feature = 1;
            await TrainRobotPickPlace(feature);
        }

        // Train điểm thả, lấy điểm gắp là feature 2
        private async void btnTrainVisionPlace_Click(object sender, EventArgs e)
        {
            int feature = 2;
            await TrainVisionPickPlace(feature);

        }

        private async void btnTrainRobotPlace_Click(object sender, EventArgs e)
        {
            int feature = 2;
            await TrainRobotPickPlace(feature);
        }
    }

}


