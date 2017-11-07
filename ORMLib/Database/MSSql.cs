using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;

using ORMLib.Constants;
      
namespace ORMLib.Database
{
    class MSSql : IDatabase
    {
        private SqlConnection conn;
		private string m_connectionString;

        public string connectionString
		{
            get { return m_connectionString; }
            set { m_connectionString = value; }
		}

        public MSSql(string ip, string dbName, string username, string password)
        {
            connectionString = String.Format("Network Library=dbmssocn;Data Source={0},80;Initial Catalog={1};User ID={2};Password={3};",
                                             ip, dbName, username, password);
        }

        public void Select(string req)
        {
			using (conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
				conn.Open();
                //"SELECT * FROM TableName WHERE FirstColumn 
				SqlCommand command = new SqlCommand(req, conn);

				using (SqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
                    {
                        Console.WriteLine(reader);
					}
				}
			} 
        }
    }
}
