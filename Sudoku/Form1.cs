using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
namespace Sudoku
{

    public partial class Form1 : Form
    {
        #region tanımlamalar
        static string file_way3 = @"D:\Projeler\Sudoku2\Sudoku\bin\Debug\metinbelgesi3.txt";
        static string file_way2 = @"D:\Projeler\Sudoku2\Sudoku\bin\Debug\metinbelgesi2.txt";
        static string file_way = @"D:\Projeler\Sudoku2\Sudoku\bin\Debug\metinbelgesi.txt";
        //İşlem yapacağımız dosyanın yolunu belirtiyoruz.
        static FileStream fs3 = new FileStream(file_way3, FileMode.OpenOrCreate, FileAccess.Write);
        static FileStream fs2 = new FileStream(file_way2, FileMode.OpenOrCreate, FileAccess.Write);
        static FileStream fs = new FileStream(file_way, FileMode.OpenOrCreate, FileAccess.Write);
        int speed = 1000;
        StreamReader sw1;
        StreamReader sw4;
        StreamReader sw5;
        //Bir file stream nesnesi oluşturuyoruz. 1.parametre dosya yolunu,
        //2.parametre dosya varsa açılacağını yoksa oluşturulacağını belirtir,
        //3.parametre dosyaya erişimin veri yazmak için olacağını gösterir.
        StreamWriter sw3 = new StreamWriter(fs3);
        StreamWriter sw2 = new StreamWriter(fs2);
        StreamWriter sw = new StreamWriter(fs);
        //Dosyadan Çektiğimiz verileri tuttuğumuz diziler
        char[,] data = new char[9, 9];
        char[,] data2 = new char[9, 9];
        char[,] data3 = new char[9, 9];
        //İhtimaller
        char[] possibility = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        List<TextBox> myTextBoxes = new List<TextBox>();
        int step3 = 0;
        int step = 0;
        int step2 = 0;
        Stopwatch stopWatch3 = new Stopwatch();
        Stopwatch stopWatch2 = new Stopwatch();
        Stopwatch stopWatch = new Stopwatch();
        //Threadler
        private Thread th1;
        private Thread th2;
        private Thread th3;
        //Animasyon Adımları
        ArrayList Animation_Steps = new ArrayList();
        ArrayList Animation_Steps2 = new ArrayList();
        ArrayList Animation_Steps3 = new ArrayList();
        #endregion

        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }
        //Threadları başlatan fonksiyon
        public void StartMultipleThread()
        {
            Thread th1 = new Thread(new ThreadStart(run));
            Thread th2 = new Thread(new ThreadStart(run2));
            Thread th3 = new Thread(new ThreadStart(run3));
            th1.Start();
            th2.Start();
            th3.Start();

            this.th1 = th1;
            this.th2 = th2;
            this.th3 = th3;

        }

        #region Solution1
        //Dosyadan okuma yapan Fonksiyon
        public void File_Reader()
        {
            List<TextBox> myTextBoxes = new List<TextBox>();
            for (int i = 1; i < 82; i++)
            {
                myTextBoxes.Add((TextBox)Controls.Find("textBox" + i, true)[0]);
            }
            this.myTextBoxes = myTextBoxes;
            StreamReader reader = new StreamReader("sudoku.txt");
            string contents = reader.ReadToEnd();
            char[,] data = new char[9, 9];

            var lines = contents.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int row = 0;
            foreach (string line in lines)
            {
                int column = 0;
                foreach (char character in line)
                {
                    data[row, column] = character;
                    column++;
                }
                row++;
            }
            reader.Close();
            for (int j = -1; j < 80; j++)
            {
                for (int a = 0; a < 9; a++)
                {
                    for (int b = 0; b < 9; b++)
                    {
                        j++;
                        myTextBoxes[j].Text = data[a, b].ToString();
                        if (data[a, b] == '*')
                        {
                            myTextBoxes[j].Text = " ";
                        }
                    }

                }
            }
            this.data = data;
        }
       
