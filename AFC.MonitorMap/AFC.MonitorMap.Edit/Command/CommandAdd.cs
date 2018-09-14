/**********************************************************
** �ļ����� CommandAdd.cs
** �ļ�����:�������
**
**---------------------------------------------------------
**�޸���ʷ��¼��
**�޸�ʱ��      �޸���    �޸����ݸ�Ҫ
**2018-09-14    xwj       ����
**
**********************************************************/

using DrawTools.DocToolkit;
using System;

namespace DrawTools.Command
{
    public class CommandAdd : Command
    {
        private DrawObject drawObject;

        public CommandAdd(GraphicsList graphiList)
            : base(graphiList)
        {
            this.drawObject = (DrawObject)graphicsList[0].Clone();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Undo()
        {
            graphicsList.DeleteLastAddedObject();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Redo()
        {
            graphicsList.UnselectAll();
            graphicsList.Add(drawObject);
        }
    }
}