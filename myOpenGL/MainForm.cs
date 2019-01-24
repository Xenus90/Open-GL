using System;
using System.Windows.Forms;
using OpenGL;

namespace myOpenGL
{
    public partial class MainForm : Form
    {
        cOGL cGL;
        int[] indexFingerOldPosition = { 0, 0, 0 };
        int[] middleFingerOldPosition = { 0, 0, 0 };
        int[] ringFingerOldPosition = { 0, 0, 0 };
        int[] pinkyFingerOldPosition = { 0, 0, 0 };
        int[] thumbFingerOldPosition = { 0, 0 };

        public MainForm()
        {
            InitializeComponent();
            cGL = new cOGL(DrawPanel);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void timerRepaint_Tick(object sender, EventArgs e)
        {
            cGL.Draw();
            timerRepaint.Enabled = false;
        }

        private void timerRUN_Tick(object sender, EventArgs e)
        {
            cGL.Draw();
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            cGL.Draw();
        }

        private void TurnButton_Click(object sender, EventArgs e)
        {
            timerRUN.Enabled = !timerRUN.Enabled;

            if (timerRUN.Enabled)
            {
                TurnButton.Text = "Stop";
                cGL.enableRotate = true;
            }
            else
            {
                TurnButton.Text = "Rotate";
                cGL.enableRotate = false;
            }         
        }        

        private void IndexFingerAngle_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newPosition = (int)numericSender.Value;

            switch (numericSender.Name)
            {
                case "IndexPhalanx1Angle":
                    if (newPosition > indexFingerOldPosition[0])
                        cGL.indexFingerAngle[0] += 10;                       
                    else
                        cGL.indexFingerAngle[0] -= 10;

                    indexFingerOldPosition[0] = newPosition;                                  
                    break;

                case "IndexPhalanx2Angle":
                    if (newPosition > indexFingerOldPosition[1])
                        cGL.indexFingerAngle[1] += 10;
                    else
                        cGL.indexFingerAngle[1] -= 10;

                    indexFingerOldPosition[1] = newPosition;
                    break;

                case "IndexPhalanx3Angle":
                    if (newPosition > indexFingerOldPosition[2])
                        cGL.indexFingerAngle[2] += 10;
                    else
                        cGL.indexFingerAngle[2] -= 10;

                    indexFingerOldPosition[2] = newPosition;
                    break;
            }

            cGL.movingPart = 1;
            cGL.CreateHandList();
            cGL.Draw();            
        }

        private void MiddleFingerAngle_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newPosition = (int)numericSender.Value;

            switch (numericSender.Name)
            {
                case "MiddlePhalanx1Angle":
                    if (newPosition > middleFingerOldPosition[0])
                        cGL.middleFingerAngle[0] += 10;
                    else
                        cGL.middleFingerAngle[0] -= 10;

                    middleFingerOldPosition[0] = newPosition;                    
                    break;

                case "MiddlePhalanx2Angle":
                    if (newPosition > middleFingerOldPosition[1])
                        cGL.middleFingerAngle[1] += 10;
                    else
                        cGL.middleFingerAngle[1] -= 10;

                    middleFingerOldPosition[1] = newPosition;
                    break;

                case "MiddlePhalanx3Angle":
                    if (newPosition > middleFingerOldPosition[2])
                        cGL.middleFingerAngle[2] += 10;
                    else
                        cGL.middleFingerAngle[2] -= 10;

                    middleFingerOldPosition[2] = newPosition;
                    break;
            }

            cGL.movingPart = 2;
            cGL.CreateHandList();
            cGL.Draw();
        }

        private void RingFingerAngle_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newPosition = (int)numericSender.Value;

            switch (numericSender.Name)
            {
                case "RingPhalanx1Angle":
                    if (newPosition > ringFingerOldPosition[0])
                        cGL.ringFingerAngle[0] += 10;
                    else
                        cGL.ringFingerAngle[0] -= 10;

                    ringFingerOldPosition[0] = newPosition;
                    break;

                case "RingPhalanx2Angle":
                    if (newPosition > ringFingerOldPosition[1])
                        cGL.ringFingerAngle[1] += 10;
                    else
                        cGL.ringFingerAngle[1] -= 10;

                    ringFingerOldPosition[1] = newPosition;
                    break;

                case "RingPhalanx3Angle":
                    if (newPosition > ringFingerOldPosition[2])
                        cGL.ringFingerAngle[2] += 10;
                    else
                        cGL.ringFingerAngle[2] -= 10;

                    ringFingerOldPosition[2] = newPosition;
                    break;
            }

