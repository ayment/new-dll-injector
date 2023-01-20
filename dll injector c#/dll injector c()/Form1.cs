using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
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
            deleteToolStripMenuItem.Margin = new Padding(0);
            deleteToolStripMenuItem.Padding = new Padding(0);
            deleteToolStripMenuItem.BackColor = Color.FromArgb(28, 28, 28);
            deleteToolStripMenuItem.ForeColor = Color.White;
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
        Dictionary<string, FileInfo> dic = new Dictionary<string, FileInfo>();
        class FileInfo
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
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

                string json = Properties.Settings.Default.listboxItems;
                List<FileInfo> items = JsonConvert.DeserializeObject<List<FileInfo>>(json) ?? new List<FileInfo>();
                string[] newItems = OFD.FileNames;
                foreach (string newItem in newItems)
                {
                    string name = Path.GetFileName(newItem);
                    if (!items.Any(i => i.FileName == name))
                    {
                        items.Add(new FileInfo { FileName = name, FilePath = newItem });
                    }
                    if (!dic.ContainsKey(name))
                    {
                        dic.Add(name, new FileInfo { FileName = name, FilePath = newItem });
                    }
                }
                json = JsonConvert.SerializeObject(items);
                Properties.Settings.Default.listboxItems = json;
                Properties.Settings.Default.Save();

                listBox1.Items.Clear();

                foreach (var item in items)
                {
                    listBox1.Items.Add(item.FileName);
                }

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
            TextBox1.ReadOnly = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/e7hcEqdG8a");
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            DLLP = TextBox1.Text;
            TextBox1.ReadOnly = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.listboxItems))
            {
                string json = Properties.Settings.Default.listboxItems;
                List<FileInfo> items = JsonConvert.DeserializeObject<List<FileInfo>>(json);
                foreach (var item in items)
                {
                    listBox1.Items.Add(item.FileName);
                    dic.Add(item.FileName, item);
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                string selectedItem = (string)listBox1.SelectedItem;
                TextBox1.Text = dic[selectedItem].FilePath;
            }
            catch (ArgumentNullException)
            {

            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.listboxItems = "";
            Properties.Settings.Default.Save();
            listBox1.Items.Clear();

        }

        private void label2_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Asterisk.Play();
            Form2 form2 = new Form2();
            form2.Size = new Size(379, 401);
            form2.panel1.Size = new Size(373, 395);
            form2.button1.Location = new Point(289, 363);
            form2.button2.Visible = true;
            form2.button2.Location = new Point(208, 363);
            form2.titleof.Text = "Info";
            form2.typeot.Text = "Choose Dll\nto choose the dll file you want to inject\n\nInject DLL\nto inject the dll you choosed\n\nDiscord\nto join my discord server bro\n\nClear\nto clear the box list\nif you don't want to use the clear button,\nYou can Right click on the File you want to delete\nand it will show an delete Button (:\n\nAuto Exit\nto close the tool if Injection Succeeded or Failed\n\nReport\nPlease Report if You Getting any Error";
            this.Enabled = false;
            form2.ShowDialog();
            this.Enabled = true;
            form2.button2.Visible = false;
        }

        private void ddToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string selectedItem = (string)listBox1.SelectedItem;
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                dic.Remove(selectedItem);
                string json = Properties.Settings.Default.listboxItems;
                List<FileInfo> items = JsonConvert.DeserializeObject<List<FileInfo>>(json);
                items.RemoveAll(i => i.FileName == selectedItem);
                json = JsonConvert.SerializeObject(items);
                Properties.Settings.Default.listboxItems = json;
                Properties.Settings.Default.Save();
            }
        }
    }
}
