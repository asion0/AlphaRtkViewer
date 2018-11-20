using MiscUtil.App;
using System;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace RtkViewerUpdater
{
    static class Program
    {
        public static AppInfo appInfo = null;
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppTools.DebugTitle = "[RTKU]";
            if (args.Length < 1)
            {
                return;
            }

            string xmlString;
            if (args[0].StartsWith("<STQXML>"))
            {
                xmlString = args[0];
            }
            else
            {
                xmlString = Encoding.GetEncoding("utf-8").GetString(Convert.FromBase64String(args[0]));
            }
            appInfo = AppTools.GetAppInfoFromXml(xmlString);
            if (!appInfo.Validate())
            {
                return;
            }
            
            AppTools.CreateMutex(AppTools.RtkViewerUpdaterMutexName);
            AppTools.ShowDebug("RTK Updater starts");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RtkViewerUpdaterForm());
            AppTools.ReleaseMutex();
        }

    }
}
