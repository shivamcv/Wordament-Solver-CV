using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wordament_Solver_CV.Cloud;

namespace Wordament_Solver_CV.ViewModel
{
 public  class DictionaryService
    {
     public List<string> RequestList {
         set 
         {
             CollectData(value);

         } }

     private void CollectData(List<string> value)
     {
         Thread t = new Thread(new ThreadStart(() =>
         {
             for (int i = 0; i < value.Count; i=i+5)
             {
                 List<string> Batch = new List<string>();
                 for (int j = i; j < i+5; j++)
                 {

                     if (j < value.Count)
                     {
                         if (Dictionary.ContainsKey(value[j].ToUpper()))
                             continue;
                         Batch.Add(value[j]);
                     }
                 }
               string json =  DreamFactory.FetchDictionary(Batch,DreamFactory.getSession());
               if (!string.IsNullOrEmpty(json))
               {
                   dynamic stuff = JObject.Parse(json);
                   for (int x = 0; x < stuff.record.Count; x++)
                   {
                       if (!Dictionary.ContainsKey((string)(stuff.record[x].Word)))
                       Dictionary.Add((string)(stuff.record[x].Word),(string) (stuff.record[x].Meaning));
                   }
               }
             }

         }));

        
         t.Start();
     }

        public Dictionary<string,string> Dictionary { get; set; }
      
        
        public DictionaryService()
        {
            Dictionary = new Dictionary<string, string>();
        }

       public string GetMeaning(string word)
        {
            if (Dictionary.ContainsKey(word.ToUpper()))
            {
                return Dictionary[word.ToUpper()];
            }
            else
                return null;
        }

    }

 public class cloudDic
 {
     public List<record> record { get; set; }

     public cloudDic() { record = new List<record>(); }
 }
 public class record
 {
     public string Word { get; set; }
     public string Meaning { get; set; }

     public record() { }

     public record(string word, string meaning)
     {
         Word = word;
         Meaning = meaning;
     }
 }
}
