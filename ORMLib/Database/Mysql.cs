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
    public class Mysql : IDatabase
    {
        private DbConnection mdbConnection;
        private string mconnectionString;

        private MySqlConnection mysqlConnection;

        public Mysql(string ip, string port, string dbName, string username, string password)
        {
            connectionString = String.Format("SERVER={0}:{1}; DATABASE={2}; UID={3}; PASSWORD={4}",ip, port, dbName, username, password);
        }

        public Mysql(string ip, string dbName, string username, string password)
        {
            connectionString = String.Format("SERVER={0}; DATABASE={1}; UID={2}; PASSWORD={3}", ip, dbName, username, password);
        }


        public string connectionString { get { return mconnectionString; }  set { mconnectionString = value; } }
        public DbConnection dbConnection { get { return mdbConnection; } set { mdbConnection = value; } }

        public MySqlConnection MysqlConnection { get { return mysqlConnection; } set { mysqlConnection = value; } }
     

        public List<T> Execute<T>(string req, List<DbParameter> listOfParameters)
        {
            List<T> list = new List<T>();
            using (mysqlConnection = new MySqlConnection(connectionString))
            {
             //   dbConnection.ConnectionString = connectionString;
                MySqlCommand dbCommand = mysqlConnection.CreateCommand();
                dbCommand.CommandText = req;
                dbCommand.CommandType = CommandType.Text;
                dbCommand.Connection = mysqlConnection;
                foreach (DbParameter param in listOfParameters)
                {
                    DbParameter sqlParameter = dbCommand.CreateParameter();
                    sqlParameter.ParameterName = param.ParameterName;
                    sqlParameter.Value = param.Value;
                    dbCommand.Parameters.Add(sqlParameter);
                }
                mysqlConnection.Open();
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
                mysqlConnection.Close();
                mysqlConnection.Dispose();
                return list;
            }
        }

        public List<T> Select<T>(string req)
        {
            using (mysqlConnection = new MySqlConnection(connectionString))
            {
                List<T> list = new List<T>();
                T obj = default(T);

                // dbConnection.ConnectionString = connectionString;
                mysqlConnection.Open();
                MySqlCommand dbCommand = mysqlConnection.CreateCommand();
                dbCommand.CommandText = req;
                dbCommand.CommandType = CommandType.Text;
                dbCommand.Connection = mysqlConnection;
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
                // Always call Close when done reading.
                reader.Close();
                mysqlConnection.Close();
                mysqlConnection.Dispose();
                return list;
            }
        }
    }
}
