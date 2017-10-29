using MARC2.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApplicationTest
{
    /// <summary>
    /// Interaction logic for EditStopwordsWindow.xaml
    /// </summary>
    public partial class EditStopwordsWindow : Window
    {

        public MyViewModel Model { get; set; }

        public EditStopwordsWindow(MyViewModel model)
        {
            InitializeComponent();
            Model = model;
            this.DataContext = this;
            ReadStopwordsList();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReadStopwordsList()
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 2.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            string[] lines = System.IO.File.ReadAllLines(specificFolder + "\\InputData\\stopwords_en.txt");

            Model.StopwordsList = lines.ToList();


            List<Item> items = new List<Item>();
            if (lines != null)
            {
                foreach (var stopword in lines)
                {
                    items.Add(new Item { Name = stopword });
                }
                stopwordsListbox.ItemsSource = items;
            }
        }

        /// <summary>
        /// Delete Stopword 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = stopwordsListbox.Items.GetItemAt(stopwordsListbox.SelectedIndex) as Item;
            var stopwordToRemove = item.Name;

            Model.StopwordsList.Remove(stopwordToRemove);


            //Add to file
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 2.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            using (var sW = new StreamWriter(specificFolder + "\\InputData\\stopwords_en.txt"))
            {
                foreach (var word in Model.StopwordsList)
                {
                    sW.WriteLine(word);
                }
                sW.Close();
            }

            //Update Model
            List<Item> items = new List<Item>();
            foreach (var stopword in Model.StopwordsList)
            {
                items.Add(new Item { Name = stopword });
            }
            stopwordsListbox.ItemsSource = items;

            

            toastBox.Text = "Stopword Successfully Deleted";
            toastBox.Background = Brushes.Red;
            toastBox.Visibility = Visibility.Visible;
            await Task.Delay(1000);
            toastBox.Visibility = Visibility.Hidden;

            //Clear Saved Model
            clearSavedModelButton_Click(null, null);
        }






        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void clearSavedModelButton_Click(object sender, RoutedEventArgs e)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your Specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 2.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            // Combine the base folder with your Model folder....
            string modelFolder = System.IO.Path.Combine(specificFolder, "Model");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(modelFolder))
                Directory.CreateDirectory(modelFolder);

            string SVMFolder = System.IO.Path.Combine(modelFolder, "SVM");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(SVMFolder))
                Directory.CreateDirectory(SVMFolder);

            string NBFolder = System.IO.Path.Combine(modelFolder, "NB");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(NBFolder))
                Directory.CreateDirectory(NBFolder);

            try
            {
                Array.ForEach(Directory.GetFiles(SVMFolder), File.Delete);
                Array.ForEach(Directory.GetFiles(NBFolder), File.Delete);


                //Toast to show Model deletion
                //toastBox.Text = "Models Successfully Deleted";
                //toastBox.Background = Brushes.Red;
                //toastBox.Visibility = Visibility.Visible;
                //await Task.Delay(1000);
                //toastBox.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ErrorEventArgs:" + ex.ToString());
            }

        }


        /// <summary>
        /// Add Stopword to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void addStopwordButton_Click(object sender, RoutedEventArgs e)
        {
            var newStopword = newStopwordTextbox.Text;
            if (!String.IsNullOrWhiteSpace(newStopword))
            {
                if (Model.StopwordsList.Contains(newStopword))
                {
                    MessageBox.Show("List already contains the entered word.");
                }
                else
                {
                    //Add the word to the list
                    Model.StopwordsList.Add(newStopword);

                    //Sort List
                    var sortedList = Model.StopwordsList.OrderBy(s => s);

                    //Add to file

                    var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

                    // Combine the base folder with your specific folder....
                    string specificFolder = System.IO.Path.Combine(currDir, "MARC 2.0");

                    // Check if folder exists and if not, create it
                    if (!Directory.Exists(specificFolder))
                        Directory.CreateDirectory(specificFolder);

                    using (var sW = new StreamWriter(specificFolder + "\\InputData\\stopwords_en.txt"))
                    {
                        foreach (var word in sortedList)
                        {
                            sW.WriteLine(word);
                        }
                        sW.Close();
                    }

                    //Update Model
                    Model.StopwordsList = sortedList.ToList();
                    List<Item> items = new List<Item>();

                    foreach (var stopword in Model.StopwordsList)
                    {
                        items.Add(new Item { Name = stopword });
                    }
                    stopwordsListbox.ItemsSource = items;

                    toastBox.Text = "Stopword Successfully Added";
                    toastBox.Background = Brushes.Green;
                    toastBox.Visibility = Visibility.Visible;
                    await Task.Delay(1000);
                    toastBox.Visibility = Visibility.Hidden;

                    //Clear Saved Model
                    clearSavedModelButton_Click(null, null);
                }
            }

        }
    }

    /// <summary>
    /// Class AppItem
    /// </summary>
    public class Item
    {
        public string Name { get; set; }
    }
}
