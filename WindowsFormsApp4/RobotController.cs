using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public class RobotController
    {
        private TcpClient robotClient;
        public TextBox TextReceivedData { get; set; }
        public bool IsConnected { get { return robotClient != null && robotClient.Connected; } }
        public string x { get; set; }
        public string y { get; set; }

        public string z { get; set; }
        public string rx { get; set; }
        public string ry { get; set; }
        public string rz { get; set; }
        public string j1 { get; set; }
        public string j2 { get; set; }

        public string j3 { get; set; }
        public string j4 { get; set; }
        public string j5 { get; set; }
        public string j6 { get; set; }
        public string fig { get; set; }



        public void ConnectRobot(string ipAddress, int port)
        {
            try
            {
                if (!IsConnected)
                {
                    robotClient = new TcpClient();
                    robotClient.Connect(ipAddress, port);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến robot. Lỗi: " + ex.Message);

            }
        }

        public void DisConnectRobot()
        {
            if (IsConnected)
            {
                robotClient.Close();
                robotClient = null;

            }
        }
        public async Task SendCommand(string command)
        {
            if (IsConnected)
            {
                try
                {
                    byte[] dataRobot = Encoding.ASCII.GetBytes(command);
                    TextReceivedData.Invoke((MethodInvoker)(() =>
                    {
                        TextReceivedData.AppendText(">>>> Robot: " + command + Environment.NewLine);
                    }));
                    await robotClient.GetStream().WriteAsync(dataRobot, 0, dataRobot.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể gửi lệnh đến robot. Lỗi: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Robot chưa được kết nối!");
            }
        }

        public async Task<string> ReceiveData(byte[] buffer)
        {
            if (IsConnected)
            {
                try
                {
                    StringBuilder dataBuilderRobot = new StringBuilder();

                    int bytesReadRobot = await robotClient.GetStream().ReadAsync(buffer, 0, buffer.Length);
                    if (bytesReadRobot > 0)
                    {
                        string receivedDataRobot = Encoding.ASCII.GetString(buffer, 0, bytesReadRobot);
                        dataBuilderRobot.Append(receivedDataRobot);

                        TextReceivedData.Invoke((MethodInvoker)(() =>
                        {
                            TextReceivedData.AppendText("<<<< Robot: " + receivedDataRobot + Environment.NewLine);
                        }));

                        return receivedDataRobot;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể nhận dữ liệu từ ROBOT. Lỗi: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Camera chưa được kết nối!");
            }

            return string.Empty;
        }

        public async Task GetRobotCurrentPosition()
        {
            if (IsConnected)
            {
                byte[] buffer = new byte[1024];

                try
                {
                    string receivedDataRobot = await ReceiveData(buffer);

                    string[] commandLinesRobot = receivedDataRobot.Split(' ');
                    if (commandLinesRobot.Length >= 6)
                    {
                        x = commandLinesRobot[0];
                        y = commandLinesRobot[1];
                        z = commandLinesRobot[2];
                        rx = commandLinesRobot[3];
                        ry = commandLinesRobot[4];
                        rz = commandLinesRobot[5];
                        fig = commandLinesRobot[6];

                    }
                }

                catch (Exception ex)
                {
                    //MessageBox.Show("Không thể gửi lệnh đến ROBOT(@@). Lỗi: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Kết nối robot !");
            }
        }

        public async Task GetRobotCurrentJoint()
        {
            if (IsConnected)
            {
                byte[] buffer = new byte[1024];

                try
                {
                    string receivedDataRobot = await ReceiveData(buffer);

                    string[] commandLinesRobot = receivedDataRobot.Split(' ');
                    if (commandLinesRobot.Length >= 5)
                    {
                        j1 = commandLinesRobot[0];
                        j2 = commandLinesRobot[1];
                        j3 = commandLinesRobot[2];
                        j4 = commandLinesRobot[3];
                        j5 = commandLinesRobot[4];
                        j6 = commandLinesRobot[5];
                        

                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Không thể gửi lệnh đến ROBOT(?). Lỗi: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Kết nối robot !");
            }
        }
        public async Task MoveRobot(string x, string y, string z, string rx, string ry, string rz, string fig)
        {

            string PosRobot = $"{x},{y},{z},{rx},{ry},{rz},{fig},";
            //NetworkStream robotStream = robotClient.GetStream();
            if (IsConnected)
            {
                await SendCommand("ROBOTMOVE,");
                await Task.Delay(20);
                await SendCommand(PosRobot);

            }
        }

        //public async Task MoveJoint(string j1, string j2, string j3, string j4, string j5, string j6)
        //{
        //    string JointRobot = $"{j1},{j2},{j3},{j4},{j5},{j6},";
        //    //NetworkStream robotStream = robotClient.GetStream();
        //    if (IsConnected)
        //    {
        //        await SendCommand("MoveJoint,");
        //        await Task.Delay(20);
        //        await SendCommand(JointRobot);

        //    }

        //}
    }
}