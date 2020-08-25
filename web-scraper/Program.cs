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
            var browser = Starter.Main(args);

            if (args.Length ==0)
            {
                Thread thread = new Thread(() =>
                {
                    GmailService service = GmailAnnonceService.GetGmailService();
                    var gmailmessages = GmailAnnonceService.GetMessages(service);
                    Thread.Sleep(2000);
                    foreach(var message in gmailmessages)
                    {
                        WebScraperService.RunScrapping(browser, message);
                    }
                    Application.Exit();
                });
                thread.Start();
            }
            else
            {
                Thread thread = new Thread(() =>
                {
                    Thread.Sleep(2000);
                    List<Annonce> lists = new List<Annonce>();
                    if(args.Length>1)
                        lists.Add(new Annonce(DateTime.Now, args[0],long.Parse(args[1])));
                    else
                        lists.Add(new Annonce(DateTime.Now, args[0]));
                    WebScraperService.RunScrapping(browser, lists);
                    Application.Exit();
                });
                thread.Start();
            }
                
            Application.Run(browser);

            return 0;
        }
    }
}
