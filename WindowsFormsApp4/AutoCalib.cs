using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form1
    {
        private bool isFeature = true;
        
        private async Task<string> AutoHandEyeBegin()
        {
            var commandHandEyeBegin = $"ACB,1,1,{x},{y},{z},{rz},{ry},{rx}"; // Lệnh Start HE
            await robotController.SendCommand("HE"); // gửi chữ HE để robot nhảy vào phần HE trên WC3
            await cameraController.SendCommand(commandHandEyeBegin);
            var receivedDataCam = await cameraController.ReceiveData();

            if (!receivedDataCam.Contains("ACB,2"))
            {
                isFeature = false;
                return string.Empty;
            }

            var nextPosForCam = receivedDataCam.Substring(6).Replace("\r\n", ""); // Lấy kí tự thứ 6 trở đi (ACB,2,....)
            var nextPosForRobot = ChangeDataFromCamToPosRobot(nextPosForCam);
            var posRobot = $"{nextPosForRobot},{fig}";
            await robotController.SendCommand(posRobot);
            await robotController.ReceiveData();
            return nextPosForCam;
        }

        private async Task AutoHandEyeStep(string nextPosForCam)
        {
            while (true)
            {
                var commandHandEyeStep = "AC,1,1," + nextPosForCam;
                await cameraController.SendCommand(commandHandEyeStep);
                var camResponse = await cameraController.ReceiveData();
                if (camResponse.Contains("AC,1"))
                {
                    btnAutoCalib.Enabled = true;
                    MessageBox.Show("Calib Success");
                    return;
                }
                else if (camResponse.Contains("AC,2"))
                {
                    var nextPositionForCam = camResponse.Substring(5).Replace("\r\n", ""); ; // Lấy kí tự thứ 5 trở đi (AC,2,....)
                    var nextPositionForRobot = ChangeDataFromCamToPosRobot(nextPositionForCam);
                    var sendPosToRobot = $"{nextPositionForRobot},{fig}";
                    await robotController.SendCommand(sendPosToRobot);
                    await robotController.ReceiveData();
                    nextPosForCam = nextPositionForCam;
                    await Task.Delay(500);

                }
                else
                {
                    MessageBox.Show("Không tìm thấy Feature", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnAutoCalib.Enabled = true;
                    return;
                }

            }
        }
        
        private async void btnAutoCalib_Click(object sender, EventArgs e)
        {
            if (!cameraController.IsConnected || !robotController.IsConnected)
            {
                MessageBox.Show("Kết nối Cam và Robot trước khi Calib");
                return;
            }
            btnAutoCalib.Enabled = false;
            await robotController.SendCommand("CRP");
            await UpdateCurrentPos();
            var nextPosForCam = await AutoHandEyeBegin();
            if (!isFeature)
            {
                MessageBox.Show("Không tìm thấy Feature", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnAutoCalib.Enabled = true;
                return;
            }
            await AutoHandEyeStep(nextPosForCam);
        }
    }
}
