using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Vitakor_Server
{
    class DBSQLServerUtils
    {
        public static SqlConnection
                GetDBConnection(string datasource, string database)
        {

            string connString = @"Server=" + datasource + "; Database= " + database + "; Integrated Security=True;";
            SqlConnection conn = new SqlConnection(connString);
            return conn;
        }
    }
}
