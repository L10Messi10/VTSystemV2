using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VTSystemV2.Models;
using static VTSystemV2.Includes.SqlConfig;

namespace VTSystemV2.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //var students = GetStudents();
            //if (students.Count > 0)
            //    ListStudents.ItemsSource = students;
        }

        protected override async void OnActivated(EventArgs e)
        {
            await FillStudents();
        }

        private void BtnExpand_OnClick(object sender, RoutedEventArgs e)
        {
            BtnExpand.Visibility = Visibility.Collapsed;
            BtnClose.Visibility = Visibility.Visible;

        }
        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            BtnExpand.Visibility = Visibility.Visible;
            BtnClose.Visibility = Visibility.Collapsed;
            
        }

        private async Task FillStudents()
        {
            studprogress.Visibility = Visibility.Visible;
            await Conopen();
            Sqlcmd.Parameters.Clear();
            Strsql = "Select TOP 50 * from V_Students order by stud_id";
            Sqlcmd.CommandText = Strsql;
            Sqlcmd.Connection = Cnn;
            Sqladapter.SelectCommand = Sqlcmd;
            Sqlreader = await Sqlcmd.ExecuteReaderAsync();
            DataTable dt = new DataTable();
            dt.Load(Sqlreader);
            ListStudents.ItemsSource = dt.DefaultView;
            Sqlcmd.Dispose();
            Sqlreader.Close();
            Cnn.Close();
            Strsql = "";
            studprogress.Visibility = Visibility.Collapsed;
        }

        private void Btndashboard_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            paneldashboard.Visibility = Visibility.Visible;
            panelstud.Visibility = Visibility.Collapsed;
        }

        private void Btnstudent_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            paneldashboard.Visibility = Visibility.Collapsed;
            panelstud.Visibility = Visibility.Visible;
        }

        private void Dbrd_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            paneldashboard.Visibility = Visibility.Visible;
            panelstud.Visibility = Visibility.Collapsed;
        }

        private void Std_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            paneldashboard.Visibility = Visibility.Collapsed;
            panelstud.Visibility = Visibility.Visible;
        }

        private async void Studrefresh_OnClick(object sender, RoutedEventArgs e)
        {
            await FillStudents();
        }

        private void Regstudent_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegisterStudentWindow registerStudent = new RegisterStudentWindow();
            Opacity = 0.4;
            registerStudent.ShowDialog();
            Opacity = 1;
        }
    }
}
