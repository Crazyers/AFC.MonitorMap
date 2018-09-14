/**********************************************************
** �ļ����� GraphicsProperties.cs
** �ļ�����:
**
**---------------------------------------------------------
**�޸���ʷ��¼��
**�޸�ʱ��      �޸���    �޸����ݸ�Ҫ
**2018-09-14    xwj       ����
**
**********************************************************/

#region Using directives

using System.Drawing;

#endregion Using directives

namespace DrawTools.Model
{
    public class GraphicsProperties
    {
        private Color? color;
        private int? penWidth;

        public GraphicsProperties()
        {
            color = null;
            penWidth = null;
        }

        public Color? Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        public int? PenWidth
        {
            get
            {
                return penWidth;
            }
            set
            {
                penWidth = value;
            }
        }
    }
}