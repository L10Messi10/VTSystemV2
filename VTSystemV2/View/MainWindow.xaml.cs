using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using VTSystemV2.Models;
using static VTSystemV2.App;
using static VTSystemV2.Includes.SqlConfig;
using static VTSystemV2.View.RegisterStudentWindow;

namespace VTSystemV2.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Students> studentsList = new();
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
            studentsList.Clear();
            ListStudents.ItemsSource = null;
            Strsql = "Select TOP 50 * from V_Students order by stud_id";
            Sqlcmd.CommandText = Strsql;
            Sqlcmd.Connection = Cnn;
            Sqladapter.SelectCommand = Sqlcmd;
            Sqlreader = await Sqlcmd.ExecuteReaderAsync();
            while (Sqlreader.Read())
            {
                studentsList.Add(new Students
                {
                    Stud_Id = Sqlreader["Stud_Id"].ToString(),
                    Full_Name = Sqlreader["Full_Name"].ToString(),
                    Course_Year = Sqlreader["Course_Year"].ToString(),
                    stud_img = LoadImage((byte[]) Sqlreader["stud_img"])
                });
            }
            Sqlcmd.Dispose();
            Sqlreader.Close();
            Cnn.Close();
            ListStudents.ItemsSource = studentsList;
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
            Xadd = true;
            registerStudent.ShowDialog();
            Opacity = 1;
        }

        private void EditStudent_OnClick(object sender, RoutedEventArgs e)
        {
            //Can't select a student using the popupbox only
            //var stud = ListStudents.SelectedItem as Students;
            //if (stud != null) SelectedStudId = stud.Stud_Id;
        }

        private void EditStudent_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var stud = ListStudents.SelectedItem as Students;
            if (stud != null)
            {
                SelectedStudId = stud.Stud_Id;
                Xadd = false;
                RegisterStudentWindow editStudentWindow = new RegisterStudentWindow();
                Opacity = 0.4;
                editStudentWindow.ShowDialog();
                Opacity = 1;
            }
        }

        private void ListStudents_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //dynamic
            var stud = ListStudents.SelectedItem as Students;
            //if (stud != null) SelectedStudId = stud.Stud_Id;
        }
    }
}
