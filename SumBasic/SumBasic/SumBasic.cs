using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SumBasic
{
    public class SumBasic
    {
        List<string> ReviewList { get; set; }
        static int numberOfSummary = 0;
        Dictionary<string, double> WordCountsDictionary { get; set; }
        Dictionary<string, double> ReviewWeightDictionary { get; set; }

        public List<string> finalResult = new List<string>();

        /// <summary>
        /// Static Main Entry point for testing
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
            SumBasic sumBasic = new SumBasic(tempList, 2);
            sumBasic.PerformSumBasic();
        }

        /// <summary>
        /// SumBasic Constructor
        /// </summary>
        /// <param name="inputList"></param>
        /// <param name="nos"></param>
        public SumBasic(List<string> inputList, int nos)
        {
            ReviewList = inputList.Distinct().ToList();
            WordCountsDictionary = new Dictionary<string, double>();
            numberOfSummary = nos;
        }

        /// <summary>
        /// Perform SumBasic Logic
        /// </summary>
        public void PerformSumBasic()
        {
            finalResult = new List<string>();
            importWords();
            calculateWordProbability();
            for (int i = 0; i < numberOfSummary; i++)
            {
                var result = selectBestComment(WordCountsDictionary);
                finalResult.Add(result);
                updateProbability(result);
            }
        }

        /// <summary>
        /// Update Probability after a review has been selected
        /// </summary>
        /// <param name="result"></param>
        private void updateProbability(string result)
        {
            var words = result.ToLower().Split(' ').ToList();
            foreach (var word in words)
            {
                WordCountsDictionary[word] *= WordCountsDictionary[word];
            }
        }

        /// <summary>
        /// Select the Best comment by computing the probability
        /// </summary>
        /// <param name="wordCountsDictionary"></param>
        /// <returns></returns>
        private string selectBestComment(Dictionary<string, double> wordCountsDictionary)
        {
            var tempDict = new Dictionary<string, double>();
            foreach (var review in ReviewList)
            {
                try
                {
                    double score = 0;
                    var words = review.ToLower().Split(' ').ToList();

                    foreach (var word in words.Distinct().ToList())
                    {
                        score += wordCountsDictionary[word];
                    }

                    tempDict.Add(review, score / (words.Distinct().ToList().Count));
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return tempDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).First().Key;
        }

        /// <summary>
        /// Calculate probability of individual words
        /// </summary>
        private void calculateWordProbability()
        {
            var tempCollection = new Dictionary<string, double>();
            double probability;
            foreach (var item in WordCountsDictionary)
            {
                probability = item.Value / WordCountsDictionary.Sum(x => x.Value);
                tempCollection.Add(item.Key, probability);
            }
            WordCountsDictionary = tempCollection;
        }

        /// <summary>
        /// Import words to Dictionary
        /// </summary>
        private void importWords()
        {
            foreach (var review in ReviewList)
            {
                try
                {
                    var words = review.ToLower().Split(' ').ToList();


                    //words = RemoveDuplicates(words);
                    foreach (var word in words)
                    {
                        AddWordToDictionary(word);
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        /// <summary>
        /// Add words to dictionary and its count
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
