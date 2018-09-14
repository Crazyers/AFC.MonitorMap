/**********************************************************
** �ļ����� Tool.cs
** �ļ�����:Base class for all drawing tools
**
**---------------------------------------------------------
**�޸���ʷ��¼��
**�޸�ʱ��      �޸���    �޸����ݸ�Ҫ
**2018-09-14    xwj       ����
**
**********************************************************/

using System.Windows.Forms;

namespace DrawTools.DeviceTools
{
    public abstract class Tool
    {
        public virtual void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
        }

        public virtual void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
        {
        }

        public virtual void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
        {
        }
    }
}