using System;

using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using MiscUtil.UI;
using StqMessageParser;
using System.Text;
using System.Collections.Generic;
using MiscUtil.Network;
using System.Threading;
using System.ComponentModel;
using System.Xml;
using MiscUtil.App;
using MiscUtil.IO;

namespace RtkViewer
{
    public partial class AlphaView : Form
    {
        public AlphaView()
        {
            InitializeComponent();
        }

        private int snr2X = 0;
        private int snr2Y = 0;
        private int snr3X = 0;
        private int snr3Y = 0;
        private void AlphaView_Load(object sender, EventArgs e)
        {
            Text = AppTools.GetAppTitleWithVersion();

            ClearAllControls(ClearControlsType.ClearAll);
            CalcLayoutParameter();
            InitComPortCmb();
            InitBaudRateCmb();
            UpdateConnectionBtn();
            UpdateFixStatus(ParsingStatus.FixedMode.None);
            BackupSnrPanelPosition();

            gpDrawSnrBar = new DrawSnrBar(Color.FromArgb(255, (byte)255, (byte)255, (byte)204),
                    "gpActImg", "gpDisImg", "gpSnrTitle",
                    Color.FromArgb(255, (byte)61, (byte)179, (byte)255),
                    Color.FromArgb(255, 26, 144, 255));
            bdDrawSnrBar = new DrawSnrBar(Color.FromArgb(255, (byte)204, (byte)255, (byte)255),
                    "bdActImg", "bdDisImg", "bdSnrTitle",
                    Color.FromArgb(255, (byte)255, (byte)153, (byte)1),
                    Color.FromArgb(255, 255, 153, 1));
            glDrawSnrBar = new DrawSnrBar(Color.FromArgb(255, (byte)204, (byte)255, (byte)204),
                    "glActImg", "glDisImg", "glSnrTitle",
                    Color.FromArgb(255, (byte)195, (byte)128, (byte)255),
                    Color.FromArgb(255, 156, 102, 204));
            drawEarth = new DrawEarth("gpActImg", "gpDisImg", "glActImg", "glDisImg", "bdActImg", "bdDisImg", "giActImg", "giDisImg");
            drawScatter = new DrawScatter();
            InitScatterScaleCmb();
            
            if (Properties.Settings.Default.autoCheckAppUpdate)
            {
                CheckSoftwareUpdate(true);
            }
        }

        private BackgroundWorker bwCheckFtp = new BackgroundWorker();
        private bool CheckSoftwareUpdate(bool autoClose)
        {
            CheckAppUpdateForm form = new CheckAppUpdateForm(Properties.Settings.Default.autoCheckAppUpdate, autoClose);
            DialogResult dr = form.ShowDialog(this);

            Properties.Settings.Default.autoCheckAppUpdate = form.GetCheckBoxValue();
            Properties.Settings.Default.Save();

            if (dr != DialogResult.OK)
            {   //User cancel update
                return false;
            }

            string appInfoXml = form.GetAppInfoXml();
            AppTools.GetRtkViewerDownloadFolder(true);
            string updaterPath = AppTools.GetRtkViewerUpdaterPath();
            if (AppTools.CreateMutex(AppTools.RtkViewerUpdaterMutexName))
            {
                MessageBox.Show("The updater is already in progress, please wait for the updater to complete.");
                return false;
            }
            if (AppTools.ExtractEmbeddedFile("RtkViewer.RtkViewerUpdater.exe", updaterPath))
            {
                string base64 = Convert.ToBase64String(Encoding.GetEncoding("utf-8").GetBytes(appInfoXml));
                AppTools.LaunchExe(updaterPath, string.Format("\"{0}\"", base64));
                Close();
            }
            return true;
        }

        private const int DBT_DEVTYP_HANDLE = 6;
        private const int DBT_DEVTYP_PORT = 3;
        private const int BROADCAST_QUERY_DENY = 0x424D5144;
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000; // system detected a new device(any program can disable the removal)
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004; // removed 
        private const int DBT_DEVTYP_VOLUME = 0x00000002; // drive type is logical volume
        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_PORT
        {
            public int dbcp_size;
            public int dbcp_devicetype;
            public int dbcp_reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] dbcp_name;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);        // call default p
            if (m.Msg != WM_DEVICECHANGE)
            {
                return;
            }
            // WM_DEVICECHANGE can have several meanings depending on the WParam value...
            int msgType = m.WParam.ToInt32();
            if (msgType != DBT_DEVICEARRIVAL && msgType != DBT_DEVICEREMOVECOMPLETE)
            {
                return;
            }

            int devType = Marshal.ReadInt32(m.LParam, 4);
            if (DBT_DEVTYP_PORT != devType)
            {
                return;
            }

