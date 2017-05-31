using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MARC2.Model;
using WpfApplicationTest.Enums;
using System.Diagnostics;
using System.IO;

namespace MARC2
{

    /// <summary>
    /// Interaction logic for SummarizePage.xaml
    /// </summary>
    public partial class SummarizePage : Page
    {
        public MyViewModel Model { get; set; }
        public bool changeInProgress = false;

        bool HTFCheckboxCheckedState = false;
        bool HTFIDFCheckboxCheckedState = false;
        bool SBCheckboxCheckedState = false;
        bool LRCheckboxCheckedState = false;

        double threshold = 10;

        public SummarizePage(MyViewModel model)
        {
            InitializeComponent();
            Model = model;
            this.DataContext = this;

            HTFCheckbox.IsChecked = true;

            PopulateViewFromModel();
        }

        /// <summary>
        /// Summarize Button Click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void summarizeButton_Click(object sender, RoutedEventArgs e)
        {
            //Show Loading Bar
            progressBarContainer.Visibility = Visibility.Visible;

            //Retrieve the reviews
            var bugReports = Model.BugReportList;
            var userRequirements = Model.UserRequirementList;

            threshold = ThresholdSlider.Value;

            HTFCheckboxCheckedState = HTFCheckbox.IsChecked ?? false;
            HTFIDFCheckboxCheckedState = HTFIDFCheckbox.IsChecked ?? false;
            SBCheckboxCheckedState = SBCheckbox.IsChecked ?? false;
            LRCheckboxCheckedState = LRCheckbox.IsChecked ?? false;



            var slowTask = Task.Factory.StartNew(() => SummarizeReviewThread());

            await slowTask;
            PopulateViewFromModel();

            //Hide Loading Bar
            progressBarContainer.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Summarize Review Entry Point for Thread
        /// </summary>
        private void SummarizeReviewThread()
        {
            if (HTFCheckboxCheckedState) {  SummarizeReviews(SummarizationAlgorithm.HTF); }
            else if (HTFIDFCheckboxCheckedState) {  SummarizeReviews(SummarizationAlgorithm.HTFIDF); }
            else if (SBCheckboxCheckedState) {  SummarizeReviews(SummarizationAlgorithm.SumBasic); }
            else if (LRCheckboxCheckedState) {  SummarizeReviews(SummarizationAlgorithm.LexRank); }
            else { MessageBox.Show("Something went wrong!"); }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SumAlgo"></param>
        private void SummarizeReviews(SummarizationAlgorithm SumAlgo)
        {
            int numberOfBRReviews = (Model.BugReportList.Count <= Convert.ToInt32(threshold)) ? Model.BugReportList.Count : Convert.ToInt32(threshold);
            int numberOfURReviews = (Model.UserRequirementList.Count <= Convert.ToInt32(threshold)) ? Model.UserRequirementList.Count : Convert.ToInt32(threshold);
            switch (SumAlgo)
            {
                case SummarizationAlgorithm.HTF:
                    HybridTF.HybridTF htfBugReports = new HybridTF.HybridTF(Model.BugReportList);
                    htfBugReports.PerformHybridTF();



                    Model.BugReportSummaryList = htfBugReports.SortedDictionary.Select(m => m.Key).ToList().GetRange(0,numberOfBRReviews);

                    HybridTF.HybridTF htfUserRequirements = new HybridTF.HybridTF(Model.UserRequirementList);
                    htfUserRequirements.PerformHybridTF();
                    Model.UserRequirementsSummaryList = htfUserRequirements.SortedDictionary.Select(m => m.Key).ToList().GetRange(0, numberOfURReviews);
                    break;
                case SummarizationAlgorithm.HTFIDF:
                    HybridTFIDF.HybridTFIDF htfidfBugReports = new HybridTFIDF.HybridTFIDF(Model.BugReportList);
                    htfidfBugReports.PerformHybridTFIDF();
                    Model.BugReportSummaryList = htfidfBugReports.FinalReviewList.GetRange(0, numberOfBRReviews);
                    

                    HybridTFIDF.HybridTFIDF htfidfUserRequirements = new HybridTFIDF.HybridTFIDF(Model.UserRequirementList);
                    htfidfUserRequirements.PerformHybridTFIDF();
                    Model.UserRequirementsSummaryList = htfidfUserRequirements.FinalReviewList.GetRange(0, numberOfURReviews);
                    break;
                case SummarizationAlgorithm.SumBasic:
                    SumBasic.SumBasic SBBugReports = new SumBasic.SumBasic(Model.BugReportList, Model.BugReportList.Count);
                    SBBugReports.PerformSumBasic();
                    Model.BugReportSummaryList = SBBugReports.finalResult.GetRange(0, numberOfBRReviews);

                    SumBasic.SumBasic SBUserRequirements = new SumBasic.SumBasic(Model.UserRequirementList, Model.UserRequirementList.Count);
                    SBUserRequirements.PerformSumBasic();
                    Model.UserRequirementsSummaryList = SBUserRequirements.finalResult.GetRange(0, numberOfURReviews);
                    break;
                case SummarizationAlgorithm.LexRank:
                    PerformLexRank(Model.BugReportList, Classifications.BugReport);
                    PerformLexRank(Model.UserRequirementList, Classifications.UserRequirements);
                    break;
                default:
                    break;
            }

        }


        /// <summary>
        /// Main Logic for Lexrank Algorithm
        /// </summary>
        /// <param name="reviews"></param>
        /// <param name="classification"></param>
        private void PerformLexRank(List<string> reviews, Classifications classification)
        {
            var tempPath = Directory.GetCurrentDirectory().ToString();
            var summarizationInputFile = tempPath + "\\SummarizeTemp.txt";
            //Copy reviews to a temp file for LexRank to Read it
            using (var sW = new StreamWriter(summarizationInputFile))
            {
                foreach (var item in reviews)
                {
                    sW.WriteLine(item);
                }
            }

            int numberOfReviews = (reviews.Count <= Convert.ToInt32(threshold)) ? reviews.Count : Convert.ToInt32(threshold);

            //Run LexRank on the file
            Process myProcess = new Process();
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "java";
            var argument = "-jar \"" + tempPath + "\\SummarizerExecutable.jar\" \"" + summarizationInputFile + "\" LEXRANK " + numberOfReviews;
            myProcess.StartInfo.Arguments = argument;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.RedirectStandardOutput = true;
            myProcess.Start();
            string output = myProcess.StandardOutput.ReadToEnd();
            myProcess.WaitForExit();

            //Retrieve data back
            ParseOutput(output, classification);

        }


        /// <summary>
        /// Parse the output generated from LexRank Java code
        /// </summary>
        /// <param name="input"></param>
        /// <param name="classification"></param>
        private void ParseOutput(string input, Classifications classification)
        {
            List<string> list = new List<string>(
                           input.Split(new string[] { "\r\n" },
                           StringSplitOptions.RemoveEmptyEntries));

            List<string> newList = new List<string>();
            bool enabled = false;
            int i = 0;
            foreach (var item in list)
            {        
                if (item.Contains("*** SUMMARY"))
                {
                    enabled = true;  
                }
                if (enabled)
                {
                    i++;
                    newList.Add(i == 1 ? item.Substring(24) : item.Substring(3));
                }
            }

            //Check Classification and Add to Model 
            if (classification == Classifications.BugReport)
            {
                Model.BugReportSummaryList = newList;
            }
            else if (classification == Classifications.UserRequirements)
            {
                Model.UserRequirementsSummaryList = newList;
            }

        }


        /// <summary>
        /// Populate View from the Model Data
        /// </summary>
        private void PopulateViewFromModel()
        {
            List<ReviewItem> items = new List<ReviewItem>();
            if (Model.BugReportSummaryList != null)
            {
                foreach (var item in Model.BugReportSummaryList)
                {
                    items.Add(new ReviewItem() { Review = item });
                }
                bugReportSummaryListbox.ItemsSource = items;
                noBugReportSummaryTextBlock.Visibility = items.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }

            items = new List<ReviewItem>();
            if (Model.UserRequirementsSummaryList != null)
            {
                foreach (var item in Model.UserRequirementsSummaryList)
                {
                    items.Add(new ReviewItem() { Review = item });
                }
                userRequirementSummaryListbox.ItemsSource = items;
                noUserRequirementSummaryTextBlock.Visibility = items.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        /// <summary>
        /// Click Handler for CheckedState of Summarization Algorithms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SummarizerCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (!changeInProgress)
            {
                changeInProgress = true;
                HTFCheckbox.IsChecked = false;
                HTFIDFCheckbox.IsChecked = false;
                SBCheckbox.IsChecked = false;
                LRCheckbox.IsChecked = false;
                (sender as CheckBox).IsChecked = true;
                changeInProgress = false;
            }
        }

        /// <summary>
        /// Threshold value change event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThresholdValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            threshold = e.NewValue;
        }
    }
}
