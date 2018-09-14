/**********************************************************
** �ļ����� Command.cs
** �ļ�����:�����������
**
**---------------------------------------------------------
**�޸���ʷ��¼��
**�޸�ʱ��      �޸���    �޸����ݸ�Ҫ
**2018-09-14    xwj       ����
**
**********************************************************/

namespace DrawTools.Command
{
    public abstract class Command
    {
        protected GraphicsList graphicsList;

        public Command(GraphicsList graphicsList)
        {
            this.graphicsList = graphicsList;
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// 
        /// </summary>
        public abstract void Undo();

        /// <summary>
        /// 
        /// </summary>
        public abstract void Redo();
    }
}