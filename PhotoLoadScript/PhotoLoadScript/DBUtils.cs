using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Npgsql;


namespace PhotoLoadScript
{
    class DBUtils
    {
        public static NpgsqlConnection GetDBConnection()
        {
            
            return DBSQLServerUtils.GetDBConnection();
        }
    }

}