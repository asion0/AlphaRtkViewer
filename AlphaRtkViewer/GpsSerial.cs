using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;
using MiscUtil.Conversion;
using System.Collections.Generic;
using MiscUtil.App;

namespace RtkViewer
{
    public class BinaryCommand
    {
        private const int CommandExtraSize = 7;
        private const int CommandHeaderSize = 4;

        private byte[] commandData;

        public BinaryCommand()
        {

        }

        public BinaryCommand(byte[] data)
        {
            SetData(data);
        }

        private void SetData(byte[] data)
        {
            commandData = new byte[CommandExtraSize + data.Length];
            data.CopyTo(commandData, CommandHeaderSize);
        }

        public byte[] GetBuffer()
        {
            byte checkSum = 0;
            for (int i = 0; i < commandData.Length - CommandExtraSize; ++i)
            {
                checkSum ^= commandData[i + CommandHeaderSize];
            }

            commandData[0] = (byte)0xA0;
            commandData[1] = (byte)0xA1;
            commandData[2] = (byte)((commandData.Length - CommandExtraSize) >> 8);
            commandData[3] = (byte)((commandData.Length - CommandExtraSize) & 0xff);
            commandData[commandData.Length - 3] = checkSum;
            commandData[commandData.Length - 2] = (byte)0x0D;
            commandData[commandData.Length - 1] = (byte)0x0A;
            return commandData;
        }

        public int Size()
        {
            return commandData.Length;
        }
    }

    public sealed class GpsSerial
    {
        private GpsSerial() { }
        private static GpsSerial _GpsSerial = new GpsSerial();
        public static GpsSerial Default
        {
            get { return _GpsSerial; }
        }

        private Queue<byte[]> DataQueue = new Queue<byte[]>();

        private SerialPort _SerialPort = null;
        public SerialPort SerialPort
        {
            get
            {
                if (null == _SerialPort)
                {
                    _SerialPort = new SerialPort();
                    //_SerialPort.DataReceived += _SerialPort_DataReceived;
                }
                return _SerialPort;
            }
        }

        private string errorMessage = "";
        public enum GPS_RESPONSE
        {
            NONE,
            ACK,    //0x83
            NACK,      //0x84
            FORMAT_ERROR,   //0x85
            TIMEOUT,
            CHKSUM_FAIL,
            OK,
        }

        public string GetErrorMessage() { return errorMessage; }
        public int GetBaudRateIndex() { return SerialTool.GetIndexOfBaudRate(SerialPort.BaudRate); }
        public string GetPortName() { return SerialPort.PortName; }
        public bool Open(string comName, int baudIdx)
        {
            if (SerialPort.IsOpen)
            {
                SerialPort.Close();
            }

            SerialPort.PortName = comName;
            SerialPort.BaudRate = SerialTool.GetBaudRateByIndex(baudIdx);

            try
            {
                SerialPort.Open();
                SerialPort.ReadTimeout = 5000;
                SerialPort.WriteTimeout = 5000;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _SerialPort.DataReceived -= _SerialPort_DataReceived;
                //_Timer.Elapsed -= _Timer_Elapsed;
                SerialPort.Dispose();
                //Timer.Dispose();
                return false;
            }
            return true;
        }

        public bool ReOpen(int baudIdx)
        {
            return Open(SerialPort.PortName, baudIdx);
        }

        public void ClearInBuffer()
        {
            SerialPort.DiscardInBuffer();
        }

        private event SerialDataInHandler serialDataInEvent;
        public void InstallDataReceiver(SerialDataInHandler dataInEvent)
        {
            serialDataInEvent += dataInEvent;
            SerialPort.DataReceived += _SerialPort_DataReceived;
        }

        public void UninstallDataReceiver()
        {
            serialDataInEvent = null;
            SerialPort.DataReceived -= _SerialPort_DataReceived;
        }

        private bool disableDataReceiver = false;
        public void DisableDataReceiver(bool b) { disableDataReceiver = b; }
        private void _SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (disableDataReceiver)
                return;
            try
            {
                var num = SerialPort.BytesToRead;
                byte[] buffer = new byte[num];
                SerialPort.Read(buffer, 0, num);
                if (buffer.Length > 0)
                {
                    serialDataInEvent?.Invoke(this, new SerialDataArgs(buffer));
                }
            }
            catch (Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
            }
        }

