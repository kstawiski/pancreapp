using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using Finisar.SQLite;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Resources;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
//using WebSocket4Net;
//using Alchemy;
//using Alchemy.Classes;
//using System.Collections.IComparer;

namespace PancreAppPC
{
    
 


  
    public partial class Form1 : Form
    {
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        public int i = 0;

        WebSocket ws = new WebSocket(new Uri("ws://62.87.177.20:29995"));
//WebSocket websocket = new WebSocket("");
  //      websocket.Opened += new EventHandler(websocket_Opened);
//websocket.Error += new EventHandler<ErrorEventArgs>(websocket_Error);
//websocket.Closed += new EventHandler(websocket_Closed);
//websocket.MessageReceived += new EventHandler(websocket_MessageReceived);
//websocket.Open()

//private void websocket_Opened(object sender, EventArgs e)
//{
//     websocket.Send("Hello World!");
//}

        public bool zalogowany = false;
        public bool polzserv = false;
        public int uid = -1;

        
        //OBSŁUGA MD5
        public MD5 md5Hash = MD5.Create();
        public string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public string serwer(string typ, string zapytanie) {

            try
            {
                WebRequest request = WebRequest.Create(Uri.EscapeUriString("http://62.87.177.20:8001/konrad_server/" + typ + ".php"));
                ((HttpWebRequest)request).UserAgent = "PancreApp PC";
                DateTime dt = DateTime.Now;
                string timestamp = dt.ToString("yyyyMMddHHmmss");
                request.Method = "POST";
                // Create POST data and convert it to a byte array.
                string postData = Uri.EscapeUriString("apikey=dQVcJm9RX0HF6cE6OLe1&apipass=l6WugXhWKpjefPOJmFU9&timestamp=" + timestamp + "&" + zapytanie);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/x-www-form-urlencoded";
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                return responseFromServer;
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
                polzserv = true;
            }
            catch (Exception e) { return "-"; 
            
                List <string> bufor = new List<string>();
                if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\bufor")) { string[] odczyt = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\bufor");
                foreach (string o in odczyt) {
                    bufor.Add(o);
                }
                bufor.Add(typ + "\t" + zapytanie);
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\bufor", bufor);
                }
            
            
            
            
            
            
            
            
            
            
            
            
            }
            
