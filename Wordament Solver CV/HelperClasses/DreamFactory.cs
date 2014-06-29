using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wordament_Solver_CV.HelperClasses;

namespace Wordament_Solver_CV.Cloud
{
   public static class DreamFactory
    {
       public static string getSession()
       {
           try
           {
               return Retry.Do(() =>
                 {
                     string uri = "https://dsp-cv.cloud.dreamfactory.com/rest/user/session?app_name=wordament";

                     string data = "{\"email\":\"wordamentcv@gmail.com\", \"password\":\"borninmay90\"}";
                     System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                     byte[] bytes = encoding.GetBytes(data);

                     WebClient wc = new WebClient();

                     wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                     dynamic stuff = JObject.Parse(wc.UploadString(uri, "PUT", data));

                     string session = stuff.session_id;
                     if (string.IsNullOrEmpty(session))
                         throw new Exception("Session Error");
                     return session;
                 }, TimeSpan.FromMilliseconds(100), 5);
           }
           catch (Exception ex)
           {
               if(!(ex is AggregateException))
                 ErrorReporting.ReportError(ex);

               return null;
           }

       }

    

       internal static string getGameSettings(string session)
       {
           if (session == null)
               return null;
           //WordamentSettings
           try
           {
               return Retry.Do(() =>
                   {
                       string uri = "https://dsp-cv.cloud.dreamfactory.com/rest/db/WordamentSettings";

                       WebClient wc = new WebClient();

                       wc.Headers["X-DreamFactory-Application-Name"] = "wordament";
                       wc.Headers["X-DreamFactory-Session-Token"] = session;
                       wc.Headers[HttpRequestHeader.ContentType] = "application/json";

                       return wc.DownloadString(uri);
                   }, TimeSpan.FromMilliseconds(100), 5);
           }
           catch (Exception ex)
           {
               ErrorReporting.ReportError(ex);

               return null;
           }
       }

       internal static void getWordFile(string p,string session)
       {

           if (session == null)
               return;
               string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".Zip");
           Retry.Do(() =>
           {

               string uri = "https://dsp-cv.cloud.dreamfactory.com/rest/files/applications/Wordament/" + p;

               WebClient wc = new WebClient();

               wc.Headers["X-DreamFactory-Application-Name"] = "wordament";
               wc.Headers["X-DreamFactory-Session-Token"] = session;
               //wc.Headers[HttpRequestHeader.ContentType] = "application/json";

               wc.DownloadFile(uri, tempPath);
           }, TimeSpan.FromMilliseconds(100), 5);

           ZipFile.ExtractToDirectory(tempPath, App.AppData);

           File.Delete(tempPath);

            
        
       }

       internal static string FetchDictionary(List<string> value,string session)
       {

           if (session == null)
               return null;

           try
           {
               return Retry.Do(() => 
               {
                   string uri = "https://dsp-cv.cloud.dreamfactory.com/rest/db/Dictionary?filter=";

                   string filter = "";
                   foreach (var item in value)
                   {
                       filter += "Word=\"" + item.ToUpper() +"\"||";
                   }

                  filter= filter.TrimEnd('|');

                   uri += Uri.EscapeDataString( filter);

                   WebClient wc = new WebClient();

                   wc.Headers["X-DreamFactory-Application-Name"] = "wordament";
                   wc.Headers["X-DreamFactory-Session-Token"] = session;
                 //  wc.Headers[HttpRequestHeader.ContentType] = "application/json";

                 return  wc.DownloadString(uri);

               }, TimeSpan.FromMilliseconds(100), 5);
           }
           catch (Exception ex)
           {
               ErrorReporting.ReportError(ex);

               return null;
           }
          
       }

       internal static void IncrementCount()
       {

           try
           {
               var session = getSession();

               if (session == null)
                   return;

               string uri = "https://dsp-cv.cloud.dreamfactory.com/rest/db/WordamentUsers?filter=";

               string filter = "UserId=\"" + App.gameSettings.UserId + "\"";
               uri += Uri.EscapeDataString(filter);

               WebClient wc = new WebClient();
               wc.Headers["X-DreamFactory-Application-Name"] = "wordament";
               wc.Headers["X-DreamFactory-Session-Token"] = session;
               //  wc.Headers[HttpRequestHeader.ContentType] = "application/json";

               var json = wc.DownloadString(uri);

               dynamic stuff = JObject.Parse(json);

               if (stuff.record.Count == 0)
               {
                   uri = "https://dsp-cv.cloud.dreamfactory.com/rest/db/WordamentUsers/Id";

                   string Content = "{\"UserId\" : \"" + App.gameSettings.UserId + "\", \"Count\":\"0\"}";

                   wc.UploadStringAsync(new Uri(uri), "POST", Content);
               }
               else
               {
                   uri = "https://dsp-cv.cloud.dreamfactory.com/rest/db/WordamentUsers/" + stuff.record[0].Id;

                   int ctr;
                   int.TryParse((string)stuff.record[0].Count, out ctr);
                   string Content = "{\"Count\" : \"" + ++ctr + "\"}";

                   wc.UploadStringAsync(new Uri(uri), "PATCH", Content);
               }
           }
           catch (Exception ex)
           {
               ErrorReporting.ReportError(ex);
           }

       }



       internal static void IncrementGameCount()
       {
           try
           {
               var session = getSession();

               if (session == null)
                   return ;
               string uri = "https://dsp-cv.cloud.dreamfactory.com/rest/db/WordamentUsers?filter=";

               string filter = "UserId=\"" + App.gameSettings.UserId + "\"";
               uri += Uri.EscapeDataString(filter);

               WebClient wc = new WebClient();
               wc.Headers["X-DreamFactory-Application-Name"] = "wordament";
               wc.Headers["X-DreamFactory-Session-Token"] = session;
               //  wc.Headers[HttpRequestHeader.ContentType] = "application/json";

               var json = wc.DownloadString(uri);

               dynamic stuff = JObject.Parse(json);
             
               if (stuff.record.Count > 0)
               {
                   uri = "https://dsp-cv.cloud.dreamfactory.com/rest/db/WordamentUsers/" + stuff.record[0].Id;

                   int ctr;
                   int.TryParse((string)stuff.record[0].GameCount, out ctr);
                   string Content = "{\"GameCount\" : \"" + ++ctr + "\"}";

                   wc.UploadStringAsync(new Uri(uri), "PATCH", Content);
               }
           }
           catch (Exception ex)
           {
               ErrorReporting.ReportError(ex);

           }
       }
    }
}
