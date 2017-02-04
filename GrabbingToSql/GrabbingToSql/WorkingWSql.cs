using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace GrabbingToSql
{
    public class WorkingWSql
    {
        private string CurrCompId;
        private SqlConnection Connection;
        public WorkingWSql(string userid, string password, string Server, string Database)
        {
            SqlConnectionStringBuilder builder =
            new SqlConnectionStringBuilder();
            builder["user id"] = userid;
            builder["password"] = password;
            builder["Server"] = Server;
            builder["Database"] = Database;
            builder["Connect Timeout"] = 1000;

            string ConnectionStr = builder.ConnectionString;

            Connection = new SqlConnection(ConnectionStr);
            string QueryCreateDatabase = $"CREATE DATABASE IF NOT EXISTS '{Database}'";
            string QueryCreateTable1 = @"CREATE TABLE IF NOT EXISTS 'OverView' (
            'id' INT, 
            'RegisteredOfficeAddress' VARCHAR(255),
            PRIMARY KEY ('id')";

            SqlCommand Command = new SqlCommand(QueryCreateTable1, Connection);
            Command.Connection.Open();
            MessageBox.Show(Command.ExecuteScalar().ToString());
        }
    }
}
