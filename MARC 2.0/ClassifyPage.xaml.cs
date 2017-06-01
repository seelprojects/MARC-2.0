using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using PorterStemmer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using WekaClassifier;
using WekaClassifier.Enums;
using MARC2.Model;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace MARC2
{
    /// <summary>
    /// Interaction logic for Classify.xaml
    /// </summary>
    public partial class ClassifyPage : Page
    {
        public MyViewModel Model { get; set; }
        public bool changeInProgress = false;

        string exceptionMessage;

        List<string> allClassification = new List<string>();
        List<List<string>> listOfReviewsBoF;
        int currentReviewIndex = 0;
        List<string> filteredReviews;
        TextFilterType txtfilterType = TextFilterType.NoFilter;

        bool BOFCheckboxCheckedState;
        bool BOWCheckboxCheckedState;
        bool NoSWCheckboxCheckedState;
        bool STCheckboxCheckedState;

        bool NBCheckboxCheckedState;
        bool SVMCheckboxCheckedState;

        bool DTCheckboxCheckedState;
        bool CTCheckboxCheckedState;

        /// <summary>
        /// Classify Page accepts model with MyViewModel type 
        /// </summary>
        /// <param name="model"></param>
        public ClassifyPage(MyViewModel model)
        {
            InitializeComponent();

            // Initialize Model 
            Model = model;
            this.DataContext = this;

            //Initialize Checkbox checked state
            NBCheckbox.IsChecked = true;
            DTCheckbox.IsChecked = true;
            BOWCheckbox.IsChecked = true;

            PopulateViewFromModel();
        }


        /// <summary>
        /// Populate View from the data retrieved from the Model
        /// </summary>
        private void PopulateViewFromModel()
        {
            List<ReviewItem> items = new List<ReviewItem>();
            if (Model.BugReportList != null)
            {
                foreach (var item in Model.BugReportList)
                {
                    items.Add(new ReviewItem() { Review = item });
                }
                bugReportListbox.ItemsSource = items;
                noBugReportTextBlock.Visibility = items.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }

            items = new List<ReviewItem>();
            if (Model.UserRequirementList != null)
            {
                foreach (var item in Model.UserRequirementList)
                {
                    items.Add(new ReviewItem() { Review = item });
                }
                userRequirementListbox.ItemsSource = items;
                noUserRequirementTextBlock.Visibility = items.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }

            //Update Pie Chart for Bug Reports, User Requirements and Miscellaneous
            try
            {
                Model.ClassfyReviewsResultsCollection.Clear();
                Model.ClassfyReviewsResultsCollection.Add(
                    new PieSeries
                    {
                        Title = "Bug Reports",
                        Values = new ChartValues<ObservableValue> { new ObservableValue(Model.BugReportList.Count) },
                        DataLabels = true
                    });
                Model.ClassfyReviewsResultsCollection.Add(
                    new PieSeries
                    {
                        Title = "User Requirements",
                        Values = new ChartValues<ObservableValue> { new ObservableValue(Model.UserRequirementList.Count) },
                        DataLabels = true
                    });
                Model.ClassfyReviewsResultsCollection.Add(
                    new PieSeries
                    {
                        Title = "Miscellaneous",
                        Values = new ChartValues<ObservableValue> { new ObservableValue(Model.MiscellaneousList.Count) },
                        DataLabels = true
                    });
                progressBarContainer.Visibility = Visibility.Hidden;
            }
            catch (Exception)
            {

                //Todo: 
            }
        }

        /// <summary>
        /// Approach Checkbox event handler for BOW and BOF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApproachCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (!changeInProgress)
            {
                changeInProgress = true;
                BOFCheckbox.IsChecked = false;
                BOWCheckbox.IsChecked = false;
                (sender as CheckBox).IsChecked = true;
                changeInProgress = false;
            }
        }

        /// <summary>
        /// Filter Checkbox event handler for Stopwords Removal and Stemming
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            //No need to make sure only one filter is selected.
        }

        /// <summary>
        /// Training Checkbox event handler for Custom Training and Default Training
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrainingCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (!changeInProgress)
            {
                changeInProgress = true;
                CTCheckbox.IsChecked = false;
                DTCheckbox.IsChecked = false;
                (sender as CheckBox).IsChecked = true;
                changeInProgress = false;
            }
        }

        /// <summary>
        /// Classifier Checkbox event handler for NB, SVM, and RF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClassifierCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (!changeInProgress)
            {
                changeInProgress = true;
                NBCheckbox.IsChecked = false;
                SVMCheckbox.IsChecked = false;
                //RFCheckbox.IsChecked = false;
                (sender as CheckBox).IsChecked = true;
                changeInProgress = false;
            }
        }

        /// <summary>
        /// Classify Reviews Button Click Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void classifyButton_Click(object sender, RoutedEventArgs e)
        {
            progressBarContainer.Visibility = Visibility.Visible;

            var userReviews = Model.ReviewList;
            BOFCheckboxCheckedState = BOFCheckbox.IsChecked ?? false;
            BOWCheckboxCheckedState = BOWCheckbox.IsChecked ?? false;
            NoSWCheckboxCheckedState = NoSWCheckbox.IsChecked ?? false;
            STCheckboxCheckedState = STCheckbox.IsChecked ?? false;

            NBCheckboxCheckedState = NBCheckbox.IsChecked ?? false;
            SVMCheckboxCheckedState = SVMCheckbox.IsChecked ?? false;

            DTCheckboxCheckedState = DTCheckbox.IsChecked ?? false;
            CTCheckboxCheckedState = CTCheckbox.IsChecked ?? false;

            var CTFilePath = browseCustomTrainingFileTextbox.Text;
            if (userReviews.Count != 0)
            {
                var bwClassifyAllAndExport = new BackgroundWorker();
                bwClassifyAllAndExport.DoWork += (o, args)
                    => classifyAllReviews
                    (
                        userReviews,
                        CTCheckboxCheckedState ? CTFilePath : null,
                        SVMCheckboxCheckedState ? ClassifierName.SupportVectorMachine : ClassifierName.NaiveBayes
                        );
                bwClassifyAllAndExport.RunWorkerCompleted += (o, args) => classifyAllAndExportUpdateControl();
                bwClassifyAllAndExport.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Classify all and Export Update Control
        /// </summary>
        /// <param name="trainingFilePath"></param>
        private void classifyAllAndExportUpdateControl()
        {
            List<string> bugReports = new List<string>();
            List<string> userRequirements = new List<string>();
            List<string> miscellaneous = new List<string>();

            if (allClassification != null && allClassification.Count > 0)
            {
                for (int i = 0; i < allClassification.Count; i++)
                {
                    if (allClassification[i] == "Bug Report")
                    {
                        bugReports.Add(Model.ReviewList[i]);
                    }
                    else if (allClassification[i] == "Feature Request")
                    {
                        userRequirements.Add(Model.ReviewList[i]);
                    }
                    else
                    {
                        miscellaneous.Add(Model.ReviewList[i]);
                    }
                }
            }

            Model.UserRequirementList = userRequirements;
            Model.BugReportList = bugReports;
            Model.MiscellaneousList = miscellaneous;

            PopulateViewFromModel();
        }

        /// <summary>
        /// Classify all user reviews and export. Takes in a list of user reviews and training File path
        /// </summary>
        /// <param name="userReviews"></param>
        /// <param name="trainingFilePath"></param>
        private void classifyAllReviews(List<string> userReviews, string trainingFilePath, ClassifierName classifierName)
        {
            ResolveTextFilterType();


            WekaClassifier.WekaClassifier classifier;
            allClassification = new List<string>();

            if (BOFCheckboxCheckedState)
            {
                //Server Test
                FrameNetOnline.FrameNetOnline frameNetServerTest = new FrameNetOnline.FrameNetOnline("This is a test.");

                //if server test passes (returns more than 0 frames) then proceed with frame extraction.
                if (frameNetServerTest.output.Count != 0)
                {
                    listOfReviewsBoF = new List<List<string>>();
                    currentReviewIndex = 0;
                    foreach (string review in userReviews)
                    {
                        currentReviewIndex++;
                        FrameNetOnline.FrameNetOnline abc = new FrameNetOnline.FrameNetOnline(review);
                        listOfReviewsBoF.Add(abc.output);
                    }
                    try
                    {
                        classifier = new WekaClassifier.WekaClassifier(listOfReviewsBoF, trainingFilePath, Directory.GetCurrentDirectory(), classifierName, txtfilterType);

                        foreach (string data in classifier.AllClassification)
                        {
                            allClassification.Add(data);
                        }
                    }
                    catch (Exception e)
                    {
                        exceptionMessage = e.ToString();
                    }
                }
                else
                {
                    MessageBox.Show("Looks like the server is down");
                }
            }
            else if (BOWCheckboxCheckedState)
            {
                filteredReviews = new List<string>();
                foreach (string review in userReviews)
                {
                    filteredReviews.Add(FilterText(review));
                }

                try
                {
                    classifier = new WekaClassifier.WekaClassifier(filteredReviews, trainingFilePath, Directory.GetCurrentDirectory(), classifierName, txtfilterType);
                    foreach (string data in classifier.AllClassification)
                    {
                        allClassification.Add(data);
                    }
                }
                catch (Exception e)
                {
                    exceptionMessage = e.ToString();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResolveTextFilterType()
        {
            if (NoSWCheckboxCheckedState == false && STCheckboxCheckedState == false)
            {
                txtfilterType = TextFilterType.NoFilter;
            }
            else if (NoSWCheckboxCheckedState == true && STCheckboxCheckedState == true)
            {
                txtfilterType = TextFilterType.StopwordsRemovalStemming;
            }
            else if (NoSWCheckboxCheckedState == true)
            {
                txtfilterType = TextFilterType.StopwordsRemoval;
            }
            else if (STCheckboxCheckedState == true)
            {
                txtfilterType = TextFilterType.Stemming;
            }
        }


        /// <summary>
        /// Method to filter input text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string FilterText(string text)
        {
            text.Replace('.', ' ');
            if (NoSWCheckboxCheckedState)
            {
                StopWordRemoval.StopWordRemoval temp = new StopWordRemoval.StopWordRemoval(text, Directory.GetCurrentDirectory());
                text = temp.output;
            }
            if (STCheckboxCheckedState)
            {
                string[] words = text.Split(' ');
                string finalStemOutput = "";
                foreach (string word in words)
                {
                    Stemmer temp = new Stemmer();
                    temp.add(word.ToCharArray(), word.Length);
                    temp.stem();
                    var stemOutput = temp.ToString();
                    finalStemOutput += stemOutput + " ";
                }
                text = finalStemOutput;
            }
            text = RemoveSpecialCharacters(text);
            return text;
        }


        /// <summary>
        /// Remove special characters from input string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                // if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ' || c == '-')
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// Browse Custom Training File Button Click Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browseCustomTrainingFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();


            //fdlg.InitialDirectory = System.IO.Directory.GetCurrentDirectory() + "\\InputData\\TrainingDatasets";
            fdlg.Filter = "Arff Files (*.arff)|*.arff";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == true)
            {
                browseCustomTrainingFileTextbox.Text = fdlg.FileName;
            }
        }

        /// <summary>
        /// Event handler for Exporting Classification Results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportClassificationResults_button(object sender, RoutedEventArgs e)
        {
            if (Model.BugReportList == null || Model.BugReportList.Count == 0)
            {
                MessageBox.Show("One or more list may be empty.");
            }
            else if (Model.UserRequirementList == null || Model.UserRequirementList.Count == 0)
            {
                MessageBox.Show("One or more list may be empty.");
            }

            try
            {
                if (Model.BugReportList.Count != 0 || Model.UserRequirementList.Count != 0)
                {
                    var outputDialogFolder = ShowSelectOutputFolderDialog();
                    if (outputDialogFolder != null)
                    {
                        ExportClassificationResults(outputDialogFolder);
                        Process.Start("explorer.exe", outputDialogFolder);
                    }
                }
            }
            catch (Exception)
            {

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
        /// Main Method to Export Classification Results
        /// </summary>
        /// <param name="outputFolder"></param>
        private void ExportClassificationResults(string outputFolder)
        {
            //Write Bug Reports to OutputFolder
            using (var brWriter = new StreamWriter(outputFolder + @"\Bug Reports.txt"))
            {
                if (Model.BugReportList != null && Model.BugReportList.Count > 0)
                {
                    foreach (var item in Model.BugReportList)
                    {
                        brWriter.WriteLine(item);
                    }
                }
            }

            //Write User Requierments to OutputFolder
            using (var urWriter = new StreamWriter(outputFolder + @"\User Requirement.txt"))
            {
                if (Model.UserRequirementList != null && Model.UserRequirementList.Count > 0)
                {
                    foreach (var item in Model.UserRequirementList)
                    {
                        urWriter.WriteLine(item);
                    }
                }
            }

            //Write Miscellaneous to OutputFolder
            using (var otWriter = new StreamWriter(outputFolder + @"\Miscellaneous.txt"))
            {
                if (Model.MiscellaneousList != null && Model.MiscellaneousList.Count > 0)
                {
                    foreach (var item in Model.MiscellaneousList)
                    {
                        otWriter.WriteLine(item);
                    }
                }
            }
        }
    }
}
