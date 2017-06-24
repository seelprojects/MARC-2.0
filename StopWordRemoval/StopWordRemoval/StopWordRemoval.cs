using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopWordRemoval
{
    public class StopWordRemoval
    {

        public string output { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="directoryName"></param>
        /// <param name="externalStopwords"></param>
        public StopWordRemoval(string sentence, string directoryName, List<string> externalStopwords = null)
        {
            string[] lines = System.IO.File.ReadAllLines(directoryName + "\\InputData\\stopwords_en.txt");
            //Apply External Stopwords if supplied
            if (externalStopwords != null)
            {
                var temp = lines.ToList();
                temp.AddRange(externalStopwords);
                lines = temp.ToArray();
            }
            
            output = string.Join(
    " ",
    sentence
        .ToLower()
        .Split(new[] { ' ', '\t', '\n', '\r' /* etc... */ })
        .Where(word => !lines.Contains(word))
);
        }
    }
}
