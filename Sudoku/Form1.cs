using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
          //  textbox();
        }
        public void oku()
        {
            List<TextBox> myTextBoxes = new List<TextBox>();
            for (int i = 1; i < 82; i++)
            {

                //This String should refer to = textBox2, textBox3, etc
                myTextBoxes.Add((TextBox)Controls.Find("textBox" + i, true)[0]);
                textBox82.Text = myTextBoxes.Count.ToString();
            }
            StreamReader reader = new StreamReader("sudoku.txt");
string contents = reader.ReadToEnd();         
char[] dataarray = new char[contents.Length];
reader.Close();
int len = contents.Length;
for (int j=0; j < 82; j++) myTextBoxes[82].Text= dataarray[j].ToString() ; // Copy process
        }
        /*
         public void dosyadanOku()
        {
            List<TextBox> myTextBoxes = new List<TextBox>();
            for (int i = 1; i < 82; i++)
            {

                //This String should refer to = textBox2, textBox3, etc
                myTextBoxes.Add((TextBox)Controls.Find("textBox" + i, true)[0]);
                textBox82.Text = myTextBoxes.Count.ToString();
            }
            string dosya_yolu = @"C:\sudoku.txt";

            //Okuma işlem yapacağımız dosyanın yolunu belirtiyoruz.
            FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
            //Bir file stream nesnesi oluşturuyoruz. 1.parametre dosya yolunu,
            //2.parametre dosyanın açılacağını,
            //3.parametre dosyaya erişimin veri okumak için olacağını gösterir.
            StreamReader sw = new StreamReader(fs);
            string yazi = sw.ReadLine();
            //Okuma işlemi için bir StreamReader nesnesi oluşturduk.
            while (sw != null)
            {
                for (int i = 0; i <82; i++)
                {

              //      string body = File.ReadAllText("sudoku.txt");
                 //   foreach (char c in body) myTextBoxes[i].Text=c.ToString();
                    
                myTextBoxes[i].Text = sw.ReadToEn();

         }
     }
     //Satır satır okuma işlemini gerçekleştirdik ve ekrana yazdırdık
     //Son satır okunduktan sonra okuma işlemini bitirdik
    sw.Close();
    fs.Close();
            //İşimiz bitince kullandığımız nesneleri iade ettik.
        }
         */
        private void button1_Click(object sender, EventArgs e)
        {
           oku();
        }
       
    }
}
