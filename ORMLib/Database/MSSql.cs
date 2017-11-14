using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Data;
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

        public MSSql(string ip, string port, string dbName, string username, string password)
        {
            connectionString = String.Format("Network Library=dbmssocn;Data Source={0},{1};Initial Catalog={2};User ID={3};Password={4};",
                                             ip, port, dbName, username, password);
        }

        public List<T> Select<T>(string req)
        {
			using (conn = new SqlConnection())
            {
                List<T> list = new List<T>();
                T obj = default(T);

                conn.ConnectionString = connectionString;
				conn.Open();
				SqlCommand command = new SqlCommand(req, conn);
                SqlDataReader reader;
                try {
                    reader = command.ExecuteReader();
                } catch (Exception e) {
                    throw new ArgumentException("Problème lors de l'éxécution de la requête SQL");
                }

                if (reader.HasRows){
                    while (reader.Read())
                    {
                        obj = Activator.CreateInstance<T>();
                        foreach (PropertyInfo prop in obj.GetType().GetProperties())
                        {
                            if (!object.Equals(reader[prop.Name], DBNull.Value))
                            {
                                prop.SetValue(obj, reader[prop.Name], null);
                            }
                        }
                        list.Add(obj);
                    }
                    return list;
                }
                else{
                    Console.WriteLine("No rows found.");
                }
                // Always call Close when done reading.
                reader.Close();
                return null;
			}
        }
    }
}
