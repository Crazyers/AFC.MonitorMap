/**********************************************************
** �ļ����� CommandDelete.cs
** �ļ�����:Delete command
**
**---------------------------------------------------------
**�޸���ʷ��¼��
**�޸�ʱ��      �޸���    �޸����ݸ�Ҫ
**2018-09-14    xwj       ����
**
**********************************************************/

using DrawTools.Device;
using DrawTools.DocToolkit;
using System.Collections.Generic;

namespace DrawTools.Command
{
    public class CommandDelete : Command
    {
        private List<DrawObject> cloneList;

        public CommandDelete(GraphicsList graphiList)
            : base(graphiList)
        {
            cloneList = new List<DrawObject>();
            foreach (DrawObject o in graphicsList.Selection)
            {
                cloneList.Add(o.Clone());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            foreach (DrawObject o in cloneList)
            {
                if (o.GetType().Name.Equals("Text"))
                {
                    Text label = (Text)o;
                    switch (label.TextType)
                    {
                        case "BOM_Logical_ID":
                            graphicsList.BomVerify.Remove(label.Texttest);
                            graphicsList.BomVerify.Remove(label.Texttest.Substring(label.Texttest.Length - 2)); 
                            break;

                        case "AGM_Logical_ID":
                            graphicsList.AGMVerify.Remove(label.Texttest);
                            graphicsList.AGMVerify.Remove(label.Texttest.Substring(label.Texttest.Length - 2));
                            break;

                        case "AGMDual_Logical_ID":
                            graphicsList.AGMVerify.Remove(label.Texttest);
                            graphicsList.AGMVerify.Remove(label.Texttest.Substring(label.Texttest.Length - 2));
                            break;

                        case "AGMWallDual_ID":
                            graphicsList.AGMWallDualVerify.Remove(label.Texttest);
                            break;

                        case "AGMWallDummy_Logical_ID":
                            graphicsList.AGMWallDummyVerify.Remove(label.Texttest);
                            break;

                        case "AGMWallSingle_Logical_ID":
                            graphicsList.AGMWallSingleVerify.Remove(label.Texttest);
                            break;

                        case "TVM_Logical_ID":
                            graphicsList.TVMVerify.Remove(label.Texttest);
                            graphicsList.TVMVerify.Remove(label.Texttest.Substring(label.Texttest.Length - 2));
                            break;

                        case "TCM_Logical_ID":
                            graphicsList.TCMVerify.Remove(label.Texttest);
                            graphicsList.TCMVerify.Remove(label.Texttest.Substring(label.Texttest.Length - 2));
                            break;

                        case "SC_Logical_ID":
                            graphicsList.SCVerify.Remove(label.Texttest);
                            graphicsList.SCVerify.Remove(label.Texttest.Substring(label.Texttest.Length - 2));
                            break;

                        default:
                            break;
                    }
                    graphicsList.SelectDevice(label.ObjectID);
                }
                else
                {
                    switch (o.GetType().Name)
                    {
                        case "BOM":
                            graphicsList.BomVerify.Remove(o.LogicIDTail);
                            graphicsList.IPVerify.Remove(o.DeviceIP);
                            graphicsList.SelectAlist(o.TagIDBase);
                            //LogHelper.DeviceDeviceLogInfo(string.Format("ɾ��BOM�豸������Ϣ_�豸���:{0},�豸IP��ַ:{1}", o.LogicIDTail,o.DeviceIP));
                            break;

                        case "AGMChannel":
                            graphicsList.AGMVerify.Remove(o.LogicIDTail);
                            graphicsList.IPVerify.Remove(o.DeviceIP);
                            graphicsList.SelectAlist(o.TagIDBase);
                            //LogHelper.DeviceDeviceLogInfo(string.Format("ɾ��AGMChannel�豸������Ϣ_�豸���:{0},�豸IP��ַ:{1}", o.LogicIDTail, o.DeviceIP));
                            break;

                        case "AGMChannelDual":
                            graphicsList.AGMVerify.Remove(o.LogicIDTail);
                            graphicsList.IPVerify.Remove(o.DeviceIP);
                            graphicsList.SelectAlist(o.TagIDBase);
                            //LogHelper.DeviceDeviceLogInfo(string.Format("ɾ��AGMChannelDual�豸������Ϣ_�豸���:{0},�豸IP��ַ:{1}", o.LogicIDTail, o.DeviceIP));
                            break;

                        case "AGMWallDual":
                            graphicsList.AGMWallDualVerify.Remove(o.LogicIDTail);
                            graphicsList.SelectAlist(o.TagIDBase);
                            //LogHelper.DeviceDeviceLogInfo(string.Format("ɾ��AGMWallDual�豸������Ϣ_�豸���:{0},�豸IP��ַ:{1}", o.LogicIDTail, o.DeviceIP));
                            break;

                        case "AGMWallDummy":
                            graphicsList.AGMWallDummyVerify.Remove(o.LogicIDTail);
                            graphicsList.SelectAlist(o.TagIDBase);
                            //LogHelper.DeviceDeviceLogInfo(string.Format("ɾ��AGMWallDummy�豸������Ϣ_�豸���:{0},�豸IP��ַ:{1}", o.LogicIDTail, o.DeviceIP));
                            break;

                        case "AGMWallSingle":
                            graphicsList.AGMWallSingleVerify.Remove(o.LogicIDTail);
                            graphicsList.SelectAlist(o.TagIDBase);
                            //LogHelper.DeviceDeviceLogInfo(string.Format("ɾ��AGMWallSingle�豸������Ϣ_�豸���:{0},�豸IP��ַ:{1}", o.LogicIDTail, o.DeviceIP));
                            break;

                        case "TCM":
                            graphicsList.TCMVerify.Remove(o.LogicIDTail);
                            graphicsList.IPVerify.Remove(o.DeviceIP);
                            graphicsList.SelectAlist(o.TagIDBase);
                            //LogHelper.DeviceDeviceLogInfo(string.Format("ɾ��TCM�豸������Ϣ_�豸���:{0},�豸IP��ַ:{1}", o.LogicIDTail, o.DeviceIP));
                            break;

                        case "TVM":
                            graphicsList.TVMVerify.Remove(o.LogicIDTail);
                            graphicsList.IPVerify.Remove(o.DeviceIP);
                            graphicsList.SelectAlist(o.TagIDBase);
                            //LogHelper.DeviceDeviceLogInfo(string.Format("ɾ��TVM�豸������Ϣ_�豸���:{0},�豸IP��ַ:{1}", o.LogicIDTail, o.DeviceIP));
                            break;

                        case "SC":
                            graphicsList.SCVerify.Remove(o.LogicIDTail);
                            graphicsList.IPVerify.Remove(o.DeviceIP);
                            graphicsList.SelectAlist(o.TagIDBase);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Undo()
        {
            graphicsList.UnselectAll();
            foreach (DrawObject o in cloneList)
            {
                graphicsList.Add(o);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Redo()
        {
            int n = graphicsList.Count;
            for (int i = n - 1; i >= 0; i--)
            {
                bool toDelete = false;
                DrawObject objectToDelete = graphicsList[i];
                foreach (DrawObject o in cloneList)
                {
                    if (objectToDelete.ID == o.ID)
                    {
                        toDelete = true;
                        break;
                    }
                }
                if (toDelete)
                {
                    graphicsList.RemoveAt(i);
                }
            }
        }
    }
}