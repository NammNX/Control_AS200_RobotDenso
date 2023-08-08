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
            if (!IsConnected)
            { 
                MessageBox.Show("Robot chưa được kết nối!");
                return;
            }
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

        public async Task<string> ReceiveData()
        {
            if (IsConnected)
            {
                try
                {
                    StringBuilder dataBuilderRobot = new StringBuilder();
                    byte[] buffer = new byte[1024];

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
            if (!IsConnected)
            { 
                MessageBox.Show("Kết nối robot !");
                return;
            }    
                try
                {
                byte[] buffer = new byte[1024];
                string receivedDataRobot = await ReceiveData();

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
                    MessageBox.Show("Không thể gửi lệnh đến ROBOT. Lỗi: " + ex.Message);
                }
           
        }

        //public async Task GetRobotCurrentJoint()
        //{
        //    if (!IsConnected)
        //    {
        //        MessageBox.Show("Kết nối robot !");
        //        return;
        //    }
               

        //        try
        //        {
        //        byte[] buffer = new byte[1024];
        //        string receivedDataRobot = await ReceiveData();

        //            string[] commandLinesRobot = receivedDataRobot.Split(' ');
        //            if (commandLinesRobot.Length >= 5)
        //            {
        //                j1 = commandLinesRobot[0];
        //                j2 = commandLinesRobot[1];
        //                j3 = commandLinesRobot[2];
        //                j4 = commandLinesRobot[3];
        //                j5 = commandLinesRobot[4];
        //                j6 = commandLinesRobot[5];

        //            }
        //        }

        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Không thể gửi lệnh đến ROBOT(?). Lỗi: " + ex.Message);
        //        }
           
        //}
        public async Task MoveRobot(string x, string y, string z, string rx, string ry, string rz, string fig)
        {

            var PosRobot = $"{x},{y},{z},{rx},{ry},{rz},{fig},";
            if (!IsConnected)
            {
                MessageBox.Show("Kết nối robot !");
                return; 
            }
                await SendCommand("ROBOTMOVE,");
                await Task.Delay(20);
                await SendCommand(PosRobot);

            }
        }

       
    }
