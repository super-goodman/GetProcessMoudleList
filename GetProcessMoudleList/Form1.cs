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
            openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:,0,0而不是c:,0
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

        public bool injection(int pid)
        {

            IntPtr sectionHandle = default;
            long sectionMaxSize = 4096;

            //创建映射对象
            Ntstatus state =  NativeMethods.NtCreateSection(
                ref sectionHandle, 
                AccessMask.SECTION_MAP_READ | AccessMask.SECTION_MAP_WRITE | AccessMask.SECTION_MAP_EXECUTE, 
                IntPtr.Zero, 
                ref sectionMaxSize,
                MemoryProtectionConstraints.PAGE_EXECUTE_READWRITE,
                SectionProtectionConstraints.SEC_COMMIT,
                IntPtr.Zero);
            if (state != Ntstatus.STATUS_SUCCESS)
                return false;

            //映射自身
            long sectionOffset = 0;
            IntPtr localSectionAddress = IntPtr.Zero;
            uint size = 4096;
            state = NativeMethods.NtMapViewOfSection(
                sectionHandle,
                Process.GetCurrentProcess().Handle, 
                ref  localSectionAddress,
                UIntPtr.Zero,
                0,
                ref sectionOffset, 
                ref size, 
                2,
                0,
                MemoryProtectionConstraints.PAGE_EXECUTE_READWRITE);
            if (state != Ntstatus.STATUS_SUCCESS)
                return false;

            //打开目标进程
            if (pid == 0)
            {
                textBox1.Text = "请先获得PID";
                return false;
            }
            Process process = Process.GetProcessById(pid);
            IntPtr hProcess = NativeMethods.OpenProcess(ProcessAccessFlags.PROCESS_ALL_ACCESS, false, process.Id);

            //映射目标进程
            sectionOffset = 0;
            IntPtr remoteSectionAddress = IntPtr.Zero;
            state = NativeMethods.NtMapViewOfSection(
             sectionHandle,
             hProcess,
             ref remoteSectionAddress,
             UIntPtr.Zero,
             0,
             ref sectionOffset,
             ref size,
             2,
             0,
             MemoryProtectionConstraints.PAGE_EXECUTE_READWRITE);
            if (state != Ntstatus.STATUS_SUCCESS)
                return false;

            //向内存写入数据

            byte[] code = new byte[] {0xfc,0x68,0x6a,0x0a,0x38,0x1e,0x68,0x63,0x89,0xd1,0x4f,0x68,0x32,0x74,0x91,0x0c
            ,0x8b,0xf4,0x8d,0x7e,0xf4,0x33,0xdb,0xb7,0x04,0x2b,0xe3,0x66,0xbb,0x33,0x32,0x53
            ,0x68,0x75,0x73,0x65,0x72,0x54,0x33,0xd2,0x64,0x8b,0x5a,0x30,0x8b,0x4b,0x0c,0x8b
            ,0x49,0x1c,0x8b,0x09,0x8b,0x69,0x08,0xad,0x3d,0x6a,0x0a,0x38,0x1e,0x75,0x05,0x95
            ,0xff,0x57,0xf8,0x95,0x60,0x8b,0x45,0x3c,0x8b,0x4c,0x05,0x78,0x03,0xcd,0x8b,0x59
            ,0x20,0x03,0xdd,0x33,0xff,0x47,0x8b,0x34,0xbb,0x03,0xf5,0x99,0x0f,0xbe,0x06,0x3a
            ,0xc4,0x74,0x08,0xc1,0xca,0x07,0x03,0xd0,0x46,0xeb,0xf1,0x3b,0x54,0x24,0x1c,0x75
            ,0xe4,0x8b,0x59,0x24,0x03,0xdd,0x66,0x8b,0x3c,0x7b,0x8b,0x59,0x1c,0x03,0xdd,0x03
            ,0x2c,0xbb,0x95,0x5f,0xab,0x57,0x61,0x3d,0x6a,0x0a,0x38,0x1e,0x75,0xa9,0x33,0xdb
            ,0x53,0x68,0x77,0x65,0x73,0x74,0x68,0x66,0x61,0x69,0x6c,0x8b,0xc4,0x53,0x50,0x50
            ,0x53,0xff,0x57,0xfc,0x53,0xff,0x57,0xf8};

            bool br = NativeMethods.WriteProcessMemory(Process.GetCurrentProcess().Handle, localSectionAddress,code,code.Length+1,out IntPtr bytesWritten);
            if (!br)
            {
                return false;   
            }
            IntPtr targetThreadHandle = IntPtr.Zero;
            state =  NativeMethods.RtlCreateUserThread(
                hProcess,
                IntPtr.Zero, 
                false, 
                0,
                IntPtr.Zero,
                IntPtr.Zero, 
                remoteSectionAddress,
                IntPtr.Zero, 
                ref targetThreadHandle,
                IntPtr.Zero);

            return true;

        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (ProcessID != 0)
            {
                injection(ProcessID);
            }
   
        }
    }
}
