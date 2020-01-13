using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MiscUtil.Conversion;
using MiscUtil.UI;
using MiscUtil.App;

namespace RtkViewer
{
    public partial class DetectingDeviceForm : Form
    {
        private static GpsSerial gps = null;
        private static DeviceInformation deviceInfo = null;
        public DetectingDeviceForm(GpsSerial g, DeviceInformation di)
        {
            gps = g;
            deviceInfo = di;
            InitializeComponent();
        }

        private void DetectingDeviceForm_Load(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            detectWorker.RunWorkerAsync();
        }

        private static Stopwatch watch = new Stopwatch();
        private static DeviceInformation.FinalStage WaitDeviceOutput(int workType, BackgroundWorker w, GpsSerial g)
        {
            watch.Reset();
            watch.Start();
            w.ReportProgress(-1, "Waiting for device output...");

            int charCount = 20;
            int timeout = 1500;
            bool detectResult = false;
            g.DisableDataReceiver(true);
            while (watch.ElapsedMilliseconds < timeout)
            {
                if (g.SerialPort.BytesToRead >= charCount)
                {
                    detectResult = true;
                    break;
                }
                Thread.Sleep(1);
            }
            g.DisableDataReceiver(false);
            if (detectResult)
            {
                w.ReportProgress(-1, "The device output has been detected.");
                g.ClearInBuffer();
            }
            else
            {
                w.ReportProgress(-1, "The device has no output!");
            }
            w.ReportProgress(-1, String.Format("Number of characters output: {0} ({1} ms)", g.SerialPort.BytesToRead, watch.ElapsedMilliseconds));
            return (detectResult) ? DeviceInformation.FinalStage.None : DeviceInformation.FinalStage.Device_Error;
        }

        private static DeviceInformation.FinalStage ReopenDevice(int workType, BackgroundWorker w, GpsSerial g)
        {
            watch.Reset();
            watch.Start();

            int baud = g.GetBaudRateIndex();
            string port = g.GetPortName();
            g.Close();
            Thread.Sleep(500);
            g.Open(port, baud);
            return QuerySwVersion(workType, w, g);
        }

