using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace GrabbingToSql
{
    public class WorkingWSql
    {
        private string CurrCompId;
        private SqlConnection Connection;
        public WorkingWSql(string ConnectionStr)
        {
            Connection = new SqlConnection(ConnectionStr);

        }
    }
}
