using Microsoft.Win32.SafeHandles;
using MouseGetProcess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BypassCrc;
using System.Diagnostics;
using System.IO;

namespace GetProcessMoudleList
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        public bool ReadModule1(int dwProcessID)
        {
            if (dwProcessID != 0)
            {
                Process process = Process.GetProcessById(dwProcessID);
                ProcessModuleCollection myProcessModuleCollection = process.Modules;
                for (int i = 0; i < myProcessModuleCollection.Count; i++)
                {
                    ProcessModule myProcessModule = myProcessModuleCollection[i];
                    textBox3.Text += "ModuleName";
                    textBox3.Text += "[";
                    textBox3.Text += i.ToString();
                    textBox3.Text += "] = ";
                    textBox3.Text += myProcessModule.ModuleName;
                    textBox3.Text += "\r\n";
                    textBox3.Text += "Address = 0x";
                    textBox3.Text += myProcessModule.BaseAddress.ToString("X");
                    textBox3.Text += "\r\n";
                    textBox3.Text += "--------------------------------------";
                    textBox3.Text += "\r\n";
                }
                process.Close();
                return true;
            }
            else
            {
                MessageBox.Show("请选择目标进程！", "警告");
                return false;
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            ReadModule1(ProcessID);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox1.Image = (Image)Properties.Resources.ResourceManager.GetObject("鼠标选择窗口按钮2");
            pictureBox1.BackColor = Color.Black;
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.Image = (Image)Properties.Resources.ResourceManager.GetObject("鼠标选择窗口按钮2");

        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Image = (Image)Properties.Resources.ResourceManager.GetObject("鼠标选择窗口按钮");
        }
        public static int ProcessID = default;
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox1.BackColor = Control.DefaultBackColor;
            MouseGetProcessClass.POINTAPI point = new MouseGetProcessClass.POINTAPI();
            MouseGetProcessClass.GetCursorPos(ref point);//获取当前鼠标坐标
            int hwnd = MouseGetProcessClass.WindowFromPoint(point);//获取指定坐标处窗口的句柄
            StringBuilder name = new StringBuilder(256);
            MouseGetProcessClass.GetWindowText(hwnd, name, 256);
            textBox1.Text = name.ToString();
            MouseGetProcessClass.GetWindowThreadProcessId(hwnd, out ProcessID);
            textBox2.Text = ProcessID.ToString();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        public bool injectDll(string name, int dwProcessID)
        {
            Process process = Process.GetProcessById(dwProcessID);
   
            IntPtr procHandle = NativeMethods.OpenProcess(ProcessAccessFlags.PROCESS_ALL_ACCESS, false, process.Id);
            if (procHandle == IntPtr.Zero)
            {
                return false;
            }
            IntPtr loadLibraryAddr = NativeMethods.GetProcAddress(NativeMethods.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (loadLibraryAddr == IntPtr.Zero)
            {
                return false;
            }
            IntPtr allocMemAddress = NativeMethods.VirtualAllocEx(procHandle, IntPtr.Zero, ((name.Length + 1) * Marshal.SizeOf(typeof(char))), MemoryAllocationType.MEM_COMMIT | MemoryAllocationType.MEM_RESERVE, MemoryProtectionConstraints.PAGE_EXECUTE_READWRITE);
            if (allocMemAddress == IntPtr.Zero)
            {
                return false;
            }
            if(!NativeMethods.WriteProcessMemory(procHandle, allocMemAddress, Encoding.Default.GetBytes(name), (int)((name.Length + 1) * Marshal.SizeOf(typeof(char))), out IntPtr bytesWritten))
                return false;
            IntPtr address = NativeMethods.CreateRemoteThread(procHandle, IntPtr.Zero, 0, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);
            if (address == IntPtr.Zero)
            {
                return false;
            }
            return true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string dllName = textBox4.Text;
            if (ProcessID != 0)
            {
                injectDll(dllName, ProcessID);
            } 
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            openFileDialog.Filter = "文本文件|*.*|C#文件|*.cs|所有文件|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = openFileDialog.FileName;
                textBox4.Text = fName;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