        private static DeviceInformation.FinalStage QuerySwVersion(int workType, BackgroundWorker w, GpsSerial g)
        {
            watch.Reset();
            watch.Start();

            //Query Slave Software Version
            GpsSerial.GPS_RESPONSE rep = deviceInfo.QuerySoftwareVersion(true);
            w.ReportProgress(-1, String.Format("Querying slave FW version returns {0} ({1} ms)", rep.ToString(), watch.ElapsedMilliseconds));
            if (GpsSerial.GPS_RESPONSE.ACK != rep && GpsSerial.GPS_RESPONSE.NACK != rep)
            {   //Device error
                w.ReportProgress(-1, "Querying slave FW version failed!");
                return DeviceInformation.FinalStage.Device_Error;
            }

            byte[] kVer;
            byte[] sVer;
            byte[] rev;
            if (GpsSerial.GPS_RESPONSE.NACK != rep)
            {
                kVer = deviceInfo.GetKernelVersion(true);
                sVer = deviceInfo.GetSoftwareVersion(true);
                rev = deviceInfo.GetRevision(true);
                w.ReportProgress(-1, "Slave K.Ver.: " + deviceInfo.GetFormatKernelVersion(true));
                w.ReportProgress(-1, "Slave S.Ver.: " + deviceInfo.GetFormatSoftwareVersion(true));
                w.ReportProgress(-1, "Slave Rev.: " + deviceInfo.GetFormatRevision(true));

                //Get Firmware CRC
                watch.Reset();
                watch.Start();
                rep = deviceInfo.QueryCrc(true);
                w.ReportProgress(-1, String.Format("Querying slave FW CRC returns {0} ({1} ms)", rep.ToString(), watch.ElapsedMilliseconds));
                if (GpsSerial.GPS_RESPONSE.ACK != rep && GpsSerial.GPS_RESPONSE.NACK != rep)
                {   //Device error
                    w.ReportProgress(-1, "Querying slave CRC failed!");
                    return DeviceInformation.FinalStage.Device_Error;
                }
                w.ReportProgress(-1, "Slave CRC: " + deviceInfo.GetFormatCrc(true));
            }

            //Query Master Software Version
            rep = deviceInfo.QuerySoftwareVersion(false);
            w.ReportProgress(-1, String.Format("Querying master FW version returns {0} ({1} ms)", rep.ToString(), watch.ElapsedMilliseconds));
            if (GpsSerial.GPS_RESPONSE.ACK != rep)
            {   //Device error
                w.ReportProgress(-1, "Querying master FW version failed!");
                return DeviceInformation.FinalStage.Device_Error;
            }
            
            kVer = deviceInfo.GetKernelVersion(false);
            sVer = deviceInfo.GetSoftwareVersion(false);
            rev = deviceInfo.GetRevision(false);
            w.ReportProgress(-1, "Master K.ver.: " + deviceInfo.GetFormatKernelVersion(false));
            w.ReportProgress(-1, "Master S.Ver.: " + deviceInfo.GetFormatSoftwareVersion(false));
            w.ReportProgress(-1, "Master Rev.: " + deviceInfo.GetFormatRevision(false));

            //Get Firmware CRC
            watch.Reset();
            watch.Start();
            rep = deviceInfo.QueryCrc(false);
            w.ReportProgress(-1, String.Format("Querying master FW CRC returns {0} ({1} ms)", rep.ToString(), watch.ElapsedMilliseconds));
            if (GpsSerial.GPS_RESPONSE.ACK != rep)
            {   //Device error
                w.ReportProgress(-1, "Querying master CRC failed!");
                return DeviceInformation.FinalStage.Device_Error;
            }
            w.ReportProgress(-1, "Master CRC: " + deviceInfo.GetFormatCrc(false));

            if (deviceInfo.GetSoftwareVersion(false)[1] == 10 || deviceInfo.GetSoftwareVersion(false)[1] == 200)
            {   //FW for Alpha
                return DeviceInformation.FinalStage.None;
            }

            //Check Boot Status
            watch.Reset();
            watch.Start();
            rep = deviceInfo.QueryBootStatus();
            w.ReportProgress(-1, String.Format("Querying boot status returns {0} ({1} ms)", rep.ToString(), watch.ElapsedMilliseconds));
            if (GpsSerial.GPS_RESPONSE.ACK != rep)
            {   //Device error
                w.ReportProgress(-1, "Querying boot status failed!");
                return DeviceInformation.FinalStage.Device_Error;
            }

            w.ReportProgress(-1, "Failover: " + deviceInfo.GetBootFailedFlag().ToString());
            w.ReportProgress(-1, "Boot ROM: " + deviceInfo.GetBootRomFlag().ToString());
            if (deviceInfo.GetBootFailedFlag() || deviceInfo.GetBootRomFlag())
            {   //In ROM mode
                return DeviceInformation.FinalStage.Rom_Mode;
            }

            //Unable to support
            w.ReportProgress(-1, "Unsupported firmware!");
            return DeviceInformation.FinalStage.Device_Not_Support;
        }

