using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Wordament_Solver_CV.HelperClasses;

namespace Wordament_Solver_CV
{
  public static class XmlHelper
    {
        public static T readXml<T>(string fileName)
        {
          return  Retry.Do(() =>
            {
                T tempList;
                XmlSerializer deserializer = new XmlSerializer(typeof(T));
                TextReader textReader = new StreamReader(fileName);
                tempList = (T)deserializer.Deserialize(textReader);
                textReader.Close();

                return tempList;
            }, TimeSpan.FromMilliseconds(100), 5);
        }

        public static void writeXml<T>(T tempList, string fileName)
        {
            Retry.Do(() =>
                {

                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    TextWriter textWriter = new StreamWriter(fileName);
                    serializer.Serialize(textWriter, tempList);
                    textWriter.Close();
                }, TimeSpan.FromMilliseconds(100), 5);
        }
    }
}
