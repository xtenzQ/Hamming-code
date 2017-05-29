using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgCrypt
{
    public delegate void ShowFrm2();

    public partial class CRC : Form
    {
        public event ShowFrm evtFrm2;

        public CRC()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1.pol = textBox1.Text;
            this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !("\b01".Contains(e.KeyChar));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "1000010001";
        }

        private void CRC_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!(Form1.pol == null || Form1.pol.Length == 0))
            {
                if (evtFrm2 != null)
                {
                    evtFrm2();
                }
            }
        }
    }
}