        //Kontrolleri yapan Fonksiyon
        bool check(char[,] data, int row, int col)
        {
            for (int i = 0; i < 9; ++i)
            {
                int p = 3 * (row / 3) + i % 3, q = 3 * (col / 3) + i / 3;
                if (i != row && data[i, col] == data[row, col] // col
                  || i != col && data[row, i] == data[row, col] // row
                  || (p != row || q != col) && data[p, q] == data[row, col])
                { // box
                    return false;
                }
            }
            return true;
        }
       
        //Çözümü yapan Fonksiyon
        bool solve(char[,] data, int row, int col)
        {
            stopWatch.Start();
            bool complete = false;
            int i, j = col;

            for (i = row; i < 9; ++i)
            {
                for (; j < 9; ++j)
                {
                    if (data[i, j] == '*')
                    {
                        int nextcol = (j + 1) % 9, nextrow = i;
                        if (nextcol == '*')
                        {
                            nextrow = i + 1;
                            complete = nextrow == 9;
                        }
                        for (int v = 0; v < 9; ++v)
                        {
                            data[i, j] = possibility[v];
                            step++;
                            Txt_Writer(data);
                            // backtracking prune
                            if (check(data, i, j))
                            {
                                if (complete || solve(data, nextrow, nextcol))
                                {
                                    label2.Text = "Adım Sayısı: " + step.ToString();
                                    stopWatch.Stop();
                                    return true;
                                }
                            }
                        }
                        data[i, j] = '*';
                        return false;
                    }

                }
                j = 0;
            }
            return true;
        }
        
