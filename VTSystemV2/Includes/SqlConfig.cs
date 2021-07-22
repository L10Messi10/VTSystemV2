using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSystemV2.Includes
{
    static class SqlConfig
    {
        public static SqlDataReader Sqlreader;
        public static readonly SqlDataAdapter Sqladapter = new SqlDataAdapter();
        public static readonly SqlCommand Sqlcmd = new SqlCommand();
        public static SqlConnection Cnn = new SqlConnection();
        public static string Strsql;
        public static string Ipaddress, Uid, Upass;
        public static bool Sucs = false;

        public static async Task Conopen()
        {
            //try
            //{
            //ipaddress = Properties.Settings.Default.ipadd;
            //uid = Properties.Settings.Default.uid;
            //upass = Properties.Settings.Default.pass;
            string Cstring = null;
            Cstring = "Data Source = 192.168.102.59;Network Library=DBMSSOCN;Initial Catalog=DB_VotingSystem;User ID=sa;Password=administrator01;";
            Cnn = new SqlConnection(Cstring);
            await Cnn.OpenAsync();
            //}
            //catch
            //{
            //    //Form frm = new Form();
            //    //MetroFramework.MetroMessageBox.Show(frm, "Connection Failed! Please Ensure that all Server Settings are fully set-up!", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    // MessageBox.Show("", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

        }
    }
}
