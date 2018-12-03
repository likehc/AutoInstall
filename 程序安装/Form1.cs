using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 程序安装
{
    public partial class Form1 : Form
    {
        int AppNum = 0; //安装到第几行APP
        CheckBox chkBox;
        int page = 0;
        private delegate void FlushClient();//代理     

        public class Mouse
        {
            public static bool leave;           
        }
        public class Global_data
        {
            public static string AccDp; //程序路径放入控件说明里的
        }
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(@".\Programs") == false) ;
            {
                Directory.CreateDirectory(@".\Programs");
            }

            listView1.View = View.Details;//设置视图  
            listView1.SmallImageList = imageList1;//设置图标  
            //添加列  
            listView1.Columns.Add("图标", 40, HorizontalAlignment.Left);
            listView1.Columns.Add("已选择程序", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("程序目录", 200, HorizontalAlignment.Left);
            listView1.FullRowSelect = true; //迁中一点，突出一行
            searchApp_Click(); //查找文件夹、用以创建tables及checkbox
        }
        private void searchApp_Click()
        {
            DirectoryInfo di = new DirectoryInfo(@".\Programs");
            DirectoryInfo[] diArr = di.GetDirectories();
            foreach (DirectoryInfo dri in diArr)
            {
                int left = 25;  //控件靠左
                int top = 2;
                tabControl1.TabPages.Add(dri.ToString());
                DirectoryInfo App = new DirectoryInfo(@".\Programs\" + dri);
                DirectoryInfo[] AppArr = App.GetDirectories();
                int AppRow = 0;
                foreach (DirectoryInfo Appdri in AppArr)  //遍历文件
                {
                    chkBox = new CheckBox();
                    chkBox.Left = left;
                    chkBox.Top = top;
                    string str1 = Appdri.Name;
                    chkBox.AutoSize = true;
                    int len = Appdri.Name.Length; //文件名长度
                    if (len > 13)
                    {
                        str1 = str1.Substring(0, 12);
                        str1 = str1 + "..";
                    }
                    tabControl1.TabPages[page].Controls.Add(chkBox);
                    chkBox.AutoSize = false;
                    chkBox.Height = 30;
                    chkBox.Width = 110;
                    chkBox.Click += chkBox_Click;
                    chkBox.Text = str1.ToString();
                    PictureBox pic = new PictureBox();
                    pic.Left = left - 22;   //ico图片离checkbox的距离
                    pic.Top = top + 6;
                    if (File.Exists(@Appdri.FullName + "\\" + "ico.ico"))
                    {
                        pic.Image = Image.FromFile(Appdri.FullName + "\\" + "ico.ico");
                        chkBox.AccessibleDescription = Appdri.FullName;
                    }
                    else
                    {
                        if (File.Exists(di.FullName + "\\" + "ico.ico"))
                        {
                            pic.Image = Image.FromFile(di.FullName + "\\" + "ico.ico");
                            chkBox.AccessibleDescription = Appdri.FullName;
                        }
                    }
                    tabControl1.TabPages[page].Controls.Add(pic);
                    pic.Height = 20;
                    pic.Width = 20;
                    pic.SizeMode = PictureBoxSizeMode.Zoom;
                    top = top + 25;
                    AppRow++;
                    if ((AppRow >= 7) && ((AppRow % 7) == 0))
                    {
                        top = 2;
                    }
                    left = (AppRow / 7) * 140 + 25;
                }
                page++;
            }
        }

        #region 执行cmd命令

        /// <summary>
        /// 执行cmd命令
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private string ExeCommand(string commandText)
        {
            Process p = new Process();  //创建并实例化一个操作进程的类：Process
            p.StartInfo.FileName = "cmd.exe";    //设置要启动的应用程序
            p.StartInfo.UseShellExecute = false;   //设置是否使用操作系统shell启动进程
            p.StartInfo.RedirectStandardInput = true;  //指示应用程序是否从StandardInput流中读取
            p.StartInfo.RedirectStandardOutput = true; //将应用程序的输入写入到StandardOutput流中
            p.StartInfo.RedirectStandardError = true;  //将应用程序的错误输出写入到StandarError流中
            p.StartInfo.CreateNoWindow = true;    //是否在新窗口中启动进程
            string strOutput = null;
            try
            {
                p.Start();
                p.StandardInput.WriteLine(commandText);    //将CMD命令写入StandardInput流中
                p.StandardInput.WriteLine("exit");         //将 exit 命令写入StandardInput流中
                strOutput = p.StandardOutput.ReadToEnd();   //读取所有输出的流的所有字符
                p.WaitForExit();                           //无限期等待，直至进程退出
                p.Close();                                  //释放进程，关闭进程
            }
            catch (Exception e)
            {
                strOutput = e.Message;
            }
            return strOutput;

        }
        //调用：

        // 例：ExeCommand（“shutdown -s -t 7200”）   //设定两小时后关机
        #endregion

        private void btnIstall_Click(object sender, EventArgs e)
        {
            #region 判断所选程序是否为空
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show("尚未选择安装程序！", "提示");
                return;
            }
            else
            {
                btnIstall.Enabled = false;
            }
            #endregion
            timer1.Enabled = true;
        }
        #region 线程代理，防主窗体假死
        /// <summary>
        /// 线程代理，防主窗体假死
        /// </summary>
        private void CrossThreadFlush()
        {
            ThreadFunction();
        }
        #endregion
        private void ThreadFunction()
        {
            if (this.listView1.InvokeRequired)//等待异步
            {
                FlushClient fc = new FlushClient(ThreadFunction);
                this.Invoke(fc);//通过代理调用刷新方法
            }
            else
            {
                ExeCommand(@".\Programs\" + "hide.exe " + listView1.Items[AppNum].SubItems[2].Text.ToString());
                listView1.Items[AppNum].SubItems[2].Text = "OK!";
            }
        }
        private void chkBox_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            imageList1.Images.Clear();
            int picNum = 0;     //listImage 的items值            
            for (int i = 0; i < page; i++)
            {
                foreach (Control c in tabControl1.TabPages[i].Controls)  //遍历tabControl1内的所有控件
                {
                    if (c is CheckBox)//只遍历CheckBox控件 
                    {
                        if (((CheckBox)c).Checked)
                        {
                            if (File.Exists(@c.AccessibleDescription + "\\" + "ico.ico"))
                            {
                                imageList1.Images.Add(Image.FromFile(@c.AccessibleDescription + "\\" + "ico.ico"));
                            }
                            ListViewItem lv = new ListViewItem();
                            lv.ImageIndex = picNum;//显示imageList中的第picNum张图片
                            lv.SubItems.Add(c.Text.ToString());
                            lv.SubItems.Add(c.AccessibleDescription.ToString());
                            listView1.Items.Add(lv);
                            picNum++;
                        }
                    }
                }
            }
            listView1.Columns[1].Text = ("已选择程序(" + listView1.Items.Count + ")");
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].SubItems[2].Text.ToString() != "OK!")
                {

                    AppNum = i;
                    break;
                }
                else
                {
                    if (i + 1 >= listView1.Items.Count)
                    {
                        timer1.Enabled = false;
                        MessageBox.Show("所选程序已安装完毕！", "提示");
                        btnIstall.Enabled = true;
                        return;
                    }
                }
            }
            Thread thread = new Thread(CrossThreadFlush);
            thread.IsBackground = true;
            thread.Start();
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToString();
            
        }
        
        private void listView1_SelectedIndexChanged(object sender, EventArgs e) //提取路径
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                Global_data.AccDp = "";
                //listView1.Items[i].SubItems[2].Text.ToString()
                Global_data.AccDp += item.SubItems[2].Text;
                Mouse.leave = false;
            }           
        }
        private void listView1_MouseLeave(object sender, EventArgs e)
        {           
             Mouse.leave = true;           
        }
        private void listView1_MouseEnter(object sender, EventArgs e)
        {
            Mouse.leave = false;
            ToolTip p = new ToolTip();
            p.ShowAlways = true;
            p.SetToolTip(this.listView1, "可拖拽删除,(拖拽请勿出此控件)");
        }   
        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
       
            if (Mouse.leave == false)
            {
                return;
            }    
                int Index = 0;
                if (this.listView1.SelectedItems.Count > 0)//判断listview有被选中项
                {
                    Index = this.listView1.SelectedItems[0].Index;//取当前选中项的index,SelectedItems[0]这必须为0
                    listView1.Items[Index].Remove();
                }
                for (int i = 0; i < page; i++)
                {
                    int chkNum = 0;  //存放每个page下，第几个checkbox
                    foreach (Control c in tabControl1.TabPages[i].Controls)  //遍历tabControl1内的所有控件
                    {
                        if (c is CheckBox)//只遍历CheckBox控件 
                        {
                            if (((CheckBox)c).AccessibleDescription == Global_data.AccDp)
                            {
                                ((CheckBox)c).Checked = false;
                            }
                            chkNum++;
                        }
                    }
                }
                listView1.Columns[1].Text = ("已选择程序(" + listView1.Items.Count + ")");  
        }
        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {            
            Mouse.leave = true;             
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.leave = true;
        }
    }
}
