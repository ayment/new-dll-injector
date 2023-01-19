using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace dll_injector_c__
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            refreshlisttimer();
            Process[] PC = Process.GetProcesses().Where(p => (long)p.MainWindowHandle != 0).ToArray();
            Combobox1.Items.Clear();
            foreach (Process p in PC)
            {
                _ = Combobox1.Items.Add(p.ProcessName);
            }
        }


        public void refreshlisttimer()
        {
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 2000;
            timer1.Start();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            refreshlist();
        }

        public void refreshlist()
        {
            Process[] PC = Process.GetProcesses().Where(p => (long)p.MainWindowHandle != 0).ToArray();
            Combobox1.Items.Clear();
            foreach (Process p in PC)
            {
                _ = Combobox1.Items.Add(p.ProcessName);
            }
        }
        Point lastPoint;
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog OFD = new OpenFileDialog();
                OFD.RestoreDirectory = true;
                OFD.Title = "Please Choose a DLL File";
                OFD.DefaultExt = "dll";
                OFD.Filter = "DLL Files (*.dll)|*.dll";
                OFD.CheckFileExists = true;
                OFD.CheckPathExists = true;
                OFD.ShowDialog();

                TextBox1.Text = OFD.FileName;
                DLLP = OFD.FileName;

            }
            catch (Exception ed)
            {
                MessageBox.Show(ed.Message);
            }
        }
        public static readonly IntPtr INTPTR_ZERO = (IntPtr)0;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        private void button4_Click(object sender, EventArgs e)
        {
            int Result = Inject(Combobox1.Text, DLLP);

            if (Result != 1)
            {
                if (Result == 2)
                {
                    System.Media.SystemSounds.Hand.Play();
                    Form2 form2 = new Form2();
                    form2.titleof.Text = "Error";
                    form2.typeot.Text = "Process Does Not Exist";
                    this.Enabled = false;
                    form2.ShowDialog();
                    this.Enabled = true;

                }
                else if (Result == 3)
                {
                    System.Media.SystemSounds.Hand.Play();

                    Form2 form2 = new Form2();
                    form2.titleof.Text = "Error";
                    form2.typeot.Text = "Injection Failed";
                    this.Enabled = false;
                    form2.ShowDialog();
                    this.Enabled = true;
                    if (checkBox2.Checked)
                    {
                        Environment.Exit(0);
                    }
                }
                else if (Result == 4)
                {
                    System.Media.SystemSounds.Asterisk.Play();
                    Form2 form2 = new Form2();
                    form2.titleof.Text = "Info";
                    form2.typeot.Text = "Injection Succeeded";
                    this.Enabled = false;
                    form2.ShowDialog();
                    this.Enabled = true;
                    if (checkBox2.Checked)
                    {
                        Environment.Exit(0);
                    }

                }
            }
            else
            {
                System.Media.SystemSounds.Hand.Play();

                Form2 form2 = new Form2();
                form2.titleof.Text = "Error";
                form2.typeot.Text = "Please Pick a DLL file!";
                this.Enabled = false;
                form2.ShowDialog();
                this.Enabled = true;
            }
        }
        public static int Inject(string PM, string DLLP)
        {

            if (!File.Exists(DLLP)) { return 1; }
            uint _procId = 0;
            Process[] _procs = Process.GetProcesses();
            for (int i = 0; i < _procs.Length; i++)
            {
                if (_procs[i].ProcessName == PM)
                {
                    _procId = (uint)_procs[i].Id;
                }
            }
            if (_procId == 0) { return 2; }
            if (!SI(_procId, DLLP))
            {
                return 3;
            }
            return 4;
        }
        public static bool SI(uint p, string DLLP)
        {
            IntPtr intPtr = Form1.OpenProcess(1082U, 1, p);
            if (intPtr == Form1.INTPTR_ZERO)
            {
                return false;
            }
            IntPtr procAddress = Form1.GetProcAddress(Form1.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (procAddress == Form1.INTPTR_ZERO)
            {
                return false;
            }
            IntPtr intPtr2 = Form1.VirtualAllocEx(intPtr, (IntPtr)null, (IntPtr)DLLP.Length, 12288U, 64U);
            if (intPtr2 == Form1.INTPTR_ZERO)
            {
                return false;
            }
            byte[] bytes = Encoding.ASCII.GetBytes(DLLP);
            if (Form1.WriteProcessMemory(intPtr, intPtr2, bytes, (uint)bytes.Length, 0) == 0)
            {
                return false;
            }
            if (Form1.CreateRemoteThread(intPtr, (IntPtr)null, Form1.INTPTR_ZERO, procAddress, intPtr2, 0U, (IntPtr)null) == Form1.INTPTR_ZERO)
            {
                return false;
            }
            Form1.CloseHandle(intPtr);
            return true;

        }

        private static string DLLP { get; set; }
        private void TextBox1_TextChanged_1(object sender, EventArgs e)
        {
            DLLP = TextBox1.Text;
            TextBox1.ReadOnly = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/e7hcEqdG8a");
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            DLLP = TextBox1.Text;
            TextBox1.ReadOnly = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer2.Start();
        }
    }
}
