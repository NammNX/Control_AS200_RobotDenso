using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp4;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form1
    {
        private async Task MouseUpPos()
        {
            await robotController.SendCommand("OK");
            await UpdateCurrentPos();
            UpdateUIComponents();

        }
        private async void HandleXYZButton(Button button, string command)
        {
            await robotController.SendCommand(command);
        }
        private void AssignXYZButton(Button button, string command)
        {
            button.MouseDown += (sender, e) => HandleXYZButton(button, command);
            button.MouseUp += async (sender, e) => await MouseUpPos();
        }

        private void RegisterXYZButton()
        {
            AssignXYZButton(btnXup, "X+");
            AssignXYZButton(btnXdown, "X-");
            AssignXYZButton(btnYup, "Y+");
            AssignXYZButton(btnYdown, "Y-");
            AssignXYZButton(btnZup, "Z+");
            AssignXYZButton(btnZdown, "Z-");
            AssignXYZButton(btnRzup, "RZ+");
            AssignXYZButton(btnRzdown, "RZ-");
        }

       /* #region Move XYZ
        //move X -----------------------------------------------------
        private async void btnXup_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("X+");
        }

        private async void btnXup_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpPos();
        }

        private async void btnXdown_MouseDown(object sender, MouseEventArgs e)
        {

            await robotController.SendCommand("X-");
        }

        private async void btnXdown_MouseUp(object sender, MouseEventArgs e)
        {

            await MouseUpPos();
        }
        // move Y -------------------------------------------------
        private async void btnYup_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("Y+");

        }

        private async void btnYup_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpPos();
        }

        private async void btnYdown_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("Y-");
        }

        private async void btnYdown_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpPos();
        }

        //move Z -----------------------------------------------
        private async void btnZup_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("Z+");
        }

        private async void btnZup_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpPos();
        }

        private async void btnZdown_MouseDown(object sender, MouseEventArgs e)
        {

            await robotController.SendCommand("Z-");
        }

        private async void btnZdown_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpPos();
        }

        //move Rz ---------------------------------------------------------------
        private async void btnRzup_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("RZ+");
        }

        private async void btnRzup_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpPos();
        }

        private async void btnRzdown_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("RZ-");
        }

        private async void btnRzdown_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpPos();
        }



        #endregion
       */
    }

}
