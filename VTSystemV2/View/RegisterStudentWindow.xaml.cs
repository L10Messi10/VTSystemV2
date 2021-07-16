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
using Microsoft.Win32;

namespace VTSystemV2.View
{
    /// <summary>
    /// Interaction logic for RegisterStudentWindow.xaml
    /// </summary>
    public partial class RegisterStudentWindow : Window
    {
        private string _selectedFileName;
        public RegisterStudentWindow()
        {
            InitializeComponent();
        }

        private void BtnBrowse_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                _selectedFileName = dlg.FileName;
                //FileNameLabel.Content = selectedFileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_selectedFileName);
                bitmap.EndInit();
                studimg.ImageSource = bitmap;
            }
        }
    }
}
