using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageBox
{
    public partial class Form1 : Form
    {
        ImageList list = null;
        string Folder = "";

        public Form1()
        {
            InitializeComponent();

            list = new ImageList();
            list.AddRange(GetAllImagesFromServer());
            RefreshListBox();
        }

        private void RefreshListBox()
        {
            // fill the listbox with all the files 
            listBox1.Items.Clear();
            listBox1.Items.AddRange(list.ToArray());
        }

        private void GetAllImagesFromLocalFolder()
        {
            string[] images = Directory.GetFiles(Folder, "*.jpg", SearchOption.TopDirectoryOnly);

            foreach ( string s in images )
            {
                Image i = new Image();
                //i.Filename = s;

                FileInfo fi = new FileInfo(s);
                i.Filename = fi.Name;
                i.Size = fi.Length;
                i.Hash = fi.GetHashCode();

                list.Add(i);
            }
        }

        private List<Image> GetAllImagesFromServer()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("http://localhost/list.php");
                return DeserializeToList(json);
            }
        }

        //public static List<string> InvalidJsonElements;
        public List<Image> DeserializeToList(string jsonString)
        {
            //InvalidJsonElements = null;
            var array = JArray.Parse(jsonString);
            List<Image> objectsList = new List<Image>();

            foreach (JObject regel in array)
            {
                try
                {
                    Image i = new Image();
                    i.Status = SyncStatus.OnlyServer;

                    foreach (JProperty prop in regel.Properties())
                    {
                        if (prop.Name == "Filename") i.Filename = prop.Value.ToString();
                        if (prop.Name == "Hash") i.Hash = Int32.Parse(prop.Value.ToString());
                        if (prop.Name == "Size") i.Size = Int32.Parse(prop.Value.ToString());
                    }
                    objectsList.Add(i);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return objectsList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // if something is entered in the textbox, then start at that folder when opening the dialog
            if ( folderTxt.Text.Length > 0 ) folderDlg.SelectedPath = folderTxt.Text;

            // show dialog 
            var result = folderDlg.ShowDialog();
            if ( result == DialogResult.OK )
            {
                folderTxt.Text = folderDlg.SelectedPath;
                Folder = folderDlg.SelectedPath;

                GetAllImagesFromLocalFolder();
                RefreshListBox();
            }
        }
    }
}
