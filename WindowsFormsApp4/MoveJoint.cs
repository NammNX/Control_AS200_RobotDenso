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
        private async Task MouseUpJoint()
        {
            await robotController.SendCommand("OK,");
            await UpdateCurrentPos();
            UpdateUIComponents();
        }
        // --------J1---------
        private async void btnJ1Plus_MouseDown(object sender, MouseEventArgs e)
        {

            await robotController.SendCommand("J1+,");
        }
        private async void btnJ1Plus_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpJoint();
        }

        private async void btnJ1Minus_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("J1-,");
        }

        private async void btnJ1Minus_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpJoint();
        }
        // --------J2---------
        private async void btnJ2Plus_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("J2+,");
        }

        private async void btnJ2Plus_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpJoint();
        }
        private async void btnJ2Minus_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("J2-,");
        }

        private async void btnJ2Minus_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpJoint();
        }
        // --------J3---------

        private async void btnJ3Plus_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("J3+,");
        }

        private async void btnJ3Plus_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpJoint();
        }

        private async void btnJ3Minus_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("J3-,");
        }

        private async void btnJ3Minus_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpJoint();
        }

        // --------J4---------
        private async void btnJ4Plus_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("J4+,");
        }



        private async void btnJ4Plus_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpJoint();
        }



        private async void btnJ4Minus_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("J4-,");
        }

        private async void btnJ4Minus_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpJoint();
        }
        // --------J5---------

        private async void btnJ5Plus_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("J5+,");
        }



        private async void btnJ5Plus_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpJoint();
        }

        private async void btnJ5Minus_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("J5-,");
        }



        private async void btnJ5Minus_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpJoint();
        }

        // --------J6---------

        private async void btnJ6Plus_MouseDown(object sender, MouseEventArgs e)
        {

            await robotController.SendCommand("J6+,");
        }

        private async void btnJ6Plus_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpJoint();
        }

        private async void btnJ6Minus_MouseDown(object sender, MouseEventArgs e)
        {
            await robotController.SendCommand("J6-,");
        }

        private async void btnJ6Minus_MouseUp(object sender, MouseEventArgs e)
        {
            await MouseUpJoint();
        }
    }

    
}
