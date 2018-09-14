/**********************************************************
** �ļ����� SetDrawForm.cs
** �ļ�����:
** 
**---------------------------------------------------------
**�޸���ʷ��¼��
**�޸�ʱ��      �޸���    �޸����ݸ�Ҫ
**2018-09-14    xwj       ����
**
**********************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DrawTools
{
    public partial class SetDrawForm : Form
    {
        private string stationID;
        private int width;
        private int height;
        private string deviceIP;
        public SetDrawForm(string stationID,int width,int height,string deviceIP)
        {
            InitializeComponent();
            textBox1.Text = stationID;
           // textBox2.Text = width.ToString();
           // textBox3.Text = height.ToString();
            ipInputTextbox1.IP = deviceIP.ToString();
            this.deviceIP = deviceIP;
            this.stationID = stationID;
            this.width = width;
            this.height = height;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StationID = textBox1.Text.Trim();
            //AWidth = int.Parse(textBox2.Text.Trim());
            //AHeight = int.Parse(textBox3.Text.Trim());
            DeviceIP = ipInputTextbox1.IP.ToString().Trim();
            //LogHelper.DeviceDeviceLogInfo(string.Format("���³�վ������SC������Ϣ_��վSC���:{0},IP��ַ:{1}",StationID,DeviceIP));
        }

        public string DeviceIP
        {
            get { return deviceIP; }
            set { deviceIP = value; }
        }

        public string StationID
        {
            get { return stationID; }
            set { stationID = value; }
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

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                //if (int.Parse(textBox2.Text.Trim()) > 2000 || int.Parse(textBox2.Text.Trim()) < 0)
                //{
                //    MessageBox.Show(this, "��Ч��ֵ,Widthֵ������2000", "��֤����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    e.Cancel = true;
                //}
            }
            catch
            {
                MessageBox.Show(this,"����Widthֵ��Ч", "��֤����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void textBox3_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                //if (int.Parse(textBox3.Text.Trim()) > 1000 || int.Parse(textBox3.Text.Trim()) < 0)
                //{
                //    MessageBox.Show(this, "��Ч��ֵ,Widthֵ������1000", "��֤����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    e.Cancel = true;
                //}
            }
            catch
            {
                MessageBox.Show(this, "����Widthֵ��Ч", "��֤����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void txtBoxIP_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                //if (_ipVerifyList.Contains(this.txtBoxIP.Text.Trim()))
                //{
                //    MessageBox.Show("�豸IP�ظ���ͬһ��վ�豸�������ظ����豸IP,���������ã���");
                //}

                //Regex reg = new Regex(@"(?n)^(([1-9]?[0-9]|1[0-9]{2}|2([0-4][0-9]|5[0-5]))\.){3}([1-9]?[0-9]|1[0-9]{2}|2([0-4][0-9]|5[0-5]))$");
                //if (!reg.IsMatch(this.txtBoxIP.Text.Trim()))
                //{
                //    e.Cancel = true;
                //    MessageBox.Show("�豸IP��ʽ����ȷ�����������ã���");
                //}
            }
            catch
            {

            }
        }
    }
}