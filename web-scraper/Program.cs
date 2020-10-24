using CefSharp.MinimalExample.WinForms;
using GmailQuickstart;
using Google.Apis.Gmail.v1;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace web_scraper
{
    public class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/gmail-dotnet-quickstart.json
        public static string[] scopes = { GmailService.Scope.GmailModify, SheetsService.Scope.Spreadsheets };
        public static string ApplicationName = "Gmail API .NET Quickstart";

        [STAThread]
        private static int Main(string[] args)
        {
            BrowserForm browser;
            Starter.Initialize(args);
            if (args.Length == 0)
            {
                MainForm form = new MainForm();
                Application.Run(form);
            }
            else if (args.Length ==1)
            {
                browser = new BrowserForm();
                Thread thread = new Thread(() =>
                {
                    GmailService service = GmailAnnonceService.GetGmailService();
                    var gmailmessages = GmailAnnonceService.GetMessages(service);
                    Thread.Sleep(2000);
                    foreach(var message in gmailmessages)
                    {
                        WebScraperService.RunScrapping(browser, args[0].Split('/')[5], message);
                    }
                    Application.Exit();
                });
                thread.Start();
                Application.Run(browser);
            }
            else
            {
                browser = new BrowserForm();
                Thread thread = new Thread(() =>
                {
                    Thread.Sleep(2000);
                    List<Annonce> lists = new List<Annonce>();
                    if(args.Length>1)
                        lists.Add(new Annonce(DateTime.Now, args[0],long.Parse(args[1])));
                    else
                        lists.Add(new Annonce(DateTime.Now, args[0]));
                    WebScraperService.RunScrapping(browser, args[0].Split('/')[5], lists);
                    Application.Exit();
                });
                thread.Start();
                Application.Run(browser);
            }
            
            return 0;
        }
    }
}
