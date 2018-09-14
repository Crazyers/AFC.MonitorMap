/**********************************************************
** �ļ����� DrawArea.cs
** �ļ�����:���崦����
** 
**---------------------------------------------------------
**�޸���ʷ��¼��
**�޸�ʱ��      �޸���    �޸����ݸ�Ҫ
**2018-09-14    xwj       ����
**
**********************************************************/
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Xml;
using System.IO;
using DrawTools.DocToolkit;
using DrawTools.DeviceTools;
using DrawTools.Model;
using DrawTools.Command;
using DrawTools.Device;
using DrawTools.DB;
using DrawTools.Controls;

namespace DrawTools
{
    public partial class DrawArea : DrawControl
    {
        #region Constructor
        public DrawArea(string stationId)
        {
            InitializeComponent();
            _isNetMap = true;
            _isDeviceMap = true;
            width = 1200;
            height = 800;
            this.StationID = stationId;
        }
        #endregion Constructor

        #region Members
        private GraphicsList graphicsList;
        private List<DrawObject> copyList = new List<DrawObject>();
        private DrawToolType activeTool;
        private Tool[] tools;
        private bool _isNetMap;
        public bool IsNetMap
        {
            get { return _isNetMap; }
            set { _isNetMap = value; }
        }
        private bool _isDeviceMap;
        public bool IsDeviceMap
        {
            get { return _isDeviceMap; }
            set { _isDeviceMap = value; }
        }
        private DrawForm owner;
        private DocManager docManager;
        private ContextMenuStrip m_ContextMenu;
        private UndoManager undoManager;
        private int blockWidth = 100;
        private Point gPoint;

        private int width;
        private int height;

        #endregion

        #region Properties

        /// <summary>
        /// Reference to the owner form
        /// </summary>
        public DrawForm Owner
        {
            get
            {
                return owner;
            }
            set
            {
                owner = value;
            }
        }

        /// <summary>
        /// Reference to DocManager
        /// </summary>
        public DocManager DocManager
        {
            get
            {
                return docManager;
            }
            set
            {
                docManager = value;
            }
        }

        /// <summary>
        /// Active drawing tool.
        /// </summary>
        public DrawToolType ActiveTool
        {
            get
            {
                return activeTool;
            }
            set
            {
                activeTool = value;
            }
        }

        /// <summary>
        /// List of graphics objects.
        /// </summary>
        public GraphicsList GraphicsList
        {
            get
            {
                return graphicsList;
            }
            set
            {
                graphicsList = value;
            }
        }

        /// <summary>
        /// Return True if Undo operation is possible
        /// </summary>
        public bool CanUndo
        {
            get
            {
                if (undoManager != null)
                {
                    return undoManager.CanUndo;
                }

                return false;
            }
        }

        /// <summary>
        /// Return True if Redo operation is possible
        /// </summary>
        public bool CanRedo
        {
            get
            {
                if (undoManager != null)
                {
                    return undoManager.CanRedo;
                }

                return false;
            }
        }

        public int AWidth
        {
            get { return width; }
            set { width = value; }
        }

        public int AHeight
        {
            get { return height; }
            set { height = value; }
        }

        private string stationIP = "0";

        public string StationIP
        {
            get { return stationIP; }
            set { stationIP = value; }
        }


        private string stationID;

        public string StationID
        {
            get { return stationID; }
            set { stationID = value; }
        }


        #endregion

        #region Other Functions

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="docManager"></param>
        public void Initialize(DrawForm owner, DocManager docManager)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            this.Owner = owner;
            this.DocManager = docManager;
            activeTool = DrawToolType.Pointer;
            graphicsList = new GraphicsList();
            undoManager = new UndoManager(graphicsList);
            tools = new Tool[(int)DrawToolType.NumberOfDrawTools];
            tools[(int)DrawToolType.Pointer] = new ToolPointer();
            tools[(int)DrawToolType.Rectangle] = new ToolRectangle();
            tools[(int)DrawToolType.Ellipse] = new ToolEllipse();
            tools[(int)DrawToolType.Line] = new ToolLine();
            tools[(int)DrawToolType.Polygon] = new ToolPolygon();
            tools[(int)DrawToolType.Bom] = new ToolBOM();
            tools[(int)DrawToolType.AGMChannel] = new ToolAGMChannel();
            tools[(int)DrawToolType.AGMChannelDual] = new ToolAGMChannelDual();
            tools[(int)DrawToolType.AGMWallDual] = new ToolAGMWallDual();
            tools[(int)DrawToolType.AGMWallDummy] = new ToolAGMWallDummy();
            tools[(int)DrawToolType.AGMWallSingle] = new ToolAGMWallSingle();
            tools[(int)DrawToolType.Array] = new ToolArray();
            tools[(int)DrawToolType.PaidArea] = new ToolPaidArea();
            tools[(int)DrawToolType.SC] = new ToolSC();
            tools[(int)DrawToolType.TCM] = new ToolTCM();
            tools[(int)DrawToolType.TVM] = new ToolTVM();
            tools[(int)DrawToolType.Text] = new ToolText();
            tools[(int)DrawToolType.Switch] = new ToolSwitch();
            tools[(int)DrawToolType.Port] = new ToolPort();
        }

        /// <summary>
        /// Add command to history.
        /// </summary>
        public void AddCommandToHistory(DrawTools.Command.Command command)
        {
            undoManager.AddCommandToHistory(command);
        }

        /// <summary>
        /// Clear Undo history.
        /// </summary>
        public void ClearHistory()
        {
            undoManager.ClearHistory();
        }

        /// <summary>
        /// Undo
        /// </summary>
        public void Undo()
        {
            undoManager.Undo();
            SetDirty();
            Refresh();
        }

        /// <summary>
        /// Redo
        /// </summary>
        public void Redo()
        {
            undoManager.Redo();
            SetDirty();
            Refresh();
        }

        /// <summary>
        /// Set dirty flag (file is changed after last save operation)
        /// </summary>
        public void SetDirty()
        {
            DocManager.Dirty = true;
        }

        /// <summary>
        ///��ʱ��ת��
        /// </summary>
        public void AntiClockWise()
        {
            if (this.GraphicsList.SelectionCount == 1)
            {
                ((DrawObject)this.GraphicsList.SelectObject).AntiClockWiseDirection();
                SetDirty();
                Refresh();
            }
        }

        /// <summary>
        /// ˳ʱ��ת��
        /// </summary>
        public void ClockWise()
        {
            if (this.GraphicsList.SelectionCount == 1)
            {
                ((DrawObject)this.GraphicsList.SelectObject).ClockWiseDirection();
                SetDirty();
                Refresh();
            }
        }

        /// <summary>
        /// �����ı���ɫ
        /// </summary>
        public void setTextcolor()
        {
            if (this.GraphicsList.SelectionCount == 1)
            {
                ((DrawObject)this.GraphicsList.SelectObject).SetTextColor();
                SetDirty();
                Refresh();
            }
        }
        /// <summary>
        /// �����ı������С
        /// </summary>
        public void setTextsize()
        {
            if (this.GraphicsList.SelectionCount == 1)
            {
                ((DrawObject)this.GraphicsList.SelectObject).SetTextSize();
                SetDirty();
                Refresh();
            }
        }