        private static DeviceInformation.FinalStage DetectFwType(int workType, BackgroundWorker w, GpsSerial g)
        {
            watch.Reset();
            watch.Start();

            //Update Rate
            GpsSerial.GPS_RESPONSE rep = deviceInfo.QueryUpdateRate();
            w.ReportProgress(-1, String.Format("Querying updat rate return {0} ({1} ms)", rep.ToString(), watch.ElapsedMilliseconds));
            if (GpsSerial.GPS_RESPONSE.ACK != rep)
            {
                w.ReportProgress(-1, "Device error! ");
                return DeviceInformation.FinalStage.Device_Error;
            }
            w.ReportProgress(-1, "Update rate: " + deviceInfo.GetUpdateRate());

            //Is DR FW
            watch.Reset();
            watch.Start();
            rep = deviceInfo.QueryDrPredictUpdateRate();
            w.ReportProgress(-1, String.Format("Querying DR mode return {0} ({1} ms)", rep.ToString(), watch.ElapsedMilliseconds));
            if (GpsSerial.GPS_RESPONSE.ACK == rep)
            {   //Only DR FW will response this command
                w.ReportProgress(-1, "Operation mode: ODR ");
                return DeviceInformation.FinalStage.Normal_In_Odr;
            }

            //Which RTK mode
            watch.Reset();
            watch.Start();
            rep = deviceInfo.QueryRtkMode();
            w.ReportProgress(-1, String.Format("Querying RTK mode return {0} ({1} ms)", rep.ToString(), watch.ElapsedMilliseconds));
            if (GpsSerial.GPS_RESPONSE.ACK != rep)
            {   //Only DR FW will response this command
                w.ReportProgress(-1, "Device error! ");
                return DeviceInformation.FinalStage.Device_Error;
            }
            w.ReportProgress(-1, "RTK mode: " + deviceInfo.GetRtkInfo().rtkMode.ToString());

            //Which constellation type
            watch.Reset();
            watch.Start();
            rep = deviceInfo.QueryConstellationType();
            w.ReportProgress(-1, String.Format("Querying constellation type return {0} ({1} ms)", rep.ToString(), watch.ElapsedMilliseconds));
            if (GpsSerial.GPS_RESPONSE.ACK != rep)
            {   //Only DR FW will response this command
                w.ReportProgress(-1, "Device error! ");
                return DeviceInformation.FinalStage.Device_Error;
            }
            w.ReportProgress(-1, "Constellation type: " + deviceInfo.GetConstellationType().ToString());

            //Detect message type
            watch.Reset();
            watch.Start();
            rep = deviceInfo.QueryMessageType();
            w.ReportProgress(-1, String.Format("Querying message type return {0} ({1} ms)", rep.ToString(), watch.ElapsedMilliseconds));
            if (GpsSerial.GPS_RESPONSE.ACK != rep)
            { 
                w.ReportProgress(-1, "Device error! ");
                return DeviceInformation.FinalStage.Device_Error;
            }
            w.ReportProgress(-1, "Message type: " + deviceInfo.GetMessageTypeString());

            DeviceInformation.FinalStage ret = DeviceInformation.FinalStage.Device_Error;
            if (deviceInfo.IsGlonassModule())
            {
                ret = (deviceInfo.IsRtkBaseMode()) ? DeviceInformation.FinalStage.Normal_In_Gl_Base : DeviceInformation.FinalStage.Normal_In_Gl_Rover;
            }
            else if (deviceInfo.IsBeidouModule())
            {
                ret = (deviceInfo.IsRtkBaseMode()) ? DeviceInformation.FinalStage.Normal_In_Bd_Base : DeviceInformation.FinalStage.Normal_In_Bd_Rover;
            }
            else
            {
                w.ReportProgress(-1, "Unknown constellation type! " + deviceInfo.GetConstellationType().ToString());
            }

            if (deviceInfo.IsAlphaFirmware())
            {
                //Query Alpha License
                watch.Reset();
                watch.Start();
                rep = deviceInfo.QueryMiscInformation();
                w.ReportProgress(-1, String.Format("Querying misc info return {0} ({1} ms)", rep.ToString(), watch.ElapsedMilliseconds));
                if (GpsSerial.GPS_RESPONSE.ACK != rep)
                {
                    w.ReportProgress(-1, "Device error! ");
                    return DeviceInformation.FinalStage.Device_Error;
                }
                w.ReportProgress(-1, "MiscInfo: " + deviceInfo.GetMiscInfoString().ToString());
            }
            return ret;
        }


        public delegate DeviceInformation.FinalStage WorkerFunction(int workType, BackgroundWorker w, GpsSerial g);

        private class DetectWorker
        {
            public WorkerFunction currentStat;
            public DeviceInformation.FinalStage result;
            public WorkerFunction nextStat;
            public DeviceInformation.FinalStage fs;
            public DetectWorker(WorkerFunction c, DeviceInformation.FinalStage r, WorkerFunction n, DeviceInformation.FinalStage f)
            {
                currentStat = c;
                result = r;
                nextStat = n;
                fs = f;
            }
        }

