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

namespace VTSystemV2.View
{
    /// <summary>
    /// Interaction logic for RegisterStudentWindow.xaml
    /// </summary>
    public partial class RegisterStudentWindow : Window
    {
        private string _selectedFileName;
        Byte[] _imagebytearray;
        private BitmapImage bitmap;
        public RegisterStudentWindow()
        {
            InitializeComponent();
        }
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.BottomRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        private void BtnBrowse_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
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
                notifier.ShowError("Please enter Student's Id #");
                studid.Focus();
                return;
            }
            if (fname.Text == "")
            {
                notifier.ShowError("Please enter Student's First name");
                fname.Focus();
                return;
            }
            if (lname.Text == "")
            {
                notifier.ShowError("Please enter Student's Last name");
                lname.Focus();
                return;
            }
            if (minitial.Text == "")
            {
                notifier.ShowError("Please enter Student's Middle initial");
                fname.Focus();
                return;
            }
            saveprogress.Visibility = Visibility.Visible;
            await Xaddmode();
            saveprogress.Visibility = Visibility.Collapsed;
        }
        private async Task Xaddmode()
        {
            //try
            //{
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
                //sqlcmd.CommandType = CommandType.Text;
                Sqlcmd.CommandText = Strsql;
                await Sqlcmd.ExecuteNonQueryAsync();
                //var a = new T_Message();
                //var frmok = new FrmOk();
                //a.Show(this);
                //frmok.titletxt.Text = "Success";
                //frmok.msgtxt.Text = "Record has been successfully Added! Please refresh the student data to see modified changes.";
                //frmok.OkDescription = "Success";
                //frmok.ShowDialog(this);
                //a.Hide();
                Sqlcmd.Dispose();
                Strsql = "";
                Cnn.Close();
                Close();

            //}
            //catch
            //{
            //    //var a = new T_Message();
            //    //var frmok = new FrmOk();
            //    //a.Show(this);
            //    //frmok.titletxt.Text = "Error";
            //    //frmok.msgtxt.Text = "The ID # you've Entered is already in the record! Please verify this problem to the registrar!";
            //    //frmok.OkDescription = "Error";
            //    //frmok.ShowDialog(this);
            //    //a.Hide();
            //    //studid.Text = "";
            //    //studid.Focus();
            //}

        }

    }
}