            cGL.movingPart = 3;
            cGL.CreateHandList();
            cGL.Draw();
        }

        private void PinkyFingerAngle_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newPosition = (int)numericSender.Value;

            switch (numericSender.Name)
            {
                case "PinkyPhalanx1Angle":
                    if (newPosition > pinkyFingerOldPosition[0])
                        cGL.pinkyFingerAngle[0] += 10;
                    else
                        cGL.pinkyFingerAngle[0] -= 10;

                    pinkyFingerOldPosition[0] = newPosition;
                    break;

                case "PinkyPhalanx2Angle":
                    if (newPosition > pinkyFingerOldPosition[1])
                        cGL.pinkyFingerAngle[1] += 10;
                    else
                        cGL.pinkyFingerAngle[1] -= 10;

                    pinkyFingerOldPosition[1] = newPosition;
                    break;

                case "PinkyPhalanx3Angle":
                    if (newPosition > pinkyFingerOldPosition[2])
                        cGL.pinkyFingerAngle[2] += 10;
                    else
                        cGL.pinkyFingerAngle[2] -= 10;

                    pinkyFingerOldPosition[2] = newPosition;
                    break;
            }

            cGL.movingPart = 4;
            cGL.CreateHandList();
            cGL.Draw();
        }

        private void ThumbFingerAngle_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newPosition = (int)numericSender.Value;

            switch (numericSender.Name)
            {
                case "ThumbPhalanx1Angle":
                    if (newPosition > thumbFingerOldPosition[0])
                        cGL.thumbFingerAngle[0] += 10;
                    else    
                        cGL.thumbFingerAngle[0] -= 10;

                    thumbFingerOldPosition[0] = newPosition;
                    break;

                case "ThumbPhalanx2Angle":
                    if (newPosition > thumbFingerOldPosition[1])
                        cGL.thumbFingerAngle[1] += 10;
                    else
                        cGL.thumbFingerAngle[1] -= 10;
;
                    thumbFingerOldPosition[1] = newPosition;
                    break;             
            }

            cGL.movingPart = 5;
            cGL.CreateHandList();
            cGL.Draw();
        }

        private void LightPositionX_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newValue = (int)numericSender.Value;
            cGL.lightPosition[0] = newValue;
            cGL.Draw();
        }

        private void LightPositionY_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newValue = (int)numericSender.Value;
            cGL.lightPosition[1] = newValue;
            cGL.Draw();
        }

        private void LightPositionZ_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newValue = (int)numericSender.Value;
            cGL.lightPosition[2] = newValue;
            cGL.Draw();
        }

        private void CoordSystemX_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newValue = (int)numericSender.Value;

            if (cGL.coordSysNewPosition[0] < newValue)
            {
                cGL.coordSysNewPosition[0] += 0.25f;
                cGL.coordSysMoveDirection = 1;
            }
            else
            {
                cGL.coordSysNewPosition[0] -= 0.25f;
                cGL.coordSysMoveDirection = -1;
            }              

            cGL.Draw();
            cGL.coordSysMoveDirection = 0;
            cGL.coordSysNewPosition[0] = newValue;
        }

        private void CoordSystemY_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newValue = (int)numericSender.Value;

            if (cGL.coordSysNewPosition[1] < newValue)
            {
                cGL.coordSysNewPosition[1] += 0.25f;
                cGL.coordSysMoveDirection = 2;
            }
            else
            {
                cGL.coordSysNewPosition[1] -= 0.25f;
                cGL.coordSysMoveDirection = -2;
            }

            cGL.Draw();
            cGL.coordSysMoveDirection = 0;
            cGL.coordSysNewPosition[1] = newValue;
        }

        private void CoordSystemZ_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newValue = (int)numericSender.Value;

            if (cGL.coordSysNewPosition[2] < newValue)
            {
                cGL.coordSysNewPosition[2] += 0.25f;
                cGL.coordSysMoveDirection = 3;
            }
            else
            {
                cGL.coordSysNewPosition[2] -= 0.25f;
                cGL.coordSysMoveDirection = -3;
            }
                
            cGL.Draw();
            cGL.coordSysMoveDirection = 0;
            cGL.coordSysNewPosition[2] = newValue;
        }

        private void CoordSystemRotationX_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newValue = (int)numericSender.Value;

            if (cGL.coordSysRotationNewPosition[0] < newValue)
            {
                cGL.coordSysRotationNewPosition[0] += 0.25f;
                cGL.coordSysMoveDirection = 4;
            }
            else
            {
                cGL.coordSysRotationNewPosition[0] -= 0.25f;
                cGL.coordSysMoveDirection = -4;
            }

            cGL.Draw();
            cGL.coordSysMoveDirection = 0;
            cGL.coordSysRotationNewPosition[0] = newValue;
        }

        private void CoordSystemRotationY_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newValue = (int)numericSender.Value;

            if (cGL.coordSysRotationNewPosition[1] < newValue)
            {
                cGL.coordSysRotationNewPosition[1] += 0.25f;
                cGL.coordSysMoveDirection = 5;
            }
            else
            {
                cGL.coordSysRotationNewPosition[1] -= 0.25f;
                cGL.coordSysMoveDirection = -5;
            }

            cGL.Draw();
            cGL.coordSysMoveDirection = 0;
            cGL.coordSysRotationNewPosition[1] = newValue;
        }

        private void CoordSystemRotationZ_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericSender = (NumericUpDown)sender;
            int newValue = (int)numericSender.Value;

            if (cGL.coordSysRotationNewPosition[2] < newValue)
            {
                cGL.coordSysRotationNewPosition[2] += 0.25f;
                cGL.coordSysMoveDirection = 6;
            }
            else
            {
                cGL.coordSysRotationNewPosition[2] -= 0.25f;
                cGL.coordSysMoveDirection = -6;
            }

            cGL.Draw();
            cGL.coordSysMoveDirection = 0;
            cGL.coordSysRotationNewPosition[2] = newValue;
        }

        private void EnableLightCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox enableLightCheckSender = (CheckBox)sender;

            if (enableLightCheckSender.Checked)
                cGL.enableLightSource = true;
            else
                cGL.enableLightSource = false;

            cGL.Draw();
        }

        private void EnableDefaultCoordSystemCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox enableDefaultCoordSystemCheckSender = (CheckBox)sender;

            if (enableDefaultCoordSystemCheckSender.Checked)
                cGL.enableDefaultCoordSystem = true;
            else
                cGL.enableDefaultCoordSystem = false;

            cGL.Draw();
        }

        private void EnableLookAtCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox enableLootAtCheckSender = (CheckBox)sender;

            if (enableLootAtCheckSender.Checked)
                cGL.enableLookAtValue = true;
            else
                cGL.enableLookAtValue = false;

            cGL.Draw();
        }

        private void LookAtValue_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown newValue = (NumericUpDown)sender;

            switch (newValue.Name)
            {
                case "LookAtEyeX":
                    cGL.LookAtNumberValue[0] = (int)newValue.Value;
                    break;

                case "LookAtEyeY":
                    cGL.LookAtNumberValue[1] = (int)newValue.Value;
                    break;

                case "LookAtEyeZ":
                    cGL.LookAtNumberValue[2] = (int)newValue.Value;
                    break;

                case "LookAtCenterX":
                    cGL.LookAtNumberValue[3] = (int)newValue.Value;
                    break;

                case "LookAtCenterY":
                    cGL.LookAtNumberValue[4] = (int)newValue.Value;
                    break;

                case "LookAtCenterZ":
                    cGL.LookAtNumberValue[5] = (int)newValue.Value;
                    break;

                case "LookAtUpX":
                    cGL.LookAtNumberValue[6] = (int)newValue.Value;
                    break;

                case "LookAtUpY":
                    cGL.LookAtNumberValue[7] = (int)newValue.Value;
                    break;

                case "LookAtUpZ":
                    cGL.LookAtNumberValue[8] = (int)newValue.Value;
                    break;
            }

            cGL.Draw();
        }

        private void EnableReflectionCheckBox_Checked(object sender, EventArgs e)
        {
            CheckBox enableReflectionCheckSender = (CheckBox)sender;

            if (enableReflectionCheckSender.Checked)
                cGL.enableReflection = true;
            else
                cGL.enableReflection = false;

            cGL.Draw();
        }
    }
}