            //WebResponse response = request.GetResponse();
            //    // Display the status.
            //    Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            //    // Get the stream containing content returned by the server.
            //    Stream dataStream = response.GetResponseStream();
            //    // Open the stream using a StreamReader for easy access.
            //    StreamReader reader = new StreamReader(dataStream);
            //    // Read the content.
            //    string responseFromServer = reader.ReadToEnd();
            //    // Display the content.
            //    return responseFromServer;
            
        
        }

        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        public static extern int AddFontResource(
            [In][MarshalAs(UnmanagedType.LPWStr)]
        string lpFileName);
        
        public Form1()
        {
            
            
            
            string fontName = "Trebuchet MS";
            float fontSize = 12;
            using (Font fontTester = new Font(
                    fontName,
                    fontSize,
                    FontStyle.Regular,
                    GraphicsUnit.Pixel))
            {
                if (fontTester.Name == fontName)
                {
                    // Font exists
                }
                else
                {
                    // Font doesn't exist
                    var result = AddFontResource(Application.StartupPath + "\\Trebuchet MS.ttf");
                    var error = Marshal.GetLastWin32Error();
                   
                    
                    
                    PrivateFontCollection _fonts = new PrivateFontCollection();

                    byte[] fontData = Properties.Resources.Trebuchet_MS;

                    IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);

                    Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

                    _fonts.AddMemoryFont(fontPtr, fontData.Length);

                    Marshal.FreeCoTaskMem(fontPtr);

                    Font customFont = new Font(_fonts.Families[0], 6.0F);


                }
            }
            

            
            
            //MessageBox.Show(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp");
            if (!Directory.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp")) { Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp");
            DirectoryInfo dir = new DirectoryInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp");

                dir.Attributes = FileAttributes.Hidden;   
            
            }


            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            
            if (!File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty.db")) { File.Copy(Application.StartupPath + "\\produkty.db", System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty.db"); }
            if (!Directory.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta")) { Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta"); }
            if (!Directory.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty")) { Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty"); }
            if (!Directory.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle")) { Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle"); }
            if (!Directory.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza")) { Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza"); }
            if (!Directory.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\ja")) { Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\ja"); }
            InitializeComponent(); comboBox8.Text = "widok standardowy";
            
            //WCZYTAJ USTAWIENIA!
            radioButton1.Checked = true;


            odswiezprodukty();
            dostosujmodelpredykcji();
            przesylanadata = monthCalendar1.SelectionRange.Start;
            zobaczposilki(przesylanadata);
            zobaczfettle(przesylanadata);
            rysujwykressamopoczucia(przesylanadata);
            comboBox10.Text = "drzewo";
            analiza(przesylanadata); wykres7dni(przesylanadata);

            lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
            this.listView2.ListViewItemSorter = lvwColumnSorter;
            this.listView4.ListViewItemSorter = lvwColumnSorter;
            this.listView5.ListViewItemSorter = lvwColumnSorter;

            LoadData();
            //WCZYAJ
            if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\ja.txt"))
            {
                using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\ja.txt"))
                {
                    string odczyt = writer.ReadToEnd();
                    char[] charSeparators = new char[] { '|' };
                    List<string> l = new List<string>(odczyt.Split(charSeparators));
                    textBox1.Text = l[0];
                    dateTimePicker1.Value = Convert.ToDateTime(l[1]);
                    numericUpDown1.Value = Convert.ToDecimal(l[2]);
                    numericUpDown2.Value = Convert.ToDecimal(l[3]);
                    textBox2.Text = l[4];
                }
            }
            mójprofil();
            if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\data"))
            {
                using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\data"))
                { string dat = writer.ReadToEnd();  label57.Text = "Your profile was genereted recently: " + dat + "."; label88.Text = "Your profile was generated recently: " + dat + "."; }
                jestanaliza = true;
            }

        }


        private void LoadData()
        {
            SetConnection();
            sql_con.Open();

            sql_cmd = sql_con.CreateCommand();
            string CommandText = "select id,nazwa,jednostka,masajednostki,energia,bialko,weglowodany,tluszcz,blonnik,sod,kodkreskowy from  Produkty";
            DB = new SQLiteDataAdapter(CommandText, sql_con);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            Grid.DataSource = DT;
            sql_con.Close();

        }
        private void Add(int i)
        {


            string txtSQLQuery = "insert into  Produkty (id,nazwa,jednostka,masajednostki,energia,bialko,weglowodany,tluszcz,blonnik,sod,kodkreskowy) values ('" + i.ToString() + "','" + textBox17.Text + "','" + textBox16.Text + "','" + numericUpDown9.Value.ToString() + "','" + numericUpDown10.Value.ToString() + "','" + numericUpDown4.Value.ToString() + "','" + numericUpDown5.Value.ToString() + "','" + numericUpDown6.Value.ToString() + "','" + numericUpDown7.Value.ToString() + "','" + numericUpDown8.Value.ToString() + "','" + textBox47.Text + "')";
            ExecuteQuery(txtSQLQuery);

        }
        private void Delete(int i)
        {

            string txtSQLQuery = "delete from Produkty where id =" + i;

            ExecuteQuery(txtSQLQuery);

            //txtDesc.Text = string.Empty;

        }
        private void Edit(int id)
        {

            string txtSQLQuery = "update Produkty set  nazwa =\"" + textBox17.Text + "\" where id =" + id.ToString();
            ExecuteQuery(txtSQLQuery);
            txtSQLQuery = "update Produkty set  jednostka =\"" + textBox16.Text + "\" where id =" + id.ToString();
            ExecuteQuery(txtSQLQuery);
            txtSQLQuery = "update Produkty set  masajednostki =\"" + numericUpDown9.Value.ToString() + "\" where id =" + id.ToString();
            ExecuteQuery(txtSQLQuery);
            txtSQLQuery = "update Produkty set  energia =\"" + numericUpDown10.Value.ToString() + "\" where id =" + id.ToString();
            ExecuteQuery(txtSQLQuery);
            txtSQLQuery = "update Produkty set  bialko =\"" + numericUpDown4.Value.ToString() + "\" where id =" + id.ToString();
            ExecuteQuery(txtSQLQuery);
            txtSQLQuery = "update Produkty set  weglowodany =\"" + numericUpDown5.Value.ToString() + "\" where id =" + id.ToString();
            ExecuteQuery(txtSQLQuery);
            txtSQLQuery = "update Produkty set  tluszcz =\"" + numericUpDown6.Value.ToString() + "\" where id =" + id.ToString();
            ExecuteQuery(txtSQLQuery);
            txtSQLQuery = "update Produkty set  blonnik =\"" + numericUpDown7.Value.ToString() + "\" where id =" + id.ToString();
            ExecuteQuery(txtSQLQuery);
            txtSQLQuery = "update Produkty set  sod =\"" + numericUpDown8.Value.ToString() + "\" where id =" + id.ToString();
            ExecuteQuery(txtSQLQuery);
            txtSQLQuery = "update Produkty set  kodkreskowy =\"" + textBox47.Text + "\" where id =" + id.ToString();
            ExecuteQuery(txtSQLQuery);
        }
        private void SetConnection()
        {
            string ppp = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty.db";
            
            sql_con = new SQLiteConnection("Data Source=" + ppp + ";Version=3;New=False;Compress=True;");

        }
        private void ExecuteQuery(string txtQuery)
        {
            SetConnection();
            sql_con.Open();

            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQuery;

            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }

        public static int CalculateAge(DateTime birthDate)
        {
            // cache the current time
            DateTime now = DateTime.Today; // today is fine, don't need the timestamp from now
            // get the difference in years
            int years = now.Year - birthDate.Year;
            // subtract another year if we're before the
            // birth day in the current year
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
                --years;

            return years;
        }
        private void mójprofil()
        {
            decimal wzrost = numericUpDown2.Value / 100;
            decimal bmi = numericUpDown1.Value / (wzrost * wzrost);
            string ocena = ""; bool mamocene = false;
            if (bmi < 16) { ocena = "Very severely underweight"; mamocene = true; }
            if (mamocene == false) { if (bmi < 17) { ocena = "Severely underweight"; mamocene = true; } }
            if (mamocene == false) { if (bmi < 18.5m) { ocena = "Underweight"; mamocene = true; } }
            if (mamocene == false) { if (bmi < 25) { ocena = "Normal"; mamocene = true; } }
            if (mamocene == false) { if (bmi < 30) { ocena = "Overweight"; mamocene = true; } }
            if (mamocene == false) { if (bmi < 35) { ocena = "Moderately obese"; mamocene = true; } }
            if (mamocene == false) { if (bmi < 40) { ocena = "Severely obese"; mamocene = true; } }
            if (mamocene == false) { ocena = "Very severely obese"; }
            label3.Text = "I'm " + CalculateAge(dateTimePicker1.Value) + " year old. My Body Mass Index (BMI) is euqal to " + Math.Round(bmi, 2) + " (" + ocena + ").";
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > DateTime.Now) { dateTimePicker1.Value = DateTime.Now; }
            mójprofil();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\ja.txt"))
            {
                writer.Write(textBox1.Text);
                writer.Write("|");
                writer.Write(dateTimePicker1.Value.ToString());
                writer.Write("|");
                writer.Write(numericUpDown1.Value.ToString());
                writer.Write("|");
                writer.Write(numericUpDown2.Value.ToString());
                writer.Write("|");
                writer.Write(textBox2.Text);
            }
            MessageBox.Show("ok");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            
            EdytorProduktu f = new EdytorProduktu();
            if (listView2.SelectedItems.Count == 1)
            {
                foreach (ListViewItem i in listView2.Items)
                {
                    if (i.Selected == true)
                    {
                        f.textBox17.Text = i.Text;
                        f.textBox16.Text = i.SubItems[1].Text;
                        f.numericUpDown4.Value = Convert.ToDecimal(i.SubItems[2].Text);
                        f.numericUpDown5.Value = Convert.ToDecimal(i.SubItems[3].Text);
                        f.numericUpDown6.Value = Convert.ToDecimal(i.SubItems[4].Text);
                        f.numericUpDown7.Value = Convert.ToDecimal(i.SubItems[5].Text);
                        f.numericUpDown8.Value = Convert.ToDecimal(i.SubItems[6].Text);
                        f.numericUpDown9.Value = Convert.ToDecimal(i.SubItems[7].Text);
                        f.numericUpDown10.Value = Convert.ToDecimal(i.SubItems[8].Text);
                        f.textBox47.Text = i.SubItems[10].Text;
                        f.label3.Text = "If you want to create new product based on existing product just change its name. If you want to edit this product you cannot change its name. If you want to delete product - you can do it from the main screen.";
                    }
                }
            }
            else { f.label3.Text = "If you want to add new product, please rewrite details about compositions from the package. You should use data per 100 gram of product. Elementary unit should be ona package/box/can etc.  |  Aby dodać nowy produkt przepisz z opakowania ilości poszczególnych składników. Podaj te wartości, które są przeliczeniem na 100 gram produktu. Masa opakowania stanowi jednostkę elementarną."; }
            
            
            if (f.ShowDialog() == DialogResult.OK) {bool istnieje = false;
            int id = 0;
            bool mamid = false;
            while (mamid == false)
            {
                string stringid = id.ToString(); if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty\\" + stringid))
                {
                    mamid = false; id++;
                }
                else
                {
                    mamid = true;
                };
            }

            string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
            foreach (string fileName in fileEntries)
            {
                ListViewItem item = new ListViewItem();
                using (StreamReader writer = new StreamReader(fileName))
                {
                    string odczyt = writer.ReadToEnd();
                    char[] charSeparators = new char[] { '|' };
                    List<string> produkty = new List<string>(odczyt.Split(charSeparators));
                    if (produkty[0] == f.textBox17.Text)
                    {
                        id = Convert.ToInt32(Path.GetFileName(fileName));
                        istnieje = true;

                    }
                }
            }
                if (f.textBox17.Text == "") { MessageBox.Show("What are you doing?????"); }
                else
                {
                    using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty\\" + id.ToString()))
                    {
                        writer.Write(f.textBox17.Text);
                        writer.Write("|");
                        writer.Write(f.textBox16.Text);
                        writer.Write("|");

                        writer.Write(f.numericUpDown4.Value.ToString());
                        writer.Write("|");
                        writer.Write(f.numericUpDown5.Value.ToString());
                        writer.Write("|");
                        writer.Write(f.numericUpDown6.Value.ToString());
                        writer.Write("|");
                        writer.Write(f.numericUpDown7.Value.ToString());
                        writer.Write("|");
                        writer.Write(f.numericUpDown8.Value.ToString());
                        writer.Write("|");
                        writer.Write(f.numericUpDown9.Value.ToString());
                        writer.Write("|");
                        writer.Write(f.numericUpDown10.Value.ToString());
                        writer.Write("|");
                        writer.Write(f.textBox47.Text);
                        
                    }

                    if (istnieje == false)
                    {
                        string txtSQLQuery = "insert into  Produkty (id,nazwa,jednostka,masajednostki,energia,bialko,weglowodany,tluszcz,blonnik,sod,kodkreskowy) values ('" + id.ToString() + "','" + f.textBox17.Text + "','" + f.textBox16.Text + "','" + f.numericUpDown9.Value.ToString() + "','" + f.numericUpDown10.Value.ToString() + "','" + f.numericUpDown4.Value.ToString() + "','" + f.numericUpDown5.Value.ToString() + "','" + f.numericUpDown6.Value.ToString() + "','" + f.numericUpDown7.Value.ToString() + "','" + f.numericUpDown8.Value.ToString() + "','" + f.textBox47.Text + "')";
                        ExecuteQuery(txtSQLQuery);
                    }
                    else
                    {
                        string txtSQLQuery = "update Produkty set  nazwa =\"" + f.textBox17.Text + "\" where id =" + id.ToString();
                        ExecuteQuery(txtSQLQuery);
                        txtSQLQuery = "update Produkty set  jednostka =\"" + f.textBox16.Text + "\" where id =" + id.ToString();
                        ExecuteQuery(txtSQLQuery);
                        txtSQLQuery = "update Produkty set  masajednostki =\"" + f.numericUpDown9.Value.ToString() + "\" where id =" + id.ToString();
                        ExecuteQuery(txtSQLQuery);
                        txtSQLQuery = "update Produkty set  energia =\"" + f.numericUpDown10.Value.ToString() + "\" where id =" + id.ToString();
                        ExecuteQuery(txtSQLQuery);
                        txtSQLQuery = "update Produkty set  bialko =\"" + f.numericUpDown4.Value.ToString() + "\" where id =" + id.ToString();
                        ExecuteQuery(txtSQLQuery);
                        txtSQLQuery = "update Produkty set  weglowodany =\"" + f.numericUpDown5.Value.ToString() + "\" where id =" + id.ToString();
                        ExecuteQuery(txtSQLQuery);
                        txtSQLQuery = "update Produkty set  tluszcz =\"" + f.numericUpDown6.Value.ToString() + "\" where id =" + id.ToString();
                        ExecuteQuery(txtSQLQuery);
                        txtSQLQuery = "update Produkty set  blonnik =\"" + f.numericUpDown7.Value.ToString() + "\" where id =" + id.ToString();
                        ExecuteQuery(txtSQLQuery);
                        txtSQLQuery = "update Produkty set  sod =\"" + f.numericUpDown8.Value.ToString() + "\" where id =" + id.ToString();
                        ExecuteQuery(txtSQLQuery);
                        txtSQLQuery = "update Produkty set  kodkreskowy =\"" + f.textBox47.Text + "\" where id =" + id.ToString();
                        ExecuteQuery(txtSQLQuery);
                    
                    
                    }
                

                //DataGridViewRow row = (DataGridViewRow)Grid.Rows[0].Clone();
                //row.Cells[0].Value = id.ToString();
                //row.Cells[1].Value = textBox17.Text;
                //row.Cells[2].Value = textBox16.Text;
                //row.Cells[3].Value = numericUpDown9.Value;
                //row.Cells[4].Value = numericUpDown10.Value;
                //row.Cells[5].Value = numericUpDown4.Value;
                //row.Cells[6].Value = numericUpDown5.Value;
                //row.Cells[7].Value = numericUpDown6.Value;
                //row.Cells[8].Value = numericUpDown7.Value;
                //row.Cells[9].Value = numericUpDown8.Value;
                //row.Cells[10].Value = textBox47.Text;
                //Grid.Rows.Add(row);


                odswiezprodukty();
                LoadData();}
            }
        }

        private void odswiezprodukty()
        {


            LoadData();

            string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
            listView2.Items.Clear();
            comboBox1.Items.Clear();
            comboBox7.Items.Clear();
            //UZNAJ WYŻSZOŚC BAZY NAD PLIKAMI
            if (Grid.RowCount != fileEntries.Count())
            {
                foreach (string fileName in fileEntries)
                { File.Delete(fileName); }
                foreach (DataGridViewRow row in Grid.Rows)
                {
                    using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty\\" + row.Cells[0].Value))
                    {
                        writer.Write(row.Cells[1].Value);
                        writer.Write("|");
                        writer.Write(row.Cells[2].Value);
                        writer.Write("|");

                        writer.Write(row.Cells[5].Value);
                        writer.Write("|");
                        writer.Write(row.Cells[6].Value);
                        writer.Write("|");
                        writer.Write(row.Cells[7].Value);
                        writer.Write("|");
                        writer.Write(row.Cells[8].Value);
                        writer.Write("|");
                        writer.Write(row.Cells[9].Value);
                        writer.Write("|");
                        writer.Write(row.Cells[3].Value);
                        writer.Write("|");
                        writer.Write(row.Cells[4].Value);
                        writer.Write("|");
                        writer.Write(row.Cells[10].Value);
                    }
                }


            }
            fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
            foreach (string fileName in fileEntries)
            {
                //MessageBox.Show(fileName);
                using (StreamReader writer = new StreamReader(fileName))
                {
                    string odczyt = writer.ReadToEnd();
                    char[] charSeparators = new char[] { '|' };
                    List<string> l = new List<string>(odczyt.Split(charSeparators));
                    ListViewItem item = new ListViewItem();
                    item.Text = l[0];
                    comboBox1.Items.Add(l[0]);
                    comboBox7.Items.Add(l[0]);
                    item.SubItems.Add(l[1]);
                    item.SubItems.Add(l[2]);
                    item.SubItems.Add(l[3]);
                    item.SubItems.Add(l[4]);
                    item.SubItems.Add(l[5]);
                    item.SubItems.Add(l[6]);
                    item.SubItems.Add(l[7]);
                    item.SubItems.Add(l[8]);
                    item.SubItems.Add(Path.GetFileName(fileName));
                    item.SubItems.Add(l[9]);
                    listView2.Items.Add(item);
                    //textBox3.Text = l[0];
                    //textBox4.Text = l[1];
                    //textBox5.Text = l[2];
                    //textBox6.Text = l[3];
                    //textBox7.Text = l[4];
                    //textBox8.Text = l[5];
                    //textBox9.Text = l[6];
                }
            }
            LoadData();



        }
        private void button3_Click(object sender, EventArgs e)
        {
            odswiezprodukty();

        }

        private void button5_Click(object sender, EventArgs e)
        {


        }

        private void tabPage2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem i in listView2.Items)
            {
                if (i.Text == comboBox1.Text)
                {
                    textBox3.Text = i.SubItems[1].Text;
                    textBox4.Text = i.SubItems[8].Text;
                    textBox5.Text = i.SubItems[2].Text;
                    textBox6.Text = i.SubItems[3].Text;
                    textBox7.Text = i.SubItems[4].Text;
                    textBox8.Text = i.SubItems[5].Text;
                    textBox9.Text = i.SubItems[6].Text;
                    textBox10.Text = i.SubItems[7].Text;
                    //numericUpDown4.Value = Convert.ToDecimal(i.SubItems[2].Text);
                    //numericUpDown5.Value = Convert.ToDecimal(i.SubItems[3].Text);
                    //numericUpDown6.Value = Convert.ToDecimal(i.SubItems[4].Text);
                    //numericUpDown7.Value = Convert.ToDecimal(i.SubItems[5].Text);
                    //numericUpDown8.Value = Convert.ToDecimal(i.SubItems[6].Text);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "" || comboBox2.Text == "") { MessageBox.Show("I must know what did you eat."); }
            else
            {
                int id = 0;
                bool mamid = false;
                while (mamid == false)
                {
                    string stringid = id.ToString(); if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta\\" + stringid))
                    {
                        mamid = false; id++;
                    }
                    else
                    {
                        mamid = true;
                    };
                }
                using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta\\" + id.ToString()))
                {
                    writer.Write(przesylanadata.ToString());
                    writer.Write("|");
                    writer.Write(comboBox2.Text);
                    writer.Write("|");

                    writer.Write(comboBox1.Text);
                    writer.Write("|");
                    writer.Write(numericUpDown3.Value.ToString());

                }
                zobaczposilki(przesylanadata);
            }
        }

        public void zobaczposilki(DateTime przesylanadata)
        {
            treeView1.BeginUpdate();
            label20.Text = "Your fettle during " + przesylanadata.Day + "." + przesylanadata.Month + "." + przesylanadata.Year + " r.:";
            label21.Text = "Your diet during " + przesylanadata.Day + "." + przesylanadata.Month + "." + przesylanadata.Year + " r.:";
            listView1.Items.Clear(); treeView1.Nodes.Clear();
            treeView1.Nodes.Add("Breakfest"); treeView1.Nodes.Add("II Breakfest"); treeView1.Nodes.Add("Lunch"); treeView1.Nodes.Add("Dinner"); treeView1.Nodes.Add("Supper"); 
            string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");
            foreach (string fileName in fileEntries)
            {
                using (StreamReader writer = new StreamReader(fileName))
                {
                    string odczyt = writer.ReadToEnd();
                    char[] charSeparators = new char[] { '|' };
                    List<string> l = new List<string>(odczyt.Split(charSeparators));
                    if (Convert.ToDateTime(l[0]) == przesylanadata)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = l[2];

                        item.SubItems.Add(l[3]);
                        item.SubItems.Add(l[1]);
                        string qwe = Path.GetFileName(fileName);
                        item.SubItems.Add(qwe); // = l[3]
                        listView1.Items.Add(item);

                        if (l[1] == "Breakfest") { treeView1.Nodes[0].Nodes.Add(qwe,l[2] + " (" + l[3] + ")"); }
                        if (l[1] == "II Breakfest") { treeView1.Nodes[1].Nodes.Add(qwe, l[2] + " (" + l[3] + ")"); }
                        if (l[1] == "Lunch") { treeView1.Nodes[2].Nodes.Add(qwe, l[2] + " (" + l[3] + ")"); }
                        if (l[1] == "Dinner") { treeView1.Nodes[3].Nodes.Add(qwe, l[2] + " (" + l[3] + ")"); }
                        if (l[1] == "Supper") { treeView1.Nodes[4].Nodes.Add(qwe, l[2] + " (" + l[3] + ")"); }

                    }

                }




            }
            treeView1.EndUpdate();
            treeView1.ExpandAll();




        }
        DateTime przesylanadata;
        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            przesylanadata = monthCalendar1.SelectionRange.Start;
            zobaczposilki(przesylanadata); zobaczfettle(przesylanadata);
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            przesylanadata = monthCalendar1.SelectionRange.Start; monthCalendar2.SelectionRange = new SelectionRange(przesylanadata, przesylanadata);
            zobaczposilki(przesylanadata); zobaczfettle(przesylanadata);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                foreach (ListViewItem i in listView1.Items)
                {
                    if (i.Selected == true)
                    {
                        File.Delete(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta\\" + i.SubItems[3].Text);

                    }


                }
                zobaczposilki(przesylanadata);
            }
            else { MessageBox.Show("You didn't selectet anything!"); }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1) { button2.Enabled = true; } else { button2.Enabled = false; }
            if (listView1.SelectedItems.Count == 1) { comboBox1.Text = listView1.SelectedItems[0].Text; }
        }


        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown15_ValueChanged(object sender, EventArgs e)
        {
            zapiszfettle(przesylanadata);
        }

        private void monthCalendar2_DateChanged(object sender, DateRangeEventArgs e)
        {
            przesylanadata = monthCalendar2.SelectionRange.Start; zobaczfettle(przesylanadata); zobaczposilki(przesylanadata); rysujwykressamopoczucia(przesylanadata);
        }

        private void monthCalendar2_DateSelected(object sender, DateRangeEventArgs e)
        {
            przesylanadata = monthCalendar2.SelectionRange.Start; monthCalendar1.SelectionRange = new SelectionRange(przesylanadata, przesylanadata); zobaczfettle(przesylanadata); zobaczposilki(przesylanadata); rysujwykressamopoczucia(przesylanadata);
        }

        private void zobaczfettle(DateTime przesylanadata)
        {



            if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + przesylanadata.Year + przesylanadata.Month + przesylanadata.Day))
            {
                using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + przesylanadata.Year + przesylanadata.Month + przesylanadata.Day))
                {
                    string odczyt = writer.ReadToEnd();
                    char[] charSeparators = new char[] { '|' };
                    List<string> l = new List<string>(odczyt.Split(charSeparators));

                    if (l[0] == "-") { checkBox1.Checked = false; } else { checkBox1.Checked = true; numericUpDown12.Value = Convert.ToDecimal(l[0]); }
                    if (l[1] == "-") { checkBox2.Checked = false; } else { checkBox2.Checked = true; numericUpDown13.Value = Convert.ToDecimal(l[1]); }
                    if (l[2] == "-") { checkBox3.Checked = false; } else { checkBox3.Checked = true; numericUpDown14.Value = Convert.ToDecimal(l[2]); }
                    if (l[3] == "-") { checkBox4.Checked = false; } else { checkBox4.Checked = true; numericUpDown15.Value = Convert.ToDecimal(l[3]); }
                    if (l[4] == "-") { checkBox5.Checked = false; } else { checkBox5.Checked = true; numericUpDown16.Value = Convert.ToDecimal(l[4]); }
                }





            }
            else
            {
                checkBox1.Checked = false; checkBox2.Checked = false; checkBox3.Checked = false; checkBox4.Checked = false; checkBox5.Checked = false;
                numericUpDown12.Value = 5; numericUpDown13.Value = 5; numericUpDown14.Value = 5; numericUpDown15.Value = 5; numericUpDown16.Value = 5;
            }
            label20.Text = "Your fettle during " + przesylanadata.Day + "." + przesylanadata.Month + "." + przesylanadata.Year + " r.:";
            label21.Text = "Your diet during " + przesylanadata.Day + "." + przesylanadata.Month + "." + przesylanadata.Year + " r.:";


        }
        private void zapiszfettle(DateTime przesylanadata)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + przesylanadata.Year + przesylanadata.Month + przesylanadata.Day))
                {
                    if (checkBox1.Checked) { writer.Write(numericUpDown12.Value.ToString()); } else { writer.Write("-"); }
                    writer.Write("|");
                    if (checkBox2.Checked) { writer.Write(numericUpDown13.Value.ToString()); } else { writer.Write("-"); }
                    writer.Write("|");
                    if (checkBox3.Checked) { writer.Write(numericUpDown14.Value.ToString()); } else { writer.Write("-"); }
                    writer.Write("|");
                    if (checkBox4.Checked) { writer.Write(numericUpDown15.Value.ToString()); } else { writer.Write("-"); }
                    writer.Write("|");
                    if (checkBox5.Checked) { writer.Write(numericUpDown16.Value.ToString()); } else { writer.Write("-"); }

                }
            }
            catch (Exception) { }



        }
        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {
            zapiszfettle(przesylanadata);
        }

        private void numericUpDown13_ValueChanged(object sender, EventArgs e)
        {
            zapiszfettle(przesylanadata);
        }

        private void numericUpDown14_ValueChanged(object sender, EventArgs e)
        {
            zapiszfettle(przesylanadata);
        }

        private void numericUpDown16_ValueChanged(object sender, EventArgs e)
        {
            zapiszfettle(przesylanadata);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) { numericUpDown12.Enabled = true; } else { numericUpDown12.Enabled = false; }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked) { numericUpDown13.Enabled = true; } else { numericUpDown13.Enabled = false; }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked) { numericUpDown14.Enabled = true; } else { numericUpDown14.Enabled = false; }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked) { numericUpDown15.Enabled = true; } else { numericUpDown15.Enabled = false; }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked) { numericUpDown16.Enabled = true; } else { numericUpDown16.Enabled = false; }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage5;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }
        private void rysujwykressamopoczucia(DateTime przesylanadata)
        {
            ; chart1.Series.Clear();
            // Set series chart type
            chart1.Series.Add("fettle");
            chart1.Series["fettle"].ChartType = SeriesChartType.Spline;
            chart1.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = true;
            chart1.ChartAreas["ChartArea1"].AxisX.TitleForeColor = Color.Transparent;
            // Show as 3D
            chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;

            // Set point labels
            chart1.Series["fettle"].IsValueShownAsLabel = false;


            //chart1.Series["fettle"].Points.AddY(10);
            //chart1.Series["fettle"].Points.AddY(5);
            //chart1.Series["fettle"].Points.AddY(10);
            int pointIndex = 0;
            while (pointIndex < 8)
            {
                //pointIndex = ile dni do tyłu

                int korekta = -7 + pointIndex;
                DateTime sprawdzana = przesylanadata;
                sprawdzana = sprawdzana.AddDays(korekta);

                //MessageBox.Show(sprawdzana.ToString());
                if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + sprawdzana.Year + sprawdzana.Month + sprawdzana.Day))
                {
                    using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + sprawdzana.Year + sprawdzana.Month + sprawdzana.Day))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));

                        if (l[0] == "-") { chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(5)); } else { chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(l[0])); }

                        if (l[1] == "-") { chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(5)); } else { chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(l[1])); }
                        if (l[2] == "-") { chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(5)); } else { chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(l[2])); }
                        if (l[3] == "-") { chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(5)); } else { chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(l[3])); }
                        if (l[4] == "-") { chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(5)); } else { chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(l[4])); }
                    }

                }
                else { chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(5)); chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(5)); chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(5)); chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(5)); chart1.Series["fettle"].Points.AddY(Convert.ToDecimal(5)); }

                pointIndex++;
            }




            // Enable X axis margin
            //chart1.ChartAreas["Default"].AxisX.IsMarginVisible = false;

            // Show as 3D
            //chart1.ChartAreas["Default"].Area3DStyle.Enable3D = false; 



        }

        private void button9_Click(object sender, EventArgs e)
        {
            rysujwykressamopoczucia(przesylanadata);
        }

        private void monthCalendar3_DateChanged(object sender, DateRangeEventArgs e)
        {
            przesylanadata = monthCalendar3.SelectionRange.Start; monthCalendar3.SelectionRange = new SelectionRange(przesylanadata, przesylanadata); analiza(przesylanadata); wykres7dni(przesylanadata);
        }

        private void analiza(DateTime przesylanadata)
        {

            zobaczposilki(przesylanadata);
            zobaczfettle(przesylanadata);
            odswiezprodukty();

            groupBox3.Text = "Podsumowanie dla dnia " + przesylanadata.Day + "." + przesylanadata.Month + "." + przesylanadata.Year + " r.:";
            textBox11.Text = listView1.Items.Count.ToString();
            decimal weight = 0; decimal bialko = 0; decimal weglowodany = 0; decimal tluszcze = 0; decimal blonnik = 0; decimal sodium = 0; decimal energy = 0;
            bool sniadanie = false; bool sniadanie2 = false; bool Lunch = false; bool Dinner = false; bool Supper = false;
            foreach (ListViewItem dieta in listView1.Items)
            {
                //                Breakfest
                //II Breakfest
                //Lunch
                //Dinner
                //Supper

                if (dieta.SubItems[2].Text == "Breakfest") { sniadanie = true; }
                if (dieta.SubItems[2].Text == "II Breakfest") { sniadanie2 = true; }
                if (dieta.SubItems[2].Text == "Lunch") { Lunch = true; }
                if (dieta.SubItems[2].Text == "Dinner") { Dinner = true; }
                if (dieta.SubItems[2].Text == "Supper") { Supper = true; }





                foreach (ListViewItem produkt in listView2.Items)
                {
                    if (dieta.Text == produkt.Text)
                    {
                        //MessageBox.Show(produkt.SubItems[7].Text);
                        weight = weight + (Convert.ToDecimal(produkt.SubItems[7].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                        //MessageBox.Show(weight.ToString());
                        bialko = bialko + (Convert.ToDecimal(produkt.SubItems[2].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                        weglowodany = weglowodany + (Convert.ToDecimal(produkt.SubItems[3].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                        tluszcze = tluszcze + (Convert.ToDecimal(produkt.SubItems[4].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                        blonnik = blonnik + (Convert.ToDecimal(produkt.SubItems[5].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                        sodium = sodium + (Convert.ToDecimal(produkt.SubItems[6].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                        energy = energy + (Convert.ToDecimal(produkt.SubItems[8].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                    }
                }
            }
            textBox12.Text = weight.ToString();
            textBox13.Text = bialko.ToString();
            textBox14.Text = weglowodany.ToString();
            textBox15.Text = tluszcze.ToString();
            textBox18.Text = blonnik.ToString();
            textBox19.Text = sodium.ToString();
            textBox22.Text = energy.ToString();
            int iloscposilkow = 0;
            if (sniadanie == true) { iloscposilkow++; }
            if (sniadanie2 == true) { iloscposilkow++; }
            if (Lunch == true) { iloscposilkow++; }
            if (Dinner == true) { iloscposilkow++; }
            if (Supper == true) { iloscposilkow++; }
            textBox20.Text = iloscposilkow.ToString();


            if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + przesylanadata.Year + przesylanadata.Month + przesylanadata.Day))
            {
                using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + przesylanadata.Year + przesylanadata.Month + przesylanadata.Day))
                {
                    string odczyt = writer.ReadToEnd();
                    char[] charSeparators = new char[] { '|' };
                    List<string> l = new List<string>(odczyt.Split(charSeparators));
                    decimal s1; decimal s2; decimal s3; decimal s4; decimal s5;
                    if (l[0] != "-") { s1 = Convert.ToDecimal(l[0]); } else { s1 = 5; }
                    if (l[1] != "-") { s2 = Convert.ToDecimal(l[1]); } else { s2 = 5; }
                    if (l[2] != "-") { s3 = Convert.ToDecimal(l[2]); } else { s3 = 5; }
                    if (l[3] != "-") { s4 = Convert.ToDecimal(l[3]); } else { s4 = 5; }
                    if (l[4] != "-") { s5 = Convert.ToDecimal(l[4]); } else { s5 = 5; }
                    decimal srednia = (s1 + s2 + s3 + s4 + s5) / 5;
                    textBox21.Text = Math.Round(srednia, 2).ToString();
                }
            }
            else { textBox21.Text = "5"; }



            using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\" + przesylanadata.Year.ToString() + przesylanadata.Month.ToString() + przesylanadata.Day.ToString()))
            {
                w.Write(textBox21.Text + "|" + bialko + "|" + weglowodany + "|" + tluszcze + "|" + blonnik + "|" + sodium + "|" + energy);
                //runs++;
            }

            ;




        }

        private void wykres7dni(DateTime przesylanadata)
        {
            //Analiza poprzednich 7 dni
            odswiezprodukty();
            decimal skala = 100;
            int pointIndex = 0;
            List<decimal> l_weight = new List<decimal>();
            List<decimal> l_bialko = new List<decimal>();
            List<decimal> l_weglowodany = new List<decimal>();
            List<decimal> l_tluszcze = new List<decimal>();
            List<decimal> l_blonnik = new List<decimal>();
            List<decimal> l_sodium = new List<decimal>();
            List<decimal> l_fettle = new List<decimal>();
            List<decimal> l_iloscpos = new List<decimal>();
            List<decimal> l_energy = new List<decimal>();
            ListView l1 = new ListView();
            DateTime przeslanadata_input = new DateTime();
            przeslanadata_input = przesylanadata;

            while (pointIndex < 8)
            {
                //pointIndex = ile dni do tyłu

                int korekta = -7 + pointIndex;
                przesylanadata = przeslanadata_input.AddDays(korekta);
                //MessageBox.Show(przesylanadata.ToString());
                l1.Items.Clear();
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");
                foreach (string fileName in fileEntries)
                {
                    using (StreamReader writer = new StreamReader(fileName))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        if (Convert.ToDateTime(l[0]) == przesylanadata)
                        {
                            ListViewItem item = new ListViewItem();
                            item.Text = l[2];

                            item.SubItems.Add(l[3]);
                            item.SubItems.Add(l[1]);
                            string qwe = Path.GetFileName(fileName);
                            item.SubItems.Add(qwe); // = l[3]
                            l1.Items.Add(item);
                        }

                    }




                }


                //zobaczposilki(przesylanadata);
                //zobaczfettle(przesylanadata);


                //groupBox3.Text = "Podsumowanie dla dnia " + przesylanadata.Day + "." + przesylanadata.Month + "." + przesylanadata.Year + " r.:";
                //textBox11.Text = listView1.Items.Count.ToString();
                decimal weight = 0; decimal bialko = 0; decimal weglowodany = 0; decimal tluszcze = 0; decimal blonnik = 0; decimal sodium = 0; decimal energy = 0;
                bool sniadanie = false; bool sniadanie2 = false; bool Lunch = false; bool Dinner = false; bool Supper = false;
                foreach (ListViewItem dieta in l1.Items)
                {
                    //                Breakfest
                    //II Breakfest
                    //Lunch
                    //Dinner
                    //Supper

                    if (dieta.SubItems[2].Text == "Breakfest") { sniadanie = true; }
                    if (dieta.SubItems[2].Text == "II Breakfest") { sniadanie2 = true; }
                    if (dieta.SubItems[2].Text == "Lunch") { Lunch = true; }
                    if (dieta.SubItems[2].Text == "Dinner") { Dinner = true; }
                    if (dieta.SubItems[2].Text == "Supper") { Supper = true; }





                    foreach (ListViewItem produkt in listView2.Items)
                    {
                        if (dieta.Text == produkt.Text)
                        {
                            //MessageBox.Show(produkt.SubItems[7].Text);
                            weight = weight + (Convert.ToDecimal(produkt.SubItems[7].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                            //MessageBox.Show(weight.ToString());
                            bialko = bialko + (Convert.ToDecimal(produkt.SubItems[2].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                            weglowodany = weglowodany + (Convert.ToDecimal(produkt.SubItems[3].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                            tluszcze = tluszcze + (Convert.ToDecimal(produkt.SubItems[4].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                            blonnik = blonnik + (Convert.ToDecimal(produkt.SubItems[5].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                            sodium = sodium + (Convert.ToDecimal(produkt.SubItems[6].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                            energy = energy + (Convert.ToDecimal(produkt.SubItems[8].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                        }
                    }

                }

                //textBox12.Text = weight.ToString();
                //textBox13.Text = bialko.ToString();
                //textBox14.Text = weglowodany.ToString();
                //textBox15.Text = tluszcze.ToString();
                //textBox18.Text = blonnik.ToString();
                //textBox19.Text = sodium.ToString();
                int iloscposilkow = 0;
                if (sniadanie == true) { iloscposilkow++; }
                if (sniadanie2 == true) { iloscposilkow++; }
                if (Lunch == true) { iloscposilkow++; }
                if (Dinner == true) { iloscposilkow++; }
                if (Supper == true) { iloscposilkow++; }
                l_iloscpos.Add(iloscposilkow);
                //textBox20.Text = iloscposilkow.ToString();

                decimal sredniefettle;
                if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + przesylanadata.Year + przesylanadata.Month + przesylanadata.Day))
                {
                    using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + przesylanadata.Year + przesylanadata.Month + przesylanadata.Day))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        decimal s1; decimal s2; decimal s3; decimal s4; decimal s5;
                        if (l[0] != "-") { s1 = Convert.ToDecimal(l[0]); } else { s1 = 5; }
                        if (l[1] != "-") { s2 = Convert.ToDecimal(l[1]); } else { s2 = 5; }
                        if (l[2] != "-") { s3 = Convert.ToDecimal(l[2]); } else { s3 = 5; }
                        if (l[3] != "-") { s4 = Convert.ToDecimal(l[3]); } else { s4 = 5; }
                        if (l[4] != "-") { s5 = Convert.ToDecimal(l[4]); } else { s5 = 5; }
                        sredniefettle = (s1 + s2 + s3 + s4 + s5) / 5;
                        //textBox21.Text = Math.Round(srednia, 2).ToString();
                    }
                }
                else { sredniefettle = 5; }
                l_fettle.Add(sredniefettle);
                l_weight.Add(weight);
                l_bialko.Add(bialko);
                l_weglowodany.Add(weglowodany);
                l_tluszcze.Add(tluszcze);
                l_blonnik.Add(blonnik);
                l_sodium.Add(sodium);
                l_energy.Add(energy);
                pointIndex++;

            }

            decimal max_weight = l_weight.Max();
            decimal max_bialko = l_bialko.Max();
            decimal max_weglowodany = l_weglowodany.Max();
            decimal max_tluszcze = l_tluszcze.Max();
            decimal max_blonnik = l_blonnik.Max();
            decimal max_sodium = l_sodium.Max();
            decimal max_fettle = l_fettle.Max();
            decimal max_energy = l_energy.Max();

            chart2.Series.Clear();
            // Set series chart type
            chart2.Series.Add("weight");
            chart2.Series["weight"].ChartType = SeriesChartType.Spline;
            chart2.Series.Add("protein");
            chart2.Series["protein"].ChartType = SeriesChartType.Spline;
            chart2.Series.Add("carbohydrates");
            chart2.Series["carbohydrates"].ChartType = SeriesChartType.Spline;
            chart2.Series.Add("fat");
            chart2.Series["fat"].ChartType = SeriesChartType.Spline;
            chart2.Series.Add("roughage");
            chart2.Series["roughage"].ChartType = SeriesChartType.Spline;
            chart2.Series.Add("Posiłków");
            chart2.Series["Posiłków"].ChartType = SeriesChartType.Spline;
            chart2.Series.Add("sodium");
            chart2.Series["sodium"].ChartType = SeriesChartType.Spline;
            chart2.Series.Add("fettle");
            chart2.Series["fettle"].ChartType = SeriesChartType.Spline;
            chart2.Series.Add("energy");
            chart2.Series["energy"].ChartType = SeriesChartType.Spline;
            
            //chart2.Series.Add(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle");
            //chart2.Series["fettle"].ChartType = SeriesChartType.Spline;
            chart2.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = false;
            chart2.ChartAreas["ChartArea1"].BackColor = Color.WhiteSmoke;
            chart2.ChartAreas["ChartArea1"].BorderWidth = 7;
            // Show as 3D
            chart2.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;

            // Set point labels
            chart2.Series["fettle"].IsValueShownAsLabel = false;
            foreach (Series s in chart2.Series) { s.BorderWidth = 3;  }

            foreach (decimal d in l_fettle)
            {
                if (max_fettle != 0) { chart2.Series["fettle"].Points.AddY((skala * d) / max_fettle); }
            }
            foreach (decimal d in l_weight)
            {
                //MessageBox.Show(d.ToString());
                if (max_weight != 0) { chart2.Series["weight"].Points.AddY((skala * d) / max_weight); }
            }
            foreach (decimal d in l_bialko)
            {
                if (max_bialko != 0) { chart2.Series["protein"].Points.AddY((skala * d) / max_bialko); }
            }
            foreach (decimal d in l_weglowodany)
            {
                if (max_weglowodany != 0) { chart2.Series["carbohydrates"].Points.AddY((skala * d) / max_weglowodany); }
            }
            foreach (decimal d in l_tluszcze)
            {
                if (max_tluszcze != 0) { chart2.Series["fat"].Points.AddY((skala * d) / max_tluszcze); }
            }
            foreach (decimal d in l_blonnik)
            {
                if (max_blonnik != 0) { chart2.Series["roughage"].Points.AddY((skala * d) / max_blonnik); }
            }
            foreach (decimal d in l_sodium)
            {
                if (max_sodium != 0) { chart2.Series["sodium"].Points.AddY((skala * d) / max_sodium); }
            }
            foreach (decimal d in l_energy)
            {
                if (max_energy != 0) { chart2.Series["energy"].Points.AddY((skala * d) / max_energy); }
            }
            foreach (decimal d in l_iloscpos)
            {
                chart2.Series["Posiłków"].Points.AddY((skala * d) / 5);
            }














        }

        private void button10_Click(object sender, EventArgs e)
        {
            analiza(przesylanadata);
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            wykres7dni(przesylanadata);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.Text != "") { comboBox4.Enabled = true; } else { comboBox4.Enabled = false; }
            if (comboBox4.Text != "") { analizailosciowa(); }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            analizailosciowa();
        }

        private void analizailosciowa()
        {
            listView4.Items.Clear();
            if (comboBox3.Text == "Whole day")
            {
                //MessageBox.Show("działam");
                listView3.Items.Clear();
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");
                foreach (string fileName in fileEntries)
                {
                    using (StreamReader writer = new StreamReader(fileName))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        DateTime data = new DateTime();


                        //
                        string[] fileEntries2 = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
                        foreach (string fileName2 in fileEntries2)
                        {
                            using (StreamReader writer2 = new StreamReader(fileName2))
                            {
                                string odczyt2 = writer2.ReadToEnd();

                                List<string> prod = new List<string>(odczyt2.Split(charSeparators));
                                //MessageBox.Show(l[2] + " : " + prod[0]);
                                if (l[2] == prod[0])
                                {
                                    //MessageBox.Show("działam");
                                    data = Convert.ToDateTime(l[0]);
                                    ListViewItem item = new ListViewItem();
                                    if (comboBox4.Text == "protein") { item.Text = Convert.ToString(Convert.ToDecimal(prod[2]) * Convert.ToDecimal(l[3])); }
                                    if (comboBox4.Text == "carbohydrates") { item.Text = Convert.ToString(Convert.ToDecimal(prod[3]) * Convert.ToDecimal(l[3])); }
                                    if (comboBox4.Text == "Tłuszcz") { item.Text = Convert.ToString(Convert.ToDecimal(prod[4]) * Convert.ToDecimal(l[3])); }
                                    if (comboBox4.Text == "roughage") { item.Text = Convert.ToString(Convert.ToDecimal(prod[5]) * Convert.ToDecimal(l[3])); }
                                    if (comboBox4.Text == "sodium") { item.Text = Convert.ToString(Convert.ToDecimal(prod[6]) * Convert.ToDecimal(l[3])); }
                                    if (comboBox4.Text == "Wartość energetyczna") { item.Text = Convert.ToString(Convert.ToDecimal(prod[8]) * Convert.ToDecimal(l[3])); }
                                    if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                    {
                                        using (StreamReader writer3 = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                        {
                                            string odczyt3 = writer3.ReadToEnd();

                                            List<string> fettle = new List<string>(odczyt3.Split(charSeparators));
                                            decimal s1; decimal s2; decimal s3; decimal s4; decimal s5;
                                            if (fettle[0] != "-") { s1 = Convert.ToDecimal(fettle[0]); } else { s1 = 5; }
                                            if (fettle[1] != "-") { s2 = Convert.ToDecimal(fettle[1]); } else { s2 = 5; }
                                            if (fettle[2] != "-") { s3 = Convert.ToDecimal(fettle[2]); } else { s3 = 5; }
                                            if (fettle[3] != "-") { s4 = Convert.ToDecimal(fettle[3]); } else { s4 = 5; }
                                            if (fettle[4] != "-") { s5 = Convert.ToDecimal(fettle[4]); } else { s5 = 5; }
                                            item.SubItems.Add(Convert.ToString((s1 + s2 + s3 + s4 + s5) / 5));

                                            //
                                            item.SubItems.Add(data.ToString());


                                        }
                                    }
                                    else { item.SubItems.Add("5"); item.SubItems.Add(data.ToString()); }
                                    listView3.Items.Add(item);
                                }


                            }

                        }
                    }
                }
            }


            if (comboBox3.Text == "Breakfest")
            {
                //MessageBox.Show("działam");
                listView3.Items.Clear();
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");
                foreach (string fileName in fileEntries)
                {
                    using (StreamReader writer = new StreamReader(fileName))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        DateTime data = new DateTime();


                        //
                        string[] fileEntries2 = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
                        foreach (string fileName2 in fileEntries2)
                        {
                            using (StreamReader writer2 = new StreamReader(fileName2))
                            {
                                string odczyt2 = writer2.ReadToEnd();

                                List<string> prod = new List<string>(odczyt2.Split(charSeparators));
                                //MessageBox.Show(l[2] + " : " + prod[0]);
                                if (l[1] == "Breakfest")
                                {
                                    if (l[2] == prod[0])
                                    {
                                        //MessageBox.Show("działam");
                                        data = Convert.ToDateTime(l[0]);
                                        ListViewItem item = new ListViewItem();
                                        if (comboBox4.Text == "protein") { item.Text = Convert.ToString(Convert.ToDecimal(prod[2]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "carbohydrates") { item.Text = Convert.ToString(Convert.ToDecimal(prod[3]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "Tłuszcz") { item.Text = Convert.ToString(Convert.ToDecimal(prod[4]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "roughage") { item.Text = Convert.ToString(Convert.ToDecimal(prod[5]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "sodium") { item.Text = Convert.ToString(Convert.ToDecimal(prod[6]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "Wartość energetyczna") { item.Text = Convert.ToString(Convert.ToDecimal(prod[8]) * Convert.ToDecimal(l[3])); }
                                        if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                        {
                                            using (StreamReader writer3 = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                            {
                                                string odczyt3 = writer3.ReadToEnd();

                                                List<string> fettle = new List<string>(odczyt3.Split(charSeparators));

                                                if (fettle[0] != "-")
                                                {
                                                    item.SubItems.Add(fettle[0]); item.SubItems.Add(data.ToString());
                                                }
                                                else { item.SubItems.Add("5"); item.SubItems.Add(data.ToString()); }

                                            }
                                        }
                                        else { item.SubItems.Add("5"); item.SubItems.Add(data.ToString()); }
                                        listView3.Items.Add(item);
                                    }

                                }
                            }

                        }
                    }
                }


            }

            if (comboBox3.Text == "II Breakfest")
            {
                //MessageBox.Show("działam");
                listView3.Items.Clear();
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");
                foreach (string fileName in fileEntries)
                {
                    using (StreamReader writer = new StreamReader(fileName))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        DateTime data = new DateTime();


                        //
                        string[] fileEntries2 = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
                        foreach (string fileName2 in fileEntries2)
                        {
                            using (StreamReader writer2 = new StreamReader(fileName2))
                            {
                                string odczyt2 = writer2.ReadToEnd();

                                List<string> prod = new List<string>(odczyt2.Split(charSeparators));
                                //MessageBox.Show(l[2] + " : " + prod[0]);
                                if (l[1] == "II Breakfest")
                                {
                                    if (l[2] == prod[0])
                                    {
                                        //MessageBox.Show("działam");
                                        data = Convert.ToDateTime(l[0]);
                                        ListViewItem item = new ListViewItem();
                                        if (comboBox4.Text == "protein") { item.Text = Convert.ToString(Convert.ToDecimal(prod[2]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "carbohydrates") { item.Text = Convert.ToString(Convert.ToDecimal(prod[3]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "Tłuszcz") { item.Text = Convert.ToString(Convert.ToDecimal(prod[4]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "roughage") { item.Text = Convert.ToString(Convert.ToDecimal(prod[5]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "sodium") { item.Text = Convert.ToString(Convert.ToDecimal(prod[6]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "Wartość energetyczna") { item.Text = Convert.ToString(Convert.ToDecimal(prod[8]) * Convert.ToDecimal(l[3])); }
                                        if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                        {
                                            using (StreamReader writer3 = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                            {
                                                string odczyt3 = writer3.ReadToEnd();

                                                List<string> fettle = new List<string>(odczyt3.Split(charSeparators));

                                                if (fettle[1] != "-")
                                                {
                                                    item.SubItems.Add(fettle[1]); item.SubItems.Add(data.ToString());
                                                }
                                                else { item.SubItems.Add("5"); item.SubItems.Add(data.ToString()); }

                                            }
                                        }
                                        else { item.SubItems.Add("5"); item.SubItems.Add(data.ToString()); }
                                        listView3.Items.Add(item);
                                    }

                                }
                            }

                        }
                    }
                }
            }

            if (comboBox3.Text == "Lunch")
            {
                //MessageBox.Show("działam");
                listView3.Items.Clear();
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");
                foreach (string fileName in fileEntries)
                {
                    using (StreamReader writer = new StreamReader(fileName))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        DateTime data = new DateTime();


                        //
                        string[] fileEntries2 = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
                        foreach (string fileName2 in fileEntries2)
                        {
                            using (StreamReader writer2 = new StreamReader(fileName2))
                            {
                                string odczyt2 = writer2.ReadToEnd();

                                List<string> prod = new List<string>(odczyt2.Split(charSeparators));
                                //MessageBox.Show(l[2] + " : " + prod[0]);
                                if (l[1] == "Lunch")
                                {
                                    if (l[2] == prod[0])
                                    {
                                        //MessageBox.Show("działam");
                                        data = Convert.ToDateTime(l[0]);
                                        ListViewItem item = new ListViewItem();
                                        if (comboBox4.Text == "protein") { item.Text = Convert.ToString(Convert.ToDecimal(prod[2]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "carbohydrates") { item.Text = Convert.ToString(Convert.ToDecimal(prod[3]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "Tłuszcz") { item.Text = Convert.ToString(Convert.ToDecimal(prod[4]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "roughage") { item.Text = Convert.ToString(Convert.ToDecimal(prod[5]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "sodium") { item.Text = Convert.ToString(Convert.ToDecimal(prod[6]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "Wartość energetyczna") { item.Text = Convert.ToString(Convert.ToDecimal(prod[8]) * Convert.ToDecimal(l[3])); }
                                        if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                        {
                                            using (StreamReader writer3 = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                            {
                                                string odczyt3 = writer3.ReadToEnd();

                                                List<string> fettle = new List<string>(odczyt3.Split(charSeparators));

                                                if (fettle[2] != "-")
                                                {
                                                    item.SubItems.Add(fettle[2]); item.SubItems.Add(data.ToString());
                                                }
                                                else { item.SubItems.Add("5"); item.SubItems.Add(data.ToString()); }

                                            }
                                        }
                                        else { item.SubItems.Add("5"); item.SubItems.Add(data.ToString()); }
                                        listView3.Items.Add(item);
                                    }

                                }
                            }

                        }
                    }
                }
            }

            if (comboBox3.Text == "Dinner")
            {
                //MessageBox.Show("działam");
                listView3.Items.Clear();
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");
                foreach (string fileName in fileEntries)
                {
                    using (StreamReader writer = new StreamReader(fileName))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        DateTime data = new DateTime();


                        //
                        string[] fileEntries2 = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
                        foreach (string fileName2 in fileEntries2)
                        {
                            using (StreamReader writer2 = new StreamReader(fileName2))
                            {
                                string odczyt2 = writer2.ReadToEnd();

                                List<string> prod = new List<string>(odczyt2.Split(charSeparators));
                                //MessageBox.Show(l[2] + " : " + prod[0]);
                                if (l[1] == "Dinner")
                                {
                                    if (l[2] == prod[0])
                                    {
                                        //MessageBox.Show("działam");
                                        data = Convert.ToDateTime(l[0]);
                                        ListViewItem item = new ListViewItem();
                                        if (comboBox4.Text == "protein") { item.Text = Convert.ToString(Convert.ToDecimal(prod[2]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "carbohydrates") { item.Text = Convert.ToString(Convert.ToDecimal(prod[3]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "Tłuszcz") { item.Text = Convert.ToString(Convert.ToDecimal(prod[4]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "roughage") { item.Text = Convert.ToString(Convert.ToDecimal(prod[5]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "sodium") { item.Text = Convert.ToString(Convert.ToDecimal(prod[6]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "Wartość energetyczna") { item.Text = Convert.ToString(Convert.ToDecimal(prod[8]) * Convert.ToDecimal(l[3])); }
                                        if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                        {
                                            using (StreamReader writer3 = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                            {
                                                string odczyt3 = writer3.ReadToEnd();

                                                List<string> fettle = new List<string>(odczyt3.Split(charSeparators));

                                                if (fettle[3] != "-")
                                                {
                                                    item.SubItems.Add(fettle[3]); item.SubItems.Add(data.ToString());
                                                }
                                                else { item.SubItems.Add("5"); item.SubItems.Add(data.ToString()); }

                                            }
                                        }
                                        else { item.SubItems.Add("5"); item.SubItems.Add(data.ToString()); }
                                        listView3.Items.Add(item);
                                    }

                                }
                            }

                        }
                    }
                }
            }
            if (comboBox3.Text == "Supper")
            {
                //MessageBox.Show("działam");
                listView3.Items.Clear();
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");
                foreach (string fileName in fileEntries)
                {
                    using (StreamReader writer = new StreamReader(fileName))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        DateTime data = new DateTime();


                        //
                        string[] fileEntries2 = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
                        foreach (string fileName2 in fileEntries2)
                        {
                            using (StreamReader writer2 = new StreamReader(fileName2))
                            {
                                string odczyt2 = writer2.ReadToEnd();

                                List<string> prod = new List<string>(odczyt2.Split(charSeparators));
                                //MessageBox.Show(l[2] + " : " + prod[0]);
                                if (l[1] == "Supper")
                                {
                                    if (l[2] == prod[0])
                                    {
                                        //MessageBox.Show("działam");
                                        data = Convert.ToDateTime(l[0]);
                                        ListViewItem item = new ListViewItem();
                                        if (comboBox4.Text == "protein") { item.Text = Convert.ToString(Convert.ToDecimal(prod[2]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "carbohydrates") { item.Text = Convert.ToString(Convert.ToDecimal(prod[3]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "Tłuszcz") { item.Text = Convert.ToString(Convert.ToDecimal(prod[4]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "roughage") { item.Text = Convert.ToString(Convert.ToDecimal(prod[5]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "sodium") { item.Text = Convert.ToString(Convert.ToDecimal(prod[6]) * Convert.ToDecimal(l[3])); }
                                        if (comboBox4.Text == "Wartość energetyczna") { item.Text = Convert.ToString(Convert.ToDecimal(prod[8]) * Convert.ToDecimal(l[3])); }
                                        if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                        {
                                            using (StreamReader writer3 = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                            {
                                                string odczyt3 = writer3.ReadToEnd();

                                                List<string> fettle = new List<string>(odczyt3.Split(charSeparators));

                                                if (fettle[4] != "-")
                                                {
                                                    item.SubItems.Add(fettle[4]); item.SubItems.Add(data.ToString());
                                                }
                                                else { item.SubItems.Add("5"); item.SubItems.Add(data.ToString()); }


                                            }
                                        }
                                        else { item.SubItems.Add("5"); item.SubItems.Add(data.ToString()); }
                                        listView3.Items.Add(item);
                                    }

                                }
                            }

                        }
                    }
                }
            }

            //listView 6 - zsumowane datami
            if (listView3.Items.Count > 0)
            {
                listView6.Items.Clear(); List<DateTime> listadat = new List<DateTime>();
                foreach (ListViewItem pierwszy in listView3.Items)
                {

                    listadat.Add(Convert.ToDateTime(pierwszy.SubItems[2].Text));
                }
                listadat.Sort();
                Int32 index = 0;
                while (index < listadat.Count - 1)
                {
                    if (listadat[index] == listadat[index + 1])
                        listadat.RemoveAt(index);
                    else
                        index++;
                }
                //foreach (DateTime d in listadat) { MessageBox.Show(d.ToString()); }

                //foreach (ListViewItem pierwszy in listView3.Items)
                //{
                //    List<decimal > lista = new List<decimal >();
                //    lista.Add(Convert.ToDecimal(pierwszy.SubItems[0].Text));
                //    foreach (ListViewItem drugi in listView3.Items)
                //    {
                //        if (pierwszy.SubItems[2].Text == drugi.SubItems[2].Text)
                //        {
                //            lista.Add(Convert.ToDecimal(drugi.SubItems[0].Text));
                //            //listView3.Items.Remove(drugi);

                //        }



                //    }

                foreach (DateTime d in listadat)
                {
                    List<decimal> suma = new List<decimal>(); List<decimal> samopocz = new List<decimal>();
                    foreach (ListViewItem i in listView3.Items)
                    {
                        if (i.SubItems[2].Text == d.ToString()) { suma.Add(Convert.ToDecimal(i.SubItems[0].Text)); samopocz.Add(Convert.ToDecimal(i.SubItems[1].Text)); }
                    }
                    ListViewItem item = new ListViewItem();
                    decimal oblicz = suma.Sum() / 2;
                    item.Text = oblicz.ToString();
                    item.SubItems.Add(samopocz.Average().ToString());
                    listView6.Items.Add(item);
                }

                //tag duplicates for removal

                //List<ListViewItem> toRemove = new List<ListViewItem>();



            }

            if (listView6.Items.Count > 0)
            {

                listView6.Sort();
                listView4.Items.Clear();
                //tworzymy listę dat

                //czyszczenie i uśrednianie
                foreach (ListViewItem pierwszy in listView6.Items)
                {
                    List<decimal> lista = new List<decimal>();
                    //lista.Add(Convert.ToDecimal(pierwszy.SubItems[1].Text));
                    foreach (ListViewItem drugi in listView6.Items)
                    {
                        if (pierwszy.Text == drugi.Text)
                        {
                            lista.Add(Convert.ToDecimal(drugi.SubItems[1].Text));
                            //listView3.Items.Remove(drugi);

                        }



                    }
                    ListViewItem item = new ListViewItem();
                    item.Text = pierwszy.Text;
                    item.SubItems.Add(Convert.ToString(lista.Average()));
                    listView4.Items.Add(item);
                }

                //tag duplicates for removal

                //List<ListViewItem> toRemove = new List<ListViewItem>();
                foreach (ListViewItem item1 in listView4.Items)
                {
                    bool pierwszyraz = true;
                    foreach (ListViewItem item2 in listView4.Items)
                    {
                        //compare the two items
                        if (item1.Text == item2.Text)
                        {
                            if (pierwszyraz == false)
                            {
                                listView4.Items.Remove(item2);
                            }
                            else { pierwszyraz = false; }
                        }
                    }
                }

                //remove duplicates

                


                chart3.Series.Clear();
                // Set series chart type
                chart3.Series.Add("w");
                chart3.Series.Add("i");
                //chart3.Series["w"].ChartType = SeriesChartType.Spline;
                chart3.Series["w"].ChartType = SeriesChartType.Spline  ;
                chart3.Series["w"].BorderWidth = 3;
                chart3.Series["i"].ChartType = SeriesChartType.Point    ;
                chart3.Series["i"].BorderWidth = 5;
                //chart2.Series.Add("protein");
                //chart2.Series["protein"].ChartType = SeriesChartType.Spline;
                //chart2.Series.Add("carbohydrates");
                //chart2.Series["carbohydrates"].ChartType = SeriesChartType.Spline;
                //chart2.Series.Add("fat");
                //chart2.Series["fat"].ChartType = SeriesChartType.Spline;
                //chart2.Series.Add("roughage");
                //chart2.Series["roughage"].ChartType = SeriesChartType.Spline;
                //chart2.Series.Add("Posiłków");
                //chart2.Series["Posiłków"].ChartType = SeriesChartType.Spline;
                //chart2.Series.Add("sodium");
                //chart2.Series["sodium"].ChartType = SeriesChartType.Spline;
                //chart2.Series.Add(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle");
                //chart2.Series["fettle"].ChartType = SeriesChartType.Spline;
                //chart2.Series.Add("energy");
                //chart2.Series["energy"].ChartType = SeriesChartType.Spline;
                //chart2.Series.Add(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle");
                //chart2.Series["fettle"].ChartType = SeriesChartType.Spline;
                chart3.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = true;
                chart3.ChartAreas["ChartArea1"].BackColor = Color.WhiteSmoke;
                chart3.ChartAreas["ChartArea1"].BorderWidth = 11;
                // Show as 3D
                chart3.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;

                // Set point labels
                chart3.Series["w"].IsValueShownAsLabel = false;


                NumericComparer ns = new NumericComparer();
                //List<string> p = new List<string>();
                //object[] array = new object[listView1.Items.Count];
                String[] items = new String[listView4.Items.Count];
                //listView4.Items.CopyTo(items.ToString() ,0);
                int runs = 0;
                foreach (ListViewItem i in listView4.Items)
                {
                    //for (int runs = 0; runs < listView4.Items.Count; runs++)
                    //{
                    items[runs] = i.Text + "|" + i.SubItems[1].Text;
                    runs++;
                    //}

                }
                //}
                ////listView1.Items.CopyTo(array, 0);
                Array.Sort(items, ns);
                //listView4.Items.Clear();
                //    foreach(ListViewItem item in items) {
                //        listView4.Items.Add(item);

                //    }
                //foreach (ListViewItem i in listView4.Items) { 



                //}


                //listView4.Sort();
                foreach (String i in items)
                {
                    string[] split = i.Split(new Char[] { '|' });
                    //MessageBox.Show(i);
                    //
                    chart3.Series["w"].Points.AddXY(Convert.ToDecimal(split[0]), Convert.ToDecimal(split[1])); ;
                    chart3.Series["i"].Points.AddXY(Convert.ToDecimal(split[0]), Convert.ToDecimal(split[1])); ;
                }




                if (!Directory.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\ilosc")) { Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\ilosc"); }
                if (!Directory.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\ilosc\\" + comboBox3.Text)) { Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\ilosc\\" + comboBox3.Text); }
                using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\ilosc\\" + comboBox3.Text + "\\" + comboBox4.Text))
                {
                    string dozapisu = "";
                    foreach (ListViewItem rzecz in listView6.Items) {
                        if (dozapisu != "") { dozapisu += "\n" + rzecz.SubItems[0].Text + "\t" + rzecz.SubItems[1].Text; } else { dozapisu = rzecz.SubItems[0].Text + "\t" + rzecz.SubItems[1].Text; }
                    }

                    w.Write(dozapisu);
                
                }








            }






















        }


        private void analiza_dobowa()
        {

            if (comboBox5.Text != "")
            {
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza");
                foreach (string f in fileEntries)
                {
                    File.Delete(f);
                }
            }
            if (comboBox5.Text == "Whole day")
            {
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");

                //LISTA DAT
                List<DateTime> listadat = new List<DateTime>();
                foreach (string fileName in fileEntries)
                {
                    using (StreamReader writer = new StreamReader(fileName))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        listadat.Add(Convert.ToDateTime(l[0]));
                    }
                }
                listadat.Sort();
                Int32 index = 0;
                while (index < listadat.Count - 1)
                {
                    if (listadat[index] == listadat[index + 1])
                        listadat.RemoveAt(index);
                    else
                        index++;
                }
                NumericComparer ns = new NumericComparer();
                foreach (DateTime d in listadat)
                {
                    analiza(d);
                }
                //List<string> p = new List<string>();
                //object[] array = new object[listView1.Items.Count];
                //String[] posiłek = new String[fileEntries.Count()];
                //listView4.Items.CopyTo(items.ToString() ,0);
                //int runs = 0;
                ////foreach (ListViewItem i in listView4.Items)
                ////{
                ////    //for (int runs = 0; runs < listView4.Items.Count; runs++)
                ////    //{
                ////    items[runs] = i.Text + "|" + i.SubItems[1].Text;
                ////    runs++;
                ////    //}

                ////}
                //////}
                ////////listView1.Items.CopyTo(array, 0);
                ////Array.Sort(items, ns);
                ////listView4.Items.Clear();
                ////    foreach(ListViewItem item in items) {
                ////        listView4.Items.Add(item);

                ////    }
                ////foreach (ListViewItem i in listView4.Items) { 



                ////}


                ////listView4.Sort();
                ////foreach (String i in items)
                ////{
                ////    string[] split = i.Split(new Char[] { '|' });
                ////    //MessageBox.Show(i);
                ////    //
                ////    chart3.Series["w"].Points.AddXY(Convert.ToDecimal(split[0]), Convert.ToDecimal(split[1])); ;
                ////}






                ////MessageBox.Show("działam");
                ////listView3.Items.Clear();
                //foreach (DateTime d in listadat)
                //{
                //    decimal ilosc_bi = 0;
                //        decimal ilosc_weg = 0;
                //        decimal ilosc_tlusz = 0;
                //        decimal ilosc_blon = 0;
                //        decimal ilosc_sod = 0;
                //        decimal ilosc_ener = 0;
                //        decimal samopocz = 0;
                //    foreach (string fileName in fileEntries)
                //    {

                //        using (StreamReader writer = new StreamReader(fileName))
                //        {
                //            string odczyt = writer.ReadToEnd();
                //            char[] charSeparators = new char[] { '|' };
                //            List<string> l = new List<string>(odczyt.Split(charSeparators));
                //            DateTime data = new DateTime();

                //            if (l[0] == d.ToString())
                //            {
                //                //
                //                string[] fileEntries2 = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
                //                foreach (string fileName2 in fileEntries2)
                //                {
                //                    using (StreamReader writer2 = new StreamReader(fileName2))
                //                    {
                //                        string odczyt2 = writer2.ReadToEnd();

                //                        List<string> prod = new List<string>(odczyt2.Split(charSeparators));
                //                        //MessageBox.Show(l[2] + " : " + prod[0]);
                //                        if (l[2] == prod[0])
                //                        {
                //                            //MessageBox.Show("działam");
                //                            data = Convert.ToDateTime(l[0]);
                //                            //ListViewItem item = new ListViewItem();
                //                            ilosc_bi += Convert.ToDecimal(prod[2]) * Convert.ToDecimal(l[3]);
                //                            ilosc_weg += Convert.ToDecimal(prod[3]) * Convert.ToDecimal(l[3]);
                //                            ilosc_tlusz += Convert.ToDecimal(prod[4]) * Convert.ToDecimal(l[3]);
                //                            ilosc_blon += Convert.ToDecimal(prod[5]) * Convert.ToDecimal(l[3]);
                //                            ilosc_sod += Convert.ToDecimal(prod[6]) * Convert.ToDecimal(l[3]);
                //                            ilosc_ener += Convert.ToDecimal(prod[8]) * Convert.ToDecimal(l[3]);
                //                            if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                //                            {
                //                                using (StreamReader writer3 = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                //                                {
                //                                    string odczyt3 = writer3.ReadToEnd();

                //                                    List<string> fettle = new List<string>(odczyt3.Split(charSeparators));
                //                                    decimal s1; decimal s2; decimal s3; decimal s4; decimal s5;
                //                                    if (fettle[0] != "-") { s1 = Convert.ToDecimal(fettle[0]); } else { s1 = 5; }
                //                                    if (fettle[1] != "-") { s2 = Convert.ToDecimal(fettle[1]); } else { s2 = 5; }
                //                                    if (fettle[2] != "-") { s3 = Convert.ToDecimal(fettle[2]); } else { s3 = 5; }
                //                                    if (fettle[3] != "-") { s4 = Convert.ToDecimal(fettle[3]); } else { s4 = 5; }
                //                                    if (fettle[4] != "-") { s5 = Convert.ToDecimal(fettle[4]); } else { s5 = 5; }
                //                                    samopocz = (s1 + s2 + s3 + s4 + s5) / 5;


                //                                }
                //                            }
                //                            else { samopocz = 5; }
                //                            //listView3.Items.Add(item);
                //                            //for (int runs = 0; runs < listView4.Items.Count; runs++)
                //                            //{

                //                            //}
                //                        }


                //                    }

                //                }
                //            }

                //        }
                //    }
                //using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\" + runs.ToString()))
                //                            {
                //                                w.Write(samopocz + "|" + ilosc_bi + "|" + ilosc_weg + "|" + ilosc_tlusz + "|" + ilosc_blon + "|" + ilosc_sod + "|" + ilosc_ener);
                //                                runs++;
                //                            }
                //}
                string[] an = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza");
                List<decimal> anal_sampoczucie = new List<decimal>();
                List<decimal> anal_bialko = new List<decimal>();
                List<decimal> anal_weglowodany = new List<decimal>();
                List<decimal> anal_tluszcze = new List<decimal>();
                List<decimal> anal_blonnik = new List<decimal>();
                List<decimal> anal_sodium = new List<decimal>();
                List<decimal> anal_energia = new List<decimal>();


                foreach (string f in an)
                {
                    using (StreamReader writer = new StreamReader(f))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));

                        anal_sampoczucie.Add(Convert.ToDecimal(l[0]));

                    }

                }
                //Ustalamy max samopoczucia i podmaxy.
                decimal fettle_1 = anal_sampoczucie.Max();
                decimal fettle_2 = 0;
                decimal fettle_3 = 0;

                foreach (decimal d in anal_sampoczucie)
                {
                    bool tomax = false;
                    if (fettle_1 == d) { tomax = true; }
                    if (tomax == false)
                    {
                        bool byla2 = false;
                        //MessageBox.Show("d= " + d);
                        if (fettle_2 == d) { byla2 = true; }
                        if (byla2 == false) { if (d > fettle_2) { fettle_3 = fettle_2; fettle_2 = d; byla2 = true; } }
                        if (byla2 == false) { if (d > fettle_3) { fettle_3 = d; } }
                        //
                        //MessageBox.Show(fettle_1 + " / " + fettle_2 + " / " + fettle_3);





                    }




                }

                foreach (string f in an)
                {
                    using (StreamReader writer = new StreamReader(f))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));

                        if (Convert.ToDecimal(l[0]) == fettle_1)
                        {
                            int waga = 0;
                            while (waga < 3)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));


                                waga++;
                            }

                        }

                        if (Convert.ToDecimal(l[0]) == fettle_2)
                        {
                            int waga = 0;
                            while (waga < 4)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));

                                //MessageBox.Show(l[1]);
                                waga++;
                            }

                        }
                        if (Convert.ToDecimal(l[0]) == fettle_3)
                        {
                            int waga = 0;
                            while (waga < 2)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));


                                waga++;
                            }

                        }

                    }

                }

                //Kończymy analizę.
                decimal naj_bialko = anal_bialko.Average();
                decimal naj_weglowodany = anal_weglowodany.Average();
                decimal naj_tluszcze = anal_tluszcze.Average();
                decimal naj_blonnik = anal_blonnik.Average();
                decimal naj_sodium = anal_sodium.Average();
                decimal naj_energia = anal_energia.Average();
                using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\ja\\predykcja_dzienna"))
                {
                    w.Write(naj_bialko + "|" + naj_weglowodany + "|" + naj_tluszcze + "|" + naj_blonnik + "|" + naj_sodium + "|" + naj_energia);

                }

                textBox28.Text = naj_bialko.ToString();
                textBox27.Text = naj_weglowodany.ToString();
                textBox26.Text = naj_tluszcze.ToString();
                textBox25.Text = naj_blonnik.ToString();
                textBox24.Text = naj_sodium.ToString();
                textBox29.Text = naj_energia.ToString();







            }




            if (comboBox5.Text == "Breakfest")
            {
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");

                NumericComparer ns = new NumericComparer();
                //List<string> p = new List<string>();
                //object[] array = new object[listView1.Items.Count];
                //String[] posiłek = new String[fileEntries.Count()];
                //listView4.Items.CopyTo(items.ToString() ,0);
                int runs = 0;
                //foreach (ListViewItem i in listView4.Items)
                //{
                //    //for (int runs = 0; runs < listView4.Items.Count; runs++)
                //    //{
                //    items[runs] = i.Text + "|" + i.SubItems[1].Text;
                //    runs++;
                //    //}

                //}
                ////}
                //////listView1.Items.CopyTo(array, 0);
                //Array.Sort(items, ns);
                //listView4.Items.Clear();
                //    foreach(ListViewItem item in items) {
                //        listView4.Items.Add(item);

                //    }
                //foreach (ListViewItem i in listView4.Items) { 



                //}


                //listView4.Sort();
                //foreach (String i in items)
                //{
                //    string[] split = i.Split(new Char[] { '|' });
                //    //MessageBox.Show(i);
                //    //
                //    chart3.Series["w"].Points.AddXY(Convert.ToDecimal(split[0]), Convert.ToDecimal(split[1])); ;
                //}






                //MessageBox.Show("działam");
                //listView3.Items.Clear();

                foreach (string fileName in fileEntries)
                {
                    decimal ilosc_bi = 0;
                    decimal ilosc_weg = 0;
                    decimal ilosc_tlusz = 0;
                    decimal ilosc_blon = 0;
                    decimal ilosc_sod = 0;
                    decimal ilosc_ener = 0;
                    decimal samopocz = 0;
                    using (StreamReader writer = new StreamReader(fileName))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        DateTime data = new DateTime();


                        //
                        string[] fileEntries2 = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
                        foreach (string fileName2 in fileEntries2)
                        {
                            using (StreamReader writer2 = new StreamReader(fileName2))
                            {
                                string odczyt2 = writer2.ReadToEnd();

                                List<string> prod = new List<string>(odczyt2.Split(charSeparators));
                                //MessageBox.Show(l[2] + " : " + prod[0]);
                                if (l[1] == "Breakfest")
                                {
                                    if (l[2] == prod[0])
                                    {
                                        //MessageBox.Show("działam");
                                        data = Convert.ToDateTime(l[0]);
                                        //ListViewItem item = new ListViewItem();
                                        ilosc_bi = Convert.ToDecimal(prod[2]) * Convert.ToDecimal(l[3]);
                                        ilosc_weg = Convert.ToDecimal(prod[3]) * Convert.ToDecimal(l[3]);
                                        ilosc_tlusz = Convert.ToDecimal(prod[4]) * Convert.ToDecimal(l[3]);
                                        ilosc_blon = Convert.ToDecimal(prod[5]) * Convert.ToDecimal(l[3]);
                                        ilosc_sod = Convert.ToDecimal(prod[6]) * Convert.ToDecimal(l[3]);
                                        ilosc_ener = Convert.ToDecimal(prod[8]) * Convert.ToDecimal(l[3]);
                                        if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                        {
                                            using (StreamReader writer3 = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                            {
                                                string odczyt3 = writer3.ReadToEnd();

                                                List<string> fettle = new List<string>(odczyt3.Split(charSeparators));
                                                decimal s1; decimal s2; decimal s3; decimal s4; decimal s5;
                                                if (fettle[0] != "-") { s1 = Convert.ToDecimal(fettle[0]); } else { s1 = 5; }
                                                if (fettle[1] != "-") { s2 = Convert.ToDecimal(fettle[1]); } else { s2 = 5; }
                                                if (fettle[2] != "-") { s3 = Convert.ToDecimal(fettle[2]); } else { s3 = 5; }
                                                if (fettle[3] != "-") { s4 = Convert.ToDecimal(fettle[3]); } else { s4 = 5; }
                                                if (fettle[4] != "-") { s5 = Convert.ToDecimal(fettle[4]); } else { s5 = 5; }
                                                samopocz = (s1 + s2 + s3 + s4 + s5) / 5;


                                            }
                                        }
                                        else { samopocz = 5; }
                                        //listView3.Items.Add(item);
                                        //for (int runs = 0; runs < listView4.Items.Count; runs++)
                                        //{
                                        using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\" + runs.ToString()))
                                        {
                                            w.Write(samopocz + "|" + ilosc_bi + "|" + ilosc_weg + "|" + ilosc_tlusz + "|" + ilosc_blon + "|" + ilosc_sod + "|" + ilosc_ener);
                                            runs++;
                                        }
                                        //}
                                    }
                                }

                            }

                        }
                    }


                }

                string[] an = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza");
                List<decimal> anal_sampoczucie = new List<decimal>();
                List<decimal> anal_bialko = new List<decimal>();
                List<decimal> anal_weglowodany = new List<decimal>();
                List<decimal> anal_tluszcze = new List<decimal>();
                List<decimal> anal_blonnik = new List<decimal>();
                List<decimal> anal_sodium = new List<decimal>();
                List<decimal> anal_energia = new List<decimal>();


                foreach (string f in an)
                {
                    using (StreamReader writer = new StreamReader(f))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));

                        anal_sampoczucie.Add(Convert.ToDecimal(l[0]));

                    }

                }
                //Ustalamy max samopoczucia i podmaxy.
                decimal fettle_1 = anal_sampoczucie.Max();
                decimal fettle_2 = 0;
                decimal fettle_3 = 0;

                foreach (decimal d in anal_sampoczucie)
                {
                    bool tomax = false;
                    if (fettle_1 == d) { tomax = true; }
                    if (tomax == false)
                    {
                        bool byla2 = false;
                        //MessageBox.Show("d= " + d);
                        if (fettle_2 == d) { byla2 = true; }
                        if (byla2 == false) { if (d > fettle_2) { fettle_3 = fettle_2; fettle_2 = d; byla2 = true; } }
                        if (byla2 == false) { if (d > fettle_3) { fettle_3 = d; } }
                        //
                        //MessageBox.Show(fettle_1 + " / " + fettle_2 + " / " + fettle_3);





                    }




                }

                foreach (string f in an)
                {
                    using (StreamReader writer = new StreamReader(f))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));

                        if (Convert.ToDecimal(l[0]) == fettle_1)
                        {
                            int waga = 0;
                            while (waga < 3)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));


                                waga++;
                            }

                        }

                        if (Convert.ToDecimal(l[0]) == fettle_2)
                        {
                            int waga = 0;
                            while (waga < 4)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));

                                //MessageBox.Show(l[1]);
                                waga++;
                            }

                        }
                        if (Convert.ToDecimal(l[0]) == fettle_3)
                        {
                            int waga = 0;
                            while (waga < 2)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));


                                waga++;
                            }

                        }

                    }

                }

                //Kończymy analizę.
                decimal naj_bialko = anal_bialko.Average();
                decimal naj_weglowodany = anal_weglowodany.Average();
                decimal naj_tluszcze = anal_tluszcze.Average();
                decimal naj_blonnik = anal_blonnik.Average();
                decimal naj_sodium = anal_sodium.Average();
                decimal naj_energia = anal_energia.Average();
                using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\ja\\predykcja_sniadanie"))
                {
                    w.Write(naj_bialko + "|" + naj_weglowodany + "|" + naj_tluszcze + "|" + naj_blonnik + "|" + naj_sodium + "|" + naj_energia);

                }

                textBox28.Text = naj_bialko.ToString();
                textBox27.Text = naj_weglowodany.ToString();
                textBox26.Text = naj_tluszcze.ToString();
                textBox25.Text = naj_blonnik.ToString();
                textBox24.Text = naj_sodium.ToString();
                textBox29.Text = naj_energia.ToString();







            }


            if (comboBox5.Text == "II Breakfest")
            {
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");

                NumericComparer ns = new NumericComparer();
                //List<string> p = new List<string>();
                //object[] array = new object[listView1.Items.Count];
                //String[] posiłek = new String[fileEntries.Count()];
                //listView4.Items.CopyTo(items.ToString() ,0);
                int runs = 0;
                //foreach (ListViewItem i in listView4.Items)
                //{
                //    //for (int runs = 0; runs < listView4.Items.Count; runs++)
                //    //{
                //    items[runs] = i.Text + "|" + i.SubItems[1].Text;
                //    runs++;
                //    //}

                //}
                ////}
                //////listView1.Items.CopyTo(array, 0);
                //Array.Sort(items, ns);
                //listView4.Items.Clear();
                //    foreach(ListViewItem item in items) {
                //        listView4.Items.Add(item);

                //    }
                //foreach (ListViewItem i in listView4.Items) { 



                //}


                //listView4.Sort();
                //foreach (String i in items)
                //{
                //    string[] split = i.Split(new Char[] { '|' });
                //    //MessageBox.Show(i);
                //    //
                //    chart3.Series["w"].Points.AddXY(Convert.ToDecimal(split[0]), Convert.ToDecimal(split[1])); ;
                //}






                //MessageBox.Show("działam");
                //listView3.Items.Clear();

                foreach (string fileName in fileEntries)
                {
                    decimal ilosc_bi = 0;
                    decimal ilosc_weg = 0;
                    decimal ilosc_tlusz = 0;
                    decimal ilosc_blon = 0;
                    decimal ilosc_sod = 0;
                    decimal ilosc_ener = 0;
                    decimal samopocz = 0;
                    using (StreamReader writer = new StreamReader(fileName))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        DateTime data = new DateTime();


                        //
                        string[] fileEntries2 = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
                        foreach (string fileName2 in fileEntries2)
                        {
                            using (StreamReader writer2 = new StreamReader(fileName2))
                            {
                                string odczyt2 = writer2.ReadToEnd();

                                List<string> prod = new List<string>(odczyt2.Split(charSeparators));
                                //MessageBox.Show(l[2] + " : " + prod[0]);
                                if (l[1] == "II Breakfest")
                                {
                                    if (l[2] == prod[0])
                                    {
                                        //MessageBox.Show("działam");
                                        data = Convert.ToDateTime(l[0]);
                                        //ListViewItem item = new ListViewItem();
                                        ilosc_bi = Convert.ToDecimal(prod[2]) * Convert.ToDecimal(l[3]);
                                        ilosc_weg = Convert.ToDecimal(prod[3]) * Convert.ToDecimal(l[3]);
                                        ilosc_tlusz = Convert.ToDecimal(prod[4]) * Convert.ToDecimal(l[3]);
                                        ilosc_blon = Convert.ToDecimal(prod[5]) * Convert.ToDecimal(l[3]);
                                        ilosc_sod = Convert.ToDecimal(prod[6]) * Convert.ToDecimal(l[3]);
                                        ilosc_ener = Convert.ToDecimal(prod[8]) * Convert.ToDecimal(l[3]);
                                        if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                        {
                                            using (StreamReader writer3 = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                            {
                                                string odczyt3 = writer3.ReadToEnd();

                                                List<string> fettle = new List<string>(odczyt3.Split(charSeparators));
                                                decimal s1; decimal s2; decimal s3; decimal s4; decimal s5;
                                                if (fettle[0] != "-") { s1 = Convert.ToDecimal(fettle[0]); } else { s1 = 5; }
                                                if (fettle[1] != "-") { s2 = Convert.ToDecimal(fettle[1]); } else { s2 = 5; }
                                                if (fettle[2] != "-") { s3 = Convert.ToDecimal(fettle[2]); } else { s3 = 5; }
                                                if (fettle[3] != "-") { s4 = Convert.ToDecimal(fettle[3]); } else { s4 = 5; }
                                                if (fettle[4] != "-") { s5 = Convert.ToDecimal(fettle[4]); } else { s5 = 5; }
                                                samopocz = (s1 + s2 + s3 + s4 + s5) / 5;


                                            }
                                        }
                                        else { samopocz = 5; }
                                        //listView3.Items.Add(item);
                                        //for (int runs = 0; runs < listView4.Items.Count; runs++)
                                        //{
                                        using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\" + runs.ToString()))
                                        {
                                            w.Write(samopocz + "|" + ilosc_bi + "|" + ilosc_weg + "|" + ilosc_tlusz + "|" + ilosc_blon + "|" + ilosc_sod + "|" + ilosc_ener);
                                            runs++;
                                        }
                                        //}
                                    }
                                }

                            }

                        }
                    }


                }

                string[] an = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza");
                List<decimal> anal_sampoczucie = new List<decimal>();
                List<decimal> anal_bialko = new List<decimal>();
                List<decimal> anal_weglowodany = new List<decimal>();
                List<decimal> anal_tluszcze = new List<decimal>();
                List<decimal> anal_blonnik = new List<decimal>();
                List<decimal> anal_sodium = new List<decimal>();
                List<decimal> anal_energia = new List<decimal>();


                foreach (string f in an)
                {
                    using (StreamReader writer = new StreamReader(f))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));

                        anal_sampoczucie.Add(Convert.ToDecimal(l[0]));

                    }

                }
                //Ustalamy max samopoczucia i podmaxy.
                decimal fettle_1 = anal_sampoczucie.Max();
                decimal fettle_2 = 0;
                decimal fettle_3 = 0;

                foreach (decimal d in anal_sampoczucie)
                {
                    bool tomax = false;
                    if (fettle_1 == d) { tomax = true; }
                    if (tomax == false)
                    {
                        bool byla2 = false;
                        //MessageBox.Show("d= " + d);
                        if (fettle_2 == d) { byla2 = true; }
                        if (byla2 == false) { if (d > fettle_2) { fettle_3 = fettle_2; fettle_2 = d; byla2 = true; } }
                        if (byla2 == false) { if (d > fettle_3) { fettle_3 = d; } }
                        //
                        //MessageBox.Show(fettle_1 + " / " + fettle_2 + " / " + fettle_3);





                    }




                }

                foreach (string f in an)
                {
                    using (StreamReader writer = new StreamReader(f))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));

                        if (Convert.ToDecimal(l[0]) == fettle_1)
                        {
                            int waga = 0;
                            while (waga < 3)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));


                                waga++;
                            }

                        }

                        if (Convert.ToDecimal(l[0]) == fettle_2)
                        {
                            int waga = 0;
                            while (waga < 4)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));

                                //MessageBox.Show(l[1]);
                                waga++;
                            }

                        }
                        if (Convert.ToDecimal(l[0]) == fettle_3)
                        {
                            int waga = 0;
                            while (waga < 2)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));


                                waga++;
                            }

                        }

                    }

                }

                //Kończymy analizę.
                decimal naj_bialko = anal_bialko.Average();
                decimal naj_weglowodany = anal_weglowodany.Average();
                decimal naj_tluszcze = anal_tluszcze.Average();
                decimal naj_blonnik = anal_blonnik.Average();
                decimal naj_sodium = anal_sodium.Average();
                decimal naj_energia = anal_energia.Average();
                using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\ja\\predykcja_IIsniadanie"))
                {
                    w.Write(naj_bialko + "|" + naj_weglowodany + "|" + naj_tluszcze + "|" + naj_blonnik + "|" + naj_sodium + "|" + naj_energia);

                }

                textBox28.Text = naj_bialko.ToString();
                textBox27.Text = naj_weglowodany.ToString();
                textBox26.Text = naj_tluszcze.ToString();
                textBox25.Text = naj_blonnik.ToString();
                textBox24.Text = naj_sodium.ToString();
                textBox29.Text = naj_energia.ToString();







            }


            if (comboBox5.Text == "Lunch")
            {
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");

                NumericComparer ns = new NumericComparer();
                //List<string> p = new List<string>();
                //object[] array = new object[listView1.Items.Count];
                //String[] posiłek = new String[fileEntries.Count()];
                //listView4.Items.CopyTo(items.ToString() ,0);
                int runs = 0;
                //foreach (ListViewItem i in listView4.Items)
                //{
                //    //for (int runs = 0; runs < listView4.Items.Count; runs++)
                //    //{
                //    items[runs] = i.Text + "|" + i.SubItems[1].Text;
                //    runs++;
                //    //}

                //}
                ////}
                //////listView1.Items.CopyTo(array, 0);
                //Array.Sort(items, ns);
                //listView4.Items.Clear();
                //    foreach(ListViewItem item in items) {
                //        listView4.Items.Add(item);

                //    }
                //foreach (ListViewItem i in listView4.Items) { 



                //}


                //listView4.Sort();
                //foreach (String i in items)
                //{
                //    string[] split = i.Split(new Char[] { '|' });
                //    //MessageBox.Show(i);
                //    //
                //    chart3.Series["w"].Points.AddXY(Convert.ToDecimal(split[0]), Convert.ToDecimal(split[1])); ;
                //}






                //MessageBox.Show("działam");
                //listView3.Items.Clear();

                foreach (string fileName in fileEntries)
                {
                    decimal ilosc_bi = 0;
                    decimal ilosc_weg = 0;
                    decimal ilosc_tlusz = 0;
                    decimal ilosc_blon = 0;
                    decimal ilosc_sod = 0;
                    decimal ilosc_ener = 0;
                    decimal samopocz = 0;
                    using (StreamReader writer = new StreamReader(fileName))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        DateTime data = new DateTime();


                        //
                        string[] fileEntries2 = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
                        foreach (string fileName2 in fileEntries2)
                        {
                            using (StreamReader writer2 = new StreamReader(fileName2))
                            {
                                string odczyt2 = writer2.ReadToEnd();

                                List<string> prod = new List<string>(odczyt2.Split(charSeparators));
                                //MessageBox.Show(l[2] + " : " + prod[0]);
                                if (l[1] == "Lunch")
                                {
                                    if (l[2] == prod[0])
                                    {
                                        //MessageBox.Show("działam");
                                        data = Convert.ToDateTime(l[0]);
                                        //ListViewItem item = new ListViewItem();
                                        ilosc_bi = Convert.ToDecimal(prod[2]) * Convert.ToDecimal(l[3]);
                                        ilosc_weg = Convert.ToDecimal(prod[3]) * Convert.ToDecimal(l[3]);
                                        ilosc_tlusz = Convert.ToDecimal(prod[4]) * Convert.ToDecimal(l[3]);
                                        ilosc_blon = Convert.ToDecimal(prod[5]) * Convert.ToDecimal(l[3]);
                                        ilosc_sod = Convert.ToDecimal(prod[6]) * Convert.ToDecimal(l[3]);
                                        ilosc_ener = Convert.ToDecimal(prod[8]) * Convert.ToDecimal(l[3]);
                                        if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                        {
                                            using (StreamReader writer3 = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                            {
                                                string odczyt3 = writer3.ReadToEnd();

                                                List<string> fettle = new List<string>(odczyt3.Split(charSeparators));
                                                decimal s1; decimal s2; decimal s3; decimal s4; decimal s5;
                                                if (fettle[0] != "-") { s1 = Convert.ToDecimal(fettle[0]); } else { s1 = 5; }
                                                if (fettle[1] != "-") { s2 = Convert.ToDecimal(fettle[1]); } else { s2 = 5; }
                                                if (fettle[2] != "-") { s3 = Convert.ToDecimal(fettle[2]); } else { s3 = 5; }
                                                if (fettle[3] != "-") { s4 = Convert.ToDecimal(fettle[3]); } else { s4 = 5; }
                                                if (fettle[4] != "-") { s5 = Convert.ToDecimal(fettle[4]); } else { s5 = 5; }
                                                samopocz = (s1 + s2 + s3 + s4 + s5) / 5;


                                            }
                                        }
                                        else { samopocz = 5; }
                                        //listView3.Items.Add(item);
                                        //for (int runs = 0; runs < listView4.Items.Count; runs++)
                                        //{
                                        using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\" + runs.ToString()))
                                        {
                                            w.Write(samopocz + "|" + ilosc_bi + "|" + ilosc_weg + "|" + ilosc_tlusz + "|" + ilosc_blon + "|" + ilosc_sod + "|" + ilosc_ener);
                                            runs++;
                                        }
                                        //}
                                    }
                                }

                            }

                        }
                    }


                }

                string[] an = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza");
                List<decimal> anal_sampoczucie = new List<decimal>();
                List<decimal> anal_bialko = new List<decimal>();
                List<decimal> anal_weglowodany = new List<decimal>();
                List<decimal> anal_tluszcze = new List<decimal>();
                List<decimal> anal_blonnik = new List<decimal>();
                List<decimal> anal_sodium = new List<decimal>();
                List<decimal> anal_energia = new List<decimal>();


                foreach (string f in an)
                {
                    using (StreamReader writer = new StreamReader(f))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));

                        anal_sampoczucie.Add(Convert.ToDecimal(l[0]));

                    }

                }
                //Ustalamy max samopoczucia i podmaxy.
                decimal fettle_1 = anal_sampoczucie.Max();
                decimal fettle_2 = 0;
                decimal fettle_3 = 0;

                foreach (decimal d in anal_sampoczucie)
                {
                    bool tomax = false;
                    if (fettle_1 == d) { tomax = true; }
                    if (tomax == false)
                    {
                        bool byla2 = false;
                        //MessageBox.Show("d= " + d);
                        if (fettle_2 == d) { byla2 = true; }
                        if (byla2 == false) { if (d > fettle_2) { fettle_3 = fettle_2; fettle_2 = d; byla2 = true; } }
                        if (byla2 == false) { if (d > fettle_3) { fettle_3 = d; } }
                        //
                        //MessageBox.Show(fettle_1 + " / " + fettle_2 + " / " + fettle_3);





                    }




                }

                foreach (string f in an)
                {
                    using (StreamReader writer = new StreamReader(f))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));

                        if (Convert.ToDecimal(l[0]) == fettle_1)
                        {
                            int waga = 0;
                            while (waga < 3)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));


                                waga++;
                            }

                        }

                        if (Convert.ToDecimal(l[0]) == fettle_2)
                        {
                            int waga = 0;
                            while (waga < 4)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));

                                //MessageBox.Show(l[1]);
                                waga++;
                            }

                        }
                        if (Convert.ToDecimal(l[0]) == fettle_3)
                        {
                            int waga = 0;
                            while (waga < 2)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));


                                waga++;
                            }

                        }

                    }

                }

                //Kończymy analizę.
                decimal naj_bialko = anal_bialko.Average();
                decimal naj_weglowodany = anal_weglowodany.Average();
                decimal naj_tluszcze = anal_tluszcze.Average();
                decimal naj_blonnik = anal_blonnik.Average();
                decimal naj_sodium = anal_sodium.Average();
                decimal naj_energia = anal_energia.Average();
                using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\ja\\predykcja_Lunch"))
                {
                    w.Write(naj_bialko + "|" + naj_weglowodany + "|" + naj_tluszcze + "|" + naj_blonnik + "|" + naj_sodium + "|" + naj_energia);

                }

                textBox28.Text = naj_bialko.ToString();
                textBox27.Text = naj_weglowodany.ToString();
                textBox26.Text = naj_tluszcze.ToString();
                textBox25.Text = naj_blonnik.ToString();
                textBox24.Text = naj_sodium.ToString();
                textBox29.Text = naj_energia.ToString();







            }



            if (comboBox5.Text == "Dinner")
            {
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");

                NumericComparer ns = new NumericComparer();
                //List<string> p = new List<string>();
                //object[] array = new object[listView1.Items.Count];
                //String[] posiłek = new String[fileEntries.Count()];
                //listView4.Items.CopyTo(items.ToString() ,0);
                int runs = 0;
                //foreach (ListViewItem i in listView4.Items)
                //{
                //    //for (int runs = 0; runs < listView4.Items.Count; runs++)
                //    //{
                //    items[runs] = i.Text + "|" + i.SubItems[1].Text;
                //    runs++;
                //    //}

                //}
                ////}
                //////listView1.Items.CopyTo(array, 0);
                //Array.Sort(items, ns);
                //listView4.Items.Clear();
                //    foreach(ListViewItem item in items) {
                //        listView4.Items.Add(item);

                //    }
                //foreach (ListViewItem i in listView4.Items) { 



                //}


                //listView4.Sort();
                //foreach (String i in items)
                //{
                //    string[] split = i.Split(new Char[] { '|' });
                //    //MessageBox.Show(i);
                //    //
                //    chart3.Series["w"].Points.AddXY(Convert.ToDecimal(split[0]), Convert.ToDecimal(split[1])); ;
                //}






                //MessageBox.Show("działam");
                //listView3.Items.Clear();

                foreach (string fileName in fileEntries)
                {
                    decimal ilosc_bi = 0;
                    decimal ilosc_weg = 0;
                    decimal ilosc_tlusz = 0;
                    decimal ilosc_blon = 0;
                    decimal ilosc_sod = 0;
                    decimal ilosc_ener = 0;
                    decimal samopocz = 0;
                    using (StreamReader writer = new StreamReader(fileName))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        DateTime data = new DateTime();


                        //
                        string[] fileEntries2 = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
                        foreach (string fileName2 in fileEntries2)
                        {
                            using (StreamReader writer2 = new StreamReader(fileName2))
                            {
                                string odczyt2 = writer2.ReadToEnd();

                                List<string> prod = new List<string>(odczyt2.Split(charSeparators));
                                //MessageBox.Show(l[2] + " : " + prod[0]);
                                if (l[1] == "Dinner")
                                {
                                    if (l[2] == prod[0])
                                    {
                                        //MessageBox.Show("działam");
                                        data = Convert.ToDateTime(l[0]);
                                        //ListViewItem item = new ListViewItem();
                                        ilosc_bi = Convert.ToDecimal(prod[2]) * Convert.ToDecimal(l[3]);
                                        ilosc_weg = Convert.ToDecimal(prod[3]) * Convert.ToDecimal(l[3]);
                                        ilosc_tlusz = Convert.ToDecimal(prod[4]) * Convert.ToDecimal(l[3]);
                                        ilosc_blon = Convert.ToDecimal(prod[5]) * Convert.ToDecimal(l[3]);
                                        ilosc_sod = Convert.ToDecimal(prod[6]) * Convert.ToDecimal(l[3]);
                                        ilosc_ener = Convert.ToDecimal(prod[8]) * Convert.ToDecimal(l[3]);
                                        if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                        {
                                            using (StreamReader writer3 = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                            {
                                                string odczyt3 = writer3.ReadToEnd();

                                                List<string> fettle = new List<string>(odczyt3.Split(charSeparators));
                                                decimal s1; decimal s2; decimal s3; decimal s4; decimal s5;
                                                if (fettle[0] != "-") { s1 = Convert.ToDecimal(fettle[0]); } else { s1 = 5; }
                                                if (fettle[1] != "-") { s2 = Convert.ToDecimal(fettle[1]); } else { s2 = 5; }
                                                if (fettle[2] != "-") { s3 = Convert.ToDecimal(fettle[2]); } else { s3 = 5; }
                                                if (fettle[3] != "-") { s4 = Convert.ToDecimal(fettle[3]); } else { s4 = 5; }
                                                if (fettle[4] != "-") { s5 = Convert.ToDecimal(fettle[4]); } else { s5 = 5; }
                                                samopocz = (s1 + s2 + s3 + s4 + s5) / 5;


                                            }
                                        }
                                        else { samopocz = 5; }
                                        //listView3.Items.Add(item);
                                        //for (int runs = 0; runs < listView4.Items.Count; runs++)
                                        //{
                                        using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\" + runs.ToString()))
                                        {
                                            w.Write(samopocz + "|" + ilosc_bi + "|" + ilosc_weg + "|" + ilosc_tlusz + "|" + ilosc_blon + "|" + ilosc_sod + "|" + ilosc_ener);
                                            runs++;
                                        }
                                        //}
                                    }
                                }

                            }

                        }
                    }


                }

                string[] an = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza");
                List<decimal> anal_sampoczucie = new List<decimal>();
                List<decimal> anal_bialko = new List<decimal>();
                List<decimal> anal_weglowodany = new List<decimal>();
                List<decimal> anal_tluszcze = new List<decimal>();
                List<decimal> anal_blonnik = new List<decimal>();
                List<decimal> anal_sodium = new List<decimal>();
                List<decimal> anal_energia = new List<decimal>();


                foreach (string f in an)
                {
                    using (StreamReader writer = new StreamReader(f))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));

                        anal_sampoczucie.Add(Convert.ToDecimal(l[0]));

                    }

                }
                //Ustalamy max samopoczucia i podmaxy.
                decimal fettle_1 = anal_sampoczucie.Max();
                decimal fettle_2 = 0;
                decimal fettle_3 = 0;

                foreach (decimal d in anal_sampoczucie)
                {
                    bool tomax = false;
                    if (fettle_1 == d) { tomax = true; }
                    if (tomax == false)
                    {
                        bool byla2 = false;
                        //MessageBox.Show("d= " + d);
                        if (fettle_2 == d) { byla2 = true; }
                        if (byla2 == false) { if (d > fettle_2) { fettle_3 = fettle_2; fettle_2 = d; byla2 = true; } }
                        if (byla2 == false) { if (d > fettle_3) { fettle_3 = d; } }
                        //
                        //MessageBox.Show(fettle_1 + " / " + fettle_2 + " / " + fettle_3);





                    }




                }

                foreach (string f in an)
                {
                    using (StreamReader writer = new StreamReader(f))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));

                        if (Convert.ToDecimal(l[0]) == fettle_1)
                        {
                            int waga = 0;
                            while (waga < 3)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));


                                waga++;
                            }

                        }

                        if (Convert.ToDecimal(l[0]) == fettle_2)
                        {
                            int waga = 0;
                            while (waga < 4)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));

                                //MessageBox.Show(l[1]);
                                waga++;
                            }

                        }
                        if (Convert.ToDecimal(l[0]) == fettle_3)
                        {
                            int waga = 0;
                            while (waga < 2)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));


                                waga++;
                            }

                        }

                    }

                }

                //Kończymy analizę.
                decimal naj_bialko = anal_bialko.Average();
                decimal naj_weglowodany = anal_weglowodany.Average();
                decimal naj_tluszcze = anal_tluszcze.Average();
                decimal naj_blonnik = anal_blonnik.Average();
                decimal naj_sodium = anal_sodium.Average();
                decimal naj_energia = anal_energia.Average();
                using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\ja\\predykcja_Dinner"))
                {
                    w.Write(naj_bialko + "|" + naj_weglowodany + "|" + naj_tluszcze + "|" + naj_blonnik + "|" + naj_sodium + "|" + naj_energia);

                }

                textBox28.Text = naj_bialko.ToString();
                textBox27.Text = naj_weglowodany.ToString();
                textBox26.Text = naj_tluszcze.ToString();
                textBox25.Text = naj_blonnik.ToString();
                textBox24.Text = naj_sodium.ToString();
                textBox29.Text = naj_energia.ToString();







            }


            if (comboBox5.Text == "Supper")
            {
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta");

                NumericComparer ns = new NumericComparer();
                //List<string> p = new List<string>();
                //object[] array = new object[listView1.Items.Count];
                //String[] posiłek = new String[fileEntries.Count()];
                //listView4.Items.CopyTo(items.ToString() ,0);
                int runs = 0;
                //foreach (ListViewItem i in listView4.Items)
                //{
                //    //for (int runs = 0; runs < listView4.Items.Count; runs++)
                //    //{
                //    items[runs] = i.Text + "|" + i.SubItems[1].Text;
                //    runs++;
                //    //}

                //}
                ////}
                //////listView1.Items.CopyTo(array, 0);
                //Array.Sort(items, ns);
                //listView4.Items.Clear();
                //    foreach(ListViewItem item in items) {
                //        listView4.Items.Add(item);

                //    }
                //foreach (ListViewItem i in listView4.Items) { 



                //}


                //listView4.Sort();
                //foreach (String i in items)
                //{
                //    string[] split = i.Split(new Char[] { '|' });
                //    //MessageBox.Show(i);
                //    //
                //    chart3.Series["w"].Points.AddXY(Convert.ToDecimal(split[0]), Convert.ToDecimal(split[1])); ;
                //}






                //MessageBox.Show("działam");
                //listView3.Items.Clear();

                foreach (string fileName in fileEntries)
                {
                    decimal ilosc_bi = 0;
                    decimal ilosc_weg = 0;
                    decimal ilosc_tlusz = 0;
                    decimal ilosc_blon = 0;
                    decimal ilosc_sod = 0;
                    decimal ilosc_ener = 0;
                    decimal samopocz = 0;
                    using (StreamReader writer = new StreamReader(fileName))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        DateTime data = new DateTime();


                        //
                        string[] fileEntries2 = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
                        foreach (string fileName2 in fileEntries2)
                        {
                            using (StreamReader writer2 = new StreamReader(fileName2))
                            {
                                string odczyt2 = writer2.ReadToEnd();

                                List<string> prod = new List<string>(odczyt2.Split(charSeparators));
                                //MessageBox.Show(l[2] + " : " + prod[0]);
                                if (l[1] == "Supper")
                                {
                                    if (l[2] == prod[0])
                                    {
                                        //MessageBox.Show("działam");
                                        data = Convert.ToDateTime(l[0]);
                                        //ListViewItem item = new ListViewItem();
                                        ilosc_bi = Convert.ToDecimal(prod[2]) * Convert.ToDecimal(l[3]);
                                        ilosc_weg = Convert.ToDecimal(prod[3]) * Convert.ToDecimal(l[3]);
                                        ilosc_tlusz = Convert.ToDecimal(prod[4]) * Convert.ToDecimal(l[3]);
                                        ilosc_blon = Convert.ToDecimal(prod[5]) * Convert.ToDecimal(l[3]);
                                        ilosc_sod = Convert.ToDecimal(prod[6]) * Convert.ToDecimal(l[3]);
                                        ilosc_ener = Convert.ToDecimal(prod[8]) * Convert.ToDecimal(l[3]);
                                        if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                        {
                                            using (StreamReader writer3 = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle\\" + data.Year + data.Month + data.Day))
                                            {
                                                string odczyt3 = writer3.ReadToEnd();

                                                List<string> fettle = new List<string>(odczyt3.Split(charSeparators));
                                                decimal s1; decimal s2; decimal s3; decimal s4; decimal s5;
                                                if (fettle[0] != "-") { s1 = Convert.ToDecimal(fettle[0]); } else { s1 = 5; }
                                                if (fettle[1] != "-") { s2 = Convert.ToDecimal(fettle[1]); } else { s2 = 5; }
                                                if (fettle[2] != "-") { s3 = Convert.ToDecimal(fettle[2]); } else { s3 = 5; }
                                                if (fettle[3] != "-") { s4 = Convert.ToDecimal(fettle[3]); } else { s4 = 5; }
                                                if (fettle[4] != "-") { s5 = Convert.ToDecimal(fettle[4]); } else { s5 = 5; }
                                                samopocz = (s1 + s2 + s3 + s4 + s5) / 5;


                                            }
                                        }
                                        else { samopocz = 5; }
                                        //listView3.Items.Add(item);
                                        //for (int runs = 0; runs < listView4.Items.Count; runs++)
                                        //{
                                        using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\" + runs.ToString()))
                                        {
                                            w.Write(samopocz + "|" + ilosc_bi + "|" + ilosc_weg + "|" + ilosc_tlusz + "|" + ilosc_blon + "|" + ilosc_sod + "|" + ilosc_ener);
                                            runs++;
                                        }
                                        //}
                                    }
                                }

                            }

                        }
                    }


                }

                string[] an = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza");
                List<decimal> anal_sampoczucie = new List<decimal>();
                List<decimal> anal_bialko = new List<decimal>();
                List<decimal> anal_weglowodany = new List<decimal>();
                List<decimal> anal_tluszcze = new List<decimal>();
                List<decimal> anal_blonnik = new List<decimal>();
                List<decimal> anal_sodium = new List<decimal>();
                List<decimal> anal_energia = new List<decimal>();


                foreach (string f in an)
                {
                    using (StreamReader writer = new StreamReader(f))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));

                        anal_sampoczucie.Add(Convert.ToDecimal(l[0]));

                    }

                }
                //Ustalamy max samopoczucia i podmaxy.
                decimal fettle_1 = anal_sampoczucie.Max();
                decimal fettle_2 = 0;
                decimal fettle_3 = 0;

                foreach (decimal d in anal_sampoczucie)
                {
                    bool tomax = false;
                    if (fettle_1 == d) { tomax = true; }
                    if (tomax == false)
                    {
                        bool byla2 = false;
                        //MessageBox.Show("d= " + d);
                        if (fettle_2 == d) { byla2 = true; }
                        if (byla2 == false) { if (d > fettle_2) { fettle_3 = fettle_2; fettle_2 = d; byla2 = true; } }
                        if (byla2 == false) { if (d > fettle_3) { fettle_3 = d; } }
                        //
                        //MessageBox.Show(fettle_1 + " / " + fettle_2 + " / " + fettle_3);





                    }




                }

                foreach (string f in an)
                {
                    using (StreamReader writer = new StreamReader(f))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));

                        if (Convert.ToDecimal(l[0]) == fettle_1)
                        {
                            int waga = 0;
                            while (waga < 3)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));


                                waga++;
                            }

                        }

                        if (Convert.ToDecimal(l[0]) == fettle_2)
                        {
                            int waga = 0;
                            while (waga < 4)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));

                                //MessageBox.Show(l[1]);
                                waga++;
                            }

                        }
                        if (Convert.ToDecimal(l[0]) == fettle_3)
                        {
                            int waga = 0;
                            while (waga < 2)
                            {
                                anal_bialko.Add(Convert.ToDecimal(l[1]));
                                anal_weglowodany.Add(Convert.ToDecimal(l[2]));
                                anal_tluszcze.Add(Convert.ToDecimal(l[3]));
                                anal_blonnik.Add(Convert.ToDecimal(l[4]));
                                anal_sodium.Add(Convert.ToDecimal(l[5]));
                                anal_energia.Add(Convert.ToDecimal(l[6]));


                                waga++;
                            }

                        }

                    }

                }

                //Kończymy analizę.
                decimal naj_bialko = anal_bialko.Average();
                decimal naj_weglowodany = anal_weglowodany.Average();
                decimal naj_tluszcze = anal_tluszcze.Average();
                decimal naj_blonnik = anal_blonnik.Average();
                decimal naj_sodium = anal_sodium.Average();
                decimal naj_energia = anal_energia.Average();
                using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\ja\\predykcja_Supper"))
                {
                    w.Write(naj_bialko + "|" + naj_weglowodany + "|" + naj_tluszcze + "|" + naj_blonnik + "|" + naj_sodium + "|" + naj_energia);

                }

                textBox28.Text = naj_bialko.ToString();
                textBox27.Text = naj_weglowodany.ToString();
                textBox26.Text = naj_tluszcze.ToString();
                textBox25.Text = naj_blonnik.ToString();
                textBox24.Text = naj_sodium.ToString();
                textBox29.Text = naj_energia.ToString();







            }











        }





































        private void button9_Click_1(object sender, EventArgs e)
        {
            button9.Enabled = false;
            if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty\\" + listView2.SelectedItems[0].SubItems[9].Text)) { File.Delete(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty\\" + listView2.SelectedItems[0].SubItems[9].Text); }
            Delete(Convert.ToInt32(listView2.SelectedItems[0].SubItems[9].Text));
            odswiezprodukty();
        }

        private void button10_Click_2(object sender, EventArgs e)
        {
            analiza_dobowa();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            analiza_dobowa();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (jestanaliza == false) { }
            else
            {
                if (comboBox6.Text != "") { comboBox7.Enabled = true; numericUpDown11.Enabled = true; button10.Enabled = true; } else { comboBox7.Enabled = false; numericUpDown11.Enabled = false; }
                if (jestanaliza == true)
                {
                    if (comboBox6.Text == "Whole Day")
                    {
                        using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\calydzien"))
                        {
                            string odczyt = writer.ReadToEnd();
                            char[] charSeparators = new char[] { '|' };
                            List<string> l = new List<string>(odczyt.Split(charSeparators));
                            textBox28.Text = l[0]; textBox39.Text = l[0];
                            textBox27.Text = l[1]; textBox38.Text = l[1];
                            textBox26.Text = l[2]; textBox37.Text = l[2];
                            textBox29.Text = l[3]; textBox40.Text = l[3];
                            textBox25.Text = l[4]; textBox36.Text = l[4];
                            textBox24.Text = l[5]; textBox35.Text = l[5];
                        }
                    }
                    if (comboBox6.Text == "Breakfest")
                    {
                        using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\sniad"))
                        {
                            string odczyt = writer.ReadToEnd();
                            char[] charSeparators = new char[] { '|' };
                            List<string> l = new List<string>(odczyt.Split(charSeparators));
                            textBox28.Text = l[0]; textBox39.Text = l[0];
                            textBox27.Text = l[1]; textBox38.Text = l[1];
                            textBox26.Text = l[2]; textBox37.Text = l[2];
                            textBox29.Text = l[3]; textBox40.Text = l[3];
                            textBox25.Text = l[4]; textBox36.Text = l[4];
                            textBox24.Text = l[5]; textBox35.Text = l[5];
                        }
                    }
                    if (comboBox6.Text == "II Breakfest")
                    {
                        using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\iisniad"))
                        {
                            string odczyt = writer.ReadToEnd();
                            char[] charSeparators = new char[] { '|' };
                            List<string> l = new List<string>(odczyt.Split(charSeparators));
                            textBox28.Text = l[0]; textBox39.Text = l[0];
                            textBox27.Text = l[1]; textBox38.Text = l[1];
                            textBox26.Text = l[2]; textBox37.Text = l[2];
                            textBox29.Text = l[3]; textBox40.Text = l[3];
                            textBox25.Text = l[4]; textBox36.Text = l[4];
                            textBox24.Text = l[5]; textBox35.Text = l[5];
                        }
                    }
                    if (comboBox6.Text == "Lunch")
                    {
                        using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\Lunch"))
                        {
                            string odczyt = writer.ReadToEnd();
                            char[] charSeparators = new char[] { '|' };
                            List<string> l = new List<string>(odczyt.Split(charSeparators));
                            textBox28.Text = l[0]; textBox39.Text = l[0];
                            textBox27.Text = l[1]; textBox38.Text = l[1];
                            textBox26.Text = l[2]; textBox37.Text = l[2];
                            textBox29.Text = l[3]; textBox40.Text = l[3];
                            textBox25.Text = l[4]; textBox36.Text = l[4];
                            textBox24.Text = l[5]; textBox35.Text = l[5];
                        }
                    }
                    if (comboBox6.Text == "podwieczorku")
                    {
                        using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\podwiecz"))
                        {
                            string odczyt = writer.ReadToEnd();
                            char[] charSeparators = new char[] { '|' };
                            List<string> l = new List<string>(odczyt.Split(charSeparators));
                            textBox28.Text = l[0]; textBox39.Text = l[0];
                            textBox27.Text = l[1]; textBox38.Text = l[1];
                            textBox26.Text = l[2]; textBox37.Text = l[2];
                            textBox29.Text = l[3]; textBox40.Text = l[3];
                            textBox25.Text = l[4]; textBox36.Text = l[4];
                            textBox24.Text = l[5]; textBox35.Text = l[5];
                        }
                    }
                    if (comboBox6.Text == "kolacji")
                    {
                        using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\kol"))
                        {
                            string odczyt = writer.ReadToEnd();
                            char[] charSeparators = new char[] { '|' };
                            List<string> l = new List<string>(odczyt.Split(charSeparators));
                            textBox28.Text = l[0]; textBox39.Text = l[0];
                            textBox27.Text = l[1]; textBox38.Text = l[1];
                            textBox26.Text = l[2]; textBox37.Text = l[2];
                            textBox29.Text = l[3]; textBox40.Text = l[3];
                            textBox25.Text = l[4]; textBox36.Text = l[4];
                            textBox24.Text = l[5]; textBox35.Text = l[5];
                        }
                    }
                }
                bool spelniawymagania = true;
                if (textBox39.Text == "0") { spelniawymagania = false; }
                if (textBox38.Text == "0") { spelniawymagania = false; }
                if (textBox37.Text == "0") { spelniawymagania = false; }
                if (textBox40.Text == "0") { spelniawymagania = false; }
                if (textBox36.Text == "0") { spelniawymagania = false; }
                if (textBox35.Text == "0") { spelniawymagania = false; }

                if (spelniawymagania == false)
                {
                    listView5.Items.Clear();

                    comboBox7.Text = "";
                    numericUpDown11.Value = 1;

                    textBox39.Text = ""; textBox38.Text = ""; textBox37.Text = "";
                    textBox40.Text = ""; textBox36.Text = ""; textBox35.Text = "";
                    textBox23.Text = ""; textBox34.Text = ""; textBox33.Text = "";
                    textBox32.Text = ""; textBox31.Text = ""; textBox30.Text = "";
                    textBox41.Text = ""; textBox42.Text = ""; textBox43.Text = "";
                    textBox44.Text = ""; textBox45.Text = ""; textBox46.Text = "";
                    label79.Text = "00 %";
                    progressBar1.Value = 0;
                    button10.Enabled = false; comboBox6.Text = "";
                    MessageBox.Show("The ammount of data is too low for good prediction. Please enter more data about your fettle and diet. | Niestety, jest jeszcze wprowadzona za mała ilość danych na temat twojej diety i samopoczucia do wiarygodnej predykcji. Wprowadź dane, a następnie wygeneruj swój najlepszy profil.");
                }

            }
        }

        private void button10_Click_3(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                if (comboBox7.Text == "") { MessageBox.Show("What would you like to eat?"); }
                else
                {
                    button17.Enabled = true;
                    ListViewItem dod = new ListViewItem();
                    dod.Text = comboBox7.Text;
                    dod.SubItems.Add(numericUpDown11.Value.ToString());
                    //dod.SubItems[2].Text = comboBox6.Text;
                    listView5.Items.Add(dod);

                    odswiezprodukty();
                    //Sumowanie
                    decimal bialko = 0; decimal weglowodany = 0; decimal tluszcze = 0; decimal blonnik = 0; decimal sodium = 0; decimal energy = 0; decimal waga = 0;
                    //bool sniadanie = false; bool sniadanie2 = false; bool Lunch = false; bool Dinner = false; bool Supper = false;

                    foreach (ListViewItem dieta in listView5.Items)
                    {
                        //                Breakfest
                        //II Breakfest
                        //Lunch
                        //Dinner
                        //Supper

                        //if (dieta.SubItems[2].Text == "Breakfest") { sniadanie = true; }
                        //if (dieta.SubItems[2].Text == "II Breakfest") { sniadanie2 = true; }
                        //if (dieta.SubItems[2].Text == "Lunch") { Lunch = true; }
                        //if (dieta.SubItems[2].Text == "Dinner") { Dinner = true; }
                        //if (dieta.SubItems[2].Text == "Supper") { Supper = true; }





                        foreach (ListViewItem produkt in listView2.Items)
                        {
                            if (dieta.Text == produkt.Text)
                            {
                                //MessageBox.Show(produkt.SubItems[7].Text);
                                //weight = weight + (Convert.ToDecimal(produkt.SubItems[7].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                //MessageBox.Show(weight.ToString());
                                waga = waga + (Convert.ToDecimal(produkt.SubItems[7].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                bialko = bialko + (Convert.ToDecimal(produkt.SubItems[2].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                weglowodany = weglowodany + (Convert.ToDecimal(produkt.SubItems[3].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                tluszcze = tluszcze + (Convert.ToDecimal(produkt.SubItems[4].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                blonnik = blonnik + (Convert.ToDecimal(produkt.SubItems[5].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                sodium = sodium + (Convert.ToDecimal(produkt.SubItems[6].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                energy = energy + (Convert.ToDecimal(produkt.SubItems[8].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                            }
                        }
                    }
                    //textBox12.Text = weight.ToString();
                    textBox33.Text = bialko.ToString();
                    textBox32.Text = weglowodany.ToString();
                    textBox31.Text = tluszcze.ToString();
                    textBox30.Text = blonnik.ToString();
                    textBox23.Text = sodium.ToString();
                    textBox34.Text = energy.ToString();
                    textBox49.Text = waga.ToString();
                    textBox48.Text = "No analysis conducted yet. Click on button above.";
                }


            }
            if (radioButton2.Checked)
            {
                if (jestanaliza == true)
                {
                    if (comboBox7.Text == "") { MessageBox.Show("What whould you like to eat?"); }
                    else
                    {
                        ListViewItem dod = new ListViewItem();
                        dod.Text = comboBox7.Text;
                        dod.SubItems.Add(numericUpDown11.Value.ToString());
                        //dod.SubItems[2].Text = comboBox6.Text;
                        listView5.Items.Add(dod);

                        odswiezprodukty();
                        //Sumowanie
                        decimal bialko = 0; decimal weglowodany = 0; decimal tluszcze = 0; decimal blonnik = 0; decimal sodium = 0; decimal energy = 0; decimal waga = 0;
                        //bool sniadanie = false; bool sniadanie2 = false; bool Lunch = false; bool Dinner = false; bool Supper = false;

                        foreach (ListViewItem dieta in listView5.Items)
                        {
                            //                Breakfest
                            //II Breakfest
                            //Lunch
                            //Dinner
                            //Supper

                            //if (dieta.SubItems[2].Text == "Breakfest") { sniadanie = true; }
                            //if (dieta.SubItems[2].Text == "II Breakfest") { sniadanie2 = true; }
                            //if (dieta.SubItems[2].Text == "Lunch") { Lunch = true; }
                            //if (dieta.SubItems[2].Text == "Dinner") { Dinner = true; }
                            //if (dieta.SubItems[2].Text == "Supper") { Supper = true; }





                            foreach (ListViewItem produkt in listView2.Items)
                            {
                                if (dieta.Text == produkt.Text)
                                {
                                    //MessageBox.Show(produkt.SubItems[7].Text);
                                    //weight = weight + (Convert.ToDecimal(produkt.SubItems[7].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                    //MessageBox.Show(weight.ToString());
                                    waga = waga + (Convert.ToDecimal(produkt.SubItems[7].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                    bialko = bialko + (Convert.ToDecimal(produkt.SubItems[2].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                    weglowodany = weglowodany + (Convert.ToDecimal(produkt.SubItems[3].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                    tluszcze = tluszcze + (Convert.ToDecimal(produkt.SubItems[4].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                    blonnik = blonnik + (Convert.ToDecimal(produkt.SubItems[5].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                    sodium = sodium + (Convert.ToDecimal(produkt.SubItems[6].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                    energy = energy + (Convert.ToDecimal(produkt.SubItems[8].Text) * Convert.ToDecimal(dieta.SubItems[1].Text));
                                }
                            }
                        }
                        //textBox12.Text = weight.ToString();
                        textBox33.Text = bialko.ToString();
                        textBox32.Text = weglowodany.ToString();
                        textBox31.Text = tluszcze.ToString();
                        textBox30.Text = blonnik.ToString();
                        textBox23.Text = sodium.ToString();
                        textBox34.Text = energy.ToString();
                        textBox49.Text = waga.ToString();








                        //Analiza zysków i strat
                        decimal proc_bi = wartośćbezwzgledna(Convert.ToDecimal(textBox39.Text) - Convert.ToDecimal(textBox33.Text)) * 100 / Convert.ToDecimal(textBox39.Text);
                        decimal proc_w = wartośćbezwzgledna(Convert.ToDecimal(textBox38.Text) - Convert.ToDecimal(textBox32.Text)) * 100 / Convert.ToDecimal(textBox38.Text);
                        decimal proc_t = wartośćbezwzgledna(Convert.ToDecimal(textBox37.Text) - Convert.ToDecimal(textBox31.Text)) * 100 / Convert.ToDecimal(textBox37.Text);
                        decimal proc_bł = wartośćbezwzgledna(Convert.ToDecimal(textBox36.Text) - Convert.ToDecimal(textBox30.Text)) * 100 / Convert.ToDecimal(textBox36.Text);
                        decimal proc_s = wartośćbezwzgledna(Convert.ToDecimal(textBox35.Text) - Convert.ToDecimal(textBox23.Text)) * 100 / Convert.ToDecimal(textBox35.Text);
                        decimal proc_e = wartośćbezwzgledna(Convert.ToDecimal(textBox40.Text) - Convert.ToDecimal(textBox34.Text)) * 100 / Convert.ToDecimal(textBox40.Text);

                        textBox45.Text = proc_bi.ToString(); textBox44.Text = proc_w.ToString(); textBox43.Text = proc_t.ToString();
                        textBox42.Text = proc_bł.ToString(); textBox41.Text = proc_s.ToString(); textBox46.Text = proc_e.ToString();

                        decimal efektfettle = Math.Round(100 - ((proc_bi + proc_w + proc_t + proc_bł + proc_s + proc_e) / 6), 0);
                        if (efektfettle < 0)
                        {
                            MessageBox.Show("This meal is so much different than any another meal in your history that I should show you: " + efektfettle.ToString() + " %.");

                            label79.Text = "0 %";
                            progressBar1.Value = 0;
                        }
                        else
                        {
                            label79.Text = efektfettle.ToString() + " %";
                            progressBar1.Value = Convert.ToInt32(efektfettle);
                        }


                    }
                }
                else { MessageBox.Show("There is no analysis of profile conducted yet. | Nie przeprowadzono jeszcze ani jednej analizy profilu! Możesz to zrobic w zakładce \"Analiza Ilościowa\". Zawsze, gdy dodasz więcej nowych danych nie zapomnij o generowaniu profilu! (tylko w analizie odchyleniowej)"); }
            }
        }
        private decimal wartośćbezwzgledna(decimal x)
        {
            decimal wynik;
            if (x >= 0)
            { wynik = x; }
            else
            { wynik = -x; }
            return (wynik);

        }

        private void button11_Click(object sender, EventArgs e)
        {
            comboBox6.Text = ""; textBox49.Text = "";
            listView5.Items.Clear();
            textBox48.Text = "What do you want to eat? | Co chcesz zjeść? Wprowadź produkty i ich ilości aby wykonać analizę.";
            progressBar3.Value = 0;
            button17.Enabled = false;
            comboBox7.Text = "";
            numericUpDown11.Value = 1;
            button10.Enabled = false; comboBox7.Enabled = false;
            textBox39.Text = ""; textBox38.Text = ""; textBox37.Text = "";
            textBox40.Text = ""; textBox36.Text = ""; textBox35.Text = "";
            textBox23.Text = ""; textBox34.Text = ""; textBox33.Text = "";
            textBox32.Text = ""; textBox31.Text = ""; textBox30.Text = "";
            textBox41.Text = ""; textBox42.Text = ""; textBox43.Text = "";
            textBox44.Text = ""; textBox45.Text = ""; textBox46.Text = ""; 
            label79.Text = "00 %";
            progressBar1.Value = 0;
            button10.Enabled = true; comboBox6.Text = "";
            comboBox6.SelectedIndex = -1; comboBox7.SelectedIndex = -1;
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listView1.Sort();
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listView2.Sort();
        }

        private void listView4_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listView4.Sort();
        }

        private void listView5_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listView5.Sort();
        }

        private void Grid_Click(object sender, EventArgs e)
        {
            //i = Convert.ToInt32(DT.Rows[Grid.CurrentRowIndex]["id"]);

            //btnDel.Enabled = true;
            //btnEdit.Enabled = true;
            //txtDesc.Text = DT.Rows[Grid.CurrentRowIndex]["desc"].ToString();
        }

        private void listView2_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count == 1)
            {
                foreach (ListViewItem i in listView2.Items)
                {
                    if (i.Selected == true)
                    {
                        textBox17.Text = i.Text;
                        textBox16.Text = i.SubItems[1].Text;
                        numericUpDown4.Value = Convert.ToDecimal(i.SubItems[2].Text);
                        numericUpDown5.Value = Convert.ToDecimal(i.SubItems[3].Text);
                        numericUpDown6.Value = Convert.ToDecimal(i.SubItems[4].Text);
                        numericUpDown7.Value = Convert.ToDecimal(i.SubItems[5].Text);
                        numericUpDown8.Value = Convert.ToDecimal(i.SubItems[6].Text);
                        numericUpDown9.Value = Convert.ToDecimal(i.SubItems[7].Text);
                        numericUpDown10.Value = Convert.ToDecimal(i.SubItems[8].Text);
                    }
                }
            }
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            odswiezprodukty();
            if (comboBox8.Text == "widok standardowy") { Grid.Visible = false; listView2.Visible = true; groupBox2.Enabled = true; button7.Enabled = true; }
            if (comboBox8.Text == "przegląd bazy") { Grid.Visible = true; listView2.Visible = false; groupBox2.Enabled = false; button9.Enabled = false; button7.Enabled = false; }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            // Set filter options and filter index.
            saveFileDialog1.Title = "Eksportuj bazę produktów.";
            saveFileDialog1.Filter = "Plik bazy produktów programu PancreAppPC (*.pancreapp)|*.pancreapp";
            saveFileDialog1.FilterIndex = 1;



            // Call the ShowDialog method to show the dialog box.
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {

                //if (!Directory.Exists(Application.StartupPath + katalogpacjenta + "\\pliki")) { Directory.CreateDirectory(Application.StartupPath + katalogpacjenta + "\\pliki") }
                File.Copy(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty.db", saveFileDialog1.FileName);





            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Importuj bazę produktów.";
            openFileDialog1.Filter = "Plik bazy produktów programu PancreAppPC (*.pancreapp)|*.pancreapp";
            openFileDialog1.FilterIndex = 1;



            // Call the ShowDialog method to show the dialog box.
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {

                //if (!Directory.Exists(Application.StartupPath + katalogpacjenta + "\\pliki")) { Directory.CreateDirectory(Application.StartupPath + katalogpacjenta + "\\pliki") }
                File.Copy(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty.db", System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty_old.db", true);
                File.Copy(openFileDialog1.FileName, System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty.db", true);
                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");
                foreach (string f in fileEntries) { File.Delete(f); }
                odswiezprodukty();



            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Dodaj produkty z bazy produktów.";
            openFileDialog1.Filter = "Plik bazy produktów programu PancreAppPC (*.pancreapp)|*.pancreapp";
            openFileDialog1.FilterIndex = 1;



            // Call the ShowDialog method to show the dialog box.
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {

                //if (!Directory.Exists(Application.StartupPath + katalogpacjenta + "\\pliki")) { Directory.CreateDirectory(Application.StartupPath + katalogpacjenta + "\\pliki") }
                File.Copy(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty.db", System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty_old.db", true);
                File.Copy(openFileDialog1.FileName, System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty.db", true);
                LoadData(); int id = 0;
                List<string> lista = new List<string>();
                bool mamid = false;
                while (mamid == false)
                {
                    if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty\\" + id.ToString()))
                    {
                        mamid = false; id++;
                    }
                    else
                    {
                        mamid = true;
                    }
                }

                foreach (DataGridViewRow row in Grid.Rows)
                {


                    //using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty\\" + id.ToString()))
                    //{
                    //    writer.Write(row.Cells[1].Value);
                    //    writer.Write("|");
                    //    writer.Write(row.Cells[2].Value);
                    //    writer.Write("|");

                    //    writer.Write(row.Cells[5].Value);
                    //    writer.Write("|");
                    //    writer.Write(row.Cells[6].Value);
                    //    writer.Write("|");
                    //    writer.Write(row.Cells[7].Value);
                    //    writer.Write("|");
                    //    writer.Write(row.Cells[8].Value);
                    //    writer.Write("|");
                    //    writer.Write(row.Cells[9].Value);
                    //    writer.Write("|");
                    //    writer.Write(row.Cells[3].Value);
                    //    writer.Write("|");
                    //    writer.Write(row.Cells[4].Value);
                    //}
                    //UZNAJ WYŻSZOŚC PLIKÓW NAD BAZĄ
                    //MessageBox.Show(id.ToString());
                    //File.Copy(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty_old.db", System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty.db", true);
                    //string txtSQLQuery = "insert into  Produkty (id,nazwa,jednostka,masajednostki,energia,bialko,weglowodany,tluszcz,blonnik,sod,kodkreskowy) values ('" + id.ToString() + "','" + row.Cells[1].Value + "','" + row.Cells[2].Value + "','" + row.Cells[3].Value + "','" + row.Cells[4].Value + "','" + row.Cells[5].Value + "','" + row.Cells[5].Value + "','" + row.Cells[6].Value + "','" + row.Cells[7].Value + "','" + row.Cells[8].Value + "','" + row.Cells[9].Value + "')";
                    //ExecuteQuery(txtSQLQuery);

                    lista.Add("insert into  Produkty (id,nazwa,jednostka,masajednostki,energia,bialko,weglowodany,tluszcz,blonnik,sod,kodkreskowy) values ('" + id.ToString() + "','" + row.Cells[1].Value + "','" + row.Cells[2].Value + "','" + row.Cells[3].Value + "','" + row.Cells[4].Value + "','" + row.Cells[5].Value + "','" + row.Cells[5].Value + "','" + row.Cells[6].Value + "','" + row.Cells[7].Value + "','" + row.Cells[8].Value + "','" + row.Cells[9].Value + "')");
                    id++;
                    bool mamid2 = false;
                    while (mamid2 == false)
                    {
                        if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty\\" + id.ToString()))
                        {
                            mamid2 = false; id++;
                        }
                        else
                        {
                            mamid2 = true;
                        }
                    }
                }
                File.Copy(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty_old.db", System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty.db", true); LoadData();
                foreach (string s in lista) { ExecuteQuery(s); }





                string[] fileEntries = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty");





                foreach (string f in fileEntries) { File.Delete(f); }
                odswiezprodukty();



            }
        }

        private void listView2_MouseMove(object sender, MouseEventArgs e)
        {
            if (listView2.SelectedItems.Count != 0) { button9.Enabled = true; } else { button9.Enabled = false; }
            if (comboBox8.Text == "widok standardowy") { Grid.Visible = false; listView2.Visible = true; groupBox2.Enabled = true; button7.Enabled = true; }
            if (comboBox8.Text == "przegląd bazy") { Grid.Visible = true; listView2.Visible = false; groupBox2.Enabled = false; button9.Enabled = false; button7.Enabled = false; }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            mójprofil();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            mójprofil();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            downPoint = new Point(e.X, e.Y);
        }
        public Point downPoint = Point.Empty;

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            downPoint = Point.Empty;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (downPoint == Point.Empty)
            {
                return;
            }
            Point location = new Point(
                this.Left + e.X - downPoint.X,
                this.Top + e.Y - downPoint.Y);
            this.Location = location;
        }

        private void label85_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            downPoint = new Point(e.X, e.Y);
        }

        private void label85_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            downPoint = Point.Empty;
        }

        private void label85_MouseMove(object sender, MouseEventArgs e)
        {
            if (downPoint == Point.Empty)
            {
                return;
            }
            Point location = new Point(
                this.Left + e.X - downPoint.X,
                this.Top + e.Y - downPoint.Y);
            this.Location = location;
        }

        private void tabPage1_MouseEnter(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\ja.txt"))
            {
                writer.Write(textBox1.Text);
                writer.Write("|");
                writer.Write(dateTimePicker1.Value.ToString());
                writer.Write("|");
                writer.Write(numericUpDown1.Value.ToString());
                writer.Write("|");
                writer.Write(numericUpDown2.Value.ToString());
                writer.Write("|");
                writer.Write(textBox2.Text);
            }
        }

        private void tabPage1_MouseLeave(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\ja.txt"))
            {
                writer.Write(textBox1.Text);
                writer.Write("|");
                writer.Write(dateTimePicker1.Value.ToString());
                writer.Write("|");
                writer.Write(numericUpDown1.Value.ToString());
                writer.Write("|");
                writer.Write(numericUpDown2.Value.ToString());
                writer.Write("|");
                writer.Write(textBox2.Text);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you want to close the application? | Na pewno chcesz zakończyć działanie programu?", "Zamknij Program", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {


            }
            else
            {
                e.Cancel = true;
                this.Activate();
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void mojeDaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }

        private void mojeProduktyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }

        private void mojefettleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage5;
        }

        private void dziennaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage4;
        }

        private void ilościowaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage6;
        }

        private void prognozaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage7;
        }

        private void oProgramieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage8;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void tabPage2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void tabPage5_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void tabPage3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void tabPage4_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void tabPage6_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void tabPage7_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void tabPage8_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void label3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void chart2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void chart3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void label83_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void label85_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(MousePosition.X, MousePosition.Y);
                contextMenuStrip1.Show(p);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click_2(object sender, EventArgs e)
        {
            MessageBox.Show("This analyzer seeks to determine what amount of consumed components is associated with the achievement of the highest level of fettle. In contrast to the another method, the linearity of the correlation found between the points of the source is assumed.");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            wygenerujprofil();
        }
        private void wygenerujprofil()
        {
            if (MessageBox.Show("Would you like to conduct the analysis of profile? It may take a while. | Chcesz przeprowadzić analizę i wyznaczenie profilu żywieniowego? Zależenie od ilości danych może to trwać od kilku sekund do kliku minut. Stary profil zostanie utracony.", "Potwierdź.", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                progressBar2.Value = 0;
                Size size = new Size(693, 507);
                progressBar2.Size = size; progressBar2.Visible = true; ;
                //Tworzymy listy danych
                //Whole day
                //protein
                List<string> calydzienibialko = new List<string>();
                comboBox3.Text = "Whole day";
                comboBox4.Text = "protein";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    calydzienibialko.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //foreach (string s in calydzienibialko) { MessageBox.Show(s); }
                //        carbohydrates
                List<string> calydzieniw = new List<string>();
                comboBox3.Text = "Whole day";
                comboBox4.Text = "carbohydrates";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    calydzieniw.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //Wartość energetyczna
                List<string> calydzieniener = new List<string>();
                comboBox3.Text = "Whole day";
                comboBox4.Text = "Wartość energetyczna";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    calydzieniener.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //Tłuszcz
                List<string> calydzienit = new List<string>();
                comboBox3.Text = "Whole day";
                comboBox4.Text = "Tłuszcz";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    calydzienit.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //roughage
                List<string> calydzieniblonnik = new List<string>();
                comboBox3.Text = "Whole day";
                comboBox4.Text = "roughage";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    calydzieniblonnik.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //sodium
                List<string> calydzienis = new List<string>();
                comboBox3.Text = "Whole day";
                comboBox4.Text = "sodium";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    calydzienis.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }

                progressBar2.Value++;
                //Breakfest
                //protein
                List<string> sniadibialko = new List<string>();
                comboBox3.Text = "Breakfest";
                comboBox4.Text = "protein";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    sniadibialko.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //foreach (string s in sniadibialko) { MessageBox.Show(s); }
                //        carbohydrates
                List<string> sniadiw = new List<string>();
                comboBox3.Text = "Breakfest";
                comboBox4.Text = "carbohydrates";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    sniadiw.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //Wartość energetyczna
                List<string> sniadiener = new List<string>();
                comboBox3.Text = "Breakfest";
                comboBox4.Text = "Wartość energetyczna";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    sniadiener.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //Tłuszcz
                List<string> sniadit = new List<string>();
                comboBox3.Text = "Breakfest";
                comboBox4.Text = "Tłuszcz";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    sniadit.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //roughage
                List<string> sniadiblonnik = new List<string>();
                comboBox3.Text = "Breakfest";
                comboBox4.Text = "roughage";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    sniadiblonnik.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //sodium
                List<string> sniadis = new List<string>();
                comboBox3.Text = "Breakfest";
                comboBox4.Text = "sodium";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    sniadis.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                progressBar2.Value++;

                //II Breakfest
                //II Breakfest
                //protein
                List<string> iisniadibialko = new List<string>();
                comboBox3.Text = "II Breakfest";
                comboBox4.Text = "protein";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    iisniadibialko.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //foreach (string s in iisniadibialko) { MessageBox.Show(s); }
                //        carbohydrates
                List<string> iisniadiw = new List<string>();
                comboBox3.Text = "II Breakfest";
                comboBox4.Text = "carbohydrates";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    iisniadiw.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //Wartość energetyczna
                List<string> iisniadiener = new List<string>();
                comboBox3.Text = "II Breakfest";
                comboBox4.Text = "Wartość energetyczna";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    iisniadiener.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //Tłuszcz
                List<string> iisniadit = new List<string>();
                comboBox3.Text = "II Breakfest";
                comboBox4.Text = "Tłuszcz";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    iisniadit.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //roughage
                List<string> iisniadiblonnik = new List<string>();
                comboBox3.Text = "II Breakfest";
                comboBox4.Text = "roughage";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    iisniadiblonnik.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //sodium
                List<string> iisniadis = new List<string>();
                comboBox3.Text = "II Breakfest";
                comboBox4.Text = "sodium";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    iisniadis.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }


                //Lunch
                //Lunch
                //protein
                List<string> Lunchibialko = new List<string>();
                comboBox3.Text = "Lunch";
                comboBox4.Text = "protein";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    Lunchibialko.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //foreach (string s in Lunchibialko) { MessageBox.Show(s); }
                //        carbohydrates
                List<string> Lunchiw = new List<string>();
                comboBox3.Text = "Lunch";
                comboBox4.Text = "carbohydrates";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    Lunchiw.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //Wartość energetyczna
                List<string> Lunchiener = new List<string>();
                comboBox3.Text = "Lunch";
                comboBox4.Text = "Wartość energetyczna";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    Lunchiener.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //Tłuszcz
                List<string> Lunchit = new List<string>();
                comboBox3.Text = "Lunch";
                comboBox4.Text = "Tłuszcz";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    Lunchit.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //roughage
                List<string> Lunchiblonnik = new List<string>();
                comboBox3.Text = "Lunch";
                comboBox4.Text = "roughage";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    Lunchiblonnik.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //sodium
                List<string> Lunchis = new List<string>();
                comboBox3.Text = "Lunch";
                comboBox4.Text = "sodium";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    Lunchis.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }

                progressBar2.Value++;
                //Dinner
                //Dinner
                //protein
                List<string> podwieczibialko = new List<string>();
                comboBox3.Text = "Dinner";
                comboBox4.Text = "protein";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    podwieczibialko.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //foreach (string s in podwieczibialko) { MessageBox.Show(s); }
                //        carbohydrates
                List<string> podwiecziw = new List<string>();
                comboBox3.Text = "Dinner";
                comboBox4.Text = "carbohydrates";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    podwiecziw.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //Wartość energetyczna
                List<string> podwiecziener = new List<string>();
                comboBox3.Text = "Dinner";
                comboBox4.Text = "Wartość energetyczna";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    podwiecziener.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //Tłuszcz
                List<string> podwieczit = new List<string>();
                comboBox3.Text = "Dinner";
                comboBox4.Text = "Tłuszcz";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    podwieczit.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //roughage
                List<string> podwiecziblonnik = new List<string>();
                comboBox3.Text = "Dinner";
                comboBox4.Text = "roughage";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    podwiecziblonnik.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //sodium
                List<string> podwieczis = new List<string>();
                comboBox3.Text = "Dinner";
                comboBox4.Text = "sodium";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    podwieczis.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }


                progressBar2.Value++;
                //Supper
                //Supper
                //protein
                List<string> kolibialko = new List<string>();
                comboBox3.Text = "Supper";
                comboBox4.Text = "protein";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    kolibialko.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //foreach (string s in kolibialko) { MessageBox.Show(s); }
                //        carbohydrates
                List<string> koliw = new List<string>();
                comboBox3.Text = "Supper";
                comboBox4.Text = "carbohydrates";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    koliw.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //Wartość energetyczna
                List<string> koliener = new List<string>();
                comboBox3.Text = "Supper";
                comboBox4.Text = "Wartość energetyczna";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    koliener.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //Tłuszcz
                List<string> kolit = new List<string>();
                comboBox3.Text = "Supper";
                comboBox4.Text = "Tłuszcz";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    kolit.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //roughage
                List<string> koliblonnik = new List<string>();
                comboBox3.Text = "Supper";
                comboBox4.Text = "roughage";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    koliblonnik.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }
                //sodium
                List<string> kolis = new List<string>();
                comboBox3.Text = "Supper";
                comboBox4.Text = "sodium";
                analizailosciowa();
                foreach (ListViewItem i in listView4.Items)
                {
                    kolis.Add(i.SubItems[0].Text + "|" + i.SubItems[1].Text);
                }

                comboBox3.Text = "";
                comboBox4.Text = "";
                listView4.Items.Clear();
                //KONIEC ZBIERANIA DANYCH, CZAS NA ANALIZĘ!
                progressBar2.Value++;
                //SPLITER
                //char[] charSeparators = new char[] { '|' };
                //List<string> l = new List<string>(XXXXXXXXXXXXXXXXX.Split(charSeparators));

                using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\calydzien"))
                {
                    //Whole day
                    //1. Rozdziel dane
                    List<decimal> anal_fettle = new List<decimal>();
                    foreach (string s in calydzienibialko)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        anal_fettle.Add(Convert.ToDecimal(l[1]));
                    }
                    //2. Oznaczenie maksimum samopoczucia
                    //Ustalamy max samopoczucia i podmaxy.
                    decimal fettle_1 = anal_fettle.Max();
                    decimal fettle_2 = 0;
                    decimal fettle_3 = 0;

                    foreach (decimal d in anal_fettle)
                    {
                        bool tomax = false;
                        if (fettle_1 == d) { tomax = true; }
                        if (tomax == false)
                        {
                            bool byla2 = false;
                            //MessageBox.Show("d= " + d);
                            if (fettle_2 == d) { byla2 = true; }
                            if (byla2 == false) { if (d > fettle_2) { fettle_3 = fettle_2; fettle_2 = d; byla2 = true; } }
                            if (byla2 == false) { if (d > fettle_3) { fettle_3 = d; } }
                            //
                            //MessageBox.Show(fettle_1 + " / " + fettle_2 + " / " + fettle_3);





                        }
                    }
                    //3. protein
                    List<decimal> calydzienibalko_w = new List<decimal>();
                    foreach (string s in calydzienibialko)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { calydzienibalko_w.Add(Convert.ToDecimal(l[0])); calydzienibalko_w.Add(Convert.ToDecimal(l[0])); calydzienibalko_w.Add(Convert.ToDecimal(l[0])); calydzienibalko_w.Add(Convert.ToDecimal(l[0])); calydzienibalko_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { calydzienibalko_w.Add(Convert.ToDecimal(l[0])); calydzienibalko_w.Add(Convert.ToDecimal(l[0])); calydzienibalko_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { calydzienibalko_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //MessageBox.Show(calydzienibalko_w.Average().ToString());
                    //4. carbohydrates
                    List<decimal> calydzieniw_w = new List<decimal>();
                    foreach (string s in calydzieniw)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { calydzieniw_w.Add(Convert.ToDecimal(l[0])); calydzieniw_w.Add(Convert.ToDecimal(l[0])); calydzieniw_w.Add(Convert.ToDecimal(l[0])); calydzieniw_w.Add(Convert.ToDecimal(l[0])); calydzieniw_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { calydzieniw_w.Add(Convert.ToDecimal(l[0])); calydzieniw_w.Add(Convert.ToDecimal(l[0])); calydzieniw_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { calydzieniw_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //5.Tłuszcz
                    List<decimal> calydzienit_w = new List<decimal>();
                    foreach (string s in calydzienit)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { calydzienit_w.Add(Convert.ToDecimal(l[0])); calydzienit_w.Add(Convert.ToDecimal(l[0])); calydzienit_w.Add(Convert.ToDecimal(l[0])); calydzienit_w.Add(Convert.ToDecimal(l[0])); calydzienit_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { calydzienit_w.Add(Convert.ToDecimal(l[0])); calydzienit_w.Add(Convert.ToDecimal(l[0])); calydzienit_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { calydzienit_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //6. Wartość ener
                    List<decimal> calydzieniener_w = new List<decimal>();
                    foreach (string s in calydzieniener)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { calydzieniener_w.Add(Convert.ToDecimal(l[0])); calydzieniener_w.Add(Convert.ToDecimal(l[0])); calydzieniener_w.Add(Convert.ToDecimal(l[0])); calydzieniener_w.Add(Convert.ToDecimal(l[0])); calydzieniener_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { calydzieniener_w.Add(Convert.ToDecimal(l[0])); calydzieniener_w.Add(Convert.ToDecimal(l[0])); calydzieniener_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { calydzieniener_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //7. Blonnik
                    List<decimal> calydzieniblonnik_w = new List<decimal>();
                    foreach (string s in calydzieniblonnik)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { calydzieniblonnik_w.Add(Convert.ToDecimal(l[0])); calydzieniblonnik_w.Add(Convert.ToDecimal(l[0])); calydzieniblonnik_w.Add(Convert.ToDecimal(l[0])); calydzieniblonnik_w.Add(Convert.ToDecimal(l[0])); calydzieniblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { calydzieniblonnik_w.Add(Convert.ToDecimal(l[0])); calydzieniblonnik_w.Add(Convert.ToDecimal(l[0])); calydzieniblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { calydzieniblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //8. sodium
                    List<decimal> calydzienis_w = new List<decimal>();
                    foreach (string s in calydzienis)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { calydzienis_w.Add(Convert.ToDecimal(l[0])); calydzienis_w.Add(Convert.ToDecimal(l[0])); calydzienis_w.Add(Convert.ToDecimal(l[0])); calydzienis_w.Add(Convert.ToDecimal(l[0])); calydzienis_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { calydzienis_w.Add(Convert.ToDecimal(l[0])); calydzienis_w.Add(Convert.ToDecimal(l[0])); calydzienis_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { calydzienis_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //9. Zapisz
                    writer.Write(calydzienibalko_w.Average().ToString() + "|" + calydzieniw_w.Average().ToString() + "|" + calydzienit_w.Average().ToString() + "|" + calydzieniener_w.Average().ToString() + "|" + calydzieniblonnik_w.Average().ToString() + "|" + calydzienis_w.Average().ToString());
                }




                progressBar2.Value++;

                using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\sniad"))
                {
                    //Whole day
                    //1. Rozdziel dane
                    List<decimal> anal_fettle = new List<decimal>();
                    foreach (string s in sniadibialko)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        anal_fettle.Add(Convert.ToDecimal(l[1]));
                    }
                    //2. Oznaczenie maksimum samopoczucia
                    //Ustalamy max samopoczucia i podmaxy.
                    decimal fettle_1 = anal_fettle.Max();
                    decimal fettle_2 = 0;
                    decimal fettle_3 = 0;

                    foreach (decimal d in anal_fettle)
                    {
                        bool tomax = false;
                        if (fettle_1 == d) { tomax = true; }
                        if (tomax == false)
                        {
                            bool byla2 = false;
                            //MessageBox.Show("d= " + d);
                            if (fettle_2 == d) { byla2 = true; }
                            if (byla2 == false) { if (d > fettle_2) { fettle_3 = fettle_2; fettle_2 = d; byla2 = true; } }
                            if (byla2 == false) { if (d > fettle_3) { fettle_3 = d; } }
                            //
                            //MessageBox.Show(fettle_1 + " / " + fettle_2 + " / " + fettle_3);





                        }
                    }
                    //3. protein
                    List<decimal> sniadibalko_w = new List<decimal>();
                    foreach (string s in sniadibialko)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { sniadibalko_w.Add(Convert.ToDecimal(l[0])); sniadibalko_w.Add(Convert.ToDecimal(l[0])); sniadibalko_w.Add(Convert.ToDecimal(l[0])); sniadibalko_w.Add(Convert.ToDecimal(l[0])); sniadibalko_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { sniadibalko_w.Add(Convert.ToDecimal(l[0])); sniadibalko_w.Add(Convert.ToDecimal(l[0])); sniadibalko_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { sniadibalko_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //MessageBox.Show(sniadibalko_w.Average().ToString());
                    //4. carbohydrates
                    List<decimal> sniadiw_w = new List<decimal>();
                    foreach (string s in sniadiw)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { sniadiw_w.Add(Convert.ToDecimal(l[0])); sniadiw_w.Add(Convert.ToDecimal(l[0])); sniadiw_w.Add(Convert.ToDecimal(l[0])); sniadiw_w.Add(Convert.ToDecimal(l[0])); sniadiw_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { sniadiw_w.Add(Convert.ToDecimal(l[0])); sniadiw_w.Add(Convert.ToDecimal(l[0])); sniadiw_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { sniadiw_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //5.Tłuszcz
                    List<decimal> sniadit_w = new List<decimal>();
                    foreach (string s in sniadit)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { sniadit_w.Add(Convert.ToDecimal(l[0])); sniadit_w.Add(Convert.ToDecimal(l[0])); sniadit_w.Add(Convert.ToDecimal(l[0])); sniadit_w.Add(Convert.ToDecimal(l[0])); sniadit_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { sniadit_w.Add(Convert.ToDecimal(l[0])); sniadit_w.Add(Convert.ToDecimal(l[0])); sniadit_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { sniadit_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //6. Wartość ener
                    List<decimal> sniadiener_w = new List<decimal>();
                    foreach (string s in sniadiener)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { sniadiener_w.Add(Convert.ToDecimal(l[0])); sniadiener_w.Add(Convert.ToDecimal(l[0])); sniadiener_w.Add(Convert.ToDecimal(l[0])); sniadiener_w.Add(Convert.ToDecimal(l[0])); sniadiener_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { sniadiener_w.Add(Convert.ToDecimal(l[0])); sniadiener_w.Add(Convert.ToDecimal(l[0])); sniadiener_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { sniadiener_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //7. Blonnik
                    List<decimal> sniadiblonnik_w = new List<decimal>();
                    foreach (string s in sniadiblonnik)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { sniadiblonnik_w.Add(Convert.ToDecimal(l[0])); sniadiblonnik_w.Add(Convert.ToDecimal(l[0])); sniadiblonnik_w.Add(Convert.ToDecimal(l[0])); sniadiblonnik_w.Add(Convert.ToDecimal(l[0])); sniadiblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { sniadiblonnik_w.Add(Convert.ToDecimal(l[0])); sniadiblonnik_w.Add(Convert.ToDecimal(l[0])); sniadiblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { sniadiblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //8. sodium
                    List<decimal> sniadis_w = new List<decimal>();
                    foreach (string s in sniadis)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { sniadis_w.Add(Convert.ToDecimal(l[0])); sniadis_w.Add(Convert.ToDecimal(l[0])); sniadis_w.Add(Convert.ToDecimal(l[0])); sniadis_w.Add(Convert.ToDecimal(l[0])); sniadis_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { sniadis_w.Add(Convert.ToDecimal(l[0])); sniadis_w.Add(Convert.ToDecimal(l[0])); sniadis_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { sniadis_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //9. Zapisz
                    writer.Write(sniadibalko_w.Average().ToString() + "|" + sniadiw_w.Average().ToString() + "|" + sniadit_w.Average().ToString() + "|" + sniadiener_w.Average().ToString() + "|" + sniadiblonnik_w.Average().ToString() + "|" + sniadis_w.Average().ToString());
                }


                progressBar2.Value++;

                using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\iisniad"))
                {
                    //Whole day
                    //1. Rozdziel dane
                    List<decimal> anal_fettle = new List<decimal>();
                    foreach (string s in iisniadibialko)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        anal_fettle.Add(Convert.ToDecimal(l[1]));
                    }
                    //2. Oznaczenie maksimum samopoczucia
                    //Ustalamy max samopoczucia i podmaxy.
                    decimal fettle_1 = anal_fettle.Max();
                    decimal fettle_2 = 0;
                    decimal fettle_3 = 0;

                    foreach (decimal d in anal_fettle)
                    {
                        bool tomax = false;
                        if (fettle_1 == d) { tomax = true; }
                        if (tomax == false)
                        {
                            bool byla2 = false;
                            //MessageBox.Show("d= " + d);
                            if (fettle_2 == d) { byla2 = true; }
                            if (byla2 == false) { if (d > fettle_2) { fettle_3 = fettle_2; fettle_2 = d; byla2 = true; } }
                            if (byla2 == false) { if (d > fettle_3) { fettle_3 = d; } }
                            //
                            //MessageBox.Show(fettle_1 + " / " + fettle_2 + " / " + fettle_3);





                        }
                    }
                    //3. protein
                    List<decimal> iisniadibalko_w = new List<decimal>();
                    foreach (string s in iisniadibialko)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { iisniadibalko_w.Add(Convert.ToDecimal(l[0])); iisniadibalko_w.Add(Convert.ToDecimal(l[0])); iisniadibalko_w.Add(Convert.ToDecimal(l[0])); iisniadibalko_w.Add(Convert.ToDecimal(l[0])); iisniadibalko_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { iisniadibalko_w.Add(Convert.ToDecimal(l[0])); iisniadibalko_w.Add(Convert.ToDecimal(l[0])); iisniadibalko_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { iisniadibalko_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //MessageBox.Show(iisniadibalko_w.Average().ToString());
                    //4. carbohydrates
                    List<decimal> iisniadiw_w = new List<decimal>();
                    foreach (string s in iisniadiw)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { iisniadiw_w.Add(Convert.ToDecimal(l[0])); iisniadiw_w.Add(Convert.ToDecimal(l[0])); iisniadiw_w.Add(Convert.ToDecimal(l[0])); iisniadiw_w.Add(Convert.ToDecimal(l[0])); iisniadiw_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { iisniadiw_w.Add(Convert.ToDecimal(l[0])); iisniadiw_w.Add(Convert.ToDecimal(l[0])); iisniadiw_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { iisniadiw_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //5.Tłuszcz
                    List<decimal> iisniadit_w = new List<decimal>();
                    foreach (string s in iisniadit)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { iisniadit_w.Add(Convert.ToDecimal(l[0])); iisniadit_w.Add(Convert.ToDecimal(l[0])); iisniadit_w.Add(Convert.ToDecimal(l[0])); iisniadit_w.Add(Convert.ToDecimal(l[0])); iisniadit_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { iisniadit_w.Add(Convert.ToDecimal(l[0])); iisniadit_w.Add(Convert.ToDecimal(l[0])); iisniadit_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { iisniadit_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //6. Wartość ener
                    List<decimal> iisniadiener_w = new List<decimal>();
                    foreach (string s in iisniadiener)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { iisniadiener_w.Add(Convert.ToDecimal(l[0])); iisniadiener_w.Add(Convert.ToDecimal(l[0])); iisniadiener_w.Add(Convert.ToDecimal(l[0])); iisniadiener_w.Add(Convert.ToDecimal(l[0])); iisniadiener_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { iisniadiener_w.Add(Convert.ToDecimal(l[0])); iisniadiener_w.Add(Convert.ToDecimal(l[0])); iisniadiener_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { iisniadiener_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //7. Blonnik
                    List<decimal> iisniadiblonnik_w = new List<decimal>();
                    foreach (string s in iisniadiblonnik)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { iisniadiblonnik_w.Add(Convert.ToDecimal(l[0])); iisniadiblonnik_w.Add(Convert.ToDecimal(l[0])); iisniadiblonnik_w.Add(Convert.ToDecimal(l[0])); iisniadiblonnik_w.Add(Convert.ToDecimal(l[0])); iisniadiblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { iisniadiblonnik_w.Add(Convert.ToDecimal(l[0])); iisniadiblonnik_w.Add(Convert.ToDecimal(l[0])); iisniadiblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { iisniadiblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //8. sodium
                    List<decimal> iisniadis_w = new List<decimal>();
                    foreach (string s in iisniadis)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { iisniadis_w.Add(Convert.ToDecimal(l[0])); iisniadis_w.Add(Convert.ToDecimal(l[0])); iisniadis_w.Add(Convert.ToDecimal(l[0])); iisniadis_w.Add(Convert.ToDecimal(l[0])); iisniadis_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { iisniadis_w.Add(Convert.ToDecimal(l[0])); iisniadis_w.Add(Convert.ToDecimal(l[0])); iisniadis_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { iisniadis_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //9. Zapisz
                    writer.Write(iisniadibalko_w.Average().ToString() + "|" + iisniadiw_w.Average().ToString() + "|" + iisniadit_w.Average().ToString() + "|" + iisniadiener_w.Average().ToString() + "|" + iisniadiblonnik_w.Average().ToString() + "|" + iisniadis_w.Average().ToString());
                }


                progressBar2.Value++;
                using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\podwiecz"))
                {
                    //Whole day
                    //1. Rozdziel dane
                    List<decimal> anal_fettle = new List<decimal>();
                    foreach (string s in podwieczibialko)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        anal_fettle.Add(Convert.ToDecimal(l[1]));
                    }
                    //2. Oznaczenie maksimum samopoczucia
                    //Ustalamy max samopoczucia i podmaxy.
                    decimal fettle_1 = anal_fettle.Max();
                    decimal fettle_2 = 0;
                    decimal fettle_3 = 0;

                    foreach (decimal d in anal_fettle)
                    {
                        bool tomax = false;
                        if (fettle_1 == d) { tomax = true; }
                        if (tomax == false)
                        {
                            bool byla2 = false;
                            //MessageBox.Show("d= " + d);
                            if (fettle_2 == d) { byla2 = true; }
                            if (byla2 == false) { if (d > fettle_2) { fettle_3 = fettle_2; fettle_2 = d; byla2 = true; } }
                            if (byla2 == false) { if (d > fettle_3) { fettle_3 = d; } }
                            //
                            //MessageBox.Show(fettle_1 + " / " + fettle_2 + " / " + fettle_3);





                        }
                    }
                    //3. protein
                    List<decimal> podwieczibalko_w = new List<decimal>();
                    foreach (string s in podwieczibialko)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { podwieczibalko_w.Add(Convert.ToDecimal(l[0])); podwieczibalko_w.Add(Convert.ToDecimal(l[0])); podwieczibalko_w.Add(Convert.ToDecimal(l[0])); podwieczibalko_w.Add(Convert.ToDecimal(l[0])); podwieczibalko_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { podwieczibalko_w.Add(Convert.ToDecimal(l[0])); podwieczibalko_w.Add(Convert.ToDecimal(l[0])); podwieczibalko_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { podwieczibalko_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //MessageBox.Show(podwieczibalko_w.Average().ToString());
                    //4. carbohydrates
                    List<decimal> podwiecziw_w = new List<decimal>();
                    foreach (string s in podwiecziw)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { podwiecziw_w.Add(Convert.ToDecimal(l[0])); podwiecziw_w.Add(Convert.ToDecimal(l[0])); podwiecziw_w.Add(Convert.ToDecimal(l[0])); podwiecziw_w.Add(Convert.ToDecimal(l[0])); podwiecziw_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { podwiecziw_w.Add(Convert.ToDecimal(l[0])); podwiecziw_w.Add(Convert.ToDecimal(l[0])); podwiecziw_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { podwiecziw_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //5.Tłuszcz
                    List<decimal> podwieczit_w = new List<decimal>();
                    foreach (string s in podwieczit)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { podwieczit_w.Add(Convert.ToDecimal(l[0])); podwieczit_w.Add(Convert.ToDecimal(l[0])); podwieczit_w.Add(Convert.ToDecimal(l[0])); podwieczit_w.Add(Convert.ToDecimal(l[0])); podwieczit_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { podwieczit_w.Add(Convert.ToDecimal(l[0])); podwieczit_w.Add(Convert.ToDecimal(l[0])); podwieczit_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { podwieczit_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //6. Wartość ener
                    List<decimal> podwiecziener_w = new List<decimal>();
                    foreach (string s in podwiecziener)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { podwiecziener_w.Add(Convert.ToDecimal(l[0])); podwiecziener_w.Add(Convert.ToDecimal(l[0])); podwiecziener_w.Add(Convert.ToDecimal(l[0])); podwiecziener_w.Add(Convert.ToDecimal(l[0])); podwiecziener_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { podwiecziener_w.Add(Convert.ToDecimal(l[0])); podwiecziener_w.Add(Convert.ToDecimal(l[0])); podwiecziener_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { podwiecziener_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //7. Blonnik
                    List<decimal> podwiecziblonnik_w = new List<decimal>();
                    foreach (string s in podwiecziblonnik)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { podwiecziblonnik_w.Add(Convert.ToDecimal(l[0])); podwiecziblonnik_w.Add(Convert.ToDecimal(l[0])); podwiecziblonnik_w.Add(Convert.ToDecimal(l[0])); podwiecziblonnik_w.Add(Convert.ToDecimal(l[0])); podwiecziblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { podwiecziblonnik_w.Add(Convert.ToDecimal(l[0])); podwiecziblonnik_w.Add(Convert.ToDecimal(l[0])); podwiecziblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { podwiecziblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //8. sodium
                    List<decimal> podwieczis_w = new List<decimal>();
                    foreach (string s in podwieczis)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { podwieczis_w.Add(Convert.ToDecimal(l[0])); podwieczis_w.Add(Convert.ToDecimal(l[0])); podwieczis_w.Add(Convert.ToDecimal(l[0])); podwieczis_w.Add(Convert.ToDecimal(l[0])); podwieczis_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { podwieczis_w.Add(Convert.ToDecimal(l[0])); podwieczis_w.Add(Convert.ToDecimal(l[0])); podwieczis_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { podwieczis_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //9. Zapisz
                    writer.Write(podwieczibalko_w.Average().ToString() + "|" + podwiecziw_w.Average().ToString() + "|" + podwieczit_w.Average().ToString() + "|" + podwiecziener_w.Average().ToString() + "|" + podwiecziblonnik_w.Average().ToString() + "|" + podwieczis_w.Average().ToString());
                }
                progressBar2.Value++;
                using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\kol"))
                {
                    //Whole day
                    //1. Rozdziel dane
                    List<decimal> anal_fettle = new List<decimal>();
                    foreach (string s in kolibialko)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        anal_fettle.Add(Convert.ToDecimal(l[1]));
                    }
                    //2. Oznaczenie maksimum samopoczucia
                    //Ustalamy max samopoczucia i podmaxy.
                    decimal fettle_1 = anal_fettle.Max();
                    decimal fettle_2 = 0;
                    decimal fettle_3 = 0;

                    foreach (decimal d in anal_fettle)
                    {
                        bool tomax = false;
                        if (fettle_1 == d) { tomax = true; }
                        if (tomax == false)
                        {
                            bool byla2 = false;
                            //MessageBox.Show("d= " + d);
                            if (fettle_2 == d) { byla2 = true; }
                            if (byla2 == false) { if (d > fettle_2) { fettle_3 = fettle_2; fettle_2 = d; byla2 = true; } }
                            if (byla2 == false) { if (d > fettle_3) { fettle_3 = d; } }
                            //
                            //MessageBox.Show(fettle_1 + " / " + fettle_2 + " / " + fettle_3);





                        }
                    }
                    //3. protein
                    List<decimal> kolibalko_w = new List<decimal>();
                    foreach (string s in kolibialko)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { kolibalko_w.Add(Convert.ToDecimal(l[0])); kolibalko_w.Add(Convert.ToDecimal(l[0])); kolibalko_w.Add(Convert.ToDecimal(l[0])); kolibalko_w.Add(Convert.ToDecimal(l[0])); kolibalko_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { kolibalko_w.Add(Convert.ToDecimal(l[0])); kolibalko_w.Add(Convert.ToDecimal(l[0])); kolibalko_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { kolibalko_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //MessageBox.Show(kolibalko_w.Average().ToString());
                    //4. carbohydrates
                    List<decimal> koliw_w = new List<decimal>();
                    foreach (string s in koliw)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { koliw_w.Add(Convert.ToDecimal(l[0])); koliw_w.Add(Convert.ToDecimal(l[0])); koliw_w.Add(Convert.ToDecimal(l[0])); koliw_w.Add(Convert.ToDecimal(l[0])); koliw_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { koliw_w.Add(Convert.ToDecimal(l[0])); koliw_w.Add(Convert.ToDecimal(l[0])); koliw_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { koliw_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //5.Tłuszcz
                    List<decimal> kolit_w = new List<decimal>();
                    foreach (string s in kolit)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { kolit_w.Add(Convert.ToDecimal(l[0])); kolit_w.Add(Convert.ToDecimal(l[0])); kolit_w.Add(Convert.ToDecimal(l[0])); kolit_w.Add(Convert.ToDecimal(l[0])); kolit_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { kolit_w.Add(Convert.ToDecimal(l[0])); kolit_w.Add(Convert.ToDecimal(l[0])); kolit_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { kolit_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //6. Wartość ener
                    List<decimal> koliener_w = new List<decimal>();
                    foreach (string s in koliener)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { koliener_w.Add(Convert.ToDecimal(l[0])); koliener_w.Add(Convert.ToDecimal(l[0])); koliener_w.Add(Convert.ToDecimal(l[0])); koliener_w.Add(Convert.ToDecimal(l[0])); koliener_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { koliener_w.Add(Convert.ToDecimal(l[0])); koliener_w.Add(Convert.ToDecimal(l[0])); koliener_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { koliener_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //7. Blonnik
                    List<decimal> koliblonnik_w = new List<decimal>();
                    foreach (string s in koliblonnik)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { koliblonnik_w.Add(Convert.ToDecimal(l[0])); koliblonnik_w.Add(Convert.ToDecimal(l[0])); koliblonnik_w.Add(Convert.ToDecimal(l[0])); koliblonnik_w.Add(Convert.ToDecimal(l[0])); koliblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { koliblonnik_w.Add(Convert.ToDecimal(l[0])); koliblonnik_w.Add(Convert.ToDecimal(l[0])); koliblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { koliblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //8. sodium
                    List<decimal> kolis_w = new List<decimal>();
                    foreach (string s in kolis)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { kolis_w.Add(Convert.ToDecimal(l[0])); kolis_w.Add(Convert.ToDecimal(l[0])); kolis_w.Add(Convert.ToDecimal(l[0])); kolis_w.Add(Convert.ToDecimal(l[0])); kolis_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { kolis_w.Add(Convert.ToDecimal(l[0])); kolis_w.Add(Convert.ToDecimal(l[0])); kolis_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { kolis_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //9. Zapisz
                    writer.Write(kolibalko_w.Average().ToString() + "|" + koliw_w.Average().ToString() + "|" + kolit_w.Average().ToString() + "|" + koliener_w.Average().ToString() + "|" + koliblonnik_w.Average().ToString() + "|" + kolis_w.Average().ToString());
                }
                using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\Lunch"))
                {
                    //Whole day
                    //1. Rozdziel dane
                    List<decimal> anal_fettle = new List<decimal>();
                    foreach (string s in Lunchibialko)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        anal_fettle.Add(Convert.ToDecimal(l[1]));
                    }
                    //2. Oznaczenie maksimum samopoczucia
                    //Ustalamy max samopoczucia i podmaxy.
                    decimal fettle_1 = anal_fettle.Max();
                    decimal fettle_2 = 0;
                    decimal fettle_3 = 0;

                    foreach (decimal d in anal_fettle)
                    {
                        bool tomax = false;
                        if (fettle_1 == d) { tomax = true; }
                        if (tomax == false)
                        {
                            bool byla2 = false;
                            //MessageBox.Show("d= " + d);
                            if (fettle_2 == d) { byla2 = true; }
                            if (byla2 == false) { if (d > fettle_2) { fettle_3 = fettle_2; fettle_2 = d; byla2 = true; } }
                            if (byla2 == false) { if (d > fettle_3) { fettle_3 = d; } }
                            //
                            //MessageBox.Show(fettle_1 + " / " + fettle_2 + " / " + fettle_3);





                        }
                    }
                    //3. protein
                    List<decimal> Lunchibalko_w = new List<decimal>();
                    foreach (string s in Lunchibialko)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { Lunchibalko_w.Add(Convert.ToDecimal(l[0])); Lunchibalko_w.Add(Convert.ToDecimal(l[0])); Lunchibalko_w.Add(Convert.ToDecimal(l[0])); Lunchibalko_w.Add(Convert.ToDecimal(l[0])); Lunchibalko_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { Lunchibalko_w.Add(Convert.ToDecimal(l[0])); Lunchibalko_w.Add(Convert.ToDecimal(l[0])); Lunchibalko_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { Lunchibalko_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //MessageBox.Show(Lunchibalko_w.Average().ToString());
                    //4. carbohydrates
                    List<decimal> Lunchiw_w = new List<decimal>();
                    foreach (string s in Lunchiw)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { Lunchiw_w.Add(Convert.ToDecimal(l[0])); Lunchiw_w.Add(Convert.ToDecimal(l[0])); Lunchiw_w.Add(Convert.ToDecimal(l[0])); Lunchiw_w.Add(Convert.ToDecimal(l[0])); Lunchiw_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { Lunchiw_w.Add(Convert.ToDecimal(l[0])); Lunchiw_w.Add(Convert.ToDecimal(l[0])); Lunchiw_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { Lunchiw_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //5.Tłuszcz
                    List<decimal> Lunchit_w = new List<decimal>();
                    foreach (string s in Lunchit)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { Lunchit_w.Add(Convert.ToDecimal(l[0])); Lunchit_w.Add(Convert.ToDecimal(l[0])); Lunchit_w.Add(Convert.ToDecimal(l[0])); Lunchit_w.Add(Convert.ToDecimal(l[0])); Lunchit_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { Lunchit_w.Add(Convert.ToDecimal(l[0])); Lunchit_w.Add(Convert.ToDecimal(l[0])); Lunchit_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { Lunchit_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //6. Wartość ener
                    List<decimal> Lunchiener_w = new List<decimal>();
                    foreach (string s in Lunchiener)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { Lunchiener_w.Add(Convert.ToDecimal(l[0])); Lunchiener_w.Add(Convert.ToDecimal(l[0])); Lunchiener_w.Add(Convert.ToDecimal(l[0])); Lunchiener_w.Add(Convert.ToDecimal(l[0])); Lunchiener_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { Lunchiener_w.Add(Convert.ToDecimal(l[0])); Lunchiener_w.Add(Convert.ToDecimal(l[0])); Lunchiener_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { Lunchiener_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //7. Blonnik
                    List<decimal> Lunchiblonnik_w = new List<decimal>();
                    foreach (string s in Lunchiblonnik)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { Lunchiblonnik_w.Add(Convert.ToDecimal(l[0])); Lunchiblonnik_w.Add(Convert.ToDecimal(l[0])); Lunchiblonnik_w.Add(Convert.ToDecimal(l[0])); Lunchiblonnik_w.Add(Convert.ToDecimal(l[0])); Lunchiblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { Lunchiblonnik_w.Add(Convert.ToDecimal(l[0])); Lunchiblonnik_w.Add(Convert.ToDecimal(l[0])); Lunchiblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { Lunchiblonnik_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //8. sodium
                    List<decimal> Lunchis_w = new List<decimal>();
                    foreach (string s in Lunchis)
                    {
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(s.Split(charSeparators));
                        if (fettle_1 != 0) { if (Convert.ToDecimal(l[1]) == fettle_1) { Lunchis_w.Add(Convert.ToDecimal(l[0])); Lunchis_w.Add(Convert.ToDecimal(l[0])); Lunchis_w.Add(Convert.ToDecimal(l[0])); Lunchis_w.Add(Convert.ToDecimal(l[0])); Lunchis_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_2 != 0) { if (Convert.ToDecimal(l[1]) == fettle_2) { Lunchis_w.Add(Convert.ToDecimal(l[0])); Lunchis_w.Add(Convert.ToDecimal(l[0])); Lunchis_w.Add(Convert.ToDecimal(l[0])); } }
                        if (fettle_3 != 0) { if (Convert.ToDecimal(l[1]) == fettle_3) { Lunchis_w.Add(Convert.ToDecimal(l[0])); } }
                    }
                    //9. Zapisz
                    writer.Write(Lunchibalko_w.Average().ToString() + "|" + Lunchiw_w.Average().ToString() + "|" + Lunchit_w.Average().ToString() + "|" + Lunchiener_w.Average().ToString() + "|" + Lunchiblonnik_w.Average().ToString() + "|" + Lunchis_w.Average().ToString());
                }
                progressBar2.Value++;
                //DATA
                using (StreamWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\data"))
                { writer.Write(DateTime.Now); }

                //POBIERZ DATE
                using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\data"))
                { string dat = writer.ReadToEnd(); label57.Text = "Twój obecny profil został wygenerowany: " + dat + "."; label88.Text = "Twój obecny profil został wygenerowany: " + dat + "."; }
                jestanaliza = true; progressBar2.Visible = false;
            }
        }

        bool jestanaliza = false;

        private void progressBar2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (jestanaliza == true)
            {
                if (comboBox9.Text == "Whole day")
                {
                    using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\calydzien"))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        textBox28.Text = l[0]; textBox39.Text = l[0];
                        textBox27.Text = l[1]; textBox38.Text = l[1];
                        textBox26.Text = l[2]; textBox37.Text = l[2];
                        textBox29.Text = l[3]; textBox40.Text = l[3];
                        textBox25.Text = l[4]; textBox36.Text = l[4];
                        textBox24.Text = l[5]; textBox35.Text = l[5];
                    }
                }
                if (comboBox9.Text == "Breakfest")
                {
                    using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\sniad"))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        textBox28.Text = l[0]; textBox39.Text = l[0];
                        textBox27.Text = l[1]; textBox38.Text = l[1];
                        textBox26.Text = l[2]; textBox37.Text = l[2];
                        textBox29.Text = l[3]; textBox40.Text = l[3];
                        textBox25.Text = l[4]; textBox36.Text = l[4];
                        textBox24.Text = l[5]; textBox35.Text = l[5];
                    }
                }
                if (comboBox9.Text == "II Breakfest")
                {
                    using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\iisniad"))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        textBox28.Text = l[0]; textBox39.Text = l[0];
                        textBox27.Text = l[1]; textBox38.Text = l[1];
                        textBox26.Text = l[2]; textBox37.Text = l[2];
                        textBox29.Text = l[3]; textBox40.Text = l[3];
                        textBox25.Text = l[4]; textBox36.Text = l[4];
                        textBox24.Text = l[5]; textBox35.Text = l[5];
                    }
                }
                if (comboBox9.Text == "Lunch")
                {
                    using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\Lunch"))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        textBox28.Text = l[0]; textBox39.Text = l[0];
                        textBox27.Text = l[1]; textBox38.Text = l[1];
                        textBox26.Text = l[2]; textBox37.Text = l[2];
                        textBox29.Text = l[3]; textBox40.Text = l[3];
                        textBox25.Text = l[4]; textBox36.Text = l[4];
                        textBox24.Text = l[5]; textBox35.Text = l[5];
                    }
                }
                if (comboBox9.Text == "Dinner")
                {
                    using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\podwiecz"))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        textBox28.Text = l[0]; textBox39.Text = l[0];
                        textBox27.Text = l[1]; textBox38.Text = l[1];
                        textBox26.Text = l[2]; textBox37.Text = l[2];
                        textBox29.Text = l[3]; textBox40.Text = l[3];
                        textBox25.Text = l[4]; textBox36.Text = l[4];
                        textBox24.Text = l[5]; textBox35.Text = l[5];
                    }
                }
                if (comboBox9.Text == "Supper")
                {
                    using (StreamReader writer = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\analiza\\kol"))
                    {
                        string odczyt = writer.ReadToEnd();
                        char[] charSeparators = new char[] { '|' };
                        List<string> l = new List<string>(odczyt.Split(charSeparators));
                        textBox28.Text = l[0]; textBox39.Text = l[0];
                        textBox27.Text = l[1]; textBox38.Text = l[1];
                        textBox26.Text = l[2]; textBox37.Text = l[2];
                        textBox29.Text = l[3]; textBox40.Text = l[3];
                        textBox25.Text = l[4]; textBox36.Text = l[4];
                        textBox24.Text = l[5]; textBox35.Text = l[5];
                    }
                }
            }

        }

        private void button16_Click(object sender, EventArgs e)
        {
            
            
            /*/*System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse("62.87.177.20");
            var listener = new TcpListener(ipaddress, 29995);
            listener.Start();
            using (var client = listener.AcceptTcpClient())
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("GET / HTTP/1.1");
                writer.WriteLine("Host: 62.87.177.20:29995");
                writer.WriteLine("Upgrade: WebSocket");
                writer.WriteLine("Connection: Upgrade");
                writer.WriteLine("Sec-WebSocket-Key: a4on2b+RhIScj/u5h6yYMQ==");
                writer.WriteLine("Sec-WebSocket-Version: 13");
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    MessageBox.Show(line);
                }
            }
            listener.Stop();*/
            
           DateTime dt = DateTime.Now;
            string timestamp = dt.ToString("yyyyMMddHHmmss");
            int typ = 1;
            string tresc = "<credentials><login>kbtest</login><password>594f803b380a41396ed63dca39503542</password></credentials>";
            //MessageBox.Show(serwer("<pancreappRequest type=\"" + typ + "\" timestamp=\"" + timestamp + "\" client=\"pc\">" + tresc + "</pancreappRequest>"));
            
            /*ws.Connect();
            
            //MessageBox.Show(timestamp);
            //ws.Send("ECHO");
            MessageBox.Show("<pancreappRequest type=\"" + typ + "\" timestamp=\"" + timestamp + "\" client=\"pc\">" + tresc + "</pancreappRequest>");
            Clipboard.SetText("<pancreappRequest type=\"" + typ + "\" timestamp=\"" + timestamp + "\" client=\"pc\">" + tresc + "</pancreappRequest>");
            ws.Send("<pancreappRequest type=\"" + typ + "\" timestamp=\"" + timestamp + "\" client=\"pc\">" + tresc + "</pancreappRequest>");
            //MessageBox.Show(ws.Recv());*/
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) { radioButton2.Checked = false; }
            if (radioButton2.Checked) { radioButton1.Checked = false; }
            dostosujmodelpredykcji();
        }

        private void dostosujmodelpredykcji() {
            if (radioButton1.Checked) {
                groupBox6.Visible = true;
                groupBox5.Visible = false;
                listView4.Size = new Size(213, 430);
                chart3.Size = new Size(449, 497);
                groupBox8.Visible = false;
            }
            if (radioButton2.Checked)
            {
                groupBox6.Visible = false ;
                groupBox5.Visible = true ;
                listView4.Size = new Size(213, 266);
                chart3.Size = new Size(449, 331);
                groupBox8.Visible = true ;
            }
            comboBox6.Text = ""; textBox49.Text = "";
            listView5.Items.Clear();
            textBox48.Text = "Co chcesz zjeść? Wprowadź produkty i ich ilości aby wykonać analizę.";
            progressBar3.Value = 0;
            button17.Enabled = false;
            comboBox7.Text = "";
            numericUpDown11.Value = 1;

            textBox39.Text = ""; textBox38.Text = ""; textBox37.Text = "";
            textBox40.Text = ""; textBox36.Text = ""; textBox35.Text = "";
            textBox23.Text = ""; textBox34.Text = ""; textBox33.Text = "";
            textBox32.Text = ""; textBox31.Text = ""; textBox30.Text = "";
            textBox41.Text = ""; textBox42.Text = ""; textBox43.Text = "";
            textBox44.Text = ""; textBox45.Text = ""; textBox46.Text = "";
            label79.Text = "00 %";
            progressBar1.Value = 0;
            button10.Enabled = false; comboBox6.Text = "";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) { radioButton2.Checked = false; }
            if (radioButton2.Checked) { radioButton1.Checked = false; }
            dostosujmodelpredykcji();
        }
        
        private void button17_Click(object sender, EventArgs e)
        {
            textBox48.Text = "Przeprowadzam analizę...";
            progressBar3.Value = 0;
            List<string> wynik = new List<string>(); 
            string path_dieta = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\dieta";
            string path_produkty = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\produkty";
            string path_fettle = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\fettle";

            List<string> listadat = new List<string>();
            foreach (string data in Directory.GetFiles(path_fettle)) { listadat.Add(data); }


            
            

            if (comboBox6.Text == "Whole day") {
                progressBar3.Value = 0; progressBar3.Maximum = 100; int krok = 0;
            foreach (string data in listadat)
{
    
               
decimal fettle_wynik = -1;
        string fettle_odczyt = File.ReadAllText(data);
        string[] s = fettle_odczyt.Split('|');
        List<decimal> samo = new List<decimal>();
        if (s[0] != "-") { samo.Add(Convert.ToDecimal(s[0])); }
        if (s[1] != "-") { samo.Add(Convert.ToDecimal(s[1])); }
        if (s[2] != "-") { samo.Add(Convert.ToDecimal(s[2])); }
        if (s[3] != "-") { samo.Add(Convert.ToDecimal(s[3])); }
        if (s[4] != "-") { samo.Add(Convert.ToDecimal(s[4])); }

        if (samo.Count > 0) { fettle_wynik = samo.Average(); } else { fettle_wynik = -1; }



    
                
                string dataa = Path.GetFileName(data);
                List<decimal> bi = new List<decimal>(); List<decimal> w = new List<decimal>(); List<decimal> t = new List<decimal>(); 
            List<decimal> bł = new List<decimal>(); List<decimal> sodium = new List<decimal>(); 
            List<decimal> waga = new List<decimal>(); List<decimal> ener = new List<decimal>();
                
                foreach (string dieta in Directory.GetFiles(path_dieta))
    {
        string dieta_odczyt = File.ReadAllText(dieta); string[] d = dieta_odczyt.Split('|');
        DateTime data_odczyt = Convert.ToDateTime(d[0]);
        string data_odczyt_sformatowana = data_odczyt.Year.ToString() + data_odczyt.Month.ToString()+ data_odczyt.Day.ToString();
        if (dataa == data_odczyt_sformatowana)
        {
            
            foreach (string produkt in Directory.GetFiles(path_produkty)) {
                string[] p = File.ReadAllText(produkt).Split('|');
                if (p[0] == d[2]) {
                    bi.Add(Convert.ToDecimal(p[2]) * Convert.ToDecimal(d[3]));
                    w.Add(Convert.ToDecimal(p[3]) * Convert.ToDecimal(d[3]));
                    t.Add(Convert.ToDecimal(p[4]) * Convert.ToDecimal(d[3]));
                    bł.Add(Convert.ToDecimal(p[5]) * Convert.ToDecimal(d[3]));
                    sodium.Add(Convert.ToDecimal(p[6]) * Convert.ToDecimal(d[3]));
                    waga.Add(Convert.ToDecimal(p[7]) * Convert.ToDecimal(d[3]));
                    ener.Add(Convert.ToDecimal(p[2]) * Convert.ToDecimal(d[3]));
                }
            
            
            
            
            }
        
        
        }
    
    }
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0)); if (bi.Count > 0 && fettle_wynik != -1)
                {
                    wynik.Add(dataa + "\t" + fettle_wynik + "\t" + bi.Sum().ToString() + "\t" + w.Sum().ToString() + "\t" + t.Sum().ToString() + "\t" + bł.Sum().ToString() + "\t" + sodium.Sum().ToString() + "\t" + waga.Sum().ToString() + "\t" + ener.Sum().ToString());
                }
}
            File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik);
            //Metoda Sheparda
                string[] odczyt_linie = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                List<string> wynik2 = new List<string>(); List<double> dystanse = new List<double>();
                foreach (string linia in odczyt_linie) {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    //liczymy dystans
                    double prognoza_b = Convert.ToDouble(textBox33.Text);
                    double prognoza_w = Convert.ToDouble(textBox32.Text);
                    double prognoza_t = Convert.ToDouble(textBox31.Text);
                    double prognoza_bł = Convert.ToDouble(textBox30.Text);
                    double prognoza_sodium = Convert.ToDouble(textBox23.Text);
                    double prognoza_waga = Convert.ToDouble(textBox49.Text);
                    double prognoza_ener = Convert.ToDouble(textBox34.Text);

                    double dystans = Math.Sqrt(Math.Pow(lll[2] - prognoza_b, 2) + Math.Pow(lll[3] - prognoza_w, 2) + Math.Pow(lll[4] - prognoza_t, 2) + Math.Pow(lll[5] - prognoza_bł, 2) + Math.Pow(lll[6] - prognoza_sodium, 2) + Math.Pow(lll[7] - prognoza_waga, 2) + Math.Pow(lll[8] - prognoza_ener, 2));
                    dystanse.Add(dystans);
                    wynik2.Add(linia + "\t" + dystans.ToString());
                    
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik2);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double maxdystans = dystanse.Max();
                List<string> wynik3 = new List<string>(); List<double> wiliczniki = new List<double>();
                string[] odczyt_linie2 = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                foreach (string linia in odczyt_linie2)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    double wilicznik = (maxdystans - lll[9]) / (Math.Pow(maxdystans * lll[9], 2));
                    wiliczniki.Add(wilicznik);
                    wynik3.Add(linia + "\t" + wilicznik);
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik3);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double sumawilicznik = wiliczniki.Sum();
                List<string> wynik4 = new List<string>(); List<double> Fy = new List<double>();
                string[] odczyt_linie3 = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                foreach (string linia in odczyt_linie3)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    double wi = lll[10] / sumawilicznik;
                    double F = wi * lll[1]; Fy.Add(F);
                    wynik4.Add( linia + "\t" + wi + "\t" + F);
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik4);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double predykcja = Math.Round(Fy.Sum(), 3);
                textBox48.Text = "Prediction of fettle: " + predykcja.ToString();
                progressBar3.Maximum = 10000;
                progressBar3.Value = Convert.ToInt32(predykcja * 1000);
            
            
            
            
            }
            if (comboBox6.Text == "Breakfest")
            {
                progressBar3.Value = 0; progressBar3.Maximum = 100; int krok = 0;
                
                progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                foreach (string data in listadat)
                {


                    decimal fettle_wynik = -1;
                    string fettle_odczyt = File.ReadAllText(data);
                    string[] s = fettle_odczyt.Split('|');
                    List<decimal> samo = new List<decimal>();
                    if (s[0] != "-") { samo.Add(Convert.ToDecimal(s[0])); }
                    if (s[1] != "-") { samo.Add(Convert.ToDecimal(s[1])); }
                    if (s[2] != "-") { samo.Add(Convert.ToDecimal(s[2])); }
                    if (s[3] != "-") { samo.Add(Convert.ToDecimal(s[3])); }
                    if (s[4] != "-") { samo.Add(Convert.ToDecimal(s[4])); }

                    if (samo.Count > 0) { fettle_wynik = samo.Average(); } else { fettle_wynik = -1; }





                    string dataa = Path.GetFileName(data);
                    List<decimal> bi = new List<decimal>(); List<decimal> w = new List<decimal>(); List<decimal> t = new List<decimal>();
                    List<decimal> bł = new List<decimal>(); List<decimal> sodium = new List<decimal>();
                    List<decimal> waga = new List<decimal>(); List<decimal> ener = new List<decimal>();

                    foreach (string dieta in Directory.GetFiles(path_dieta))
                    {
                        string dieta_odczyt = File.ReadAllText(dieta); string[] d = dieta_odczyt.Split('|');
                        DateTime data_odczyt = Convert.ToDateTime(d[0]);
                        string data_odczyt_sformatowana = data_odczyt.Year.ToString() + data_odczyt.Month.ToString() + data_odczyt.Day.ToString();
                        if (d[1] == "Breakfest")
                        {
                            if (dataa == data_odczyt_sformatowana)
                            {

                                foreach (string produkt in Directory.GetFiles(path_produkty))
                                {
                                    string[] p = File.ReadAllText(produkt).Split('|');
                                    if (p[0] == d[2])
                                    {
                                        bi.Add(Convert.ToDecimal(p[2]) * Convert.ToDecimal(d[3]));
                                        w.Add(Convert.ToDecimal(p[3]) * Convert.ToDecimal(d[3]));
                                        t.Add(Convert.ToDecimal(p[4]) * Convert.ToDecimal(d[3]));
                                        bł.Add(Convert.ToDecimal(p[5]) * Convert.ToDecimal(d[3]));
                                        sodium.Add(Convert.ToDecimal(p[6]) * Convert.ToDecimal(d[3]));
                                        waga.Add(Convert.ToDecimal(p[7]) * Convert.ToDecimal(d[3]));
                                        ener.Add(Convert.ToDecimal(p[2]) * Convert.ToDecimal(d[3]));
                                    }




                                }


                            }
                        }

                    }
                    krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0)); if (bi.Count > 0 && fettle_wynik != -1)
                    {
                        wynik.Add(dataa + "\t" + fettle_wynik + "\t" + bi.Sum().ToString() + "\t" + w.Sum().ToString() + "\t" + t.Sum().ToString() + "\t" + bł.Sum().ToString() + "\t" + sodium.Sum().ToString() + "\t" + waga.Sum().ToString() + "\t" + ener.Sum().ToString());
                        
                    }
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik);

                //Metoda Sheparda
                string[] odczyt_linie = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                List<string> wynik2 = new List<string>(); List<double> dystanse = new List<double>();
                foreach (string linia in odczyt_linie) {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    //liczymy dystans
                    double prognoza_b = Convert.ToDouble(textBox33.Text);
                    double prognoza_w = Convert.ToDouble(textBox32.Text);
                    double prognoza_t = Convert.ToDouble(textBox31.Text);
                    double prognoza_bł = Convert.ToDouble(textBox30.Text);
                    double prognoza_sodium = Convert.ToDouble(textBox23.Text);
                    double prognoza_waga = Convert.ToDouble(textBox49.Text);
                    double prognoza_ener = Convert.ToDouble(textBox34.Text);

                    double dystans = Math.Sqrt(Math.Pow(lll[2] - prognoza_b, 2) + Math.Pow(lll[3] - prognoza_w, 2) + Math.Pow(lll[4] - prognoza_t, 2) + Math.Pow(lll[5] - prognoza_bł, 2) + Math.Pow(lll[6] - prognoza_sodium, 2) + Math.Pow(lll[7] - prognoza_waga, 2) + Math.Pow(lll[8] - prognoza_ener, 2));
                    dystanse.Add(dystans);
                    wynik2.Add(linia + "\t" + dystans.ToString());
                    
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik2);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double maxdystans = dystanse.Max();
                List<string> wynik3 = new List<string>(); List<double> wiliczniki = new List<double>();
                string[] odczyt_linie2 = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                foreach (string linia in odczyt_linie2)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    double wilicznik = (maxdystans - lll[9]) / (Math.Pow(maxdystans * lll[9], 2));
                    wiliczniki.Add(wilicznik);
                    wynik3.Add(linia + "\t" + wilicznik);
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik3);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double sumawilicznik = wiliczniki.Sum();
                List<string> wynik4 = new List<string>(); List<double> Fy = new List<double>();
                string[] odczyt_linie3 = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                foreach (string linia in odczyt_linie3)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    double wi = lll[10] / sumawilicznik;
                    double F = wi * lll[1]; Fy.Add(F);
                    wynik4.Add( linia + "\t" + wi + "\t" + F);
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik4);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double predykcja = Math.Round(Fy.Sum(), 3);
                textBox48.Text = "Prediction of fettle: " + predykcja.ToString();
                progressBar3.Maximum = 10000;
                progressBar3.Value = Convert.ToInt32(predykcja * 1000);
            }
            if (comboBox6.Text == "II Breakfest")
            {
                progressBar3.Value = 0; progressBar3.Maximum = 100; int krok = 0;

                progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                foreach (string data in listadat)
                {


                    decimal fettle_wynik = -1;
                    string fettle_odczyt = File.ReadAllText(data);
                    string[] s = fettle_odczyt.Split('|');
                    List<decimal> samo = new List<decimal>();
                    if (s[0] != "-") { samo.Add(Convert.ToDecimal(s[0])); }
                    if (s[1] != "-") { samo.Add(Convert.ToDecimal(s[1])); }
                    if (s[2] != "-") { samo.Add(Convert.ToDecimal(s[2])); }
                    if (s[3] != "-") { samo.Add(Convert.ToDecimal(s[3])); }
                    if (s[4] != "-") { samo.Add(Convert.ToDecimal(s[4])); }

                    if (samo.Count > 0) { fettle_wynik = samo.Average(); } else { fettle_wynik = -1; }





                    string dataa = Path.GetFileName(data);
                    List<decimal> bi = new List<decimal>(); List<decimal> w = new List<decimal>(); List<decimal> t = new List<decimal>();
                    List<decimal> bł = new List<decimal>(); List<decimal> sodium = new List<decimal>();
                    List<decimal> waga = new List<decimal>(); List<decimal> ener = new List<decimal>();

                    foreach (string dieta in Directory.GetFiles(path_dieta))
                    {
                        string dieta_odczyt = File.ReadAllText(dieta); string[] d = dieta_odczyt.Split('|');
                        DateTime data_odczyt = Convert.ToDateTime(d[0]);
                        string data_odczyt_sformatowana = data_odczyt.Year.ToString() + data_odczyt.Month.ToString() + data_odczyt.Day.ToString();
                        if (d[1] == "II Breakfest")
                        {
                            if (dataa == data_odczyt_sformatowana)
                            {

                                foreach (string produkt in Directory.GetFiles(path_produkty))
                                {
                                    string[] p = File.ReadAllText(produkt).Split('|');
                                    if (p[0] == d[2])
                                    {
                                        bi.Add(Convert.ToDecimal(p[2]) * Convert.ToDecimal(d[3]));
                                        w.Add(Convert.ToDecimal(p[3]) * Convert.ToDecimal(d[3]));
                                        t.Add(Convert.ToDecimal(p[4]) * Convert.ToDecimal(d[3]));
                                        bł.Add(Convert.ToDecimal(p[5]) * Convert.ToDecimal(d[3]));
                                        sodium.Add(Convert.ToDecimal(p[6]) * Convert.ToDecimal(d[3]));
                                        waga.Add(Convert.ToDecimal(p[7]) * Convert.ToDecimal(d[3]));
                                        ener.Add(Convert.ToDecimal(p[2]) * Convert.ToDecimal(d[3]));
                                    }




                                }


                            }
                        }

                    }
                    krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0)); if (bi.Count > 0 && fettle_wynik != -1)
                    {
                        wynik.Add(dataa + "\t" + fettle_wynik + "\t" + bi.Sum().ToString() + "\t" + w.Sum().ToString() + "\t" + t.Sum().ToString() + "\t" + bł.Sum().ToString() + "\t" + sodium.Sum().ToString() + "\t" + waga.Sum().ToString() + "\t" + ener.Sum().ToString());

                    }
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik);

                //Metoda Sheparda
                string[] odczyt_linie = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                List<string> wynik2 = new List<string>(); List<double> dystanse = new List<double>();
                foreach (string linia in odczyt_linie)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    //liczymy dystans
                    double prognoza_b = Convert.ToDouble(textBox33.Text);
                    double prognoza_w = Convert.ToDouble(textBox32.Text);
                    double prognoza_t = Convert.ToDouble(textBox31.Text);
                    double prognoza_bł = Convert.ToDouble(textBox30.Text);
                    double prognoza_sodium = Convert.ToDouble(textBox23.Text);
                    double prognoza_waga = Convert.ToDouble(textBox49.Text);
                    double prognoza_ener = Convert.ToDouble(textBox34.Text);

                    double dystans = Math.Sqrt(Math.Pow(lll[2] - prognoza_b, 2) + Math.Pow(lll[3] - prognoza_w, 2) + Math.Pow(lll[4] - prognoza_t, 2) + Math.Pow(lll[5] - prognoza_bł, 2) + Math.Pow(lll[6] - prognoza_sodium, 2) + Math.Pow(lll[7] - prognoza_waga, 2) + Math.Pow(lll[8] - prognoza_ener, 2));
                    dystanse.Add(dystans);
                    wynik2.Add(linia + "\t" + dystans.ToString());

                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik2);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double maxdystans = dystanse.Max();
                List<string> wynik3 = new List<string>(); List<double> wiliczniki = new List<double>();
                string[] odczyt_linie2 = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                foreach (string linia in odczyt_linie2)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    double wilicznik = (maxdystans - lll[9]) / (Math.Pow(maxdystans * lll[9], 2));
                    wiliczniki.Add(wilicznik);
                    wynik3.Add(linia + "\t" + wilicznik);
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik3);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double sumawilicznik = wiliczniki.Sum();
                List<string> wynik4 = new List<string>(); List<double> Fy = new List<double>();
                string[] odczyt_linie3 = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                foreach (string linia in odczyt_linie3)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    double wi = lll[10] / sumawilicznik;
                    double F = wi * lll[1]; Fy.Add(F);
                    wynik4.Add(linia + "\t" + wi + "\t" + F);
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik4);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double predykcja = Math.Round(Fy.Sum(), 3);
                textBox48.Text = "Prediction of fettle: " + predykcja.ToString();
                progressBar3.Maximum = 10000;
                progressBar3.Value = Convert.ToInt32(predykcja * 1000);
            }
            if (comboBox6.Text == "Lunch")
            {
                progressBar3.Value = 0; progressBar3.Maximum = 100; int krok = 0;

                progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                foreach (string data in listadat)
                {


                    decimal fettle_wynik = -1;
                    string fettle_odczyt = File.ReadAllText(data);
                    string[] s = fettle_odczyt.Split('|');
                    List<decimal> samo = new List<decimal>();
                    if (s[0] != "-") { samo.Add(Convert.ToDecimal(s[0])); }
                    if (s[1] != "-") { samo.Add(Convert.ToDecimal(s[1])); }
                    if (s[2] != "-") { samo.Add(Convert.ToDecimal(s[2])); }
                    if (s[3] != "-") { samo.Add(Convert.ToDecimal(s[3])); }
                    if (s[4] != "-") { samo.Add(Convert.ToDecimal(s[4])); }

                    if (samo.Count > 0) { fettle_wynik = samo.Average(); } else { fettle_wynik = -1; }





                    string dataa = Path.GetFileName(data);
                    List<decimal> bi = new List<decimal>(); List<decimal> w = new List<decimal>(); List<decimal> t = new List<decimal>();
                    List<decimal> bł = new List<decimal>(); List<decimal> sodium = new List<decimal>();
                    List<decimal> waga = new List<decimal>(); List<decimal> ener = new List<decimal>();

                    foreach (string dieta in Directory.GetFiles(path_dieta))
                    {
                        string dieta_odczyt = File.ReadAllText(dieta); string[] d = dieta_odczyt.Split('|');
                        DateTime data_odczyt = Convert.ToDateTime(d[0]);
                        string data_odczyt_sformatowana = data_odczyt.Year.ToString() + data_odczyt.Month.ToString() + data_odczyt.Day.ToString();
                        if (d[1] == "Lunch")
                        {
                            if (dataa == data_odczyt_sformatowana)
                            {

                                foreach (string produkt in Directory.GetFiles(path_produkty))
                                {
                                    string[] p = File.ReadAllText(produkt).Split('|');
                                    if (p[0] == d[2])
                                    {
                                        bi.Add(Convert.ToDecimal(p[2]) * Convert.ToDecimal(d[3]));
                                        w.Add(Convert.ToDecimal(p[3]) * Convert.ToDecimal(d[3]));
                                        t.Add(Convert.ToDecimal(p[4]) * Convert.ToDecimal(d[3]));
                                        bł.Add(Convert.ToDecimal(p[5]) * Convert.ToDecimal(d[3]));
                                        sodium.Add(Convert.ToDecimal(p[6]) * Convert.ToDecimal(d[3]));
                                        waga.Add(Convert.ToDecimal(p[7]) * Convert.ToDecimal(d[3]));
                                        ener.Add(Convert.ToDecimal(p[2]) * Convert.ToDecimal(d[3]));
                                    }




                                }


                            }
                        }

                    }
                    krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0)); if (bi.Count > 0 && fettle_wynik != -1)
                    {
                        wynik.Add(dataa + "\t" + fettle_wynik + "\t" + bi.Sum().ToString() + "\t" + w.Sum().ToString() + "\t" + t.Sum().ToString() + "\t" + bł.Sum().ToString() + "\t" + sodium.Sum().ToString() + "\t" + waga.Sum().ToString() + "\t" + ener.Sum().ToString());

                    }
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik);

                //Metoda Sheparda
                string[] odczyt_linie = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                List<string> wynik2 = new List<string>(); List<double> dystanse = new List<double>();
                foreach (string linia in odczyt_linie)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    //liczymy dystans
                    double prognoza_b = Convert.ToDouble(textBox33.Text);
                    double prognoza_w = Convert.ToDouble(textBox32.Text);
                    double prognoza_t = Convert.ToDouble(textBox31.Text);
                    double prognoza_bł = Convert.ToDouble(textBox30.Text);
                    double prognoza_sodium = Convert.ToDouble(textBox23.Text);
                    double prognoza_waga = Convert.ToDouble(textBox49.Text);
                    double prognoza_ener = Convert.ToDouble(textBox34.Text);

                    double dystans = Math.Sqrt(Math.Pow(lll[2] - prognoza_b, 2) + Math.Pow(lll[3] - prognoza_w, 2) + Math.Pow(lll[4] - prognoza_t, 2) + Math.Pow(lll[5] - prognoza_bł, 2) + Math.Pow(lll[6] - prognoza_sodium, 2) + Math.Pow(lll[7] - prognoza_waga, 2) + Math.Pow(lll[8] - prognoza_ener, 2));
                    dystanse.Add(dystans);
                    wynik2.Add(linia + "\t" + dystans.ToString());

                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik2);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double maxdystans = dystanse.Max();
                List<string> wynik3 = new List<string>(); List<double> wiliczniki = new List<double>();
                string[] odczyt_linie2 = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                foreach (string linia in odczyt_linie2)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    double wilicznik = (maxdystans - lll[9]) / (Math.Pow(maxdystans * lll[9], 2));
                    wiliczniki.Add(wilicznik);
                    wynik3.Add(linia + "\t" + wilicznik);
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik3);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double sumawilicznik = wiliczniki.Sum();
                List<string> wynik4 = new List<string>(); List<double> Fy = new List<double>();
                string[] odczyt_linie3 = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                foreach (string linia in odczyt_linie3)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    double wi = lll[10] / sumawilicznik;
                    double F = wi * lll[1]; Fy.Add(F);
                    wynik4.Add(linia + "\t" + wi + "\t" + F);
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik4);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double predykcja = Math.Round(Fy.Sum(), 3);
                textBox48.Text = "Prediction of fettle: " + predykcja.ToString();
                progressBar3.Maximum = 10000;
                progressBar3.Value = Convert.ToInt32(predykcja * 1000);
            }
            if (comboBox6.Text == "podwieczorku")
            {
                progressBar3.Value = 0; progressBar3.Maximum = 100; int krok = 0;

                progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                foreach (string data in listadat)
                {


                    decimal fettle_wynik = -1;
                    string fettle_odczyt = File.ReadAllText(data);
                    string[] s = fettle_odczyt.Split('|');
                    List<decimal> samo = new List<decimal>();
                    if (s[0] != "-") { samo.Add(Convert.ToDecimal(s[0])); }
                    if (s[1] != "-") { samo.Add(Convert.ToDecimal(s[1])); }
                    if (s[2] != "-") { samo.Add(Convert.ToDecimal(s[2])); }
                    if (s[3] != "-") { samo.Add(Convert.ToDecimal(s[3])); }
                    if (s[4] != "-") { samo.Add(Convert.ToDecimal(s[4])); }

                    if (samo.Count > 0) { fettle_wynik = samo.Average(); } else { fettle_wynik = -1; }





                    string dataa = Path.GetFileName(data);
                    List<decimal> bi = new List<decimal>(); List<decimal> w = new List<decimal>(); List<decimal> t = new List<decimal>();
                    List<decimal> bł = new List<decimal>(); List<decimal> sodium = new List<decimal>();
                    List<decimal> waga = new List<decimal>(); List<decimal> ener = new List<decimal>();

                    foreach (string dieta in Directory.GetFiles(path_dieta))
                    {
                        string dieta_odczyt = File.ReadAllText(dieta); string[] d = dieta_odczyt.Split('|');
                        DateTime data_odczyt = Convert.ToDateTime(d[0]);
                        string data_odczyt_sformatowana = data_odczyt.Year.ToString() + data_odczyt.Month.ToString() + data_odczyt.Day.ToString();
                        if (d[1] == "Dinner")
                        {
                            if (dataa == data_odczyt_sformatowana)
                            {

                                foreach (string produkt in Directory.GetFiles(path_produkty))
                                {
                                    string[] p = File.ReadAllText(produkt).Split('|');
                                    if (p[0] == d[2])
                                    {
                                        bi.Add(Convert.ToDecimal(p[2]) * Convert.ToDecimal(d[3]));
                                        w.Add(Convert.ToDecimal(p[3]) * Convert.ToDecimal(d[3]));
                                        t.Add(Convert.ToDecimal(p[4]) * Convert.ToDecimal(d[3]));
                                        bł.Add(Convert.ToDecimal(p[5]) * Convert.ToDecimal(d[3]));
                                        sodium.Add(Convert.ToDecimal(p[6]) * Convert.ToDecimal(d[3]));
                                        waga.Add(Convert.ToDecimal(p[7]) * Convert.ToDecimal(d[3]));
                                        ener.Add(Convert.ToDecimal(p[2]) * Convert.ToDecimal(d[3]));
                                    }




                                }


                            }
                        }

                    }
                    krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0)); if (bi.Count > 0 && fettle_wynik != -1)
                    {
                        wynik.Add(dataa + "\t" + fettle_wynik + "\t" + bi.Sum().ToString() + "\t" + w.Sum().ToString() + "\t" + t.Sum().ToString() + "\t" + bł.Sum().ToString() + "\t" + sodium.Sum().ToString() + "\t" + waga.Sum().ToString() + "\t" + ener.Sum().ToString());

                    }
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik);

                //Metoda Sheparda
                string[] odczyt_linie = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                List<string> wynik2 = new List<string>(); List<double> dystanse = new List<double>();
                foreach (string linia in odczyt_linie)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    //liczymy dystans
                    double prognoza_b = Convert.ToDouble(textBox33.Text);
                    double prognoza_w = Convert.ToDouble(textBox32.Text);
                    double prognoza_t = Convert.ToDouble(textBox31.Text);
                    double prognoza_bł = Convert.ToDouble(textBox30.Text);
                    double prognoza_sodium = Convert.ToDouble(textBox23.Text);
                    double prognoza_waga = Convert.ToDouble(textBox49.Text);
                    double prognoza_ener = Convert.ToDouble(textBox34.Text);

                    double dystans = Math.Sqrt(Math.Pow(lll[2] - prognoza_b, 2) + Math.Pow(lll[3] - prognoza_w, 2) + Math.Pow(lll[4] - prognoza_t, 2) + Math.Pow(lll[5] - prognoza_bł, 2) + Math.Pow(lll[6] - prognoza_sodium, 2) + Math.Pow(lll[7] - prognoza_waga, 2) + Math.Pow(lll[8] - prognoza_ener, 2));
                    dystanse.Add(dystans);
                    wynik2.Add(linia + "\t" + dystans.ToString());

                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik2);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double maxdystans = dystanse.Max();
                List<string> wynik3 = new List<string>(); List<double> wiliczniki = new List<double>();
                string[] odczyt_linie2 = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                foreach (string linia in odczyt_linie2)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    double wilicznik = (maxdystans - lll[9]) / (Math.Pow(maxdystans * lll[9], 2));
                    wiliczniki.Add(wilicznik);
                    wynik3.Add(linia + "\t" + wilicznik);
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik3);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double sumawilicznik = wiliczniki.Sum();
                List<string> wynik4 = new List<string>(); List<double> Fy = new List<double>();
                string[] odczyt_linie3 = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                foreach (string linia in odczyt_linie3)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    double wi = lll[10] / sumawilicznik;
                    double F = wi * lll[1]; Fy.Add(F);
                    wynik4.Add(linia + "\t" + wi + "\t" + F);
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik4);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double predykcja = Math.Round(Fy.Sum(), 3);
                textBox48.Text = "Prediction of fettle: " + predykcja.ToString();
                progressBar3.Maximum = 10000;
                progressBar3.Value = Convert.ToInt32(predykcja * 1000);
            }
            if (comboBox6.Text == "kolacji")
            {
                progressBar3.Value = 0; progressBar3.Maximum = 100; int krok = 0;

                progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                foreach (string data in listadat)
                {


                    decimal fettle_wynik = -1;
                    string fettle_odczyt = File.ReadAllText(data);
                    string[] s = fettle_odczyt.Split('|');
                    List<decimal> samo = new List<decimal>();
                    if (s[0] != "-") { samo.Add(Convert.ToDecimal(s[0])); }
                    if (s[1] != "-") { samo.Add(Convert.ToDecimal(s[1])); }
                    if (s[2] != "-") { samo.Add(Convert.ToDecimal(s[2])); }
                    if (s[3] != "-") { samo.Add(Convert.ToDecimal(s[3])); }
                    if (s[4] != "-") { samo.Add(Convert.ToDecimal(s[4])); }

                    if (samo.Count > 0) { fettle_wynik = samo.Average(); } else { fettle_wynik = -1; }





                    string dataa = Path.GetFileName(data);
                    List<decimal> bi = new List<decimal>(); List<decimal> w = new List<decimal>(); List<decimal> t = new List<decimal>();
                    List<decimal> bł = new List<decimal>(); List<decimal> sodium = new List<decimal>();
                    List<decimal> waga = new List<decimal>(); List<decimal> ener = new List<decimal>();

                    foreach (string dieta in Directory.GetFiles(path_dieta))
                    {
                        string dieta_odczyt = File.ReadAllText(dieta); string[] d = dieta_odczyt.Split('|');
                        DateTime data_odczyt = Convert.ToDateTime(d[0]);
                        string data_odczyt_sformatowana = data_odczyt.Year.ToString() + data_odczyt.Month.ToString() + data_odczyt.Day.ToString();
                        if (d[1] == "Supper")
                        {
                            if (dataa == data_odczyt_sformatowana)
                            {

                                foreach (string produkt in Directory.GetFiles(path_produkty))
                                {
                                    string[] p = File.ReadAllText(produkt).Split('|');
                                    if (p[0] == d[2])
                                    {
                                        bi.Add(Convert.ToDecimal(p[2]) * Convert.ToDecimal(d[3]));
                                        w.Add(Convert.ToDecimal(p[3]) * Convert.ToDecimal(d[3]));
                                        t.Add(Convert.ToDecimal(p[4]) * Convert.ToDecimal(d[3]));
                                        bł.Add(Convert.ToDecimal(p[5]) * Convert.ToDecimal(d[3]));
                                        sodium.Add(Convert.ToDecimal(p[6]) * Convert.ToDecimal(d[3]));
                                        waga.Add(Convert.ToDecimal(p[7]) * Convert.ToDecimal(d[3]));
                                        ener.Add(Convert.ToDecimal(p[2]) * Convert.ToDecimal(d[3]));
                                    }




                                }


                            }
                        }

                    }
                    krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0)); if (bi.Count > 0 && fettle_wynik != -1)
                    {
                        wynik.Add(dataa + "\t" + fettle_wynik + "\t" + bi.Sum().ToString() + "\t" + w.Sum().ToString() + "\t" + t.Sum().ToString() + "\t" + bł.Sum().ToString() + "\t" + sodium.Sum().ToString() + "\t" + waga.Sum().ToString() + "\t" + ener.Sum().ToString());

                    }
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik);

                //Metoda Sheparda
                string[] odczyt_linie = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                List<string> wynik2 = new List<string>(); List<double> dystanse = new List<double>();
                foreach (string linia in odczyt_linie)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    //liczymy dystans
                    double prognoza_b = Convert.ToDouble(textBox33.Text);
                    double prognoza_w = Convert.ToDouble(textBox32.Text);
                    double prognoza_t = Convert.ToDouble(textBox31.Text);
                    double prognoza_bł = Convert.ToDouble(textBox30.Text);
                    double prognoza_sodium = Convert.ToDouble(textBox23.Text);
                    double prognoza_waga = Convert.ToDouble(textBox49.Text);
                    double prognoza_ener = Convert.ToDouble(textBox34.Text);

                    double dystans = Math.Sqrt(Math.Pow(lll[2] - prognoza_b, 2) + Math.Pow(lll[3] - prognoza_w, 2) + Math.Pow(lll[4] - prognoza_t, 2) + Math.Pow(lll[5] - prognoza_bł, 2) + Math.Pow(lll[6] - prognoza_sodium, 2) + Math.Pow(lll[7] - prognoza_waga, 2) + Math.Pow(lll[8] - prognoza_ener, 2));
                    dystanse.Add(dystans);
                    wynik2.Add(linia + "\t" + dystans.ToString());

                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik2);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double maxdystans = dystanse.Max();
                List<string> wynik3 = new List<string>(); List<double> wiliczniki = new List<double>();
                string[] odczyt_linie2 = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                foreach (string linia in odczyt_linie2)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    double wilicznik = (maxdystans - lll[9]) / (Math.Pow(maxdystans * lll[9], 2));
                    wiliczniki.Add(wilicznik);
                    wynik3.Add(linia + "\t" + wilicznik);
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik3);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double sumawilicznik = wiliczniki.Sum();
                List<string> wynik4 = new List<string>(); List<double> Fy = new List<double>();
                string[] odczyt_linie3 = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text);
                foreach (string linia in odczyt_linie3)
                {
                    string[] l = linia.Split('\t');
                    List<double> lll = new List<double>();
                    foreach (string ll in l) { lll.Add(Convert.ToDouble(ll)); }
                    double wi = lll[10] / sumawilicznik;
                    double F = wi * lll[1]; Fy.Add(F);
                    wynik4.Add(linia + "\t" + wi + "\t" + F);
                }
                File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\shepard_" + comboBox6.Text, wynik4);
                krok++; progressBar3.Value = Convert.ToInt32(Math.Round(Convert.ToDecimal(Convert.ToDecimal(krok) / (Convert.ToDecimal(listadat.Count) + 3)), 0));
                double predykcja = Math.Round(Fy.Sum(), 3);
                textBox48.Text = "Prediction of fettle: " + predykcja.ToString();
                progressBar3.Maximum = 10000;
                progressBar3.Value = Convert.ToInt32(predykcja * 1000);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            foreach (string i in comboBox1.Items) {
                f.nazwyproduktów.Add(i);
        }
            if (f.ShowDialog() == DialogResult.OK) {
                comboBox7.Text = f.listBox1.SelectedItem.ToString();
            
            
            
            }
        
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            foreach (string i in comboBox1.Items)
            {
                f.nazwyproduktów.Add(i);
            }
            if (f.ShowDialog() == DialogResult.OK)
            {
                comboBox1.Text = f.listBox1.SelectedItem.ToString();



            }
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox10.Text == "tabela") { treeView1.Visible = false; }
            if (comboBox10.Text == "drzewo") { treeView1.Visible = true ; }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listView1.Select(); listView1.Focus();  listView1.HideSelection = false;
            foreach (ListViewItem i in listView1.Items)
            {
                
                if (treeView1.SelectedNode.Name.ToString() == i.SubItems[3].Text)
                {

                    i.Selected = true;
                    i.Focused = true;
                }
                else { i.Selected = false; i.Focused = false; }

                


            }
            if (listView1.SelectedItems.Count == 1) { button2.Enabled = true; } else { button2.Enabled = false; }
            if (listView1.SelectedItems.Count == 1) { comboBox1.Text = listView1.SelectedItems[0].Text; }
            //MessageBox.Show(treeView1.SelectedNode.Name.ToString());
        }

        private BackgroundWorker bw = new BackgroundWorker();
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            for (int i = 1; (i <= 10); i++)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    System.Threading.Thread.Sleep(500);
                    worker.ReportProgress((i * 10));
                }
            }
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                this.textBox48.Text = "Anulowano analizę... Aby rozpocząć kolejną kliknij na przycisk raz jeszcze";
            }

            else if (!(e.Error == null))
            {
                this.textBox48.Text = ("Błąd: " + e.Error.Message);
            }

            else
            {
                //this.textBox48.Text = "Done!";
            }
        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.textBox48.Text = ("Przeprowadzam analizę..." + e.ProgressPercentage.ToString() + "%");
        }

        private void chart3_Click(object sender, EventArgs e)
        {
            if (chart3.Series.Count > 1)
            {
                ZobaczWykres z = new ZobaczWykres();
                z.chart3.Legends.Clear();
                foreach (Series s in chart3.Series) { z.chart3.Series.Add(s); }
                z.ShowDialog();
            }
        }

        private void chart3_MouseDown(object sender, MouseEventArgs e)
        {
//chart3.Scale(2);
        }

        private void chart3_MouseUp(object sender, MouseEventArgs e)
        {
            //chart3.Scale(1);
        }

        private void chart2_Click(object sender, EventArgs e)
        {
            if (chart2.Series.Count > 0)
            {
                ZobaczWykres z = new ZobaczWykres();
                foreach (Series s in chart2.Series) { z.chart3.Series.Add(s); }
                z.ShowDialog();
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            if (chart1.Series.Count > 0)
            {
                ZobaczWykres z = new ZobaczWykres();
                foreach (Series s in chart1.Series) { z.chart3.Series.Add(s); }
                z.ShowDialog();
            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {
            listView2.SelectedItems[0].Selected = false;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            //if ("OK" != serwer("popraw", "tab=users&param=uid&war=" + uid + "&co=checksum&jak=1")) { MessageBox.Show("Błąd serwera!"); }
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(serwer("tabela", "tab=meals&selektor=uid&war=34"));
            //XmlReader r = new XmlReader.
            //    (doc);
            
            
            using (StreamWriter w = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\meals.xml"))
            {
                w.Write(serwer("tabelax", "tab=meals&selektor=uid&war=34"));
            }
            XDocument doc = XDocument.Load(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\meals.xml");

            foreach (var dm in doc.Descendants("meals"))
            {


                MessageBox.Show(dm.Element("mid").Value);
        
            }



            //string[] meals = XDocument.Load(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\meals.xml").Descendants("").Select(element => element.Value).ToArray();

            //foreach (string m in meals)
            //{
            //    MessageBox.Show(m);
            //}
            //Console.ReadLine();
        }
        public string login; public string passmd5;
        private void Form1_Load(object sender, EventArgs e)
        {
            label91.Text = "Połączenie z serwerem PancreApp: " + serwer("ping", "");
            if (!File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\id"))
            {
                Logowanie l = new Logowanie();
                if (l.ShowDialog() == DialogResult.OK)
                {

                    textBox50.Text = login;

                }


            }
            else {
                using (StreamReader r = new StreamReader(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\id"))
                {
                    string[] id = r.ReadToEnd().Split('\t');
                    r.Close();
                    login = id[0];
                    passmd5 = id[1];

                    string wynik = serwer("check_credentials", "login=" + login + "&pass=" + passmd5);
                    if (wynik != "NIE" && wynik != "BLAD") { zalogowany = true; textBox50.Text = login; }
                    else
                    {
                        zalogowany = false;

                        if (polzserv == true)
                        {
                            MessageBox.Show("Zapamiętane dane logowania są nie są poprawne! Zmieniłeś coś korzystając z urządzenia mobilnego? Aplikacja zostanie uruchomiona w trybie offline. Aby przejść do trybu online i podać dane jeszcze raz zamknij i uruchom ponownie aplikację.");
                            File.Delete(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\id");
                        }
                        else { MessageBox.Show("Nie można nawiązać połączenia z serwerem. Aplikacja zostanie uruchomiona w trybie offline."); }
                    } 

                }
            
            
            }
            
            
            //Wykonaj stare błędy...
            if (File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\bufor")) {
                File.Move(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\bufor", System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\bufor_");
                string[] odczytbuforu = File.ReadAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\bufor_");
                foreach (string o in odczytbuforu) {
                    string[] oo = o.Split('\t');
                    string dupa = serwer(oo[0], oo[1]);
                
                
                }
                File.Delete(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\pancreapp\\bufor_");
            
            }
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                MessageBox.Show("What the Ctrl+F?");
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            string wynik = serwer("add_meals", "uid=" + uid + "&kind_of_meal=0" + "&date=0" + "&fettle=5");
            if ("OK" != wynik) { MessageBox.Show("Bład serwera! Opis: " + wynik); }
        }
    }

       
    



    class Sorter : System.Collections.IComparer
    {
        public int Column = 0;
        public System.Windows.Forms.SortOrder Order = SortOrder.Ascending;
        public int Compare(object x, object y) // IComparer Member
        {
            if (!(x is ListViewItem))
                return (0);
            if (!(y is ListViewItem))
                return (0);

            ListViewItem l1 = (ListViewItem)x;
            ListViewItem l2 = (ListViewItem)y;

            if (l1.ListView.Columns[Column].Tag == null)
            {
                l1.ListView.Columns[Column].Tag = "Text";
            }

            if (l1.ListView.Columns[Column].Tag.ToString() == "Numeric")
            {
                float fl1 = float.Parse(l1.SubItems[Column].Text);
                float fl2 = float.Parse(l2.SubItems[Column].Text);

                if (Order == SortOrder.Ascending)
                {
                    return fl1.CompareTo(fl2);
                }
                else
                {
                    return fl2.CompareTo(fl1);
                }
            }
            else
            {
                string str1 = l1.SubItems[Column].Text;
                string str2 = l2.SubItems[Column].Text;

                if (Order == SortOrder.Ascending)
                {
                    return str1.CompareTo(str2);
                }
                else
                {
                    return str2.CompareTo(str1);
                }
            }
        }
    }
}
