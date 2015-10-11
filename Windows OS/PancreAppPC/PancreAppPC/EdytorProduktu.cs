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
    public partial class EdytorProduktu : Form
    {
        bool pierwszyraz = true;
        public EdytorProduktu()
        {
            
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox17.Text != "" && textBox16.Text != "")
            {
                DialogResult = DialogResult.OK;

            }
            else { MessageBox.Show("Produkt musi mieć jakąś nazwę i jednostke elementarną"); }
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            if (pierwszyraz == false)
            {
                numericUpDown4.Value = numericUpDown13.Value * numericUpDown9.Value / 100;
                numericUpDown5.Value = numericUpDown12.Value * numericUpDown9.Value / 100;
                numericUpDown10.Value = numericUpDown1.Value * numericUpDown9.Value / 100;
                numericUpDown6.Value = numericUpDown11.Value * numericUpDown9.Value / 100;
                numericUpDown7.Value = numericUpDown3.Value * numericUpDown9.Value / 100;
                numericUpDown8.Value = numericUpDown2.Value * numericUpDown9.Value / 100;
            }
        }

        private void numericUpDown13_ValueChanged(object sender, EventArgs e)
        {
            if (pierwszyraz == false) {numericUpDown4.Value = numericUpDown13.Value * numericUpDown9.Value / 100;
            numericUpDown5.Value = numericUpDown12.Value * numericUpDown9.Value / 100;
            numericUpDown10.Value = numericUpDown1.Value * numericUpDown9.Value / 100;
            numericUpDown6.Value = numericUpDown11.Value * numericUpDown9.Value / 100;
            numericUpDown7.Value = numericUpDown3.Value * numericUpDown9.Value / 100;
            numericUpDown8.Value = numericUpDown2.Value * numericUpDown9.Value / 100;}
        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {
           if (pierwszyraz == false) { numericUpDown4.Value = numericUpDown13.Value * numericUpDown9.Value / 100;
            numericUpDown5.Value = numericUpDown12.Value * numericUpDown9.Value / 100;
            numericUpDown10.Value = numericUpDown1.Value * numericUpDown9.Value / 100;
            numericUpDown6.Value = numericUpDown11.Value * numericUpDown9.Value / 100;
            numericUpDown7.Value = numericUpDown3.Value * numericUpDown9.Value / 100;
            numericUpDown8.Value = numericUpDown2.Value * numericUpDown9.Value / 100;}
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
           if (pierwszyraz == false) { numericUpDown4.Value = numericUpDown13.Value * numericUpDown9.Value / 100;
            numericUpDown5.Value = numericUpDown12.Value * numericUpDown9.Value / 100;
            numericUpDown10.Value = numericUpDown1.Value * numericUpDown9.Value / 100;
            numericUpDown6.Value = numericUpDown11.Value * numericUpDown9.Value / 100;
            numericUpDown7.Value = numericUpDown3.Value * numericUpDown9.Value / 100;
            numericUpDown8.Value = numericUpDown2.Value * numericUpDown9.Value / 100;}
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
           if (pierwszyraz == false) { numericUpDown4.Value = numericUpDown13.Value * numericUpDown9.Value / 100;
            numericUpDown5.Value = numericUpDown12.Value * numericUpDown9.Value / 100;
            numericUpDown10.Value = numericUpDown1.Value * numericUpDown9.Value / 100;
            numericUpDown6.Value = numericUpDown11.Value * numericUpDown9.Value / 100;
            numericUpDown7.Value = numericUpDown3.Value * numericUpDown9.Value / 100;
            numericUpDown8.Value = numericUpDown2.Value * numericUpDown9.Value / 100;}
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
           if (pierwszyraz == false) { numericUpDown4.Value = numericUpDown13.Value * numericUpDown9.Value / 100;
            numericUpDown5.Value = numericUpDown12.Value * numericUpDown9.Value / 100;
            numericUpDown10.Value = numericUpDown1.Value * numericUpDown9.Value / 100;
            numericUpDown6.Value = numericUpDown11.Value * numericUpDown9.Value / 100;
            numericUpDown7.Value = numericUpDown3.Value * numericUpDown9.Value / 100;
            numericUpDown8.Value = numericUpDown2.Value * numericUpDown9.Value / 100;}
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
           if (pierwszyraz == false) { numericUpDown4.Value = numericUpDown13.Value * numericUpDown9.Value / 100;
            numericUpDown5.Value = numericUpDown12.Value * numericUpDown9.Value / 100;
            numericUpDown10.Value = numericUpDown1.Value * numericUpDown9.Value / 100;
            numericUpDown6.Value = numericUpDown11.Value * numericUpDown9.Value / 100;
            numericUpDown7.Value = numericUpDown3.Value * numericUpDown9.Value / 100;
            numericUpDown8.Value = numericUpDown2.Value * numericUpDown9.Value / 100;}
        }

        private void EdytorProduktu_Load(object sender, EventArgs e)
        {
            numericUpDown13.Value = numericUpDown4.Value * 100 / numericUpDown9.Value;
            numericUpDown12.Value = numericUpDown5.Value * 100 / numericUpDown9.Value;
            numericUpDown1.Value = numericUpDown10.Value * 100 / numericUpDown9.Value;
            numericUpDown11.Value = numericUpDown6.Value * 100 / numericUpDown9.Value;
            numericUpDown3.Value = numericUpDown7.Value * 100 / numericUpDown9.Value;
            numericUpDown2.Value = numericUpDown8.Value * 100 / numericUpDown9.Value;
            
        }

        private void EdytorProduktu_Shown(object sender, EventArgs e)
        {
            pierwszyraz = false;
        }
    }
}