        private DetectWorker[] workerStateTable = {
            new DetectWorker(WaitDeviceOutput, DeviceInformation.FinalStage.None, QuerySwVersion, DeviceInformation.FinalStage.None),
            new DetectWorker(WaitDeviceOutput, DeviceInformation.FinalStage.Device_Error, null, DeviceInformation.FinalStage.Device_Error),
            new DetectWorker(QuerySwVersion, DeviceInformation.FinalStage.None, DetectFwType, DeviceInformation.FinalStage.None),
            new DetectWorker(QuerySwVersion, DeviceInformation.FinalStage.Device_Error, null, DeviceInformation.FinalStage.Device_Error),
            new DetectWorker(QuerySwVersion, DeviceInformation.FinalStage.Rom_Mode, null, DeviceInformation.FinalStage.Rom_Mode),
            new DetectWorker(QuerySwVersion, DeviceInformation.FinalStage.Device_Not_Support, null, DeviceInformation.FinalStage.Device_Not_Support),
            //new DetectWorker(QuerySwVersion, DeviceInformation.FinalStage.Device_FirstTimeout, ReopenDevice, DeviceInformation.FinalStage.None),
            new DetectWorker(DetectFwType, DeviceInformation.FinalStage.Device_Error, null, DeviceInformation.FinalStage.Device_Error),
            new DetectWorker(DetectFwType, DeviceInformation.FinalStage.Normal_In_Gl_Rover, null, DeviceInformation.FinalStage.Normal_In_Gl_Rover),
            new DetectWorker(DetectFwType, DeviceInformation.FinalStage.Normal_In_Gl_Base, null, DeviceInformation.FinalStage.Normal_In_Gl_Base),
            new DetectWorker(DetectFwType, DeviceInformation.FinalStage.Normal_In_Bd_Rover, null, DeviceInformation.FinalStage.Normal_In_Bd_Rover),
            new DetectWorker(DetectFwType, DeviceInformation.FinalStage.Normal_In_Bd_Base, null, DeviceInformation.FinalStage.Normal_In_Bd_Base),
            new DetectWorker(DetectFwType, DeviceInformation.FinalStage.Normal_In_Odr, null, DeviceInformation.FinalStage.Normal_In_Odr),
        };

        private void detectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch w = new Stopwatch();
            w.Start();

            WorkerFunction current = WaitDeviceOutput;
            try
            {
                do
                {
                    DeviceInformation.FinalStage result = current(0, detectWorker, gps);
                    foreach(DetectWorker dw in workerStateTable)
                    {
                        if(dw.currentStat != current || result != dw.result)
                        {
                            continue;
                        }
                        current = dw.nextStat;
                        if (dw.fs != DeviceInformation.FinalStage.None)
                        {
                            deviceInfo.SetFinalStage(dw.fs);
                        }
                        break;
                    }
                } while (current != null);
                detectWorker.ReportProgress(-1, "Total: " + (w.ElapsedMilliseconds).ToString() + " ms");
            }
            catch(Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
            }
        }

