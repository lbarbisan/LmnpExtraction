using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace web_scraper.Services
{
    public class SheetService
    {
        public static void Save(params object[] items)
        {
            var spreadsheet = "1sCdc-O1N8Ve8wD1syqiYDu4XWwpB_QT3rMkRObnoYKc";
            var range = "RowData!R1:R";

            var sheetsdata = new List<IList<object>>();
            sheetsdata.Add(items);

            var request = GetSheetsService().Spreadsheets.Values.Append(new ValueRange() { Values = sheetsdata }, spreadsheet, range);
            request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            var response = request.Execute();
        }

        public static SheetsService GetSheetsService()
        {

            UserCredential credential;

            using (var stream =
                new FileStream("credentials/global.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Program.scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Program.ApplicationName,
            });

            return service;
        }

    }
}
