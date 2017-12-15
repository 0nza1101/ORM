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

        /// <summary>
        /// Constructor of Mysql Objet
        /// </summary>
        /// <param name="ip">adress IP of database</param>
        /// <param name="dbName">name of database</param>
        /// <param name="username">username to connect database</param>
        /// <param name="password">password to connect database</param>
        public Mysql(string ip, string dbName, string username, string password)
        {
            connectionString = String.Format("SERVER={0}; DATABASE={1}; UID={2}; PASSWORD={3}", ip, dbName, username, password);
        }


        public string connectionString { get { return mconnectionString; }  set { mconnectionString = value; } }
        public DbConnection dbConnection { get { return mdbConnection; } set { mdbConnection = value; } }

        public MySqlConnection MysqlConnection { get { return mysqlConnection; } set { mysqlConnection = value; } }
     
        /// <summary>
        /// The execute a CRUD (select, insert, updaten delete) query
        /// </summary>
        /// <typeparam name="T">generic objet</typeparam>
        /// <param name="req">sql query</param>
        /// <param name="listOfParameters">contains a result sql query</param>
        /// <returns>return list of result</returns>
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
        
        /// <summary>
        /// execute of select query
        /// </summary>
        /// <typeparam name="T">generic objet</typeparam>
        /// <param name="req">sql query</param>
        /// <returns>return list of result</returns>
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
