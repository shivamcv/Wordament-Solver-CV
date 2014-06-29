using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wordament_Solver_CV.Cloud;

namespace Wordament_Solver_CV.HelperClasses
{
  public static  class ErrorReporting
    {

    static  Exception previousException;
      public static void ReportError(Exception ex, string msg = "")
      {
          try
          {
              if (previousException == null)
                  previousException = ex;
              else if (previousException.Message == ex.Message)
                  return;
              else
                  previousException = ex;
                   
              var session = DreamFactory.getSession();

              string uri = "https://dsp-cv.cloud.dreamfactory.com/rest/db/WordamentCrashReport/Key";

              string filter = "UserId=\"" + App.gameSettings.UserId + "\"";
              uri += Uri.EscapeDataString(filter);

              WebClient wc = new WebClient();
              wc.Headers["X-DreamFactory-Application-Name"] = "wordament";
              wc.Headers["X-DreamFactory-Session-Token"] = session;
              //  wc.Headers[HttpRequestHeader.ContentType] = "application/json";

              string Content = "{\"User\" : \"" + App.gameSettings.UserId + "\", \"Message\":\"" + ex.Message + "\n" + msg + "\"}";

              wc.UploadStringAsync(new Uri(uri), "POST", Content);
          }catch{}
      }
    }
}
