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
using System.Windows.Shapes;
using MARC2.Model;

namespace MARC2
{
    /// <summary>
    /// Interaction logic for NewAppDialogWindow.xaml
    /// </summary>
    public partial class NewAppDialogWindow : Window
    {
        public MyViewModel Model { get; set; }
        public NewAppDialogWindow()
        {
            InitializeComponent();
            Model = new MyViewModel();
            this.DataContext = this;

        }
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }

        public string Answer
        {
            get { return txtAnswer.Text; }
        }
    }
}
