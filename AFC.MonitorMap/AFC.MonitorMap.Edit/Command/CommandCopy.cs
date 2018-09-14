﻿/**********************************************************
** 文件名： CommandCopy.cs
** 文件作用:复制命令
**
**---------------------------------------------------------
**修改历史记录：
**修改时间      修改人    修改内容概要
**2018-09-14    xwj       增加
**
**********************************************************/

using DrawTools.Device;
using DrawTools.DocToolkit;
using System;
using System.Collections.Generic;

namespace DrawTools.Command
{
    public class CommandCopy : Command
    {
        public CommandCopy(GraphicsList graphiList)
            : base(graphiList)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            List<DrawObject> copyList = new List<DrawObject>();
            copyList.Clear();
            int n = graphicsList.ASelection.Count;
            for (int i = n - 1; i >= 0; i--)
            {
                bool addObject = true;
                string objName = ((DrawObject)graphicsList.ASelection[i]).GetType().Name;
                switch (objName)
                {
                    case "Text":
                        Text txt = (Text)graphicsList.ASelection[i];
                        if (!txt.TextType.Equals("Text"))
                        {
                            addObject = false;
                        }
                        break;
                    default:
                        break;
                }
                if (addObject)
                    copyList.Add(((DrawObject)graphicsList.ASelection[i]).Clone());
            }
            graphicsList.CopyList = copyList;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Redo()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Undo()
        {
            graphicsList.CopyList.Clear();
        }
    }
}