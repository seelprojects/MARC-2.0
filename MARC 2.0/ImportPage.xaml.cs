using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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

namespace MARC2
{
    /// <summary>
    /// Interaction logic for Import.xaml
    /// </summary>
    public partial class ImportPage : Page
    {

        public MyViewModel Model { get; set; }
        List<string> userReviews = new List<string>();
        public string appName = "";
        public string appID = "";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public ImportPage(MyViewModel model)
        {
            InitializeComponent();
            Model = model;
            this.DataContext = this;
            ReadLocalAppDataFile();
        }


        /// <summary>
        /// Read Local App Data File
        /// </summary>
        private void ReadLocalAppDataFile()
        {
            var myAppsList = new List<string>();

            var currDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 2.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            //var currDir = System.IO.Directory.GetCurrentDirectory();
            using (var localAppDataFileStreamReader = new StreamReader(specificFolder + @"/MyAppsList.txt"))
            {
                string line = "";
                while ((line = localAppDataFileStreamReader.ReadLine()) != null)
                {
                    myAppsList.Add(line);
                }
            }

            Model.AppList = myAppsList;
            List<AppItem> items = new List<AppItem>();
            if (myAppsList != null)
            {
                foreach (var item in myAppsList)
                {
                    var info = item.Split(',');
                    items.Add(new AppItem { Platform = info[0], Name = info[1], Link = info[2] });
                }
                myAppsListbox.ItemsSource = items;

                MyAppsMessageTextBlock.Visibility = myAppsList.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
        }



        /// <summary>
        /// Add new application button click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewAppButton_Click(object sender, RoutedEventArgs e)
        {
            NewAppDialogWindow newAppInputDialog = new NewAppDialogWindow();
            if (newAppInputDialog.ShowDialog() == true)
            {
                appID = newAppInputDialog.Answer;
                progressBarContainer.Visibility = Visibility.Visible;
                GetiOSAppName(appID);
            }
        }


        /// <summary>
        /// Initiator of retieveing app name
        /// </summary>
        /// <param name="appID"></param>
        private void GetiOSAppName(string appID)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (o, args) => RetrieveAppName(appID, 1);
            bw.RunWorkerCompleted += (o, args) => RetrieveAppNameUpdateControl();
            bw.RunWorkerAsync();
        }


        /// <summary>
        /// Invoked after Name is Retrieved
        /// </summary>
        private void RetrieveAppNameUpdateControl()
        {
            //MessageBox.Show(appName);
            if (null != appName && appName != "")
            {
                AddNewAppToAppList(appID, appName);
                downloadReviewButton_Click(null, null);
                //progressBarContainer.Visibility = Visibility.Hidden;
            }
            else
            {
                progressBarContainer.Visibility = Visibility.Hidden;
                MessageBox.Show("App name could not be resolved!");
            }

        }