        public void setStationObject()
        {
            SetDrawForm dlg = new SetDrawForm(this.StationID, this.AWidth, this.AHeight, this.StationIP);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.StationID = dlg.StationID;
                this.AWidth = dlg.AWidth;
                this.AHeight = dlg.AHeight;
                this.StationIP = dlg.DeviceIP;
                this.SetDirty();
                Refresh();
            }
        }

        /// <summary>
        /// ��ʾ�豸����
        /// </summary>
        /// <param name="check"></param>
        public void showPreproty(bool check)
        {
            Refresh();
        }

        public void gridlines()
        {
            Refresh();
        }

        /// <summary>
        /// ����ѡ�ж���
        /// </summary>
        public void CopySelectionObject()
        {
            CommandCopy cmdCopy = new CommandCopy(graphicsList);
            cmdCopy.Execute();
        }

        /// <summary>
        /// ���ѡ�ж���
        /// </summary>
        public void PasteSelectionObject()
        {
            CommandPaste cmdPaste = new CommandPaste(graphicsList);
            cmdPaste.SetMousePosition(gPoint.X, gPoint.Y);
            cmdPaste.Execute();
            Capture = true;

            SetDirty();
            Refresh();
        }

        /// <summary>
        /// �Ƿ��������ֵ
        /// </summary>
        /// <param name="_verify"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public bool HasInclude(List<string> _verify, string sKey)
        {
            if (sKey != "")
            {
                if (_verify.Contains(sKey))
                {
                    return true;
                }
                else
                {
                    _verify.Add(sKey);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        ///�ж��Ƿ�������IP��ַ
        /// </summary>
        /// <param name="_verify"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool IPHasInclude(List<string> _verify, string ip)
        {
            if (ip != "")
            {
                if (_verify.Contains(ip))
                {
                    return true;
                }
                else
                {
                    _verify.Add(ip);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// �����豸��������
        /// </summary>
        /// <param name="rate"></param>
        public void SetRate(float rate)
        {
            if (MessageBox.Show("�Ƿ��ƽ�漰����ͼ�ν��б�������", "��ʾ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                this.AWidth = (int)(AWidth * rate);
                this.AHeight = (int)(AHeight * rate);
                //this.AWidth = (int)(850 * rate);  //���ڹ̶��������е���
                //this.AHeight = (int)(560 * rate);
                CommandFullZoom commandFullZoom = new CommandFullZoom(graphicsList);
                commandFullZoom.SetRate(rate);
                commandFullZoom.Execute();
            }
            else
            {
                CommandSingleZoom commandSingleZoom = new CommandSingleZoom(graphicsList);
                commandSingleZoom.SetRate(rate);
                commandSingleZoom.Execute();
            }
            SetDirty();
            Refresh();
        }

        /// <summary>
        /// ��ȡ����λ��XY����
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private int GetDirectionXY(string direction)
        {
            int result = 0;
            int tempValue = 0;
            int realvalue = 0;
            int n = GraphicsList.SelectionCount;
            DrawObject obj = null;
            switch (direction)
            {
                case "Top":
                    realvalue = ((DrawObject)GraphicsList.ASelection[0]).GetRectangle().Y;
                    for (int i = n - 1; i >= 0; i--)
                    {
                        obj = (DrawObject)GraphicsList.ASelection[i];
                        tempValue = obj.GetRectangle().Y;
                        if (tempValue < realvalue) realvalue = tempValue;
                    }
                    result = realvalue;
                    break;
                case "Bottom":
                    realvalue = ((DrawObject)GraphicsList.ASelection[0]).GetRectangle().Y
;
                    for (int i = n - 1; i >= 0; i--)
                    {
                        obj = (DrawObject)GraphicsList.ASelection[i];
                        tempValue = obj.GetRectangle().Y;
                        if (tempValue > realvalue) realvalue = tempValue;
                    }
                    result = realvalue;
                    break;
                case "Left":
                    realvalue = ((DrawObject)GraphicsList.ASelection[0]).GetRectangle().X;
                    for (int i = n - 1; i >= 0; i--)
                    {
                        obj = (DrawObject)GraphicsList.ASelection[i];
                        tempValue = obj.GetRectangle().X;
                        if (tempValue < realvalue) realvalue = tempValue;
                    }
                    result = realvalue;
                    break;
                case "Right":
                    realvalue = ((DrawObject)GraphicsList.ASelection[0]).GetRectangle().X;
                    for (int i = n - 1; i >= 0; i--)
                    {
                        obj = (DrawObject)GraphicsList.ASelection[i];
                        tempValue = obj.GetRectangle().X;
                        if (tempValue > realvalue) realvalue = tempValue;
                    }
                    result = realvalue;
                    break;
            }
            return result;
        }

        /// <summary>
        /// ���ö��뷽ʽ
        /// </summary>
        /// <param name="Direction"></param>
        public void setAlign(string Direction)
        {
            if (this.GraphicsList.SelectionCount < 2)
                return;

            int location = 0;
            //ԭ�����㷨�ǣ������ǰ���һ����x���룬�����ǰ��յ�һ���Ŀ���룬�����ǰ���һ����y���룬�����ǰ���һ���ĸ߶ȶ��롣
            int n = GraphicsList.SelectionCount;
            DrawObject obj = null;
            location = GetDirectionXY(Direction);
            switch (Direction)
            {
                case "Left":
                case "Right":
                    for (int i = n - 1; i >= 0; i--)
                    {
                        obj = (DrawObject)GraphicsList.ASelection[i];
                        obj.setRectangleX(location);
                    }
                    break;
                case "Top":
                case "Bottom":
                    for (int i = n - 1; i >= 0; i--)
                    {
                        obj = (DrawObject)GraphicsList.ASelection[i];
                        obj.setRectangleY(location);
                    }
                    break;
                default:
                    break;
            }
            SetDirty();
            Refresh();
        }

        /// <summary>
        /// ���ô�С
        /// </summary>
        /// <param name="Size"></param>
        public void setSize(string Size)
        {
            if (this.GraphicsList.SelectionCount < 2)
                return;

            int width = 0, height = 0;

            int n = GraphicsList.SelectionCount;
            DrawObject obj = null;
            for (int i = n - 1; i >= 0; i--)
            {
                obj = (DrawObject)GraphicsList.ASelection[i];
                if (i == n - 1)
                {
                    width = obj.GetRectangle().Width;
                    height = obj.GetRectangle().Height;
                }
                else
                {
                    if (Size == "Width")
                        obj.setRectangleWidth(width);
                    else if (Size == "Height")
                        obj.setRectangleHeight(height);
                    else if (Size == "Both")
                    {
                        obj.setRectangleWidth(width);
                        obj.setRectangleHeight(height);
                    }
                }
            }
            SetDirty();
            Refresh();
        }

        private void DrawArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.GraphicsList.SelectionCount == 0)
                return;

            bool left = false, right = false, up = false, down = false;
            if (e.KeyCode == Keys.Left)
                left = true;
            else if (e.KeyCode == Keys.Right)
                right = true;
            else if (e.KeyCode == Keys.Up)
                up = true;
            else if (e.KeyCode == Keys.Down)
                down = true;

            int n = GraphicsList.SelectionCount;
            DrawObject obj = null;
            for (int i = n - 1; i >= 0; i--)
            {
                obj = (DrawObject)GraphicsList.ASelection[i];
                if (left)
                    obj.setRectangleX(obj.GetRectangle().X - 1);
                else if (right)
                    obj.setRectangleX(obj.GetRectangle().X + 1);
                else if (up)
                    obj.setRectangleY(obj.GetRectangle().Y - 1);
                else if (down)
                    obj.setRectangleY(obj.GetRectangle().Y + 1);
            }
            SetDirty();
            Refresh();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (this.Focused)
            {
                Keys keys1 = (keyData & Keys.KeyCode);
                switch (keys1)
                {
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Left:
                    case Keys.Right:
                        {
                            return true;
                        }
                }
            }
            return base.IsInputKey(keyData);
        }

        /// <summary>
        /// Right-click handler
        /// </summary>
        /// <param name="e"></param>
        private void OnContextMenu(MouseEventArgs e)
        {
            Point point = new Point(e.X, e.Y);
            int n = GraphicsList.Count;
            DrawObject o = null;
            for (int i = 0; i < n; i++)
            {
                if (GraphicsList[i].HitTest(gPoint) == 0)
                {
                    o = GraphicsList[i];
                    break;
                }
            }
            if (o != null)
            {
                if (!o.Selected)
                    GraphicsList.UnselectAll();

                o.Selected = true;
            }
            else
            {
                GraphicsList.UnselectAll();
            }

            Refresh();
            m_ContextMenu = new ContextMenuStrip();
            int nItems = owner.ContextParent.DropDownItems.Count;
            for (int i = nItems - 1; i >= 0; i--)
            {
                m_ContextMenu.Items.Insert(0, owner.ContextParent.DropDownItems[i]);
            }
            point.X += this.Left;
            point.Y += this.Top;
            m_ContextMenu.Show(owner, point);
            Owner.mainform.SetStateOfControls();
            m_ContextMenu.Closed += delegate(object sender, ToolStripDropDownClosedEventArgs args)
            {
                if (m_ContextMenu != null)      // precaution
                {
                    nItems = m_ContextMenu.Items.Count;

                    for (int k = nItems - 1; k >= 0; k--)
                    {
                        owner.ContextParent.DropDownItems.Insert(0, m_ContextMenu.Items[k]);
                    }
                }
            };
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Draw graphic objects and 
        /// group selection rectangle (optionally)
        /// </summary>
        private void DrawArea_Paint(object sender, PaintEventArgs e)
        {

            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            SolidBrush mySolidBrush = new SolidBrush(System.Drawing.Color.White);
            Pen myPen = new Pen(Color.Blue, 1);
            Pen keyPen = new Pen(Color.Black, 1);

            g.FillRectangle(mySolidBrush, 0, 0, width, height);

            myPen.DashStyle = DashStyle.Dash;
            int horizontalCnt = (width / blockWidth);
            int verticalCnt = (height / blockWidth);

            bool bgrid = true;
            if (bgrid)
            {
                //������
                for (int i = 1; i <= horizontalCnt; i++)
                {
                    System.Drawing.Point p1 = new System.Drawing.Point(i * blockWidth, 0);
                    System.Drawing.Point p2 = new System.Drawing.Point(i * blockWidth, height);

                    g.DrawLine(myPen, p1, p2);
                }

                //������
                for (int i = 1; i <= verticalCnt; i++)
                {
                    System.Drawing.Point p1 = new System.Drawing.Point(0, i * blockWidth);
                    System.Drawing.Point p2 = new System.Drawing.Point(width, i * blockWidth);

                    g.DrawLine(myPen, p1, p2);
                }
            }
            e.Graphics.DrawImage(bmp, 0, 0, width, height);
            if (bgrid)
                e.Graphics.DrawRectangle(keyPen, 0, 0, width, height);

            if ((graphicsList != null) && (graphicsList.Count > 0))
            {
                graphicsList.showproperty(false); //����ʾ����
                graphicsList.Draw(e.Graphics);
            }
            mySolidBrush.Dispose();
            myPen.Dispose();
        }

        /// <summary>
        /// Mouse down.
        /// Left button down event is passed to active tool.
        /// Right button down event is handled in this class.
        /// </summary>
        private void DrawArea_MouseDown(object sender, MouseEventArgs e)
        {
            gPoint = new Point(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
                tools[(int)activeTool].OnMouseDown(this, e);
            else if (e.Button == MouseButtons.Right)
                OnContextMenu(e);
        }

        /// <summary>
        /// Mouse move.
        /// Moving without button pressed or with left button pressed
        /// is passed to active tool.
        /// </summary>
        private void DrawArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.None)
                tools[(int)activeTool].OnMouseMove(this, e);
            else
                this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Mouse up event.
        /// Left button up event is passed to active tool.
        /// </summary>
        private void DrawArea_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                tools[(int)activeTool].OnMouseUp(this, e);
        }

        private void DrawArea_DoubleClick(object sender, EventArgs e)
        {
            if (this.GraphicsList.SelectionCount == 1)
            {
                ((DrawObject)this.GraphicsList.SelectObject).OnDoubleClick(sender, e);
            }
            else if (this.GraphicsList.SelectionCount == 0)
            {
                setStationObject();
            }
        }

        public void GetDrawArea_DoubleClick()
        {
            DrawArea_DoubleClick(this, null);
        }

        #endregion Event Handlers

        #region  ������������/���豸��ͼ

        /// <summary>
        /// ��վ��ͼ����(device-�豸,switch-������)
        /// </summary>
        public string mapType = "";

        /// <summary>
        /// �������޸ĵĳ�վ��ͼ����
        /// </summary>
        /// <returns></returns>
        public override bool SaveData(string mapType)
        {
            List<StationMapModel> modelList = new List<StationMapModel>();
            this.mapType = mapType;
            int n = GraphicsList.Count;
            DrawObject o = null;
            string deviceId = string.Empty;
            string deviceName = string.Empty;
            bool saveSu = false;
            //������X����
            int paid_X = 0;
            //������Y����
            int paid_Y = 0;
            // �������߶�
            int paid_Height = 0;
            //���������
            int paid_Width = 0;

            //�豸���ĵ�X����
            int deviceCenterPointX = 0;

            //�豸���ĵ�Y����
            int deviceCenterPointY = 0;

            if (this.StationID != null)
            {
                //Ϊȷ��˫��AGM�ڸ����������λ�ã������ȵõ�������X��Y�͸߶ȺͿ��
                for (int i = n - 1; i >= 0; i--)
                {
                    o = graphicsList[i];
                    string name = o.GetType().Name;
                    if (name == "PaidArea")
                    {
                        var paidObj = o as PaidArea;
                        paid_X = paidObj.RectangleLs.X;
                        paid_Y = paidObj.RectangleLs.Y;
                        paid_Height = paidObj.RectangleLs.Height;
                        paid_Width = paidObj.RectangleLs.Width;
                    }
                }

                for (int i = n - 1; i >= 0; i--)
                {
                    o = graphicsList[i];
                    string name = o.GetType().Name;
                    if (name == "BOM")
                    {
                        var bomObj = o as BOM;
                        if (bomObj.DeviceID != null)
                        {
                            var model = new StationMapModel()
                            {
                                DeviceID = bomObj.DeviceID.Length == 2 ? (this.StationID + "03" + bomObj.DeviceID) : bomObj.DeviceID,
                                XPos = bomObj.RectangleLs.X.ToString(),
                                YPos = bomObj.RectangleLs.Y.ToString(),
                                DeviceName = bomObj.DeviceName,
                                DeviceType = "03",
                                DeviceSubType = "0",
                                Angle = "0",
                                IpAdd = bomObj.IpAdd,
                                Device_H = bomObj.RectangleLs.Height.ToString(),
                                Device_W = bomObj.RectangleLs.Width.ToString(),
                                StationID = this.StationID,
                                LableId = bomObj.TagIDBase.ToString(),
                                TextFontSize = "0",
                                MapType = mapType
                            };
                            modelList.Add(model);
                        }
                    }
                    else if (name == "AGMChannelDual")
                    {
                        var agmDulObj = o as AGMChannelDual;
                        deviceCenterPointX = agmDulObj.RectangleLs.X + agmDulObj.RectangleLs.Width / 2;
                        deviceCenterPointY = agmDulObj.RectangleLs.Y + agmDulObj.RectangleLs.Height / 2;

                        if (agmDulObj.DeviceID != null)
                        {
                            var model = new StationMapModel()
                            {
                                DeviceID = agmDulObj.DeviceID.Length == 2 ? (this.StationID + "43" + agmDulObj.DeviceID) : agmDulObj.DeviceID,
                                XPos = agmDulObj.RectangleLs.X.ToString(),
                                YPos = agmDulObj.RectangleLs.Y.ToString(),
                                DeviceName = agmDulObj.DeviceName,
                                DeviceType = "04",
                                DeviceSubType = "03", //˫��
                                IpAdd = agmDulObj.IpAdd,
                                Device_H = agmDulObj.RectangleLs.Height.ToString(),
                                Device_W = agmDulObj.RectangleLs.Width.ToString(),
                                StationID = this.StationID,
                                Angle = agmDulObj.Direction == HVDirection.Horizontal ? "1" : "2",
                                LableId = agmDulObj.TagIDBase.ToString(),
                                TextFontSize = "0",
                                MapType = mapType,

                                Entry = AGMChannelEntryHelp.ConvertAgmChannelDualDirection(paid_X, paid_Y, paid_Height, paid_Width, deviceCenterPointX, deviceCenterPointY)
                            };
                            modelList.Add(model);
                        }
                    }
                    else if (name == "AGMChannel")
                    {
                        var agmChObj = o as AGMChannel;
                        if (agmChObj.DeviceID != null)
                        {
                            var model = new StationMapModel()
                            {
                                DeviceID = agmChObj.DeviceID.Length == 2 ? (this.StationID + "04" + agmChObj.DeviceID) : agmChObj.DeviceID,
                                XPos = agmChObj.RectangleLs.X.ToString(),
                                YPos = agmChObj.RectangleLs.Y.ToString(),
                                DeviceName = agmChObj.DeviceName,
                                DeviceType = "04",
                                DeviceSubType = "01", //����
                                IpAdd = agmChObj.IpAdd,
                                Device_H = agmChObj.RectangleLs.Height.ToString(),
                                Device_W = agmChObj.RectangleLs.Width.ToString(),
                                StationID = this.StationID,
                                Entry = agmChObj.Direction.ToString().Trim(),
                                LableId = agmChObj.TagIDBase.ToString(),
                                TextFontSize = "0",
                                MapType = mapType
                            };
                            modelList.Add(model);
                        }
                    }
                    else if (name == "TVM")
                    {
                        var tvmObj = o as TVM;
                        if (tvmObj.DeviceID != null)
                        {
                            var model = new StationMapModel()
                            {
                                DeviceID = tvmObj.DeviceID.Length == 2 ? (this.StationID + "02" + tvmObj.DeviceID) : tvmObj.DeviceID,
                                XPos = tvmObj.RectangleLs.X.ToString(),
                                YPos = tvmObj.RectangleLs.Y.ToString(),
                                DeviceName = tvmObj.DeviceName,
                                DeviceType = "02",
                                DeviceSubType = "0",
                                Angle = "0",
                                IpAdd = tvmObj.IpAdd,
                                Device_H = tvmObj.RectangleLs.Height.ToString(),
                                Device_W = tvmObj.RectangleLs.Width.ToString(),
                                StationID = this.StationID,
                                LableId = tvmObj.TagIDBase.ToString(),
                                TextFontSize = "0",
                                MapType = mapType
                            };
                            modelList.Add(model);
                        }
                    }
                    else if (name == "TCM")
                    {
                        var tcmObj = o as TCM;
                        if (tcmObj.DeviceID != null)
                        {
                            var model = new StationMapModel()
                            {
                                DeviceID = tcmObj.DeviceID.Length == 2 ? (this.StationID + "04" + tcmObj.DeviceID) : tcmObj.DeviceID,
                                XPos = tcmObj.RectangleLs.X.ToString(),
                                YPos = tcmObj.RectangleLs.Y.ToString(),
                                DeviceName = tcmObj.DeviceName,
                                DeviceType = "04",
                                DeviceSubType = "0",
                                Angle = "0",
                                IpAdd = tcmObj.IpAdd,
                                Device_H = tcmObj.RectangleLs.Height.ToString(),
                                Device_W = tcmObj.RectangleLs.Width.ToString(),
                                StationID = this.StationID,
                                LableId = tcmObj.TagIDBase.ToString(),
                                TextFontSize = "0",
                                MapType = mapType
                            };
                            modelList.Add(model);
                        }
                    }
                    else if (name == "AGMWallSingle")
                    {
                        var agmWallSingleChObj = o as AGMWallSingle;
                        var model = new StationMapModel()
                        {
                            DeviceID = (agmWallSingleChObj.RectangleLs.X + agmWallSingleChObj.RectangleLs.Y).ToString().Replace('-', ' ').Trim().ToString(), //�����豸�ڷǻ滭�����л��ƣ�XY������ָ���,���յ����豸���Ϊ����
                            XPos = agmWallSingleChObj.RectangleLs.X.ToString(),
                            YPos = agmWallSingleChObj.RectangleLs.Y.ToString(),
                            DeviceName = "AGMWallSingle", 
                            DeviceType = "85",//�Զ������ͱ��
                            DeviceSubType = "0",
                            Angle = "0",
                            StationID = this.StationID,
                            Device_H = agmWallSingleChObj.RectangleLs.Height.ToString(),
                            Device_W = agmWallSingleChObj.RectangleLs.Width.ToString(),
                            LableId = "0",
                            TextFontSize = "0",
                            MapType = mapType,
                            Entry = agmWallSingleChObj.Direction.ToString().Trim()
                        };
                        modelList.Add(model);
                    }
                    else if (name == "AGMWallDual")
                    {
                        var agmWallDualChObj = o as AGMWallDual;
                        var model = new StationMapModel()
                        {
                            DeviceID = (agmWallDualChObj.RectangleLs.X + agmWallDualChObj.RectangleLs.Y).ToString().Replace('-', ' ').Trim().ToString(),//�����豸�ڷǻ滭�����л��ƣ�XY������ָ���,���յ����豸���Ϊ����
                            XPos = agmWallDualChObj.RectangleLs.X.ToString(),
                            YPos = agmWallDualChObj.RectangleLs.Y.ToString(),
                            DeviceName = "AGMWallDual",
                            DeviceType = "86",//�Զ������ͱ��
                            DeviceSubType = "0",
                            Angle = "0",
                            StationID = this.StationID,
                            Device_H = agmWallDualChObj.RectangleLs.Height.ToString(),
                            Device_W = agmWallDualChObj.RectangleLs.Width.ToString(),
                            LableId = "0",
                            TextFontSize = "0",
                            MapType = mapType,
                            Entry = agmWallDualChObj.Direction.ToString().Trim()
                        };
                        modelList.Add(model);
                    }
                    else if (name == "AGMWallDummy")
                    {
                        var agmWallDummyChObj = o as AGMWallDummy;
                        var model = new StationMapModel()
                        {
                            DeviceID = (agmWallDummyChObj.RectangleLs.X + agmWallDummyChObj.RectangleLs.Y).ToString().Replace('-', ' ').Trim().ToString(),//�����豸�ڷǻ滭�����л��ƣ�XY������ָ���,���յ����豸���Ϊ����
                            XPos = agmWallDummyChObj.RectangleLs.X.ToString(),
                            YPos = agmWallDummyChObj.RectangleLs.Y.ToString(),
                            DeviceName = "AGMWallDummy",
                            DeviceType = "87",//�Զ������ͱ��
                            DeviceSubType = "0",
                            Angle = "0",
                            StationID = this.StationID,
                            Device_H = agmWallDummyChObj.RectangleLs.Height.ToString(),
                            Device_W = agmWallDummyChObj.RectangleLs.Width.ToString(),
                            LableId = "0",
                            TextFontSize = "0",
                            MapType = mapType,
                            Entry = agmWallDummyChObj.Direction.ToString().Trim()
                        };
                        modelList.Add(model);
                    }
                    else if (name == "PaidArea")
                    {
                        var paidObj = o as PaidArea;
                        var model = new StationMapModel()
                        {
                            DeviceID = (paidObj.RectangleLs.X + paidObj.RectangleLs.Y).ToString().Replace('-',' ').Trim().ToString(),//�����豸�ڷǻ滭�����л��ƣ�XY������ָ���,���յ����豸���Ϊ����
                            XPos = paidObj.RectangleLs.X.ToString(),
                            YPos = paidObj.RectangleLs.Y.ToString(),
                            DeviceName = "PaidArea",
                            DeviceType = "88",//�Զ������ͱ��
                            DeviceSubType = "0",
                            Angle = "0",
                            StationID = this.StationID,
                            Device_H = paidObj.RectangleLs.Height.ToString(),
                            Device_W = paidObj.RectangleLs.Width.ToString(),
                            LableId = "0",
                            TextFontSize = "0",
                            MapType = mapType
                        };
                        modelList.Add(model);
                    }
                    else if (name == "Array")
                    {
                        var arrayObj = o as DrawTools.Device.Array;
                        var model = new StationMapModel()
                        {
                            DeviceID = (arrayObj.RectangleLs.X + arrayObj.RectangleLs.Y).ToString().Replace('-', ' ').Trim().ToString(),//�����豸�ڷǻ滭�����л��ƣ�XY������ָ���,���յ����豸���Ϊ����
                            XPos = arrayObj.RectangleLs.X.ToString(),
                            YPos = arrayObj.RectangleLs.Y.ToString(),
                            DeviceName = "Array",
                            DeviceType = "89",//�Զ������ͱ��
                            DeviceSubType = "0",
                            Angle = "0",
                            StationID = this.StationID,
                            Device_H = arrayObj.RectangleLs.Height.ToString(),
                            Device_W = arrayObj.RectangleLs.Width.ToString(),
                            LableId = "0",
                            TextFontSize = "0",
                            MapType = mapType
                        };
                        modelList.Add(model);
                    }
                    else if (name == "Text")
                    {
                        var textObj = o as Text;
                        var textType = textObj.TextType;
                        var model = new StationMapModel()
                        {
                            DeviceID = textObj.ObjectID.ToString(),
                            XPos = textObj.RectangleLs.X.ToString(),
                            YPos = textObj.RectangleLs.Y.ToString(),
                            DeviceName = textObj.Texttest,
                            DeviceType = "90",//�Զ������ͱ��
                            DeviceSubType = "0",
                            Angle = "0",
                            StationID = this.StationID,
                            Device_H = textObj.RectangleLs.Height.ToString(),
                            Device_W = textObj.RectangleLs.Width.ToString(),
                            TextFontSize = textObj.TextFont.Size.ToString(),
                            TextFonStyle = textObj.TextFont.Style.ToString(),
                            TextColor = textObj.FontColor.Name.ToString(),
                            TextType = textObj.TextType.ToString(),
                            LableId = "0",
                            MapType = mapType
                        };
                        modelList.Add(model);
                    }
                }
                //���Ĭ��SC������
                var SCModel = new StationMapModel()
                {
                    DeviceID = string.Format("{0}{1}{2}", this.stationID, "01", "01"),
                    DeviceType = "01",
                    DeviceSubType = "0",
                    StationID = this.StationID,
                    IpAdd = this.stationIP,
                    MapType = mapType,
                    Angle = "0",

                    XPos = "0",
                    YPos = "0",
                    DeviceName = "0",
                    Device_H = "0",
                    Device_W = "0",
                    LableId = "0",
                    TextFontSize = "0"
                };

                modelList.Add(SCModel);
                DBDeviceService.dbDevice.SaveStationDevice(modelList);
                //LogHelper.DeviceDeviceLogInfo(string.Format("���泵վ:{0}�豸���õ�ͼ���ݳɹ�!", this.StationID));
                MessageBox.Show("���泵վ�豸���ݳɹ�", "��ʾ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("����˫����������ӳ�վ���!");
            }
            return saveSu;
        }

        /// <summary>
        /// ���豸����
        /// </summary>
        /// <param name="modelList"></param>
        /// <param name="mapType"></param>
        /// <returns></returns>
        public override bool OpenData(List<StationMapModel> modelList, string mapType)
        {
            int id = 0, objectid = 0;
            try
            {
                foreach (StationMapModel node in modelList)
                {
                    this.StationID = node.StationID;
                    this.AWidth = int.Parse(node.Region_W);
                    this.AHeight = int.Parse(node.Region_H);
                    this.mapType = mapType;
                    DrawObject drawobject = DataToControl(node, out  objectid);
                    graphicsList.SetDictionary(node);
                    if (drawobject != null)
                    {
                        if (objectid > id)
                        {
                            id = objectid;
                        }
                        this.graphicsList.Add(drawobject);
                    }
                    DrawObject.objIdInc = id + 1;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            Refresh();
            SetMapTypeFlag();
            return true;
        }

        /// <summary>
        /// ��ȡ�豸���������豸�ؼ�
        /// </summary>
        /// <param name="node"></param>
        /// <param name="objectid"></param>
        /// <returns></returns>
        public DrawObject DataToControl(StationMapModel node, out int objectid)
        {
            DrawObject device = null;
            int left = 0, top = 0, width = 0, height = 0;
            objectid = 0;
            if (node.DeviceType == "03")  //02 bom
            {
                BOM obj = new BOM();
                obj.IpAdd = node.IpAdd;
                obj.DeviceID = node.DeviceID;
                //��������
                obj.DeviceName = node.DeviceName;
                obj.DeviceType = node.DeviceType;
                obj.DeviceSubType = node.DeviceSubType;
                obj.DeviceSeqInStation = node.DeviceSeqInStation;
                obj.LobbyId = node.LobbyId;
                obj.GroupID = node.GroupID;
                obj.DeviceSeqInGroup = node.DeviceSeqInGroup;
                obj.XPos = node.XPos;
                obj.YPos = node.YPos;
                obj.RecID = node.RecID;
                obj.StationID = node.StationID;
                obj.LogicIDTail = node.DeviceID;
                obj.TagIDBase = int.Parse(node.LableId); //���ı�����
                left = int.Parse(node.XPos);
                top = int.Parse(node.YPos);
                width = int.Parse(node.Device_W);
                height = int.Parse(node.Device_H);
                obj.DeviceIP = node.IpAdd;
                obj.RectangleLs = new Rectangle(left, top, width, height);
                if (obj.TagIDBase != 0)
                    objectid = obj.TagIDBase;
                else
                    objectid = obj.ObjectID;
                device = obj;
            }
            else if (node.DeviceType == "01") //11 server
            {
                SC obj = new SC();
                this.stationIP = node.IpAdd;
                obj.LogicIDTail = node.DeviceID;
                obj.IpAdd = node.IpAdd;
                obj.DeviceID = node.DeviceID;
                obj.DeviceName = node.DeviceName;
                obj.DeviceType = node.DeviceType;
                obj.DeviceSubType = node.DeviceSubType;
                obj.DeviceSeqInStation = node.DeviceSeqInStation;
                obj.LobbyId = node.LobbyId;
                obj.GroupID = node.GroupID;
                obj.DeviceSeqInGroup = node.DeviceSeqInGroup;
                obj.XPos = node.XPos;
                obj.YPos = node.YPos;
                obj.RecID = node.RecID;
                obj.StationID = node.StationID;
                obj.TagIDBase = int.Parse(node.LableId); //���ı�����
                left = int.Parse(node.XPos);
                top = int.Parse(node.YPos);
                width = int.Parse(node.Device_W);
                height = int.Parse(node.Device_H);
                obj.RectangleLs = new Rectangle(left, top, width, height);
                obj.DeviceIP = node.IpAdd;
                objectid = obj.ObjectID;
                device = obj;
            }
            else if (node.DeviceType == "02")  //01 tvm
            {
                TVM obj = new TVM();
                obj.LogicIDTail = node.DeviceID;
                obj.IpAdd = node.IpAdd;
                obj.DeviceID = node.DeviceID;
                obj.DeviceName = node.DeviceName;
                obj.DeviceType = node.DeviceType;
                obj.DeviceSubType = node.DeviceSubType;
                obj.DeviceSeqInStation = node.DeviceSeqInStation;
                obj.LobbyId = node.LobbyId;
                obj.GroupID = node.GroupID;
                obj.DeviceSeqInGroup = node.DeviceSeqInGroup;
                obj.XPos = node.XPos;
                obj.YPos = node.YPos;
                obj.RecID = node.RecID;
                obj.StationID = node.StationID;

                obj.TagIDBase = int.Parse(node.LableId);
                //obj.Flag = int.Parse(node.LableId);  //���ı�����

                left = int.Parse(node.XPos);
                top = int.Parse(node.YPos);
                width = int.Parse(node.Device_W);
                height = int.Parse(node.Device_H);
                obj.DeviceIP = node.IpAdd;
                obj.RectangleLs = new Rectangle(left, top, width, height);
                if (obj.TagIDBase != 0)
                    objectid = obj.TagIDBase;
                else
                    objectid = obj.ObjectID;
                device = obj;
            }
            //else if (node.DeviceType == "04")  //04TCM
            //{
            //    TCM obj = new TCM();
            //    obj.LogicIDTail = node.DeviceID;

            //    obj.TagIDBase = int.Parse(node.LableId);
            //    //obj.Flag = int.Parse(node.LableId);  //���ı�����
            //    obj.IpAdd = node.IpAdd;
            //    obj.DeviceID = node.DeviceID;
            //    obj.DeviceName = node.DeviceName;
            //    obj.DeviceType = node.DeviceType;
            //    obj.DeviceSubType = node.DeviceSubType;
            //    obj.DeviceSeqInStation = node.DeviceSeqInStation;
            //    obj.LobbyId = node.LobbyId;
            //    obj.GroupID = node.GroupID;
            //    obj.DeviceSeqInGroup = node.DeviceSeqInGroup;
            //    obj.XPos = node.XPos;
            //    obj.YPos = node.YPos;
            //    obj.RecID = node.RecID;
            //    obj.StationID = node.StationID;
            //    left = int.Parse(node.XPos);
            //    top = int.Parse(node.YPos);
            //    width = int.Parse(node.Device_W);
            //    height = int.Parse(node.Device_H);
            //    obj.DeviceIP = node.IpAdd;
            //    obj.RectangleLs = new Rectangle(left, top, width, height);
            //    if (obj.TagIDBase != 0)
            //        objectid = obj.TagIDBase;
            //    else
            //        objectid = obj.ObjectID;
            //    device = obj;
            //}
            //TODO:ȡ��������
            //if (node.DeviceType == "12")  //12 switch
            //{
            //    Switch obj = new Switch();
            //    obj.SwitchID = node.DeviceID;
            //    //obj.IpAdd = node.IpAdd;
            //    obj.DeviceID = node.DeviceID;
            //    obj.DeviceName = node.DeviceName;
            //    obj.DeviceType = node.DeviceType;
            //    obj.DeviceSubType = node.DeviceSubType;
            //    obj.DeviceSeqInStation = node.DeviceSeqInStation;
            //    obj.LobbyId = node.LobbyId;
            //    obj.GroupID = node.GroupID;
            //    obj.DeviceSeqInGroup = node.DeviceSeqInGroup;
            //    obj.XPos = node.XPos;
            //    obj.YPos = node.YPos;
            //    obj.RecID = node.RecID;
            //    obj.StationID = node.StationID;

            //    obj.TagIDBase = int.Parse(node.LableId);
            //    // obj.Flag = int.Parse(node.LableId);  //���ı�����
            //    obj.LogicIDTail = node.DeviceID;
            //    left = int.Parse(node.XPos);
            //    top = int.Parse(node.YPos);
            //    width = int.Parse(node.Device_W);
            //    height = int.Parse(node.Device_H);
            //    obj.RectangleLs = new Rectangle(left, top, width, height);
            //    objectid = objectid + 1;
            //    device = obj;
            //}
            //else if (node.DeviceType == "13") //sc_network_device_port 13
            //{
            //    SwitchPort obj = new SwitchPort();
            //    obj.DeviceID = node.DeviceID;
            //    obj.TagIDBase = int.Parse(node.LableId);
            //    // obj.Flag = int.Parse(node.LableId);  //���ı�����
            //    obj.LogicIDTail = node.DeviceID;
            //    //obj.IpAdd = node.IpAdd;
            //    obj.DeviceID = node.DeviceID;
            //    obj.DeviceName = node.DeviceName;
            //    obj.DeviceType = node.DeviceType;
            //    obj.DeviceSubType = node.DeviceSubType;
            //    obj.DeviceSeqInStation = node.DeviceSeqInStation;
            //    obj.LobbyId = node.LobbyId;
            //    obj.GroupID = node.GroupID;
            //    obj.DeviceSeqInGroup = node.DeviceSeqInGroup;
            //    obj.XPos = node.XPos;
            //    obj.YPos = node.YPos;
            //    obj.RecID = node.RecID;
            //    obj.StationID = node.StationID;
            //    left = int.Parse(node.XPos);
            //    top = int.Parse(node.YPos);
            //    width = int.Parse(node.Device_W);
            //    height = int.Parse(node.Device_H);
            //    obj.RectangleLs = new Rectangle(left, top, width, height);
            //    objectid = objectid + 1;
            //    device = obj;
            //}
            else if (node.DeviceType == "04")  //06AGM 
            {
                if (node.DeviceSubType == "02" || node.DeviceSubType == "01")  //��վ���վ����բ��
                {
                    var obj = new AGMChannel();
                    obj.LogicIDTail = node.DeviceID;
                    obj.IpAdd = node.IpAdd;
                    obj.DeviceID = node.DeviceID;
                    obj.DeviceName = node.DeviceName;
                    obj.DeviceType = node.DeviceType;
                    obj.DeviceSubType = node.DeviceSubType;
                    obj.DeviceSeqInStation = node.DeviceSeqInStation;
                    obj.LobbyId = node.LobbyId;
                    obj.GroupID = node.GroupID;
                    obj.DeviceSeqInGroup = node.DeviceSeqInGroup;
                    obj.XPos = node.XPos;
                    obj.YPos = node.YPos;
                    obj.RecID = node.RecID;
                    obj.StationID = node.StationID;
                    //obj.Angle = node.Entry;  
                    obj.Entry = node.Entry;
                    obj.TagIDBase = int.Parse(node.LableId);
                    // obj.Flag = int.Parse(node.LableId);  //���ı�����
                    left = int.Parse(node.XPos);
                    top = int.Parse(node.YPos);
                    width = int.Parse(node.Device_W);
                    height = int.Parse(node.Device_H);
                    obj.DeviceIP = node.IpAdd;
                    obj.RectangleLs = new Rectangle(left, top, width, height);
                    if (obj.TagIDBase != 0)
                        objectid = obj.TagIDBase;
                    else
                        objectid = obj.ObjectID;
                    device = obj;
                }
                else if (node.DeviceSubType == "03") //˫��բ��
                {
                    var obj = new AGMChannelDual();
                    obj.LogicIDTail = node.DeviceID;
                    obj.IpAdd = node.IpAdd;
                    obj.DeviceID = node.DeviceID;
                    obj.DeviceName = node.DeviceName;
                    obj.DeviceType = node.DeviceType;
                    obj.DeviceSubType = node.DeviceSubType;
                    obj.DeviceSeqInStation = node.DeviceSeqInStation;
                    obj.LobbyId = node.LobbyId;
                    obj.GroupID = node.GroupID;
                    obj.DeviceSeqInGroup = node.DeviceSeqInGroup;
                    obj.XPos = node.XPos;
                    obj.YPos = node.YPos;
                    obj.RecID = node.RecID;
                    obj.StationID = node.StationID;

                    obj.TagIDBase = int.Parse(node.LableId);
                    //obj.Flag = int.Parse(node.LableId);  //���ı�����
                    obj.Angle = node.Angle == "1" ? "Horizontal" : "Vertical";
                    left = int.Parse(node.XPos);
                    top = int.Parse(node.YPos);
                    width = int.Parse(node.Device_W);
                    height = int.Parse(node.Device_H);
                    obj.DeviceIP = node.IpAdd;
                    obj.RectangleLs = new Rectangle(left, top, width, height);
                    if (obj.TagIDBase != 0)
                        objectid = obj.TagIDBase;
                    else
                        objectid = obj.ObjectID;
                    device = obj;
                }
            }

            else if (node.DeviceType == "85")
            {
                // AGMWallSingle 
                var obj = new AGMWallSingle();
                // obj.ID = node.DeviceSubType;
                obj.LogicIDTail = node.DeviceID;
                obj.IpAdd = node.IpAdd;
                obj.DeviceID = node.DeviceID;
                obj.DeviceName = node.DeviceName;
                obj.DeviceType = node.DeviceType;
                obj.DeviceSubType = node.DeviceSubType;
                obj.DeviceSeqInStation = node.DeviceSeqInStation;
                obj.LobbyId = node.LobbyId;
                obj.GroupID = node.GroupID;
                obj.DeviceSeqInGroup = node.DeviceSeqInGroup;
                obj.XPos = node.XPos;
                obj.YPos = node.YPos;
                obj.RecID = node.RecID;
                obj.StationID = node.StationID;

                obj.TagIDBase = int.Parse(node.LableId);
                //obj.Flag = int.Parse(node.LableId);  //���ı�����
                obj.Entry = node.Entry;
                left = int.Parse(node.XPos);
                top = int.Parse(node.YPos);
                width = int.Parse(node.Device_W);
                height = int.Parse(node.Device_H);
                obj.RectangleLs = new Rectangle(left, top, width, height);
                if (obj.TagIDBase != 0)
                    objectid = obj.TagIDBase;
                else
                    objectid = obj.ObjectID;
                device = obj;
            }
            else if (node.DeviceType == "86")
            {
                //AGMWallDual  
                var obj = new AGMWallDual();
                // obj.ID = node.DeviceSubType;
                obj.LogicIDTail = node.DeviceID;
                obj.IpAdd = node.IpAdd;
                obj.DeviceID = node.DeviceID;
                obj.DeviceName = node.DeviceName;
                obj.DeviceType = node.DeviceType;
                obj.DeviceSubType = node.DeviceSubType;
                obj.DeviceSeqInStation = node.DeviceSeqInStation;
                obj.LobbyId = node.LobbyId;
                obj.GroupID = node.GroupID;
                obj.DeviceSeqInGroup = node.DeviceSeqInGroup;
                obj.XPos = node.XPos;
                obj.YPos = node.YPos;
                obj.RecID = node.RecID;
                obj.StationID = node.StationID;

                obj.TagIDBase = int.Parse(node.LableId);
                //obj.Flag = int.Parse(node.LableId);  //���ı�����
                obj.Entry = node.Entry;
                left = int.Parse(node.XPos);
                top = int.Parse(node.YPos);
                width = int.Parse(node.Device_W);
                height = int.Parse(node.Device_H);
                obj.RectangleLs = new Rectangle(left, top, width, height);
                if (obj.TagIDBase != 0)
                    objectid = obj.TagIDBase;
                else
                    objectid = obj.ObjectID;
                device = obj;
            }
            else if (node.DeviceType == "87")
            {
                //AGMWallDummy  
                var obj = new AGMWallDummy();
                //obj.ID = node.DeviceSubType;
                obj.LogicIDTail = node.DeviceID;
                obj.IpAdd = node.IpAdd;
                obj.DeviceID = node.DeviceID;
                obj.DeviceName = node.DeviceName;
                obj.DeviceType = node.DeviceType;
                obj.DeviceSubType = node.DeviceSubType;
                obj.DeviceSeqInStation = node.DeviceSeqInStation;
                obj.LobbyId = node.LobbyId;
                obj.GroupID = node.GroupID;
                obj.DeviceSeqInGroup = node.DeviceSeqInGroup;
                obj.XPos = node.XPos;
                obj.YPos = node.YPos;
                obj.RecID = node.RecID;
                obj.StationID = node.StationID;
                obj.Entry = node.Entry;
                obj.TagIDBase = int.Parse(node.LableId);
                // obj.Flag = int.Parse(node.LableId);  //���ı�����
                left = int.Parse(node.XPos);
                top = int.Parse(node.YPos);
                width = int.Parse(node.Device_W);
                height = int.Parse(node.Device_H);
                obj.RectangleLs = new Rectangle(left, top, width, height);
                if (obj.TagIDBase != 0)
                    objectid = obj.TagIDBase;
                else
                    objectid = obj.ObjectID;
                device = obj;
            }
            else if (node.DeviceType == "88")
            {
                //PaidArea  
                var obj = new PaidArea();
                obj.LogicIDTail = node.DeviceID;
                obj.TagIDBase = int.Parse(node.LableId);
                // obj.Flag = int.Parse(node.LableId);  //���ı�����
                left = int.Parse(node.XPos);
                top = int.Parse(node.YPos);
                width = int.Parse(node.Device_W);
                height = int.Parse(node.Device_H);
                obj.RectangleLs = new Rectangle(left, top, width, height);
                if (obj.TagIDBase != 0)
                    objectid = obj.TagIDBase;
                else
                    objectid = obj.ObjectID;
                device = obj;
            }
            else if (node.DeviceType == "89")
            {
                //Array 
                var obj = new DrawTools.Device.Array();
                obj.LogicIDTail = node.DeviceID;
                obj.TagIDBase = int.Parse(node.LableId);
                // obj.Flag = int.Parse(node.LableId);  //���ı�����
                left = int.Parse(node.XPos);
                top = int.Parse(node.YPos);
                width = int.Parse(node.Device_W);
                height = int.Parse(node.Device_H);
                obj.RectangleLs = new Rectangle(left, top, width, height);
                if (obj.TagIDBase != 0)
                    objectid = obj.TagIDBase;
                else
                    objectid = obj.ObjectID;
                device = obj;
            }
            else if (node.DeviceType == "90")
            {
                //Text
                var obj = new Text();
                obj.LogicIDTail = node.DeviceID;
                obj.ObjectID = int.Parse(node.DeviceID);  //���ı�����
                obj.FontColor = Color.FromName(node.TextColor);
                obj.TextFont = new Font(node.TextFonStyle, float.Parse(node.TextFontSize));
                obj.Texttest = node.DeviceName;
                left = int.Parse(node.XPos);
                top = int.Parse(node.YPos);
                width = 150;
                height = 40;
                obj.RectangleLs = new Rectangle(left, top, width, height);
                if (obj.TagIDBase != 0)
                    objectid = obj.TagIDBase;
                else
                    objectid = obj.ObjectID;
                device = obj;
            }
            else if (node.DeviceType == "91")
            {
                SwitchPort obj = new SwitchPort();
                obj.MonitorID = node.DeviceName;
                obj.PortID = int.Parse(node.LableId.Length > 0 ? node.LableId : "");
                obj.DeviceID = node.DeviceID;
                left = int.Parse(node.XPos);
                top = int.Parse(node.YPos);
                width = int.Parse(node.Device_W);
                height = int.Parse(node.Device_H);
                obj.RectangleLs = new Rectangle(left, top, width, height);
                objectid = objectid + 1;
                device = obj;
            }
            else if (node.DeviceType == "92")
            {
                Switch obj = new Switch();
                obj.SwitchID = node.DeviceID;
                left = int.Parse(node.XPos);
                top = int.Parse(node.YPos);
                width = int.Parse(node.Device_W);
                height = int.Parse(node.Device_H);
                obj.RectangleLs = new Rectangle(left, top, width, height);
                objectid = objectid + 1;
                device = obj;
            }
            return device;
        }

        /// <summary>
        /// ���SC�豸�б�����
        /// </summary>
        /// <returns></returns>
        public override int CheckSCDeviceVertyList()
        {
            int scDeviceCount = graphicsList.SCVerify.Count;
            return scDeviceCount;
        }

        /// <summary>
        /// �����豸�������ÿؼ����ɼ�
        /// </summary>
        private void SetMapTypeFlag()
        {
            switch (mapType)
            {
                case "device":
                    _isNetMap = false;
                    break;
                case "NetDataSet":
                    _isDeviceMap = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// ����豸��ʾ�ı�
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool AddText(DrawObject obj)
        {
            bool flag = false;
            int textid = obj.ID;
            string objname = obj.GetType().Name;
            string id;
            int left, top, tagid, intflag;
            string deviceName = "";
            DrawObject o = null;

            if (objname == "BOM")
            {
                BOM _property = (BOM)obj;
                left = _property.RectangleLs.X;
                top = _property.RectangleLs.Y;
                id = _property.LogicIDTail;
                tagid = _property.TagIDBase;
                intflag = _property.Flag;

                if (IPHasInclude(graphicsList.IPVerify, _property.IpAdd))
                {
                    //MessageBox.Show("�豸IP�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸IP,���������ã���");
                }

                deviceName = _property.DeviceName;
                if (tagid != 0)
                {
                    for (int i = 0; i < GraphicsList.Count; i++)
                    {
                        o = GraphicsList[i];
                        if (o.GetType().Name == "Text")
                        {
                            if (((Text)o).ObjectID == tagid)
                            {
                                if (HasInclude(graphicsList.BomVerify, id))
                                {
                                    MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID,���������ã���");
                                }
                                else
                                {
                                    ((Text)o).Texttest = deviceName;
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Text tproperty = new Text();
                    tproperty.TextFont = new Font("����", 9, FontStyle.Regular);
                    tproperty.Texttest = deviceName;
                    tproperty.setTextDisplay(left, top - 25);
                    tproperty.ObjectID = intflag;
                    tproperty.TextType = "BOM_Logical_ID";

                    if (HasInclude(graphicsList.BomVerify, id))
                    {
                        MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                    }
                    this.GraphicsList.Add(tproperty);
                    flag = true;//�豸��ɫ����ɾ��ʱͬʱѡ���ı�
                }
            }
            else if (objname == "AGMChannel")
            {
                AGMChannel _property = (AGMChannel)obj;
                left = _property.RectangleLs.X;
                top = _property.RectangleLs.Y;
                id = _property.LogicIDTail;
                tagid = _property.TagIDBase;
                intflag = _property.Flag;
                deviceName = _property.DeviceName;

                if (IPHasInclude(graphicsList.IPVerify, _property.IpAdd))
                {
                    // MessageBox.Show("�豸IP�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸IP,���������ã���");
                }

                if (tagid != 0)
                {
                    for (int i = 0; i < GraphicsList.Count; i++)
                    {
                        o = GraphicsList[i];
                        if (o.GetType().Name == "Text")
                        {
                            if (((Text)o).ObjectID == tagid)
                            {
                                if (HasInclude(graphicsList.AGMVerify, id))
                                {
                                    MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                                }
                                else
                                {
                                    ((Text)o).Texttest = deviceName;
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Text tproperty = new Text();
                    tproperty.TextFont = new Font("����", 9, FontStyle.Regular);
                    tproperty.Texttest = deviceName;
                    tproperty.setTextDisplay(left, top - 25);
                    tproperty.ObjectID = intflag;
                    tproperty.TextType = "AGM_Logical_ID";
                    if (HasInclude(graphicsList.AGMVerify, id))
                    {
                        // MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                    }
                    this.GraphicsList.Add(tproperty);
                    flag = true;
                }
            }
            else if (objname == "AGMChannelDual")
            {
                AGMChannelDual _property = (AGMChannelDual)obj;
                left = _property.RectangleLs.X;
                top = _property.RectangleLs.Y;
                id = _property.LogicIDTail.Trim().ToString();
                tagid = _property.TagIDBase;
                intflag = _property.Flag;
                deviceName = _property.DeviceName;

                if (IPHasInclude(graphicsList.IPVerify, _property.IpAdd))
                {
                    // MessageBox.Show("�豸IP�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸IP,���������ã���");
                }

                if (tagid != 0)
                {
                    for (int i = 0; i < GraphicsList.Count; i++)
                    {
                        o = GraphicsList[i];
                        if (o.GetType().Name == "Text")
                        {
                            if (((Text)o).ObjectID == tagid)
                            {
                                if (HasInclude(graphicsList.AGMVerify, id))
                                {
                                    MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                                }
                                else
                                {
                                    ((Text)o).Texttest = deviceName;
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Text tproperty = new Text();
                    tproperty.TextFont = new Font("����", 9, FontStyle.Regular);
                    tproperty.Texttest = deviceName;
                    tproperty.setTextDisplay(left, top - 25);
                    tproperty.ObjectID = intflag;
                    tproperty.TextType = "AGMDual_Logical_ID";
                    if (HasInclude(graphicsList.AGMVerify, id))
                    {
                        // MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                    }
                    this.GraphicsList.Add(tproperty);
                    flag = true;
                }
            }

            else if (objname == "AGMWallDual")
            {
                AGMWallDual _property = (AGMWallDual)obj;
                left = _property.RectangleLs.X;
                top = _property.RectangleLs.Y;
                id = _property.LogicIDTail.Trim().ToString();
                tagid = _property.TagIDBase;
                intflag = _property.Flag;
                deviceName = _property.DeviceName;


                if (tagid != 0)
                {
                    for (int i = 0; i < GraphicsList.Count; i++)
                    {
                        o = GraphicsList[i];
                        if (o.GetType().Name == "Text")
                        {
                            if (((Text)o).ObjectID == tagid)
                            {
                                if (HasInclude(graphicsList.AGMVerify, id))
                                {
                                    MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                                }
                                else
                                {
                                    ((Text)o).Texttest = deviceName;
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Text tproperty = new Text();
                    tproperty.TextFont = new Font("����", 9, FontStyle.Regular);
                    tproperty.Texttest = deviceName;
                    tproperty.setTextDisplay(left, top - 25);
                    tproperty.ObjectID = intflag;
                    tproperty.TextType = "AGMWallDual_ID";
                    if (HasInclude(graphicsList.AGMVerify, id))
                    {
                        //MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                    }
                    this.GraphicsList.Add(tproperty);
                    flag = true;
                }
            }
            else if (objname == "AGMWallDummy")
            {
                AGMWallDummy _property = (AGMWallDummy)obj;
                left = _property.RectangleLs.X;
                top = _property.RectangleLs.Y;
                id = _property.LogicIDTail.Trim().ToString();
                tagid = _property.TagIDBase;
                intflag = _property.Flag;
                deviceName = _property.DeviceName;

                if (tagid != 0)
                {
                    for (int i = 0; i < GraphicsList.Count; i++)
                    {
                        o = GraphicsList[i];
                        if (o.GetType().Name == "Text")
                        {
                            if (((Text)o).ObjectID == tagid)
                            {
                                if (HasInclude(graphicsList.AGMVerify, id))
                                {
                                    MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                                }
                                else
                                {
                                    ((Text)o).Texttest = deviceName;
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Text tproperty = new Text();

                    tproperty.TextFont = new Font("����", 9, FontStyle.Regular);
                    tproperty.Texttest = deviceName;
                    tproperty.setTextDisplay(left, top - 25);
                    tproperty.ObjectID = intflag;
                    tproperty.TextType = "AGMWallDummy_Logical_ID";

                    if (HasInclude(graphicsList.AGMVerify, id))
                    {
                        MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                    }
                    this.GraphicsList.Add(tproperty);
                    flag = true;
                }
            }

            else if (objname == "AGMWallSingle")
            {
                AGMWallSingle _property = (AGMWallSingle)obj;
                left = _property.RectangleLs.X;
                top = _property.RectangleLs.Y;
                id = _property.LogicIDTail.Trim().ToString();
                tagid = _property.TagIDBase;
                intflag = _property.Flag;
                deviceName = _property.DeviceName;

                if (tagid != 0)
                {
                    for (int i = 0; i < GraphicsList.Count; i++)
                    {
                        o = GraphicsList[i];
                        if (o.GetType().Name == "Text")
                        {
                            if (((Text)o).ObjectID == tagid)
                            {
                                if (HasInclude(graphicsList.AGMVerify, id))
                                {
                                    MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                                }
                                else
                                {
                                    ((Text)o).Texttest = deviceName;
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Text tproperty = new Text();
                    tproperty.TextFont = new Font("����", 9, FontStyle.Regular);
                    tproperty.Texttest = deviceName;
                    tproperty.setTextDisplay(left, top - 25);
                    tproperty.ObjectID = intflag;
                    tproperty.TextType = "AGMWallSingle_Logical_ID";
                    if (HasInclude(graphicsList.AGMVerify, id))
                    {
                        //MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                    }
                    this.GraphicsList.Add(tproperty);
                    flag = true;
                }
            }
            else if (objname == "TVM")
            {
                TVM _property = (TVM)obj;
                left = _property.RectangleLs.X;
                top = _property.RectangleLs.Y;
                id = _property.LogicIDTail.Trim().ToString();
                tagid = _property.TagIDBase;
                intflag = _property.Flag;
                deviceName = _property.DeviceName;
                if (IPHasInclude(graphicsList.IPVerify, _property.IpAdd))
                {
                    //MessageBox.Show("�豸IP�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸IP,���������ã���");
                }
                if (tagid != 0)
                {
                    for (int i = 0; i < GraphicsList.Count; i++)
                    {
                        o = GraphicsList[i];
                        if (o.GetType().Name == "Text")
                        {
                            if (((Text)o).ObjectID == tagid)
                            {
                                if (HasInclude(graphicsList.TVMVerify, id))
                                {
                                    MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                                }
                                else
                                {
                                    ((Text)o).Texttest = deviceName;
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Text tproperty = new Text();

                    tproperty.TextFont = new Font("����", 9, FontStyle.Regular);
                    tproperty.Texttest = deviceName;
                    tproperty.setTextDisplay(left, top - 25);
                    tproperty.ObjectID = intflag;
                    tproperty.TextType = "TVM_Logical_ID";
                    if (HasInclude(graphicsList.TVMVerify, id))
                    {
                        // MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                    }
                    this.GraphicsList.Add(tproperty);
                    flag = true;
                }
            }

            else if (objname == "TCM")
            {
                TCM _property = (TCM)obj;
                left = _property.RectangleLs.X;
                top = _property.RectangleLs.Y;
                id = _property.LogicIDTail.Trim().ToString();
                tagid = _property.TagIDBase;
                intflag = _property.Flag;
                deviceName = _property.DeviceName;
                if (IPHasInclude(graphicsList.IPVerify, _property.IpAdd))
                {
                    //MessageBox.Show("�豸IP�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸IP,���������ã���");
                }
                if (tagid != 0)
                {
                    for (int i = 0; i < GraphicsList.Count; i++)
                    {
                        o = GraphicsList[i];
                        if (o.GetType().Name == "Text")
                        {
                            if (((Text)o).ObjectID == tagid)
                            {

                                if (HasInclude(graphicsList.TCMVerify, id))
                                {
                                    MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                                }
                                else
                                {
                                    ((Text)o).Texttest = deviceName;
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Text tproperty = new Text();

                    tproperty.TextFont = new Font("����", 9, FontStyle.Regular);
                    tproperty.Texttest = deviceName;
                    tproperty.setTextDisplay(left, top - 25);
                    tproperty.ObjectID = intflag;
                    tproperty.TextType = "TCM_Logical_ID";
                    if (HasInclude(graphicsList.TCMVerify, id))
                    {
                        // MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                    }
                    this.GraphicsList.Add(tproperty);
                    flag = true;
                }
            }
            else if (objname == "SC")
            {
                SC _property = (SC)obj;
                left = _property.RectangleLs.X;
                top = _property.RectangleLs.Y;
                id = _property.LogicIDTail;
                tagid = _property.TagIDBase;
                intflag = _property.Flag;
                deviceName = _property.DeviceName;
                if (IPHasInclude(graphicsList.IPVerify, _property.IpAdd))
                {
                    //MessageBox.Show("�豸IP�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸IP,���������ã���");
                }
                if (tagid != 0)
                {
                    for (int i = 0; i < GraphicsList.Count; i++)
                    {
                        o = GraphicsList[i];
                        if (o.GetType().Name == "Text")
                        {
                            if (((Text)o).ObjectID == tagid)
                            {

                                if (HasInclude(graphicsList.SCVerify, id))
                                {
                                    MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                                }
                                else
                                {
                                    ((Text)o).Texttest = deviceName;
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Text tproperty = new Text();
                    tproperty.TextFont = new Font("����", 9, FontStyle.Regular);
                    tproperty.Texttest = deviceName;
                    tproperty.setTextDisplay(left, top - 25);
                    tproperty.ObjectID = intflag;
                    tproperty.TextType = "SC_Logical_ID";
                    if (HasInclude(graphicsList.SCVerify, id))
                    {
                        // MessageBox.Show("�豸ID�ظ���ͬһ��վͬһ�豸���Ͳ������ظ����豸ID��");
                    }
                    this.GraphicsList.Add(tproperty);
                    flag = true;
                }
            }
            return flag;
        }
        #endregion
    }
}
