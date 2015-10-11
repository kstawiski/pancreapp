using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PancreAppPC
{
    public partial class ZobaczWykres : Form
    {
        public ZobaczWykres()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart3.Series.Clear(); DialogResult = DialogResult.OK; this.Close();
        }

        private void chart3_Click(object sender, EventArgs e)
        {
            chart3.Series.Clear(); DialogResult = DialogResult.OK; this.Close();
        }
    }
}
