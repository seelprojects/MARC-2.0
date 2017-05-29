using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridTF
{
    public class HybridTF
    {
        List<string> ReviewList { get; set; }
        List<double> ReviewListScore { get; set; }
        Dictionary<string, int> WordCountsDictionary { get; set; }
        public Dictionary<string, double> SortedDictionary { get; set; }

        /// <summary>
        /// Static Entry point for testing purpose 
        /// </summary>
        public static void Main()
        {
            var inputFile = @"C:\Users\Nishant\Desktop\TF.txt";
            var tempList = new List<string>();
            using (var sR = new StreamReader(inputFile))
            {
                var line = "";
                while ((line = sR.ReadLine()) != null)
                {
                    tempList.Add(line);
                }
            }
            HybridTF hybridtf = new HybridTF(tempList);
            hybridtf.PerformHybridTF();
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="reviewList"></param>
        public HybridTF(List<string> reviewList)
        {
            ReviewList = reviewList;
            WordCountsDictionary = new Dictionary<string, int>();
            SortedDictionary = new Dictionary<string, double>();
        }

        /// <summary>
        /// Performs Hybrid TF operation 
        /// </summary>
        public void PerformHybridTF()
        {
            foreach (var review in ReviewList)
            {
                try
                {
                    var words = review.Split(' ').ToList();
                    words.Remove(" ");
                    words.Remove("");

                    words = RemoveDuplicates(words);
                    foreach (var word in words)
                    {
                        AddWordToDictionary(word);
                    }
                }
                catch (Exception ex)
                { }
            }
            CalculateScoreForEachReview(WordCountsDictionary.Sum(x => x.Value));
            ReorderReviewsBasedOnScore();
        }


        /// <summary>
        /// Re order the input list based on the score.
        /// </summary>
        private void ReorderReviewsBasedOnScore()
        {
            Dictionary<string, double> tempDict = new Dictionary<string, double>();
            for (int i = 0; i < ReviewList.Count; i++)
            {
                if (!tempDict.ContainsKey(ReviewList[i]))
                {
                    tempDict.Add(ReviewList[i], ReviewListScore[i]);
                }
            }
            tempDict = tempDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            SortedDictionary = tempDict;
        }


        /// <summary>
        /// Remove duplicate words from the input List
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        private List<string> RemoveDuplicates(List<string> words)
        {
            return words.Distinct().ToList();
        }


        /// <summary>
        /// Calculates Score for each review
        /// </summary>
        /// <param name="v"></param>
        private void CalculateScoreForEachReview(int v)
        {
            List<double> scores = new List<double>();
            foreach (var review in ReviewList)
            {
                double score = 0;
                try
                {
                    var words = review.Split(' ').ToList();
                    foreach (var word in words)
                    {
                        if (WordCountsDictionary.ContainsKey(word))
                        {
                            score += WordCountsDictionary[word] / (v*1.0); 
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                scores.Add(score);
            }
            ReviewListScore = scores;
        }

        /// <summary>
        /// Add the input word to dictionary and increase the value if already in the dictionary
        /// </summary>
        /// <param name="word"></param>
        private void AddWordToDictionary(string word)
        {
            if (!WordCountsDictionary.ContainsKey(word))
            {
                WordCountsDictionary.Add(word, 1);
            }
            else
            {
                WordCountsDictionary[word]++;
            }
        }
    }
}
