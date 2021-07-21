using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using static VTSystemV2.Includes.SqlConfig;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using static VTSystemV2.App;

namespace VTSystemV2.View
{
    /// <summary>
    /// Interaction logic for RegisterStudentWindow.xaml
    /// </summary>
    public partial class RegisterStudentWindow : Window
    {
        private string _selectedFileName;
        public static bool Xadd;
        Byte[] _imagebytearray;
        private BitmapImage bitmap;
        public RegisterStudentWindow()
        {
            InitializeComponent();
        }
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                Application.Current.MainWindow, Corner.BottomRight, 10, 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                TimeSpan.FromSeconds(3),
                MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        private void BtnBrowse_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.png) | *.jpg; *.jpeg; *.jpe; *.png";
            if (dlg.ShowDialog() == true)
            {
                _selectedFileName = dlg.FileName;
                //FileNameLabel.Content = selectedFileName;
                bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_selectedFileName);
                bitmap.EndInit();
                studimg.ImageSource = bitmap;
            }
        }
        private async Task XfillComboboxes()
        {
            Sqlcmd.Parameters.Clear();
            await Conopen();
            Strsql = "Select * from tbl_course";
            Sqlcmd.CommandText = Strsql;
            Sqlcmd.Connection = Cnn;
            Sqladapter.SelectCommand = Sqlcmd;
            Sqlreader = await Sqlcmd.ExecuteReaderAsync();
            course.Items.Clear();
            while (Sqlreader.Read())
            {
                course.Items.Add(Sqlreader.GetValue(1));
            }
            Sqlcmd.Dispose();
            Sqlreader.Close();
            Cnn.Close();
            Strsql = "";
            //retrieve year level
            Sqlcmd.Parameters.Clear();
            await Conopen();
            Strsql = "Select * from tbl_yearLevel";
            Sqlcmd.CommandText = Strsql;
            Sqlcmd.Connection = Cnn;
            Sqladapter.SelectCommand = Sqlcmd;
            Sqlreader = await Sqlcmd.ExecuteReaderAsync();
            yrlevel.Items.Clear();
            while (Sqlreader.Read())
            {
                yrlevel.Items.Add(Sqlreader.GetValue(0));
            }
            Sqlcmd.Dispose();
            Sqlreader.Close();
            Cnn.Close();
            Strsql = "";
            //retrieve voter's category
            Sqlcmd.Parameters.Clear();
            await Conopen();
            Strsql = "Select * from tbl_Vtrs_Category";
            Sqlcmd.CommandText = Strsql;
            Sqlcmd.Connection = Cnn;
            Sqladapter.SelectCommand = Sqlcmd;
            Sqlreader = await Sqlcmd.ExecuteReaderAsync();
            vtCategory.Items.Clear();
            while (Sqlreader.Read())
            {
                vtCategory.Items.Add(Sqlreader.GetValue(1));
            }
            Sqlcmd.Dispose();
            Sqlreader.Close();
            Cnn.Close();
            Strsql = "";
        }

        private async void RegisterStudentWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await XfillComboboxes();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SaveData_OnClick(object sender, RoutedEventArgs e)
        {
            if (studid.Text == "")
            {
                notifier.ShowError("Please enter Student's Id #.");
                studid.Focus();
                return;
            }
            if (fname.Text == "")
            {
                notifier.ShowError("Please enter Student's First name.");
                fname.Focus();
                return;
            }
            if (lname.Text == "")
            {
                notifier.ShowError("Please enter Student's Last name.");
                course.Focus();
                return;
            }
            if (minitial.Text == "")
            {
                notifier.ShowError("Please enter Student's Middle initial.");
                minitial.Focus();
                return;
            }
            if (course.Text == "")
            {
                notifier.ShowError("Please Select Student's Course.");
                fname.Focus();
                return;
            }
            if (yrlevel.Text == "")
            {
                notifier.ShowError("Please Select Student's Year level.");
                yrlevel.Focus();
                return;
            }
            if (vtCategory.Text == "")
            {
                notifier.ShowError("Please Select Voter's category");
                vtCategory.Focus();
                return;
            }
            saveprogress.Visibility = Visibility.Visible;
            await Xaddmode();
            saveprogress.Visibility = Visibility.Collapsed;
        }
        private async Task XDisplayEdit()
        {
            byte[] img;
            //bunifuTextBox1.Text = Form1.XID;
            Sqlcmd.Parameters.Clear();
            await Conopen();
            Strsql = "Select * from tbl_Students where stud_id = '" + studid.Text + "'";
            Sqlcmd.CommandText = Strsql;
            Sqlcmd.Connection = Cnn;
            Sqladapter.SelectCommand = Sqlcmd;
            Sqlreader = Sqlcmd.ExecuteReader();
            //metroComboBox1.Items.Clear();
            if (Sqlreader.Read())
            {
                studid.Text = Sqlreader.GetValue(0).ToString();
                fname.Text = Sqlreader.GetValue(1).ToString();
                lname.Text = Sqlreader.GetValue(2).ToString();
                minitial.Text = Sqlreader.GetValue(3).ToString();
                course.Text = Sqlreader.GetValue(5).ToString();
                yrlevel.Text = Sqlreader.GetValue(6).ToString();
                img = (byte[])Sqlreader.GetValue(7);
                if (img.Length != 0)
                {
                    //studimg.ImageSource = (ImageBrush)Image.FromStream(new MemoryStream(img));
                    studimg.ImageSource = LoadImage(img);
                }

            }
            Sqlcmd.Dispose();
            Sqlreader.Close();
            Cnn.Close();
            Strsql = "";
        }
        //Convert byte image to imagesource
        private async Task Xaddmode()
        {
            try
            {
                //This still needs to edit if the students had no picture, the system must still sav the file.
                Sqlcmd.Parameters.Clear();
                Bitmap temp = new(_selectedFileName);
                MemoryStream strm = new();
                temp.Save(strm, System.Drawing.Imaging.ImageFormat.Jpeg);
                _imagebytearray = strm.ToArray();
                await Conopen();
                Strsql =
                    "Insert into tbl_Students(Stud_Id, Stud_FName, Stud_LName, Stud_MInit,Crs_Description,Yr_Level,stud_img, vtc_desc) " +
                                   "Values(@Stud_Id, @Stud_FName, @Stud_LName, @Stud_MInit, @Crs_Description, @Yr_Level,@stud_img, @vtc_desc)";
                Sqlcmd.Parameters.AddWithValue("@Stud_Id", studid.Text);
                Sqlcmd.Parameters.AddWithValue("@Stud_FName", fname.Text);
                Sqlcmd.Parameters.AddWithValue("@Stud_LName", lname.Text);
                Sqlcmd.Parameters.AddWithValue("@Stud_MInit", minitial.Text);
                Sqlcmd.Parameters.AddWithValue("@Crs_Description", course.Text);
                Sqlcmd.Parameters.AddWithValue("@Yr_Level", yrlevel.Text);
                Sqlcmd.Parameters.AddWithValue("@stud_img", _imagebytearray);
                Sqlcmd.Parameters.AddWithValue("@vtc_desc", vtCategory.Text);
                Sqlcmd.Connection = Cnn;
                Sqlcmd.CommandText = Strsql;
                await Sqlcmd.ExecuteNonQueryAsync();
                Sqlcmd.Dispose();
                Strsql = "";
                Cnn.Close();
                Close();
                notifier.ShowSuccess("Data successfully saved!");

            }
            catch
            {
                //var a = new T_Message();
                //var frmok = new FrmOk();
                //a.Show(this);
                //frmok.titletxt.Text = "Error";
                //frmok.msgtxt.Text = "The ID # you've Entered is already in the record! Please verify this problem to the registrar!";
                //frmok.OkDescription = "Error";
                //frmok.ShowDialog(this);
                //a.Hide();
                //studid.Text = "";
                //studid.Focus();
            }

        }

    }
}
