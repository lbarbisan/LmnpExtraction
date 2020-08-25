using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace web_scraper
{
    public class GmailAnnonceService
    {
        public static IEnumerable<IList<Annonce>> GetMessages(GmailService service)
        {
            var list = service.Users.Messages.List("me");
            list.Q = "from:seloger@al.alerteimmo.com is:unread";
            // List labels.
            IList<Message> labels = list.Execute().Messages;
            if (labels != null && labels.Count > 0)
            {
                foreach (var labelItem in labels)
                {
                    var messageRequest = service.Users.Messages.Get("me", labelItem.Id);
                    var emailInfoResponse = messageRequest.Execute();
                    var tuple = new SimpleGmailMessage();
                    if (emailInfoResponse != null)
                    {
                        // Loop through the headers and get the fields we need...
                        foreach (var mParts in emailInfoResponse.Payload.Headers)
                        {
                            if (mParts.Name == "Date")
                            {
                                tuple.Date = DateTime.Parse(mParts.Value);
                            }

                        }
                        var part = emailInfoResponse.Payload.Parts[0];
                        String codedBody = part.Body.Data.Replace("-", "+");
                        codedBody = codedBody.Replace("_", "/");
                        byte[] data = Convert.FromBase64String(codedBody);
                        tuple.Message = Encoding.UTF8.GetString(data);

                        List<Annonce> annonces = new List<Annonce>();
                        var result = tuple.Message.Split(new[] { '\r', '\n' });
                        foreach (var line in result)
                        {
                            if (line.Contains("Voir l'annonce"))
                            {
                                annonces.Add(new Annonce(tuple.Date, line.Replace(" Voir l'annonce", "").Trim()));
                            }
                        }

                        yield return annonces;

                        ModifyMessageRequest request = new ModifyMessageRequest();
                        request.RemoveLabelIds = new List<string>();
                        request.RemoveLabelIds.Add("UNREAD");
                        var requeste = service.Users.Messages.Trash("me", labelItem.Id);
                        requeste.Execute();
                    }

                }
            }
        }

        public static GmailService GetGmailService()
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

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Program.ApplicationName,
            });
            return service;
        }
    }
}
