using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dll_injector_c__
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public Form2(string text)
        {
            titleof.Text = text;
            typeot.Text = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Asterisk.Play();
            Form3 form2 = new Form3();
            this.Enabled = false;
            form2.ShowDialog();
            this.Enabled = true;
        }
    }
}
