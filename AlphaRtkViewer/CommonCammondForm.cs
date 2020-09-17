using MiscUtil.UI;
using System;
using System.Windows.Forms;

namespace RtkViewer
{
    public partial class CommonCammondForm : Form
    {
        public CommonCammondForm()
        {
            InitializeComponent();
        }

        public enum Mode
        {
            SetFactoryDefault,
            ConfigureSerialPort,
            QuerySoftwareInformation,
            QueryRtkMode,
            ConfigurePositioningUpdateRate,
            ConfigureVeryLowSpeedFilter,
        }

        private Mode mode = Mode.SetFactoryDefault;
        private DeviceInformation deviceInfo = null;
        public Mode GetMode()
        {
            return mode;
        }
        
        public void SetMode(Mode value)
        {
            mode = value;
        }

        public void SetDeviceInfo(DeviceInformation di)
        {
            deviceInfo = di;
        }

        public void CommonCammondForm_Load(object sender, EventArgs e)
        {
            FormTools.OnLoadResize(this);
            CenterToParent();
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            Panel p = null;
            String title = "";
            setFactoryDefaultPanel.Visible = false;
            configureSerialPortPanel.Visible = false;
            queryRtkModePanel.Visible = false;
            configurePositionUpdateRatePanel.Visible = false;
            switch (mode)
            {
                case Mode.SetFactoryDefault:
                    p = setFactoryDefaultPanel;
                    title = "Set Factory Default";
                    break;
                case Mode.ConfigureSerialPort:
                    p = configureSerialPortPanel;
                    title = "Configure Serial Port";
                    break;
                case Mode.QueryRtkMode:
                    p = queryRtkModePanel;
                    title = "Query RTK Mode";
                    GpsSerial.RtkModeInfo r = deviceInfo.GetRtkInfo();
                    rtkModeLbl.Text = r.GetRtkModeString(r.rtkMode);
                    savedRtkOpLbl.Text = r.GetOperationModeString(r.optMode);
                    runtimeRtkOpLbl.Text = r.GetOperationModeString(r.runtimeOptMode);
                    savedRtkLatLbl.Text = r.savedLat.ToString("F7");
                    savedRtkLonLbl.Text = r.savedLon.ToString("F7");
                    savedRtkAltLbl.Text = r.savedAlt.ToString("F2");
                    break;
                case Mode.ConfigurePositioningUpdateRate:
                    p = configurePositionUpdateRatePanel;
                    title = "Configure Positioning Update Rate";
                    break;
                case Mode.ConfigureVeryLowSpeedFilter:
                    p = configureVeryLowSpeedPanel;
                    title = "Configure Very Low Speed Filter";
                    break;
                default:
                    break;
            }
            p.Left = 0;
            p.Top = 0;
            p.Visible = true;
            this.Text = title;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void queryRtkModeOkBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int selectedBaudrate = 115200;
        public int GetSelectedBaudrate() { return selectedBaudrate; }
        private void baudrateRadio_clicked(object sender, EventArgs e)
        {
            if((sender as RadioButton).Checked)
                selectedBaudrate = Convert.ToInt32((sender as RadioButton).Text);
        }

        private void configureSerialPortAcceptBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void configureSerialPortCancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void configurePositionUpdateRateAcceptBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void configurePositionUpdateRateCancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void configureMessageTypeRoverAcceptBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Configure Message Type successful.");
            this.Close();

        }

        private void configureMessageTypeRoverCancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void queryVersionOkBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void setFactoryDefaultOkBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void configureMessageTypeBase1_1AcceptBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Configure Message Type successful.");
            this.Close();
        }

        private void configureMessageTypeBase1_2AcceptBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Configure Message Type successful.");
            this.Close();
        }

        private void configureMessageTypeBase1AcceptBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Configure Message Type successful.");
            this.Close();
        }

        private void configureMessageTypeBase1CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int selectedUpdateRate = 1;
        public int GetSelectedUpdateRate() { return selectedUpdateRate; }
        private void updateRateRadio_clicked(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                selectedUpdateRate = Convert.ToInt32((sender as RadioButton).Text.Split(' ')[0]);
            }
        }

        private void configureVeryLowSpeedAcceptBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void configureVeryLowSpeedCancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool veryLowSpeed = false;
        public bool GetVeryLowSpeedFilter() { return veryLowSpeed; }
        private void veryLowDisableRdo_CheckedChanged(object sender, EventArgs e)
        {
            veryLowSpeed = false;
        }

        private void veryLowEnableRdo_CheckedChanged(object sender, EventArgs e)
        {
            veryLowSpeed = true;
        }
    }
}
