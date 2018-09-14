/**********************************************************
** �ļ����� DrawRectangle.cs
** �ļ�����:
**
**---------------------------------------------------------
**�޸���ʷ��¼��
**�޸�ʱ��      �޸���    �޸����ݸ�Ҫ
**2018-09-14    xwj       ����
**
**********************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;

namespace DrawTools.DocToolkit
{
    public class DrawRectangle : DrawObject
    {
        public string RecID { get; set; }

        public string StationID { get; set; }

        public string DeviceID { get; set; }

        public string DeviceName { get; set; }

        public string DeviceType { get; set; }

        public string DeviceSubType { get; set; }

        public string DeviceSeqInStation { get; set; }

        public string LobbyId { get; set; }

        public string GroupID { get; set; }

        public string DeviceSeqInGroup { get; set; }

        public string XPos { get; set; }

        public string YPos { get; set; }

        public string Angle { get; set; }

        public string Entry { get; set; }

        public string IpAdd { get; set; }

        public string Device_W { get; set; }
        public string Device_H { get; set; }

        public string Region_W { get; set; }
        public string Region_H { get; set; }

        private Rectangle rectangle;
        private const string entryRectangle = "Rect";

        protected Rectangle Rectangle
        {
            get
            {
                return rectangle;
            }
            set
            {
                rectangle = value;
            }
        }

        public DrawRectangle()
            : this(0, 0, 1, 1)
        {
        }

        public DrawRectangle(int x, int y, int width, int height)
            : base()
        {
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
            Initialize();
        }

        public override DrawObject Clone()
        {
            return null;
        }

        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(Color, PenWidth);

            g.DrawRectangle(pen, DrawRectangle.GetNormalizedRectangle(Rectangle));

            pen.Dispose();
        }

        protected void SetRectangle(int x, int y, int width, int height)
        {
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
        }

        public override int HandleCount
        {
            get
            {
                return 8;
            }
        }

        public override Point GetHandle(int handleNumber)
        {
            int x, y, xCenter, yCenter;

            xCenter = rectangle.X + rectangle.Width / 2;
            yCenter = rectangle.Y + rectangle.Height / 2;
            x = rectangle.X;
            y = rectangle.Y;

            switch (handleNumber)
            {
                case 1:
                    x = rectangle.X;
                    y = rectangle.Y;
                    break;

                case 2:
                    x = xCenter;
                    y = rectangle.Y;
                    break;

                case 3:
                    x = rectangle.Right;
                    y = rectangle.Y;
                    break;

                case 4:
                    x = rectangle.Right;
                    y = yCenter;
                    break;

                case 5:
                    x = rectangle.Right;
                    y = rectangle.Bottom;
                    break;

                case 6:
                    x = xCenter;
                    y = rectangle.Bottom;
                    break;

                case 7:
                    x = rectangle.X;
                    y = rectangle.Bottom;
                    break;

                case 8:
                    x = rectangle.X;
                    y = yCenter;
                    break;
            }

            return new Point(x, y);
        }

        public override int HitTest(Point point)
        {
            if (Selected)
            {
                for (int i = 1; i <= HandleCount; i++)
                {
                    if (GetHandleRectangle(i).Contains(point))
                        return i;
                }
            }

            if (PointInObject(point))
                return 0;

            return -1;
        }

        protected override bool PointInObject(Point point)
        {
            return rectangle.Contains(point);
        }

        public override Cursor GetHandleCursor(int handleNumber)
        {
            switch (handleNumber)
            {
                case 1:
                    return Cursors.SizeNWSE;

                case 2:
                    return Cursors.SizeNS;

                case 3:
                    return Cursors.SizeNESW;

                case 4:
                    return Cursors.SizeWE;

                case 5:
                    return Cursors.SizeNWSE;

                case 6:
                    return Cursors.SizeNS;

                case 7:
                    return Cursors.SizeNESW;

                case 8:
                    return Cursors.SizeWE;

                default:
                    return Cursors.Default;
            }
        }

        public override void MoveHandleTo(Point point, int handleNumber)
        {
            int left = Rectangle.Left;
            int top = Rectangle.Top;
            int right = Rectangle.Right;
            int bottom = Rectangle.Bottom;

            switch (handleNumber)
            {
                case 1:
                    left = point.X;
                    top = point.Y;
                    break;

                case 2:
                    top = point.Y;
                    break;

                case 3:
                    right = point.X;
                    top = point.Y;
                    break;

                case 4:
                    right = point.X;
                    break;

                case 5:
                    right = point.X;
                    bottom = point.Y;
                    break;

                case 6:
                    bottom = point.Y;
                    break;

                case 7:
                    left = point.X;
                    bottom = point.Y;
                    break;

                case 8:
                    left = point.X;
                    break;
            }

            SetRectangle(left, top, right - left, bottom - top);
        }

        public override void AntiClockWiseDirection()
        {
            SetRectangle(this.rectangle.X, this.rectangle.Y, this.rectangle.Height, this.rectangle.Width);
        }

        public override void ClockWiseDirection()
        {
            SetRectangle(this.rectangle.X, this.rectangle.Y, this.rectangle.Height, this.rectangle.Width);
        }

        public override void setRectangleX(int X)
        {
            this.rectangle.X = X;
        }

        public override void setRectangleY(int Y)
        {
            this.rectangle.Y = Y;
        }

        public override void setRectangleWidth(int width)
        {
            this.rectangle.Width = width;
        }

        public override void setRectangleHeight(int height)
        {
            this.rectangle.Height = height;
        }

        public override bool IntersectsWith(Rectangle rectangle)
        {
            return Rectangle.IntersectsWith(rectangle);
        }

        public override void Move(int deltaX, int deltaY)
        {
            rectangle.X += deltaX;
            rectangle.Y += deltaY;
        }

        public override void Dump()
        {
            base.Dump();

            Trace.WriteLine("rectangle.X = " + rectangle.X.ToString(CultureInfo.InvariantCulture));
            Trace.WriteLine("rectangle.Y = " + rectangle.Y.ToString(CultureInfo.InvariantCulture));
            Trace.WriteLine("rectangle.Width = " + rectangle.Width.ToString(CultureInfo.InvariantCulture));
            Trace.WriteLine("rectangle.Height = " + rectangle.Height.ToString(CultureInfo.InvariantCulture));
        }

        public override void Normalize()
        {
            rectangle = DrawRectangle.GetNormalizedRectangle(rectangle);
        }

        public override void SaveToStream(System.Runtime.Serialization.SerializationInfo info, int orderNumber)
        {
            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryRectangle, orderNumber),
                rectangle);

            base.SaveToStream(info, orderNumber);
        }

        public override void LoadFromStream(SerializationInfo info, int orderNumber)
        {
            rectangle = (Rectangle)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryRectangle, orderNumber),
                typeof(Rectangle));

            base.LoadFromStream(info, orderNumber);
        }

        #region Helper Functions

        public static Rectangle GetNormalizedRectangle(int x1, int y1, int x2, int y2)
        {
            if (x2 < x1)
            {
                int tmp = x2;
                x2 = x1;
                x1 = tmp;
            }

            if (y2 < y1)
            {
                int tmp = y2;
                y2 = y1;
                y1 = tmp;
            }

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public static Rectangle GetNormalizedRectangle(Point p1, Point p2)
        {
            return GetNormalizedRectangle(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static Rectangle GetNormalizedRectangle(Rectangle r)
        {
            return GetNormalizedRectangle(r.X, r.Y, r.X + r.Width, r.Y + r.Height);
        }

        #endregion Helper Functions

        public override XmlElement ToXmlElement(XmlDocument doc)
        {
            XmlElement RectangleNode = doc.CreateElement("Rectangle");
            return RectangleNode;
        }
    }
}