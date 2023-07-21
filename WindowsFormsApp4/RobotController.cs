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
        public bool IsConnected { get; private set; }
        public string x { get; private set; }
        public string y { get; private set; }

        public string z { get;private set; }
        public string rx { get; private set; }
        public string ry { get; private set; }
        public string rz { get; private set; }
        public string fig { get; private set; }


        public void ConnectRobot(string ipAddress, int port)
        {
            try
            {
                if (robotClient == null)
                {
                    robotClient = new TcpClient();
                    robotClient.Connect(ipAddress, port);
                    IsConnected = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến robot. Lỗi: " + ex.Message);
            }
        }

        public void DisConnectRobot()
        {
            if (robotClient != null && robotClient.Connected)
            {
                robotClient.Close();
                robotClient = null;
                IsConnected = false;
            }
        }
        public async Task SendCommand(string command)
        {
            if (robotClient != null && robotClient.Connected)
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
            if (robotClient != null && robotClient.Connected)
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
                    MessageBox.Show("Không thể nhận dữ liệu từ camera. Lỗi: " + ex.Message);
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
            if (robotClient != null && robotClient.Connected)
            {
                byte[] buffer = new byte[1024];
                
                try
                {
                    
                    string TakeCurrenPos = "CRP,";
                    await SendCommand(TakeCurrenPos);
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
                    MessageBox.Show("Không thể gửi lệnh đến ROBOT. Lỗi: " + ex.Message);
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
            if (robotClient != null && robotClient.Connected)
            {
                await SendCommand("ROBOTMOVE,");
                await Task.Delay(20);
                await SendCommand(PosRobot);

            }
        }

    }
}