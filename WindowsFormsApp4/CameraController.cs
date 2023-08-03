using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public class CameraController
    {
        private TcpClient cameraClient;
        public TextBox TextReceivedData { get; set; }
        public bool IsConnected { get { return cameraClient != null && cameraClient.Connected; } }

        public void ConnectCamera(string ipAddress, int port)
        {
            try
            {
                if (!IsConnected)
                {
                    cameraClient = new TcpClient();
                    cameraClient.Connect(ipAddress, port);
                   
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến camera. Lỗi: " + ex.Message);
                
            }
        }

        public void DisconnectCamera()
        {
            if (IsConnected)
            {
                cameraClient.Close();
                cameraClient = null;
                
            }

        }

        public async Task SendCommand(string command)
        {
            if (IsConnected)
            {
                try
                {
                    string commandWithNewLine = command + "\r\n";
                    byte[] dataCam = Encoding.ASCII.GetBytes(commandWithNewLine);
                    TextReceivedData.Invoke((MethodInvoker)(() =>
                    {
                        TextReceivedData.AppendText(">>>> Camera: " + commandWithNewLine + Environment.NewLine);
                    }));
                    await cameraClient.GetStream().WriteAsync(dataCam, 0, dataCam.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể gửi lệnh đến camera. Lỗi: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Camera chưa được kết nối!");
            }

        }

        public async Task<string> ReceiveData(byte[] buffer)
        {
            if (IsConnected)
            {
                try
                {
                    StringBuilder dataBuilder = new StringBuilder();
                    

                    int bytesReadCam = await cameraClient.GetStream().ReadAsync(buffer, 0, buffer.Length);
                    Console.WriteLine("bytesReadCam = " + bytesReadCam);
                    if (bytesReadCam > 0)
                    {
                        string receivedDataCam = Encoding.ASCII.GetString(buffer, 0, bytesReadCam);
                        dataBuilder.Append(receivedDataCam);

                        TextReceivedData.Invoke((MethodInvoker)(() =>
                        {
                            TextReceivedData.AppendText("<<<< Camera: " + receivedDataCam + Environment.NewLine);
                        }));

                        return receivedDataCam;
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
    }
}
