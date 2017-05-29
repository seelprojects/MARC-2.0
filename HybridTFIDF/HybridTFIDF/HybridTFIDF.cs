using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridTFIDF
{
    /// <summary>
    /// Perform Hybrid TF-IDF
    /// </summary>
    public class HybridTFIDF
    {
        List<string> ReviewList { get; set; }
        List<double> ReviewListScore { get; set; }

        Dictionary<string, int> WordCountsDictionary { get; set; }
        Dictionary<string, int> NumberofReviewsWithWord { get; set; }
        public Dictionary<string, double> FinalSortedReviewswithScores { get; set; }

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
            HybridTFIDF hybridtfidf = new HybridTFIDF(tempList);
            hybridtfidf.PerformHybridTFIDF();
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="reviewList"></param>
        public HybridTFIDF(List<string> reviewList)
        {
            ReviewList = reviewList;
            WordCountsDictionary = new Dictionary<string, int>();
            FinalSortedReviewswithScores = new Dictionary<string, double>();
            NumberofReviewsWithWord = new Dictionary<string, int>();
        }

        /// <summary>
        /// Performs Hybrid TF operation 
        /// </summary>
        public void PerformHybridTFIDF()
        {
            foreach (var review in ReviewList)
            {
                try
                {
                    var words = review.Split(' ').ToList();
                    words.Remove(" ");
                    words.Remove("");

                    //words = RemoveDuplicates(words);
                    foreach (var word in words)
                    {
                        AddWordToDictionary(word);
                    }
                }
                catch (Exception ex)
                { }
            }
            CalculateNumberOfOccuranceOfWordsInDocument();
            CalculateScoreForEachReview(WordCountsDictionary.Sum(x => x.Value));
            ReorderReviewsBasedOnScore();
        }

        //Calculates the number of user reviews in Documents that has the words w
        private void CalculateNumberOfOccuranceOfWordsInDocument()
        {
            Dictionary<string, int> tempDict = new Dictionary<string, int>();
            foreach (var value in WordCountsDictionary)
            {
                int score = 0;
                foreach (var review in ReviewList)
                {
                    if (review.ToLower().Split(' ').Contains(value.Key.ToLower()))
                    {
                        score++;
                    }
                }
                tempDict.Add(value.Key, score);
            }
            NumberofReviewsWithWord = tempDict;
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
            FinalSortedReviewswithScores = tempDict;
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
                try
                {
                    double score = 0;
                    var words = review.Split(' ').ToList();
                    foreach (var word in words)
                    {
                        if (WordCountsDictionary.ContainsKey(word))
                        {
                            score += (WordCountsDictionary[word] / (v * 1.0)) * Math.Log((double)ReviewList.Count / (double)(NumberofReviewsWithWord[word]));
                        }
                    }
                    scores.Add(score/ (double) words.Count);
                }
                catch (Exception ex)
                {

                }         
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
