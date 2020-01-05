using GemBox.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public bool dosyadanOku(string aranan, string uzanti)
        {
            string yeniyazi = "";
            string dosya_yolu = @"C:\Users\raman\Desktop\Osman\kelime-listesi" + dosyauzantisi;
            //Okuma işlem yapacağımız dosyanın yolunu belirtiyoruz.
            FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
            //Bir file stream nesnesi oluşturuyoruz. 1.parametre dosya yolunu,
            //2.parametre dosyanın açılacağını,
            //3.parametre dosyaya erişimin veri okumak için olacağını gösterir.
            StreamReader sw = new StreamReader(fs);
            //Okuma işlemi için bir StreamReader nesnesi oluşturduk.
            string yazi = sw.ReadLine();
            while (yazi != null)
            {
                yeniyazi += yazi;
                yazi = sw.ReadLine();
            }
            BruteForce(aranan, yeniyazi);
            //Satır satır okuma işlemini gerçekleştirdik ve ekrana yazdırdık
            //Son satır okunduktan sonra okuma işlemini bitirdik
            sw.Close();
            fs.Close();
            return true;
        }

        public void BruteForce(string aranan, string text)
        {
            for (int i = 0; i <= text.Length - aranan.Length; i++) //Text ve aranan boyutunun farkı kadar dön
            {
                int j = 0;
                while (j < aranan.Length && aranan[j] == text[i + j]) //Harfler eşitse
                {
                    j++;
                }
                if (j == aranan.Length) //Karşılaştırma sayısı aranan boyutuna eşitse eşleşme vardır
                {
                    //  MessageBox.Show(i.ToString()); // Eşleşme konumunu döndür 
                    listBox2.Items.Add(i + ". indiste " + aranan + " kelimesi bulunmuştur");
                }
            }

        }

        public class Spelling
        {
            private Dictionary<String, int> _dictionary = new Dictionary<String, int>();
            private static Regex _wordRegex = new Regex("[a-z]+", RegexOptions.Compiled);

            public Spelling()
            {
                string fileContent = File.ReadAllText(@"C:\Users\raman\Desktop\Osman\big.txt");
                List<string> wordList = fileContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var word in wordList)
                {
                    string trimmedWord = word.Trim().ToLower();
                    if (_wordRegex.IsMatch(trimmedWord))
                    {
                        if (_dictionary.ContainsKey(trimmedWord))
                            _dictionary[trimmedWord]++;
                        else
                            _dictionary.Add(trimmedWord, 1);
                    }
                }
            }

            public string Correct(string word)
            {
                if (string.IsNullOrEmpty(word))
                    return word;

                word = word.ToLower();

                // known()
                if (_dictionary.ContainsKey(word))
                    return word;

                List<String> list = Edits(word);
                Dictionary<string, int> candidates = new Dictionary<string, int>();

                foreach (string wordVariation in list)
                {
                    if (_dictionary.ContainsKey(wordVariation) && !candidates.ContainsKey(wordVariation))
                        candidates.Add(wordVariation, _dictionary[wordVariation]);
                }

                if (candidates.Count > 0)
                    return candidates.OrderByDescending(x => x.Value).First().Key;

                // known_edits2()
                foreach (string item in list)
                {
                    foreach (string wordVariation in Edits(item))
                    {
                        if (_dictionary.ContainsKey(wordVariation) && !candidates.ContainsKey(wordVariation))
                            candidates.Add(wordVariation, _dictionary[wordVariation]);
                    }
                }

                return (candidates.Count > 0) ? candidates.OrderByDescending(x => x.Value).First().Key : word;
            }

            private List<string> Edits(string word)
            {
                var splits = new List<Tuple<string, string>>();
                var transposes = new List<string>();
                var deletes = new List<string>();
                var replaces = new List<string>();
                var inserts = new List<string>();

                // Splits
                for (int i = 0; i < word.Length; i++)
                {
                    var tuple = new Tuple<string, string>(word.Substring(0, i), word.Substring(i));
                    splits.Add(tuple);
                }

                // Deletes
                for (int i = 0; i < splits.Count; i++)
                {
                    string a = splits[i].Item1;
                    string b = splits[i].Item2;
                    if (!string.IsNullOrEmpty(b))
                    {
                        deletes.Add(a + b.Substring(1));
                    }
                }

                // Transposes
                for (int i = 0; i < splits.Count; i++)
                {
                    string a = splits[i].Item1;
                    string b = splits[i].Item2;
                    if (b.Length > 1)
                    {
                        transposes.Add(a + b[1] + b[0] + b.Substring(2));
                    }
                }

                // Replaces
                for (int i = 0; i < splits.Count; i++)
                {
                    string a = splits[i].Item1;
                    string b = splits[i].Item2;
                    if (!string.IsNullOrEmpty(b))
                    {
                        for (char c = 'a'; c <= 'z'; c++)
                        {
                            replaces.Add(a + c + b.Substring(1));
                        }
                    }
                }

                // Inserts
                for (int i = 0; i < splits.Count; i++)
                {
                    string a = splits[i].Item1;
                    string b = splits[i].Item2;
                    for (char c = 'a'; c <= 'z'; c++)
                    {
                        inserts.Add(a + c + b);
                    }
                }

                return deletes.Union(transposes).Union(replaces).Union(inserts).ToList();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            label4.Text = "";
            Stopwatch sw = new Stopwatch();
            listBox2.Items.Clear();
            dosyauzantisi = ".txt";
            string aranan = textBox1.Text;
            sw.Start();
            dosyadanOku(aranan, dosyauzantisi);
            sw.Stop();
            Spelling spelling = new Spelling();
            string word = "";
            word = textBox1.Text;
            if (word != spelling.Correct(word))
                linkLabel1.Text = spelling.Correct(word);
            if (listBox2.Items.Count != 0)
                label4.Text += listBox2.Items.Count + " adet sonuç " + sw.ElapsedMilliseconds.ToString() + " milisaniyede bulunmuştur";
        }
        int sayac = 0;
        public void timer1_Tick(object sender, EventArgs e)
        {
            sayac++;
        }
        string dosyauzantisi = "";
        private void button2_Click(object sender, EventArgs e)
        {
            label4.Text = "";
            Stopwatch sw = new Stopwatch();
            listBox2.Items.Clear();
            dosyauzantisi = ".html";
            string aranan = textBox1.Text;
            sw.Start();
            dosyadanOku(aranan, dosyauzantisi);
            sw.Stop();
            Spelling spelling = new Spelling();
            string word = "";
            word = textBox1.Text;
            if (word != spelling.Correct(word))
                linkLabel1.Text = spelling.Correct(word);
            if (listBox2.Items.Count != 0)
                label4.Text += listBox2.Items.Count + " adet sonuç " + sw.ElapsedMilliseconds.ToString() + " milisaniyede bulunmuştur";
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label4.Text = "";
            Stopwatch sw = new Stopwatch();
            listBox2.Items.Clear();
            string aranan = textBox1.Text;
            string text;
            dosyauzantisi = ".docx";
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            var document = new DocumentModel();
            document = DocumentModel.Load(@"C:\Users\raman\Desktop\Osman\kelime-listesi-turkce.docx");
            text = document.Sections[0].Blocks.Cast<Paragraph>(0).Inlines.Cast<Run>(0).Text;
            sw.Start();
            BruteForce(aranan, text);
            sw.Stop();
            Spelling spelling = new Spelling();
            string word = "";
            word = textBox1.Text;
            if (word != spelling.Correct(word))
                linkLabel1.Text = spelling.Correct(word);
            if (listBox2.Items.Count != 0)
                label4.Text += listBox2.Items.Count + " adet sonuç " + sw.ElapsedMilliseconds.ToString() + " milisaniyede bulunmuştur";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textBox1.Text = linkLabel1.Text;
            linkLabel1.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
