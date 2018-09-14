/**********************************************************
** �ļ����� ToolPolygon.cs
** �ļ�����:Polygon Tool
**
**---------------------------------------------------------
**�޸���ʷ��¼��
**�޸�ʱ��      �޸���    �޸����ݸ�Ҫ
**2018-09-14    xwj       ����
**
**********************************************************/

using DrawTools.DocToolkit;
using System.Drawing;
using System.Windows.Forms;

namespace DrawTools.DeviceTools
{
    public class ToolPolygon : ToolObject
    {
        public ToolPolygon()
        {
            Cursor = new Cursor(GetType(), "Pencil.cur");
        }

        private int lastX;
        private int lastY;
        private DrawPolygon newPolygon;
        private const int minDistance = 15 * 15;

        public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
            newPolygon = new DrawPolygon(e.X, e.Y, e.X + 1, e.Y + 1);
            AddNewObject(drawArea, newPolygon);
            lastX = e.X;
            lastY = e.Y;
        }

        public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
        {
            drawArea.Cursor = Cursor;

            if (e.Button != MouseButtons.Left)
                return;

            if (newPolygon == null)
                return;

            Point point = new Point(e.X, e.Y);
            int distance = (e.X - lastX) * (e.X - lastX) + (e.Y - lastY) * (e.Y - lastY);

            if (distance < minDistance)
            {
                newPolygon.MoveHandleTo(point, newPolygon.HandleCount);
            }
            else
            {
                newPolygon.AddPoint(point);
                lastX = e.X;
                lastY = e.Y;
            }

            drawArea.Refresh();
        }

        public override void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
        {
            newPolygon = null;

            base.OnMouseUp(drawArea, e);
        }
    }
}