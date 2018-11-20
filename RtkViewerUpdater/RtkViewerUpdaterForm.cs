using MiscUtil.App;
using MiscUtil.IO;
using MiscUtil.Network;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace RtkViewerUpdater
{
    public partial class RtkViewerUpdaterForm : Form
    {
        public RtkViewerUpdaterForm()
        {
            InitializeComponent();
        }

        private BackgroundWorker bwFtpWorker = new BackgroundWorker();
        private void RtkViewerUpdaterForm_Load(object sender, EventArgs e)
        {
            Text = AppTools.GetAppTitle();
            downloadProgress.Maximum = Program.appInfo.size;
            UpdateServer.SetTimeout(10000);
            if (!UpdateServer.IsServerAlive())
            {
                MessageBox.Show("Application download failed! RTK Viewr is not updated.");
                Close();
                return;
            }


            bwFtpWorker.WorkerReportsProgress = true;
            bwFtpWorker.WorkerSupportsCancellation = true;
            bwFtpWorker.DoWork += new DoWorkEventHandler(BwCheckFtpDoWork);
            bwFtpWorker.ProgressChanged += new ProgressChangedEventHandler(BwCheckFtpProgressChanged);
            bwFtpWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BwCheckFtpRunWorkerCompleted);

            bwFtpWorker.RunWorkerAsync();
        }

        void ShowFtpProgress(int progressByte)
        {
            bwFtpWorker.ReportProgress(progressByte);
        }

        bool downloadFinish = false;
        private string GetDownloadFilePathName()
        {
            string path = AppTools.GetRtkViewerDownloadFolder(true);
            return string.Format("{0}\\{1}", path, Path.GetFileName(Program.appInfo.path));
        }

        private void BwCheckFtpDoWork(object sender, DoWorkEventArgs e)
        {
            string dstName = GetDownloadFilePathName();
            FileTools.ExistDelete(dstName);

            UpdateServer.SetTimeout(10000);
            downloadFinish = UpdateServer.GetFile(AppTools.ViewerFtpFolderName, dstName, Program.appInfo.path, ShowFtpProgress);
        }

        private void BwCheckFtpProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < downloadProgress.Maximum)
            {
                downloadProgress.Value = e.ProgressPercentage;
            }
        }

        private void BwCheckFtpRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FileInfo fi = new FileInfo(GetDownloadFilePathName());
            if (!downloadFinish || (fi.Length != Program.appInfo.size))
            {
                MessageBox.Show("Application download failed! RTK Viewr is not updated.");
                Close();
                return;
            }

            bool first = false;
            int tryCount = 30;
            while (!first && tryCount > 0)
            {
                Mutex mutex = new Mutex(true, MiscUtil.App.AppTools.RtkViewerMutexName, out first);
                if (first)
                {
                    mutex.ReleaseMutex();
                    break;
                }
                --tryCount;
                Thread.Sleep(100);
            }

            if(!first)
            {
                MessageBox.Show("Please close the RTK Viewer to update.");

                tryCount = 10;
                while (!first && tryCount > 0)
                {
                    Mutex mutex = new Mutex(true, MiscUtil.App.AppTools.RtkViewerMutexName, out first);
                    if (first)
                    {
                        mutex.ReleaseMutex();
                        break;
                    }
                    --tryCount;
                    Thread.Sleep(100);
                }
                if (!first)
                {
                    MessageBox.Show("Please close all instances of the RTK Viewer application and try to update again.");
                    Close();
                    return;
                }
            }

            try
            {
                string dstPath = AppTools.GetSourceLocation();
                FileTools.ExistDelete(dstPath);

                string path = AppTools.GetRtkViewerDownloadFolder(true);
                string srcPath = string.Format("{0}\\{1}", path, Path.GetFileName(Program.appInfo.path));

                if (dstPath.Length > 0)
                {
                    File.Copy(srcPath, dstPath);
                    MessageBox.Show("RTK Viewer update was successful.");
                    MiscUtil.App.AppTools.LaunchExe(dstPath, "");
                }
                else
                {
                    MessageBox.Show("RTK Viewer update was successful.");
                    MiscUtil.App.AppTools.LaunchExe(srcPath, "");
                }
            }
            catch(Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
            }
            Close();
        }
    }
}
