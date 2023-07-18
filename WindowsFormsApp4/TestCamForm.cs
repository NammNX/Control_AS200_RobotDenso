using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class TestCamForm : Form
    {
        SimpleTcpClient client = null;// new SimpleTcpClient().Connect("127.0.0.1", 8910);
        public TestCamForm()
        {
            InitializeComponent();
            this.FormClosing += TestCamForm_FormClosing;
            if (client == null)
            {
                client = new SimpleTcpClient();
            }
        }

        private void TestCamForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client == null) { return; }
            client.Disconnect();
        }

        private void btnASCommand(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if(btn == null) { return; }


            switch(btn.Name)
            {
                case "btnConnectIP":
                    var ip = txtIP.Text;
                    var port = int.Parse(txtPORT.Text);
                    client =  new SimpleTcpClient().Connect(ip, port);
                    client.DataReceived += Client_DataReceived;
                    btnConnectIP.Text = "Connected";
                    btnConnectIP.Enabled = false;

                    break;
                
                case "btnDisconnect":
                    if(client == null) { return; }  
                    client.Disconnect();
                    client.DataReceived -= Client_DataReceived; 
                    client.Dispose();
                    client = null;

                    btnConnectIP.Text = "Connect";
                    btnConnectIP.Enabled = true;

                    break;
                
                case "btnSend":
                    if(client == null) { return; }

                    var command = txtCommand.Text + "\r\n";
                    // Chuyển đổi command thành mảng byte để gửi
                    byte[] data = Encoding.ASCII.GetBytes(command);

                    // Gửi dữ liệu đến camera
                    client.Write(data);
                    break;

            }
        }

        private void Client_DataReceived(object sender, SimpleTCP.Message e)
        {
            Console.WriteLine("Data received: "+e.MessageString);
        }
    }
}
