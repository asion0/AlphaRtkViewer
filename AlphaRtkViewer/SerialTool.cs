using MiscUtil.App;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Text;

namespace RtkViewer
{
    class SerialTool
    {
        private static List<ComPortInfo> listComPortInfo1 = new List<ComPortInfo>();
        private static List<ComPortInfo> listComPortInfo2 = new List<ComPortInfo>();

        //Interface for COM port list
        public static void ReflashComList()
        {
            if (listComPortInfo1.Count == 0)
            {
                GetComPortList(listComPortInfo1);
                listComPortInfo2.Clear();
            }
            else
            {
                listComPortInfo2.Clear();
                GetComPortList(listComPortInfo2);
                listComPortInfo1.Clear();
            }
        }

        public static int GetPortCount()
        {
            return (listComPortInfo2.Count > 0) ? listComPortInfo2.Count : listComPortInfo1.Count;
        }

        public static string GetPortName(int index)
        {
            return (listComPortInfo2.Count > 0) ? listComPortInfo2[index].portName : listComPortInfo1[index].portName;
        }

        public static string GetPortDescription(int index)
        {
            return (listComPortInfo2.Count > 0) ? listComPortInfo2[index].description : listComPortInfo1[index].description;
        }

        public static int GetPortNumber(int index)
        {
            return (listComPortInfo2.Count > 0) ? listComPortInfo2[index].no : listComPortInfo1[index].no;
        }

        //Interface for baud rate list
        private static int[] baudTable = { 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
        public static int GetIndexOfBaudRate(int baudRate)
        {
            for (int i = 0; i < baudTable.GetLength(0); i++)
            {
                if (baudTable[i] == baudRate)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetBaudRateByIndex(int index)
        {
            return baudTable[index];
        }

        public static int GetBaudRateCount()
        {
            return baudTable.Length;
        }

        //COM Port List
        private static void GetComPortList(List<ComPortInfo> l)
        {
            try
            {
                //ManagementObjectSearcher searcher =
                //    new ManagementObjectSearcher("root\\CIMV2",
                //    "SELECT * FROM Win32_PnPEntity");
                //foreach (ManagementObject queryObj in searcher.Get())
                //{
                //    if (queryObj != null && queryObj["Caption"] != null &&
                //        queryObj["Caption"].ToString().Contains("(COM"))
                //    {
                //        l.Add(GenerateComPortInfo(queryObj["Caption"].ToString()));
                //    }
                //}
                string[] comArray = SerialPort.GetPortNames();
                foreach(string p in comArray)
                {
                    l.Add(GenerateComPortInfo(p));
                }
                l.Sort(CmpUserInfo);
            }
            catch (ManagementException ex)
            {
                AppTools.ShowDebug(ex.ToString());
            }
        }

        private static ComPortInfo GenerateComPortInfo(string comDesc)
        {
            //char[] delimiterChars = { '(', ')' };
            //string[] param = comDesc.Split(delimiterChars);
            //string comName = param[param.Length - 2];
            //string noString = comName.Substring(3);
            //return new ComPortInfo(param[0], param[param.Length - 2], Convert.ToInt32(noString));
            string noString = comDesc.Substring(3);
            return new ComPortInfo(comDesc, comDesc, Convert.ToInt32(noString));
        }

        private static int CmpUserInfo(ComPortInfo a, ComPortInfo b)
        {
            return a.no - b.no; // >0, ==0 and <0 means greater than, equal to, less than sort by no (b-a)
        }

        private class ComPortInfo
        {
            public string description;
            public string portName;
            public int no;
            public ComPortInfo(string desc, string name, int no)
            {
                this.description = desc;
                this.portName = name;
                this.no = no;
            }
        }

    }
}
