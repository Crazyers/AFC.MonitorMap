/**********************************************************
** �ļ����� Array.cs
** �ļ�����:Array�豸
**
**---------------------------------------------------------
**�޸���ʷ��¼��
**�޸�ʱ��      �޸���    �޸����ݸ�Ҫ
**2018-09-14    xwj       ����
**
**********************************************************/

using DrawTools.DocToolkit;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DrawTools.Device
{
    public class Array : DrawRectangle
    {
        private int objectID;
        private bool flag;
        private bool showProperty;

        public Array()
            : this(0, 0, 1, 1)
        {
        }

        public Array(int x, int y, int width, int height)
            : base()
        {
            Rectangle = new Rectangle(x, y, width, height);

            logicIDTail = "";
            objectID = objIdInc++;
            flag = false;
            showProperty = false;

            Initialize();
        }

        public override DrawObject Clone()
        {
            Array drawArray = new Array();
            objIdInc--;
            drawArray.Rectangle = this.Rectangle;
            drawArray.Flag = this.Flag;

            FillDrawObjectFields(drawArray);
            return drawArray;
        }

        public override void AntiClockWiseDirection()
        { }

        public override void ClockWiseDirection()
        { }

        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(Color, PenWidth);
            Brush brushout = new SolidBrush(Color.FromArgb(255, 0, 255, 255));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (!flag)
            {
                Rectangle frect = new Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
                g.FillRectangle(brushout, frect);
                g.DrawRectangle(pen, Rectangle);
            }
            else
                g.DrawRectangle(pen, DrawRectangle.GetNormalizedRectangle(Rectangle));

            Brush brushin0 = new SolidBrush(Color.Red);
            StringFormat style = new StringFormat();
            style.Alignment = StringAlignment.Near;
            if (showProperty)
            {
                g.DrawString(
                     LogicIDTail,
                     new Font("����", 9, FontStyle.Regular),
                     brushin0,
                     Rectangle, style);
            }
            pen.Dispose();
        }

        public int ObjectID
        {
            get { return objectID; }
            set { objectID = value; }
        }

        public Rectangle RectangleLs
        {
            get { return Rectangle; }
            set { Rectangle = value; }
        }

        public new string LogicIDTail
        {
            get { return logicIDTail; }
            set { logicIDTail = value; }
        }

        public bool Flag
        {
            get { return flag; }
            set { flag = value; }
        }

        public bool ShowProperty
        {
            get { return showProperty; }
            set { showProperty = value; }
        }

        public override void ShowItemProperty(bool IsShow)
        {
            this.showProperty = IsShow;
        }

        public override Rectangle GetRectangle()
        {
            return Rectangle;
        }
    }
}