        public void Close()
        {
            if (!SerialPort.IsOpen)
                return;

            SerialPort.DiscardOutBuffer();
            SerialPort.DiscardInBuffer();
            SerialPort.Close();
            SerialPort.DataReceived -= new SerialDataReceivedEventHandler(_SerialPort_DataReceived);
            _SerialPort = null;
            serialDataInEvent = null;

            //serialDataInEvent = null;
            //_SerialPort.DataReceived -= _SerialPort_DataReceived;

            //SerialPort.Close();
            //SerialPort.Dispose();
            //_SerialPort = null;
        }

        public enum RestartMode
        {
            None = 0,
            HotStart = 1,
            WarmStart = 2,
            ColdStart = 3,
        }

        public GPS_RESPONSE SystemRestart(int timeout, RestartMode mode, double lat, double lon, float alt)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[15];
            cmdData[0] = 0x01;
            cmdData[1] = (byte)mode;
            cmdData[2] = (byte)(DateTime.Now.Year >> 8);
            cmdData[3] = (byte)(DateTime.Now.Year & 0xFF);
            cmdData[4] = (byte)(DateTime.Now.Month);
            cmdData[5] = (byte)(DateTime.Now.Day);
            cmdData[6] = (byte)(DateTime.Now.Hour);
            cmdData[7] = (byte)(DateTime.Now.Minute);
            cmdData[8] = (byte)(DateTime.Now.Second);
            cmdData[9] = (byte)((int)(lat * 100) >> 8);
            cmdData[10] = (byte)((int)(lat * 100) & 0xFF);
            cmdData[11] = (byte)((int)(lon * 100) >> 8);
            cmdData[12] = (byte)((int)(lon * 100) & 0xFF);
            cmdData[13] = (byte)((int)alt >> 8);
            cmdData[14] = (byte)((int)alt & 0xFF);

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            return retval;
        }

        public GPS_RESPONSE SetFactoryDefaults(int timeout)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[2];
            cmdData[0] = 0x04;
            cmdData[1] = 0x01;

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            return retval;
        }

        public enum Attributes
        {
            Sram = 0,
            SramAndFlash = 1,
            Temporarily = 2
        }

        public GPS_RESPONSE ConfigureSerialPort(int timeout, int baudIdx, Attributes att)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[4];
            cmdData[0] = 0x05;
            cmdData[1] = 0x00;
            cmdData[2] = (byte)baudIdx;
            cmdData[3] = (byte)att;
            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            return retval;
        }

        public GPS_RESPONSE ConfigureUpdateRate(int timeout, int updateRate, Attributes att)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[3];
            cmdData[0] = 0x0e;
            cmdData[1] = (byte)updateRate;
            cmdData[2] = (byte)att;
            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            return retval;
        }

        public GPS_RESPONSE ConfigureMessageType(int timeout, int type, Attributes att)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[3];
            cmdData[0] = 0x09;
            cmdData[1] = (byte)type;
            cmdData[2] = (byte)att;
            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            return retval;
        }
        
        public GPS_RESPONSE SetLicenseKey(int timeout, byte[] license)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[19];
            cmdData[0] = 0x7A;
            cmdData[1] = 0x08;
            cmdData[2] = 0x7D;
            Array.Copy(license, 0, cmdData, 3, 16);
            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            return retval;
        }

        private static int[] updateRateTable = { 1, 2, 4, 5, 10, 20, 8 };
        public static int GetIndexOfUpdateRate(int ur)
        {
            for (int i = 0; i < updateRateTable.GetLength(0); ++i)
            {
                if (updateRateTable[i] == ur)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetUpdateRateByIndex(int index)
        {
            return updateRateTable[index];
        }

        public class MeasurementDataOutSetting
        {
            public static byte GetSubFrameFlag(bool gps, bool glonass, bool beidou)
            {
                byte subFrame = 0;
                if (gps)
                {
                    subFrame |= 0x1;
                }
                if (glonass)
                {
                    subFrame |= 0x2;
                }
                if (beidou)
                {
                    subFrame |= 0x8;
                }
                return subFrame;
            }

            public int updaterateIdx;
            public bool measTime;
            public bool rawMeas;
            public bool svChStatus;
            public bool rcvChStatus;
            public byte subFrame;
            public bool extRawMeas;
        }

        public GPS_RESPONSE ConfigBinaryMeasurementDataOut(int timeout, MeasurementDataOutSetting mdos, Attributes att)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[9];
            cmdData[0] = 0x1E;
            cmdData[1] = (byte)mdos.updaterateIdx;
            cmdData[2] = (byte)((mdos.measTime) ? 1 : 0);
            cmdData[3] = (byte)((mdos.rawMeas) ? 1 : 0);
            cmdData[4] = (byte)((mdos.svChStatus) ? 1 : 0);
            cmdData[5] = (byte)((mdos.rcvChStatus) ? 1 : 0);
            cmdData[6] = mdos.subFrame;
            cmdData[7] = (byte)((mdos.extRawMeas) ? 1 : 0);
            cmdData[8] = (byte)att;

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            return retval;
        }

        public class RtcmDataOutSetting
        {
            public bool enable;
            public int updaterateIdx;
            public bool rtcm1005;
            public bool rtcm1077;
            public bool rtcm1087;
            public bool rtcm1107;
            public bool rtcm1117;
            public bool rtcm1127;
        }

        public GPS_RESPONSE ConfigBinaryRtcmDataOut(int timeout, RtcmDataOutSetting rdos, Attributes att)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[17];
            cmdData[0] = 0x20;
            cmdData[1] = (byte)((rdos.enable) ? 1 : 0);
            cmdData[2] = (byte)rdos.updaterateIdx;
            cmdData[3] = (byte)((rdos.rtcm1005) ? 1 : 0);
            cmdData[4] = (byte)((rdos.rtcm1077) ? 1 : 0);
            cmdData[5] = (byte)((rdos.rtcm1087) ? 1 : 0);
            cmdData[7] = (byte)((rdos.rtcm1107) ? 1 : 0);
            cmdData[8] = (byte)((rdos.rtcm1117) ? 1 : 0);
            cmdData[9] = (byte)((rdos.rtcm1127) ? 1 : 0);
            cmdData[16] = (byte)att;

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            return retval;
        }

        public GPS_RESPONSE QueryVersion(int timeout, bool isSlave, ref byte[] kVer, ref byte[] sVer, ref byte[] rev)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[2];
            cmdData[0] = 0x02;
            cmdData[1] = (byte)((isSlave) ? 0x02 : 0x01);

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            byte[] retCmd = new byte[128];
            retval = WaitReturnCommand(0x80, retCmd, timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            Array.Copy(retCmd, 6, kVer, 0, 4);
            Array.Copy(retCmd, 10, sVer, 0, 4);
            Array.Copy(retCmd, 14, rev, 0, 4);
            return retval;
        }

        public GPS_RESPONSE QueryCrc(int timeout, bool isSlave, ref UInt16 crc)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[2];
            cmdData[0] = 0x03;
            cmdData[1] = (byte)((isSlave) ? 0x02 : 0x01);

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            byte[] retCmd = new byte[128];
            retval = WaitReturnCommand(0x81, retCmd, timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            BigEndianBitConverter c = new BigEndianBitConverter();
            crc = c.ToUInt16(retCmd, 6);
            return retval;
        }

        public GPS_RESPONSE QueryUpdateRate(int timeout, ref byte updateRate)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[1];
            cmdData[0] = 0x10;

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            byte[] retCmd = new byte[128];
            retval = WaitReturnCommand(0x86, retCmd, timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            updateRate = retCmd[5];
            return retval;
        }

        public GPS_RESPONSE QueryBootStatus(int timeout, ref bool bootFailed, ref bool bootRom)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[2];
            cmdData[0] = 0x64;
            cmdData[1] = 0x01;

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            byte[] retCmd = new byte[128];
            retval = WaitReturnCommand(0x64, retCmd, timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }
            bootFailed = (retCmd[6] == 1);
            bootRom = (retCmd[7] == 0);
            return retval;
        }

        public GPS_RESPONSE QueryDrPredictUpdateRate(int timeout, ref int rate)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[2];
            cmdData[0] = 0x6c;
            cmdData[1] = 0x01;

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            byte[] retCmd = new byte[128];
            retval = WaitReturnCommand(0x6c, retCmd, timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }
            rate = retCmd[6];
            return retval;
        }

        public class RtkModeInfo
        {
            static public RtkModeInfo GetRoverNormalSetting()
            {
                RtkModeInfo r = new RtkModeInfo();
                r.rtkMode = RtkMode.RTK_Rover;
                r.optMode = RtkOperationMode.Rover_Normal;
                return r;
            }

            public RtkModeInfo()
            {
            }

            public RtkModeInfo(RtkModeInfo r)
            {
                rtkMode = r.rtkMode;
                optMode = r.optMode;
                baselineLength = r.baselineLength;
                surveyLength = r.surveyLength;
                surveyStdDivThr = r.surveyStdDivThr;
                savedLat = r.savedLat;
                savedLon = r.savedLon;
                savedAlt = r.savedAlt;
                runtimeOptMode = r.runtimeOptMode;
                runtimeSurveyLength = r.runtimeSurveyLength;
            }

            public enum RtkMode
            {
                None = -1,
                RTK_Rover = 0,
                RTK_Base = 1,
                RTK_Advance_Moving_Base = 2,
            }

            public RtkMode rtkMode = RtkMode.None;
            public string GetRtkModeString(RtkMode rm)
            {
                if (rtkMode == RtkMode.RTK_Base)
                {
                    return "RTK Base Mode";
                }
                else if (rtkMode == RtkMode.RTK_Rover)
                {
                    return "RTK Rover Mode";
                }
                return "------";
            }

            public enum RtkOperationMode
            {
                None = -1,
                Base_Kinematic = 0,
                Base_Survey = 1,
                Base_Static = 2,
                Rover_Normal = 0,
                Rover_Float = 1,
                Rover_MovingBase = 2,
                AMB_Normal = 0,
                AMB_Float = 1,
            }
            public RtkOperationMode optMode = RtkOperationMode.None;
            //For Rover only
            public UInt32 baselineLength = 0;

            //For Base only
            public UInt32 surveyLength = 0;
            public UInt32 surveyStdDivThr = 0;

            //For Base, Static operation
            public double savedLat;
            public double savedLon;
            public float savedAlt;

            //For Base
            public RtkOperationMode runtimeOptMode = RtkOperationMode.None;
            public string GetOperationModeString(RtkOperationMode op)
            {
                if (rtkMode == RtkMode.RTK_Base)
                {
                    switch (op)
                    {
                        case RtkOperationMode.Base_Kinematic:
                            return "Kinematic";
                        case RtkOperationMode.Base_Static:
                            return "Static";
                        case RtkOperationMode.Base_Survey:
                            return "Survey";
                    }
                }
                else if (rtkMode == RtkMode.RTK_Rover)
                {
                    switch (op)
                    {
                        case RtkOperationMode.Rover_Normal:
                            return "Normal";
                        case RtkOperationMode.Rover_Float:
                            return "Float";
                        case RtkOperationMode.Rover_MovingBase:
                            return "Moving Base";
                    }
                }
                return "------";
            }

            public UInt32 runtimeSurveyLength = 0;

            //ToString
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("RtkMode:{0}\n", rtkMode.ToString());
                sb.AppendFormat("OpMode:{0}\n", optMode.ToString());
                sb.AppendFormat("BaselineLen:{0}\n", baselineLength.ToString());
                sb.AppendFormat("SurveyLen:{0}\n", surveyLength.ToString());
                sb.AppendFormat("SurveyStdDivThr:{0}\n", surveyStdDivThr.ToString());
                sb.AppendFormat("SavedLat:{0}\n", savedLat.ToString());
                sb.AppendFormat("SavedLon:{0}\n", savedLon.ToString());
                sb.AppendFormat("SavedAlt:{0}\n", savedAlt.ToString());
                sb.AppendFormat("RuntimeOpMode:{0}\n", runtimeOptMode.ToString());
                sb.AppendFormat("RuntimeSurveyLen:{0}\n", runtimeSurveyLength.ToString());

                return sb.ToString();
            }
        }

        public GPS_RESPONSE ConfigureRtkMode(int timeout, RtkModeInfo rtkInfo, Attributes att)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] tmp;
            byte[] cmdData = new byte[37];
            cmdData[0] = 0x6A;
            cmdData[1] = 0x06;
            cmdData[2] = (byte)rtkInfo.rtkMode;
            cmdData[3] = (byte)rtkInfo.optMode;
            //Base mode, Survey opt, Survey Length
            tmp = BitConverter.GetBytes(rtkInfo.surveyLength);
            Array.Reverse(tmp);
            Array.Copy(tmp, 0, cmdData, 4, 4);
            //Base mode, Survey opt, Standard Deviation
            tmp = BitConverter.GetBytes(rtkInfo.surveyStdDivThr);
            Array.Reverse(tmp);
            Array.Copy(tmp, 0, cmdData, 8, 4);
            //Base mode, Static opt, Lat
            tmp = BitConverter.GetBytes(rtkInfo.savedLat);
            Array.Reverse(tmp);
            Array.Copy(tmp, 0, cmdData, 12,  8);
            //Base mode, Static opt, Lon
            tmp = BitConverter.GetBytes(rtkInfo.savedLon);
            Array.Reverse(tmp);
            Array.Copy(tmp, 0, cmdData, 20, 8);
            //Base mode, Static opt, Lat
            tmp = BitConverter.GetBytes(rtkInfo.savedAlt);
            Array.Reverse(tmp);
            Array.Copy(tmp, 0, cmdData, 28, 4);
            //Base mode, Kinematic opt, Baseline length constraint
            tmp = BitConverter.GetBytes(0F);
            Array.Reverse(tmp);
            Array.Copy(tmp, 0, cmdData, 32, 4);

            cmdData[36] = (byte)att;
            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            return retval;
        }

        public GPS_RESPONSE QuerySerialNumber(int timeout, ref byte[] serialNo)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[2];
            cmdData[0] = 0x64;
            cmdData[1] = 0x76;

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            byte[] retCmd = new byte[128];
            retval = WaitReturnCommand(0x64, retCmd, timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            Array.Copy(retCmd, 6, serialNo, 0, 8);
            return retval;
        }

        public GPS_RESPONSE QueryRtkMode(int timeout, ref RtkModeInfo rtkInfo)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[2];
            cmdData[0] = 0x6a;
            cmdData[1] = 0x07;

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            byte[] retCmd = new byte[128];
            retval = WaitReturnCommand(0x6a, retCmd, timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            rtkInfo.rtkMode = (RtkModeInfo.RtkMode)retCmd[6];
            rtkInfo.optMode = (RtkModeInfo.RtkOperationMode)retCmd[7];
            if(rtkInfo.rtkMode != RtkModeInfo.RtkMode.RTK_Base)
            {
                rtkInfo.baselineLength = BitConverter.ToUInt32(retCmd, 36);
            }
            else
            {
                BigEndianBitConverter converter = new BigEndianBitConverter();
                rtkInfo.surveyLength = converter.ToUInt32(retCmd, 8);
                rtkInfo.surveyStdDivThr = converter.ToUInt32(retCmd, 12);
                if(rtkInfo.optMode == RtkModeInfo.RtkOperationMode.Base_Static)
                {
                    rtkInfo.savedLat = converter.ToDouble(retCmd, 16);
                    rtkInfo.savedLon = converter.ToDouble(retCmd, 24);
                    rtkInfo.savedAlt = converter.ToSingle(retCmd, 32);
                }
                rtkInfo.runtimeOptMode = (RtkModeInfo.RtkOperationMode)retCmd[36];
                rtkInfo.runtimeSurveyLength = converter.ToUInt32(retCmd, 37);
            }
            return retval;
        }

        public GPS_RESPONSE SetRegister(int timeout, UInt32 regAddr, UInt32 data)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;

            byte[] cmdData = new byte[9];
            byte[] recv_buff = new byte[128];

            cmdData[0] = 0x72;
            cmdData[1] = (byte)(regAddr >> 24 & 0xFF);
            cmdData[2] = (byte)(regAddr >> 16 & 0xFF);
            cmdData[3] = (byte)(regAddr >> 8 & 0xFF);
            cmdData[4] = (byte)(regAddr & 0xFF);
            cmdData[5] = (byte)(data >> 24 & 0xFF);
            cmdData[6] = (byte)(data >> 16 & 0xFF);
            cmdData[7] = (byte)(data >> 8 & 0xFF);
            cmdData[8] = (byte)(data & 0xFF);

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            return retval;
        }

        public GPS_RESPONSE EnterSlavePassThrough(int timeout, bool isEnter, bool romMode)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            const int commandLength = 4;
            //In : a0 a1 00 04 7a 08 01 01 72 0d 0a 
            byte[] cmdData = new byte[commandLength];
            cmdData[0] = 0x7a;
            cmdData[1] = 0x08;
            cmdData[2] = 1;
            cmdData[3] = (isEnter) ? ((romMode) ? (byte)1 : (byte)2) : (byte)0;

            BinaryCommand cmd = new BinaryCommand(cmdData);
            if (isEnter)
                retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            else
                retval = SendCmdAckForPassthroughBack(cmd.GetBuffer(), cmd.Size(), timeout, commandLength);

            return retval;
        }

        public GPS_RESPONSE SendLoaderDownload(int timeout, int downloadBaudIdx, bool useBinaryCommand)
        {
            GPS_RESPONSE rep = GPS_RESPONSE.NONE;
            if (useBinaryCommand)
            {
                rep = ExternalLoaderDownload(timeout, downloadBaudIdx);
                if (rep == GPS_RESPONSE.ACK)
                {
                    rep = GPS_RESPONSE.OK;
                }
                return rep;
            }

            String cmd = "$LOADER DOWNLOAD";
            rep = SendStringCmdAck(cmd, cmd.Length, timeout, "OK\0");
            return rep;
        }

        public GPS_RESPONSE ExternalLoaderDownload(int timeout, int downloadBaudRate)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[7];

            cmdData[0] = 0x64;
            cmdData[1] = 0x1B;
            cmdData[2] = (byte)downloadBaudRate;
            cmdData[3] = 0;
            cmdData[4] = 0;
            cmdData[5] = 0;
            cmdData[6] = 0;

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            return retval;
        }

        public GPS_RESPONSE UploadLoader(int timeout, String s)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            String[] delimiterChars = { "\r\n" };
            String[] lines = s.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);

            foreach (String l in lines)
            {
                String line = l + (char)0x0a;
                SendStringCmdNoAck(line, line.Length);
            }
            retval = WaitStringAck(timeout, "END\0");
            return retval;
        }

        public GPS_RESPONSE SendRomBinSize(int timeout, int length, byte checksum)
        {//"BINSIZE = %d Checksum = %d %lld ", promLen, mycheck, check);
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            String cmd = "BINSIZE = " + length.ToString() + " Checksum = " + checksum.ToString() +
                " " + (length + checksum).ToString() + " ";

            retval = SendStringCmdAck(cmd, cmd.Length, timeout, "OK\0");
            return retval;
        }

        public string SendDumpLoaderCmd(int timeout, string cmd)
        {
            //List<string> retData = new List<string>();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SendStringCmdNoAck(cmd, cmd.Length);

            const int ReadCount = 5;
            int strIndex = 0;
            char[] buf = new char[ReadCount + 1];
            while(sw.ElapsedMilliseconds <= timeout)
            {
                //SerialPort.ReadTimeout = timeout - (int)sw.ElapsedMilliseconds;
                //retData.Add(SerialPort.ReadLine());
                if (SerialPort.BytesToRead > 0)
                {
                    buf[strIndex++] = (char)SerialPort.ReadChar();
                }
                if(strIndex >= ReadCount)
                {
                    break;
                }
            }
            return new string(buf);
        }

        public GPS_RESPONSE SendDataWaitStringAck(byte[] data, int start, int len, int timeout, String waitingFor)
        {
            //ClearQueue();
            try
            {
                SerialPort.Write(data, start, len);
            }
            catch (Exception e)
            {
                string s = e.ToString();
                return GPS_RESPONSE.TIMEOUT;
            }
            return WaitStringAck(timeout, waitingFor);
        }

        [Flags]
        public enum ConstellationType : ushort
        {
            None = 0,
            GPS = 1 << 0,
            GLONASS = 1 << 1,
            GALILEO = 1 << 2,
            BEIDOU = 1 << 3,
            GPS_GLONASS = GPS | GLONASS,
            GPS_BEIDOU = GPS | BEIDOU,
        }

        public GPS_RESPONSE QueryConstellationType(int timeout, ref ConstellationType ctType)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[2];
            cmdData[0] = 0x64;
            cmdData[1] = 0x1a;

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            byte[] retCmd = new byte[128];
            retval = WaitReturnCommand(0x64, retCmd, timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }
            ctType = (ConstellationType)retCmd[7];
            return retval;
        }

        public enum MessageType
        {
            NoOutput,
            NMEA,
            Binary,
            UavBinary,
            None,
        }

        public GPS_RESPONSE QueryMessageType(int timeout, ref MessageType msgType)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[1];
            cmdData[0] = 0x16;
            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            byte[] retCmd = new byte[128];
            retval = WaitReturnCommand(0x8C, retCmd, timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }
            msgType = (MessageType)retCmd[5];
            return retval;
        }

        public GPS_RESPONSE QueryMiscInformation(int timeout, ref byte[] misc)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.NONE;
            byte[] cmdData = new byte[3];
            cmdData[0] = 0x7a;
            cmdData[1] = 0x08;
            cmdData[2] = 0x7e;

            BinaryCommand cmd = new BinaryCommand(cmdData);
            retval = SendCmdAck(cmd.GetBuffer(), cmd.Size(), timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            byte[] retCmd = new byte[128];
            retval = WaitReturnCommand(0x7a, retCmd, timeout);
            if (retval != GPS_RESPONSE.ACK)
            {
                return retval;
            }

            for(int i = 0; i < 10; ++i)
            {
                misc[i] = (byte)(retCmd[i + 7] ^ 0xAA);
            }
            return retval;
        }

        private GPS_RESPONSE SendCmdAck(byte[] cmd, int len, int timeout)
        {
            SerialPort.DiscardInBuffer();
            //SerialPort.Write(cmd, 0, len);   //Write at once will cause V8 no response
            for(int i = 0; i < cmd.Length; ++i)
            {
                SerialPort.Write(cmd, i, 1);
            }
            return WaitAck(cmd[4], timeout);
        }

        private GPS_RESPONSE SendStringCmdAck(String cmd, int len, int timeout, String waitingFor)
        {
            SerialPort.DiscardInBuffer();
            SerialPort.NewLine = "\0";
            SerialPort.WriteLine(cmd);
            return WaitStringAck(timeout, waitingFor);
        }

        private void SendStringCmdNoAck(String cmd, int len)
        {
            SerialPort.DiscardInBuffer();
            SerialPort.NewLine = "\0";
            SerialPort.WriteLine(cmd);
            return;
        }

        public GPS_RESPONSE WaitStringAck(int timeout, String waitingFor)
        {
            byte[] buffer = new byte[1];
            bool start = false;
            int ackLen = waitingFor.Length;
            int iter = 0;
            Stopwatch sw = new Stopwatch();

            sw.Reset();
            sw.Start();
            while (sw.ElapsedMilliseconds < timeout)
            {
                if (SerialPort.BytesToRead == 0)
                {
                    Thread.Sleep(10);
                    continue;
                }

                buffer[0] = (byte)SerialPort.ReadByte();

                if (!start && buffer[0] == waitingFor[0])
                {
                    start = true;
                }

                if (!start)
                {
                    continue;
                }

                if (buffer[0] != waitingFor[iter++])
                {
                    start = false;
                    iter = 0;
                    continue;
                }

                if (iter == ackLen)
                {
                    return GPS_RESPONSE.OK;
                }
            }
            return GPS_RESPONSE.TIMEOUT;
        }

        private GPS_RESPONSE SendCmdAckForPassthroughBack(byte[] cmd, int len, int timeout, int cmdLen)
        {
            SerialPort.DiscardInBuffer();
            //SerialPort.Write(cmd, 0, len);   //Write at once will cause V8 no response
            for (int i = 0; i < cmd.Length; ++i)
            {
                SerialPort.Write(cmd, i, 1);
            }
            return WaitAckForPassthroughBack(cmd[4], timeout, cmdLen);
        }

        private GPS_RESPONSE WaitAckForPassthroughBack(byte id, int timeout, int cmdLen)
        {
            const int ReceiveLength = 128;
            byte[] received = new byte[ReceiveLength];
            byte[] buffer = new byte[1];

            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            while (sw.ElapsedMilliseconds < timeout)
            {       //ReadPassthroughBackAck()
                int l = ReadPassthroughBackAck(ref received, timeout);
                if (l > 8)
                {   //最小的Ack封包會有8 bytes
                    if (received[0] == 0xA0 && received[4] == 0x83 && received[5] == id)
                    {
                        return GPS_RESPONSE.ACK;
                    }
                    else if (received[0] == 0xA0 && received[4] == 0x84 && l == 7 + cmdLen)
                    {
                        long spend = sw.ElapsedMilliseconds;
                        return GPS_RESPONSE.NACK;
                    }

                    Array.Clear(received, 0, received.Length);
                    continue;
                }
            }
            return GPS_RESPONSE.TIMEOUT;
        }

        private int ReadPassthroughBackAck(ref byte[] received, int timeout)
        {
            byte buffer;
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            int index = 0;
            byte[] correctAck = { 0xa0, 0xa1, 0x00, 0x04, 0x83, 0x7a, 0x08, 0x01, 0xf0, 0x0d, 0x0a };
            int ackIdx = 0;
            while (sw.ElapsedMilliseconds < timeout)
            {
                if (SerialPort.BytesToRead > 0)
                {
                    buffer = (byte)SerialPort.ReadByte();
                    if (buffer == correctAck[ackIdx])
                    {
                        ++ackIdx;
                        if (ackIdx == correctAck.Length)
                        {
                            Array.Copy(correctAck, received, correctAck.Length);
                            return ackIdx;
                        }
                    }
                    else
                    {
                        ackIdx = 0;
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            return index;
        }

        private GPS_RESPONSE WaitAck(byte id, int timeout)
        {
            const int ReceiveLength = 1024;
            byte[] received = new byte[ReceiveLength];
            byte[] buffer = new byte[1];

            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            while (sw.ElapsedMilliseconds < timeout)
            {
                int l = ReadBinLine(ref received, timeout);
                if (l > 8)
                {   //最小的Ack封包會有8 bytes
                    if (received[0] == 0xA0 && received[4] == 0x83 && received[5] == id)
                    {
                        return GPS_RESPONSE.ACK;
                    }
                    else if (received[0] == 0xA0 && received[4] == 0x84)
                    {
                        return GPS_RESPONSE.NACK;
                    }
                    else if (received[0] == 0xA0 && received[4] == 0x85)
                    {
                        return GPS_RESPONSE.FORMAT_ERROR;
                    }

                    Array.Clear(received, 0, received.Length);
                    continue;
                }
            }
            return GPS_RESPONSE.TIMEOUT;
        }

        private int ReadBinLine(ref byte[] received, int timeout)
        {
            byte buffer;
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            int index = 0;
            int packetLen = 0;
            //timeout = 100000;
            bool startLog = false;
            if(startLog)
            {
                AppTools.ShowDebug("");
            }
            disableDataReceiver = true;
            while (sw.ElapsedMilliseconds < timeout)
            {
                if (SerialPort.BytesToRead > 0)
                {
                    buffer = (byte)SerialPort.ReadByte();
                    if (startLog)
                    {
                        AppTools.ShowDebug(string.Format("{0:X2}:{1}, ", buffer, index));
                    }
                    if ((index == 0 && buffer == 0xA0) || received[0] == 0xA0)
                    {   //從收到A0開始儲存
                        if (index >= received.Length)
                        {   //儲存不下就傳回Timeout
                            disableDataReceiver = false;
                            return index;
                        }
                        if (index == 1 && buffer != 0xA1)
                        {
                            if (buffer == 0xA0)
                            {
                                index = 1;
                            }
                            else
                            {
                                index = 0;
                                received[0] = 0;
                            }
                            startLog = true;
                            continue;
                        }
                        if (index == 2 && buffer > 4)
                        {
                            index = (buffer == 0xA0) ? 1 : 0;
                            continue;
                        }
                        received[index] = buffer;
                        if (index == 3)
                        {
                            packetLen = (received[2] << 8) | received[3];
                        }
                        index++;
                        if (buffer == 0x0A && received[index - 2] == 0x0D)
                        {
                            int b = 0;
                            ++b;
                        }
                        //if (buffer == 0x0A && received[index - 2] == 0x0D)
                        if (buffer == 0x0A && received[index - 2] == 0x0D && (packetLen + 7) == index)
                        {   //收到0x0D, 0x0A後結束
                            disableDataReceiver = false;
                            return index;
                        }
                    }
                    else
                    {   //捨棄非A0開頭的資料
                        continue;
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            disableDataReceiver = false;
            return index;
        }

        private GPS_RESPONSE WaitReturnCommand(byte cmdId, byte[] retCmd, int timeout)
        {
            GPS_RESPONSE retval = GPS_RESPONSE.TIMEOUT;
            byte[] received = new byte[1024];

            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            while (sw.ElapsedMilliseconds < timeout)
            {
                int l = ReadBinLine(ref received, timeout);
                if (cmdId == CheckBinaryCommand(received, l))
                {
                    if (retCmd.Length > l)
                    {
                        Array.Copy(received, retCmd, l);
                        return GPS_RESPONSE.ACK;
                    }
                    else
                    {
                        Array.Copy(received, retCmd, retCmd.Length);
                        return GPS_RESPONSE.CHKSUM_FAIL;
                    }
                }
            }
            return retval;
        }

        public static byte CheckBinaryCommand(byte[] cmd, int l)
        {
            if (l < 8)
            {
                return 0;
            }
            if (cmd[0] != 0xa0 || cmd[1] != 0xa1)
            {   //check header format
                return 0;
            }
            if (cmd[l - 2] != 0x0d && cmd[l - 1] != 0x0a)
            {   //check tail format
                return 0;
            }

            int s = (cmd[2] << 8) | cmd[3];
            if (s != l - 7)
            {   //maybe contain 0x0d 0x0a, must read one more line.
                return 0;
            }

            byte checkSum = 0;
            for (int i = 0; i < s; ++i)
            {
                checkSum ^= (byte)cmd[i + 4];
            }

            if (checkSum != cmd[l - 3])
            {   //checksum error
                return 0;
            }
            return cmd[4];
        }
    }

    public class SerialDataArgs : EventArgs
    {
        public byte[] SerialByte;
        public SerialDataArgs(byte[] data)
        {
            SerialByte = data;
        }
    }
    public delegate void SerialDataInHandler(object sender, SerialDataArgs args);
}