        private void detectWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ListBoxUtil.AddStringToListBoxAndScrollToBottom(msgLsb, e.UserState as string);
        }

        private void detectWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ListBoxUtil.AddStringToListBoxAndScrollToBottom(msgLsb, "Device detection completed.");
            this.UseWaitCursor = false;
            progressLbl.Visible = false;
            switch(deviceInfo.GetFinalStage())
            {
                case DeviceInformation.FinalStage.Device_Error:
                    noticeLbl.Text = "Device detection failed! Please confirm that the baud rate and COM port are correct.";
                    noticePanel.Visible = true;
                    break;
                case DeviceInformation.FinalStage.Device_Not_Support:
                    noticeLbl.Text = "Unsupported device!";
                    noticePanel.Visible = true;
                    break;
                case DeviceInformation.FinalStage.Rom_Mode:
                    noticeLbl.Text = "Your device is in ROM mode, please use \"Check Firmware Update\" to restore your firmware.";
                    noticePanel.Visible = true;
                    break;
                case DeviceInformation.FinalStage.Normal_In_Odr:
                    receiverTypeLbl.Text = "ODR";
                    rtkModeLbl.Text = "---";
                    optFunctionLbl.Text = "---";
                    licenseLbl.Text = "---";
                    infoPanel.Visible = true;
                    break;
                case DeviceInformation.FinalStage.Normal_In_Gl_Base:
                case DeviceInformation.FinalStage.Normal_In_Gl_Rover:
                case DeviceInformation.FinalStage.Normal_In_Bd_Base:
                case DeviceInformation.FinalStage.Normal_In_Bd_Rover:
                    if (deviceInfo.IsGlonassModule())
                        receiverTypeLbl.Text = "GPS + GLONASS";
                    else if (deviceInfo.IsBeidouModule())
                        receiverTypeLbl.Text = "GPS + BEIDOU";
                    else
                        receiverTypeLbl.Text = "Unknown Constellation Type";

                    if (deviceInfo.IsRtkBaseMode())
                        rtkModeLbl.Text = "Base";
                    else if (deviceInfo.IsRtkRoverMode())
                        rtkModeLbl.Text = "Rover";
                    else
                        rtkModeLbl.Text = "Unknown";

                    if (deviceInfo.IsRtkBaseMode())
                    {
                        if (deviceInfo.GetRtkOperationMode() == GpsSerial.RtkModeInfo.RtkOperationMode.Base_Kinematic)
                            optFunctionLbl.Text = "Kinematic";
                        else if (deviceInfo.GetRtkOperationMode() == GpsSerial.RtkModeInfo.RtkOperationMode.Base_Static)
                            optFunctionLbl.Text = "Static";
                        else
                            optFunctionLbl.Text = "Survey";
                    }
                    else if(deviceInfo.IsRtkRoverMode())
                    {
                        if (deviceInfo.GetRtkOperationMode() == GpsSerial.RtkModeInfo.RtkOperationMode.Rover_Normal)
                            optFunctionLbl.Text = "Normal";
                        else if (deviceInfo.GetRtkOperationMode() == GpsSerial.RtkModeInfo.RtkOperationMode.Rover_MovingBase)
                            optFunctionLbl.Text = "Moving Base";
                        else
                            optFunctionLbl.Text = "Unknown";                           
                    }

                    if (deviceInfo.IsAlphaFirmware())
                    {
                        //License End YY/MM/DD
                        if (deviceInfo.GetLicenseType() == DeviceInformation.LicenseType.Perpetual)
                        {
                            licenseLbl.Text = "Perpetual License";
                        }
                        else if (deviceInfo.GetLicenseType() == DeviceInformation.LicenseType.Monthly)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("{0}/{1}/{2} ", deviceInfo.GetMiscStartY(), deviceInfo.GetMiscStartM(), deviceInfo.GetMiscStartD());
                            sb.AppendFormat(" ~ {0}/{1}/{2}", deviceInfo.GetMiscEndY(), deviceInfo.GetMiscEndM(), deviceInfo.GetMiscEndD());
                            licenseLbl.Text = sb.ToString();
                        }
                        else if (deviceInfo.GetLicenseType() == DeviceInformation.LicenseType.OneYear)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("{0}/{1}/{2} ", deviceInfo.GetMiscStartY(), deviceInfo.GetMiscStartM(), deviceInfo.GetMiscStartD());
                            sb.AppendFormat(" ~ {0}/{1}/{2}", deviceInfo.GetMiscEndY(), deviceInfo.GetMiscEndM(), deviceInfo.GetMiscEndD());
                            licenseLbl.Text = sb.ToString();
                        }
                        else
                        {
                            licenseLbl.Text = "License Required";
                        }
                    }

                    infoPanel.Visible = true;
                    DialogResult = DialogResult.OK;
                    Close();
                    break;
            }
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
