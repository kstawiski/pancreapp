using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PancreAppPC
{
    public partial class Logowanie : Form
    {
        public Logowanie()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 main = new Form1();
            main.login = textBox1.Text;
            main.passmd5 = main.GetMd5Hash(main.md5Hash, textBox2.Text);
            //MessageBox.Show(main.passmd5);
            string wynik = main.serwer("check_credentials", "login=" + main.login + "&pass=" + main.passmd5);
            //MessageBox.Show(wynik);
            if (wynik != "NIE" && wynik != "BLAD" && wynik != "-") { main.zalogowany = true; main.uid = Convert.ToInt32(wynik);
            using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\id"))
            {
                w.Write(main.login + "\t" + main.passmd5);

                w.Close();
            }


            DialogResult = DialogResult.OK;
            }
            else { main.zalogowany = false; MessageBox.Show("Welcome to PancreApp! Server denied login as it is ImagineCup test edition. Only local usage is possible."); DialogResult = DialogResult.OK; } 
        }
    }
}
