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
        }


        /// <summary>
        /// Import Page Button Click Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importPageButton_Click(object sender, RoutedEventArgs e)
        {
            LeftContent.Content = importPage;
            importPageCard.Background = Brushes.OrangeRed;
            classifyPageCard.Background = Brushes.White;
            summarizePageCard.Background = Brushes.White;

            importPageArrow.Visibility = Visibility.Visible;
            classifyPageArrow.Visibility = Visibility.Hidden;
            summarizePageArrow.Visibility = Visibility.Hidden;
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
                classifyPageCard.Background = Brushes.OrangeRed;
                importPageCard.Background = Brushes.White;
                summarizePageCard.Background = Brushes.White;

                importPageArrow.Visibility = Visibility.Hidden;
                classifyPageArrow.Visibility = Visibility.Visible;
                summarizePageArrow.Visibility = Visibility.Hidden;
            }
            else
            {
                MessageBox.Show("Please import reviews before trying to classify");
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
                summarizePageCard.Background = Brushes.OrangeRed;

                importPageArrow.Visibility = Visibility.Hidden;
                classifyPageArrow.Visibility = Visibility.Hidden;
                summarizePageArrow.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Please classify reviews before trying to summarize");
            }   
        }

        private void aboutPageButton_Click(object sender, RoutedEventArgs e)
        {
            AboutPage newAppInputDialog = new AboutPage();
            newAppInputDialog.Show();
        }
    }
}
