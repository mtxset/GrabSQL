using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace GrabbingToSql
{
    public class WorkingWSql
    {
        private string CurrCompId;
        private MySqlConnection Connection;
        public WorkingWSql(string userid, string password, string Server, string Database)
        {
            InitializeDB(userid, password, Server, Database);
        }

        private bool InitializeDB(string userid, string password, string Server, string Database)
        {
            string QueryCreateDatabase = $"CREATE DATABASE IF NOT EXISTS {Database}";
            string QueryCreateTable1 = @"CREATE TABLE IF NOT EXISTS OverView (
                id INT NOT NULL, 
                RegisteredOfficeAddress VARCHAR(255),
                PRIMARY KEY (id))";
            try
            {
                MySqlConnectionStringBuilder builder =
                new MySqlConnectionStringBuilder();
                builder["user id"] = userid;
                builder["password"] = password;
                builder["Server"] = Server;
                builder["Database"] = "sys";
                builder["Connect Timeout"] = 2;

                string ConnectionStr = builder.ConnectionString;
                
                Connection = new MySqlConnection(ConnectionStr);

                MySqlCommand Command = new MySqlCommand(QueryCreateDatabase, Connection);
                if (!OpenConnection()) return false;
                Command.ExecuteNonQuery();
                if (!CloseConnection()) return false;

                builder["Database"] = Database;
                ConnectionStr = builder.ConnectionString;
                Connection = new MySqlConnection(ConnectionStr);

                if (!OpenConnection()) return false;                
                Command = new MySqlCommand(QueryCreateTable1, Connection);                
                Command.ExecuteNonQuery();
                if (!CloseConnection()) return false;

                return true;
            }
            catch
            {
                MessageBox.Show("Error initializing database");
                return false;
            }
        }

        private bool OpenConnection()
        {
            try
            {
                Connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }
        private bool CloseConnection()
        {
            try
            {
                Connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
