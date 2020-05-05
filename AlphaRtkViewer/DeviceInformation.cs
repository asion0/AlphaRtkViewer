using StqMessageParser;
using MiscUtil.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;

namespace RtkViewer
{
    public class DeviceInformation
    {
        private GpsSerial gps = null;
        public DeviceInformation(GpsSerial g)
        {
            gps = g;
        }

        public enum FinalStage
        {
            None,
            No_Change,
            Device_Error,
            Device_Not_Support,
            Rom_Mode,
            Normal_In_Gl_Rover,
            Normal_In_Gl_Base,
            Normal_In_Bd_Rover,
            Normal_In_Bd_Base,
            Normal_In_Odr,
            Normal_In_Phoenix_Rover,
            Normal_In_Phoenix_Base,
            Device_FirstTimeout,
            Rom_Mode_Phoenix,
            Viewer_Mode,
        }

        private FinalStage finalStage = FinalStage.None;
        public FinalStage GetFinalStage() { return finalStage; }
        public void SetFinalStage(FinalStage fs) { finalStage = fs; }

        private int DefaultCmdTimeout()
        {
            return (40960000 + 10000000) / gps.SerialPort.BaudRate + 1000;
        }

        class VersionInfo
        { 
            public byte[] sVer = new byte[4];
            public byte[] kVer = new byte[4];
            public byte[] rev = new byte[4];
            public UInt16 crc = 0;
        }
        VersionInfo masterVer = new VersionInfo();
        VersionInfo slaveVer = new VersionInfo();

        public byte[] GetKernelVersion(bool isSlave) { return (isSlave) ? slaveVer.kVer : masterVer.kVer; }
        public byte[] GetSoftwareVersion(bool isSlave) { return (isSlave) ? slaveVer.sVer : masterVer.sVer; }
        public byte[] GetRevision(bool isSlave) { return (isSlave) ? slaveVer.rev : masterVer.rev; }
        public bool IsAlphaFirmware() { return (masterVer.sVer[1] == 10 || masterVer.sVer[1] == 11); }
        public bool IsAlphaPlusFirmware() { return masterVer.sVer[1] == 11; }
        public bool IsAlphaStartKitFirmware() { return masterVer.sVer[1] == 200; }
        public bool IsPhoenixFirmware() { return masterVer.kVer[1] > 2; }
        public bool IsRomMode() { return (GetFinalStage() == FinalStage.Rom_Mode || GetFinalStage() == FinalStage.Rom_Mode_Phoenix); }

        public string GetFormatKernelVersion(bool isSlave)
        {
            return string.Format("{0:00}.{1:00}.{2:00}",
                (isSlave) ? slaveVer.kVer[1] : masterVer.kVer[1],
                (isSlave) ? slaveVer.kVer[2] : masterVer.kVer[2],
                (isSlave) ? slaveVer.kVer[3] : masterVer.kVer[3]);
        }
        public string GetFormatSoftwareVersion(bool isSlave)
        {
            return string.Format("{0:00}.{1:00}.{2:00}",
                (isSlave) ? slaveVer.sVer[1] : masterVer.sVer[1],
                (isSlave) ? slaveVer.sVer[2] : masterVer.sVer[2],
                (isSlave) ? slaveVer.sVer[3] : masterVer.sVer[3]);
        }
        public string GetFormatRevision(bool isSlave)
        {
            return string.Format("{0:0000}{1:00}{2:00}",
                ((isSlave) ? slaveVer.rev[1] : masterVer.rev[1]) + 2000,
                (isSlave) ? slaveVer.rev[2] : masterVer.rev[2],
                (isSlave) ? slaveVer.rev[3] : masterVer.rev[3]);
        }

        public string GetFormatCrc(bool isSlave)
        {
            return string.Format("{0:X4}", (isSlave) ? slaveVer.crc : masterVer.crc);
        }

        public GpsSerial.GPS_RESPONSE QuerySoftwareVersion(bool isSlave)
        {
            GpsSerial.GPS_RESPONSE rep = GpsSerial.GPS_RESPONSE.NONE;
            if (isSlave)
            {
                rep = gps.QueryVersion(DefaultCmdTimeout(), isSlave, ref slaveVer.kVer, ref slaveVer.sVer, ref slaveVer.rev);
            }
            else
            {
                rep = gps.QueryVersion(DefaultCmdTimeout(), isSlave, ref masterVer.kVer, ref masterVer.sVer, ref masterVer.rev);
            }
            return rep;
        }