        /// <summary>
        /// Method to retrieve app name
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="v"></param>
        private void RetrieveAppName(string appID, int v)
        {
            try
            {
                string url = "https://itunes.apple.com/rss/customerreviews/page=" + v + "/id=" + appID + "/sortby=mostrecent/json";
                var request = WebRequest.Create(url);
                request.ContentType = "application/json; charset=utf-8";
                string text;
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                    text = text.Replace("im:name", "AppName").Replace("im:image", "Image");
                }

                JObject jsonObject = JObject.Parse(text);
                Apple_User_Review_Sniffer.RootObject deserializedObject = JsonConvert.DeserializeObject<Apple_User_Review_Sniffer.RootObject>(jsonObject.ToString());
                appName = deserializedObject.feed.entry[0].AppName.label;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Add the new app to app list
        /// </summary>
        /// <param name="appID"></param>
        private void AddNewAppToAppList(string appID, string appName)
        {
            var currDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 2.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            //Check if app is already added
            var tempAppIDs = new List<string>();
            using (var sR = new StreamReader(specificFolder + @"/MyAppsList.txt"))
            {
                var line = "";
                while ((line = sR.ReadLine()) != null)
                {
                    tempAppIDs.Add(line.Split(',').ToList().Last());

                }
                sR.Close();
            }

            if (tempAppIDs.Contains(appID))
            {
                progressBarContainer.Visibility = Visibility.Hidden;
                MessageBox.Show("Looks like the app already exists in the app list. Importing review directly", "Duplicate app", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                using (var localAppDataFileStreamWriter = new StreamWriter(specificFolder + @"/MyAppsList.txt", true))
                {
                    localAppDataFileStreamWriter.WriteLine("iOS," + appName + "," + appID);
                }
                ReadLocalAppDataFile();
            }
        }

        /// <summary>
        /// Browse Local File Button Click Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browseLocalFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            fdlg.Filter = "Text Files (*.txt)|*.txt";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == true)
            {
                browseLocalFileTextbox.Text = fdlg.FileName;
                importLocalReviews(browseLocalFileTextbox.Text);
            }
        }

        /// <summary>
        /// appID from first import is sent if app is imported for the first time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="appIDFromFirstImport"></param>
        private void downloadReviewButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (myAppsListbox.Items.Count > 0)
            {
                progressBarContainer.Visibility = Visibility.Visible;

                var selectedIndex = 0;
                var appId = "";
                if (null == sender)
                {
                    appId = appID;
                    selectedIndex = myAppsListbox.Items.Count - 1;
                }
                else
                {
                    if (myAppsListbox.SelectedIndex == -1)
                    {
                        myAppsListbox.SelectedIndex = 0;
                    }
                    selectedIndex = myAppsListbox.SelectedIndex;
                    appId = Model.AppList[selectedIndex].Split(',').ToList().Last();
                }

                try
                {
                    var numPage = 50;
                    var bw = new BackgroundWorker();
                    bw.DoWork += (o, args) => RetrieveUserReviews(appId, numPage);
                    bw.RunWorkerCompleted += (o, args) => RetrieveUserReviewsUpdateControl();
                    bw.RunWorkerAsync();
                    Model.CurrentSource = "Imported Reviews : " + Model.AppList[selectedIndex].Split(',').ToList()[1];
                    Model.ImportedFromLocal = false;
                    ImportedReviewsHeader.Header = Model.CurrentSource;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please select an app before importing reviews!");
                    progressBarContainer.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                MessageBox.Show("Your app list contains no apps. Please select \"Add an app\" option to import reviews or select a local text file to import reviews.","No Saved Apps", MessageBoxButton.OK, MessageBoxImage.Question);
            }            
        }

        /// <summary>
        /// Server call to retrieve reviews
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private List<string> makeServerCall(string appID, int page)
        {
            List<string> allReviews = new List<string>();

            try
            {
                string url = "https://itunes.apple.com/rss/customerreviews/page=" + page + "/id=" + appID + "/sortby=mostrecent/json";
                var request = WebRequest.Create(url);
                request.ContentType = "application/json; charset=utf-8";
                string text;
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                }
                JObject jsonObject = JObject.Parse(text);
                Apple_User_Review_Sniffer.RootObject deserializedObject = JsonConvert.DeserializeObject<Apple_User_Review_Sniffer.RootObject>(jsonObject.ToString());
                foreach (var entry in deserializedObject.feed.entry)
                {
                    if (entry != null)
                    {
                        try
                        {
                            allReviews.Add(entry.content.label);
                        }
                        catch (Exception exq)
                        {
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //error
                return null;
            }
            return allReviews;
        }


        /// <summary>
        /// Initiator for retrieving user Reviews 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="numPage"></param>
        private void RetrieveUserReviews(string appId, int numPage)
        {
            userReviews = new List<string>();
            for (int i = 1; i <= numPage; i++)
            {
                try
                {
                    userReviews.AddRange(makeServerCall(appId, i));
                }
                catch (Exception ex)
                {

                }
            }
        }

        /// <summary>
        /// Control for after Reviews are retieved
        /// </summary>
        private void RetrieveUserReviewsUpdateControl()
        {
            importedReviewsListbox.Visibility = Visibility.Visible;
            importedReviewsListbox.ItemsSource = null;
            //Remove all empty strings
            userReviews.Remove("");
            Model.ReviewList = userReviews;
            List<ReviewItem> items = new List<ReviewItem>();
            if (Model.ReviewList != null)
            {
                foreach (var item in Model.ReviewList)
                {
                    items.Add(new ReviewItem() { Review = item });
                }
                importedReviewsListbox.ItemsSource = items;
                noReviewsMessageTextBlock.Visibility = items.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            progressBarContainer.Visibility = Visibility.Hidden;

            if (Model.ImportedReviewsCollection != null && Model.ImportedReviewsCollection.Count > 0)
            {
                Model.ImportedReviewsCollection.Clear();
            }

            Model.ImportedReviewsCollection.Add(new PieSeries
            {
                Title = "Imported Reviews",
                Values = new ChartValues<ObservableValue> { new ObservableValue(Model.ReviewList.Count) },
                DataLabels = true
            });

        }


        /// <summary>
        /// Import Local Reviews Based on the input Path Supplied
        /// </summary>
        /// <param name="text"></param>
        private void importLocalReviews(string text)
        {
            List<string> importedComments = new List<string>();
            using (var sR = new StreamReader(text))
            {
                string line = "";
                while ((line = sR.ReadLine()) != null)
                {
                    importedComments.Add(line);
                }
            }

            //Remove all empty strings
            importedComments.Remove("");
            Model.ReviewList = importedComments;
            List<ReviewItem> items = new List<ReviewItem>();
            if (Model.ReviewList != null)
            {
                foreach (var item in Model.ReviewList)
                {
                    items.Add(new ReviewItem() { Review = item });
                }
                importedReviewsListbox.ItemsSource = items;
                noReviewsMessageTextBlock.Visibility = items.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }

            userReviews = importedComments;

            Model.CurrentSource = "Imported Reviews : " + browseLocalFileTextbox.Text;
            Model.ImportedFromLocal = true;
            ImportedReviewsHeader.Header = Model.CurrentSource;

            RetrieveUserReviewsUpdateControl();
        }

        /// <summary>
        /// Event handler for enter press on textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browseLocalFileTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    importLocalReviews(browseLocalFileTextbox.Text);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Could not find the specified file. Please select a valid file location.", "Error opening file",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var currDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 2.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var tempAppIDs = new List<string>();
            using (var sR = new StreamReader(specificFolder + @"/MyAppsList.txt"))
            {
                var line = "";
                while ((line = sR.ReadLine()) != null)
                {
                    tempAppIDs.Add(line);

                }
                sR.Close();
            }

            tempAppIDs.RemoveAt(myAppsListbox.SelectedIndex);
            using (var localAppDataFileStreamWriter = new StreamWriter(specificFolder + @"/MyAppsList.txt", false))
            {
                foreach (var item in tempAppIDs)
                {
                    localAppDataFileStreamWriter.WriteLine(item);
                }
            }

            ReadLocalAppDataFile();
        }


        /// <summary>
        /// Vertical Scroll Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void instScroll_Loaded(object sender, RoutedEventArgs e)
        {
            importedReviewsListbox.AddHandler(MouseWheelEvent, new RoutedEventHandler(MyMouseWheelH), true);
        }


        /// <summary>
        /// Vertical Scroll Initiator
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
    }



    /// <summary>
    /// Class AppItem
    /// </summary>
    public class AppItem
    {
        public string Name { get; set; }
        public string Platform { get; set; }
        public string Link { get; set; }
    }

    /// <summary>
    /// Class ReviewItem
    /// </summary>
    public class ReviewItem
    {
        public string Review { get; set; }
    }

}
