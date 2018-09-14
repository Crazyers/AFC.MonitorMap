/**********************************************************
** �ļ����� ToolTCM.cs
** �ļ�����:TCM Tool
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
    public class ToolTCM : ToolRectangle
    {
        public ToolTCM()
        {
            Cursor = new Cursor(GetType(), "Ellipse.cur");
        }

        public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
            AddNewObject(drawArea, new TCM(e.X, e.Y, 34, 56));
        }
    }
}