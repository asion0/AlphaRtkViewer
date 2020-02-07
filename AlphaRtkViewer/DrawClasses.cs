using RtkViewer.Properties;
using StqMessageParser;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace RtkViewer
{
    class DrawSnrBar
    {
        private Brush bgBrush = null;
        private Pen bgLinePen = new Pen(Color.FromArgb(255, 150, 150, 150));
        private Image inUseImage = null;
        private Image nonUseImage = null;
        private Image titleImage = null;
        //private SolidBrush inUseTxtDrawBrush = new SolidBrush(Color.FromArgb(255, 250, 250, 250));
        private SolidBrush inUseTxtDrawBrush = new SolidBrush(Color.FromArgb(255, 10, 10, 11));
        private SolidBrush nonUseTxtDrawBrush = null;
        private const float titleBgWidth = 182;
        private const float titleBgHeight = 22;
        private const float chartBgLeft = 2;
        private const float chartBgTop = 22;
        private const float chartBgRight = -7;
        private const float chartBgBottom = -28;
        private const float chartYGap = 11;
        private const float chartYGapLineCount = 5;
        private const float prnNumBgXStart = 6;
        private const float prnNumBgYStart = 80;
        private const float prnNumBgXGap = 25;
        private const float prnNumBgWidth = 20;
        private const float prnNumBgHeight = 20;
        private const float prnNumTxXOffset = -5;
        private const float prnNumTxYOffset = 2;
        private const float prnNumTxXRightOffset = 8;
        //private Font prnNumTxtFont = new Font("Impact", 10);
        private Font prnNumTxtFont = new Font("Arial", 10);
        private StringFormat drawFormat = new StringFormat();
        private SolidBrush inUseSnrBarBrush = null;
        private SolidBrush inUseSnrBarBrush2 = null;
        private SolidBrush nonUseSnrBarBrush = new SolidBrush(Color.FromArgb(255, 250, 250, 250));
        private const float prnSnrBarXOffset = 0;
        private const float prnSnrBarYOffset = 1;
        private const float prnSnrBarMaxHeight = 54;
        private const float prnSnrBarWidth = 20;
        private const float prnSnrMaxValue = 50;
        private Pen inUseSnrBarOuterPen = new Pen(Color.FromArgb(255, 200, 200, 200));
        private Pen nonUseSnrBarOuterPen = null;
        private SolidBrush inUseSnrBarTextBrush = new SolidBrush(Color.FromArgb(255, 250, 250, 250));
        private SolidBrush nonUseSnrBarTextBrush = new SolidBrush(Color.FromArgb(255, 128, 128, 128));
        private SolidBrush lowSnrBarTextBrush = new SolidBrush(Color.FromArgb(255, 5, 5, 5));
        private Font snrBarTxtFont = new Font("Tahoma", 7);
        private const int lowSnrBoundary = 12;
        private const int snrBarTxtYOffset = 62;
        private const int snrBarTxtHeight = 15;
        public DrawSnrBar(Color bgColor, string inUseImageName, string nonUseImageName, string titleImageName,
            Color nonUseTxtColor, Color inUseSnrBarColor, Color inUseSnrBarColor2)
        {
            bgBrush = new SolidBrush(bgColor);
            inUseImage = (Image)Resources.ResourceManager.GetObject(inUseImageName);
            nonUseImage = (Image)Resources.ResourceManager.GetObject(nonUseImageName);
            titleImage = (Image)Resources.ResourceManager.GetObject(titleImageName);
            drawFormat.Alignment = StringAlignment.Center;
            nonUseTxtDrawBrush = new SolidBrush(nonUseTxtColor);
            inUseSnrBarBrush = new SolidBrush(inUseSnrBarColor);
            inUseSnrBarBrush2 = new SolidBrush(inUseSnrBarColor2);
            nonUseSnrBarOuterPen = new Pen(inUseSnrBarColor);
        }

        public void DrawBg(Graphics g, int w, int h)
        {
            //w = (int)g.ClipBounds.Width;
            //h = (int)g.ClipBounds.Height;
            //w += 1;
            //h += 1;
            g.DrawImage(titleImage, 0, 0, titleBgWidth, titleBgHeight);
            g.FillRectangle(bgBrush, chartBgLeft + 1, chartBgTop + 1,
                w + chartBgRight - 1, h + chartBgBottom - 1);
            g.DrawRectangle(bgLinePen, chartBgLeft, chartBgTop, w + chartBgRight,
                h + chartBgBottom);
            for (int i = 0; i < chartYGapLineCount; ++i)
            {
                g.DrawLine(bgLinePen, chartBgLeft + 1, chartBgTop + (i + 1) * chartYGap,
                    w + chartBgRight + 1, chartBgTop + (i + 1) * chartYGap);
            }
        }

        public void Draw(Graphics g, List<ParsingStatus.SateInfo> s, List<int> sigList, int w, int h)
        {
            DrawBg(g, w, h);
            //return;
            if (sigList.Count == 1)
            {
                DrawSingle(g, s, w, h);
            }
            else
            {
                DrawDual(g, s, sigList, w, h);
            }
        }

        public void DrawSingle(Graphics g, List<ParsingStatus.SateInfo> s, int w, int h)
        {
            for (int i = 0; i < s.Count; i++)
            {
                if (s[i].prn == ParsingStatus.NullValue)
                {
                    break;
                }

                //Draw PRN image and number string
                Image img = (s[i].inUse) ? inUseImage : nonUseImage;
                Brush br = inUseTxtDrawBrush;
                g.DrawImage(img, prnNumBgXStart + i * prnNumBgXGap, prnNumBgYStart, prnNumBgWidth, prnNumBgHeight);
                g.DrawString(s[i].prn.ToString(), prnNumTxtFont, br,
                    new RectangleF(
                        prnNumBgXStart + i * prnNumBgXGap + prnNumTxXOffset,
                        prnNumBgYStart + prnNumTxYOffset,
                        prnNumBgWidth + prnNumTxXRightOffset,
                        prnNumBgHeight), drawFormat);

                if (s[i].GetSnr() == 0 || s[i].GetSnr() == ParsingStatus.NullValue)
                {
                    continue;
                }

                br = (s[i].inUse) ? inUseSnrBarBrush : nonUseSnrBarBrush;
                int snr = s[i].GetSnr();
                float barHeight = prnSnrBarMaxHeight * ((s[i].GetSnr() > prnSnrMaxValue) ? prnSnrMaxValue : s[i].GetSnr()) / prnSnrMaxValue;

                //Draw SNR color bar
                g.FillRectangle(br,
                    prnNumBgXStart + i * prnNumBgXGap + prnSnrBarXOffset,
                    chartBgTop + prnSnrBarYOffset + prnSnrBarMaxHeight - barHeight,
                    prnSnrBarWidth, barHeight);

                //Draw SNR bar frame
                Pen pen = (s[i].inUse) ? inUseSnrBarOuterPen : nonUseSnrBarOuterPen;
                g.DrawRectangle(pen,
                    prnNumBgXStart + i * prnNumBgXGap + prnSnrBarXOffset,
                    chartBgTop + prnSnrBarYOffset + prnSnrBarMaxHeight - barHeight,
                    prnSnrBarWidth, barHeight);

                //Draw SNR text
                br = (snr > lowSnrBoundary) ? ((s[i].inUse) ? inUseSnrBarTextBrush : nonUseSnrBarTextBrush) : (lowSnrBarTextBrush);
                g.DrawString(snr.ToString(), snrBarTxtFont, br,
                    new RectangleF(prnNumBgXStart + i * prnNumBgXGap + prnSnrBarXOffset, snrBarTxtYOffset,
                    prnSnrBarWidth, snrBarTxtHeight), drawFormat);
            }
        }

        public void DrawDual(Graphics g, List<ParsingStatus.SateInfo> s, List<int> sigList, int w, int h)
        {
            for (int i = 0; i < s.Count; i++)
            {
                if (s[i].prn == ParsingStatus.NullValue)
                {
                    break;
                }

                //Draw PRN image and number string
                Image img = (s[i].inUse) ? inUseImage : nonUseImage;
                Brush br = inUseTxtDrawBrush;
                g.DrawImage(img, prnNumBgXStart + i * prnNumBgXGap, prnNumBgYStart, prnNumBgWidth, prnNumBgHeight);
                g.DrawString(s[i].prn.ToString(), prnNumTxtFont, br,
                    new RectangleF(
                        prnNumBgXStart + i * prnNumBgXGap + prnNumTxXOffset,
                        prnNumBgYStart + prnNumTxYOffset,
                        prnNumBgWidth + prnNumTxXRightOffset,
                        prnNumBgHeight), drawFormat);

                //Draw first SNR bar
                int snr1 = s[i].GetSnr(sigList[0]);
                if (snr1 != 0 && snr1 != ParsingStatus.NullValue)
                {
                    br = (s[i].inUse) ? inUseSnrBarBrush : nonUseSnrBarBrush;
                    float barHeight = prnSnrBarMaxHeight * ((snr1 > prnSnrMaxValue) ? prnSnrMaxValue : snr1) / prnSnrMaxValue;

                    //Draw SNR color bar
                    g.FillRectangle(br,
                        prnNumBgXStart + i * prnNumBgXGap + prnSnrBarXOffset,
                        chartBgTop + prnSnrBarYOffset + prnSnrBarMaxHeight - barHeight,
                        prnSnrBarWidth / 2,
                        barHeight);

                    //Draw SNR bar frame
                    Pen pen = (s[i].inUse) ? inUseSnrBarOuterPen : nonUseSnrBarOuterPen;
                    g.DrawRectangle(pen,
                        prnNumBgXStart + i * prnNumBgXGap + prnSnrBarXOffset,
                        chartBgTop + prnSnrBarYOffset + prnSnrBarMaxHeight - barHeight,
                        prnSnrBarWidth / 2,
                        barHeight);

                    //Draw SNR text
                    br = (snr1 > lowSnrBoundary) ? ((s[i].inUse) ? inUseSnrBarTextBrush : nonUseSnrBarTextBrush) : (lowSnrBarTextBrush);
                    g.DrawString(snr1.ToString(), snrBarTxtFont, br,
                        new RectangleF(
                            prnNumBgXStart + i * prnNumBgXGap + prnSnrBarXOffset - 2,
                            snrBarTxtYOffset,
                            prnSnrBarWidth / 2 + 4,
                            snrBarTxtHeight), drawFormat);
                }

                //Draw second SNR bar
                int snr2 = s[i].GetSnr(sigList[1]);
                if (snr2 != 0 && snr2 != ParsingStatus.NullValue)
                {
                    br = (s[i].inUse) ? inUseSnrBarBrush2 : nonUseSnrBarBrush;
                    float barHeight = prnSnrBarMaxHeight * ((snr2 > prnSnrMaxValue) ? prnSnrMaxValue : snr2) / prnSnrMaxValue;

                    //Draw SNR color bar
                    g.FillRectangle(br,
                        prnNumBgXStart + i * prnNumBgXGap + prnSnrBarXOffset + prnSnrBarWidth / 2,
                        chartBgTop + prnSnrBarYOffset + prnSnrBarMaxHeight - barHeight,
                        prnSnrBarWidth / 2 - 1,
                        barHeight);

                    //Draw SNR bar frame
                    Pen pen = (s[i].inUse) ? inUseSnrBarOuterPen : nonUseSnrBarOuterPen;
                    g.DrawRectangle(pen,
                        prnNumBgXStart + i * prnNumBgXGap + prnSnrBarXOffset + prnSnrBarWidth / 2,
                        chartBgTop + prnSnrBarYOffset + prnSnrBarMaxHeight - barHeight,
                        prnSnrBarWidth / 2 - 1,
                        barHeight);

                    //Draw SNR text
                    br = (snr2 > lowSnrBoundary) ? ((s[i].inUse) ? inUseSnrBarTextBrush : nonUseSnrBarTextBrush) : (lowSnrBarTextBrush);
                    g.DrawString(snr2.ToString(), snrBarTxtFont, br,
                        new RectangleF(
                            prnNumBgXStart + i * prnNumBgXGap + prnSnrBarXOffset + prnSnrBarWidth / 2 - 2,
                            snrBarTxtYOffset,
                            prnSnrBarWidth / 2 + 4,
                            snrBarTxtHeight), drawFormat);
                }
            }
        }
    }

    class DrawEarth
    {
        private const int MaxConstellationSupport = 4;
        private const int MaxEarthBackground = 6;
        private Image[] inUsePrnImage = new Image[MaxConstellationSupport];
        private Image[] nonUsePrnImage = new Image[MaxConstellationSupport];
        private Image[] earthImage = new Image[MaxEarthBackground];
        private SolidBrush[] inUsePrnBrush = new SolidBrush[MaxConstellationSupport];
        private SolidBrush[] nonUsePrnBrush = new SolidBrush[MaxConstellationSupport];
        private int earthIndex = 0;
        public DrawEarth(string gpInuse, string gpNonUse,
            string glInuse, string glNonUse,
            string bdInuse, string bdNonUse,
            string giInuse, string giNonUse)
        {
            string[] inUseTable = { gpInuse, glInuse, bdInuse, giInuse };
            string[] nonUseTable = { gpNonUse, glNonUse, bdNonUse, giNonUse };
            Color[] inUseColorTable = {
                Color.FromArgb(255, 250, 250, 250),
                Color.FromArgb(255, 250, 250, 250),
                Color.FromArgb(255, 250, 250, 250),
                Color.FromArgb(255, 250, 250, 250) };
            Color[] nonUseColorTable = {
                Color.FromArgb(255, 26, 144, 255),
                Color.FromArgb(255, 156, 102, 204),
                Color.FromArgb(255, 255, 153, 1),
                Color.FromArgb(255, 255, 153, 1) };

            for (int i = 0; i < MaxConstellationSupport; ++i)
            {
                inUsePrnImage[i] = (Image)Resources.ResourceManager.GetObject(inUseTable[i]);
                nonUsePrnImage[i] = (Image)Resources.ResourceManager.GetObject(nonUseTable[i]);
                inUsePrnBrush[i] = new SolidBrush(inUseColorTable[i]);
                nonUsePrnBrush[i] = new SolidBrush(nonUseColorTable[i]);
            }
            earthImage[0] = (Image)Resources.ResourceManager.GetObject("Earth_N30_E120");
            earthImage[1] = (Image)Resources.ResourceManager.GetObject("Earth_S30_E120");
            earthImage[2] = (Image)Resources.ResourceManager.GetObject("Earth_N38_E22");
            earthImage[3] = (Image)Resources.ResourceManager.GetObject("Earth_S10_E26");
            earthImage[4] = (Image)Resources.ResourceManager.GetObject("Earth_N28_W98");
            earthImage[5] = (Image)Resources.ResourceManager.GetObject("Earth_S10_W60");

            drawFormat.Alignment = StringAlignment.Center;
        }

        public void SetEarthCenter(double lat, double lon)
        {
            if (lon > -142.0 && lon < -11.0)  //Americas
            {
                earthIndex = (lat > 0) ? 4 : 5;
            }
            else if (lon > -11 && lon < 70)    //Europe
            {
                earthIndex = (lat > 0) ? 2 : 3;
            }
            else
            {
                earthIndex = (lat > 0) ? 0 : 1;
            }
        }

        public void DrawBg(Graphics g)
        {
            g.DrawImage(earthImage[earthIndex], 17, 28, 178, 178);
        }

        private const int centerX = 105;
        private const int centerY = 116;
        private const int radius = 168 / 2;
        private const int prnNumBgW = 20;
        private const int prnNumBgH = 20;
        private const int prnNumBgXOffset = -(prnNumBgW / 2);
        private const int prnNumBgYOffset = -(prnNumBgH / 2);
        private const float prnNumTxXOffset = -5;
        private const float prnNumTxYOffset = 1;
        private const float prnNumTxXRightOffset = 8;
        private Font prnNumTxtFont = new Font("Impact", 10);
        private StringFormat drawFormat = new StringFormat();

        public void Draw(Graphics g, List<ParsingStatus.SateInfo>[] l, ParsingStatus.SateType[] t)
        {
            DrawBg(g);
            for (int i = 0; i < l.Length; i++)
            {
                if (l[i].Count == 0)
                {
                    continue;
                }

                foreach(ParsingStatus.SateInfo s in l[i])
                {
                    //2018/11/05 Alex modify ele scale as u center
                    //double ele = radius * Math.Cos(s.ele * Math.PI / 180.0);
                    double ele = radius * ((90 - s.ele) / 90.0);
                    double x = (ele * Math.Sin(s.azi * Math.PI / 180.0));
                    double y = -(ele * Math.Cos(s.azi * Math.PI / 180.0));

                    x += centerX + prnNumBgXOffset;
                    y += centerY + prnNumBgYOffset;

                    int index = (int)t[i] - 1;
                    Image img = ((s.inUse) ? inUsePrnImage : nonUsePrnImage)[index];
                    Brush br = ((s.inUse) ? inUsePrnBrush : nonUsePrnBrush)[index];

                    g.DrawImage(img, (float)x, (float)y, prnNumBgW, prnNumBgH);
                    g.DrawString(s.prn.ToString(), prnNumTxtFont, br,
                    new RectangleF( (int)x + prnNumTxXOffset, (int)y + prnNumTxYOffset,
                    prnNumBgW + prnNumTxXRightOffset, prnNumBgH), drawFormat);

                }
            }
        }
    }

    class DrawScatter
    {
        public static double[] GetRow(double[,] array2d, int row)
        {
            var w = array2d.GetLength(0);
            var h = array2d.GetLength(1);

            if (row >= h)
                throw new IndexOutOfRangeException("Row Index Out of Range");
            // Ensures the row requested is within the range of the 2-d array
            var returnRow = new double[w];
            for (var i = 0; i < w; i++)
                returnRow[i] = array2d[i, row];

            return returnRow;
        }

        public class LLA
        {
            public double latitude;
            public double longitude;
            public float altitude;
            public double currentX;
            public double currentY;
            public double currentZ;
            public double wgs_x;
            public double wgs_y;
            public double wgs_z;

            public LLA(double lat, double lon, float alt)
            {
                latitude = lat;
                longitude = lon;
                altitude = alt;
            }
            public LLA()
            {
                latitude = 0;
                longitude = 0;
                altitude = 0;
            }
            public double GetLatitude() { return latitude; }
            public double GetLontitude() { return longitude; }
            public float GetAltitude() { return altitude; }
        }

        protected class ScatterData
        {
            public class Matrix
            {
                public class BaseMatrix
                {
                    public double[,] val;
                    public int row, col, rowSize, colSize;
                    public int refCnt;

                    public BaseMatrix(int r, int c, double[,] v)
                    {
                        row = r;
                        rowSize = r;
                        col = c;
                        colSize = c;
                        refCnt = 1;
                        if (v != null)
                            val = (double[,])v.Clone();
                        else
                            val = new double[r, c];
                    }
                }

                public Matrix(Matrix m)
                {
                    _m = m._m;
                    _m.refCnt++;
                }

                public Matrix(int row, int col)
                {
                    _m = new BaseMatrix(row, col, null);
                }

                public Matrix()
                {
                    _m = new BaseMatrix(6, 6, null);
                }

                private BaseMatrix _m;
                private void Clone()
                {
                    _m.refCnt--;
                    _m = new BaseMatrix(_m.row, _m.col, _m.val);
                }

                private void Realloc(int r, int c)
                {
                    if (r == _m.rowSize && c == _m.colSize)
                    {
                        _m.row = _m.rowSize;
                        _m.col = _m.colSize;
                        return;
                    }

                    BaseMatrix m1 = new BaseMatrix(r, c, null);
                    for (int i = 0; i < ((r < _m.rowSize) ? r : _m.rowSize); ++i)
                    {
                        for (int j = 0; j < ((c < _m.colSize) ? c : _m.colSize); ++j)
                        {
                            m1.val[i, j] = _m.val[i, j];
                        }
                    }
                }

                private int pivot(int row)
                {
                    int k = row;
                    double antiMtx = -1;
                    for (int i = row; i < _m.row; ++i)
                    {
                        double temp = Math.Abs(_m.val[i, row]);
                        if (temp > antiMtx && temp != 0.0)
                        {
                            antiMtx = temp;
                            k = i;
                        }
                    }
                    if (_m.val[k, row] == 0)
                        return -1;
                    if (k != row)
                    {
                        for (int i = 0; i < _m.val.GetLength(1); ++i)
                        {
                            double tmp = _m.val[k, i];
                            _m.val[k, i] = _m.val[row, i];
                            _m.val[row, i] = tmp;
                        }
                        return k;
                    }
                    return 0;
                }

                // Value extraction method
                public int RowNo() { return _m.row; }
                public int ColNo() { return _m.col; }

                // Subscript operator
                public double GetAt(int r, int c)
                {
                    return _m.val[r, c];
                }

                public double SetAt(int r, int c, double v)
                {
                    _m.val[r, c] = v;
                    return v;
                }

                public Matrix MultiplicationBy(Matrix m)
                {
                    Matrix temp = new Matrix(_m.row, m._m.col);

                    for (int i = 0; i < _m.row; ++i)
                    {
                        for (int j = 0; j < m._m.col; ++j)
                        {
                            temp._m.val[i, j] = 0;
                            for (int k = 0; k < _m.col; ++k)
                            {
                                temp._m.val[i, j] += _m.val[i, k] * m._m.val[k, j];
                            }
                        }
                    }
                    return temp;
                }
            }

            public bool isInitialized = false;
            public List<LLA> pData = new List<LLA>();
            public Matrix centerWgs84 = new Matrix(3, 3);
            public Matrix enu = new Matrix(3, 1);
            protected double orgLatDeg, orgLonDeg;
            protected float orgH;
            protected Position centerPt;
            public ScatterData()
            {
                enu.SetAt(0, 0, 0);
                enu.SetAt(1, 0, 0);
                enu.SetAt(2, 0, 0);
            }

            public static double ToRadians(double val)
            {
                return (Math.PI / 180) * val;
            }

            protected void SetRotationMatrix()
            {
                double latRad = ToRadians(orgLatDeg);
                double lonRad = ToRadians(orgLonDeg);

                centerWgs84.SetAt(0, 0, -Math.Sin(lonRad));
                centerWgs84.SetAt(0, 1, Math.Cos(lonRad));
                centerWgs84.SetAt(0, 2, 0);
                centerWgs84.SetAt(1, 0, -Math.Sin(latRad) * Math.Cos(lonRad));
                centerWgs84.SetAt(1, 1, -Math.Sin(latRad) * Math.Sin(lonRad));
                centerWgs84.SetAt(1, 2, Math.Cos(latRad));
                centerWgs84.SetAt(2, 0, Math.Cos(latRad) * Math.Cos(lonRad));
                centerWgs84.SetAt(2, 1, Math.Cos(latRad) * Math.Sin(lonRad));
                centerWgs84.SetAt(2, 2, Math.Sin(latRad));
                centerPt = COO_geodetic_to_cartesian(latRad, lonRad, orgH);
            }

            protected void UpdateAllData()
            {
                for (int i = 0; i < pData.Count; ++i)
                {
                    LLA lla = pData[i];
                    pData[i] = SetEnu(lla.latitude, lla.longitude, lla.altitude);
                }
            }

            public void SetOrigin()
            {
                if (pData.Count == 0)
                    return;
                LLA p = pData[pData.Count - 1];
                orgLatDeg = p.GetLatitude();
                orgLonDeg = p.GetLontitude();
                orgH = p.GetAltitude();
                SetRotationMatrix();
                isInitialized = true;
                UpdateAllData();
            }

            public void SetOrigin(double latDeg, double lonDeg, float alt)
            {
                orgLatDeg = latDeg;
                orgLonDeg = lonDeg;
                orgH = alt;
                SetRotationMatrix();
                isInitialized = true;
                UpdateAllData();
            }

            public void ClearAllData()
            {
                pData.Clear();
            }

            const double WGS84_RA = 6378137.0;  // semi-major earth axis(ellipsoid equatorial radius)
            const double WGS84_INV_F = 298.257223563;   // inverse flattening of WGS-84
            const double WGS84_F = 1.0 / WGS84_INV_F; // inverse flattening of WGS-84
            const double WGS84_RB = WGS84_RA * (1.0 - WGS84_F); // semi-major earth axis(ellipsoid polar radius)
            const double WGS84_E2 = 2.0 * WGS84_F - WGS84_F * WGS84_F;    // eccentricity squared: (RA*RA-RB*RB)/RA*RA
            const double WGS84_E2P = WGS84_E2 / (1.0 - WGS84_E2);   // eccentricity squared: (RA*RA-RB*RB)/RB*RB
            public class Position
            {
                public double px;
                public double py;
                public double pz;
            };

            protected Position COO_geodetic_to_cartesian(double latRad, double lonRad, double alt)
            {
                double temp;
                double s_phi = Math.Sin(latRad);
                double c_phi = Math.Cos(latRad);

                // radius of curvature in prime vertical
                double N = WGS84_RA / Math.Sqrt(1 - WGS84_E2 * s_phi * s_phi);
                temp = (N + alt) * c_phi;
                Position xyz = new Position();
                xyz.px = temp * Math.Cos(lonRad);
                xyz.py = temp * Math.Sin(lonRad);
                xyz.pz = (N * (1 - WGS84_E2) + alt) * s_phi;
                return xyz;
            }

            protected LLA SetEnu(double latRad, double lonRad, double h)
            {
                Position pt = COO_geodetic_to_cartesian(latRad, lonRad, h);
                enu.SetAt(0, 0, pt.px - centerPt.px);
                enu.SetAt(1, 0, pt.py - centerPt.py);
                enu.SetAt(2, 0, pt.pz - centerPt.pz);

                enu = centerWgs84.MultiplicationBy(enu);
                LLA lla = new LLA(latRad, lonRad, (float)h);
                lla.wgs_x = pt.px;
                lla.wgs_y = pt.py;
                lla.wgs_z = pt.pz;
                lla.currentX = enu.GetAt(0, 0);
                lla.currentY = enu.GetAt(1, 0);
                lla.currentZ = enu.GetAt(2, 0);
                return lla;
            }

            protected LLA CreateLlaFromCooridate(double latDeg, double lonDeg, float alt)
            {
                return SetEnu(ToRadians(latDeg), ToRadians(lonDeg), (double)alt);
            }

            public static int MaxScatterPointCount = 500;
            public LLA AddCooridate(double latDeg, double lonDeg, float alt)
            {
                if (pData.Count == MaxScatterPointCount)
                {
                    pData.RemoveAt(0);
                }
                LLA lla = CreateLlaFromCooridate(latDeg, lonDeg, alt);
                pData.Add(lla);
                return lla;
            }
        }

        public DrawScatter()
        {
        }

        public int GetScaleCount() { return scaleTable.Length; }
        public double GetScale(int index) { return scaleTable[index]; }
        public void SetCurrentScale(int index) { currentScale = index; }

        public LLA AddLla(double latDeg, double lonDeg, float alt)
        {
            if (!scatterData.isInitialized)
            {
                SetOrigin(latDeg, lonDeg, alt);
            }
            return scatterData.AddCooridate(latDeg, lonDeg, alt);
        }

        public void SetOrigin() { scatterData.SetOrigin(); }
        public void SetOrigin(double latDeg, double lonDeg, float alt) { scatterData.SetOrigin(latDeg, lonDeg, alt); }
        public void ClearAllData() { scatterData.pData.Clear(); }

        private ScatterData scatterData = new ScatterData();
        private double[] scaleTable = { 0.01, 0.05, 0.1, 0.2, 0.5, 1, 2, 3, 5, 10, 20, 30, 40, 50, 100, 150, 200, 300 };
        private int currentScale = 5;

        private Font drawTextFont = new Font("Tahoma", 9);
        private StringFormat drawFormat = new StringFormat();
        private SolidBrush drawBrush = new SolidBrush(Color.FromArgb(255, 5, 5, 5));

        public void DrawBg(Graphics g)
        {
            //Draw axis scale
            for(int i = -2; i <= 2; ++i)
            {
                double f = scaleTable[currentScale] * i;
                string t;
                if (scaleTable[currentScale] < 0.1F)
                    t = f.ToString("F2");
                else if (scaleTable[currentScale] < 1F)
                    t = f.ToString("F1");
                else
                    t = f.ToString("F0");
                drawFormat.Alignment = StringAlignment.Far;
                g.DrawString(t, drawTextFont, drawBrush, new RectangleF(3, 182 - 37 * (i + 2), 33, 12), drawFormat);
                drawFormat.Alignment = StringAlignment.Center;
                g.DrawString(t, drawTextFont, drawBrush, new RectangleF(16 + 39 * (i + 2), 194, 33, 12), drawFormat);
            }
        }

        protected float graphWidth = 152;
        protected float graphHeight = 152;
        protected double centerX = 112, centerY = 116;
        protected float mTopPadding = 38;
        protected float mBottomPadding = 189;
        protected float mLeftPadding = 34;
        protected float mRightPadding = 122;

        private float[] CalcCoordinates(int index)
        {
            LLA lla = scatterData.pData[index];
            double plot_x = (lla.currentX * (graphWidth / 4) / scaleTable[currentScale]) + centerX;
            double plot_y = centerY - (lla.currentY * (graphHeight / 4) / scaleTable[currentScale]);
            if (plot_x < mLeftPadding || plot_x > graphWidth + mLeftPadding ||
                    plot_y < mTopPadding || plot_y > mTopPadding + graphHeight)
                return null;

            float[] coords = { (float)(int)(plot_x + 0.5), (float)(int)(plot_y + 0.5) };
            return coords;
        }

        float pointW = 4, pointH = 4;
        float pointOffsetX = -2F, pointOffsetY = -2F;
        const int MaxGBColor = 240;
        const int GBColorOfset = 5;
        public void Draw(Graphics g)
        {
            DrawBg(g);

            if (scatterData.pData.Count <= 0)
                return;
            float[] c = null;

            Pen pen = new Pen(Color.Red);
            Pen scatterPointBlue = new Pen(Color.Blue);
            SolidBrush whiteBr = new SolidBrush(Color.White);
            for (int i = 0; i < scatterData.pData.Count - 1; ++i)
            {
                c = CalcCoordinates(i);
                if (c != null)
                {
                    int gb = (int)(MaxGBColor - (double)(i + 2) * MaxGBColor / scatterData.pData.Count)+ GBColorOfset;
                    pen.Color = Color.FromArgb(255, 255, gb, gb);
                    g.FillEllipse(whiteBr, c[0] + pointOffsetX, c[1] + pointOffsetY, pointW, pointH);
                    g.DrawEllipse(pen, c[0] + pointOffsetX, c[1] + pointOffsetY, pointW, pointH);
                }
            }

            c = CalcCoordinates(scatterData.pData.Count - 1);
            if (c != null)
            {
                g.FillEllipse(whiteBr, c[0] + pointOffsetX, c[1] + pointOffsetY, pointW, pointH);
                g.DrawEllipse(scatterPointBlue, c[0] + pointOffsetX, c[1] + pointOffsetY, pointW, pointH);
            }
        }
    }

}
