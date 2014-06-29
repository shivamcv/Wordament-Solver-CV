using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordament_Solver_CV.HelperClasses
{
  public  class GameSettings
    {
        public int Version { get; set; }
        public string WordFile { get; set; }
        public Guid UserId { get; set; }
        public int AppVersion { get; set; }
      public GameSettings()
        {

        }
    }
}
