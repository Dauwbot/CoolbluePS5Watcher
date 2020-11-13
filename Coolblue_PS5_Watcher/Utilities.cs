﻿using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Coolblue_PS5_Watcher
{
    public class Utilities
    {
        public string HtmlContentStream(string urlAddress)
        {
            string data = null;
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(urlAddress);
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.193 Safari/537.36";
            request.Headers["cookie"] = "Coolblue-Session=10316bdae7e1793985a7553e4b313437; Secure-Coolblue=0dbf3e47077716eae77af95ee19c4713; PHPSESSID=qh139u3ibs9ckr410uisuerjbn; wishListRequested=true; seenLanguageEmphasis=1; locale=fr_BE; assignedVariations=egDtDbt4N36YYsF9XkYW1qGeWFEe61EitS9UDsTm9F1MP3fTxY5VJ7ViUUy3UkLUY1WSn0UFzYivhdhgNDI21LVTWRtSxrooQWAEioaafRPa6e9gxlPJVQ6YTBC7nms97U17DnTXuS6i8aMpSLdBwV2mjQCEQnbmoquk3ayFsP6QykCmO7XURaHyet6MbyWZPtMPb2cAIGKeY1G3SHqRiq8fdDrYpTSFowWtqLFZtYx2HuRAvNxSpNXD19GZ8wVR0JOoJ4i5; _dd_s=rum=0&expire=1605202357056";
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader reader = null;

                if (string.IsNullOrWhiteSpace(response.CharacterSet))
                {
                    reader = new StreamReader(receiveStream);
                }
                else
                {
                    reader = new StreamReader(receiveStream,Encoding.GetEncoding(response.CharacterSet));
                }

                data = reader.ReadToEnd();
                response.Close();
                reader.Close();
                
            }
            return data;
        }

        public void HtmlToFile(string htmlData, string filePath)
        {
            StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8);
            writer.Write(htmlData);
        }
        
        internal bool IsFileLocked(FileInfo file)
        {
            try
            {
                using(FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }

        public int CheckStringForSubstring(string completeString, string substringToFind)
        {
            return completeString.IndexOf(substringToFind);
        }
    }
}