using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Vitakor_Server
{
    class DBUtils
    {
        public static SqlConnection GetDBConnection()
        {
            string datasource = @"(LocalDB)/MSSQLLocalDB";
            string database = "Informations";
            return DBSQLServerUtils.GetDBConnection(datasource, database);
        }
    }
}
