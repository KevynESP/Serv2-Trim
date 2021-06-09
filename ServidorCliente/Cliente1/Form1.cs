using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cliente1
{
    public partial class Form1 : Form
    {
        public string ipmia = "127.0.0.1";
        public int port = 31416;
        public string msg;

        public Form1()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(ipmia), port);
            try
            {
                using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    s.Connect(ip);
                    using (NetworkStream ns = new NetworkStream(s))
                    using (StreamReader sr = new StreamReader(ns))
                    using (StreamWriter sw = new StreamWriter(ns))
                    {
                        label1.Text = sr.ReadLine();
                        Button b = sender as Button;

                        sw.WriteLine(b.Tag);
                        sw.Flush();
                        if (b.Tag.Equals("APAGAR"))
                        {
                            this.Close();
                        }
                        else
                        {
                            label1.Text = sr.ReadLine();
                        }
                    }
                }    
            }
            catch (SocketException i)
            {
                Console.WriteLine("Error de conexion: {0}", (SocketError)i.ErrorCode);
                return;
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            DialogResult res = f.ShowDialog();
            if (res == DialogResult.OK)
            {
                ipmia = f.textBox1.Text;
                int.TryParse(f.textBox2.Text, out port);
            }
        }
    }
}
