using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using MiscUtil.App;
using System.Diagnostics;
using MiscUtil.IO;

namespace RtkViewer
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ParsingParameters(args);
            //Not allow user clicked in application folder to launch
            if (!localRun && DirectlyRunInAppFolder())
            {
                MessageBox.Show("Can't be executed here!");
                return;
            }
            
            if (!localRun)
            {   //Not in LOCAL_RUN mode, Entering the installation mode 
                if (CheckInstall())
                {   //Need install this to app folder
                    if(!InstallLocal())
                    {   //Install failed
                        MessageBox.Show("Application installation failed!");
                        return;
                    }
                }
                AppTools.WriteSourceLocation(Assembly.GetExecutingAssembly().Location);
                LaunchLocal();
                return;
            }

            // 20190117 Do not block multiple executions, request from Oliver
            //if (AppTools.CreateMutex(AppTools.RtkViewerMutexName))
            //{
            //    MessageBox.Show("Application" + " " + Application.ProductName.ToString() + " " + "already running");
            //    return;
            //}

            AppTools.ShowDebug("RTK Viewer2 starts");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AlphaView());
            //AppTools.ReleaseMutex();
        }

        public static bool localRun = false;
#if _USER_YVES_
        public static string swUpdateProfile = "List_Yves.xml";
#else
        public static string swUpdateProfile = "List.xml";
#endif
        public static string fwStandardUpdateProfile = "List.xml";
        public static string fwStartKitUpdateProfile = "ListStartKit.xml";

        private const string LocalRunParam = "/LOCAL_RUN";
        static void ParsingParameters(string[] args)
        {
#if _LOCAL_RUN_
            localRun = true;
#else
            foreach (string s in args)
            {
                if(s == LocalRunParam)
                {
                    localRun = true;
                }
            }
#endif
        }

        static bool DirectlyRunInAppFolder()
        {
            string appFolder = AppTools.GetRtkViewerFolder(true).ToLower();
            string appTmpFolder = AppTools.GetRtkViewerDownloadFolder(true).ToLower();
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).ToLower();
            return (exePath == appFolder || (exePath == appTmpFolder));
        }

        //return true - need install, false - newest version
        static bool CheckInstall()
        {
            string path = AppTools.GetRtkViewerFolder(true) + AppTools.RtkViewerFileName;
            if (!File.Exists(path))
            {   //No application found, need to install
                return true;
            }

            string orgVer = AppTools.GetExeFileVersionInfo(path);
            bool needInstall = AppTools.IsNewVersion(orgVer, MiscUtil.App.AppTools.GetAppVersionString());
            return needInstall;
        }

        static bool InstallLocal()
        {
            try
            {
                string dstPath = AppTools.GetRtkViewerFolder(true) + AppTools.RtkViewerFileName;
                string srcPath = Assembly.GetExecutingAssembly().Location;
                FileTools.ExistDelete(dstPath);

                File.Copy(srcPath, dstPath);
                return true;
            }
            catch(Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        static void LaunchLocal()
        {
            AppTools.LaunchExe(AppTools.GetRtkViewerFolder(true) + AppTools.RtkViewerFileName, LocalRunParam);
        }
    }
}
