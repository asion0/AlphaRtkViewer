using MiscUtil.App;
using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace StqMessageParser
{
    public enum MessageType
    {
        Unknown = 0,
        ReadError,
        StqBinary,
        NmeaMessage,
        RtcmMessage,
        UbloxMessage,
        TextMessage,
    }

    [Flags]
    public enum ParsingResult : UInt64
    {
        None,
        UpdateFixPosition = 1UL << 1,
        UpdateGpSateInfo = 1UL << 2,
        UpdateGlSateInfo = 1UL << 3,
        UpdateBdSateInfo = 1UL << 4,
        UpdateGiSateInfo = 1UL << 5,
        UpdateLocation = 1UL << 6,
        UpdateFixMode = 1UL << 7,
        UpdateTime = 1UL << 8,
        UpdateLatitude = 1UL << 9,
        UpdateLongitude = 1UL << 10,
        UpdateMslAltitude = 1UL << 11,
        UpdatePdop = 1UL << 12,
        UpdateHdop = 1UL << 13,
        UpdateVdop = 1UL << 14,
        UpdateSpeed = 1UL << 15,
        UpdateDirection = 1UL << 16,
        UpdateDate = 1UL << 17,
        UpdateRtkAge = 1UL << 18,
        UpdateRtkRatio = 1UL << 19,
        UpdateRtkEastProj = 1UL << 20,
        UpdateRtkNorthProj = 1UL << 21,
        UpdateRtkUpProj = 1UL << 22,
        UpdateRtkBaselineLen = 1UL << 23,
        UpdateRtkBaselineCour = 1UL << 24,
        UpdateRtkCycleSlip = 1UL << 25,
        UpdateElliposidalH = 1UL << 26,
        SaveDeviceOutput = 1UL << 27,
        Reboot,
    }

    public class ParsingStatus
    {
        public const int MaxSattellite = 24;
        public const int MaxChannels = 12;
        public const int NavicChannelStart = 16;
        public const int MaxNavicChannels = 24;
        public const int NullValue = int.MaxValue;
        public enum SateType
        {
            Unknown,
            Gps,
            Glonass,
            Beidou,
            Navic,
            Galileo,
            Qzss,
            Sbas,
        }

        public class SateInfo
        {
            public SateInfo()
            {
                Clear();
            }

            public SateInfo(SateInfo s)
            {
                prn = s.prn;
                snr = s.snr;
                ele = s.ele;
                azi = s.azi;
                inUse = s.inUse;
            }

            //For GSA added
            public SateInfo(int p, bool iu)
            {
                prn = p;
                snr = NullValue;
                ele = 0;
                azi = 0;
                inUse = iu;
            }

            //For GSV
            public SateInfo(int p, int e, int a, int s, bool iu)
            {
                prn = p;
                snr = s;
                ele = e;
                azi = a;
                inUse = iu;
            }

            public void Clear()
            {
                prn = NullValue;
                snr = NullValue;
                ele = 0;
                azi = 0;
                inUse = false;
            }
            public int prn { get; set; }
            public int snr { get; set; }
            public int ele { get; set; }
            public int azi { get; set; }
            public bool inUse { get; set; }
        }

        public ParsingStatus()
        {
        }

        //Interface for reconnection
        public void ClearAllSate()
        {
            gpSate.Clear();
            glSate.Clear();
            bdSate.Clear();
            giSate.Clear();
        }

        //private data for GSA
        private List<int> gpInUse = new List<int>();
        private List<int> glInUse = new List<int>();
        private List<int> bdInUse = new List<int>();
        private List<int> giInUse = new List<int>();

        //public interface for GSA
        public void ClearInUseGNList(int prn, int systemId)
        {
            if (systemId == 0)
            {   //system ID == 0, detect constellation type by prn
                ClearInUseList(GetTypeByPrn(prn));
                return;
            }

            switch (systemId)
            {
                case 1:
                    ClearInUseList(SateType.Gps);
                    break;
                case 2:
                    ClearInUseList(SateType.Glonass);
                    break;
                case 3:
                    ClearInUseList(SateType.Galileo);
                    break;
                case 4:
                    ClearInUseList(SateType.Navic);
                    break;
                default:
                    AppTools.ShowDebug(string.Format("Unknown system id:{0}", systemId));
                    break;
            }
        }

        public void ClearInUseGpsList() { ClearInUseList(SateType.Gps); }
        public void ClearInUseGlonassList() { ClearInUseList(SateType.Glonass); }
        public void ClearInUseBeidouList() { ClearInUseList(SateType.Beidou); }
        public void ClearInUseNavicList() { ClearInUseList(SateType.Navic); }

        private void ClearInUseList(SateType type)
        {
            List<int> inUseList = null;
            switch (type)
            {
                case SateType.Gps:
                    inUseList = gpInUse;
                    break;
                case SateType.Glonass:
                    inUseList = glInUse;
                    break;
                case SateType.Beidou:
                    inUseList = bdInUse;
                    break;
                case SateType.Navic:
                    inUseList = giInUse;
                    break;
                case SateType.Galileo:
                    //inUseList = gaInUse;
                    break;
                default:
                    break;
            }
            inUseList.Clear();
        }

        public void AddInUseGNPrn(int prn, int systemId)
        {
            if (systemId == 0)
            {   //system ID == 0, detect constellation type by prn
                AddInUsePrn(prn, GetTypeByPrn(prn));
                return;
            }

            switch (systemId)
            {
                case 1:
                    AddInUsePrn(prn, SateType.Gps);
                    break;
                case 2:
                    AddInUsePrn(prn, SateType.Glonass);
                    break;
                case 3:
                    AddInUsePrn(prn, SateType.Galileo);
                    break;
                case 4:
                    AddInUsePrn(prn, SateType.Navic);
                    break;
                default:
                    AppTools.ShowDebug(string.Format("Unknown system id:{0}", systemId));
                    break;
            }
        }

        public void AddInUseGpsPrn(int prn) { AddInUsePrn(prn, SateType.Gps); }
        public void AddInUseGlonassPrn(int prn) { AddInUsePrn(prn, SateType.Glonass); }
        public void AddInUseBeidouPrn(int prn){ AddInUsePrn(prn, SateType.Beidou); }
        public void AddInUseNavicPrn(int prn) { AddInUsePrn(prn, SateType.Navic); }

        private void AddInUsePrn(int prn, SateType type)
        {
            List<int> inUseList = null;
            switch (type)
            {
                case SateType.Gps:
                    inUseList = gpInUse;
                    break;
                case SateType.Glonass:
                    inUseList = glInUse;
                    break;
                case SateType.Beidou:
                    inUseList = bdInUse;
                    break;
                case SateType.Navic:
                    inUseList = giInUse;
                    break;
                case SateType.Galileo:
                    //inUseList = gaInUse;
                    break;
                default:
                    break;
            }

            if (inUseList.FindIndex(w => (w == prn)) < 0)
            {
                inUseList.Add(prn);
            }
        }

        public ParsingResult MergeInUsePrn()
        {
            ParsingResult pr = ParsingResult.None;
            List<int>[] inUseTable = { gpInUse, glInUse, bdInUse, giInUse };
            List<SateInfo>[] sateInfoTable = { gpSate, glSate, bdSate, giSate };
            UInt64[] prTable = { (UInt64)ParsingResult.UpdateGpSateInfo, (UInt64)ParsingResult.UpdateGlSateInfo,
                                        (UInt64)ParsingResult.UpdateBdSateInfo, (UInt64)ParsingResult.UpdateGiSateInfo };

            for (int i = 0; i < inUseTable.Length; ++i)
            {
                foreach (int p in inUseTable[i])
                {
                    int index = sateInfoTable[i].FindIndex(w => w.prn == p);
                    if (index < 0)
                    {
                        sateInfoTable[i].Add(new SateInfo(p, true));
                        pr |= (ParsingResult)prTable[i];
                    }
                    else
                    {
                        if (sateInfoTable[i][index].inUse == false)
                        {
                            sateInfoTable[i][index].inUse = true;
                            pr |= (ParsingResult)prTable[i];
                        }
                    }
                }
                if (inUseTable[i].Count > 0)
                {
                    sateInfoTable[i].Sort((s1, s2) => { return s1.prn.CompareTo(s2.prn); });
                }
            }
            return pr;
        }

        //private data for GSV
        private SateInfo NullSate = new SateInfo();
        private List<SateInfo> gpSate = new List<SateInfo>();
        private List<SateInfo> glSate = new List<SateInfo>();
        private List<SateInfo> bdSate = new List<SateInfo>();
        private List<SateInfo> giSate = new List<SateInfo>();

        private List<SateInfo> tmpGpSate = new List<SateInfo>();
        private List<SateInfo> tmpGlSate = new List<SateInfo>();
        private List<SateInfo> tmpBdSate = new List<SateInfo>();
        private List<SateInfo> tmpGiSate = new List<SateInfo>();
        private List<SateInfo> tmpSbSate = new List<SateInfo>();
        private List<SateInfo> tmpQzSate = new List<SateInfo>();

        public void AddGnSateInfo(int prn, int ele, int azi, int snr)
        {
            AddSateInfo(prn, ele, azi, snr, GetTypeByPrn(prn));
        }

        public void AddGpsSateInfo(int prn, int ele, int azi, int snr)
        {
            AddSateInfo(prn, ele, azi, snr, SateType.Gps);
        }

        public void AddSbasSateInfo(int prn, int ele, int azi, int snr)
        {
            AddSateInfo(prn, ele, azi, snr, SateType.Sbas);
        }

        public void AddQzssSateInfo(int prn, int ele, int azi, int snr)
        {
            AddSateInfo(prn, ele, azi, snr, SateType.Qzss);
        }

        public void AddGlonassSateInfo(int prn, int ele, int azi, int snr)
        {
            AddSateInfo(prn, ele, azi, snr, SateType.Glonass);
        }

        public void AddBeidouSateInfo(int prn, int ele, int azi, int snr)
        {
            AddSateInfo(prn, ele, azi, snr, SateType.Beidou);
        }

        public void AddNavicSateInfo(int prn, int ele, int azi, int snr)
        {
            AddSateInfo(prn, ele, azi, snr, SateType.Navic);
        }

        private void AddSateInfo(int prn, int ele, int azi, int snr, SateType type)
        {
            switch (type)
            {
                case SateType.Gps:
                    tmpGpSate.Add(new SateInfo(prn, ele, azi, snr, false));
                    break;
                case SateType.Glonass:
                    tmpGlSate.Add(new SateInfo(prn, ele, azi, snr, false));
                    break;
                case SateType.Beidou:
                    tmpBdSate.Add(new SateInfo(prn, ele, azi, snr, false));
                    break;
                case SateType.Navic:
                    tmpGiSate.Add(new SateInfo(prn, ele, azi, snr, false));
                    break;
                case SateType.Galileo:
                    break;
                case SateType.Sbas:
                    tmpSbSate.Add(new SateInfo(prn, ele, azi, snr, false));
                    break;
                case SateType.Qzss:
                    tmpQzSate.Add(new SateInfo(prn, ele, azi, snr, false));
                    break;
            }
        }

        public ParsingResult MergeSateList()
        {
            ParsingResult pr = ParsingResult.None;
            List<int>[] inUseTable = { gpInUse, glInUse, bdInUse, giInUse, gpInUse, gpInUse };
            bool[] needEraseTable = { true, true, true, true, false, false };
            List<SateInfo>[] tmpSateTable = { tmpGpSate, tmpGlSate, tmpBdSate, tmpGiSate, tmpSbSate, tmpQzSate };
            List<SateInfo>[] sateInfoTable = { gpSate, glSate, bdSate, giSate, gpSate, gpSate };
            UInt64[] prTable = { (UInt64)ParsingResult.UpdateGpSateInfo, (UInt64)ParsingResult.UpdateGlSateInfo,
                                (UInt64)ParsingResult.UpdateBdSateInfo, (UInt64)ParsingResult.UpdateGiSateInfo,
                                (UInt64)ParsingResult.UpdateGpSateInfo, (UInt64)ParsingResult.UpdateGpSateInfo };

            for (int i = 0; i < inUseTable.Length; ++i)
            {
                if (tmpSateTable[i].Count > 0)
                {
                    if (needEraseTable[i])
                    {
                        sateInfoTable[i].Clear();
                    }
                    foreach (SateInfo s in tmpSateTable[i])
                    {
                        int index = inUseTable[i].FindIndex(w => w == s.prn);
                        s.inUse = (index < 0) ? false : true;
                        sateInfoTable[i].Add(new SateInfo(s));
                    }
                    tmpSateTable[i].Clear();
                    sateInfoTable[i].Sort((s1, s2) => { return s1.prn.CompareTo(s2.prn); });
                    pr |= (ParsingResult)prTable[i];
                }
            }
            return pr;
        }

        //public interface for Draw functions
        public List<SateInfo> GetGpsSateList() { return GetSateList(SateType.Gps); }
        public List<SateInfo> GetGpsSateListClone() { return new List<SateInfo>(GetSateList(SateType.Gps)); }
        public List<SateInfo> GetGlonassSateList() { return GetSateList(SateType.Glonass); }
        public List<SateInfo> GetGlonassSateListClone() { return new List<SateInfo>(GetSateList(SateType.Glonass)); }
        public List<SateInfo> GetBeidouSateList() { return GetSateList(SateType.Beidou); }
        public List<SateInfo> GetBeidouSateListClone() { return new List<SateInfo>(GetSateList(SateType.Beidou)); }
        public List<SateInfo> GetNavicSateList() { return GetSateList(SateType.Navic); }
        public List<SateInfo> GetNavicSateListClone() { return new List<SateInfo>(GetSateList(SateType.Navic)); }

        private List<SateInfo> GetSateList(SateType t)
        {
            switch (t)
            {
                case SateType.Gps:
                    return gpSate;
                case SateType.Glonass:
                    return glSate;
                case SateType.Beidou:
                    return bdSate;
                case SateType.Navic:
                    return giSate;
                case SateType.Galileo:
                    return null;
                default:
                    return null;
            }
        }

        //private data for date / time
        private int dateY = 1980;   //GPS start date
        private int dateM = 1;      //GPS start date
        private int dateD = 6;      //GPS start date
        private int timeH = 0;
        private int timeM = 0;
        private float timeS = 0;

        //public interface for time
        public DateTime GetDateTime() { return new DateTime(dateY, dateM, dateD, timeH, timeM, (int)timeS, (int)((timeS - (int)timeS)) * 1000); }

        public string GetFormatDate()
        {
            return string.Format("{0:0000}/{1:00}/{2:00}", dateY, dateM, dateD);
        }

        public string GetFormatTime()
        {
            return string.Format("{0:00}:{1:00}:{2:00.000}", timeH, timeM, timeS);
        }

        public ParsingResult SetNmeaTimeFloat(string s)
        {
            int hh = Convert.ToInt32(s.Substring(0, 2));
            int mm = Convert.ToInt32(s.Substring(2, 2));
            float ss = Convert.ToSingle(s.Substring(4, s.Length - 4));
            return SetTime(hh, mm, ss);
        }

        public ParsingResult SetNmeaTimeInt(string s)
        {
            int hh = Convert.ToInt32(s.Substring(0, 2));
            int mm = Convert.ToInt32(s.Substring(2, 2));
            float ss = Convert.ToInt32(s.Substring(4, 2));
            return SetTime(hh, mm, ss);
        }

        public ParsingResult SetNmeaDate(string s)
        {
            int dd = Convert.ToInt32(s.Substring(0, 2));
            int mm = Convert.ToInt32(s.Substring(2, 2));
            int yy = Convert.ToInt32(s.Substring(4, 2)) + 2000;
            return SetDate(yy, mm, dd);
        }


        public ParsingResult SetStqTimeWnTow(UInt16 wn, double tow)
        {
            int year = 0, month = 0, day = 0, hour = 0, minute = 0;
            double sec = 0;

            MessageParser.GetTimeFromWnTow(wn, tow, false, ref year, ref month, ref day, ref hour, ref minute, ref sec);
            ParsingResult pr = SetDate(year, month, day);
            return pr | SetTime(hour, minute, (float)sec);
        }

        private ParsingResult SetTime(int hh, int mm, float ss)
        {
            if (hh != timeH || mm != timeM || ss != timeS)
            {
                timeH = hh;
                timeM = mm;
                timeS = ss;
                return ParsingResult.UpdateTime;
            }
            return ParsingResult.None;
        }

        private ParsingResult SetDate(int yy, int mm, int dd)
        {
            if (yy != dateY || mm != dateM || dd != dateD)
            {
                dateY = yy;
                dateM = mm;
                dateD = dd;
                return ParsingResult.UpdateDate;
            }
            return ParsingResult.None;
        }

        //private data for Lat / Lon / Alt
        private double latitude = 0;
        private char lat_ns = 'N';
        private double longitude = 0;
        private char lon_ew = 'E';
        private double mslAltitude = 0;
        private double geoidalSeparation = 0;

        //public interface for Lat / Lon / Alt
        public double GetLatitudeDegree()
        {
            int di = (int)(latitude / 100);
            double d = di + (latitude - di * 100) / 60.0;
            return d * ((lat_ns == 'S') ? -1.0 : 1);
        }

        public string GetFormatLatitudeDMS()
        {
            int d = (int)(latitude / 100.0);
            int m = (int)latitude - d * 100;
            double s = (latitude - (int)latitude) * 60.0;
            return string.Format("{0}°{1}'{2:0.00000}\" {3}", d, (int)m, s, lat_ns);
        }

        public double GetLongitudeDegree()
        {
            int di = (int)(longitude / 100);
            double d = di + (longitude - di * 100) / 60.0;
            return d* ((lon_ew == 'W') ? -1.0 : 1);
        }

        public string GetFormatLongitudeDMS()
        {
            int d = (int)(longitude / 100.0);
            int m = (int)longitude - d * 100;
            double s = (longitude - (int)longitude) * 60.0;
            return string.Format("{0}°{1}'{2:0.00000}\" {3}", d, (int)m, s, lon_ew);
        }

        public double GetMslAltitude()
        {
            return mslAltitude;
        }

        public double GetEllipsoidalHeight()
        {
            return mslAltitude + geoidalSeparation;
        }

        public ParsingResult SetNmeaLatitude(string s, string ns)
        {
            double lat = Convert.ToDouble(s);
            char c = (ns == "N" || ns == "n") ? 'N' : 'S';
            return SetLatitude(lat, c);
        }

        public ParsingResult SetNmeaMslAltitude(string s)
        {
            double d = Convert.ToDouble(s);
            if (d != mslAltitude)
            {
                mslAltitude = d;
                return ParsingResult.UpdateMslAltitude;
            }
            return ParsingResult.None;
        }

        public ParsingResult SetNmeaGeoidalSeparation(string s)
        {
            double d = Convert.ToDouble(s);
            if (d != geoidalSeparation)
            {
                geoidalSeparation = d;
                return ParsingResult.UpdateElliposidalH;
            }
            return ParsingResult.None;
        }

        public ParsingResult SetRtcmLatLonAlt(double ecefx, double ecefy, double ecefz)
        {
            return SetStqLatLonAlt((double)ecefx / 10000, (double)ecefy / 10000, (double)ecefz / 10000);
        }

        public ParsingResult SetStqLatLonAlt(double ecef_x, double ecef_y, double ecef_z)
        {
            const double WGS84_RA = 6378137.0;   // semi-major earth axis(ellipsoid equatorial radius)
            const double WGS84_INV_F = 298.257223563;   // inverse flattening of WGS-84
            const double WGS84_F = 1.0 / WGS84_INV_F;   // inverse flattening of WGS-84
            const double WGS84_RB = WGS84_RA * (1.0 - WGS84_F);    // semi-major earth axis(ellipsoid polar radius)
            const double WGS84_E2 = 2.0*WGS84_F - WGS84_F*WGS84_F;   // eccentricity squared: (RA*RA-RB*RB)/RA*RA
            const double WGS84_E2P = WGS84_E2 / (1.0-WGS84_E2);      // eccentricity squared: (RA*RA-RB*RB)/RB*RB
            const double R2D = 57.2957795131;

            double p =Math.Sqrt(ecef_x * ecef_x + ecef_y * ecef_y);
            double lat, lon;
            float alt;
            if (p <= 0.01f)
            {
                lat = (ecef_z >= 0) ? (Math.PI / 2.0) : (-Math.PI / 2.0);
                lon = 0.0;
                alt = (float)(Math.Abs(ecef_z) - WGS84_RB);
            }
            else
            {
                double theta = Math.Atan2((ecef_z * WGS84_RA), (p * WGS84_RB));
                double s_theta = Math.Sin(theta);
                double c_theta = Math.Cos(theta);

                double temp = (ecef_z + WGS84_E2P * WGS84_RB * s_theta * s_theta * s_theta);
                double phi = Math.Atan2(temp, (p - WGS84_E2 * WGS84_RA * c_theta * c_theta * c_theta));

                double s_phi = Math.Sin(phi);
                double c_phi = Math.Cos(phi);

                lat = phi;
                lon = Math.Atan2(ecef_y, ecef_x);
                alt = (float)((p / c_phi) - (WGS84_RA / Math.Sqrt(1.0 - WGS84_E2 * s_phi * s_phi)));
            }
            lat *= R2D;
            lon *= R2D;

            double lat_d = (int)Math.Abs(lat);
            double lon_d = (int)Math.Abs(lon);
            double lat_m = (Math.Abs(lat) - lat_d) * 60.0;
            double lon_m = (Math.Abs(lon) - lon_d) * 60.0;

            ParsingResult pr = SetLongitude(lon_d * 100.0 + lon_m, (lon >= 0) ? 'E' : 'W');
            pr |= SetLatitude(lat_d * 100.0 + lat_m, (lat >= 0) ? 'N' : 'S');
            if(mslAltitude != alt || geoidalSeparation != 0)
            {
                mslAltitude = alt;
                geoidalSeparation = 0;
                pr |= ParsingResult.UpdateElliposidalH;
            }
            return pr;
        }

        public ParsingResult SetNmeaLongitude(string s, string ew)
        {
            double lon = Convert.ToDouble(s);
            char c = (ew == "E" || ew == "e") ? 'E' : 'W';
            return SetLongitude(lon, c);
        }

        private ParsingResult SetLatitude(double deg, char c)
        {
            if(latitude != deg || lat_ns != c)
            {
                latitude = deg;
                lat_ns = c;
                return ParsingResult.UpdateLatitude;
            }
            return ParsingResult.None;
        }

        private ParsingResult SetLongitude(double deg, char c)
        {
            if (longitude != deg || lat_ns != c)
            {
                longitude = deg;
                lon_ew = c;
                return ParsingResult.UpdateLongitude;
            }
            return ParsingResult.None;
        }

        //private data for fix modes
        private char ggaGpsQualityInd = ' ';
        private char rmcNavStatus = ' ';
        private char gsaFixedMode = ' ';
        private bool rtcmFixedMode = false;

        //public interface for fix modes
        public enum FixedMode
        {
            None,
            PositionFix2d,
            PositionFix3d,
            DgpsMode,
            EstimatedMode,
            FloatRTK,
            FixRTK,
            RtcmMode,
            Unknown,
        }

        public bool IsBetterThanPositionFix3D() { return GetFixedMode() >= FixedMode.PositionFix3d; }

        public FixedMode GetFixedMode()
        {

            if (rtcmFixedMode)
            {
                return FixedMode.RtcmMode;
            }

            if (rmcNavStatus != ' ')
            {
                // GNS & RMC Mode indicator
                // A(1) - Autonomous.Satellites system used in non - differential mode in position fix.
                // D(2) - Differential.
                // E(6) - Estimated(dead reckoning) Mode.
                // F(5) - Float RTK.
                // M(7) - Manual Input Mode.
                // N(0) - No fix.
                // P(?) - Precise.Satellites system used in precision mode. Precision mode is defined as: no delibrate.
                // R(4) - Real Time Kinematic. Satellites system used in RTK mode with fixed integers.
                // S(8) - Simulator mode.
                switch (rmcNavStatus)
                {
                    case 'N':
                        return FixedMode.None;
                    case 'A':
                        return (gsaFixedMode == '3') ? FixedMode.PositionFix3d : FixedMode.PositionFix2d;
                    case 'D':
                        return FixedMode.DgpsMode;
                    case 'E':
                        return FixedMode.EstimatedMode;
                    case 'F':
                        return FixedMode.FloatRTK;
                    case 'R':
                        return FixedMode.FixRTK;
                }
            }

            if(ggaGpsQualityInd != ' ')
            {
                switch (ggaGpsQualityInd)
                {
                    case '0':
                        return FixedMode.None;
                    case '1':
                        return (gsaFixedMode == '3') ? FixedMode.PositionFix3d : FixedMode.PositionFix2d;
                    case '2':
                        return FixedMode.DgpsMode;
                    case '6':
                        return FixedMode.EstimatedMode;
                    case '5':
                        return FixedMode.FloatRTK;
                    case '4':
                        return FixedMode.FixRTK;
                }
            }

            if (gsaFixedMode != ' ')
            {
                switch (gsaFixedMode)
                {
                    case '1':
                        return FixedMode.None;
                    case '2':
                        return FixedMode.PositionFix3d;
                    case '3':
                        return FixedMode.PositionFix2d;
                }
            }

            return FixedMode.None;
        }

        public ParsingResult SetRtcmFixedMode(bool b)
        {
            if(rtcmFixedMode == b)
            {
                return ParsingResult.None;
            }
            else
            {
                rtcmFixedMode = b;
                return ParsingResult.UpdateFixMode;
            }
        }

        public ParsingResult SetGgaGpsQualityInd(char c)
        {
            rtcmFixedMode = false;
            if (ggaGpsQualityInd != c)
            {
                ggaGpsQualityInd = c;
                return ParsingResult.UpdateFixMode;
            }
            return ParsingResult.None;
        }

        public ParsingResult SetRmcNavStatus(char c)
        {
            rtcmFixedMode = false;
            if (rmcNavStatus != c)
            {
                rmcNavStatus = c;
                return ParsingResult.UpdateFixMode;
            }
            return ParsingResult.None;
        }

        public ParsingResult SetGsaFixedMode(char c)
        {
            rtcmFixedMode = false;
            if (gsaFixedMode != c)
            {
                gsaFixedMode = c;
                return ParsingResult.UpdateFixMode;
            }
            return ParsingResult.None;
        }

        public ParsingResult SetStqDfNavStatus(byte b)
        {
            rtcmFixedMode = false;
            ParsingResult pr = ParsingResult.None;
            switch (b)
            {
                case 0:
                    pr |= SetGgaGpsQualityInd('0');
                    pr |= SetGsaFixedMode('1');
                    pr |= SetRmcNavStatus('V');
                    break;
                case 1:
                    pr |= SetGgaGpsQualityInd('1');
                    pr |= SetGsaFixedMode('2');
                    pr |= SetRmcNavStatus('A');
                    break;
                case 2:
                    pr |= SetGgaGpsQualityInd('1');
                    pr |= SetGsaFixedMode('2');
                    pr |= SetRmcNavStatus('A');
                    break;
                case 3:
                    pr |= SetGgaGpsQualityInd('1');
                    pr |= SetGsaFixedMode('3');
                    pr |= SetRmcNavStatus('A');
                    break;
                case 4:
                    pr |= SetGgaGpsQualityInd('2');
                    pr |= SetGsaFixedMode('3');
                    pr |= SetRmcNavStatus('D');
                    break;
            }
            return pr;
        }

        //private data for dop
        private float hdop = 0;
        private float vdop = 0;
        private float pdop = 0;

        //public interface for fix modes
        public float GetPdop() { return pdop; }
        public float GetHdop() { return hdop; }
        public float GetVdop() { return vdop; }
        public ParsingResult SetNmeaPdop(string s) { return SetPdop(Convert.ToSingle(s)); }
        public ParsingResult SetPdop(float f)
        {
            if (f != pdop)
            {
                pdop = f;
                return ParsingResult.UpdatePdop;
            }
            return ParsingResult.None;
        }
        public ParsingResult SetNmeaHdop(string s) { return SetHdop(Convert.ToSingle(s)); }
        public ParsingResult SetHdop(float f)
        {
            if (f != hdop)
            {
                hdop = f;
                return ParsingResult.UpdateHdop;
            }
            return ParsingResult.None;
        }
        public ParsingResult SetNmeaVdop(string s) { return SetVdop(Convert.ToSingle(s)); }
        public ParsingResult SetVdop(float f)
        {
            if (f != vdop)
            {
                vdop = f;
                return ParsingResult.UpdateVdop;
            }
            return ParsingResult.None;
        }

        //private data for speed
        private float speedOverGround = -1;
        
        //public interface for Speed
        public float GetSpeedKmHr() { return speedOverGround * 1.852F; }
        public ParsingResult SetSpeedOverGround(string s)
        {
            float f = Convert.ToSingle(s);
            if (f != speedOverGround)
            {
                speedOverGround = f;
                return ParsingResult.UpdateSpeed;
            }
            return ParsingResult.None;
        }
        public ParsingResult SetSqrtDfSpeed(double vx, double vy, double vz)
        {
            const float KNOTS2KMHR = 1.852F;    //1.0 knots = 1.852 km / hr
            const float MS2KMHR = 3.6F;         //1.0  m/sec = 3.6 km / hr
            float f = (float)(Math.Sqrt(vx * vx + vy * vy + vz * vz) * MS2KMHR / KNOTS2KMHR);
            if (f < 0.5) f = 0; //DF messages have a tiny speed output in static
            if (f != speedOverGround)
            {
                speedOverGround = f;
                return ParsingResult.UpdateSpeed;
            }
            return ParsingResult.None;
        }

        //private data for direction
        private float courseOverGround = -1;

        //public interface for Speed
        public float GetDirection() { return courseOverGround; }
        public ParsingResult SetCourseOverGround(string s)
        {
            float f = Convert.ToSingle(s);
            if (f != courseOverGround)
            {
                courseOverGround = f;
                return ParsingResult.UpdateDirection;
            }
            return ParsingResult.None;
        }

        //private data for RTK info
        private float rtkAge = -1;
        private float rtkRatio = -1;
        private float rtkEastProjection = -1;
        private float rtkNorthProjection = -1;
        private float rtkUpProjection = -1;
        private float rtkBaselineLength = -1;
        private float rtkBaselineCourse = -1;
        private float rtkRoverCycleSlip = -1;
        private float rtkBaseCycleSlip = -1;

        public float GetRtkAge() { return rtkAge; }
        public ParsingResult SetNmeaRtkAge(string s)
        {
            float f = Convert.ToSingle(s);
            if (f != rtkAge)
            {
                rtkAge = f;
                return ParsingResult.UpdateRtkAge;
            }
            return ParsingResult.None;
        }

        public float GetRtkRatio() { return rtkRatio; }
        public ParsingResult SetNmeaRtkRatio(string s)
        {
            float f = Convert.ToSingle(s);
            if (f != rtkRatio)
            {
                rtkRatio = f;
                return ParsingResult.UpdateRtkRatio;
            }
            return ParsingResult.None;
        }

        public float GetRtkEastProj() { return rtkEastProjection; }
        public ParsingResult SetNmeaRtkEastProj(string s)
        {
            float f = Convert.ToSingle(s);
            if (f != rtkEastProjection)
            {
                rtkEastProjection = f;
                return ParsingResult.UpdateRtkEastProj;
            }
            return ParsingResult.None;
        }

        public float GetRtkNorthProj() { return rtkNorthProjection; }
        public ParsingResult SetNmeaRtkNorthProj(string s)
        {
            float f = Convert.ToSingle(s);
            if (f != rtkNorthProjection)
            {
                rtkNorthProjection = f;
                return ParsingResult.UpdateRtkNorthProj;
            }
            return ParsingResult.None;
        }

        public float GetRtkUpProj() { return rtkUpProjection; }
        public ParsingResult SetNmeaRtkUpProj(string s)
        {
            float f = Convert.ToSingle(s);
            if (f != rtkUpProjection)
            {
                rtkUpProjection = f;
                return ParsingResult.UpdateRtkUpProj;
            }
            return ParsingResult.None;
        }

        public float GetRtkBaselineLen() { return rtkBaselineLength; }
        public ParsingResult SetNmeaRtkBaselineLen(string s)
        {
            float f = Convert.ToSingle(s);
            if (f != rtkBaselineLength)
            {
                rtkBaselineLength = f;
                return ParsingResult.UpdateRtkBaselineLen;
            }
            return ParsingResult.None;
        }

        public float GetRtkBaselineCour() { return rtkBaselineCourse; }
        public ParsingResult SetNmeaRtkBaselineCour(string s)
        {
            float f = Convert.ToSingle(s);
            if (f != rtkBaselineCourse)
            {
                rtkBaselineCourse = f;
                return ParsingResult.UpdateRtkBaselineCour;
            }
            return ParsingResult.None;
        }

        public string GetFormatRtkCycleSlip() { return string.Format("{0:F0}/{1:F0}", rtkRoverCycleSlip, rtkBaseCycleSlip); }
        public ParsingResult SetNmeaRtkRoverCycleSlip(string s)
        {
            float f = Convert.ToSingle(s);
            if (f != rtkRoverCycleSlip)
            {
                rtkRoverCycleSlip = f;
                return ParsingResult.UpdateRtkCycleSlip;
            }
            return ParsingResult.None;
        }

        public ParsingResult SetNmeaRtkBaseCycleSlip(string s)
        {
            float f = Convert.ToSingle(s);
            if (f != rtkBaseCycleSlip)
            {
                rtkBaseCycleSlip = f;
                return ParsingResult.UpdateRtkCycleSlip;
            }
            return ParsingResult.None;
        }

        public String latString = "";
        public char ns = ' ';
        public String lonString = "";
        public char ew = ' ';
        public String dateString = "";
        public String timeString = "";
        public String altitudeString = "";

        private SateType GetTypeByPrn(int prn)
        {
            if ((prn >= 1 && prn < 65) || (prn > 192 && prn < 200))
            {
                return SateType.Gps;
            }
            if (prn >= 65 && prn <= 96)
            {   //GLONASS satellite using PSTI for testing.
                return SateType.Glonass;
            }
            if (prn >= 200 && prn <= 232)
            {   //Unicore NMEA
                return SateType.Beidou;
            }
            return SateType.Beidou;
        }

        public static int GetBeidouPrnBase(int prn)
        {
            if (prn >= 1 && prn <= 50 || prn == 193)
            {
                return 0;
            }
            if (prn >= 65 && prn <= 96)
            {   //GLONASS satellite using PSTI for testing.
                return 0;
            }
            if (prn >= 100 && prn <= 137)
            {   //OlinkStar NMEA
                return 100;
            }
            if (prn >= 160 && prn <= 192)
            {   //Unicore NMEA
                return 160;
            }
            if (prn >= 200 && prn <= 232)
            {   //Unicore NMEA
                return 200;
            }
            return 0;
        }

        public static int GetRealBeidouPrn(int prn)
        {
            return prn - GetBeidouPrnBase(prn);
        }
        /*
        private sateInfo GetSate(int i, SateType t)
        {
            switch (t)
            {
                case SateType.Gps:
                    return gpSate[i];
                case SateType.Glonass:
                    return glSate[i];
                case SateType.Beidou:
                    return bdSate[i];
                case SateType.Navic:
                    return giSate[i];
            }
            return NullSate;
        }

        private int GetSpecSate(SateType t, int id1, int id2, int id3, ref int sn1, ref int sn2, ref int sn3)
        {
            int getCount = 0;
            int snr;

            snr = GetSnr(id1, t).snr;
            if (snr != NullValue)
            {
                sn1 = snr;
                getCount++;
            }
            snr = GetSnr(id2, t).snr;
            if (snr != NullValue)
            {
                sn2 = snr;
                getCount++;
            }
            snr = GetSnr(id3, t).snr;
            if (snr != NullValue)
            {
                sn3 = snr;
                getCount++;
            }
            return getCount;
        }

        private int GetFrontSate(SateType t, ref int id1, ref int id2, ref int id3, ref int sn1, ref int sn2, ref int sn3)
        {
            sateInfo[] sortedSateArray = GetSateArray(t).Clone() as sateInfo[];

            for (int i = 0; i < MaxSattellite; i++)
            {
                for (int j = i + 1; j < MaxSattellite; j++)
                {
                    if (sortedSateArray[i].snr < sortedSateArray[j].snr)
                    {
                        sateInfo tmp = sortedSateArray[i];
                        sortedSateArray[i] = sortedSateArray[j];
                        sortedSateArray[j] = tmp;
                    }
                }
            }
            if (sortedSateArray[0].snr == NullValue)
            {
                return 0;
            }
            id1 = sortedSateArray[0].prn;
            sn1 = sortedSateArray[0].snr;

            if (sortedSateArray[1].snr == NullValue)
            {
                return 1;
            }
            id2 = sortedSateArray[1].prn;
            sn2 = sortedSateArray[1].snr;

            if (sortedSateArray[2].snr == NullValue)
            {
                return 2;
            }
            id3 = sortedSateArray[2].prn;
            sn3 = sortedSateArray[2].snr;
            return 3;
        }

        private sateInfo[] GetSortedSateArray(SateType t)
        {
            sateInfo[] s = GetSateArray(t).Clone() as sateInfo[];
            Array.Sort(s, delegate (sateInfo s1, sateInfo s2)
            {
                int r = s1.prn.CompareTo(s2.prn);
                return r;
            });
            return s;
        }
        */
    }

    public class MessageParser
    {
        public class ParsingResultArgs : EventArgs
        {
            public string showString;
            public ParsingResult parsingResult;
            public byte[] deviceOutput;
            public ParsingResultArgs(string s, byte[] b, ParsingResult p)
            {
                showString = s;
                deviceOutput = b;
                parsingResult = p;
            }

            public ParsingResultArgs(byte[] b, ParsingResult p)
            {
                deviceOutput = b;
                parsingResult = p;
            }
        }

        public delegate void ParsingResultHandler(object sender, ParsingResultArgs args);
        public event ParsingResultHandler parsingResultHandler;
        private bool saveDeviceOutput = false;
        public void SaveDeviceOutput(bool b) { saveDeviceOutput = b; }
        public void Parsing(byte[] data)
        {
            if (saveDeviceOutput)
            {
                parsingResultHandler?.Invoke(null, new ParsingResultArgs(data, ParsingResult.SaveDeviceOutput));
            }
            foreach (byte c in data)
            {
                ps = parsingTable[(int)ps](c);
                if (ps == ParsingState.ParsingDone)
                {
                    parsingBuffer[bi++] = c;
                    parsingBuffer[bi] = 0;
                    PrasingMessage(parsingBuffer, bi);
                    parsingResultHandler?.Invoke(null, new ParsingResultArgs(showString, parsingBuffer, parsingResult));
                    bi = 0;
                }
                else if (ps != ParsingState.NoComing)
                {
                    parsingBuffer[bi++] = c;
                }
                else
                {
                    bi = 0;
                }
            }
        }

        public static void GetTimeFromWnTow(UInt16 wn, double tow, bool noLeapSeconds,
            ref int year, ref int month, ref int day, ref int hour, ref int minute, ref double sec)
        {
            const int DefaultLeapSeconds = 18;
            // GPS Time start at 1980 Jan. 5/6 mid-night
            const Int16 DAYS_PER_YEAR = 365;
            const Int16 DAYS_PER_4_YEARS = (DAYS_PER_YEAR * 4 + 1);  // plus one day for leap year
            Int16[] day_of_year_month_table =
                { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
            Int16[] day_of_leap_year_month_table =
                { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };
            Int16[] month_tbl_p = day_of_year_month_table;

            Int32 tow_int = (Int32)Math.Floor(tow);
            double tow_frac = tow - (double)tow_int;
            Int32 total_utc_sec = 604800 * wn + tow_int - ((noLeapSeconds) ? 0 : DefaultLeapSeconds);
            Int32 total_utc_day = total_utc_sec / 86400;
            Int32 sec_of_day = total_utc_sec - 86400 * total_utc_day;
            Int32 passed_leap_days = (total_utc_day + DAYS_PER_4_YEARS
                    - day_of_leap_year_month_table[2] + 5) / DAYS_PER_4_YEARS;
            Int32 passed_utc_years = (total_utc_day + 5 - passed_leap_days) / 365;
            Int32 leap_days_of_prev_years = (passed_utc_years + 3) / 4;
            Int32 day_of_utc_year = total_utc_day + 5 - passed_utc_years * DAYS_PER_YEAR
                - leap_days_of_prev_years;

            year = 1980 + passed_utc_years;
            int day_of_year = day_of_utc_year + 1;
            if ((year & 0x3) == 0)  // utc->year % 4
                month_tbl_p = day_of_leap_year_month_table;  // this year is leap year

            int i = 1;
            for (i = 1; i < 13; ++i)
                if (day_of_utc_year < month_tbl_p[i])
                    break;

            month = i;
            day = day_of_utc_year - month_tbl_p[i - 1] + 1;

            hour = sec_of_day / 3600;
            if (hour > 23)
            {
                hour = 23;
            }

            minute = (sec_of_day - hour * 3600) / 60;
            if (minute > 59)
            {
                minute = 59;
            }

            sec = (sec_of_day - hour * 3600 - minute * 60) + tow_frac;
        }

        private enum ParsingState
        {
            NoComing,
            StqHeaderA0,
            StqHeaderA1,
            StqHeaderS1,
            StqHeaderS2,
            StqEol0D,
            NmeaHeaderDoller,
            NmeaBody,
            NmeaEol0D,
            RtcmHeaderD3,
            RtcmHeaderS1,
            RtcmHeaderS2,
            RtcmBodyType1,
            RtcmBody,
            UbloxHeaderB5,
            UbloxHeader62,
            UbloxClass,
            UbloxId,
            UbloxSize1,
            UbloxSize2,
            UbloxBody,
            UbloxChecksumA,
            UbloxChecksumB,

            UnknownMessage0D,
            UnknownMessage0A,
            HostLogHeaderS,
            HostLogHeaderT,

            ParsingDone,
        }

        private static int messageLength = 0;
        private static MessageType messageType = MessageType.Unknown;
        private static ParsingState ps = ParsingState.NoComing;
        private static byte[] parsingBuffer = new byte[4096];
        private static int bi = 0;

        private static bool IsPrintable(byte c)
        {
            return ((c >= 0x20 && c <= 0x7E) || (c == 0x09) || (c == 0x0D) || (c == 0x0A));
        }

        private delegate ParsingState ParsingFunction(byte c);
        private static ParsingState NoComingM(byte c)
        {
            switch (c)
            {
                case 0xA0:
                    return ParsingState.StqHeaderA0;
                case 0xD3:
                    return ParsingState.RtcmHeaderD3;
                case (byte)'$':
                    return ParsingState.NmeaHeaderDoller;
                case 0xB5:
                    return ParsingState.UbloxHeaderB5;
                case 0x0D:
                    return ParsingState.UnknownMessage0D;
                default:
                    return ParsingState.NoComing;
            }
        }

        private static ParsingState StqHeaderA0M(byte c)
        {
            if (c == 0xA1)
            {
                return ParsingState.StqHeaderA1;
            }
            return NoComingM(c);
        }

        private static int stqMessageSize = 0;
        private static ParsingState StqHeaderA1M(byte c)
        {
            if (c > 0x08)   //SktTraq binary length must < 2048 bytes
            { //Oversized
                return NoComingM(c);
            }
            stqMessageSize = c << 8;
            return ParsingState.StqHeaderS1;
        }

        private static int stqMessageReceivedSize = 0;
        private static ParsingState StqHeaderS1M(byte c)
        {
            if(c == 0 && stqMessageSize == 0)
            {   //Zero size is not allowed
                return NoComingM(c);
            }
            stqMessageSize |= c;
            messageLength = 4;
            stqMessageReceivedSize = 0;
            return ParsingState.StqHeaderS2;
        }

        private static ParsingState StqHeaderS2M(byte c)
        {
            ++messageLength;
            if (stqMessageReceivedSize <= stqMessageSize)
            {
                ++stqMessageReceivedSize;
                return ParsingState.StqHeaderS2;
            }
            else if (c == 0x0D && stqMessageReceivedSize == stqMessageSize + 1)
            { 
                return ParsingState.StqEol0D;
            }
            return NoComingM(c);
        }

        private static ParsingState StqEol0DM(byte c)
        {
            if (c == 0x0A)
            {
                messageType = MessageType.StqBinary;
                return ParsingState.ParsingDone;
            }
            return NoComingM(c);
        }

        private static ParsingState NmeaHeaderDollerM(byte c)
        {
            if (c >= (byte)'A' & c <= (byte)'Z')
            {
                messageLength = 2;
                return ParsingState.NmeaBody;
            }
            return NoComingM(c);
        }

        private static ParsingState NmeaBodyM(byte c)
        {
            if (0x0D == c)
            {
                ++messageLength;
                return ParsingState.NmeaEol0D;
            }
            else if (IsPrintable(c) && messageLength < 128)
            {
                ++messageLength;
                return ParsingState.NmeaBody;
            }
            return NoComingM(c);
        }

        private static ParsingState NmeaEol0DM(byte c)
        {
            if (0x0A == c)
            {
                messageType = MessageType.NmeaMessage;
                return ParsingState.ParsingDone;
            }
            return NoComingM(c);
        }

        private static int rtcmMessageSize = 0;
        private static ParsingState RtcmHeaderD3M(byte c)
        {
            if ((0xFC & c) == 0)
            {
                rtcmMessageSize = c << 8;
                return ParsingState.RtcmHeaderS1;
            }
            return NoComingM(c);
        }

        private static int rtcmReceivedMessageSize = 0;
        private static ParsingState RtcmHeaderS1M(byte c)
        {
            rtcmMessageSize |= c;
            if (rtcmMessageSize > 7) //minimum rtcm message size
            {
                rtcmReceivedMessageSize = 0;
                messageLength = 3;
                return ParsingState.RtcmHeaderS2;
            }
            return NoComingM(c);
        }

        private static int rtcmType = 0;
        private static ParsingState RtcmHeaderS2M(byte c)
        {
            rtcmType = c << 4;
            ++messageLength;
            ++rtcmReceivedMessageSize;
            return ParsingState.RtcmBodyType1;
        }

        private static ParsingState RtcmBodyType1M(byte c)
        {
            rtcmType |= c >> 4;
            ++messageLength;
            ++rtcmReceivedMessageSize;
            //if (IsSupportedRtcmType(rtcmType))
            //{
            return ParsingState.RtcmBody;
            //}
            //return NoComingM(c);
        }

        private static ParsingState RtcmBodyM(byte c)
        {
            ++messageLength;
            ++rtcmReceivedMessageSize;
            if (rtcmMessageSize + 3 == rtcmReceivedMessageSize)  //Include 3 bytes crc
            {
                messageType = MessageType.RtcmMessage;
                return ParsingState.ParsingDone;
            }
            return ParsingState.RtcmBody;
        }

        private static ParsingState UbloxHeaderB5M(byte c)
        {
            switch (c)
            {
                case 0xA0:
                    return ParsingState.StqHeaderA0;
                case 0xD3:
                    return ParsingState.RtcmHeaderD3;
                case (byte)'$':
                    return ParsingState.NmeaHeaderDoller;
                case 0x62:
                    return ParsingState.UbloxHeader62;
                default:
                    return ParsingState.NoComing;
            }
        }

        private static ParsingState UbloxHeader62M(byte c)
        {
            switch (c)
            {
                case 0x01:
                case 0x02:
                case 0x04:
                case 0x05:
                case 0x06:
                case 0x09:
                case 0x0A:
                case 0x0B:
                case 0x0D:
                case 0x10:
                case 0x13:
                case 0x21:
                case 0x27:
                    return ParsingState.UbloxClass;
                case 0xA0:
                    return ParsingState.StqHeaderA0;
                case 0xD3:
                    return ParsingState.RtcmHeaderD3;
                case (byte)'$':
                    return ParsingState.NmeaHeaderDoller;
                case 0x62:
                    return ParsingState.UbloxHeader62;
                default:
                    return ParsingState.NoComing;
            }
        }

        private static ParsingState UbloxClassM(byte c)
        {
            return ParsingState.UbloxId;
        }

        private static int ubloxMessageSize = 0;
        private static ParsingState UbloxIdM(byte c)
        {
            ubloxMessageSize = c;
            return ParsingState.UbloxSize1;
        }

        private static ParsingState UbloxSize1M(byte c)
        {
            ubloxMessageSize |= c << 8;
            messageLength = 1;
            return ParsingState.UbloxSize2;
        }

        private static ParsingState UbloxSize2M(byte c)
        {
            return ParsingState.UbloxBody;
        }

        private static ParsingState UbloxBodyM(byte c)
        {
            ++messageLength;
            if (ubloxMessageSize == messageLength)
            {
                return ParsingState.UbloxChecksumA;
            }
            return ParsingState.UbloxBody;
        }

        private static ParsingState UbloxChecksumAM(byte c)
        {
            return ParsingState.UbloxChecksumB;
        }

        private static ParsingState UbloxChecksumBM(byte c)
        {
            messageType = MessageType.UbloxMessage;
            return ParsingState.ParsingDone;
        }

        private static ParsingState UnknownMessage0DM(byte c)
        {
            switch (c)
            {
                case 0xA0:
                    return ParsingState.StqHeaderA0;
                case 0xD3:
                    return ParsingState.RtcmHeaderD3;
                case (byte)'$':
                    return ParsingState.NmeaHeaderDoller;
                case 0xB5:
                    return ParsingState.UbloxHeaderB5;
                case 0x0D:
                    return ParsingState.UnknownMessage0D;
                default:
                    return ParsingState.NoComing;
            }
        }

        private static ParsingState UnknownMessage0AM(byte c)
        {
            switch (c)
            {
                case 0xA0:
                    return ParsingState.StqHeaderA0;
                case 0xD3:
                    return ParsingState.RtcmHeaderD3;
                case (byte)'$':
                    return ParsingState.NmeaHeaderDoller;
                case 0xB5:
                    return ParsingState.UbloxHeaderB5;
                case 0x0D:
                    return ParsingState.UnknownMessage0D;
                default:
                    return ParsingState.NoComing;
            }
        }

        private static ParsingState HostLogHeaderSM(byte c)
        {
            switch (c)
            {
                case 0xA0:
                    return ParsingState.StqHeaderA0;
                case 0xD3:
                    return ParsingState.RtcmHeaderD3;
                case (byte)'$':
                    return ParsingState.NmeaHeaderDoller;
                case 0xB5:
                    return ParsingState.UbloxHeaderB5;
                case 0x0D:
                    return ParsingState.UnknownMessage0D;
                default:
                    return ParsingState.NoComing;
            }
        }

        private static ParsingState HostLogHeaderTM(byte c)
        {
            switch (c)
            {
                case 0xA0:
                    return ParsingState.StqHeaderA0;
                case 0xD3:
                    return ParsingState.RtcmHeaderD3;
                case (byte)'$':
                    return ParsingState.NmeaHeaderDoller;
                case 0xB5:
                    return ParsingState.UbloxHeaderB5;
                case 0x0D:
                    return ParsingState.UnknownMessage0D;
                default:
                    return ParsingState.NoComing;
            }
        }

        private static ParsingState ParsingDoneM(byte c)
        {
            switch (c)
            {
                case 0xA0:
                    return ParsingState.StqHeaderA0;
                case 0xD3:
                    return ParsingState.RtcmHeaderD3;
                case (byte)'$':
                    return ParsingState.NmeaHeaderDoller;
                case 0xB5:
                    return ParsingState.UbloxHeaderB5;
                case 0x0D:
                    return ParsingState.UnknownMessage0D;
                default:
                    return ParsingState.NoComing;
            }
        }

        private static ParsingFunction[] parsingTable = {
            NoComingM, StqHeaderA0M, StqHeaderA1M, StqHeaderS1M, StqHeaderS2M, StqEol0DM,
            NmeaHeaderDollerM, NmeaBodyM, NmeaEol0DM,
            RtcmHeaderD3M, RtcmHeaderS1M, RtcmHeaderS2M, RtcmBodyType1M, RtcmBodyM,
            UbloxHeaderB5M, UbloxHeader62M, UbloxClassM, UbloxIdM, UbloxSize1M, UbloxSize2M, UbloxBodyM, UbloxChecksumAM, UbloxChecksumBM,
            UnknownMessage0DM, UnknownMessage0AM,
            HostLogHeaderSM, HostLogHeaderTM,
            ParsingDoneM,
        };

        private string showString;
        private ParsingResult parsingResult = ParsingResult.None;
        private static ParsingStatus parsingStatus = new ParsingStatus();
        public static ParsingStatus GetParsingStatus() { return parsingStatus; }
        public static void ClearParsingStatus()
        {
            parsingStatus = null;
            parsingStatus = new ParsingStatus();
        }

        private NmeaParser nmeaParser = new NmeaParser();
        private RtcmParser rtcmParser = new RtcmParser();
        private StqBinaryParser stqBinParser = new StqBinaryParser();
        private UbloxParser ubloxParser = new UbloxParser();
        private void PrasingMessage(byte[] buffer, int length)
        {
            NmeaParser.NmeaType nmeaType = NmeaParser.NmeaType.NMEA_Unknown;
            StqBinaryParser.StqBinaryType stqBinaryType = StqBinaryParser.StqBinaryType.STQ_Unknown;
            RtcmParser.RtcmType rtcmType = RtcmParser.RtcmType.RTCM_Unknown;
            UbloxParser.UbloxType ubloxType = UbloxParser.UbloxType.UBX_Unknown;
            showString = "";
            switch (messageType)
            {
                case MessageType.NmeaMessage:
                    parsingResult = nmeaParser.Process(buffer, length, ref nmeaType, ref showString);
                    return;
                case MessageType.StqBinary:
                    parsingResult = stqBinParser.Process(buffer, length, ref stqBinaryType, ref showString);
                    return;// String.Format("{0}({1})", stqBinaryType.ToString(), length);
                case MessageType.RtcmMessage:
                    parsingResult = rtcmParser.Process(buffer, length, ref rtcmType, ref showString);
                    return;// String.Format("{0}({1})", rtcmType.ToString(), length);
                case MessageType.UbloxMessage:
                    parsingResult = ubloxParser.Process(buffer, length, ref ubloxType, ref showString);
                    break;
                default:
                    break;
            }

            return;
        }
    }

    public class NmeaParser
    {
        public enum NmeaType
        {
            NMEA_Unknown = 0,

            NMEA_GGA,

            NMEA_GPGSA,
            NMEA_GLGSA,
            NMEA_BDGSA,
            NMEA_GAGSA,
            NMEA_GNGSA,
            NMEA_GIGSA,

            NMEA_GPGSV,
            NMEA_GPGSV2,
            NMEA_GLGSV,
            NMEA_BDGSV,
            //NMEA_BDGSV2,
            NMEA_GAGSV,
            NMEA_GNGSV,
            NMEA_GIGSV,

            NMEA_RMC,
            NMEA_GLL,
            NMEA_ZDA,
            NMEA_GNS,
            NMEA_VTG,
            NMEA_GST,
            NMEA_STI030,
            NMEA_STI032,
            NMEA_STI033,

            NMEA_Error,
            NMEA_REBOOT,
        }

        public ParsingResult Process(byte[] buffer, int length, ref NmeaType nmeaType, ref string showText)
        {
            ParsingResult pr = ParsingResult.None;
            string nmeaTxt = "";
            try
            {
                if (length <= 5)
                {
                    nmeaTxt = Encoding.ASCII.GetString(buffer, 0, length);   //Trim the string after *
                }
                else
                {
                    nmeaTxt = Encoding.ASCII.GetString(buffer, 0, length - 5);   //Trim the string after *
                }
                showText = Encoding.ASCII.GetString(buffer, 0, length - 2); //Remove \r \n
                nmeaType = GetMessageType(buffer, length);
            }
            catch (Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
            }

            try
            { 
                switch (nmeaType)
                {
                    case NmeaType.NMEA_GGA:
                        pr |= ParsingNmeaGga(nmeaTxt, nmeaType);
                        break;
                    case NmeaType.NMEA_RMC:
                        pr |= ParsingNmeaRmc(nmeaTxt, nmeaType);
                        break;
                    case NmeaType.NMEA_GPGSA:
                    case NmeaType.NMEA_GLGSA:
                    case NmeaType.NMEA_BDGSA:
                    case NmeaType.NMEA_GAGSA:
                    case NmeaType.NMEA_GNGSA:
                    case NmeaType.NMEA_GIGSA:
                        pr = ParsingNmeaGsa(nmeaTxt, nmeaType);
                        break;
                    case NmeaType.NMEA_GPGSV:
                    case NmeaType.NMEA_GLGSV:
                    case NmeaType.NMEA_BDGSV:
                    case NmeaType.NMEA_GAGSV:
                    case NmeaType.NMEA_GNGSV:
                    case NmeaType.NMEA_GIGSV:
                        pr = ParsingNmeaGsv(nmeaTxt, nmeaType);
                        break;
                    case NmeaType.NMEA_STI030:
                        pr = ParsingNmeaPsti030(nmeaTxt, nmeaType);
                        break;
                    case NmeaType.NMEA_STI032:
                        pr = ParsingNmeaPsti032(nmeaTxt, nmeaType);
                        break;
                    case NmeaType.NMEA_STI033:
                        pr = ParsingNmeaPsti033(nmeaTxt, nmeaType);
                        break;
                    case NmeaType.NMEA_GLL:
                    case NmeaType.NMEA_ZDA:
                    case NmeaType.NMEA_GNS:
                    case NmeaType.NMEA_VTG:
                    case NmeaType.NMEA_GST:
                    default:
                        return pr;
                }
            }
            catch (Exception ex)
            {
                AppTools.ShowDebug(nmeaType.ToString() + ex.ToString());
                showText = ex.ToString();
            }
            return pr;
        }

        private  ParsingResult ParsingNmeaGga(string s, NmeaType t)
        {
            ParsingResult pr = ParsingResult.None;
            string[] param = s.Split(',');
            if (param.Length <= 11)
            {
                return ParsingResult.None;
            }

            try
            {
                //Parameter 1 : UTC
                if (param[1].Length == 6)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaTimeInt(param[1]);
                }
                else if (param[1].Length > 6)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaTimeFloat(param[1]);
                }

                //Parameter 2, 3 : Latitude, N/S
                if (param[2].Length > 5 && param[3].Length == 1)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaLatitude(param[2], param[3]);
                }

                //Parameter 4, 5 : Longitude, E/W
                if (param[4].Length > 5 && param[5].Length == 1)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaLongitude(param[4], param[5]);
                }

                //Parameter 6 : GPS Quality Indicator
                if (param[6].Length == 1)
                {
                    pr |= MessageParser.GetParsingStatus().SetGgaGpsQualityInd(param[6][0]);
                }

                //Parameter 7 : Numbers of satellites in use, doesn't need parse

                //Parameter 8 : HDOP
                if (param[8].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaHdop(param[8]);
                }

                //Parameter 9, 10 : Mean sea level and meters
                if (param[9].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaMslAltitude(param[9]);
                }

                //Parameter 11, 12 : Geoidal separation and meters
                if (param[11].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaGeoidalSeparation(param[11]);
                }
            }
            catch (Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
            }
            return pr;
        }

        private  ParsingResult ParsingNmeaRmc(string s, NmeaType t)
        {
            ParsingResult pr = ParsingResult.None;
            string[] param = s.Split(',');
            if (param.Length <= 12)
            {
                return ParsingResult.None;
            }

            try
            {
                //Parameter 1 : UTC
                if (param[1].Length == 6)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaTimeInt(param[1]);
                }
                else if (param[1].Length > 6)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaTimeFloat(param[1]);
                }

                //Parameter 2 : RMC Status, doesn't need parse
                if (param[2].Length == 1 && param[3][0] == 'V')
                {
                    return pr;
                }

                //Parameter 3, 4 : Latitude, N/S
                if (param[3].Length > 5 && param[4].Length == 1)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaLatitude(param[3], param[4]);
                }

                //Parameter 5, 6 : Longitude, E/W
                if (param[5].Length > 5 && param[6].Length == 1)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaLongitude(param[5], param[6]);
                }

                //Parameter 7 : Speed over ground
                if (param[7].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetSpeedOverGround(param[7]);
                }

                //Parameter 8 : Course over ground
                if (param[8].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetCourseOverGround(param[8]);
                }

                //Parameter 9 : date
                if (param[9].Length == 6)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaDate(param[9]);
                }

                //Parameter 10, 11 : Magnetic variation, degrees, doesn't need parse
                
                //Parameter 12 : Mode Indicator
                if (param[12].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetRmcNavStatus(param[12][0]);
                }
            }
            catch (Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
            }
            return pr;
        }

        private static ParsingResult ParsingNmeaGsa(String s, NmeaType t)
        {
            ParsingResult pr = ParsingResult.None;
            try
            {
                String[] param = s.Split(',');
                if (param.Length < 18)
                {
                    return ParsingResult.None;
                }

                //Parameter 1 : Manual / Automatic Mode, pass it

                //Parameter 2 : Fixed Mode
                if (param[2].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetGsaFixedMode(param[2][0]);
                }

                //Parameter 18 : GNSS System ID - GPS : 1, GLONASS : 2, GALILEO : 3, NAVIC : 4
                int systemId = 0;
                if (param.Length > 18)
                {   //GSA field 18 is system ID.
                    systemId = Convert.ToInt32(param[18]);
                }

                switch (t)
                {
                    case NmeaType.NMEA_GPGSA:
                        MessageParser.GetParsingStatus().ClearInUseGpsList();
                        break;
                    case NmeaType.NMEA_GLGSA:
                        MessageParser.GetParsingStatus().ClearInUseGlonassList();
                        break;
                    case NmeaType.NMEA_BDGSA:
                        MessageParser.GetParsingStatus().ClearInUseBeidouList();
                        break;
                    case NmeaType.NMEA_GIGSA:
                        MessageParser.GetParsingStatus().ClearInUseNavicList();
                        break;
                    case NmeaType.NMEA_GNGSA:
                        MessageParser.GetParsingStatus().ClearInUseGNList(Convert.ToInt32(param[3]), systemId);
                        break;
                }

                //Parameter 3 ~ 14 :In use PRN
                int posFix = Convert.ToInt32(param[2]);
                for (int i = 3; i < 15; i++)
                {
                    if (param[i].Length <= 0)
                    {
                        break;
                    }
                    int prn = Convert.ToInt32(param[i]);
                    switch (t)
                    {
                        case NmeaType.NMEA_GPGSA:
                            MessageParser.GetParsingStatus().AddInUseGpsPrn(prn);
                            break;
                        case NmeaType.NMEA_GLGSA:
                            MessageParser.GetParsingStatus().AddInUseGlonassPrn(prn);
                            break;
                        case NmeaType.NMEA_BDGSA:
                            MessageParser.GetParsingStatus().AddInUseBeidouPrn(prn);
                            break;
                        case NmeaType.NMEA_GIGSA:
                            MessageParser.GetParsingStatus().AddInUseNavicPrn(prn);
                            break;
                        case NmeaType.NMEA_GNGSA:
                            MessageParser.GetParsingStatus().AddInUseGNPrn(prn, systemId);
                            break;
                    }
                }
                pr |= MessageParser.GetParsingStatus().MergeInUsePrn();
                //Parameter 15 : PDOP
                if (param[15].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaPdop(param[15]);
                }

                //Parameter 16 : HDOP
                if (param[16].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaHdop(param[16]);
                }

                //Parameter 17 : VDOP
                if (param[17].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaVdop(param[17]);
                }
            }
            catch (Exception ex)
            {
                AppTools.ShowDebug(ex.Message);
                return pr;
            }
            return pr;
        }

        private static int totalGsv = -1;
        private static int lastGsv = -1;
        private static ParsingResult ParsingNmeaGsv(String s, NmeaType t)
        {
            String[] param = s.Split(',');
            if ((param.Length != 8 && param.Length != 12 && param.Length != 16 && param.Length != 20) &&
                (param.Length != 9 && param.Length != 13 && param.Length != 17 && param.Length != 21))     //Has Signal ID
            {
                return ParsingResult.None;
            }

            int total = Convert.ToInt32(param[1]);
            int current = Convert.ToInt32(param[2]);
            int totalSate = Convert.ToInt32(param[3]);

            if (current == 1)
            {
                totalGsv = total;
                lastGsv = current;
            }

            int MaxParam;
            switch (param.Length)
            {
                case 8:
                    MaxParam = 8;
                    break;
                case 12:
                    MaxParam = 12;
                    break;
                case 16:
                    MaxParam = 16;
                    break;
                case 20:
                    MaxParam = 20;
                    break;
                //Has Signal ID
                case 9:
                    MaxParam = 8;
                    break;
                case 13:
                    MaxParam = 12;
                    break;
                case 17:
                    MaxParam = 16;
                    break;
                case 21:
                    MaxParam = 20;
                    break;
                default:
                    MaxParam = 0;
                    break;
            }

            for (int i = 4; i < MaxParam; i += 4)
            {
                if (param[i].Length <= 0)
                {
                    break;
                }
                //String pi3 = param[i + 3];
                int prn = Convert.ToInt32(param[i]);
                int ele = (param[i + 1].Length <= 0) ? 0 : Convert.ToInt32(param[i + 1]);
                int azi = (param[i + 2].Length <= 0) ? 0 : Convert.ToInt32(param[i + 2]);
                int snr = (param[i + 3].Length <= 0) ? 0 : Convert.ToInt32(param[i + 3]);

                switch (t)
                {
                    case NmeaType.NMEA_GPGSV:
                        MessageParser.GetParsingStatus().AddGnSateInfo(prn, ele, azi, snr);
                        break;
                    case NmeaType.NMEA_GLGSV:
                        MessageParser.GetParsingStatus().AddGlonassSateInfo(prn, ele, azi, snr);
                        break;
                    case NmeaType.NMEA_BDGSV:
                        MessageParser.GetParsingStatus().AddBeidouSateInfo(prn, ele, azi, snr);
                        break;
                    case NmeaType.NMEA_GIGSV:
                        MessageParser.GetParsingStatus().AddNavicSateInfo(prn, ele, azi, snr);
                        break;
                    case NmeaType.NMEA_GNGSV:
                        MessageParser.GetParsingStatus().AddGnSateInfo(prn, ele, azi, snr);
                        break;
                }
            }

            if (total == current)
            {
                totalGsv = -1;
                lastGsv = -1;
                return MessageParser.GetParsingStatus().MergeSateList();
            }
            return ParsingResult.None;
        }

        private ParsingResult ParsingNmeaPsti030(string s, NmeaType t)
        {
            ParsingResult pr = ParsingResult.None;
            string[] param = s.Split(',');
            if (param.Length <= 15)
            {
                return ParsingResult.None;
            }

            try
            {
                //Parameter 14 : RTK Age
                if (param[14].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaRtkAge(param[14]);
                }

                //Parameter 15 : RTK Ratio
                if (param[15].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaRtkRatio(param[15]);
                }
           }
            catch (Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
            }
            return pr;
        }

        private ParsingResult ParsingNmeaPsti032(string s, NmeaType t)
        {
            ParsingResult pr = ParsingResult.None;
            string[] param = s.Split(',');
            if (param.Length <= 10)
            {
                return ParsingResult.None;
            }

            try
            {
                //Parameter 6 : RTK East Projection
                if (param[6].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaRtkEastProj(param[6]);
                }

                //Parameter 7 : RTK North Projection
                if (param[7].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaRtkNorthProj(param[7]);
                }

                //Parameter 8 : RTK Up Projection
                if (param[8].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaRtkUpProj(param[8]);
                }

                //Parameter 9 : RTK Baseline Length
                if (param[9].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaRtkBaselineLen(param[9]);
                }

                //Parameter 10 : RTK Baseline Course
                if (param[10].Length > 0)
                {
                    pr |= MessageParser.GetParsingStatus().SetNmeaRtkBaselineCour(param[10]);
                }
            }
            catch (Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
            }
            return pr;
        }

        private ParsingResult ParsingNmeaPsti033(string s, NmeaType t)
        {
            ParsingResult pr = ParsingResult.None;
            string[] param = s.Split(',');
            if (param.Length <= 6)
            {
                return ParsingResult.None;
            }

            try
            {
                char type = ' ';
                //Parameter 5 : RTK Cycle-Slip Type
                if (param[5].Length > 0)
                {
                    type = param[5][0];
                }

                //Parameter 6 : Total RTK Cycle-Slip
                if (param[6].Length > 0)
                {
                    if(type == 'R')
                        pr |= MessageParser.GetParsingStatus().SetNmeaRtkRoverCycleSlip(param[6]);
                    else if(type == 'B')
                        pr |= MessageParser.GetParsingStatus().SetNmeaRtkBaseCycleSlip(param[6]);
                }
            }
            catch (Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
            }
            return pr;
        }

        private class NmeaTypeEntry
        {
            public string subNmea;
            public NmeaType type;
            public NmeaTypeEntry(string s, NmeaType t) { subNmea = s; type = t; }
        };

        private static NmeaTypeEntry[] nmeaTable = {
            new NmeaTypeEntry("$GPGGA,", NmeaType.NMEA_GGA),
            new NmeaTypeEntry("$GNGGA,", NmeaType.NMEA_GGA),
            new NmeaTypeEntry("$GIGGA,", NmeaType.NMEA_GGA),
            new NmeaTypeEntry("$BDGGA,", NmeaType.NMEA_GGA),
            new NmeaTypeEntry("$GAGGA,", NmeaType.NMEA_GGA),

            new NmeaTypeEntry("$GPGSA,", NmeaType.NMEA_GPGSA),
            new NmeaTypeEntry("$GLGSA,", NmeaType.NMEA_GLGSA),
            new NmeaTypeEntry("$BDGSA,", NmeaType.NMEA_BDGSA),
            new NmeaTypeEntry("$GAGSA,", NmeaType.NMEA_GAGSA),
            new NmeaTypeEntry("$GNGSA,", NmeaType.NMEA_GNGSA),
            new NmeaTypeEntry("$GIGSA,", NmeaType.NMEA_GIGSA),

            new NmeaTypeEntry("$GPGSV,", NmeaType.NMEA_GPGSV),
            //new NmeaTypeEntry("$GPGSV2,", NmeaType.NMEA_GPGSV2),
            new NmeaTypeEntry("$GLGSV,", NmeaType.NMEA_GLGSV),
            new NmeaTypeEntry("$BDGSV,", NmeaType.NMEA_BDGSV),
            new NmeaTypeEntry("$GAGSV,", NmeaType.NMEA_GAGSV),
            new NmeaTypeEntry("$GNGSV,", NmeaType.NMEA_GNGSV),
            new NmeaTypeEntry("$GIGSV,", NmeaType.NMEA_GIGSV),

            new NmeaTypeEntry("$GPRMC,", NmeaType.NMEA_RMC),
            new NmeaTypeEntry("$GNRMC,", NmeaType.NMEA_RMC),
            new NmeaTypeEntry("$GIRMC,", NmeaType.NMEA_RMC),
            new NmeaTypeEntry("$BDRMC,", NmeaType.NMEA_RMC),
            new NmeaTypeEntry("$GARMC,", NmeaType.NMEA_RMC),

            new NmeaTypeEntry("$GPGNS,", NmeaType.NMEA_GNS),
            new NmeaTypeEntry("$GNGNS,", NmeaType.NMEA_GNS),

            new NmeaTypeEntry("$GPGST,", NmeaType.NMEA_GST),
            new NmeaTypeEntry("$GNGST,", NmeaType.NMEA_GST),
            new NmeaTypeEntry("$GIGST,", NmeaType.NMEA_GST),

            new NmeaTypeEntry("$GPVTG,", NmeaType.NMEA_VTG),
            new NmeaTypeEntry("$GNVTG,", NmeaType.NMEA_VTG),
            new NmeaTypeEntry("$GIVTG,", NmeaType.NMEA_VTG),

            new NmeaTypeEntry("$GPGLL,", NmeaType.NMEA_GLL),
            new NmeaTypeEntry("$GNGLL,", NmeaType.NMEA_GLL),
            new NmeaTypeEntry("$GIGLL,", NmeaType.NMEA_GLL),

            new NmeaTypeEntry("$GPZDA,", NmeaType.NMEA_ZDA),
            new NmeaTypeEntry("$GNZDA,", NmeaType.NMEA_ZDA),
            new NmeaTypeEntry("$GIZDA,", NmeaType.NMEA_ZDA),

            new NmeaTypeEntry("$PSTI,030", NmeaType.NMEA_STI030),
            new NmeaTypeEntry("$PSTI,032", NmeaType.NMEA_STI032),
            new NmeaTypeEntry("$PSTI,033", NmeaType.NMEA_STI033),
                //new NmeaTypeEntry("", NmeaType.NMEA_Unknown),
            };

        private static bool VarifyNmeaChecksum(byte[] pt, int len)
        {
            if (pt[len - 5] != '*')
                return false;

            byte checksum = 0;
            for (int j = 1; j < len - 5; ++j)
            {
                checksum ^= pt[j];
            }
            //byte c1 = MiscUtil.Conversion.MiscConverter.GetNMEACheckSum(pt[len - 4], pt[len - 3]);
            return checksum == MiscUtil.Conversion.MiscConverter.GetNMEACheckSum(pt[len - 4], pt[len - 3]);
        }

        private static NmeaType GetMessageType(byte[] pt, int len)
        {
            NmeaType type = NmeaType.NMEA_Unknown;
            if (!VarifyNmeaChecksum(pt, len))
            {
                return NmeaType.NMEA_Error;
            }

            foreach (NmeaTypeEntry mte in nmeaTable)
            {
                int i;
                for (i = 0; i < mte.subNmea.Length; ++i)
                {
                    if (mte.subNmea[i] != pt[i])
                    {
                        break;
                    }
                }
                if (i == mte.subNmea.Length)
                {
                    type = mte.type;
                    break;
                }
            }
            return type;
        }
    }

    public class StqBinaryParser
    {
        public enum StqBinaryType
        {
            STQ_Unknown = 0,
            STQ_DC,
            STQ_DD,
            STQ_DE,
            STQ_DF,
            STQ_E0,
            STQ_E1,
            STQ_E2,
            STQ_E3,
            STQ_E4,
            STQ_E5,
            STQ_A8,
        }


        public ParsingResult Process(byte[] buffer, int length, ref StqBinaryType stqType, ref string showText)
        {
            stqType = GetMessageType(buffer, length);
            ParsingResult pr = ParsingResult.None;
            switch (stqType)
            {
                case StqBinaryType.STQ_DC:
                case StqBinaryType.STQ_DD:
                    break;
                case StqBinaryType.STQ_DE:
                    pr |= ParsingStqDE(buffer, length, ref showText);
                     break;
                case StqBinaryType.STQ_DF:
                    pr |= ParsingStqDF(buffer, length, ref showText);
                    break;
                case StqBinaryType.STQ_E0:
                    pr |= ParsingStqE0(buffer, length, ref showText);
                    break;
                case StqBinaryType.STQ_E1:
                    pr |= ParsingStqE1(buffer, length, ref showText);
                    break;
                case StqBinaryType.STQ_E2:
                    pr |= ParsingStqE2(buffer, length, ref showText);
                    break;
                case StqBinaryType.STQ_E3:
                    pr |= ParsingStqE3(buffer, length, ref showText);
                    break;
                case StqBinaryType.STQ_E4:
                    pr |= ParsingStqE4(buffer, length, ref showText);
                    break;
                case StqBinaryType.STQ_E5:
                    pr |= ParsingStqE5(buffer, length, ref showText);
                    break;
                default:
                    return pr;
            }
            return pr;
        }

        private StqBinaryType GetMessageType(byte[] pt, int len)
        {
            switch(pt[4])
            {
                case 0xDC:
                    return StqBinaryType.STQ_DC;
                case 0xDD:
                    return StqBinaryType.STQ_DD;
                case 0xDE:
                    return StqBinaryType.STQ_DE;
                case 0xDF:
                    return StqBinaryType.STQ_DF;
                case 0xE0:
                    return StqBinaryType.STQ_E0;
                case 0xE1:
                    return StqBinaryType.STQ_E1;
                case 0xE2:
                    return StqBinaryType.STQ_E2;
                case 0xE3:
                    return StqBinaryType.STQ_E3;
                case 0xE4:
                    return StqBinaryType.STQ_E4;
                case 0xE5:
                    return StqBinaryType.STQ_E5;
                case 0xA8:
                    return StqBinaryType.STQ_A8;
            }
            return StqBinaryType.STQ_Unknown;
        }

        private ParsingResult ParsingStqDE(byte[] buf, int len, ref string txt)
        {
            ParsingResult pr = ParsingResult.None;
            BigEndianBitConverter c = new BigEndianBitConverter();

            byte iod = buf[5];
            byte nsvs = buf[6];

            txt = string.Format("$SV_CH_STATUS(0xDE),IOD={0},NSVS={1}", iod, nsvs);
            if (nsvs == 0)
            {
                return ParsingResult.None;
            }

            MessageParser.GetParsingStatus().ClearInUseGpsList();
            MessageParser.GetParsingStatus().ClearInUseGlonassList();
            MessageParser.GetParsingStatus().ClearInUseBeidouList();
            MessageParser.GetParsingStatus().ClearInUseNavicList();
            int idx = 7;
            for (int i = 0; i < nsvs; ++i)
            {
                byte channel_id = buf[idx++];
                byte prn = buf[idx++];
                byte SV_status = buf[idx++];
                byte URA = buf[idx++];
                char cn0 = (char)buf[idx++];
                UInt16 ele = c.ToUInt16(buf, idx); idx += 2;
                UInt16 azi = c.ToUInt16(buf, idx); idx += 2;
                byte channel_status = buf[idx++];
                MessageParser.GetParsingStatus().AddGnSateInfo(prn, ele, azi, cn0);
                if (0 != (channel_status & 0x30))
                {
                    MessageParser.GetParsingStatus().AddInUseGNPrn(prn, 0);
                }
            }
            pr |= MessageParser.GetParsingStatus().MergeSateList();
            return pr;
        }

        private ParsingResult ParsingStqDF(byte[] buf, int len, ref string txt)
        {
            ParsingResult pr = ParsingResult.None;
            BigEndianBitConverter c = new BigEndianBitConverter();

            byte iod = buf[5];
            int idx = 6;
            byte nav_status = buf[idx++];
            UInt16 wn = c.ToUInt16(buf, idx); idx += 2;
            double tow = c.ToDouble(buf, idx); idx += 8;
            double ecef_x = c.ToDouble(buf, idx); idx += 8;
            double ecef_y = c.ToDouble(buf, idx); idx += 8;
            double ecef_z = c.ToDouble(buf, idx); idx += 8;
            float ecef_vx = c.ToSingle(buf, idx); idx += 4;
            float ecef_vy = c.ToSingle(buf, idx); idx += 4;
            float ecef_vz = c.ToSingle(buf, idx); idx += 4;
            double clock_bias = c.ToDouble(buf, idx); idx += 8;
            float clock_drift = c.ToSingle(buf, idx); idx += 4;
            float gdop = c.ToSingle(buf, idx); idx += 4;
            float pdop = c.ToSingle(buf, idx); idx += 4;
            float hdop = c.ToSingle(buf, idx); idx += 4;
            float vdop = c.ToSingle(buf, idx); idx += 4;
            float tdop = c.ToSingle(buf, idx); idx += 4;

            txt = string.Format(
                "$RCV_STATE(0xDF),IOD={0},NavState={1},WN={2},TOW={3:F3}",
                  //"{4:F2},{5:F2},{6:F2},{7:F2},{8:F2},{9:F2}," +
                  //"{10:F2},{11:F2}," +
                  //"{12:F2},{13:F2},{14:F2},{15:F2},{16:F2}",
                  iod, nav_status, wn, tow);
            //ecef_x, ecef_y, ecef_z, ecef_vx, ecef_vy, ecef_vz,
            //clock_bias, clock_drift,
            //gdop, pdop, hdop, vdop, tdop);
            pr |= MessageParser.GetParsingStatus().SetStqDfNavStatus(nav_status);
            pr |= MessageParser.GetParsingStatus().SetStqLatLonAlt(ecef_x, ecef_y, ecef_z);
            pr |= MessageParser.GetParsingStatus().SetPdop(pdop);
            pr |= MessageParser.GetParsingStatus().SetHdop(hdop);
            pr |= MessageParser.GetParsingStatus().SetVdop(vdop);
            pr |= MessageParser.GetParsingStatus().SetSqrtDfSpeed(ecef_vx, ecef_vy, ecef_vz);
            return pr;
        }

        private ParsingResult ParsingStqE0(byte[] buf, int len, ref string txt)
        {
            byte prn = buf[5];
            byte subFrmId = buf[6];
            txt = string.Format("$GPS_SUBFRAME(0xE0),SVID={0},SFID={1}", prn, subFrmId);
            return ParsingResult.None;
        }

        private ParsingResult ParsingStqE1(byte[] buf, int len, ref string txt)
        {
            byte prn = buf[5];
            byte subFrmId = buf[6];
            txt = string.Format("$GLONASS_STRING(0xE1),SVID={0},SFID={1}", prn, subFrmId);
            return ParsingResult.None;
        }

        private ParsingResult ParsingStqE2(byte[] buf, int len, ref string txt)
        {
            byte prn = buf[5];
            byte subFrmId = buf[6];
            txt = string.Format("$BEIDOU2_D1_SUBFRAME(0xE2),SVID={0},SFID={1}", prn, subFrmId);
            return ParsingResult.None;
        }

        private ParsingResult ParsingStqE3(byte[] buf, int len, ref string txt)
        {
            byte prn = buf[5];
            byte subFrmId = buf[6];
            txt = string.Format("$BEIDOU2_D2_SUBFRAME(0xE3),SVID={0},SFID={1}", prn, subFrmId);
            return ParsingResult.None;
        }

        private ParsingResult ParsingStqE4(byte[] buf, int len, ref string txt)
        {
            byte prn = buf[5];
            byte subFrmId = buf[6];
            txt = string.Format("$SBAS_SUBFRAME(0xE4),SVID={0},SFID={1}", prn, subFrmId);
            return ParsingResult.None;
        }

        private ParsingResult ParsingStqE5(byte[] buf, int len, ref string txt)
        {
            ParsingResult pr = ParsingResult.None;
            BigEndianBitConverter c = new BigEndianBitConverter();

            byte ver = buf[5];
            byte iod = buf[6];
            UInt16 weeks = c.ToUInt16(buf, 7);
            UInt32 tow = c.ToUInt32(buf, 9);
            UInt16 measPeriod = c.ToUInt16(buf, 13);
            byte measIndFlag = buf[15];
            byte reserve = buf[16];
            byte nmeas = buf[17];

            txt = string.Format("$EXT_RAW_MEAS(0xE5),V{0},IOD={1},WN={2},TOW={3},MeasPeriod={4},MeasIndicator={5:X2},NMEAS={6}",
                ver, iod, weeks, tow, measPeriod, measIndFlag, nmeas);

            pr |= MessageParser.GetParsingStatus().SetStqTimeWnTow(weeks, (tow / 1000.0));
            return pr;
        }
    }

    public class RtcmParser
    {
        public enum RtcmType
        {
            RTCM_Unknown = 0,

            RTCM_1005,
            RTCM_1077,
            RTCM_1087,
            RTCM_1107,
            RTCM_1117,
            RTCM_1127,
        }

        public ParsingResult Process(byte[] buffer, int length, ref RtcmType rtcmType, ref string showText)
        {
            rtcmType = GetMessageType(buffer, length);
            ParsingResult pr = ParsingResult.None;

            switch (rtcmType)
            {
                case RtcmType.RTCM_1005:
                    pr |= ParsingRtcm1005(buffer, length, ref showText);
                    break;
                case RtcmType.RTCM_1077:
                    pr |= ParsingRtcm1077(buffer, length, ref showText);
                    break;
                case RtcmType.RTCM_1087:
                    pr |= ParsingRtcm1087(buffer, length, ref showText);
                    break;
                case RtcmType.RTCM_1107:
                    pr |= ParsingRtcm1107(buffer, length, ref showText);
                    break;
                case RtcmType.RTCM_1117:
                    pr |= ParsingRtcm1117(buffer, length, ref showText);
                    break;
                case RtcmType.RTCM_1127:
                    pr |= ParsingRtcm1127(buffer, length, ref showText);
                    break;
                default:
                    return pr;
            }
            return pr;
        }

        private RtcmType GetMessageType(byte[] pt, int len)
        {
            int msgType = (pt[3] << 4) | (pt[4] >> 4);
            switch (msgType)
            {
                case 1005:
                    return RtcmType.RTCM_1005;
                case 1077:
                    return RtcmType.RTCM_1077;
                case 1087:
                    return RtcmType.RTCM_1087;
                case 1107:
                    return RtcmType.RTCM_1107;
                case 1117:
                    return RtcmType.RTCM_1117;
                case 1127:
                    return RtcmType.RTCM_1127;
                default:
                    return RtcmType.RTCM_Unknown;
            }
        }

        private ParsingResult ParsingRtcm1005(byte[] buf, int len, ref string txt)
        {
            ParsingResult pr = ParsingResult.None;
            byte[] data = new byte[8];
            MiscConverter.GetBitData(buf, 3, 12, 12, data);
            UInt16 satId = BitConverter.ToUInt16(data, 0);
            MiscConverter.GetBitData(buf, 3, 30, 1, data);
            byte gpQtyInd = data[0];
            MiscConverter.GetBitData(buf, 3, 31, 1, data);
            byte glQtyInd = data[0];
            MiscConverter.GetBitData(buf, 3, 32, 1, data);
            byte gaQtyInd = data[0];

            MiscConverter.GetBitData(buf, 3, 34, 38, data);
            MiscConverter.ConvertInt38Sign(data);
            Int64 ecefx = BitConverter.ToInt64(data, 0);

            MiscConverter.GetBitData(buf, 3, 74, 38, data);
            MiscConverter.ConvertInt38Sign(data);
            Int64 ecefy = BitConverter.ToInt64(data, 0);

            MiscConverter.GetBitData(buf, 3, 114, 38, data);
            MiscConverter.ConvertInt38Sign(data);
            Int64 ecefz = BitConverter.ToInt64(data, 0);

            txt = string.Format("$Rtcm1005,sta-Id={0},gps-Ind={1},glo-Ind={2},gal-Ind={3},ecef-x={4},ecef-y={5},ecef-z={6}",
                  satId, gpQtyInd, glQtyInd, gaQtyInd, ecefx, ecefy, ecefz);

            //if ((gpQtyInd | glQtyInd) != 0)
            //{
            //    pr |= MessageParser.GetParsingStatus().SetGgaGpsQualityInd('1');
            //    pr |= MessageParser.GetParsingStatus().SetGsaFixedMode('3');
            //    pr |= MessageParser.GetParsingStatus().SetRmcNavStatus('A');
            //}
            pr |= MessageParser.GetParsingStatus().SetRtcmFixedMode(true);

            pr |= MessageParser.GetParsingStatus().SetRtcmLatLonAlt(ecefx, ecefy, ecefz);
            return pr;
        }

        private ParsingResult ParsingRtcm1077(byte[] buf, int len, ref string txt)
        {
            int satNum = ParsingRtcmMsm7(buf, len);
            txt = string.Format("$Rtcm1077,satNum={0},size={1}", satNum, len);
            
            for (int i = 0; i < satNum; ++i)
            {
                int bitOrder = MiscConverter.GetNoneZeroBitPosition(BitConverter.GetBytes(msm_header.sat_mask), i);
                MessageParser.GetParsingStatus().AddGpsSateInfo(1 + bitOrder, 0, 0, sigData[i].snr_ext_resol / 16);
            }
            return MessageParser.GetParsingStatus().MergeSateList();
        }

        private ParsingResult ParsingRtcm1087(byte[] buf, int len, ref string txt)
        {
            int satNum = ParsingRtcmMsm7(buf, len);
            txt = string.Format("$Rtcm1087,satNum={0},size={1}", satNum, len);

            for (int i = 0; i < satNum; ++i)
            {
                int bitOrder = MiscConverter.GetNoneZeroBitPosition(BitConverter.GetBytes(msm_header.sat_mask), i);
                MessageParser.GetParsingStatus().AddGlonassSateInfo(65 + bitOrder, 0, 0, sigData[i].snr_ext_resol / 16);
            }
            return MessageParser.GetParsingStatus().MergeSateList();
        }

        private ParsingResult ParsingRtcm1107(byte[] buf, int len, ref string txt)
        {
            int satNum = ParsingRtcmMsm7(buf, len);
            txt = string.Format("$Rtcm1107,satNum={0},size={1}", satNum, len);

            for (int i = 0; i < satNum; ++i)
            {
                int bitOrder = MiscConverter.GetNoneZeroBitPosition(BitConverter.GetBytes(msm_header.sat_mask), i);
                MessageParser.GetParsingStatus().AddSbasSateInfo(33 + bitOrder, 0, 0, sigData[i].snr_ext_resol / 16);
            }
            return MessageParser.GetParsingStatus().MergeSateList();
        }

        private ParsingResult ParsingRtcm1117(byte[] buf, int len, ref string txt)
        {
            int satNum = ParsingRtcmMsm7(buf, len);
            txt = string.Format("$Rtcm1107,satNum={0},size={1}", satNum, len);

            for (int i = 0; i < satNum; ++i)
            {
                int bitOrder = MiscConverter.GetNoneZeroBitPosition(BitConverter.GetBytes(msm_header.sat_mask), i);
                MessageParser.GetParsingStatus().AddQzssSateInfo(193 + bitOrder, 0, 0, sigData[i].snr_ext_resol / 16);
            }
            return MessageParser.GetParsingStatus().MergeSateList();
        }

        private ParsingResult ParsingRtcm1127(byte[] buf, int len, ref string txt)
        {
            int satNum = ParsingRtcmMsm7(buf, len);
            txt = string.Format("$Rtcm1107,satNum={0},size={1}", satNum, len);

            for (int i = 0; i < satNum; ++i)
            {
                int bitOrder = MiscConverter.GetNoneZeroBitPosition(BitConverter.GetBytes(msm_header.sat_mask), i);
                MessageParser.GetParsingStatus().AddBeidouSateInfo(201 + bitOrder, 0, 0, sigData[i].snr_ext_resol / 16);
            }
            return MessageParser.GetParsingStatus().MergeSateList();
        }

        private struct RTCM_MSM_HEADER
        {
            //public byte valid;
            public UInt16 sta_id;
            public UInt32 gnss_epoch_time;

            public byte multi_msg_bit;
            public byte iods;
            public byte div_free_smoothing_ind;
            public byte smoothing_interval;
            public UInt32 sig_mask;

            public UInt64 sat_mask;
            public UInt64 cell_mask;
        }

        private struct RTCM_MSM_5_7_SAT_DATA
        {
            //public byte rough_range_in_ms;
            public byte ext_sat_info;   //knum + 7 in glonass, 0 in gps
            public UInt16 rough_range_mudulo_1_ms;
            public Int16 rough_phase_range_rate;
        }
        
        private struct RTCM_MSM_7_SIG_DATA
        {
            public Int32 fine_pseudorange_ext_resol;
            public Int32 fine_phase_range_ext_resol;

            public UInt16 lock_time_ind_ext_resol;
            public byte half_cycle_ambiguity_ind;

            public UInt16 snr_ext_resol;  //cn0 * 16
            public Int16 fine_phase_range_rate;
        }

        const int Msm7DataSize = 64;
        RTCM_MSM_HEADER msm_header = new RTCM_MSM_HEADER();
        RTCM_MSM_5_7_SAT_DATA[] satData = new RTCM_MSM_5_7_SAT_DATA[Msm7DataSize];
        RTCM_MSM_7_SIG_DATA[] sigData = new RTCM_MSM_7_SIG_DATA[Msm7DataSize];
        private int ParsingRtcmMsm7(byte[] buf, int len)
        {
            byte[] data = new byte[8];
            int bitIndex = 0;
            bitIndex += 12;  //pass Message number

            //U16 satId = 0;
            MiscConverter.GetBitData(buf, 3, bitIndex, 12, data);
            msm_header.sta_id = BitConverter.ToUInt16(data, 0);
            bitIndex += 12;

            MiscConverter.GetBitData(buf, 3, bitIndex, 30, data);
            msm_header.gnss_epoch_time = BitConverter.ToUInt16(data, 0);
            bitIndex += 30;

            MiscConverter.GetBitData(buf, 3, bitIndex, 1, data);
            msm_header.multi_msg_bit = data[0];
            bitIndex += 1;

            MiscConverter.GetBitData(buf, 3, bitIndex, 3, data);
            msm_header.iods = data[0];
            bitIndex += 3;

            //Reserved 7 bits
            //Reserved 2 bits for clock steering ind
            //Reserved 2 bits for external clock ind
            bitIndex += 11;

            MiscConverter.GetBitData(buf, 3, bitIndex, 1, data);
            msm_header.div_free_smoothing_ind = data[0];
            bitIndex += 1;

            MiscConverter.GetBitData(buf, 3, bitIndex, 3, data);
            msm_header.smoothing_interval = data[0];
            bitIndex += 3;

            MiscConverter.GetBitData(buf, 3, bitIndex, 64, data);
            msm_header.sat_mask = BitConverter.ToUInt64(data, 0);
            bitIndex += 64;
            
            int nSat = MiscConverter.GetBitFlagCounts(BitConverter.GetBytes(msm_header.sat_mask));
            if (nSat == 0)
            {
                return 0;
            }

            MiscConverter.GetBitData(buf, 3, bitIndex, 32, data);
            msm_header.sig_mask = BitConverter.ToUInt32(data, 0);
            bitIndex += 32;

            MiscConverter.GetBitData(buf, 3, bitIndex, nSat, data);
            msm_header.cell_mask = BitConverter.ToUInt64(data, 0);
            bitIndex += nSat;

            //Table 3.5-81 Content of Satellite Data for MSM5 and MSM7
            int i;
            for (i = 0; i < nSat; ++i)
            {
                MiscConverter.GetBitData(buf, 3, bitIndex, 8, data);
                satData[i].rough_range_mudulo_1_ms = BitConverter.ToUInt16(data, 0);
                bitIndex += 8;
            }

            for(i = 0; i<nSat; ++i)
            {
                MiscConverter.GetBitData(buf, 3, bitIndex, 4, data);
                satData[i].ext_sat_info = data[0];
                bitIndex += 4;
            }
            for(i = 0; i<nSat; ++i)
            {
                MiscConverter.GetBitData(buf, 3, bitIndex, 10, data);
                satData[i].rough_range_mudulo_1_ms = BitConverter.ToUInt16(data, 0);
                bitIndex += 10;
            }
            for(i = 0; i<nSat; ++i)
            {
                MiscConverter.GetBitData(buf, 3, bitIndex, 14, data);
                satData[i].rough_phase_range_rate = BitConverter.ToInt16(data, 0);
                bitIndex += 14;
            }

            //Table 3.5-88 Content of Signal Data for MSM7
            for(i = 0; i<nSat; ++i)
            {
                MiscConverter.GetBitData(buf, 3, bitIndex, 20, data);
                sigData[i].fine_pseudorange_ext_resol = BitConverter.ToInt32(data, 0);
                bitIndex += 20;
            }
            for(i = 0; i<nSat; ++i)
            {
                MiscConverter.GetBitData(buf, 3, bitIndex, 24, data);
                sigData[i].fine_phase_range_ext_resol = BitConverter.ToInt32(data, 0);
                bitIndex += 24;
            }
            for(i = 0; i<nSat; ++i)
            {
                MiscConverter.GetBitData(buf, 3, bitIndex, 10, data);
                sigData[i].lock_time_ind_ext_resol = BitConverter.ToUInt16(data, 0);
                bitIndex += 10;
            }
            for(i = 0; i<nSat; ++i)
            {
                MiscConverter.GetBitData(buf, 3, bitIndex, 1, data);
                sigData[i].half_cycle_ambiguity_ind = data[0];
                bitIndex += 1;
            }
            for(i = 0; i<nSat; ++i)
            {
                MiscConverter.GetBitData(buf, 3, bitIndex, 10, data);
                sigData[i].snr_ext_resol = BitConverter.ToUInt16(data, 0);
                bitIndex += 10;
            }
            for(i = 0; i<nSat; ++i)
            {
                MiscConverter.GetBitData(buf, 3, bitIndex, 15, data);
                sigData[i].fine_phase_range_rate = BitConverter.ToInt16(data, 0);
                bitIndex += 15;
            }
            return nSat;
        }
    }

    public class UbloxParser
    {
        public enum UbloxType
        {
            UBX_Unknown = 0,
            UBX_TimTp,
            UBX_NavSol,
            UBX_NavPvt,
            UBX_NavSvInfo,
            UBX_NavSvStatus,
            UBX_NavPosllh,
            UBX_NavDop,
            UBX_NavVelend,
        }

        public ParsingResult Process(byte[] buffer, int length, ref UbloxType ubloxType, ref string showText)
        {
            ubloxType = GetMessageType(buffer, length);
            ParsingResult pr = ParsingResult.None;

            switch (ubloxType)
            {
                case UbloxType.UBX_TimTp:
                    pr |= ParsingUbxTimTp(buffer, length, ref showText);
                    break;
                case UbloxType.UBX_NavSol:
                    pr |= ParsingUbxNavSol(buffer, length, ref showText);
                    break;
                case UbloxType.UBX_NavPvt:
                    pr |= ParsingUbxNavPvt(buffer, length, ref showText);
                    break;
                case UbloxType.UBX_NavSvInfo:
                    pr |= ParsingUbxNavSvInfo(buffer, length, ref showText);
                    break;
                case UbloxType.UBX_NavSvStatus:
                    pr |= ParsingUbxNavSvStatus(buffer, length, ref showText);
                    break;
                case UbloxType.UBX_NavPosllh:
                    pr |= ParsingUbxNavPosllh(buffer, length, ref showText);
                    break;
                case UbloxType.UBX_NavDop:
                    pr |= ParsingUbxNavDop(buffer, length, ref showText);
                    break;
                case UbloxType.UBX_NavVelend:
                    pr |= ParsingUbxNavVelend(buffer, length, ref showText);
                    break;
                default:
                    return pr;
            }
            return pr;
        }

        private UbloxType GetMessageType(byte[] pt, int len)
        {
            UInt16 msgType = (UInt16)((pt[2] << 8) | pt[3]);
            switch (msgType)
            {
                case 0x0D01:
                    return UbloxType.UBX_TimTp;
                case 0x0106:
                    return UbloxType.UBX_NavSol;
                case 0x0107:
                    return UbloxType.UBX_NavPvt;
                case 0x0130:
                    return UbloxType.UBX_NavSvInfo;
                case 0x0103:
                    return UbloxType.UBX_NavSvStatus;
                case 0x0102:
                    return UbloxType.UBX_NavPosllh;
                case 0x0104:
                    return UbloxType.UBX_NavDop;
                case 0x0112:
                    return UbloxType.UBX_NavVelend;
                default:
                    return UbloxType.UBX_Unknown;
            }
        }


        private ParsingResult ParsingUbxTimTp(byte[] buf, int len, ref string txt)
        {
            ParsingResult pr = ParsingResult.None;
            txt = string.Format("$UBX-TIM-TP");
            return pr;
        }

        private ParsingResult ParsingUbxNavSol(byte[] buf, int len, ref string txt)
        {
            ParsingResult pr = ParsingResult.None;
            txt = string.Format("$UBX-NAV-SOL");
            return pr;
        }

        private ParsingResult ParsingUbxNavPvt(byte[] buf, int len, ref string txt)
        {
            ParsingResult pr = ParsingResult.None;
            txt = string.Format("$UBX-NAV-PVT");
            return pr;
        }

        private ParsingResult ParsingUbxNavSvInfo(byte[] buf, int len, ref string txt)
        {
            ParsingResult pr = ParsingResult.None;
            txt = string.Format("$UBX-NAV_SVINFO");
            return pr;
        }

        private ParsingResult ParsingUbxNavSvStatus(byte[] buf, int len, ref string txt)
        {
            ParsingResult pr = ParsingResult.None;
            txt = string.Format("$UBX-NAV_SVSTATUS");
            return pr;
        }

        private ParsingResult ParsingUbxNavPosllh(byte[] buf, int len, ref string txt)
        {
            ParsingResult pr = ParsingResult.None;
            txt = string.Format("$UBX-NAV-POSLLH");
            return pr;
        }

        private ParsingResult ParsingUbxNavDop(byte[] buf, int len, ref string txt)
        {
            ParsingResult pr = ParsingResult.None;
            txt = string.Format("$UBX-NAV-DOP");
            return pr;
        }

        private ParsingResult ParsingUbxNavVelend(byte[] buf, int len, ref string txt)
        {
            ParsingResult pr = ParsingResult.None;
            txt = string.Format("$UBX-NAV-VELEND");
            return pr;
        }
    }
}
