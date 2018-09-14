/**********************************************************
** �ļ����� ToolRectangle.cs
** �ļ�����:Rectangle Tool
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
    public class ToolRectangle : ToolObject
    {
        public ToolRectangle()
        {
            Cursor = new Cursor(GetType(), "Rectangle.cur");
        }

        public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
            AddNewObject(drawArea, new DrawRectangle(e.X, e.Y, 1, 1));
        }

        public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
        {
            drawArea.Cursor = Cursor;

            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    Point point = new Point(e.X, e.Y);
                    drawArea.GraphicsList[0].MoveHandleTo(point, 5);
                    drawArea.Refresh();
                }
                catch { }
            }
        }
    }
}