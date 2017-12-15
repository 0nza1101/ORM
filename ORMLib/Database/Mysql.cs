using MySql.Data.MySqlClient;
using ORMLib.exception;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ORMLib.Database
{
    public class MySql : IDatabase
    {
        private DbConnection m_dbConnection;
        private string m_connectionString;

        private MySqlConnection mySqlConnection;

        public MySql(string ip, string dbName, string username, string password)
        {
            connectionString = String.Format("SERVER={0}; DATABASE={1}; UID={2}; PASSWORD={3}", ip, dbName, username, password);
        }
        
        public string connectionString { get { return m_connectionString; }  set { m_connectionString = value; } }
        public DbConnection dbConnection { get { return m_dbConnection; } set { m_dbConnection = value; } }

        public MySqlConnection MySqlConnection { get { return mySqlConnection; } set { mySqlConnection = value; } }
     

        public List<T> Execute<T>(string req, List<DbParameter> listOfParameters)
        {
            List<T> list = new List<T>();
            using (mySqlConnection = new MySqlConnection(connectionString))
            {
                MySqlCommand dbCommand = mySqlConnection.CreateCommand();
                dbCommand.CommandText = req;
                dbCommand.CommandType = CommandType.Text;
                dbCommand.Connection = mySqlConnection;
                foreach (DbParameter param in listOfParameters)
                {
                    DbParameter sqlParameter = dbCommand.CreateParameter();
                    sqlParameter.ParameterName = param.ParameterName;
                    sqlParameter.Value = param.Value;
                    dbCommand.Parameters.Add(sqlParameter);
                }
                mySqlConnection.Open();
                var i = 0;
                if (req.IndexOf("SELECT", StringComparison.Ordinal) == 0)
                {
                    MySqlDataReader dbDataReader;
                    try
                    {
                        dbDataReader = dbCommand.ExecuteReader();
                    }
                    catch (Exception e)
                    {
                        throw new DatabaseException($"An error has occured. Reason : {e.Message}");
                    }

                    T obj = default(T);
                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            obj = Activator.CreateInstance<T>();
                            foreach (PropertyInfo prop in obj.GetType().GetProperties())
                            {
                                if (!dbDataReader[prop.Name].Equals(null))
                                {
                                    prop.SetValue(obj, dbDataReader[prop.Name]);
                                }
                            }
                            list.Add(obj);
                        }
                        dbDataReader.Close();
                    }
                }
                else if (req.IndexOf("INSERT", StringComparison.Ordinal) == 0
                    || req.IndexOf("UPDATE", StringComparison.Ordinal) == 0
                    || req.IndexOf("DELETE", StringComparison.Ordinal) == 0)
                {
                    try
                    {
                        i = dbCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw new DatabaseException($"An error has occured. Reason : {e.Message}");
                    }
                }
                else
                {
                    throw new WrongSqlRequestException("Your SQL statement is not a SELECT, INSERT, UPDATE or DELETE statement");
                }
                mySqlConnection.Close();
                mySqlConnection.Dispose();
                return list;
            }
        }

        public List<T> Select<T>(string req)
        {
            using (mySqlConnection = new MySqlConnection(connectionString))
            {
                List<T> list = new List<T>();
                T obj = default(T);

                // dbConnection.ConnectionString = connectionString;
                mySqlConnection.Open();
                MySqlCommand dbCommand = mySqlConnection.CreateCommand();
                dbCommand.CommandText = req;
                dbCommand.CommandType = CommandType.Text;
                dbCommand.Connection = mySqlConnection;
                MySqlDataReader reader;
                try
                {
                    reader = dbCommand.ExecuteReader();
                }
                catch (Exception e)
                {
                    throw new DatabaseException($"Problème lors de l'exécution de la requête SQL : {e.Message}");
                }

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        obj = Activator.CreateInstance<T>();
                        foreach (PropertyInfo prop in obj.GetType().GetProperties())
                        {
                            if (!reader[prop.Name].Equals(null))
                            {
                                prop.SetValue(obj, reader[prop.Name], null);
                            }
                        }
                        list.Add(obj);
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
                mySqlConnection.Close();
                mySqlConnection.Dispose();
                return list;
            }
        }
    }
}
