using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NumbcubePusher
{
    public partial class Form1 : Form
    {
        IniFiles ini = new IniFiles(Application.StartupPath + @"\stream.ini");//Application.StartupPath只适用于winform窗体程序
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                if (textBox1.Text.Trim() == "")
                { MessageBox.Show("请先选择文件"); }
                else if (textBox2.Text.Trim() == "")
                { MessageBox.Show("请填写推流地址"); }
                else if (bitRateBox.Text.Trim() == "")
                { MessageBox.Show("请正确填写推流码率"); }
                else if (Int32.Parse(bitRateBox.Text) < 0 || Int32.Parse(bitRateBox.Text) > 10000)
                { MessageBox.Show("请正确填写推流码率"); }
                else
                {
                    ini.IniWriteValue("串流设置", "推流地址", textBox2.Text.Trim());
                    ini.IniWriteValue("串流设置", "直播码", textBox3.Text.Trim());
                    if (checkBox2.Checked == true)
                    {
                        ini.IniWriteValue("串流设置", "aac", "1");
                    }
                    else
                    {
                        ini.IniWriteValue("串流设置", "aac", "0");
                    }
                    if (checkBox1.Checked == true)
                    {
                        ini.IniWriteValue("串流设置", "x264", "1");
                    }
                    else
                    {
                        ini.IniWriteValue("串流设置", "x264", "0");
                    }
                    ini.IniWriteValue("串流设置", "bitRate", bitRateBox.Text.Trim());
                    ini.IniWriteValue("串流设置", "resolution", resolutionBox.SelectedIndex.ToString());

            
                    /*
                    string strInput = "ffmpeg -re -stream_loop -1 -i "+textBox1.Text.Trim()+" -c copy -f flv "+textBox2.Text.Trim();
                    Process p = new Process();
                    //设置要启动的应用程序
                    p.StartInfo.FileName = "cmd.exe";
                    //是否使用操作系统shell启动
                    p.StartInfo.UseShellExecute = false;
                    // 接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardInput = true;
                    //输出信息
                    p.StartInfo.RedirectStandardOutput = true;
                    // 输出错误
                    p.StartInfo.RedirectStandardError = true;
                    //不显示程序窗口
                    p.StartInfo.CreateNoWindow = false;
                    //启动程序
                    p.Start();

                    //向cmd窗口发送输入信息
                    p.StandardInput.WriteLine(strInput + "&exit");

                    p.StandardInput.AutoFlush = true;

                    //获取输出信息
                    string strOuput = p.StandardOutput.ReadToEnd();
                    //等待程序执行完退出进程
                    p.WaitForExit();
                    p.Close();
                    */
                
                                      //开始写入
                        String x264str = "";
                        String bitstr = "";
                        String resstr = "";
                        String accstr = "";
                    if (checkBox1.Checked == true) {
                            x264str = " -c:v libx264 -preset superfast -tune zerolatency";
                            bitstr = " -b:v "+bitRateBox.Text.Trim()+"k";
                            if(resolutionBox.SelectedIndex != 0)
                            {
                                 resstr = " -s "+resolutionBox.Text;
                            }
                        
                         }
                    if (checkBox2.Checked == true)
                    {
                        accstr = " -c:a aac -ar 44100";
                    }
                        FileStream fs = new FileStream(Application.StartupPath + @"\readytoplay.bat", FileMode.Create);
                    //获得字节数组
                    byte[] data = System.Text.Encoding.Default.GetBytes("title=正在放映: " + textBox1.Text.Trim() + "\r\nffmpeg -re -i " + textBox1.Text.Trim() + " -c copy -f flv "+ x264str +bitstr+resstr +accstr+" "+ textBox2.Text.Trim() + textBox3.Text.Trim());

               
                        fs.Write(data, 0, data.Length);
                        //清空缓冲区、关闭流
                        fs.Flush();
                        fs.Close();


                        ProcessStartInfo processInfo = new ProcessStartInfo();
                        processInfo.FileName = Application.StartupPath + @"\readytoplay.bat";
                        processInfo.UseShellExecute = true;
                        Process.Start(processInfo);

                        this.Close();


                }
            }
            catch
            {
                MessageBox.Show("未知错误");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (ini.ExistINIFile())//验证是否存在文件，存在就读取
            {
                try
                {
                    textBox2.Text = ini.IniReadValue("串流设置", "推流地址");
                    textBox3.Text = ini.IniReadValue("串流设置", "直播码");
                    if (ini.IniReadValue("串流设置", "aac") == "1")
                    {
                        checkBox2.Checked = true;
                    }
                    else {
                        checkBox2.Checked = false;
                    }
                    if (ini.IniReadValue("串流设置", "x264") == "1")
                    {
                        checkBox1.Checked = true;
                    }
                    else
                    {
                        checkBox1.Checked = false;
                        bitRateBox.Enabled = false;
                        resolutionBox.Enabled = false;
                    }
                    bitRateBox.Text = ini.IniReadValue("串流设置", "bitRate");
                    resolutionBox.SelectedIndex = int.Parse(ini.IniReadValue("串流设置", "resolution"));
                }
                catch {
                    ini.IniWriteValue("串流设置", "推流地址", "rtmp://stream.numbcube.com/numblive/");
                    ini.IniWriteValue("串流设置", "直播码", "livestreamcode");
                    ini.IniWriteValue("串流设置", "aac", "1");
                    ini.IniWriteValue("串流设置", "x264", "0");
                    ini.IniWriteValue("串流设置", "bitRate", "2000");
                    ini.IniWriteValue("串流设置", "resolution", "0");
                    MessageBox.Show("配置文件读取错误，已删除，请重新启动");
                    this.Close();
                }
             
                
                
            }
            else
            {
                ini.IniWriteValue("串流设置", "推流地址", "rtmp://stream.numbcube.com/numblive/");
                ini.IniWriteValue("串流设置", "直播码", "livestreamcode");
                ini.IniWriteValue("串流设置", "aac", "1");
                ini.IniWriteValue("串流设置", "x264", "0");
                ini.IniWriteValue("串流设置", "bitRate", "2000");
                ini.IniWriteValue("串流设置", "resolution", "0");
                textBox2.Text = "rtmp://stream.numbcube.com/numblive/";
                textBox3.Text = "2b35ab624dcf336";
                bitRateBox.Enabled = false;
                resolutionBox.Enabled = false;
                resolutionBox.SelectedIndex = 0;
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                bitRateBox.Enabled = true;
                resolutionBox.Enabled = true;
            }
            else {
                bitRateBox.Enabled = false;
                resolutionBox.Enabled = false;
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = dialog.FileName;
            }
        }
    }
}
