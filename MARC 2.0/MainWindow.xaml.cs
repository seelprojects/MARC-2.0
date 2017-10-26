using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using MARC2.Enums;
using MARC2.Extensions;
using MARC2.Model;
using System.Diagnostics;
using WpfApplicationTest;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfApplicationTest.Enums;
using System.Text.RegularExpressions;

namespace MARC2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ImportPage importPage;
        ClassifyPage classifyPage;

        SummarizePage summarizePage;
        private Brush themeColor = (Brush)(new BrushConverter().ConvertFrom("#607D8B"));

        public MyViewModel Model { get; set; }


        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Model = new MyViewModel();
            this.DataContext = this;
            importPage = new ImportPage(Model);
            LeftContent.Content = importPage;
            this.Title = "Mobile Application Review Classifier : Home";
            importPageCard.Background = themeColor;
            homeLabel.Foreground = Brushes.White;
        }


        /// <summary>
        /// Import Page Button Click Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importPageButton_Click(object sender, RoutedEventArgs e)
        {
            LeftContent.Content = importPage;
            importPageCard.Background = themeColor;
            classifyPageCard.Background = Brushes.White;
            summarizePageCard.Background = Brushes.White;
            aboutPageCard.Background = Brushes.White;
            settingsPageCard.Background = Brushes.White;
            this.Title = "Mobile Application Review Classifier : Home";
            exportButtonLabel.Content = "  Save Imported Reviews";

            homeLabel.Foreground = Brushes.White;
            classifyLabel.Foreground = Brushes.Black;
            summarizeLabel.Foreground = Brushes.Black;
            aboutLabel.Foreground = Brushes.Black;
            settingsLabel.Foreground = Brushes.Black;

            ChartImportReviews.Visibility = Visibility.Visible;
            ChartClassifyReviews.Visibility = Visibility.Collapsed;
            exportButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Classify Page Button Click Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void classifyPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.ReviewList != null && Model.ReviewList.Count > 0)
            {
                classifyPage = new ClassifyPage(Model);
                LeftContent.Content = classifyPage;
                classifyPageCard.Background = themeColor;
                importPageCard.Background = Brushes.White;
                summarizePageCard.Background = Brushes.White;
                aboutPageCard.Background = Brushes.White;
                settingsPageCard.Background = Brushes.White;

                this.Title = "Mobile Application Review Classifier : Classify";
                exportButtonLabel.Content = " Save Classification Results";
                homeLabel.Foreground = Brushes.Black;
                classifyLabel.Foreground = Brushes.White;
                summarizeLabel.Foreground = Brushes.Black;
                aboutLabel.Foreground = Brushes.Black;
                settingsLabel.Foreground = Brushes.Black;

                ChartImportReviews.Visibility = Visibility.Collapsed;
                ChartClassifyReviews.Visibility = Visibility.Visible;
                exportButton.Visibility = Visibility.Visible;

            }
            else
            {
                MessageBox.Show("Looks like you haven't imported any reviews yet. Please import reviews on the left panel before trying to classify!", "Hold on!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        /// <summary>
        /// Summarize Page Button Click Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void summarizePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.UserRequirementList != null || Model.BugReportList != null)
            {
                summarizePage = new SummarizePage(Model);
                LeftContent.Content = summarizePage;
                classifyPageCard.Background = Brushes.White;
                importPageCard.Background = Brushes.White;
                summarizePageCard.Background = themeColor;
                aboutPageCard.Background = Brushes.White;
                settingsPageCard.Background = Brushes.White;


                ChartImportReviews.Visibility = Visibility.Collapsed;
                ChartClassifyReviews.Visibility = Visibility.Collapsed;
                exportButton.Visibility = Visibility.Visible;

                this.Title = "Mobile Application Review Classifier : Summarize";
                exportButtonLabel.Content = " Save Summarization Results";
                homeLabel.Foreground = Brushes.Black;
                classifyLabel.Foreground = Brushes.Black;
                summarizeLabel.Foreground = Brushes.White;
                aboutLabel.Foreground = Brushes.Black;
                settingsLabel.Foreground = Brushes.Black;
            }
            else
            {
                MessageBox.Show("Looks like you haven't classified any reviews yet. Please import and classify reviews before trying to summarize!", "Hold on!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        /// <summary>
        /// About Button Click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutPageButton_Click(object sender, RoutedEventArgs e)
        {
            //AboutPage newAppInputDialog = new AboutPage();
            //newAppInputDialog.Show();
            classifyPageCard.Background = Brushes.White;
            importPageCard.Background = Brushes.White;
            summarizePageCard.Background = Brushes.White;
            aboutPageCard.Background = themeColor;
            settingsPageCard.Background = Brushes.White;

            this.Title = "Mobile Application Review Classifier : About";
            homeLabel.Foreground = Brushes.Black;
            classifyLabel.Foreground = Brushes.Black;
            summarizeLabel.Foreground = Brushes.Black;
            settingsLabel.Foreground = Brushes.Black;
            aboutLabel.Foreground = Brushes.White;
            ChartImportReviews.Visibility = Visibility.Collapsed;
            ChartClassifyReviews.Visibility = Visibility.Collapsed;
            exportButton.Visibility = Visibility.Collapsed;


            NewAboutPage newAboutPage = new NewAboutPage();
            LeftContent.Content = newAboutPage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsPageButton_Click(object sender, RoutedEventArgs e)
        {
            //settingsPageCard.Background = themeColor;
            //classifyPageCard.Background = Brushes.White;
            //summarizePageCard.Background = Brushes.White;
            //aboutPageCard.Background = Brushes.White;
            //importPageCard.Background = Brushes.White;

            //this.Title = "Mobile Application Review Classifier : Settings";
            //homeLabel.Foreground = Brushes.Black;
            //classifyLabel.Foreground = Brushes.Black;
            //summarizeLabel.Foreground = Brushes.Black;
            //aboutLabel.Foreground = Brushes.Black;
            //settingsLabel.Foreground = Brushes.White;


            EditStopwordsWindow editStopwordsWindowDialog = new EditStopwordsWindow(this.Model);
            if (editStopwordsWindowDialog.ShowDialog() == true)
            {
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportResultsButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChartImportReviews.Visibility == Visibility.Visible)
            {
                ExportImportedReviews();
            }
            else if (ChartClassifyReviews.Visibility == Visibility.Visible)
            {
                ExportClassificationResultsReview();
            }
            else
            {
                ExportSummarizationResultsReview();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ExportSummarizationResultsReview()
        {
            if (Model.BugReportSummaryList == null || Model.BugReportSummaryList.Count == 0 && Model.UserRequirementsSummaryList == null || Model.UserRequirementsSummaryList.Count == 0)
            {
                MessageBox.Show("Looks like there are no reviews to export. Please make sure at least one of the categories has reviews to export.", "No Export Data", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (Model.BugReportSummaryList == null || Model.BugReportSummaryList.Count == 0)
            {
                MessageBox.Show("Looks like one of the lists is empty.", "Partial Export Data", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (Model.UserRequirementsSummaryList == null || Model.UserRequirementsSummaryList.Count == 0)
            {
                MessageBox.Show("Looks like one of the lists is empty.", "Partial Export Data", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            try
            {
                if (Model.BugReportSummaryList.Count != 0 || Model.UserRequirementsSummaryList.Count != 0)
                {
                    var outputDialogFolder = ShowSelectOutputFolderDialog(Actions.Summarize);
                    if (outputDialogFolder != null)
                    {

                        string fileName = Model.ImportedFromLocal ? "Local" : Model.CurrentSource.Replace("Imported Reviews : ", "");
                        fileName = Regex.Replace(fileName, "[^0-9A-Za-z]+", " ");

                        ExportSummarizationResults(outputDialogFolder, fileName);
                        Process.Start("explorer.exe", outputDialogFolder);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="outputFolder"></param>
       /// <param name="fileName"></param>
        private void ExportSummarizationResults(string outputFolder, string fileName)
        {
            //Write Bug Reports to OutputFolder
            using (var brWriter = new StreamWriter(outputFolder + String.Format(@"\{0} Bug Report Summary.txt", fileName)))
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
            using (var urWriter = new StreamWriter(outputFolder + String.Format(@"\{0} User Requirements Summary.txt", fileName)))
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
        /// 
        /// </summary>
        private void ExportClassificationResultsReview()
        {
            if (Model.BugReportList == null || Model.BugReportList.Count == 0 && Model.UserRequirementList == null || Model.UserRequirementList.Count == 0)
            {
                MessageBox.Show("Looks like there are no reviews to export. Please make sure at least one of the categories has reviews to export.", "No Export Data", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (Model.BugReportList == null || Model.BugReportList.Count == 0)
            {
                MessageBox.Show("Looks like one of the lists is empty.", "Partial Export Data", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (Model.UserRequirementList == null || Model.UserRequirementList.Count == 0)
            {
                MessageBox.Show("Looks like one of the lists is empty.", "Partial Export Data", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            try
            {
                if (Model.BugReportList.Count > 0 || Model.UserRequirementList.Count > 0)
                {
                    var outputDialogFolder = ShowSelectOutputFolderDialog(Actions.Classify);
                    if (outputDialogFolder != null)
                    {
                        string fileName = Model.ImportedFromLocal ? "Imported Reviews Local" : Model.CurrentSource.Replace("Imported Reviews : ", "");
                        fileName = Regex.Replace(fileName, "[^0-9A-Za-z]+", " ");

                        ExportClassificationResults(outputDialogFolder, fileName);
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
        private string ShowSelectOutputFolderDialog(Actions action)
        {
            var dlg = new CommonOpenFileDialog();

            switch (action)
            {
                case Actions.Import:
                    dlg.Title = "MARC 2.0 : Select Directory To Save Imported Results";
                    break;
                case Actions.Classify:
                    dlg.Title = "MARC 2.0 : Select Directory To Save Classification Results";
                    break;
                case Actions.Summarize:
                    dlg.Title = "MARC 2.0 : Select Directory To Save Summarization Results";
                    break;
                default:
                    break;
            }

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
        /// 
        /// </summary>
        /// <param name="outputFolder"></param>
        /// <param name="fileName"></param>
        private void ExportClassificationResults(string outputFolder, string fileName)
        {
            //Write Bug Reports to OutputFolder
            using (var brWriter = new StreamWriter(outputFolder + String.Format(@"\{0} Bug Reports.txt", fileName)))
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
            using (var urWriter = new StreamWriter(outputFolder + String.Format(@"\{0} User Requirements.txt", fileName)))
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
            using (var otWriter = new StreamWriter(outputFolder + String.Format(@"\{0} Miscellaneous.txt", fileName)))
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


        /// <summary>
        /// 
        /// </summary>
        private void ExportImportedReviews()
        {
            if (Model.ReviewList == null || Model.ReviewList.Count == 0)
            {
                MessageBox.Show("Looks like there are no reviews to export. Please import reviews before trying to export them to a local file.", "No Export Data!", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            try
            {
                if (Model.ReviewList.Count != 0)
                {
                    var outputDialogFolder = ShowSelectOutputFolderDialog(Actions.Import);
                    if (outputDialogFolder != null)
                    {
                        string fileName = Model.ImportedFromLocal ? "Imported Reviews Local" : Model.CurrentSource.Replace("Imported Reviews : ", "");
                        fileName = Regex.Replace(fileName, "[^0-9A-Za-z]+", " ");
                        ExportImportedResults(outputDialogFolder, fileName);
                        Process.Start("explorer.exe", outputDialogFolder);
                    }
                }
            }
            catch (Exception)
            {

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputFolder"></param>
        /// <param name="fileName"></param>
        private void ExportImportedResults(string outputFolder, string fileName)
        {
            try
            {
                //Write imports to OutputFolder
                using (var irWriter = new StreamWriter(outputFolder + String.Format(@"\{0}.txt", fileName)))
                {
                    if (Model.ReviewList != null && Model.ReviewList.Count > 0)
                    {
                        foreach (var item in Model.ReviewList)
                        {
                            irWriter.WriteLine(item);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something went wrong while writing output. You may not have permission to write on the selected folder. If problem persists contact the author.");
            }
        }

        
    }
}
