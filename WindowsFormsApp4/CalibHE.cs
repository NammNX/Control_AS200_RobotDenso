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
        public class HEParameters
        {
            public double MinX { get; set; }
            public double MaxX { get; set; }
            public int StepX { get; set; }
            public double MinY { get; set; }
            public double MaxY { get; set; }
            public int StepY { get; set; }
            public double MinT { get; set; }
            public double MaxT { get; set; }
            public int StepT { get; set; }
            public string Z { get; set; }
            public string Rx { get; set; }
            public string Ry { get; set; }
            public string Fig { get; set; }
        }

        public void RunHE(HEParameters heParams, StringBuilder commandBuilderCam, StringBuilder commandBuilderRobot)
        {
            for (double y = heParams.MinY; y <= heParams.MaxY; y += (heParams.MaxY - heParams.MinY) / (heParams.StepY - 1))
            {
                for (double x = heParams.MinX; x <= heParams.MaxX; x += (heParams.MaxX - heParams.MinX) / (heParams.StepX - 1))
                {
                    string entryRobot = $"{x},{y},{heParams.Z},{heParams.Rx},{heParams.Ry},{heParams.MinT},{heParams.Fig},";
                    commandBuilderRobot.AppendLine(entryRobot);

                    string entryCam = $"HE,1,1,{x},{y},0,{heParams.MinT},0,0";
                    commandBuilderCam.AppendLine(entryCam);
                }
            }

            for (double Rz = heParams.MinT; Rz <= heParams.MaxT; Rz += (heParams.MaxT - heParams.MinT) / (heParams.StepT - 1))
            {
                string entryRobot = $"{heParams.MinX + 10},{heParams.MinY + 10},{heParams.Z},{heParams.Rx},{heParams.Ry},{Rz},{heParams.Fig},";
                commandBuilderRobot.AppendLine(entryRobot);

                string entryCam = $"HE,1,1,{heParams.MinX + 10},{heParams.MinY + 10},0,{Rz},0,0";
                commandBuilderCam.AppendLine(entryCam);
            }
        }

        private async void btnHE_Click(object sender, EventArgs e)
        {
            if (!cameraController.IsConnected || !robotController.IsConnected)
            {
                MessageBox.Show("Kết nối đến camera và robot trước khi gửi lệnh!");
                return;
            }

            if (
            !double.TryParse(txtMinX.Text, out double minX) ||
            !double.TryParse(txtMaxX.Text, out double maxX) ||
            !int.TryParse(txtStepX.Text, out int stepX) ||
            !double.TryParse(txtMinY.Text, out double minY) ||
            !double.TryParse(txtMaxY.Text, out double maxY) ||
            !int.TryParse(txtStepY.Text, out int stepY) ||
            !double.TryParse(txtMinT.Text, out double minT) ||
            !double.TryParse(txtMaxT.Text, out double maxT) ||
            !int.TryParse(txtStepT.Text, out int stepT))
            
            {
                MessageBox.Show("Nhập số cho các giá trị!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            btnHE.Enabled = false;
            var heParams = new HEParameters
            {
                MinX = minX,
                MaxX = maxX,
                StepX = stepX,
                MinY = minY,
                MaxY = maxY,
                StepY = stepY,
                MinT = minT,
                MaxT = maxT,
                StepT = stepT,
                Z = robotController.z,
                Rx = robotController.rx,
                Ry = robotController.ry,
                Fig = robotController.fig
            };
            var commandBuilderCam = new StringBuilder();
            var commandBuilderRobot = new StringBuilder();

            RunHE(heParams, commandBuilderCam, commandBuilderRobot);

            string commandsCam = commandBuilderCam.ToString();
            string commandsRobot = commandBuilderRobot.ToString();
            commandsCam = commandsCam.Replace("\r\n", "$");
            string[] commandLinesCam = commandsCam.Split('$');
            string[] commandLinesRobot = commandsRobot.Split('\n');

            await robotController.SendCommand("HE,"); // gửi kí tự HE để robot nhảy vào phần HE trên WC3
            await Task.Delay(500);
            
            try
                {
                    await cameraController.SendCommand("HEB,1");
                    var receivedDataCamHEB = await cameraController.ReceiveData();
                if (!receivedDataCamHEB.Contains("HEB,1"))
                {
                    MessageBox.Show("Calib Fail");
                    btnHE.Enabled = true;
                    return;
                }

                    for (int i = 0; i < commandLinesRobot.Length - 1; i++)
                    {
                        await robotController.SendCommand(commandLinesRobot[i]);
                        string receivedDataRobot = await robotController.ReceiveData();

                        if (receivedDataRobot.Contains("OK"))
                        {
                            await cameraController.SendCommand(commandLinesCam[i]);
                            string receivedDataCam = await cameraController.ReceiveData();

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
                    await cameraController.SendCommand("HEE,1");
                    await cameraController.ReceiveData();
                    MessageBox.Show("HE finish");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi gửi và nhận dữ liệu từ camera hoặc robot: " + ex.Message);
                }
           
        }


    }
}
