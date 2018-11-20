using MiscUtil.App;
using MiscUtil.Network;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RtkViewer
{
    public partial class CheckAppUpdateForm : Form
    {
        private bool checkValue = false;
        private bool autoClose = false;
        public CheckAppUpdateForm(bool c, bool a)
        {
            checkValue = c;
            autoClose = a;
            InitializeComponent();
        }

        public bool GetCheckBoxValue() { return checkValue; }

        private enum Step
        {
            Step1,
            Step2,
            Step3
        }
        Step step = Step.Step1;

        private enum Status
        {
            None,
            NoNetwork,
            SeverShutdown,
            ServerError,
            NewestVersion,
            UpdateAvailable,
        }
        private Status status = Status.None;

        private void UpdateStatus()
        {
            switch(status)
            {
                case Status.NoNetwork:
                    progressLbl.Visible = false;
                    progressTextLbl.Visible = false;
                    softwareCheclStep2PromptLbl.Text = "There is no internet connection to update.";
                    softwareCheclStep2PromptLbl.Visible = true;
                    break;
                case Status.SeverShutdown:
                    progressLbl.Visible = false;
                    progressTextLbl.Visible = false;
                    softwareCheclStep2PromptLbl.Text = "The server did not respond. RTK Viewer could not be updated.";
                    softwareCheclStep2PromptLbl.Visible = true;
                    break;
                case Status.ServerError:
                    progressLbl.Visible = false;
                    progressTextLbl.Visible = false;
                    softwareCheclStep2PromptLbl.Text = "Server error. RTK Viewer could not be updated.";
                    softwareCheclStep2PromptLbl.Visible = true;
                    break;
                case Status.NewestVersion:
                    progressLbl.Visible = false;
                    progressTextLbl.Visible = false;
                    softwareCheclStep2PromptLbl.Text = "You are running the latest version of RTK Viewer.";
                    softwareCheclStep2PromptLbl.Visible = true;
                    break;
                case Status.UpdateAvailable:
                    progressLbl.Visible = false;
                    progressTextLbl.Visible = false;
                    softwareCheclStep3PromptLbl.Visible = true;
                    break;
            }

            Panel p = null;
            softwareCheckStep1Panel.Visible = false;
            softwareCheckStep2Panel.Visible = false;
            softwareCheckStep3Panel.Visible = false;
            switch (step)
            {
                case Step.Step1:
                    p = softwareCheckStep1Panel;
                    break;
                case Step.Step2:
                    p = softwareCheckStep2Panel;
                    break;
                case Step.Step3:
                    p = softwareCheckStep3Panel;
                    break;
            }
            p.Left = 0;
            p.Top = 0;
            p.Visible = true;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void downloadBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void dontAskChk_CheckedChanged(object sender, EventArgs e)
        {
            checkValue = (sender as CheckBox).Checked;

            if(softwareCheckStep2DontAskChk.Checked != checkValue)
            {
                softwareCheckStep2DontAskChk.Checked = checkValue;
            }
            if (softwareCheckStep3DontAskChk.Checked != checkValue)
            {
                softwareCheckStep3DontAskChk.Checked = checkValue;
            }
        }

        private BackgroundWorker bwCheckFwWorker = new BackgroundWorker();
        private void CheckAppUpdateForm_Load(object sender, EventArgs e)
        {
            Text = AppTools.GetAppTitleWithVersion();
            softwareCheckStep3DontAskChk.Checked = checkValue;

            bwCheckFwWorker.WorkerReportsProgress = true;
            bwCheckFwWorker.WorkerSupportsCancellation = true;
            bwCheckFwWorker.DoWork += new DoWorkEventHandler(BwFwCheckDoWork);
            //bwCheckFwWorker.ProgressChanged += new ProgressChangedEventHandler(BwProgressChanged);
            bwCheckFwWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BwFwCheckRunWorkerCompleted);
            bwCheckFwWorker.RunWorkerAsync();
        }

        AppInfo ai;
        private string appInfoXml;
        public string GetAppInfoXml() { return appInfoXml; }
        string dbgMsg = "";

        public object AppTool { get; private set; }

        private void BwFwCheckDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                UpdateServer.SetTimeout(10000);
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

                ai = AppTools.GetAppInfoFromXml(appInfoXml);
                if (!ai.Validate())
                {   //Server list file error, can't get server information
                    status = Status.ServerError;
                    return;
                }

                bool needUpdate = false;
                if (ai.mode == "H")
                {   //High priority, update if version not the same with server
                    needUpdate = (AppTools.GetAppVersionString() != ai.version);
                }
                else
                {   //Normal priority, update if version is smaller then server 
                    needUpdate = AppTools.IsNewVersion(AppTools.GetAppVersionString(), ai.version);
                }

                if (!needUpdate)
                {   //Doesn't need update, it's the newest version
                    status = Status.NewestVersion;
                    return;
                }

                status = Status.UpdateAvailable;
                appInfoXml = ai.xml;
            }
            catch (Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
                dbgMsg = ex.ToString();
                status = Status.ServerError;
                return;
            }
        }

        private void BwFwCheckRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (status == Status.UpdateAvailable)
            {
                softwareCheclStep3PromptLbl.Text = 
                    string.Format("A new RTK Viewer version ({0}) is available. World you like to download it and update your software now?",
                    ai.version);    //                    //ai.version.Substring(0, ai.version.LastIndexOf('.')));
                step = Step.Step3;
            }
            else
            {
                step = Step.Step2;
            }
            UpdateStatus();

            if(dbgMsg != "")
            {
                MessageBox.Show(dbgMsg);
            }
            if(status == Status.NewestVersion && autoClose)
            {
                Close();
            }
        }
    }
}
