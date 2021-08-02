using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Npgsql;


namespace PhotoLoadScript
{
    class DBSQLServerUtils
    {

        public static NpgsqlConnection
                 GetDBConnection()
        {
            string connString = "Server=localhost; Port=5432; User Id=postgres; Password=Mystery_2000; Database=postgres";

            NpgsqlConnection conn = new NpgsqlConnection(connString); //<ip> is an actual ip address
            
            return conn;
        }
    }
}