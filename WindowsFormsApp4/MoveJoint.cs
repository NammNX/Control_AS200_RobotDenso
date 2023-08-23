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
            await robotController.SendCommand("OK");
            await UpdateCurrentPos();
            UpdateUIComponents();
        }

        private async void HandleJointButton(Button button, string command)
        {
            await robotController.SendCommand(command);
        }

        
        private void AssignJointButton(Button button, string command)
        {
            button.MouseDown += (sender, e) => HandleJointButton(button, command);
            button.MouseUp += async (sender, e) => await MouseUpJoint();
        }

        private void RegisterJointButton()
        {
            AssignJointButton(btnJ1Plus, "J1+");
            AssignJointButton(btnJ1Minus, "J1-");
            AssignJointButton(btnJ2Plus, "J2+");
            AssignJointButton(btnJ2Minus, "J2-");
            AssignJointButton(btnJ3Plus, "J3+");
            AssignJointButton(btnJ3Minus, "J3-");
            AssignJointButton(btnJ4Plus, "J4+");
            AssignJointButton(btnJ4Minus, "J4-");
            AssignJointButton(btnJ5Plus, "J5+");
            AssignJointButton(btnJ5Minus, "J5-");
            AssignJointButton(btnJ6Plus, "J6+");
            AssignJointButton(btnJ6Minus, "J6-");

        }
       
    }

    
}
