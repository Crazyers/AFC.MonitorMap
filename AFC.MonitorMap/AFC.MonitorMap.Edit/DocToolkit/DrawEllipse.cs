/**********************************************************
** �ļ����� DrawEllipse.cs
** �ļ�����:
**
**---------------------------------------------------------
**�޸���ʷ��¼��
**�޸�ʱ��      �޸���    �޸����ݸ�Ҫ
**2018-09-14    xwj       ����
**
**********************************************************/

using System.Drawing;

namespace DrawTools.DocToolkit
{
    public class DrawEllipse : DrawRectangle
    {
        public DrawEllipse()
            : this(0, 0, 1, 1)
        {
        }

        public DrawEllipse(int x, int y, int width, int height)
            : base()
        {
            Rectangle = new Rectangle(x, y, width, height);
            Initialize();
        }

        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(Color, PenWidth);
            g.DrawEllipse(pen, DrawRectangle.GetNormalizedRectangle(Rectangle));
            pen.Dispose();
        }
    }
}