        public GpsSerial.GPS_RESPONSE QueryCrc(bool isSlave)
        {
            GpsSerial.GPS_RESPONSE rep = GpsSerial.GPS_RESPONSE.NONE;
            if (isSlave)
            {
                rep = gps.QueryCrc(DefaultCmdTimeout(), isSlave, ref slaveVer.crc);
            }
            else
            {
                rep = gps.QueryCrc(DefaultCmdTimeout(), isSlave, ref masterVer.crc);
            }
            return rep;
        }

        private byte updateRate;
        public byte GetUpdateRate() { return updateRate; }
        public GpsSerial.GPS_RESPONSE QueryUpdateRate()
        {
            GpsSerial.GPS_RESPONSE rep = gps.QueryUpdateRate(DefaultCmdTimeout(), ref updateRate);
#if DEBUG
            if(rep != GpsSerial.GPS_RESPONSE.ACK)
            {
                updateRate = 1;
                rep = GpsSerial.GPS_RESPONSE.ACK;
            }
#endif
            return rep;
        }

        private bool bootFailed = false;
        private bool bootRom = false;
        public bool GetBootFailedFlag() { return bootFailed; }
        public bool GetBootRomFlag() { return bootRom; }
        public GpsSerial.GPS_RESPONSE QueryBootStatus()
        {
            GpsSerial.GPS_RESPONSE rep = gps.QueryBootStatus(DefaultCmdTimeout(), ref bootFailed, ref bootRom);
            return rep;
        }

        private int drPredictRate = 0;
        private bool isDrFw = false;
        public int GetDrPredictUpdateRate() { return drPredictRate; }
        public bool IsDrFirmware() { return isDrFw; }
        public GpsSerial.GPS_RESPONSE QueryDrPredictUpdateRate()
        {
            GpsSerial.GPS_RESPONSE rep = gps.QueryDrPredictUpdateRate(DefaultCmdTimeout(), ref drPredictRate);
            if (rep == GpsSerial.GPS_RESPONSE.ACK)
                isDrFw = true;
            return rep;
        }

        private GpsSerial.RtkModeInfo rtkInfo = new GpsSerial.RtkModeInfo();
        private bool isRtkFw = false;
        public GpsSerial.RtkModeInfo GetRtkInfo() { return rtkInfo; }
        public bool IsRtkFirmware() { return isRtkFw; }
        public bool IsRtkBaseMode() { return rtkInfo.rtkMode == GpsSerial.RtkModeInfo.RtkMode.RTK_Base; }
        public bool IsRtkRoverMode() { return rtkInfo.rtkMode == GpsSerial.RtkModeInfo.RtkMode.RTK_Rover; }
        public GpsSerial.RtkModeInfo.RtkOperationMode GetRtkOperationMode() { return rtkInfo.optMode; }
        public GpsSerial.GPS_RESPONSE QueryRtkMode()
        {
            GpsSerial.GPS_RESPONSE rep = gps.QueryRtkMode(DefaultCmdTimeout(), ref rtkInfo);
#if DEBUG
            rep = GpsSerial.GPS_RESPONSE.ACK;
#endif
            if (rep == GpsSerial.GPS_RESPONSE.ACK)
                isRtkFw = true;
            return rep;
        }

        private GpsSerial.ConstellationType ctType = 0;
        public GpsSerial.ConstellationType GetConstellationType() { return ctType; }
        public bool IsGlonassModule() { return ((ctType & GpsSerial.ConstellationType.GLONASS) != 0); }
        public bool IsBeidouModule() { return ((ctType & GpsSerial.ConstellationType.BEIDOU) != 0); }
        public bool IsGalileoModule() { return ((ctType & GpsSerial.ConstellationType.GALILEO) != 0); }
        public GpsSerial.GPS_RESPONSE QueryConstellationType()
        {
            GpsSerial.GPS_RESPONSE rep = gps.QueryConstellationType(DefaultCmdTimeout(), ref ctType);
            return rep;
        }

        private GpsSerial.MessageType msgType = 0;
        public GpsSerial.MessageType GetMessageType() { return msgType; }
        public string GetMessageTypeString()
        {
            switch (msgType)
            {
                case GpsSerial.MessageType.NoOutput:
                    return "No Output";
                case GpsSerial.MessageType.NMEA:
                    return "NMEA";
                case GpsSerial.MessageType.UavBinary:
                    return "UAV Binary";
                case GpsSerial.MessageType.Binary:
                    return "Binary";
            }
            return "";
        }