            DEV_BROADCAST_PORT vol = (DEV_BROADCAST_PORT)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_PORT));
            int step = (vol.dbcp_name[1] == 0x00) ? 2 : 1;
            StringBuilder sb = new StringBuilder(8);
            for (int i = 0; i < vol.dbcp_name.Length; i += step)
            {
                if (vol.dbcp_name[i] == 0x00)
                {
                    break;
                }
                sb.Append(vol.dbcp_name[i]);
            }

            if (msgType == DBT_DEVICEARRIVAL)
            {
                ListBoxUtil.AddStringToListBoxAndScrollToBottom(responseLsb, sb.ToString() + " plugged-in");
            }
            else
            {
                ListBoxUtil.AddStringToListBoxAndScrollToBottom(responseLsb, sb.ToString() + " removed");
            }

            //The currently connected COM port has been removed.
            if (sb.ToString() == comPortCmb.Text && connected)
            {
                DoDisconnection();
                MessageBox.Show("The device has been removed and the connection is cancelled!", "Warning");
            }
            InitComPortCmb();
            UpdateConnectionBtn();
        }

        private enum ClearControlsType
        {
            ClearAll,
            Restart,
            Reconnection,
        }

        private void ClearAllControls(ClearControlsType t)
        {
            ttffLbl.Text = "";

            dateLbl.Text = "";
            timeLbl.Text = "";
            latitudeLbl.Text = "";
            longitudeLbl.Text = "";
            mslAltLbl.Text = "";
            elpHLbl.Text = "";

            if (t != ClearControlsType.Restart)
            {
                opModeLbl.Text = "";
                kVerMLbl.Text = "";
                sVerMLbl.Text = "";
                revisionMLbl.Text = "";
                crcMLbl.Text = "";
                kVerSLbl.Text = "";
                sVerSLbl.Text = "";
                revisionSLbl.Text = "";
                crcSLbl.Text = "";
                rtkActLbl.Text = "";
                licPeriodLbl.Text = "";
            }

            messageLsb.Items.Clear();
            if (t == ClearControlsType.ClearAll)
            {
                responseLsb.Items.Clear();
            }

            directionLbl.Text = "";
            speedLbl.Text = "";
            hdopLbl.Text = "";
            vdopLbl.Text = "";
            wgs84xLbl.Text = "";
            wgs84yLbl.Text = "";
            wgs84zLbl.Text = "";
            pdopLbl.Text = "";
            rtkAgeLbl.Text = "";
            rtkRatioLbl.Text = "";
            baselineLenLbl.Text = "";
            baselineCouLbl.Text = "";
            cyckeSlipLbl.Text = "";
            easeProjLbl.Text = "";
            northProjLbl.Text = "";
            upProjLbl.Text = "";
        }

        //Form Layout
        private int xGapL, xGap1, xGap2, xGapR;
        private int xGapA2;
        private int yGapT, yGap1, yGap2, yGap3, yGap4, yGapB;
        private int yGapA1, yGapA2;
        private void CalcLayoutParameter()
        {
            int totalW = this.ClientSize.Width;
            int totalH = this.ClientSize.Height - viewerPanel.Top;
            xGapL = viewerPanel.Left;
            xGap1 = messagePanel.Left - viewerPanel.Left - viewerPanel.Width;
            xGap2 = responsePanel.Left - messagePanel.Left - messagePanel.Width;
            xGapR = totalW - responsePanel.Left - responsePanel.Width;
            xGapA2 = earthPanel.Left - snr2Pbox.Left - snr2Pbox.Width;
            yGapT = viewerPanel.Top;
            yGap1 = infoPanel.Top - messagePanel.Top - messagePanel.Height;
            yGap2 = snr1Pbox.Top - infoPanel.Top - infoPanel.Height;
            yGap3 = snr2Pbox.Top - snr1Pbox.Top - snr1Pbox.Height;
            yGap4 = snr3Pbox.Top - snr2Pbox.Top - snr2Pbox.Height;
            yGapA1 = scatterPanel.Top - responsePanel.Top - responsePanel.Height;
            yGapA2 = earthPanel.Top - scatterPanel.Top - scatterPanel.Height;
            yGapB = totalH - viewerPanel.Top - viewerPanel.Height;
        }

        private void AlphaView_SizeChanged(object sender, EventArgs e)
        {
            int totalW = (sender as Form).ClientSize.Width;
            int totalH = (sender as Form).ClientSize.Height - viewerPanel.Top;

            int w = (totalW - viewerPanel.Width - xGapL - xGap1 - xGap2 - xGapR) / 2;
            int h = totalH - yGapT - yGap1 - infoPanel.Height - yGap2 - snr1Pbox.Height - yGap3 -
                    snr2Pbox.Height - yGap4 - snr3Pbox.Height - yGapB;
            viewerPanel.Height = totalH - yGapT - yGapB;
            messagePanel.Width = w;
            messagePanel.Height = h;
            responsePanel.Width = w;
            responsePanel.Height = h;
            responsePanel.Left = messagePanel.Left + xGap1 + messagePanel.Width;

            w = totalW - xGapL - viewerPanel.Width - xGap1 - xGapA2 - scatterPanel.Width;
            infoPanel.Width = w;
            snr1Pbox.Width = w;
            snr2Pbox.Width = w;
            snr3Pbox.Width = w;

            int y = messagePanel.Top + messagePanel.Height + yGap1;
            infoPanel.Top = y;
            y += infoPanel.Height + yGap2;
            snr1Pbox.Top = y;
            y += snr1Pbox.Height + yGap3;
            snr2Pbox.Top = y;
            y += snr2Pbox.Height + yGap4;
            snr3Pbox.Top = y;

            int x = infoPanel.Left + infoPanel.Width + xGapA2;
            y = responsePanel.Top + responsePanel.Height + yGapA1;
            scatterPanel.Left = x;
            scatterPanel.Top = y;
            y += scatterPanel.Height + yGapA2;
            earthPanel.Left = x;
            earthPanel.Top = y;

            BackupSnrPanelPosition();
            SwitchSnrPanel();

            earthPbox.Invalidate();
            snr1Pbox.Invalidate();
            snr2Pbox.Invalidate();
            snr3Pbox.Invalidate();
        }

        private void BackupSnrPanelPosition()
        {
            snr2X = snr2Pbox.Left;
            snr2Y = snr2Pbox.Top;
            snr3X = snr3Pbox.Left;
            snr3Y = snr3Pbox.Top;
        }

        private void messagePanel_SizeChanged(object sender, EventArgs e)
        {
            int totalW = (sender as Panel).Width - messageLsb.Left * 2 - 2;
            int totalH = (sender as Panel).Height - messageLsb.Top - 4;
            messageLsb.Width = totalW;
            messageLsb.Height = totalH;
        }

        private void responsePanel_SizeChanged(object sender, EventArgs e)
        {
            int totalW = (sender as Panel).Width - responseLsb.Left * 2 - 2;
            int totalH = (sender as Panel).Height - responseLsb.Top - 4;
            responseLsb.Width = totalW;
            responseLsb.Height = totalH;
        }

        //Get COM port list
        private void InitComPortCmb()
        {
            SerialTool.ReflashComList();
            comPortCmb.Items.Clear();
            int lastSelected = Properties.Settings.Default.lastSelectedPort;
            int select = -1;
            for (int i = 0; i < SerialTool.GetPortCount(); ++i)
            {
                comPortCmb.Items.Add(SerialTool.GetPortName(i));
                if(lastSelected != 0 && SerialTool.GetPortNumber(i) == lastSelected)
                {
                    select = i;
                }
            }
            if (select != -1)
                comPortCmb.SelectedIndex = select;
            else
                comPortCmb.SelectedIndex = comPortCmb.Items.Count - 1;
        }

        ToolTip portCmbTt = new ToolTip();
        private void comPortCmb_MouseHover(object sender, EventArgs e)
        {
            if (comPortCmb.SelectedIndex == -1)
                return;

            portCmbTt.ToolTipTitle = comPortCmb.Text;
            portCmbTt.UseFading = false;
            portCmbTt.UseAnimation = false;
            portCmbTt.IsBalloon = false;
            portCmbTt.ShowAlways = true;
            portCmbTt.AutoPopDelay = 10000;
            portCmbTt.InitialDelay = 0;
            portCmbTt.ReshowDelay = 0;
            portCmbTt.SetToolTip(comPortCmb, SerialTool.GetPortDescription(comPortCmb.SelectedIndex));
        }

        private void comPortCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            connectBtn.Enabled = (comPortCmb.SelectedIndex >= 0);
        }

        //Baud Rate
        private void InitBaudRateCmb()
        {
            baudRateCmb.Items.Clear();
            int lastSelected = Properties.Settings.Default.lastSelectedBaud;
            for (int i = 0; i < SerialTool.GetBaudRateCount(); ++i)
            {
                baudRateCmb.Items.Add(SerialTool.GetBaudRateByIndex(i).ToString());
            }
            baudRateCmb.SelectedIndex = lastSelected;
        }

        private void InitScatterScaleCmb()
        {
            for(int i = 0; i < drawScatter.GetScaleCount(); ++i)
            {
                double f = drawScatter.GetScale(i);
                if (f < 0.1F)
                    scaleCmb.Items.Add(f.ToString("F2") + "m");
                else if (f < 1F)
                    scaleCmb.Items.Add(f.ToString("F1") + "m");
                else
                    scaleCmb.Items.Add(f.ToString("F0") + "m");
            }
            scaleCmb.SelectedIndex = Properties.Settings.Default.lastScatterScaleSelected;
        }

        private void baudRateCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (baudRateCmb.SelectedIndex >= 0)
            //{
            //    //Properties.Settings.Default.lastSelectedBaud = baudRateCmb.SelectedIndex;
            //    //Properties.Settings.Default.Save();
            //}
        }

        //Connect and Disconnect
        GpsSerial gps = GpsSerial.Default;
        DeviceInformation deviceInfo = null;
        private void DoDisconnection()
        {
            connected = false;
            deviceInfo.StopMessageParser();
            gps.Close();
            UpdateConnectionBtn();
            if (ttffTimer.Enabled)
            {
                TerminateTTFFCount();
            }
        }

        private bool connected = false;
        private bool GetFirmwareConfiguration()
        {
            if (deviceInfo != null)
            {
                deviceInfo.Close();
                deviceInfo = null;
            }

            deviceInfo = new DeviceInformation(gps);
            if(soForm != null)
            {
                deviceInfo.EnableParserSaveDeviceOutput(true);
            }

            DetectingDeviceForm form = new DetectingDeviceForm(gps, deviceInfo);
            DialogResult dr = form.ShowDialog();
            if (dr != DialogResult.OK)
            {
                gps.Close();
                UpdateConnectionBtn();
                MessageBox.Show("User cancel");
                return false;
            }

            if (deviceInfo.GetFinalStage() == DeviceInformation.FinalStage.Device_Error)
            {
                gps.Close();
                UpdateConnectionBtn();
                MessageBox.Show("Unable to find supported device");
                return false;
            }

            if (deviceInfo.GetFinalStage() == DeviceInformation.FinalStage.Device_Not_Support)
            {
                gps.Close();
                UpdateConnectionBtn();
                MessageBox.Show("Unable to find supported device");
                return false;
            }

            if (deviceInfo.GetFinalStage() == DeviceInformation.FinalStage.None)
            {
                gps.Close();
                UpdateConnectionBtn();
                MessageBox.Show("Detection is not complete!");
                return false;
            }

            if (deviceInfo.GetFinalStage() == DeviceInformation.FinalStage.Rom_Mode)
            {
                MessageBox.Show("You are in ROM mode!");
            }

            if(deviceInfo.IsAlphaStartKitFirmware())
            {
                rtkActLbl.Visible = false;
                licPeriodLbl.Visible = false;
            }

            SwitchSnrPanel();
            return true;
        }

        private void DoConnection(bool getFwInfo)
        {
            if (!gps.Open(SerialTool.GetPortName(comPortCmb.SelectedIndex), baudRateCmb.SelectedIndex))
            {
                MessageBox.Show(gps.GetErrorMessage());
                return;
            }

            if (getFwInfo)
            {
                if (!GetFirmwareConfiguration())
                {
                    return;
                }
            }
            ResetConnection(ClearControlsType.Reconnection);
            UpdateConnectionBtn();
            FillInFirmwareInfo();
            gps.ClearInBuffer();
            gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
            deviceInfo.RunMessageParser(ParsingResultHandler);
            connected = true;
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (gps.SerialPort.IsOpen)
                {   //Disconnection
                    DoDisconnection();
                    return;
                }
                Properties.Settings.Default.lastSelectedBaud = baudRateCmb.SelectedIndex;
                Properties.Settings.Default.lastSelectedPort = SerialTool.GetPortNumber(comPortCmb.SelectedIndex);
                Properties.Settings.Default.Save();

                //Connection
                DoConnection(true);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private float ttff = 0;
        private void StartTTFFCount()
        {
            ttff = 0;
            ttffLbl.ForeColor = Color.Red;
            UpdateTTFF();
            ttffTimer.Start();
        }

        private void StopTTFFCount()
        {
            ttffTimer.Stop();
            ttffLbl.ForeColor = Color.Blue;
            UpdateTTFF();
        }

        private void TerminateTTFFCount()
        {
            ttffTimer.Stop();
            UpdateTTFF();
        }

        private void ttffTimer_Tick(object sender, EventArgs e)
        {
            ttff += 1F;
            UpdateTTFF();
        }

        private void UpdateTTFF()
        {
            ttffLbl.Text = ttff.ToString("F0");
        }

        private void FillInFirmwareInfo()
        {
            if(deviceInfo.IsDrFirmware())
            {
                opModeLbl.Text = "ODR";
                opModeLbl.ForeColor = Color.Navy;
            }
            else if(deviceInfo.IsRtkBaseMode())
            {
                opModeLbl.Text = "RTK Base " + ((deviceInfo.IsGlonassModule()) ? "GPS + GLONASS" : "GPS + BEIDOU");
                opModeLbl.ForeColor = (deviceInfo.IsGlonassModule() ? Color.BlueViolet : Color.DarkOrange);
            }
            else if (deviceInfo.IsRtkRoverMode())
            {
                opModeLbl.Text = "RTK Rover " + ((deviceInfo.IsGlonassModule()) ? "GPS + GLONASS" : "GPS + BEIDOU");
                opModeLbl.ForeColor = (deviceInfo.IsGlonassModule() ? Color.BlueViolet : Color.DarkOrange);
            }
            else if (deviceInfo.GetFinalStage() == DeviceInformation.FinalStage.Rom_Mode)
            {
                opModeLbl.Text = "ROM Mode";
                opModeLbl.ForeColor = Color.Red;
            }

            kVerMLbl.Text = deviceInfo.GetFormatKernelVersion(false);
            sVerMLbl.Text = deviceInfo.GetFormatSoftwareVersion(false);
            revisionMLbl.Text = deviceInfo.GetFormatRevision(false);
            crcMLbl.Text = deviceInfo.GetFormatCrc(false);

            if (!deviceInfo.IsRtkBaseMode() && !(deviceInfo.GetFinalStage() == DeviceInformation.FinalStage.Rom_Mode))
            {
                kVerSLbl.Text = deviceInfo.GetFormatKernelVersion(true);
                sVerSLbl.Text = deviceInfo.GetFormatSoftwareVersion(true);
                revisionSLbl.Text = deviceInfo.GetFormatRevision(true);
                crcSLbl.Text = deviceInfo.GetFormatCrc(true);
            }
            else
            {
                kVerSLbl.Text = "";
                sVerSLbl.Text = "";
                revisionSLbl.Text = "";
                crcSLbl.Text = "";
            }
            UpdateLicenseStatus();
        }

        private void SwitchSnrPanel()
        {
            if (deviceInfo == null)
                return;
            int x = snr3Pbox.Left;
            int y = snr3Pbox.Top;
            if (deviceInfo.IsBeidouModule())
            {
                snr2Pbox.Left = snr3X;
                snr2Pbox.Top = snr3Y;
                snr3Pbox.Left = snr2X;
                snr3Pbox.Top = snr2Y;
            }
            else
            {
                snr2Pbox.Left = snr2X;
                snr2Pbox.Top = snr2Y;
                snr3Pbox.Left = snr3X;
                snr3Pbox.Top = snr3Y;
            }
        }

        public void ParsingResultHandler(object sender, MessageParser.ParsingResultArgs args)
        {
            if (messageLsb.InvokeRequired)
            {
                MessageParser.ParsingResultHandler ph = new MessageParser.ParsingResultHandler(ParsingResultHandler);
                //When InvokeRequired is true, it means that it is on a different thread.
                if (args.parsingResult == ParsingResult.SaveDeviceOutput || args.showString?.Length > 0)
                {
                    Invoke(ph, messageLsb, args);
                    return;
                }
            }
            if(args.parsingResult == ParsingResult.SaveDeviceOutput)
            {
                if(soForm != null && soForm.Visible)
                {
                    soForm.SaveOutput(args.deviceOutput);
                }
                else
                {
                    deviceInfo.EnableParserSaveDeviceOutput(false);
                }
                return;
            }

            ParsingStatus ps = MessageParser.GetParsingStatus();
            if ((args.parsingResult & ParsingResult.UpdateGpSateInfo) != 0)
            {
                snr1Pbox.Invalidate();
            }
            if ((args.parsingResult & ParsingResult.UpdateGlSateInfo) != 0)
            {
                snr2Pbox.Invalidate();
            }
            if ((args.parsingResult & ParsingResult.UpdateBdSateInfo) != 0)
            {
                snr3Pbox.Invalidate();
            }
            if((args.parsingResult & ParsingResult.UpdateGpSateInfo) != 0 ||
                (args.parsingResult & ParsingResult.UpdateGlSateInfo) != 0 ||
                (args.parsingResult & ParsingResult.UpdateBdSateInfo) != 0)
            {
                earthPbox.Invalidate();
            }
            if ((args.parsingResult & ParsingResult.UpdateTime) != 0)
            {
                timeLbl.Text = ps.GetFormatTime();
            }
            if ((args.parsingResult & ParsingResult.UpdateLatitude) != 0)
            {
                latitudeLbl.Text = ps.GetFormatLatitudeDMS();
            }
            if ((args.parsingResult & ParsingResult.UpdateLongitude) != 0)
            {
                longitudeLbl.Text = ps.GetFormatLongitudeDMS();
            }
            if ((args.parsingResult & ParsingResult.UpdateMslAltitude) != 0)
            {
                mslAltLbl.Text = string.Format("{0:0.00} M", ps.GetMslAltitude());
                elpHLbl.Text = string.Format("{0:0.00} M", ps.GetEllipsoidalHeight());
            }
            if ( MessageParser.GetParsingStatus().GetFixedMode() != ParsingStatus.FixedMode.None &&
                ((args.parsingResult & ParsingResult.UpdateLongitude) != 0 || 
                (args.parsingResult & ParsingResult.UpdateLatitude) != 0 ||
                (args.parsingResult & ParsingResult.UpdateMslAltitude) != 0 ))
            {
                DrawScatter.LLA lla = drawScatter?.AddLla(MessageParser.GetParsingStatus().GetLatitudeDegree(),
                    MessageParser.GetParsingStatus().GetLongitudeDegree(),
                    (float)MessageParser.GetParsingStatus().GetEllipsoidalHeight());
                if(lla != null)
                {
                    wgs84xLbl.Text = lla.wgs_x.ToString("F3");
                    wgs84yLbl.Text = lla.wgs_y.ToString("F3");
                    wgs84zLbl.Text = lla.wgs_z.ToString("F3");
                }
                scatterPbox.Invalidate();
                drawEarth.SetEarthCenter(ps.GetLatitudeDegree(), ps.GetLongitudeDegree());
            }

            if ((args.parsingResult & ParsingResult.UpdateElliposidalH) != 0)
            {
                elpHLbl.Text = string.Format("{0:0.00} M", ps.GetEllipsoidalHeight());
            }
            if ((args.parsingResult & ParsingResult.UpdatePdop) != 0)
            {
                pdopLbl.Text = ps.GetPdop().ToString("F2");
            }
            if ((args.parsingResult & ParsingResult.UpdateHdop) != 0)
            {
                hdopLbl.Text = ps.GetHdop().ToString("F2");
            }
            if ((args.parsingResult & ParsingResult.UpdateVdop) != 0)
            {
                vdopLbl.Text = ps.GetVdop().ToString("F2");
            }
            if ((args.parsingResult & ParsingResult.UpdateSpeed) != 0)
            {
                speedLbl.Text = ps.GetSpeedKmHr().ToString("F2") + " km/h";
            }
            if ((args.parsingResult & ParsingResult.UpdateDirection) != 0)
            {
                directionLbl.Text = ps.GetDirection().ToString("F2") + "°";
            }
            if ((args.parsingResult & ParsingResult.UpdateDate) != 0)
            {
                dateLbl.Text = ps.GetFormatDate();
                UpdateLicenseStatus();
            }
            if ((args.parsingResult & ParsingResult.UpdateFixMode) != 0)
            {
                UpdateFixStatus(ps.GetFixedMode());
                if(ps.GetFixedMode() != ParsingStatus.FixedMode.None)
                {
                    if(ttffTimer.Enabled)
                        StopTTFFCount();
                }
                else
                {
                    //ParsingStatus.FixedMode fm = ps.GetFixedMode();
                    if (!ttffTimer.Enabled)
                        StartTTFFCount();
                }
            }
            if ((args.parsingResult & ParsingResult.UpdateRtkAge) != 0)
            {
                rtkAgeLbl.Text = ps.GetRtkAge().ToString("F1") + " sec(s)";
            }
            if ((args.parsingResult & ParsingResult.UpdateRtkRatio) != 0)
            {
                rtkRatioLbl.Text = ps.GetRtkRatio().ToString("F1");
            }
            if ((args.parsingResult & ParsingResult.UpdateRtkEastProj) != 0)
            {
                easeProjLbl.Text = ps.GetRtkEastProj().ToString("F2") + " m";
            }
            if ((args.parsingResult & ParsingResult.UpdateRtkNorthProj) != 0)
            {
                northProjLbl.Text = ps.GetRtkNorthProj().ToString("F2") + " m";
            }
            if ((args.parsingResult & ParsingResult.UpdateRtkUpProj) != 0)
            {
                upProjLbl.Text = ps.GetRtkUpProj().ToString("F2") + " m";
            }
            if ((args.parsingResult & ParsingResult.UpdateRtkBaselineLen) != 0)
            {
                baselineLenLbl.Text = ps.GetRtkBaselineLen().ToString("F2") + " m";
            }
            if ((args.parsingResult & ParsingResult.UpdateRtkBaselineCour) != 0)
            {
                baselineCouLbl.Text = ps.GetRtkBaselineCour().ToString("F2") + "°";
            }
            if ((args.parsingResult & ParsingResult.UpdateRtkCycleSlip) != 0)
            {
                cyckeSlipLbl.Text = ps.GetFormatRtkCycleSlip();
            }
                















            ListBoxUtil.AddStringToListBoxAndScrollToBottom(messageLsb, args.showString);
        }

        private void UpdateLicenseStatus()
        {
            if(deviceInfo.GetFinalStage() == DeviceInformation.FinalStage.Rom_Mode)
            {
                rtkActLbl.Text = "";
                licPeriodLbl.Text = "";
                rtkActLbl.ForeColor = Color.Blue;
                licPeriodLbl.ForeColor = Color.Blue;
                return;
            }

            if (deviceInfo.GetLicenseType() == DeviceInformation.LicenseType.Perpetual)
            {
                rtkActLbl.Text = "Perpetual License";
                licPeriodLbl.Text = "";
                rtkActLbl.ForeColor = Color.Blue;
                licPeriodLbl.ForeColor = Color.Blue;
            }
            else if (deviceInfo.GetLicenseType() == DeviceInformation.LicenseType.Monthly)
            {
                rtkActLbl.Text = "Monthly License";
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}/{1}/{2} ", deviceInfo.GetMiscStartY(), deviceInfo.GetMiscStartM(), deviceInfo.GetMiscStartD());
                sb.AppendFormat(" ~ {0}/{1}/{2}", deviceInfo.GetMiscEndY(), deviceInfo.GetMiscEndM(), deviceInfo.GetMiscEndD());
                licPeriodLbl.Text = sb.ToString();

                DateTime utc = MessageParser.GetParsingStatus().GetDateTime();
                if (deviceInfo.IsValidateMiscTime(utc))
                {
                    rtkActLbl.ForeColor = Color.Black;
                    licPeriodLbl.ForeColor = Color.Black;
                }
                else
                {
                    rtkActLbl.ForeColor = Color.Blue;
                    licPeriodLbl.ForeColor = Color.Blue;
                }
            }
            else
            {
                rtkActLbl.Text = "License Required";
                licPeriodLbl.Text = "";
                rtkActLbl.ForeColor = Color.Red;
                licPeriodLbl.ForeColor = Color.Red;
            }
        }

        void EnableSettingMenu(bool enable)
        {
            if (enable && (deviceInfo.GetFinalStage() != DeviceInformation.FinalStage.Rom_Mode))
            {
                settingsToolStripMenuItem.Enabled = true;
            }
            else
            {
                settingsToolStripMenuItem.Enabled = false;
            }
        }

        private void UpdateConnectionBtn()
        {
            if (gps.SerialPort.IsOpen)
            {
                connectBtn.Text = "Disconnect";
                connectBtn.BackColor = Color.Red;
                comPortCmb.Enabled = false;
                baudRateCmb.Enabled = false;
                hotStartBtn.Enabled = true;
                coldStartBtn.Enabled = true;
                setOriginBtn.Enabled = true;
                clearBtn.Enabled = true;
                //Viewer menu
                enableRTKFunctionToolStripMenuItem.Enabled = deviceInfo.IsAlphaFirmware();
                //Setting menu
                EnableSettingMenu(true);
                //Updates
                updatesToolStripMenuItem.Enabled = true;
                changeFirmwareConstellationTypeToolStripMenuItem.Enabled = (deviceInfo.GetFinalStage() != DeviceInformation.FinalStage.Rom_Mode);
            }
            else
            {
                connectBtn.Text = "Connect";
                connectBtn.BackColor = Color.Green;
                comPortCmb.Enabled = true;
                baudRateCmb.Enabled = true;
                hotStartBtn.Enabled = false;
                coldStartBtn.Enabled = false;
                setOriginBtn.Enabled = false;
                clearBtn.Enabled = false;
                //Viewer menu
                enableRTKFunctionToolStripMenuItem.Enabled = false;
                //Setting menu
                EnableSettingMenu(false);
                //Updates
                updatesToolStripMenuItem.Enabled = false;
            }

            if(comPortCmb.Items.Count == 0 || comPortCmb.SelectedIndex == -1)
            {
                connectBtn.Enabled = false;
            }
        }

        //Menu Items
        private void checkSoftwareUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckSoftwareUpdate(false);
        }

        private void enableRTKFunctionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[] serialNo = new byte[8];
            gps.UninstallDataReceiver();
            GpsSerial.GPS_RESPONSE rep = gps.QuerySerialNumber(2000, ref serialNo);
            SetResponseMessage(rep, "Query Serial Number");

            if (rep != GpsSerial.GPS_RESPONSE.ACK)
            {
                gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                MessageBox.Show("Query Serial Number failed!");
                return;
            }

            EnableRTKFunctionForm form = new EnableRTKFunctionForm();
            form.SetSerialNumber(MiscUtil.Conversion.MiscConverter.GetSerialNumberString(serialNo));
            DialogResult dr = form.ShowDialog();

            if(dr != DialogResult.OK)
            {
                gps.ClearInBuffer();
                gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                return;
            }

            rep = gps.SetLicenseKey(2000, form.GetLicenseKey());
            SetResponseMessage(rep, "Set License");

            if (rep != GpsSerial.GPS_RESPONSE.ACK)
            {
                gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                MessageBox.Show("Set License failed!");
                return;
            }
            DoDisconnection();
            DoConnection(true);
        }

        private void setFactoryDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonCammondForm form = new CommonCammondForm();
            form.SetMode(CommonCammondForm.Mode.SetFactoryDefault);
            DialogResult dr = form.ShowDialog();
            if (dr != DialogResult.OK || !connected)
            {
                return;
            }

            gps.UninstallDataReceiver();
            GpsSerial.GPS_RESPONSE rep = gps.SetFactoryDefaults(2000);
            SetResponseMessage(rep, "Set Factory Defaults");
            
            if(rep != GpsSerial.GPS_RESPONSE.ACK)
            {
                gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                MessageBox.Show("Set Factory Defaults failed!");
            }

            //Alpha default baud rate
            baudRateCmb.SelectedIndex = SerialTool.GetIndexOfBaudRate(115200);
            DoDisconnection();
            DoConnection(true);
        }

        private void configureOutputBaudRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonCammondForm form = new CommonCammondForm();
            form.SetMode(CommonCammondForm.Mode.ConfigureSerialPort);
            DialogResult dr = form.ShowDialog();

            if(dr != DialogResult.OK)
            {
                return;
            }

            int baudrateIdx = SerialTool.GetIndexOfBaudRate(form.GetSelectedBaudrate());
            if ( baudrateIdx < 0)
            {
                MessageBox.Show("Unsupported baud rate");
            }

            gps.UninstallDataReceiver();
            GpsSerial.GPS_RESPONSE rep = gps.ConfigureSerialPort(2000, baudrateIdx, GpsSerial.Attributes.SramAndFlash);
            SetResponseMessage(rep, "Configure Output Baud Rate");

            if (rep != GpsSerial.GPS_RESPONSE.ACK)
            {
                gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                MessageBox.Show("Configure Output Baud Rate failed!");
            }

            baudRateCmb.SelectedIndex = baudrateIdx;
            DoDisconnection();
            DoConnection(false);
        }

        private void queryFirmwareInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonCammondForm form = new CommonCammondForm();
            form.SetMode(CommonCammondForm.Mode.QuerySoftwareInformation);
            form.ShowDialog();
        }

        private void queryRTKModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gps.UninstallDataReceiver();
            GpsSerial.GPS_RESPONSE rep = deviceInfo.QueryRtkMode();
            SetResponseMessage(rep, "Query RTK Mode");

            gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
            if (rep != GpsSerial.GPS_RESPONSE.ACK)
            {
                MessageBox.Show("Query RTK Mode failed!");
                return;
            }

            CommonCammondForm form = new CommonCammondForm();
            form.SetMode(CommonCammondForm.Mode.QueryRtkMode);
            form.SetDeviceInfo(deviceInfo);
            form.ShowDialog();
        }

        private void configureOperationModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureRtkModeForm form = new ConfigureRtkModeForm();
            GpsSerial.GPS_RESPONSE rep;
            form.SetDeviceInfoAndGpsSerial(deviceInfo, gps);

            gps.UninstallDataReceiver();
            rep = gps.ConfigureRtkMode(2000, GpsSerial.RtkModeInfo.GetRoverNormalSetting(), GpsSerial.Attributes.Sram);
            if (rep == GpsSerial.GPS_RESPONSE.ACK)
            {
                SetResponseMessage(rep, "Temporarily change RTK mode for configuration");
            }
            else
            {
                SetResponseMessage(rep, "Change RTK mode failed");
            }
            gps.InstallDataReceiver(deviceInfo.GetDataInHandler());

            DialogResult dr = form.ShowDialog();
            if (dr != DialogResult.OK || !connected)
            {
                if (connected)
                {
                    bool restoreOk = false;
                    gps.UninstallDataReceiver();
                    rep = gps.SetRegister(2000, 0x50000000, 0x12345678);
                    if (rep == GpsSerial.GPS_RESPONSE.ACK)
                    {
                        rep = gps.SystemRestart(2000, GpsSerial.RestartMode.HotStart, 0, 0, 0);
                        if (rep == GpsSerial.GPS_RESPONSE.ACK)
                        {
                            restoreOk = true;
                        }
                    }

                    if (restoreOk)
                    {
                        SetResponseMessage(rep, "Restore RTK mode successfully");
                    }
                    else
                    {
                        SetResponseMessage(rep, "Restore RTK mode failed");
                    }
                    gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                }
                return;
            }

            ConfigureRtkModeForm.RtkMode mode = form.GetRtkMode();
            ConfigureRtkModeForm.MessageType type = form.GetMessageType();
            GpsSerial.RtkModeInfo rtkInfo = new GpsSerial.RtkModeInfo(deviceInfo.GetRtkInfo());
            gps.UninstallDataReceiver();
            if (mode == ConfigureRtkModeForm.RtkMode.Base)
            {
                if (!form.AlreadyConfigRtkMode())
                {
                    rtkInfo.rtkMode = GpsSerial.RtkModeInfo.RtkMode.RTK_Base;
                    if (form.GetRtkBaseOptModeSelection() == ConfigureRtkModeForm.RtkBaseOptModeSelection.FixedCoor ||
                        form.GetRtkBaseOptModeSelection() == ConfigureRtkModeForm.RtkBaseOptModeSelection.GnssCoor ||
                        form.GetRtkBaseOptModeSelection() == ConfigureRtkModeForm.RtkBaseOptModeSelection.RtkCoor)
                    {
                        rtkInfo.optMode = GpsSerial.RtkModeInfo.RtkOperationMode.Base_Static;
                        rtkInfo.savedLat = form.GetStaticLatitude();
                        rtkInfo.savedLon = form.GetStaticLongitude();
                        rtkInfo.savedAlt = form.GetStaticEllipsoidalH();
                    }
                    else
                    {
                        rtkInfo.optMode = GpsSerial.RtkModeInfo.RtkOperationMode.Base_Survey;
                        rtkInfo.surveyLength = 60;
                        rtkInfo.surveyStdDivThr = 30;
                    }
                    rep = gps.ConfigureRtkMode(2000, rtkInfo, GpsSerial.Attributes.SramAndFlash);
                    if (rep != GpsSerial.GPS_RESPONSE.ACK)
                    {
                        MessageBox.Show("Configure RTK Mode failed!");
                        SetResponseMessage(rep, "Configure RTK Mode failed!");
                        gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                        return;
                    }
                    SetResponseMessage(rep, "Configure RTK Mode");
                    Thread.Sleep(1000);
                }

                rep = GpsSerial.GPS_RESPONSE.NONE;
                if (form.GetMessageType() == ConfigureRtkModeForm.MessageType.RAW)
                {
                    GpsSerial.MeasurementDataOutSetting mdos = new GpsSerial.MeasurementDataOutSetting();
                    mdos.measTime = false;
                    mdos.rawMeas = false;
                    mdos.svChStatus = true;
                    mdos.rcvChStatus = true;
                    mdos.subFrame = GpsSerial.MeasurementDataOutSetting.GetSubFrameFlag(true, deviceInfo.IsGlonassModule(), deviceInfo.IsBeidouModule());
                    mdos.extRawMeas = true;
                    mdos.updaterateIdx = GpsSerial.GetIndexOfUpdateRate(deviceInfo.GetUpdateRate());
                    rep = gps.ConfigBinaryMeasurementDataOut(2000, mdos, GpsSerial.Attributes.SramAndFlash);
                }
                else
                {
                    GpsSerial.RtcmDataOutSetting rdos = new GpsSerial.RtcmDataOutSetting();
                    rdos.enable = true;
                    rdos.rtcm1005 = true;
                    rdos.rtcm1077= true;
                    rdos.rtcm1087= deviceInfo.IsGlonassModule();
                    rdos.rtcm1107 = true;
                    rdos.rtcm1117 = true;
                    rdos.rtcm1127 = deviceInfo.IsBeidouModule();
                    rdos.updaterateIdx = GpsSerial.GetIndexOfUpdateRate(deviceInfo.GetUpdateRate());
                    rep = gps.ConfigBinaryRtcmDataOut(2000, rdos, GpsSerial.Attributes.SramAndFlash);
                }
                if (rep != GpsSerial.GPS_RESPONSE.ACK)
                {
                    MessageBox.Show("Configure RTK Mode failed!");
                    SetResponseMessage(rep, "Configure RTK Mode failed!");
                    gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                    return;
                }
                SetResponseMessage(rep, "Configure RTK Mode");
            }
            else
            {
                rtkInfo.rtkMode = GpsSerial.RtkModeInfo.RtkMode.RTK_Rover;
                rtkInfo.optMode = GpsSerial.RtkModeInfo.RtkOperationMode.Rover_Normal;
                rtkInfo.baselineLength = 0;

                rep = gps.ConfigureRtkMode(2000, rtkInfo, GpsSerial.Attributes.SramAndFlash);
                SetResponseMessage(rep, "Configure Rtk Mode");
                if (rep != GpsSerial.GPS_RESPONSE.ACK)
                {
                    MessageBox.Show("Configure Rtk Mode failed!");
                    gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                    return;
                }

                Thread.Sleep(1000);
                rep = gps.ConfigureMessageType(2000, (int)type, GpsSerial.Attributes.SramAndFlash);
                SetResponseMessage(rep, "Configure Message Type");
                if (rep != GpsSerial.GPS_RESPONSE.ACK)
                {
                    MessageBox.Show("Configure Message Type failed!");
                    gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                    return;
                }
            }
            gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
        }

        private void changeFirmwareConstellationTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckFirmwareUpdateForm form = new CheckFirmwareUpdateForm();
            form.SetDeviceInfoAndGpsSerial(CheckFirmwareUpdateForm.Mode.ChangeConstellationType, deviceInfo, gps);
            gps.UninstallDataReceiver();
            DialogResult dr = form.ShowDialog();
            if(dr != DialogResult.OK)
            {
                if (connected)
                {
                    gps.ClearInBuffer();
                    gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                }
                return;
            }

            //Alpha firmware changed
            DoDisconnection();
            DoConnection(true);
        }

        private void checkFirmwareUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckFirmwareUpdateForm form = new CheckFirmwareUpdateForm();
            if (deviceInfo.GetFinalStage() == DeviceInformation.FinalStage.Rom_Mode)
            {
                form.SetDeviceInfoAndGpsSerial(CheckFirmwareUpdateForm.Mode.RomModeRecovery, deviceInfo, gps);
            }
            else
            {
                form.SetDeviceInfoAndGpsSerial(CheckFirmwareUpdateForm.Mode.UpdateFirmware, deviceInfo, gps);
            }

            gps.UninstallDataReceiver();
            DialogResult dr = form.ShowDialog();
            if (dr != DialogResult.OK)
            {
                if (connected)
                {
                    gps.ClearInBuffer();
                    gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                }
                return;
            }

            //Alpha firmware changed
            DoDisconnection();
            DoConnection(true);
        }

        private void polarisAlphaHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.polaris-gnss.com/");
        }

        private void configureUpdateRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonCammondForm form = new CommonCammondForm();
            form.SetMode(CommonCammondForm.Mode.ConfigurePositioningUpdateRate);
            DialogResult dr = form.ShowDialog();

            if (dr != DialogResult.OK || !connected)
            {
                return;
            }

            int updateRate = form.GetSelectedUpdateRate();
            if (updateRate <= 0)
            {
                MessageBox.Show("Unsupported update rate");
            }

            gps.UninstallDataReceiver();
            GpsSerial.GPS_RESPONSE rep = gps.ConfigureUpdateRate(2000, updateRate, GpsSerial.Attributes.SramAndFlash);
            SetResponseMessage(rep, "Configure Update Rate");

            gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
            if (rep != GpsSerial.GPS_RESPONSE.ACK)
            {
                MessageBox.Show("Configure Update Rate failed!");
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm form = new AboutForm();
            form.ShowDialog();
        }

        private void sDCardLogBackupToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //SdFormat();
            return;
        }

        //Demo codes for position fix status change
        private void UpdateFixStatus(ParsingStatus.FixedMode m)
        {
            string[] fixString = { "No Fix", "Pos. Fix 2D", "Pos. Fix 3D.", "DGPS", "DR Estimated", "Float RTK", "Fix RTK",};
            fixStatusLbl.Text = fixString[(int)m];
            switch (m)
            {
                case ParsingStatus.FixedMode.None:
                    fixStatusLbl.ForeColor = Color.Red;
                    break;
                case ParsingStatus.FixedMode.PositionFix2d:
                case ParsingStatus.FixedMode.PositionFix3d:
                case ParsingStatus.FixedMode.DgpsMode:
                    fixStatusLbl.ForeColor = Color.Blue;
                    break;
                case ParsingStatus.FixedMode.EstimatedMode:
                    fixStatusLbl.ForeColor = Color.Purple;
                    break;
                case ParsingStatus.FixedMode.FixRTK:
                    fixStatusLbl.ForeColor = Color.Green;
                    break;
                case ParsingStatus.FixedMode.FloatRTK:
                    fixStatusLbl.ForeColor = Color.Goldenrod;
                    break;
            }
        }

        private void AlphaView_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        //Test SD Format

        [DllImport("shell32.dll")]
        static extern uint SHFormatDrive(IntPtr hwnd, uint drive, uint fmtID, uint options);

        public enum SHFormatFlags : uint
        {
            SHFMT_ID_DEFAULT = 0xFFFF,
            SHFMT_OPT_FULL = 0x1,
            SHFMT_OPT_SYSONLY = 0x2,
            SHFMT_ERROR = 0xFFFFFFFF,
            SHFMT_CANCEL = 0xFFFFFFFE,
            SHFMT_NOFORMAT = 0xFFFFFFD,
        }

        private void SdFormat()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady && (d.DriveType == DriveType.Removable))
                {
                    uint drv = (uint)(d.Name[0] - 'A');
                    //(Drive letter : A is 0, Z is 25)
                    uint result = SHFormatDrive(this.Handle,
                                    drv, // formatting C:
                                    (uint)SHFormatFlags.SHFMT_ID_DEFAULT,
                                    (uint)SHFormatFlags.SHFMT_OPT_FULL); // full format of g:
                    if (result == (uint)SHFormatFlags.SHFMT_ERROR)
                        MessageBox.Show("Unable to format the drive");
                }
            }
        }

        private void scaleCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (scaleCmb.SelectedIndex >= 0)
            {
                Properties.Settings.Default.lastScatterScaleSelected = scaleCmb.SelectedIndex;
                Properties.Settings.Default.Save();
                drawScatter?.SetCurrentScale(scaleCmb.SelectedIndex);
                scatterPbox.Invalidate();
            }
        }

        private void AlphaView_FormClosing(object sender, FormClosingEventArgs e)
        {
            deviceInfo?.StopMessageParser();
            if (soForm != null && soForm.Visible)
            {
                soForm.Close();
                soForm = null;
            }
        }

        private DrawSnrBar gpDrawSnrBar = null;

        private DrawSnrBar glDrawSnrBar = null;

        private DrawSnrBar bdDrawSnrBar = null;

        private void snr1Pbox_Paint(object sender, PaintEventArgs e)
        {
            if (deviceInfo == null)
            {
                gpDrawSnrBar.DrawBg(e.Graphics, (sender as PictureBox).Width, (sender as PictureBox).Height);
                return;
            }
            gpDrawSnrBar.Draw(e.Graphics, MessageParser.GetParsingStatus().GetGpsSateListClone(), (sender as PictureBox).Width, (sender as PictureBox).Height);
        }

        private void snr2Pbox_Paint(object sender, PaintEventArgs e)
        {
            if (deviceInfo == null)
            {
                glDrawSnrBar.DrawBg(e.Graphics, (sender as PictureBox).Width, (sender as PictureBox).Height);
                return;
            }
            glDrawSnrBar.Draw(e.Graphics, MessageParser.GetParsingStatus().GetGlonassSateListClone(), (sender as PictureBox).Width, (sender as PictureBox).Height);
        }

        private void snr3Pbox_Paint(object sender, PaintEventArgs e)
        {
            if (deviceInfo == null)
            {
                bdDrawSnrBar.DrawBg(e.Graphics, (sender as PictureBox).Width, (sender as PictureBox).Height);
                return;
            }
            bdDrawSnrBar.Draw(e.Graphics, MessageParser.GetParsingStatus().GetBeidouSateListClone(), (sender as PictureBox).Width, (sender as PictureBox).Height);
        }

        private void queryUpdateRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gps.UninstallDataReceiver();
            GpsSerial.GPS_RESPONSE rep = deviceInfo.QueryUpdateRate();
            SetResponseMessage(rep, "Query Update Rate");

            gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
            if (rep != GpsSerial.GPS_RESPONSE.ACK)
            {
                gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
                MessageBox.Show("Query Update Rate failed!");
            }
            else
            {
                MessageBox.Show("Update Rate: " + deviceInfo.GetUpdateRate() + " Hz");
            }
        }

        private void copyResponseLsbMenuItem_Click(object sender, EventArgs e)
        {
            string s = "";
            foreach (object o in responseLsb.SelectedItems)
            {
                s += o.ToString() + "\r\n";
            }
            Clipboard.SetText(s);
        }

        private void configGlCpifMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void queryGlCpifMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void setOriginBtn_Click(object sender, EventArgs e)
        {
            drawScatter?.SetOrigin(MessageParser.GetParsingStatus().GetLatitudeDegree(),
                MessageParser.GetParsingStatus().GetLongitudeDegree(),
                (float)MessageParser.GetParsingStatus().GetEllipsoidalHeight());
            scatterPbox.Invalidate();
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            drawScatter?.ClearAllData();
            scatterPbox.Invalidate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void softwareHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.polaris-gnss.com/Alpha_RTK_Receiver_User_Guide.pdf");
        }

        private string saveDeviceOutputPath = "";
        private SaveOutputForm soForm = null;
        private void saveDeviceOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(null != soForm && soForm.Visible)
            {
                MessageBox.Show("The device output is already being stored.");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Output files|*.out|All files|*.*";
            sfd.Title = "Save Device Output";
            sfd.FileName = string.Format("Output{0}.out", 
                DateTime.Now.ToString("yyyy-MM-dd_hhmmss"));
            sfd.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (sfd.FileName != "")
            {
                FileTools.ExistDelete(sfd.FileName);
                saveDeviceOutputPath = sfd.FileName;

                if (null != soForm)
                {
                    soForm.Dispose();
                    soForm = null;
                }

                soForm = new SaveOutputForm();
                soForm.SetFile(sfd.FileName);
                soForm.Show();
                if (deviceInfo != null)
                {
                    deviceInfo.EnableParserSaveDeviceOutput(true);
                }
            }

            
        }

        private void clearResponseLsbMenuItem_Click(object sender, EventArgs e)
        {
            responseLsb.Items.Clear();
        }

        private DrawEarth drawEarth = null;
        private void earthPbox_Paint(object sender, PaintEventArgs e)
        {
            if(deviceInfo == null)
            {
                return;
            }
            if (deviceInfo.IsBeidouModule())
            {
                List<ParsingStatus.SateInfo>[] l = {
                    MessageParser.GetParsingStatus().GetBeidouSateListClone(),
                    MessageParser.GetParsingStatus().GetGpsSateListClone() };
                ParsingStatus.SateType[] t = { ParsingStatus.SateType.Beidou, ParsingStatus.SateType.Gps };
                drawEarth.Draw(e.Graphics, l, t);
            }
            else
            {
                List<ParsingStatus.SateInfo>[] l = {
                    MessageParser.GetParsingStatus().GetGlonassSateListClone(),
                    MessageParser.GetParsingStatus().GetGpsSateListClone() };
                ParsingStatus.SateType[] t = { ParsingStatus.SateType.Glonass, ParsingStatus.SateType.Gps };
                drawEarth.Draw(e.Graphics, l, t);
            }
        }

        private DrawScatter drawScatter = null;
        private void scatterPbox_Paint(object sender, PaintEventArgs e)
        {
            drawScatter.Draw(e.Graphics);
        }

        private void coldStartBtn_Click(object sender, EventArgs e)
        {
            gps.UninstallDataReceiver();
            GpsSerial.GPS_RESPONSE rep = gps.SystemRestart(2000, GpsSerial.RestartMode.ColdStart, 0, 0, 0);
            SetResponseMessage(rep, "Cold Start");
            gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
            ResetConnection(ClearControlsType.Restart);
        }

        private void hotStartBtn_Click(object sender, EventArgs e)
        {
            gps.UninstallDataReceiver();
            GpsSerial.GPS_RESPONSE rep = gps.SystemRestart(2000, GpsSerial.RestartMode.HotStart, 0, 0, 0);
            SetResponseMessage(rep, "Hot Start");
            gps.InstallDataReceiver(deviceInfo.GetDataInHandler());
            ResetConnection(ClearControlsType.Restart);
        }

        private void SetResponseMessage(GpsSerial.GPS_RESPONSE r, string msg)
        {
            if (r == GpsSerial.GPS_RESPONSE.ACK)
                ListBoxUtil.AddStringToListBoxAndScrollToBottom(responseLsb, msg + " successfully");
            else if(r == GpsSerial.GPS_RESPONSE.NACK)
                ListBoxUtil.AddStringToListBoxAndScrollToBottom(responseLsb, msg + " NACK");
            else if (r == GpsSerial.GPS_RESPONSE.TIMEOUT)
                ListBoxUtil.AddStringToListBoxAndScrollToBottom(responseLsb, msg + " timeout");
            else
                ListBoxUtil.AddStringToListBoxAndScrollToBottom(responseLsb, msg + " failed");
        }

        private void ResetConnection(ClearControlsType t)
        {
            StartTTFFCount();
            ClearAllControls(t);
            MessageParser.ClearParsingStatus();
            snr1Pbox.Invalidate();
            snr2Pbox.Invalidate();
            snr3Pbox.Invalidate();
        }
    }
}
