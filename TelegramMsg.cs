using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IGService3
{
    public class TelegramMsg
    {
        protected const string apiToken = "6263858173:AAEuLmAqjvv2eHg-le7TZ2hNOUOz52QI9IQ";
        protected const string chatId = "@aicodeluca";


        public void sendTelegramMessage(string Message, string chatId= "@aicodeluca", string apiToken= "6263858173:AAEuLmAqjvv2eHg-le7TZ2hNOUOz52QI9IQ")
        {

            // String variables to store the Telegram URL and message text (url encoded).
            string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
            string messageText = Message;

            // Set the Telegram URL.
            urlString = String.Format(urlString, apiToken, chatId, messageText);

            try
            {
                // Create a web request using the formatted Telegram URL.
                WebRequest request = WebRequest.Create(urlString);

                // Get the response.  
                WebResponse response = request.GetResponse();

                // Get the status.  
                string responseStatus = ((HttpWebResponse)response).StatusDescription;

                // Check the status.
                if (responseStatus == "OK")
                {
                    // Get the stream containing content returned by the server.  
                    // The using block ensures the stream is automatically closed.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.  
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            // Read the content.  
                            string responseFromServer = reader.ReadToEnd();

                            // Check the content.
                            if (responseFromServer.Contains("\"ok\":true"))
                            {
                            }
                            else
                            {
                            }

                            // Display the content.  
                            // Console.Write(Environment.NewLine);
                            // Console.WriteLine(responseFromServer);
                        }
                    }
                }
                else
                {
                }

                // Close the response.  
                response.Close();

                //          Console.ReadKey();
            }
            catch 
            {
            }
        }

    }
}
