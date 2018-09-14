/**********************************************************
** �ļ����� ToolAGMWallSingle.cs
** �ļ�����:AGMWallSingle Tool
**
**---------------------------------------------------------
**�޸���ʷ��¼��
**�޸�ʱ��      �޸���    �޸����ݸ�Ҫ
**2018-09-14    xwj       ����
**
**********************************************************/

using DrawTools.Device;
using System.Windows.Forms;

namespace DrawTools.DeviceTools
{
    public class ToolAGMWallSingle : ToolRectangle
    {
        public ToolAGMWallSingle()
        {
            Cursor = new Cursor(GetType(), "Ellipse.cur");
        }

        public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
            AddNewObject(drawArea, new AGMWallSingle(e.X, e.Y, 50, 50));
        }
    }
}