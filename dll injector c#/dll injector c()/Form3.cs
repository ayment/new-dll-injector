using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscordMessenger;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace dll_injector_c__
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public static void sendreport(string URL, string json)
        {
            var wr = WebRequest.Create(URL);
            wr.ContentType = "application/json";
            wr.Method = "POST";
            using (var sw = new StreamWriter(wr.GetRequestStream()))
                sw.Write(json);
            wr.GetResponse();
        }

        int discordColor = 3;
        private void button2_Click(object sender, EventArgs e)
        {
            new DiscordMessage()
                  .SetUsername("name of the webhook")
                  .SetAvatar("link to a png photo")
                  .AddEmbed()
                  .SetTimestamp(DateTime.Now)
            .SetTitle("something")
                  .SetAuthor(textBox1.Text, "link to a website", "link to a gif")
                  .SetDescription(textBox2.Text)
                  .SetColor((int)discordColor)
                  .SetFooter("something", "link to a gif")
                  .Build()
                  .SendMessage("the Webhook link");
            System.Media.SystemSounds.Asterisk.Play();
            Form2 form2 = new Form2();
            form2.button2.Visible = false;
            form2.titleof.Text = "Info";
            form2.typeot.Text = "Your report has been received\nand we will Help You soon as possible";
            this.Enabled = false;
            form2.ShowDialog();
            this.Enabled = true;
            textBox1.Text = String.Empty;
            textBox2.Text = String.Empty;

            var message = new DiscordMessage
            {
                Content = "something",
                Embeds = new List<Embed>()
                    {
                        new Embed
                        {
                                  Description = "something"
                        }
                    }

            };
        }
    }
}
