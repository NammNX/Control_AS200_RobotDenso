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
        public string x { get;set; }
        public string y { get; set; }

        public string z { get; set; }
        public string rx { get; set; }
        public string ry { get; set; }
        public string rz { get; set; }
        public string fig { get; set; }


        public void ConnectRobot(string ipAddress, int port)
        {
            try
            {
                robotClient = new TcpClient();
                robotClient.Connect(ipAddress, port);

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
                StringBuilder dataBuilderRobot = new StringBuilder();
                try
                {
                    NetworkStream stream = robotClient.GetStream();
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
                        Console.WriteLine(fig);
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

        public async Task MoveRobot(double x, double y, double z, double rx, double ry, double rz, double fig)
        {   
            
            string PosRobot = $"{x},{y},{z},{rx},{ry},{rz},{fig},";
            //NetworkStream robotStream = robotClient.GetStream();
            if (robotClient != null && robotClient.Connected)
            {
                await SendCommand("ROBOTMOVE,");
               // await Task.Delay(100);
                await SendCommand(PosRobot);

            }
        }

    }
}