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

        private string[] X = new string[10];
        private string[] Y = new string[10];
        private string[] Z = new string[10];
        private string[] RX = new string[10];
        private string[] RY = new string[10];
        private string[] RZ = new string[10];
        private string[] FIG = new string[10];


        public async Task SavePos()
        {

            var Pos = cbPos.Text;


            if (int.TryParse(Pos.Substring(1), out int posIndex))
            {
                int arrayIndex = posIndex - 10;

                X[arrayIndex] = x;
                Y[arrayIndex] = y;
                Z[arrayIndex] = z;
                RX[arrayIndex] = rx;
                RY[arrayIndex] = ry;
                RZ[arrayIndex] = rz;
                FIG[arrayIndex] = fig;

                var labelText = $"{X[arrayIndex]},{Y[arrayIndex]},{Z[arrayIndex]},{RX[arrayIndex]},{RY[arrayIndex]},{RZ[arrayIndex]},{FIG[arrayIndex]}";
                Controls.Find($"lbP{posIndex}", true)[0].Text = labelText;
                await robotController.SendCommand(Pos);
                await Task.Delay(30);
                await robotController.SendCommand(labelText);
                ;
            }

        }
        private async void MoveToPosition_Click(object sender, EventArgs e, int index)
        {
            Button button = (Button)sender;
            index = int.Parse(button.Tag.ToString());

            if (X[index] == null)
            {
                MessageBox.Show("Chưa gán giá trị P");
                return;
            }

            await robotController.MoveRobot(X[index], Y[index], Z[index], RX[index], RY[index], RZ[index], FIG[index]);
            x = X[index];
            y = Y[index];
            z = Z[index];
            rx = RX[index];
            ry = RY[index];
            rz = RZ[index];
            fig = FIG[index];
            UpdateUIComponents();
        }

        private void RegisterSaveButton()
        {
            for (int i = 10; i <= 19; i++)
            {
                Button button = Controls.Find($"btnMoveP{i}", true).FirstOrDefault() as Button;
                if (button != null)
                {
                    button.Tag = i - 10;
                    button.Click += (sender, e) => MoveToPosition_Click(sender, e, i - 10);
                }
            }

        }

        private async void btnSavePos_Click(object sender, EventArgs e)
        {
            if (cbPos.SelectedIndex == -1)
            {
                MessageBox.Show("Chọn P cần lưu");
                return;
            }
            await SavePos();

        }
        // Lưu trạng thái vào tệp tin
        //private void SaveStateToFile()
        //{
        //    var state = new
        //    {
        //        X = X,
        //        Y = Y,
        //        Z = Z,
        //        RX = RX,
        //        RY = RY,
        //        RZ = RZ,
        //        FIG = FIG
        //    };

        //    string json = JsonConvert.SerializeObject(state);
        //    File.WriteAllText("robot_state.json", json);
        //}

        //// Khôi phục trạng thái từ tệp tin khi ứng dụng khởi động
        //private void RestoreStateFromFile()
        //{
        //    if (File.Exists("robot_state.json"))
        //    {
        //        string json = File.ReadAllText("robot_state.json");
        //        var state = JsonConvert.DeserializeObject<dynamic>(json);

        //        X = state.X.ToObject<string[]>();
        //        Y = state.Y.ToObject<string[]>();
        //        Z = state.Z.ToObject<string[]>();
        //        RX = state.RX.ToObject<string[]>();
        //        RY = state.RY.ToObject<string[]>();
        //        RZ = state.RZ.ToObject<string[]>();
        //        FIG = state.FIG.ToObject<string[]>();
        //    }
        //    UpdateLabelText();
        //}
        private void UpdateLabelText()
        {
            for (int i = 0; i < X.Length; i++)
            {
                var labelText = $"{X[i]},{Y[i]},{Z[i]},{RX[i]},{RY[i]},{RZ[i]},{FIG[i]}";
                Controls.Find($"lbP{i + 10}", true)[0].Text = labelText;
            }
        }
        string dataReceive = "";
        private async void btnLoadPosFromPendant_Click(object sender, EventArgs e)
        {
            await robotController.SendCommand("LoadPos");
            dataReceive = await robotController.ReceiveData();
            LoadPos();

        }

        private void LoadPos()
        {
            string[] positionData = dataReceive.Split(' ');
            if (positionData.Length < 70)
            { return; }
            for (int i = 0; i < 10; i++)
            {
                int startIndex = i * 7;

                X[i] = positionData[startIndex];
                Y[i] = positionData[startIndex + 1];
                Z[i] = positionData[startIndex + 2];
                RX[i] = positionData[startIndex + 3];
                RY[i] = positionData[startIndex + 4];
                RZ[i] = positionData[startIndex + 5];
                FIG[i] = positionData[startIndex + 6];
            }
            UpdateLabelText();

        }
    }

}