        //Textboxlara yazdıran fonksiyon
        public void run()
        {
            if (solve(data, 0, 8))
            {
                for (int j = -1; j < 80; j++)
                {
                    for (int a = 0; a < 9; a++)
                    {
                        for (int b = 0; b < 9; b++)
                        {
                            j++;

                            myTextBoxes[j].Text = data[a, b].ToString();
                            if (data[a, b] == '*')
                            {
                                myTextBoxes[j].Text = " ";
                            }
                        }

                    }
                }
            }
            else
            { MessageBox.Show("Çözümü olmayan bir sudoku denediniz.!", "Bilgilendirme Penceresi"); }

            File_Closing();
            Data_Reader();
            animation(Animation_Steps);
            if (th2.IsAlive || th3.IsAlive)
            {
                th2.Abort();
                th3.Abort();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}.{1:00}", ts.Seconds,
    ts.Milliseconds);
                label9.Text = "Süre: " + elapsedTime;
                label3.Text = "Adım Sayısı: " + step2.ToString();
                label4.Text = "Adım Sayısı: " + step3.ToString();
            }

        }
       
        //Animasyon için okuma yapıp,dizileri tutan fonksiyon
        public void Data_Reader()
        {
            FileStream fs1 = new FileStream(file_way, FileMode.Open, FileAccess.Read);
            StreamReader sw1 = new StreamReader(fs1);
            int i = 1;
            string yazi = sw1.ReadLine();
            string[] array = new string[9];
            while (yazi != null)
            {
                if (yazi == "")
                {
                    yazi = sw1.ReadLine();
                    if (yazi == "")
                    {
                        break;
                    }
                    continue;
                }

                if (i % 9 == 0)
                {
                    array[i - 1] = yazi;
                    for (int j = 0; j < 9; j++)
                    {
                        Animation_Steps.Add(array[j]);
                    }
                    Array.Clear(array, 0, array.Length);

                    i = 0;

                }
                else if (i % 9 != 0)
                {
                    array[i - 1] = yazi;
                }
                yazi = sw1.ReadLine();
                i++;
            }

            this.sw1 = sw1;
        }
       
        //Metin belgelerine tüm adımları yazdıran fonksiyon
        public void Txt_Writer(char[,] data)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sw.Write(" ");
                    sw.Write(data[i, j]);

                }
                sw.WriteLine();
            }
            sw.WriteLine();
        }
      
        //Dosyaları kapatmaya yarayan fonksiyon
        public void File_Closing()
        {
            //Dosyaya ekleyeceğimiz iki satırlık yazıyı WriteLine() metodu ile yazacağız.
            sw.Flush();
            //Veriyi tampon bölgeden dosyaya aktardık.

            sw.Close();
            fs.Close();
        }
     
        //Animasyonu başlatan Fonksiyon
        public void animation(ArrayList Animation_Steps)
        {
            int a = 0;
            foreach (var index in Animation_Steps)
            {
                if (a % 9 == 0)
                {
                    label28.Text = Animation_Steps[a].ToString();
                }
                if (a % 9 == 1)
                {
                    label29.Text = Animation_Steps[a].ToString();
                }
                if (a % 9 == 2)
                {
                    label30.Text = Animation_Steps[a].ToString();
                }
                if (a % 9 == 3)
                {
                    label31.Text = Animation_Steps[a].ToString();
                }
                if (a % 9 == 4)
                {
                    label32.Text = Animation_Steps[a].ToString();
                }
                if (a % 9 == 5)
                {
                    label33.Text = Animation_Steps[a].ToString();
                }
                if (a % 9 == 6)
                {
                    label34.Text = Animation_Steps[a].ToString();
                }
                if (a % 9 == 7)
                {
                    label35.Text = Animation_Steps[a].ToString();
                }
                if (a % 9 == 8)
                {
                    label36.Text = Animation_Steps[a].ToString();
                }
                for (int i = 0; i < speed; i++) ;
                a++;
            }
        }

        #endregion

        #region Solution2
        //Dosyadan okuma yapan Fonksiyon
        public void File_Reader2()
        {
            List<TextBox> myTextBoxes = new List<TextBox>();
            for (int i = 1; i < 82; i++)
            {
                myTextBoxes.Add((TextBox)Controls.Find("textBox" + i, true)[0]);
            }
            this.myTextBoxes = myTextBoxes;
            StreamReader reader = new StreamReader("sudoku.txt");
            string contents = reader.ReadToEnd();
            char[,] data2 = new char[9, 9];

            var lines = contents.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int row = 0;
            foreach (string line in lines)
            {
                int column = 0;
                foreach (char character in line)
                {
                    data2[row, column] = character;
                    column++;
                }
                row++;
            }
            reader.Close();
            for (int j = -1; j < 80; j++)
            {
                for (int a = 0; a < 9; a++)
                {
                    for (int b = 0; b < 9; b++)
                    {
                        j++;
                        myTextBoxes[j].Text = data2[a, b].ToString();
                        if (data2[a, b] == '*')
                        {
                            myTextBoxes[j].Text = " ";
                        }
                    }

                }
            }
            this.data2 = data2;
        }

        //Çözümü başlatan fonksiyon
        public void solveSudoku(char[,] data2)
        {
            stopWatch2.Start();
            solve2(data2, 0);
        }

        //Çözümü yapan Fonksiyon
        bool solve2(char[,] data2, int ind)
        {
            if (ind == 81)
            {
                stopWatch2.Stop();
                return true;
                // solved
            }

            int row = ind / 9;
            int col = ind % 9;

            // Advance forward on cells that are prefilled
            if (data2[row, col] != '*')
                return solve2(data2, ind + 1);

            else
            {
                // we are positioned on something we need to fill in.
                // Try all possibilities

                for (int i = 0; i < 9; i++)
                {
                    if (check2(data2, row, col, possibility[i]))
                    {
                        data2[row, col] = possibility[i];
                        Txt_Writer2(data2);
                        if (solve2(data2, ind + 1))
                        {

                            return true;
                        }
                        data2[row, col] = '*';
                        if (th2.IsAlive)
                            step2++;
                        // unmake move
                    }
                }
            }

            // no solution
            return false;
        }

        //Metin belgelerine tüm adımları yazdıran fonksiyon
        public void Txt_Writer2(char[,] data)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sw2.Write(" ");
                    sw2.Write(data[i, j]);

                }
                sw2.WriteLine();
            }
            sw2.WriteLine();
        }

        //Kontrolleri yapan Fonksiyon
        public bool check2(char[,] data2, int row, int col, int c)
        {
            // check columns/rows
            for (int i = 8; i >= 0; i--)
            {

                if (data2[i, col] == c) return false;
            }
            for (int i = 0; i < 9; i++)
            {
                if (data2[row, i] == c) return false;

            }

            int rowStart = row - row % 3;
            int colStart = col - col % 3;

            for (int m = 2; m >= 0; m--)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (data2[rowStart + k, colStart + m] == c) return false;
                }
            }

            return true;
        }

        //Textboxlara yazdıran fonksiyon
        public void run2()
        {
            solveSudoku(data2);
            label3.Text = "Adım Sayısı: " + step2.ToString();
            File_Closing2();
            Data_Reader2();
            animation2(Animation_Steps2);
            if (th1.IsAlive || th3.IsAlive)
            {

                th1.Abort();
                th3.Abort();
                TimeSpan ts2 = stopWatch2.Elapsed;
                string elapsedTime2 = String.Format("{0:00}.{1:00}", ts2.Seconds,
ts2.Milliseconds);
                label1.Text = "Süre: " + elapsedTime2;
                label2.Text = "Adım Sayısı: " + step.ToString();
                label4.Text = "Adım Sayısı: " + step3.ToString();
            }


            for (int j = -1; j < 80; j++)
            {
                for (int a = 0; a < 9; a++)
                {
                    for (int b = 0; b < 9; b++)
                    {
                        j++;
                        myTextBoxes[j].Text = data2[a, b].ToString();
                        if (data2[a, b] == '*')
                        {
                            myTextBoxes[j].Text = " ";
                        }
                    }

                }
            }


        }
    
        //Dosyaları kapatmaya yarayan fonksiyon
        public void File_Closing2()
        {
            //Dosyaya ekleyeceğimiz iki satırlık yazıyı WriteLine() metodu ile yazacağız.
            //  sw2.Flush();
            //Veriyi tampon bölgeden dosyaya aktardık.

            sw2.Close();
            fs2.Close();
        }

        //Animasyon için okuma yapıp,dizileri tutan fonksiyon
        public void Data_Reader2()
        {
            FileStream fs4 = new FileStream(file_way2, FileMode.Open, FileAccess.Read);
            StreamReader sw4 = new StreamReader(fs4);
            int i = 1;
            string yazi = sw4.ReadLine();
            string[] array = new string[9];
            while (yazi != null)
            {
                if (yazi == "")
                {
                    yazi = sw4.ReadLine();
                    if (yazi == "")
                    {
                        break;
                    }
                    continue;
                }

                if (i % 9 == 0)
                {
                    array[i - 1] = yazi;
                    for (int j = 0; j < 9; j++)
                    {
                        Animation_Steps2.Add(array[j]);
                    }
                    Array.Clear(array, 0, array.Length);

                    i = 0;

                }
                else if (i % 9 != 0)
                {
                    array[i - 1] = yazi;
                }
                yazi = sw4.ReadLine();
                i++;


            }
            this.sw4 = sw4;
        }
        //Animasyonu başlatan Fonksiyon
        public void animation2(ArrayList Animation_Steps2)
        {
            int a = 0;
            foreach (var index in Animation_Steps2)
            {
                if (a % 9 == 0)
                {
                    label19.Text = Animation_Steps2[a].ToString();
                }
                if (a % 9 == 1)
                {
                    label20.Text = Animation_Steps2[a].ToString();
                }
                if (a % 9 == 2)
                {
                    label21.Text = Animation_Steps2[a].ToString();
                }
                if (a % 9 == 3)
                {
                    label22.Text = Animation_Steps2[a].ToString();
                }
                if (a % 9 == 4)
                {
                    label23.Text = Animation_Steps2[a].ToString();
                }
                if (a % 9 == 5)
                {
                    label24.Text = Animation_Steps2[a].ToString();
                }
                if (a % 9 == 6)
                {
                    label25.Text = Animation_Steps2[a].ToString();
                }
                if (a % 9 == 7)
                {
                    label26.Text = Animation_Steps2[a].ToString();
                }
                if (a % 9 == 8)
                {
                    label27.Text = Animation_Steps2[a].ToString();
                }
                for (int i = 0; i < speed; i++) ;

                a++;
            }
        }

        #endregion

        #region Solution3
        //Dosyadan okuma yapan Fonksiyon
        public void File_Reader3()
        {
            List<TextBox> myTextBoxes = new List<TextBox>();
            for (int i = 1; i < 82; i++)
            {
                myTextBoxes.Add((TextBox)Controls.Find("textBox" + i, true)[0]);
            }
            this.myTextBoxes = myTextBoxes;
            StreamReader reader = new StreamReader("sudoku.txt");
            string contents = reader.ReadToEnd();
            char[,] data3 = new char[9, 9];

            var lines = contents.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int row = 0;
            foreach (string line in lines)
            {
                int column = 0;
                foreach (char character in line)
                {
                    data3[row, column] = character;
                    column++;
                }
                row++;
            }
            reader.Close();
            for (int j = -1; j < 80; j++)
            {
                for (int a = 0; a < 9; a++)
                {
                    for (int b = 0; b < 9; b++)
                    {
                        j++;
                        myTextBoxes[j].Text = data3[a, b].ToString();
                        if (data3[a, b] == '*')
                        {
                            myTextBoxes[j].Text = " ";
                        }
                    }

                }
            }
            this.data3 = data3;
        }

        //Çözümü başlatan fonksiyon
        public void solveSudoku3(char[,] data3)
        {
            stopWatch3.Start();
            solve3(data3, 0);
        }

        //Çözümü yapan Fonksiyon
        bool solve3(char[,] data3, int ind)
        {
            if (ind == 81)
            {
                stopWatch3.Stop();
                return true;
                // solved
            }
            int row = ind / 9;
            int col = ind % 9;

            // Advance forward on cells that are prefilled
            if (data3[row, col] != '*') return solve3(data3, ind + 1);

            else
            {
                // we are positioned on something we need to fill in.
                // Try all possibilities

                for (int i = 8; i >= 0; i--)
                {
                    if (check3(data3, row, col, possibility[i]))
                    {
                        data3[row, col] = possibility[i];
                        Txt_Writer3(data3);
                        if (solve3(data3, ind + 1))
                        {

                            return true;
                        }
                        data3[row, col] = '*';
                        step3++;
                        // unmake move
                    }
                }
            }
            // no solution
            return false;
        }

        //Kontrolleri yapan Fonksiyon
        public bool check3(char[,] data3, int row, int col, int c)
        {
            // check columns/rows
            for (int i = 8; i >= 0; i--)
            {
                if (data3[row, i] == c) return false;
                if (data3[i, col] == c) return false;

            }

            int rowStart = row - row % 3;
            int colStart = col - col % 3;

            for (int m = 2; m >= 0; m--)
            {
                for (int k = 2; k >= 0; k--)
                {
                    if (data3[rowStart + k, colStart + m] == c) return false;

                }
            }

            return true;
        }

        //Textboxlara yazdıran fonksiyon
        public void run3()
        {
            solveSudoku3(data3);
            label4.Text = "Adım Sayısı: " + step3.ToString();
            File_Closing3();
            Data_Reader3();
            animation3(Animation_Steps3);
            if (th1.IsAlive || th2.IsAlive)
            {
                th1.Abort();
                th2.Abort();
                TimeSpan ts3 = stopWatch3.Elapsed;
                string elapsedTime3 = String.Format("{0:00}.{1:00}", ts3.Seconds,
ts3.Milliseconds);
                label8.Text = "Süre: " + elapsedTime3;
                label2.Text = "Adım Sayısı: " + step.ToString();
                label3.Text = "Adım Sayısı: " + step2.ToString();
            }

            for (int j = -1; j < 80; j++)
            {
                for (int a = 0; a < 9; a++)
                {
                    for (int b = 0; b < 9; b++)
                    {
                        j++;
                        myTextBoxes[j].Text = data3[a, b].ToString();
                        if (data3[a, b] == '*')
                        {
                            myTextBoxes[j].Text = " ";
                        }
                    }

                }
            }


        }

        //Metin belgelerine yazan kod
        public void Txt_Writer3(char[,] data)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sw3.Write(" ");
                    sw3.Write(data[i, j]);

                }
                sw3.WriteLine();
            }
            sw3.WriteLine();
        }

        //Dosyaları kapatmaya yarayan fonksiyon
        public void File_Closing3()
        {
            //Dosyaya ekleyeceğimiz iki satırlık yazıyı WriteLine() metodu ile yazacağız.
            // sw3.Flush();
            //Veriyi tampon bölgeden dosyaya aktardık.

            sw3.Close();
            fs3.Close();
        }

        //Animasyon için okuma yapıp,dizileri tutan fonksiyon
        public void Data_Reader3()
        {
            FileStream fs5 = new FileStream(file_way3, FileMode.Open, FileAccess.Read);
            StreamReader sw5 = new StreamReader(fs5);

            int i = 1;
            string yazi = sw5.ReadLine();
            string[] array = new string[9];
            while (yazi != null)
            {
                if (yazi == "")
                {
                    yazi = sw5.ReadLine();
                    if (yazi == "")
                    {
                        break;
                    }
                    continue;
                }

                if (i % 9 == 0)
                {
                    array[i - 1] = yazi;
                    for (int j = 0; j < 9; j++)
                    {
                        Animation_Steps3.Add(array[j]);
                    }
                    Array.Clear(array, 0, array.Length);

                    i = 0;

                }
                else if (i % 9 != 0)
                {
                    array[i - 1] = yazi;
                }
                yazi = sw5.ReadLine();
                i++;
            }
            this.sw5 = sw5;
        }

        //Animasyonu başlatan Fonksiyon
        public void animation3(ArrayList Animation_Steps3)
        {
            int a = 0;
            foreach (var index in Animation_Steps3)
            {


                if (a % 9 == 0)
                {
                    label10.Text = Animation_Steps3[a].ToString();
                }
                if (a % 9 == 1)
                {
                    label11.Text = Animation_Steps3[a].ToString();
                }
                if (a % 9 == 2)
                {
                    label12.Text = Animation_Steps3[a].ToString();
                }
                if (a % 9 == 3)
                {
                    label13.Text = Animation_Steps3[a].ToString();
                }
                if (a % 9 == 4)
                {
                    label14.Text = Animation_Steps3[a].ToString();
                }
                if (a % 9 == 5)
                {
                    label15.Text = Animation_Steps3[a].ToString();
                }
                if (a % 9 == 6)
                {
                    label16.Text = Animation_Steps3[a].ToString();
                }
                if (a % 9 == 7)
                {
                    label17.Text = Animation_Steps3[a].ToString();
                }
                if (a % 9 == 8)
                {
                    label18.Text = Animation_Steps3[a].ToString();
                }
                for (int i = 0; i < speed; i++) ;
                a++;
            }

        }
        #endregion

        #region Butonlar
        private void button1_Click(object sender, EventArgs e)
        {
            File_Reader();
            File_Reader2();
            File_Reader3();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartMultipleThread();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", "D:\\Projeler\\Sudoku2\\Sudoku\\bin\\Debug\\metinbelgesi.txt");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", "D:\\Projeler\\Sudoku2\\Sudoku\\bin\\Debug\\metinbelgesi2.txt");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", "D:\\Projeler\\Sudoku2\\Sudoku\\bin\\Debug\\metinbelgesi3.txt");
        }

        private void anime_Click(object sender, EventArgs e)
        {
            speed = speed * 10;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            speed = speed / 10;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            speed = 1000000000;
        }
        #endregion
    }
}