        public GpsSerial.GPS_RESPONSE QueryMessageType()
        {
            GpsSerial.GPS_RESPONSE rep = gps.QueryMessageType(DefaultCmdTimeout(), ref msgType);
            return rep;
        }

        private byte[] miscInfo = new byte[10];
        public enum LicenseType
        {
            None,
            Monthly,
            OneYear,
            Perpetual,
        }

        public LicenseType GetLicenseType()
        {
            if (miscInfo[5] == 0 && miscInfo[6] == 0 && miscInfo[7] == 0)
            {
                return LicenseType.Perpetual;
            }
            else if (miscInfo[2] > 0 && miscInfo[3] <= 12 && miscInfo[4] <= 31 &&
                miscInfo[5] > 0 && miscInfo[6] <= 12 && miscInfo[7] <= 31)
            {
                DateTime start = new DateTime(miscInfo[2] + 2000, miscInfo[3], miscInfo[4]);
                DateTime end = new DateTime(miscInfo[5] + 2000, miscInfo[6], miscInfo[7]);
                TimeSpan ts = end - start;

                if (ts.Days < 45)
                {
                    return LicenseType.Monthly;
                }
                else
                {
                    return LicenseType.OneYear;
                }
            }
            return LicenseType.None;
        }

        public string GetMiscInfoString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in miscInfo)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString().Substring(0, sb.Length - 4);
        }
        public int GetMiscStartY() { return miscInfo[2] + 2000; }
        public int GetMiscStartM() { return miscInfo[3]; }
        public int GetMiscStartD() { return miscInfo[4]; }
        public int GetMiscEndY() { return miscInfo[5] + 2000; }
        public int GetMiscEndM() { return miscInfo[6]; }
        public int GetMiscEndD() { return miscInfo[7]; }

        public bool IsValidateMiscTime(DateTime utc)
        {
            if (GetLicenseType() == LicenseType.None)
            {
                return false;
            }
            else if (GetLicenseType() == LicenseType.Perpetual)
            {
                return true;
            }
            DateTime start = new DateTime(miscInfo[2] + 2000, miscInfo[3], miscInfo[4], 0, 0, 0);
            DateTime end = new DateTime(miscInfo[5] + 2000, miscInfo[6], miscInfo[7], 23, 59, 59);

            return (utc >= start && utc <= end);
        }

        public GpsSerial.GPS_RESPONSE QueryMiscInformation()
        {
            GpsSerial.GPS_RESPONSE rep = gps.QueryMiscInformation(DefaultCmdTimeout(), ref miscInfo);
#if DEBUG
            return GpsSerial.GPS_RESPONSE.ACK;
#else
            return rep;
#endif
        }

        private class ParserClass
        {
            public MessageParser.ParsingResultHandler handler;
            public void RunParser()
            {
                parser.parsingResultHandler += handler;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-TW");

                while (true)
                {
                    if (_dataInQueue.Count == 0)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    byte[] data = null;
                    lock (_dataInQueueLock)
                    {
                        data = _dataInQueue.Dequeue();
                    }
                    parser.Parsing(data);
                }
            }

            private MessageParser parser = new MessageParser();
            public void EnableParserSaveDeviceOutput(bool b) { parser.SaveDeviceOutput(b); }
            public void ParserDataInHandler(object sender, SerialDataArgs args)
            {
                lock (_dataInQueueLock)
                {
                    _dataInQueue.Enqueue(args.SerialByte);
                }
            }

            private static Queue<byte[]> _dataInQueue = new Queue<byte[]>();
            private static object _dataInQueueLock = new object();
        }

        private ParserClass parser = new ParserClass();
        public SerialDataInHandler GetDataInHandler() { return parser.ParserDataInHandler; }
        private Thread parserThread = null;
        public void EnableParserSaveDeviceOutput(bool b) { parser.EnableParserSaveDeviceOutput(b); }
        public bool RunMessageParser(MessageParser.ParsingResultHandler h)
        {
            if (parserThread == null)
            {
                parser.handler = h;
                parserThread = new Thread(parser.RunParser);
                parserThread.Start();
            }
            return true;
        }

        public void Close()
        {
            StopMessageParser();
        }

        public void StopMessageParser()
        {
            if (parserThread != null)
            {
                parserThread.Abort();
                parserThread = null;
            }
        }
    }
}
