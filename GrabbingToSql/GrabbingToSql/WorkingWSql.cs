using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;

namespace GrabbingToSql
{
    public class WorkingWSql
    {
        private string CurrCompId;
        private MySqlConnection Connection;
        public WorkingWSql(string userid, string password, string Server, string Database)
        {
            Parser.ConfigLoader cLoader = new Parser.ConfigLoader();
            Dictionary<string, string> tDicOverView = cLoader.LoadFields(Parser.PageTab.Overview);
            string[] FiNamesOverView = tDicOverView.Values.ToArray();

            Dictionary<string, string> tDicPeople = cLoader.LoadFields(Parser.PageTab.People);
            string[] FiNamesPeople = tDicPeople.Values.ToArray();

            Dictionary<string, string> tDicFillingHistory = cLoader.LoadFields(Parser.PageTab.FilingHistory);
            string[] FiNamesFillingHistory = tDicFillingHistory.Values.ToArray();

            if (InitializeDB(userid, password, Server, Database))
                if (InitializeTables(FiNamesOverView, "OverView"))
                    if (InitializeTables(FiNamesPeople, "People"))
                        if (!InitializeTables(FiNamesFillingHistory, "FillingHistory"))
                            return;
                        else;
                    else return;
                else return;
            else return;
            
        }

        public bool UpdateTable(ref DataSet DataSetToUpdate)
        {
            DataTable table1 = DataSetToUpdate.Tables[0];
            return false;
        }

        private bool InitializeTables(string[] FiNames, string TableName)
        {
            string QueryCreateTable1 = $@"CREATE TABLE IF NOT EXISTS {TableName} (
id INT NOT NULL,
idComp INT NOT NULL,
";
            foreach (string FiName in FiNames)
            {
                QueryCreateTable1 += $"{FiName} VARCHAR(255),\r\n";
            }
            QueryCreateTable1 += @"PRIMARY KEY (id), INDEX idCompInd (idComp))";
            try
            {
                if (!OpenConnection()) return false;
                MySqlCommand Command = new MySqlCommand(QueryCreateTable1, Connection);
                Command.ExecuteNonQuery();
                if (!CloseConnection()) return false;

                return true;
            }
            catch
            {
                MessageBox.Show("Error initializing tables");
                return false;
            }
        }

        private bool InitializeDB(string userid, string password, string Server, string Database)
        {
            string QueryCreateDatabase = $"CREATE DATABASE IF NOT EXISTS {Database}";
            
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
