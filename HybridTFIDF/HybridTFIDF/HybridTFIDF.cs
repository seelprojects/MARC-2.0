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
        public List<string> FinalReviewList { get; set; }

        Dictionary<string, int> WordCountsDictionary { get; set; }
        Dictionary<string, int> NumberofReviewsWithWord { get; set; }
        public Dictionary<string, double> SortedReviewswithScoresBeforeCosineSim { get; set; }
        
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

            //Cosine Similarity Test Code
            //int[] vecA = { 2, 0, 1, 1, 0, 2, 1, 1 };
            //int[] vecB = { 2, 1, 1, 0, 1, 1, 1, 1 };

            //var cosSimilarity = CalculateCosineSimilarity(vecA, vecB);

            //Console.WriteLine(cosSimilarity);
            //Console.Read();
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="reviewList"></param>
        public HybridTFIDF(List<string> reviewList)
        {
            ReviewList = reviewList;
            WordCountsDictionary = new Dictionary<string, int>();
            SortedReviewswithScoresBeforeCosineSim = new Dictionary<string, double>();
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
            ApplyCosineSimilarityToRemoveSimilarConsecutiveReviews(0.7);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        private void ApplyCosineSimilarityToRemoveSimilarConsecutiveReviews(double v)
        {
            FinalReviewList = new List<string>();
            List<string> tempReviewList = new List<string>();
            foreach (var item in SortedReviewswithScoresBeforeCosineSim)
            {
                tempReviewList.Add(item.Key);
            }

            var review1 = tempReviewList[0];
            FinalReviewList.Add(review1);
            for (int i = 1; i < tempReviewList.Count; i++)
            {      
                var review2 = tempReviewList[i];
                if ((CosineSimilarity(review1, review2)) < v)
                {
                    FinalReviewList.Add(review2);
                    review1 = review2;
                }
            }

        }

        /// <summary>
        /// Perform Cosine Similarity between two reviews
        /// </summary>
        /// <param name="review1"></param>
        /// <param name="review2"></param>
        /// <returns></returns>
        private double CosineSimilarity(string review1, string review2)
        {
            List<string> uniqueWordsBetweenReview1and2 = new List<string>();
            var wordsFromReview1 = review1.Split(' ').ToList();
            var wordsFromReview2 = review2.Split(' ').ToList();

            uniqueWordsBetweenReview1and2.AddRange(wordsFromReview1);
            uniqueWordsBetweenReview1and2.AddRange(wordsFromReview2);
            uniqueWordsBetweenReview1and2 = RemoveDuplicates(uniqueWordsBetweenReview1and2);

            //Construct Vector A
            List<int> A = new List<int>();
            foreach (var item in uniqueWordsBetweenReview1and2)
            {
                A.Add(wordsFromReview1.Where(s => s == item).Count());
            }

            //Construct Vector B
            List<int> B = new List<int>();
            foreach (var item in uniqueWordsBetweenReview1and2)
            {
                B.Add(wordsFromReview2.Where(s => s == item).Count());
            }

            return CalculateCosineSimilarity(A.ToArray(), B.ToArray());
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
            //First Combine review and scores as key and pair to form a dictionary
            Dictionary<string, double> tempDict = new Dictionary<string, double>();

            for (int i = 0; i < ReviewList.Count; i++)
            {
                if (!tempDict.ContainsKey(ReviewList[i]))
                {
                    tempDict.Add(ReviewList[i], ReviewListScore[i]);
                }
            }
            //Reorder list according to score
            tempDict = tempDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            SortedReviewswithScoresBeforeCosineSim = tempDict;
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

        #region CosineSimilarity

        /// <summary>
        /// Calculates cosine similarity between two vectors
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="vecB"></param>
        /// <returns></returns>
        private static double CalculateCosineSimilarity(int[] vecA, int[] vecB)
        {
            var dotProduct = DotProduct(vecA, vecB);
            var magnitudeOfA = Magnitude(vecA);
            var magnitudeOfB = Magnitude(vecB);

            return dotProduct / (magnitudeOfA * magnitudeOfB);
        }
        

        /// <summary>
        /// Performs Dot Product- Part of Cosine Similarity
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="vecB"></param>
        /// <returns></returns>
        private static double DotProduct(int[] vecA, int[] vecB)
        {
            // I'm not validating inputs here for simplicity.            
            double dotProduct = 0;
            for (var i = 0; i < vecA.Length; i++)
            {
                dotProduct += (vecA[i] * vecB[i]);
            }

            return dotProduct;
        }

        // Magnitude of the vector is the square root of the dot product of the vector with itself.
        private static double Magnitude(int[] vector)
        {
            return Math.Sqrt(DotProduct(vector, vector));
        }
        #endregion CosineSimilarity
    }
}
