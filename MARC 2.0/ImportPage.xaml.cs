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

        private void GetiOSAppName(string appID)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (o, args) => RetrieveAppName(appID, 1);
            bw.RunWorkerCompleted += (o, args) => RetrieveAppNameUpdateControl();
            bw.RunWorkerAsync();
        }

        private void RetrieveAppNameUpdateControl()
        {
            //MessageBox.Show(appName);
            if (null != appName && appName != "")
            {
                AddNewAppToAppList(appID, appName);
                progressBarContainer.Visibility = Visibility.Hidden;
            }
            else
            {
                progressBarContainer.Visibility = Visibility.Hidden;
                MessageBox.Show("App name could not be resolved!");
            }
            
        }

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
                    text = text.Replace("im:name", "AppName").Replace("im:image","Image");
                }

                JObject jsonObject = JObject.Parse(text);
                Apple_User_Review_Sniffer.RootObject deserializedObject = JsonConvert.DeserializeObject<Apple_User_Review_Sniffer.RootObject>(jsonObject.ToString());
                appName = deserializedObject.feed.entry[0].AppName.label;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

            //var currDir = System.IO.Directory.GetCurrentDirectory();
            using (var localAppDataFileStreamWriter= new StreamWriter(specificFolder + @"/MyAppsList.txt",true))
            {
                localAppDataFileStreamWriter.WriteLine("iOS," + appName + "," + appID);
            }
            ReadLocalAppDataFile();
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
        /// Download selected Review
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downloadReviewButton_Click(object sender, RoutedEventArgs e)
        {
            progressBarContainer.Visibility = Visibility.Visible;
            try {
                var selectedIndex = myAppsListbox.SelectedIndex;
                var appId = Model.AppList[selectedIndex].Split(',').ToList().Last();
                var numPage = 50;
                var bw = new BackgroundWorker();
                bw.DoWork += (o, args) => RetrieveUserReviews(appId, numPage);
                bw.RunWorkerCompleted += (o, args) => RetrieveUserReviewsUpdateControl();
                bw.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Please select an app before importing reviews!");
                progressBarContainer.Visibility = Visibility.Hidden;
            }
        }

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

        private void RetrieveUserReviewsUpdateControl()
        {
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

            Model.ImportedReviewsCollection.Clear();
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
                while ((line=sR.ReadLine()) != null)
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

                MessageBox.Show("File Location is invalid.");
            }
            
        }
    }

    public class AppItem
    {
        public string Name { get; set; } 
        public string Platform { get; set; }      
        public string Link { get; set; }
    }

    public class ReviewItem
    {
        public string Review { get; set; }
    }

}
