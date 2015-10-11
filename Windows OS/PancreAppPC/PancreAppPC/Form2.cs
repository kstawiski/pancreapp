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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public List<string> nazwyproduktów = new List<string>();
        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0) { DialogResult = DialogResult.OK; } else { MessageBox.Show("Nie zaznaczyłeś żadnego produktu!"); }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (string i in nazwyproduktów) { listBox1.Items.Add(i); }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.TextLength > 1)
            {
                listBox1.Items.Clear();
                foreach (string i in nazwyproduktów) { if (i.Contains(textBox1.Text)) { listBox1.Items.Add(i); } }

            }
            else {
                listBox1.Items.Clear();
                foreach (string i in nazwyproduktów) { listBox1.Items.Add(i); }
            
            }
        }
    }
}
