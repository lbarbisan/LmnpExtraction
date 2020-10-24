using CefSharp.MinimalExample.WinForms;
using GmailQuickstart;
using Google.Apis.Gmail.v1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace web_scraper
{
    public partial class MainForm : Form
    {
        private BrowserForm instructionbrowser;
        private BrowserForm browser;
        private string destFileName = Path.Combine(Path.GetDirectoryName(typeof(MainForm).Assembly.Location), "credentials", "global.json");

        public MainForm()
        {
            InitializeComponent();
            label2.AllowDrop = true;
            if(File.Exists(destFileName))
            {
                label2.BackColor = Color.DarkGreen;
                label2.Text = "Ok";
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            instructionbrowser = new BrowserForm();
            instructionbrowser.LoadUrl("https://github.com/lbarbisan/LmnpExtraction/blob/master/README.md#create-the-new-google-spreadsheet-to-have-stats");
            instructionbrowser.WindowState = FormWindowState.Normal;
            instructionbrowser.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var url = textBox1.Text;
            browser = new BrowserForm();
            browser.Show();
            Thread thread = new Thread(() =>
            {
                GmailService service = GmailAnnonceService.GetGmailService();
                var gmailmessages = GmailAnnonceService.GetMessages(service);
                Thread.Sleep(2000);
                foreach (var message in gmailmessages)
                {
                    WebScraperService.RunScrapping(browser, url.Split('/')[5], message);
                }
            });
            thread.Start();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void label2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                label2.BackColor = Color.DarkGreen;
            }
        }

        private void label2_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var file = files.FirstOrDefault();

            DirectoryInfo di = Directory.CreateDirectory(destFileName.Replace("global.json",""));
            File.Copy(file,
                destFileName, true);
            label2.Text = "Ok";
        }

        private void label2_DragLeave(object sender, EventArgs e)
        {
            label2.BackColor = MainForm.DefaultBackColor;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
