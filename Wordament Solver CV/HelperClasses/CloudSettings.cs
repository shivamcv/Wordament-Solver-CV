using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wordament_Solver_CV.Cloud;
using Wordament_Solver_CV.HelperClasses;

namespace Wordament_Solver_CV
{
   public class CloudSettings
    {
        public bool IsAvailable { get; set; }

        public string FileName { get; set; }
        public int Version { get; set; }
        public int AppVersion { get; set; }
       public CloudSettings()
        {
            try
            {
                string json = getCloudSettings();
                if (string.IsNullOrEmpty(json))
                {
                    IsAvailable = false;
                    return;
                }

                dynamic stuff = JObject.Parse(json);
                FileName = stuff.record[0].FileName;
                Version = stuff.record[0].Version;
                AppVersion = stuff.record[0].AppVersion;
                IsAvailable = true;
            }
            catch (Exception ex)
            {
                ErrorReporting.ReportError(ex);
                IsAvailable = false;
            }
        }

       private string getCloudSettings()
       {
           try
           {
               string session = DreamFactory.getSession();

               string json = DreamFactory.getGameSettings(session);

               return json;
           }
           catch (Exception ex)
           {
               ErrorReporting.ReportError(ex);
               return null;
           }
           
       }
    }
}
