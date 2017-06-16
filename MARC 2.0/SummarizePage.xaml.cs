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
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text.RegularExpressions;

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
        private string previousText;
        private double HTFIDFThresholdValue = 0.7;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public SummarizePage(MyViewModel model)
        {
            InitializeComponent();
            Model = model;
            this.DataContext = this;

            HTFCheckbox.IsChecked = true;
            ThresholdTextbox.Text = "20";
            previousText = ThresholdTextbox.Text;
            HTFIDFThreshold.Text = "0.7";
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


            //threshold = ThresholdSlider.Value;
            threshold = Double.Parse(ThresholdTextbox.Text);

            HTFCheckboxCheckedState = HTFCheckbox.IsChecked ?? false;
            HTFIDFCheckboxCheckedState = HTFIDFCheckbox.IsChecked ?? false;
            SBCheckboxCheckedState = SBCheckbox.IsChecked ?? false;
            LRCheckboxCheckedState = LRCheckbox.IsChecked ?? false;


            try
            {
                var value = Double.Parse(HTFIDFThreshold.Text);
                HTFIDFThresholdValue = value;
            } catch (Exception expp)
            {
                HTFIDFThreshold.Text = "0.7";
                HTFIDFThresholdValue = 0.7;
            }


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
            if (HTFCheckboxCheckedState) { SummarizeReviews(SummarizationAlgorithm.HTF); }
            else if (HTFIDFCheckboxCheckedState) { SummarizeReviews(SummarizationAlgorithm.HTFIDF); }
            else if (SBCheckboxCheckedState) { SummarizeReviews(SummarizationAlgorithm.SumBasic); }
            else if (LRCheckboxCheckedState) { SummarizeReviews(SummarizationAlgorithm.LexRank); }
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

                    Model.BugReportSummaryList = htfBugReports.SortedDictionary.Select(m => m.Key).ToList().GetRange(0, numberOfBRReviews);

                    HybridTF.HybridTF htfUserRequirements = new HybridTF.HybridTF(Model.UserRequirementList);
                    htfUserRequirements.PerformHybridTF();
                    Model.UserRequirementsSummaryList = htfUserRequirements.SortedDictionary.Select(m => m.Key).ToList().GetRange(0, numberOfURReviews);
                    break;
                case SummarizationAlgorithm.HTFIDF:
                    HybridTFIDF.HybridTFIDF htfidfBugReports = new HybridTFIDF.HybridTFIDF(Model.BugReportList);
                    htfidfBugReports.PerformHybridTFIDF(HTFIDFThresholdValue);
                    Model.BugReportSummaryList = htfidfBugReports.FinalReviewList.GetRange(0, (htfidfBugReports.FinalReviewList.Count > numberOfBRReviews ? numberOfBRReviews : htfidfBugReports.FinalReviewList.Count));


                    HybridTFIDF.HybridTFIDF htfidfUserRequirements = new HybridTFIDF.HybridTFIDF(Model.UserRequirementList);
                    htfidfUserRequirements.PerformHybridTFIDF(HTFIDFThresholdValue);
                    //Todo:
                    Model.UserRequirementsSummaryList = htfidfUserRequirements.FinalReviewList.GetRange(0, (htfidfUserRequirements.FinalReviewList.Count > numberOfURReviews ? numberOfURReviews : htfidfUserRequirements.FinalReviewList.Count));
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
            var currDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 2.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);


            var tempPath = Directory.GetCurrentDirectory().ToString();


            var summarizationInputFile = specificFolder + "\\SummarizeTemp.txt";
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

                if ((sender as CheckBox).Name == "HTFIDFCheckbox")
                {
                    HTFIDFThreshold.IsEnabled = true;
                }
                else
                {
                    HTFIDFThreshold.IsEnabled = false;
                }

            }
        }

        ///// <summary>
        ///// Threshold value change event handler
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ThresholdValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    threshold = e.NewValue;
        //}

        /// <summary>
        /// Export Summarization Results Button Click Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportSummarizationResults_button(object sender, RoutedEventArgs e)
        {
            if (Model.BugReportSummaryList == null || Model.BugReportSummaryList.Count == 0)
            {
                MessageBox.Show("One or more list may be empty.");
            }
            else if (Model.UserRequirementsSummaryList == null || Model.UserRequirementsSummaryList.Count == 0)
            {
                MessageBox.Show("One or more list may be empty.");
            }

            try
            {
                if (Model.BugReportSummaryList.Count != 0 || Model.UserRequirementsSummaryList.Count != 0)
                {
                    var outputDialogFolder = ShowSelectOutputFolderDialog();
                    if (outputDialogFolder != null)
                    {
                        ExportSummarizationResults(outputDialogFolder);
                        Process.Start("explorer.exe", outputDialogFolder);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Export Summarization Results in User Specified Folder
        /// </summary>
        /// <param name="outputFolder"></param>
        private void ExportSummarizationResults(string outputFolder)
        {
            //Write Bug Reports to OutputFolder
            using (var brWriter = new StreamWriter(outputFolder + @"\Bug Reports Summary.txt"))
            {
                if (Model.BugReportSummaryList != null && Model.BugReportSummaryList.Count > 0)
                {
                    foreach (var item in Model.BugReportSummaryList)
                    {
                        brWriter.WriteLine(item);
                    }
                }
            }

            //Write User Requierments to OutputFolder
            using (var urWriter = new StreamWriter(outputFolder + @"\User Requirement Summary.txt"))
            {
                if (Model.UserRequirementsSummaryList != null && Model.UserRequirementsSummaryList.Count > 0)
                {
                    foreach (var item in Model.UserRequirementsSummaryList)
                    {
                        urWriter.WriteLine(item);
                    }
                }
            }
        }

        /// <summary>
        /// Show Select Output Folder Dialog
        /// </summary>
        /// <returns></returns>
        private string ShowSelectOutputFolderDialog()
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "MARC 2.0 : Select Directory To Save Classification Results";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = Directory.GetCurrentDirectory();

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = Directory.GetCurrentDirectory();
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dlg.FileName;
            }
            return null;
        }


        /// <summary>
        /// Threshold Value change handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Threshold_TextChanged(object sender, TextChangedEventArgs e)
        {
            double num = 0;
            bool success = double.TryParse(((TextBox)sender).Text, out num);
            if (success & num >= 0)
                previousText = ((TextBox)sender).Text;
            else
                ((TextBox)sender).Text = previousText;


        }


        /// <summary>
        /// Hybrid TFIDF threshold Value text change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HTFIDFThreshold_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            String newText = String.Empty;
            int count = 0;
            foreach (Char c in textBox.Text.ToCharArray())
            {
                if (Char.IsDigit(c) || Char.IsControl(c) || (c == '.' && count == 0))
                {
                    newText += c;
                    if (c == '.')
                        count += 1;
                }
            }
            textBox.Text = newText;
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;
        }


        /// <summary>
        /// Vertical Scroll Event Handler for Bug Report ListBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void instScroll_Loaded(object sender, RoutedEventArgs e)
        {
            bugReportSummaryListbox.AddHandler(MouseWheelEvent, new RoutedEventHandler(MyMouseWheelH), true);
        }


        /// <summary>
        /// Vertical Scroll Initiator for Bug Report Listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyMouseWheelH(object sender, RoutedEventArgs e)
        {
            MouseWheelEventArgs eargs = (MouseWheelEventArgs)e;
            double x = (double)eargs.Delta;
            double y = instScroll.VerticalOffset;
            instScroll.ScrollToVerticalOffset(y - x);
        }


        /// <summary>
        /// Vertical Scroll Initiator for User Requirements Listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyMouseWheelH2(object sender, RoutedEventArgs e)
        {
            MouseWheelEventArgs eargs = (MouseWheelEventArgs)e;
            double x = (double)eargs.Delta;
            double y = instScroll2.VerticalOffset;
            instScroll2.ScrollToVerticalOffset(y - x);
        }


        /// <summary>
        /// Vertical Scroll Handler for User Requirement Listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void instScroll2_Loaded(object sender, RoutedEventArgs e)
        {
            userRequirementSummaryListbox.AddHandler(MouseWheelEvent, new RoutedEventHandler(MyMouseWheelH2), true);
        }
    }
}
