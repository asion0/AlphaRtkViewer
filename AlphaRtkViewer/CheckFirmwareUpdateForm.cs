using MiscUtil.App;
using MiscUtil.IO;
using MiscUtil.Network;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RtkViewer
{
    public partial class CheckFirmwareUpdateForm : Form
    {
        public CheckFirmwareUpdateForm()
        {
            InitializeComponent();
        }

        private enum Step
        {
            Step1,
            Step2,
            Step3,
        }
        private Step step = Step.Step1;

        public enum Mode
        {
            None,
            UpdateFirmware,
            ChangeConstellationType,
            RomModeRecovery,
        }
        private Mode mode = Mode.None;
        private enum Status
        {
            None,
            NoNetwork,
            SeverShutdown,
            ServerError,
            NeedUpdateViewer,
            NewestVersion,
            UpdateAvailable,
            DownloadMasterFailed,
            DownloadSlaveFailed,
            DownloadFinished,
            FirmwareUpdating,
            UpdateMasterFailed,
            UpdateSlaveFailed,
            UpdateFinished,
            ConfigureFailed,
            ConfigureFinished,
            UnknowRomType,
        }

        private DeviceInformation deviceInfo = null;
        private GpsSerial gps = null;
        public void SetDeviceInfoAndGpsSerial(Mode m, DeviceInformation di, GpsSerial gs) { mode = m; deviceInfo = di; gps = gs; }

        private Status status = Status.None;
        private string appInfoXml;
        public string GetAppInfoXml() { return appInfoXml; }

        private BackgroundWorker bwCheckFwWorker = new BackgroundWorker();
        private BackgroundWorker bwFwDownloadWorker = new BackgroundWorker();
        private BackgroundWorker bwMasterFwUpdateWorker = new BackgroundWorker();
        private BackgroundWorker bwSlaveFwUpdateWorker = new BackgroundWorker();
        private BackgroundWorker bwConfigureWorker = new BackgroundWorker();
        private void CheckFirmwareUpdateForm_Load(object sender, EventArgs e)
        {
            if(mode == Mode.UpdateFirmware || mode == Mode.RomModeRecovery)
            {
                Text = "Check Firmware Update";
                downloadStep2TitleLbl.Text = "New Firmware Found";
                downloadStep2PromptLbl.Text = "New firmware is now available.\r\nWould you like to download it from the server and update onto it?";
            }
            else if(mode == Mode.ChangeConstellationType)
            {
                string consType = (deviceInfo.IsBeidouModule()) ? "GPS + BEIDOU" : "GPS + GLONASS";
                Text = "Alpha RTK Receiver Type Configuration";
                downloadStep2TitleLbl.Text = "Configure Alpha RTK Receiver Type";
                downloadStep2PromptLbl.Text = string.Format("{0} {1}. {2}\r\n{3}",
                    "The current constellation type for your Alpha RTK receiver is",
                    consType,
                    "You can change the constellation type by updating the following firmware.",
                    "Would you like to download it from the server and update onto it? ");
            }

            bwCheckFwWorker.WorkerReportsProgress = true;
            bwCheckFwWorker.WorkerSupportsCancellation = true;
            bwCheckFwWorker.DoWork += new DoWorkEventHandler(BwFwCheckDoWork);
            bwCheckFwWorker.ProgressChanged += new ProgressChangedEventHandler(BwProgressChanged);
            bwCheckFwWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BwFwCheckRunWorkerCompleted);
            bwCheckFwWorker.RunWorkerAsync();
        }

        private FirmwareInfo fwInfo = null;
        private enum RomModeType
        {
            None,
            AlphaStandard,
            AlphaStartKit
        }
        RomModeType romModeType = RomModeType.None;
        private void RomModeCheck()
        {
            string s = FirmwareDownload.GetRomModeTag(gps, 6, null);
            if(s.StartsWith(" A077"))
            {
                romModeType = RomModeType.AlphaStartKit;
            }
            else if (s.StartsWith(" A057"))
            {
                romModeType = RomModeType.AlphaStandard;
            }
        }

        string remoteFwFolder = "";
        string remoteListName = "";
        private void BwFwCheckDoWork(object sender, DoWorkEventArgs e)
        {
            if(mode == Mode.RomModeRecovery)
            {
                RomModeCheck();
                if(romModeType == RomModeType.None)
                {
                    status = Status.UnknowRomType;
                    return;
                }
            }

            UpdateServer.SetTimeout(5000);
            bool serverOk = UpdateServer.IsServerAlive();
            if (!serverOk)
            {   //Network connection failed, can't update
                status = Status.NoNetwork;
                return;
            }

            appInfoXml = UpdateServer.GetFileString(AppTools.ViewerFtpFolderName, Program.swUpdateProfile);
            if (appInfoXml.Length == 0)
            {   //Server list file error, can't find the file
                status = Status.SeverShutdown;
                return;
            }

            AppInfo ai = AppTools.GetAppInfoFromXml(appInfoXml);
            if (!ai.Validate())
            {   //Server list file error, can't get server information
                status = Status.ServerError;
                return;
            }

            bool needUpdateViewer = false;
            if (ai.mode == "H")  //High priority, update if version not the same with server
            {
                needUpdateViewer = (AppTools.GetAppVersionString() != ai.version);
            }
            else    //Normal priority, update if version is smaller then server 
            {
                needUpdateViewer = AppTools.IsNewVersion(AppTools.GetAppVersionString(), ai.version);
            }

            if (needUpdateViewer)
            {
                status = Status.NeedUpdateViewer;
                return;
            }

            if (mode == Mode.UpdateFirmware)
            {   //UpdateFirmware for firmware update, find the same constellation type firmware
                remoteFwFolder = deviceInfo.IsGlonassModule() ? AppTools.GlFirmwareFtpFolderName : AppTools.BdFirmwareFtpFolderName;
                remoteListName = deviceInfo.IsAlphaFirmware() ? Program.fwStandardUpdateProfile : Program.fwStartKitUpdateProfile;
            }
            else if(mode == Mode.ChangeConstellationType)
            {   //ChangeConstellationType for constellation type changed, find the different constellation type firmware
                remoteFwFolder = deviceInfo.IsBeidouModule() ? AppTools.GlFirmwareFtpFolderName : AppTools.BdFirmwareFtpFolderName;
                remoteListName = deviceInfo.IsAlphaFirmware() ? Program.fwStandardUpdateProfile : Program.fwStartKitUpdateProfile;
            }
            else if (mode == Mode.RomModeRecovery)
            {   //ChangeConstellationType for constellation type changed, find the different constellation type firmware
                remoteFwFolder = AppTools.GlFirmwareFtpFolderName;
                remoteListName = (romModeType == RomModeType.AlphaStandard) ? Program.fwStandardUpdateProfile : Program.fwStartKitUpdateProfile;
            }

            UpdateServer.SetTimeout(10000);
            string fwInfoXml = UpdateServer.GetFileString(remoteFwFolder, remoteListName);
            if (fwInfoXml.Length == 0)
            {   //Server error, list file doesn't exist
                status = Status.SeverShutdown;
                return;
            }

            fwInfo = AppTools.GetFirmwareInfoFromXml(fwInfoXml);
            if (!fwInfo.Validate())
            {   //Server list file error, list file xml can't parsing
                status = Status.ServerError;
                return;
            }

            bool needUpdateFirmware = CheckFirmwareVersion(fwInfo);
            if (!needUpdateFirmware)
            {   //Doesn't need update, it's the newest version
                status = Status.NewestVersion;
                return;
            }
            status = Status.UpdateAvailable;
        }

        private int progressBase = 0;
        private void BwProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (progressBase + e.ProgressPercentage < downloadStep3Progress.Maximum)
            {
                downloadStep3Progress.Value = progressBase + e.ProgressPercentage;
            }
        }

        private void BwFwCheckRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (status != Status.UpdateAvailable)
            {
                UpdateStatus();
                return;
            }

            step = Step.Step2;
            //Show master firmware information in server 
            newFwKVerMLbl.Text = fwInfo.fwM.kernelVer;
            newFwSVerMLbl.Text = fwInfo.fwM.softwareVer;
            newFwRevMLbl.Text = fwInfo.fwM.revision;
            newFwCrcMLbl.Text = fwInfo.fwM.crc;

            //Show slave firmware information in server
            newFwKVerSLbl.Text = fwInfo.fwS.kernelVer;
            newFwSVerSLbl.Text = fwInfo.fwS.softwareVer;
            newFwRevSLbl.Text = fwInfo.fwS.revision;
            newFwCrcSLbl.Text = fwInfo.fwS.crc;

            if (remoteFwFolder == AppTools.GlFirmwareFtpFolderName)
            {
                opModeLbl.Text = "GPS + GLONASS";
                opModeLbl.ForeColor = Color.BlueViolet;
            }
            else if (remoteFwFolder == AppTools.BdFirmwareFtpFolderName)
            {
                opModeLbl.Text = "GPS + BEIDOU";
                opModeLbl.ForeColor = Color.DarkOrange;
            }
            UpdateStatus();
        }

        private string GetDownloadFilePathName(bool isSlave)
        {
            string path = AppTools.GetRtkViewerDownloadFolder(true);
            return string.Format("{0}\\{1}", path, Path.GetFileName((isSlave) ? fwInfo.fwS.path : fwInfo.fwM.path));
        }

        void ShowFtpProgress(int progressByte)
        {
            try
            {
                bwFwDownloadWorker.ReportProgress(progressByte);
            }
            catch(Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
            }
        }

        private bool downloadFinish = false;
        private void BwFwDownloadDoWork(object sender, DoWorkEventArgs e)
        {
            string dstName = GetDownloadFilePathName(false);
            FileTools.ExistDelete(dstName);
            //Download master firmware
            progressBase = 0;
            UpdateServer.SetTimeout(10000);
            downloadFinish = UpdateServer.GetFile(remoteFwFolder, dstName, fwInfo.fwM.path, ShowFtpProgress);
            if(!downloadFinish)
            {
                status = Status.DownloadMasterFailed;
                return;
            }

            //Download slave firmware
            progressBase = fwInfo.fwM.size;
            dstName = GetDownloadFilePathName(true);
            UpdateServer.SetTimeout(10000);
            downloadFinish = UpdateServer.GetFile(remoteFwFolder, dstName, fwInfo.fwS.path, ShowFtpProgress);
            if (!downloadFinish)
            {
                status = Status.DownloadSlaveFailed;
                return;
            }

            status = Status.DownloadFinished;
        }

        private void LaunchUpdateWorker(bool isSlave)
        {
            if (isSlave)
            {
                downloadStep3ProgressLbl.Text = "Slave firmware updating...";
                progressBase = 0;
                downloadStep3Progress.Value = 0;
                downloadStep3Progress.Maximum = fwInfo.fwS.size;
                status = Status.FirmwareUpdating;
                bwSlaveFwUpdateWorker.WorkerReportsProgress = true;
                bwSlaveFwUpdateWorker.WorkerSupportsCancellation = true;
                bwSlaveFwUpdateWorker.DoWork += new DoWorkEventHandler(BwSlaveFwUpdateDoWork);
                bwSlaveFwUpdateWorker.ProgressChanged += new ProgressChangedEventHandler(BwProgressChanged);
                bwSlaveFwUpdateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BwSlaveFwUpdateRunWorkerCompleted);
                bwSlaveFwUpdateWorker.RunWorkerAsync();
            }
            else
            {
                downloadStep3ProgressLbl.Text = "Master firmware updating...";
                progressBase = 0;
                downloadStep3Progress.Value = 0;
                downloadStep3Progress.Maximum = fwInfo.fwM.size;
                status = Status.FirmwareUpdating;
                bwMasterFwUpdateWorker.WorkerReportsProgress = true;
                bwMasterFwUpdateWorker.WorkerSupportsCancellation = true;
                bwMasterFwUpdateWorker.DoWork += new DoWorkEventHandler(BwMasterFwUpdateDoWork);
                bwMasterFwUpdateWorker.ProgressChanged += new ProgressChangedEventHandler(BwProgressChanged);
                bwMasterFwUpdateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BwMasterFwUpdateRunWorkerCompleted);
                bwMasterFwUpdateWorker.RunWorkerAsync();
            }
        }

        private void LaunchConfigureWorker()
        {
            downloadStep3ProgressLbl.Text = "Restoring device settings...";
            progressBase = 0;
            downloadStep3Progress.Value = 0;
            downloadStep3Progress.Maximum = 6;
            status = Status.FirmwareUpdating;
            bwConfigureWorker.WorkerReportsProgress = true;
            bwConfigureWorker.WorkerSupportsCancellation = true;
            bwConfigureWorker.DoWork += new DoWorkEventHandler(BwConfigureDoWork);
            bwConfigureWorker.ProgressChanged += new ProgressChangedEventHandler(BwProgressChanged);
            bwConfigureWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BwConfigureRunWorkerCompleted);
            bwConfigureWorker.RunWorkerAsync();
        }

        private void BwFwDownloadRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (status != Status.DownloadFinished && !alreadyCancel)
            {
                MessageBox.Show("Firmware download failed, Update has been canceled.");
                Close();
                return;
            }

            if(alreadyCancel)
            {
                return;
            }

            if (NeedMasterUpdate(fwInfo))
            {
                LaunchUpdateWorker(false);
            }
            else if(NeedSlaveUpdate(fwInfo))
            {
                LaunchUpdateWorker(true);
            }
            else
            {
                MessageBox.Show("Unknown error!");
            }
        }

        void ShowUpdateProgress(int progressByte, int total)
        {
            if (total == fwInfo.fwM.size)
            {
                bwMasterFwUpdateWorker.ReportProgress(progressByte);
            }
            else if (total == fwInfo.fwS.size)
            {
                bwSlaveFwUpdateWorker.ReportProgress(progressByte);
            }
        }

        private void BwMasterFwUpdateDoWork(object sender, DoWorkEventArgs e)
        {
            const int defaultDownloadBaudIdx = 7;
            bool ret = false;

            ret = FirmwareDownload.DoDownloadFirmware(gps, GetDownloadFilePathName(false), defaultDownloadBaudIdx, false, ShowUpdateProgress);
            if (!ret)
            {
                status = Status.UpdateMasterFailed;
                return;
            }

            if (SerialTool.GetIndexOfBaudRate(fwInfo.fwM.baud) != gps.GetBaudRateIndex())
            {
                gps.ReOpen(SerialTool.GetIndexOfBaudRate(fwInfo.fwM.baud));
            }

            status = Status.UpdateFinished;
        }

        private void BwMasterFwUpdateRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (status != Status.UpdateFinished && !alreadyCancel)
            {
                MessageBox.Show("Firmware update failed, Update has been canceled.");
                Close();
                return;
            }

            if (alreadyCancel)
            {
                return;
            }

            if (mode != Mode.RomModeRecovery)
            {
                if (NeedSlaveUpdate(fwInfo))
                {
                    LaunchUpdateWorker(true);
                    return;
                }
                LaunchConfigureWorker();
            }
            else
            {
                MessageBox.Show("The Alpha master firmware has been restored. Please unplug the USB cable and plug it in again, then connect at 115200 baud rate. Perform a \"Check Firmware Update\" after connecting to ensure that the slave firmware is correct.");
                Close();
                return;
            }
        }

        private void BwSlaveFwUpdateDoWork(object sender, DoWorkEventArgs e)
        {
            const int defaultDownloadBaudIdx = 5;
            bool ret = false;

            ret = FirmwareDownload.DoDownloadFirmware(gps, GetDownloadFilePathName(true), defaultDownloadBaudIdx, true, ShowUpdateProgress);
            if (!ret)
            {
                status = Status.UpdateSlaveFailed;
                return;
            }

            status = Status.UpdateFinished;
        }

        private void BwSlaveFwUpdateRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (status != Status.UpdateFinished && !alreadyCancel)
            {
                MessageBox.Show("Firmware update failed, Update has been canceled.");
                Close();
                return;
            }

            if (alreadyCancel)
            {
                return;
            }

            LaunchConfigureWorker();
        }

        private void BwConfigureDoWork(object sender, DoWorkEventArgs e)
        {
            status = Status.ConfigureFinished;
            return;
            /*
            GpsSerial.GPS_RESPONSE rep = GpsSerial.GPS_RESPONSE.NONE;

            rep = gps.ConfigureRtkMode(2000, deviceInfo.GetRtkInfo(), GpsSerial.Attributes.SramAndFlash);
            if (rep != GpsSerial.GPS_RESPONSE.ACK)
            {
                status = Status.ConfigureFailed;
                return;
            }

            rep = gps.ConfigureUpdateRate(2000, deviceInfo.GetUpdateRate(), GpsSerial.Attributes.SramAndFlash);
            if (rep != GpsSerial.GPS_RESPONSE.ACK)
            {
                status = Status.ConfigureFailed;
                return;
            }

            //rep = gps.ConfigureMessageType(2000, deviceInfo.GetMessageType(), GpsSerial.Attributes.SramAndFlash);
            if (rep != GpsSerial.GPS_RESPONSE.ACK)
            {
                status = Status.ConfigureFailed;
                return;
            }

            if (deviceInfo.IsRtkBaseMode())
            {
            }

            status = Status.ConfigureFinished;
            */
        }

        private void BwConfigureRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (status != Status.ConfigureFinished)
            {
                MessageBox.Show("Device setting restory failed, Please configure your device manually.");
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            MessageBox.Show("Firmware update finished.");
            DialogResult = DialogResult.OK;
            Close();
        }

        //return true if version is not the same
        private bool CheckFirmwareVersion(FirmwareInfo fi)
        {
            if(deviceInfo.IsRtkBaseMode())
            {   //No slave FW information in RTK Base mode.
                return ((deviceInfo.GetFormatKernelVersion(false) != fi.fwM.kernelVer) ||
                    (deviceInfo.GetFormatSoftwareVersion(false) != fi.fwM.softwareVer) ||
                    (deviceInfo.GetFormatRevision(false) != fi.fwM.revision) ||
                    (deviceInfo.GetFormatCrc(false) != fi.fwM.crc.ToUpper()));
            }

            return ((deviceInfo.GetFormatKernelVersion(false) != fi.fwM.kernelVer) ||
                (deviceInfo.GetFormatSoftwareVersion(false) != fi.fwM.softwareVer) ||
                (deviceInfo.GetFormatRevision(false) != fi.fwM.revision) ||
                (deviceInfo.GetFormatCrc(false) != fi.fwM.crc.ToUpper()) ||
                (deviceInfo.GetFormatKernelVersion(true) != fi.fwS.kernelVer) ||
                (deviceInfo.GetFormatSoftwareVersion(true) != fi.fwS.softwareVer) ||
                (deviceInfo.GetFormatRevision(true) != fi.fwS.revision) ||
                (deviceInfo.GetFormatCrc(true) != fi.fwS.crc.ToUpper()));
        }

        private bool NeedMasterUpdate(FirmwareInfo fi)
        {
            return ((deviceInfo.GetFormatKernelVersion(false) != fi.fwM.kernelVer) ||
                (deviceInfo.GetFormatSoftwareVersion(false) != fi.fwM.softwareVer) ||
                (deviceInfo.GetFormatRevision(false) != fi.fwM.revision) ||
                (deviceInfo.GetFormatCrc(false) != fi.fwM.crc.ToUpper()));
        }

        private bool NeedSlaveUpdate(FirmwareInfo fi)
        {
            if (deviceInfo.IsRtkBaseMode())
            {   //No slave FW information in RTK Base mode.
                return false;
            }
            return ((deviceInfo.GetFormatKernelVersion(true) != fi.fwS.kernelVer) ||
                (deviceInfo.GetFormatSoftwareVersion(true) != fi.fwS.softwareVer) ||
                (deviceInfo.GetFormatRevision(true) != fi.fwS.revision) ||
                (deviceInfo.GetFormatCrc(true) != fi.fwS.crc.ToUpper()));
        }

        private void UpdateStatus()
        {
            switch (status)
            {
                case Status.NoNetwork:
                    progressLbl.Visible = false;
                    progressTextLbl.Visible = false;
                    checkFwPromptLbl.Text = "There is no internet connection to update.";
                    checkFwPromptLbl.Visible = true;
                    checkFwOkBtn.Visible = true;
                    break;
                case Status.SeverShutdown:
                    progressLbl.Visible = false;
                    progressTextLbl.Visible = false;
                    checkFwPromptLbl.Text = "The server did not respond. Firmware could not be updated.";
                    checkFwPromptLbl.Visible = true;
                    checkFwOkBtn.Visible = true;
                    break;
                case Status.ServerError:
                    progressLbl.Visible = false;
                    progressTextLbl.Visible = false;
                    checkFwPromptLbl.Text = "Server error. Firmware could not be updated.";
                    checkFwPromptLbl.Visible = true;
                    checkFwOkBtn.Visible = true;
                    break;
                case Status.NeedUpdateViewer:
                    progressLbl.Visible = false;
                    progressTextLbl.Visible = false;
                    checkFwPromptLbl.Text = "Updating FW requires the latest version of RTK Viewer!";
                    checkFwPromptLbl.Visible = true;
                    checkFwOkBtn.Visible = true;
                    break;
                case Status.NewestVersion:
                    progressLbl.Visible = false;
                    progressTextLbl.Visible = false;
                    checkFwPromptLbl.Text = "You are running the latest version of Alpha firmware.";
                    checkFwPromptLbl.Visible = true;
                    checkFwOkBtn.Visible = true;
                    break;
                case Status.UnknowRomType:
                    progressLbl.Visible = false;
                    progressTextLbl.Visible = false;
                    checkFwPromptLbl.Text = "Unknown ROM type!";
                    checkFwPromptLbl.Visible = true;
                    checkFwOkBtn.Visible = true;
                    break;
                case Status.UpdateAvailable:
                    break;
            }

            Panel p = null;
            downloadChangePanel.Visible = false;
            downloadUpdatePanel.Visible = false;
            downloadStep2Panel.Visible = false;
            downloadStep3Panel.Visible = false;
            switch (step)
            {
                case Step.Step1:
                    p = downloadUpdatePanel;
                    break;
                case Step.Step2:
                    p = downloadStep2Panel;
                    break;
                case Step.Step3:
                    p = downloadStep3Panel;
                    break;
            }
            p.Left = 0;
            p.Top = 0;
            p.Visible = true;
        }

        private void noBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private int maxProgressValue = 0;
        private void step2YesBtn_Click(object sender, EventArgs e)
        {
            maxProgressValue = fwInfo.fwM.size + fwInfo.fwS.size;
            downloadStep3Progress.Maximum = maxProgressValue;
            step = Step.Step3;
            UpdateStatus();

            bwFwDownloadWorker.WorkerReportsProgress = true;
            bwFwDownloadWorker.WorkerSupportsCancellation = true;
            bwFwDownloadWorker.DoWork += new DoWorkEventHandler(BwFwDownloadDoWork);
            bwFwDownloadWorker.ProgressChanged += new ProgressChangedEventHandler(BwProgressChanged);
            bwFwDownloadWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BwFwDownloadRunWorkerCompleted);
            bwFwDownloadWorker.RunWorkerAsync();
        }

        private void step2NoBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void step0YesBtn_Click(object sender, EventArgs e)
        {
            //mode = Mode.Step3;
            //UpdateStatus();
        }

        private void step0NoBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkFwOkBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        bool alreadyCancel = false;
        private void CheckFirmwareUpdateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (status == Status.FirmwareUpdating)
            {
                e.Cancel = true;
                return;
                //var window = MessageBox.Show(
                //    "The firmware is being updated and closing dialog box may cause device damage. Are you sure you want to close?",
                //    "WARNING",
                //    MessageBoxButtons.YesNo);

                //e.Cancel = (window == DialogResult.No);
            }
            alreadyCancel = true;
            if (bwCheckFwWorker.IsBusy)
            {
                bwCheckFwWorker.CancelAsync();
            }
            if (bwFwDownloadWorker.IsBusy)
            {
                bwFwDownloadWorker.CancelAsync();
            }
            if (bwMasterFwUpdateWorker.IsBusy)
            {
                bwMasterFwUpdateWorker.CancelAsync();
            }
            if (bwSlaveFwUpdateWorker.IsBusy)
            {
                bwSlaveFwUpdateWorker.CancelAsync();
            }
            if (bwConfigureWorker.IsBusy)
            {
                bwConfigureWorker.CancelAsync();
            }
        }
    }
}
