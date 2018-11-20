using MiscUtil.Conversion;
using MiscUtil.UI;
using StqMessageParser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RtkViewer
{
    public partial class ConfigureRtkModeForm : Form
    {
        public ConfigureRtkModeForm()
        {
            InitializeComponent();
        }

        private DeviceInformation deviceInfo = null;
        private GpsSerial gps = null;
        public void SetDeviceInfoAndGpsSerial(DeviceInformation di, GpsSerial gs) { deviceInfo = di; gps = gs; }
        private bool alreadyConfigRtkMode = false;
        public bool AlreadyConfigRtkMode() { return alreadyConfigRtkMode; }
        private enum Step
        {
            StepRtkMode,
            StepRtkRover1_0,
            //StepRtkRover1_1,
            StepRtkBase1_0,
            StepRtkBase1_1,
            StepRtkBase1_2,
            StepRtkBase1_3,
            StepRtkBase1_4,
            StepRtkBase1_5,
        }
        private Step step = Step.StepRtkMode;

        private void ConfigureRtkModeForm_Load(object sender, EventArgs e)
        {
            FormTools.OnLoadResize(this);
            CenterToParent();

            step = Step.StepRtkMode;
            UpdatePanel();
            InitialPanelField();
        }

        private void InitialPanelField()
        {
            //rtkBase1_2Panel
            GpsSerial.RtkModeInfo rtkInfo = deviceInfo.GetRtkInfo();
            double lon, lat, epdH;
            if (rtkInfo.savedLat != 0)
            {
                lat = rtkInfo.savedLat;
                lon = rtkInfo.savedLon;
                epdH = rtkInfo.savedAlt;
            }
            else
            {
                lat = MessageParser.GetParsingStatus().GetLatitudeDegree();
                lon = MessageParser.GetParsingStatus().GetLongitudeDegree();
                epdH = MessageParser.GetParsingStatus().GetEllipsoidalHeight();
            }
            int lat2D = 0, lat3M = 0, lon2D = 0, lon3M = 0;
            double lat2M = 0, lat3S = 0, lon2M = 0, lon3S = 0;

            MiscConverter.ConvertDegToDegMin(lat, ref lat2D, ref lat2M);
            MiscConverter.ConvertDegToDegMin(lon, ref lon2D, ref lon2M);
            MiscConverter.ConvertDegToDegMinSec(lat, ref lat2D, ref lat3M, ref lat3S);
            MiscConverter.ConvertDegToDegMinSec(lon, ref lon2D, ref lon3M, ref lon3S);

            latDeg1Txt.Text = lat.ToString("F7");
            latDeg2Txt.Text = lat2D.ToString();
            latDeg3Txt.Text = lat2D.ToString();
            lonDeg1Txt.Text = lon.ToString("F7");
            lonDeg2Txt.Text = lon2D.ToString();
            lonDeg3Txt.Text = lon2D.ToString();

            latMin2Txt.Text = lat2M.ToString("F6");
            latMin3Txt.Text = lat3M.ToString();
            lonMin2Txt.Text = lon2M.ToString("F6");
            lonMin3Txt.Text = lon3M.ToString();

            latSec3Txt.Text = lat3S.ToString("F5");
            lonSec3Txt.Text = lon3S.ToString("F5");

            latNs1Cmb.SelectedIndex = (lat < 0) ? 1 : 0;
            latNs2Cmb.SelectedIndex = (lat < 0) ? 1 : 0;
            latNs3Cmb.SelectedIndex = (lat < 0) ? 1 : 0;
            lonEw1Cmb.SelectedIndex = (lon < 0) ? 1 : 0;
            lonEw2Cmb.SelectedIndex = (lon < 0) ? 1 : 0;
            lonEw3Cmb.SelectedIndex = (lon < 0) ? 1 : 0;

            eplHTxt.Text = epdH.ToString("F2");

            //rtkBase1_4Panel
            base1_4TimerLbl.Text = base1_4SurveyTime.ToString();

            //rtkBase1_5Panel
            base1_5TimerLbl.Text = 0.ToString();
        }

        private void UpdatePanel()
        {
            Panel p = rtkModePanel;
            rtkModePanel.Visible = false;
            rtkRover1_0Panel.Visible = false;
            rtkBase1_0Panel.Visible = false;
            rtkBase1_1Panel.Visible = false;
            rtkBase1_2Panel.Visible = false;
            rtkBase1_3Panel.Visible = false;
            rtkBase1_4Panel.Visible = false;
            rtkBase1_5Panel.Visible = false;

            if (step == Step.StepRtkMode)
            {
                p = rtkModePanel;
            }
            if (step == Step.StepRtkBase1_0)
            {
                p = rtkBase1_0Panel;
            }
            if (step == Step.StepRtkBase1_1)
            {
                p = rtkBase1_1Panel;
            }
            if (step == Step.StepRtkBase1_2)
            {
                p = rtkBase1_2Panel;
            }
            if(step == Step.StepRtkBase1_3)
            {
                p = rtkBase1_3Panel;
            }
            if (step == Step.StepRtkBase1_4)
            {
                p = rtkBase1_4Panel;
            }
            if (step == Step.StepRtkBase1_5)
            {
                p = rtkBase1_5Panel;
            }
            if (step == Step.StepRtkRover1_0)
            {
                p = rtkRover1_0Panel;
            }
            p.Left = 0;
            p.Top = 0;
            p.Visible = true;
        }

        private void rtkModeNext_Click(object sender, EventArgs e)
        {
            if (baseModeRdo.Checked)
            {
                step = Step.StepRtkBase1_0;
            }
            else
            {
                step = Step.StepRtkRover1_0;
            }
            UpdatePanel();
        }

        private void rtkBase1_1BackBtn_Click(object sender, EventArgs e)
        {
            step = Step.StepRtkBase1_0;
            UpdatePanel();
        }

        private void rtkBase1_1NextBtn_Click(object sender, EventArgs e)
        {
            if(baseMode1Rdo.Checked)
            {
                step = Step.StepRtkBase1_2;
                UpdatePanel();
            }
            else if (baseMode2Rdo.Checked)
            {
                step = Step.StepRtkBase1_3;
                UpdatePanel();
            }
            else if (baseMode3Rdo.Checked)
            {
                step = Step.StepRtkBase1_4;
                base1_4TimerLbl.Text = base1_4SurveyTime.ToString();
            }
            else if (baseMode4Rdo.Checked)
            {
                step = Step.StepRtkBase1_5;
            }

            if(step == Step.StepRtkBase1_5 || step == Step.StepRtkBase1_4)
            {
                surveyCount = 0;
                llaList.Clear();
                countTimer.Start();
                UpdatePanel();
            }
        }

        private void rtkBase1_2BackBtn_Click(object sender, EventArgs e)
        {
            step = Step.StepRtkBase1_1;
            UpdatePanel();
        }

        private double staticLatitude = 0;
        private double staticLongitude = 0;
        private float staticEllipsoidalH = 0;
        public double GetStaticLatitude() { return staticLatitude; }
        public double GetStaticLongitude() { return staticLongitude; }
        public float GetStaticEllipsoidalH() { return staticEllipsoidalH; }
        private void rtkBase1_2AcceptBtn_Click(object sender, EventArgs e)
        {
            if (coorFmt1Rdo.Checked)
            {
                staticLatitude = Convert.ToDouble(latDeg1Txt.Text);
                staticLongitude = Convert.ToDouble(lonDeg1Txt.Text);
            }
            else if (coorFmt2Rdo.Checked)
            {
                staticLatitude = MiscConverter.ConvertDegMinToDeg(Convert.ToInt32(latDeg2Txt.Text), Convert.ToDouble(latMin2Txt.Text));
                staticLongitude = MiscConverter.ConvertDegMinToDeg(Convert.ToInt32(lonDeg2Txt.Text), Convert.ToDouble(lonMin2Txt.Text));
            }
            else if (coorFmt3Rdo.Checked)
            {
                staticLatitude = MiscConverter.ConvertDegMinSecToDeg(Convert.ToInt32(latDeg3Txt.Text), Convert.ToInt32(latMin3Txt.Text), Convert.ToDouble(latSec3Txt.Text));
                staticLongitude = MiscConverter.ConvertDegMinSecToDeg(Convert.ToInt32(lonDeg3Txt.Text), Convert.ToInt32(lonMin3Txt.Text), Convert.ToDouble(lonSec3Txt.Text));
            }
            staticEllipsoidalH = (float)Convert.ToDouble(eplHTxt.Text);
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void rtkBase1_3BackBtn_Click(object sender, EventArgs e)
        {
            step = Step.StepRtkBase1_1;
            UpdatePanel();
        }

        private void rtkBase1_3AcceptBtn_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("The RTK base mode configuration is completed.");
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void rtkBase1_4BackBtn_Click(object sender, EventArgs e)
        {
            countTimer.Stop();
            step = Step.StepRtkBase1_1;
            UpdatePanel();
        }

        private void rtkBase1_4AcceptBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void rtkBase1_4Panel_VisibleChanged(object sender, EventArgs e)
        {
            if(!(sender as Panel).Visible)
            {
                countTimer.Stop();
            }
        }

        private int surveyCount = 0;
        private int base1_4SurveyTime = 60;
        private int base1_5SurveyCount = 120;
        private List<DrawScatter.LLA> llaList = new List<DrawScatter.LLA>();

        private void countTimer_Tick(object sender, EventArgs e)
        {
            ++surveyCount;
            ParsingStatus ps = MessageParser.GetParsingStatus();

            if (step == Step.StepRtkBase1_4)
            {
                if (ps.IsBetterThanPositionFix3D())
                {
                    llaList.Add(new DrawScatter.LLA(
                        ps.GetLatitudeDegree(),
                        ps.GetLongitudeDegree(),
                        (float)ps.GetEllipsoidalHeight()));
                }

                base1_4TimerLbl.Text = (base1_4SurveyTime - surveyCount).ToString();
                UpdateBase1_4Cooridate();
                if (surveyCount == base1_4SurveyTime)
                {
                    countTimer.Stop();
                    if (llaList.Count == 0)
                    {
                        MessageBox.Show("Not enough coordinates to config");
                    }
                    else
                    {
                        double lat = 0, lon = 0, alt = 0;
                        CalcLlaListAverage(ref lat, ref lon, ref alt);

                        gps.UninstallDataReceiver();
                        GpsSerial.RtkModeInfo r = new GpsSerial.RtkModeInfo();
                        r.rtkMode = GpsSerial.RtkModeInfo.RtkMode.RTK_Base;
                        r.optMode = GpsSerial.RtkModeInfo.RtkOperationMode.Base_Static;
                        r.savedLat = lat;
                        r.savedLon = lon;
                        r.savedAlt = (float)alt;
                        GpsSerial.GPS_RESPONSE rep = gps.ConfigureRtkMode(2000, r, GpsSerial.Attributes.SramAndFlash);
                        if (rep != GpsSerial.GPS_RESPONSE.ACK)
                        {
                            MessageBox.Show("Configure RTK mode failed");
                        }
                        else
                        {
                            MessageBox.Show("Configure RTK mode successfully");
                        }
                        gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                        alreadyConfigRtkMode = true;
                        rtkBase1_4AcceptBtn.Enabled = true;
                        rtkBase1_4BackBtn.Enabled = false;
                    }
                }
            }
            else
            {
                if (ps.GetFixedMode() == ParsingStatus.FixedMode.FixRTK)
                {
                    llaList.Add(new DrawScatter.LLA(
                        ps.GetLatitudeDegree(),
                        ps.GetLongitudeDegree(),
                        (float)ps.GetEllipsoidalHeight()));
                }

                base1_5TimerLbl.Text = surveyCount.ToString();
                UpdateBase1_5Cooridate();
                if (llaList.Count >= base1_5SurveyCount)
                {
                    countTimer.Stop();

                    double lat = 0, lon = 0, alt = 0;
                    CalcLlaListAverage(ref lat, ref lon, ref alt);

                    gps.UninstallDataReceiver();
                    GpsSerial.RtkModeInfo r = new GpsSerial.RtkModeInfo();
                    r.rtkMode = GpsSerial.RtkModeInfo.RtkMode.RTK_Base;
                    r.optMode = GpsSerial.RtkModeInfo.RtkOperationMode.Base_Static;
                    r.savedLat = lat;
                    r.savedLon = lon;
                    r.savedAlt = (float)alt;
                    GpsSerial.GPS_RESPONSE rep = gps.ConfigureRtkMode(2000, r, GpsSerial.Attributes.SramAndFlash);
                    if (rep != GpsSerial.GPS_RESPONSE.ACK)
                    {
                        MessageBox.Show("Configure RTK mode failed");
                    }
                    else
                    {
                        MessageBox.Show("Configure RTK mode successfully");
                    }
                    gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                    alreadyConfigRtkMode = true;
                    rtkBase1_5AcceptBtn.Enabled = true;
                    rtkBase1_5BackBtn.Enabled = false;
                }
            }
            
        }

        private void UpdateBase1_4Cooridate()
        {
            validCountBase1_4Lbl.Text = llaList.Count.ToString();
            if (llaList.Count < 2)
                return;

            double avgLat = 0, avgLon = 0, avgAlt = 0;
            CalcLlaListAverage(ref avgLat, ref avgLon, ref avgAlt);

            latBase1_4Lbl.Text = avgLat.ToString("F8");
            lonBase1_4Lbl.Text = avgLon.ToString("F8");
            eplHBase1_4Lbl.Text = avgAlt.ToString("F2");
        }

        private void CalcLlaListAverage(ref double lat, ref double lon, ref double alt)
        {
            lat = llaList[0].GetLatitude();
            lon = llaList[0].GetLontitude();
            alt = llaList[0].GetAltitude();
            for (int i = 1; i < llaList.Count; ++i)
            {
                lat = (lat + (llaList[i].GetLatitude() / i)) * ((double)i / (i + 1));
                lon = (lon + (llaList[i].GetLontitude() / i)) * ((double)i / (i + 1));
                alt = (alt + (llaList[i].GetAltitude() / i)) * ((double)i / (i + 1));
            }
        }

        private void UpdateBase1_5Cooridate()
        {
            validCountBase1_5Lbl.Text = llaList.Count.ToString();
            if (llaList.Count < 2)
                return;

            double avgLat = 0, avgLon = 0, avgAlt = 0;
            CalcLlaListAverage(ref avgLat, ref avgLon, ref avgAlt);

            latBase1_5Lbl.Text = avgLat.ToString("F9");
            lonBase1_5Lbl.Text = avgLon.ToString("F9");
            eplHBase1_5Lbl.Text = avgAlt.ToString("F3");
        }

        private void rtkBase1_5BackBtn_Click(object sender, EventArgs e)
        {
            countTimer.Stop();
            step = Step.StepRtkBase1_1;
            UpdatePanel();
        }

        private void rtkBase1_5AcceptBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void rtkRover1_1BackBtn_Click(object sender, EventArgs e)
        {
            step = Step.StepRtkRover1_0;
            UpdatePanel();
        }

        public enum RtkMode
        {
            Base,
            Rover,
        }
        public enum MessageType
        {
            NoData = 0,
            NMEA = 1,
            Binary = 3,
            UAV = 4,
            RAW,
            RTCM,
        }

        public RtkMode GetRtkMode()
        {
            return ((baseModeRdo.Checked) ? RtkMode.Base : RtkMode.Rover);
        }

        public MessageType GetMessageType()
        {
            if (GetRtkMode() == RtkMode.Rover)
            {
                return (rtkRover1_1Data0Rdo.Checked) ? MessageType.NMEA : MessageType.UAV;
            }
            else
            {
                return (rtkBase1_0Data1Rdo.Checked) ? MessageType.RAW : MessageType.RTCM;
            }
        }

        private void rtkRover1_1AcceptBtn_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("The RTK rover mode configuration is completed.");

        }

        private void rtkBase1_5Panel_VisibleChanged(object sender, EventArgs e)
        {
            if (!(sender as Panel).Visible)
            {
                countTimer.Stop();
            }
        }

        private void rtkBase1_0BackBtn_Click(object sender, EventArgs e)
        {
            step = Step.StepRtkMode;
            UpdatePanel();
        }

        private void rtkBase1_0NextBtn_Click(object sender, EventArgs e)
        {
            step = Step.StepRtkBase1_1;
            UpdatePanel();
        }

        private void rtkRover1_0BackBtn_Click(object sender, EventArgs e)
        {
            step = Step.StepRtkMode;
            UpdatePanel();
        }

        private void rtkRover1_0AccaptBtn_Click(object sender, EventArgs e)
        {
            //step = Step.StepRtkRover1_1;
            //UpdatePanel();
            DialogResult = DialogResult.OK;
            this.Close();
        }

        public enum RtkBaseOptModeSelection
        {
            FixedCoor,
            SurveyCoor,
            GnssCoor,
            RtkCoor
        }
        private RtkBaseOptModeSelection baseOptSel = RtkBaseOptModeSelection.FixedCoor;
        public RtkBaseOptModeSelection GetRtkBaseOptModeSelection() { return baseOptSel; }
        private void baseMode1Rdo_CheckedChanged(object sender, EventArgs e)
        {
            baseOptSel = RtkBaseOptModeSelection.FixedCoor;
        }

        private void baseMode2Rdo_CheckedChanged(object sender, EventArgs e)
        {
            baseOptSel = RtkBaseOptModeSelection.SurveyCoor;
        }

        private void baseMode3Rdo_CheckedChanged(object sender, EventArgs e)
        {
            baseOptSel = RtkBaseOptModeSelection.GnssCoor;
        }

        private void baseMode4Rdo_CheckedChanged(object sender, EventArgs e)
        {
            baseOptSel = RtkBaseOptModeSelection.RtkCoor;
        }

        private void latDMSTxt_TextChanged(object sender, EventArgs e)
        {
            UpdateLlaStatus();
        }

        private void lonDMSTxt_TextChanged(object sender, EventArgs e)
        {
            UpdateLlaStatus();
        }

        private void latDMTxt_TextChanged(object sender, EventArgs e)
        {
            UpdateLlaStatus();
        }

        private void lonDMTxt_TextChanged(object sender, EventArgs e)
        {
            UpdateLlaStatus();
        }

        private void latDTxt_TextChanged(object sender, EventArgs e)
        {
            UpdateLlaStatus();
        }

        private void lonDTxt_TextChanged(object sender, EventArgs e)
        {
            UpdateLlaStatus();
        }

        private void UpdateLlaStatus()
        {
            bool allOk = true;
            if (coorFmt1Rdo.Checked)
            {
                allOk = ValidateLlaString(new string[] { latDeg1Txt.Text }, new int[] { 90 },
                    new TextBox[] { latDeg1Txt }) && allOk;
                allOk = ValidateLlaString(new string[] { lonDeg1Txt.Text }, new int[] { 180 },
                    new TextBox[] { lonDeg1Txt }) && allOk;

            }
            else if (coorFmt2Rdo.Checked)
            {
                allOk = ValidateLlaString(new string[] { latDeg2Txt.Text, latMin2Txt.Text }, new int[] { 90, 60 },
                    new TextBox[] { latDeg2Txt, latMin2Txt }) && allOk;
                allOk = ValidateLlaString(new string[] { lonDeg2Txt.Text, lonMin2Txt.Text }, new int[] { 180, 60 },
                    new TextBox[] { lonDeg2Txt, lonMin2Txt }) && allOk;
            }
            else if (coorFmt3Rdo.Checked)
            {
                allOk = ValidateLlaString(new string[] { latDeg3Txt.Text, latMin3Txt.Text, latSec3Txt.Text }, 
                    new int[] { 90, 60, 60 }, new TextBox[] { latDeg3Txt, latMin3Txt, latSec3Txt }) && allOk;
                allOk = ValidateLlaString(new string[] { lonDeg3Txt.Text, lonMin3Txt.Text, lonSec3Txt.Text },
                    new int[] { 180, 60, 60 }, new TextBox[] { lonDeg3Txt, lonMin3Txt, lonSec3Txt }) && allOk;
            }   

            try
            {
                double h = Convert.ToDouble(eplHTxt.Text);
                eplHTxt.ForeColor = Color.Black;
            }
            catch
            {
                allOk = false;
                eplHTxt.ForeColor = Color.Red;
            }
            rtkBase1_2AcceptBtn.Enabled = allOk;
        }

        private void coorFmtRdo_CheckedChanged(object sender, EventArgs e)
        {
            if (coorFmt1Rdo.Checked && !coorFmt1Panel.Visible)
            {
                coorFmt1Panel.Visible = true;
                coorFmt2Panel.Visible = false;
                coorFmt3Panel.Visible = false;
            }
            else if (coorFmt2Rdo.Checked && !coorFmt2Panel.Visible)
            {
                coorFmt1Panel.Visible = false;
                coorFmt2Panel.Visible = true;
                coorFmt3Panel.Visible = false;
            }
            else if (coorFmt3Rdo.Checked && !coorFmt3Panel.Visible)
            {
                coorFmt1Panel.Visible = false;
                coorFmt2Panel.Visible = false;
                coorFmt3Panel.Visible = true;
            }
            UpdateLlaStatus();
        }

        private bool ValidateLlaString(string[] s, int[] max, TextBox[] t)
        {
            int i = 0;
            bool v = false;
            bool allPass = true;
            for(i = 0; i < s.Length; ++i)
            {
                v = false;
                if (i != s.Length - 1)
                {
                    int ii = 0;
                    try
                    {
                        ii = Convert.ToInt32(s[i]);
                        if (ii >= 0 && ii < max[i])
                        {
                            v = true;
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    double dd = 0;
                    try
                    {
                        dd = Convert.ToDouble(s[i]);
                        if (dd >= 0 && dd < max[i])
                        {
                            v = true;
                        }
                    }
                    catch
                    {
                    }
                }
                t[i].ForeColor = (v) ? Color.Black : Color.Red;
                allPass = allPass && v;
            }
            return allPass;
        }

        private void eplHTxt_TextChanged(object sender, EventArgs e)
        {
            UpdateLlaStatus();
        }
    }
}
