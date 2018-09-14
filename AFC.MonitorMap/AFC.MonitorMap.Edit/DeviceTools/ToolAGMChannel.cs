/**********************************************************
** �ļ����� ToolAGMChannel.cs
** �ļ�����:AGMChannel tool
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
    public class ToolAGMChannel : ToolRectangle
    {
        public ToolAGMChannel()
        {
            Cursor = new Cursor(GetType(), "Ellipse.cur");
        }

        public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
            AddNewObject(drawArea, new AGMChannel(e.X, e.Y, 71, 27));
        }
    }
}