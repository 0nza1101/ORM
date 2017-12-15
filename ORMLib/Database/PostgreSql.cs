using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using ORMLib.exception;
using Npgsql;


namespace ORMLib.Database
{
    public class PostgreSql : IDatabase
    {
        private DbConnection _dbConnection;
        private string m_connectionString;


        public string connectionString
        {
            get { return m_connectionString; }
            set { m_connectionString = value; }
        }

        public DbConnection dbConnection
        {
            get
            {
                return this._dbConnection;
            }
            set
            {
                this._dbConnection = value;
            }
        }

        /// <param name="ip">The IP Address of the database. It generally is localhost</param>
        /// <param name="dbName">Name of your database</param>
        /// <param name="username">Username to connect to the database</param>
        /// <param name="password">Password to connect to the database. There is none</param>
        public PostgreSql(string ip, string dbName, string username, string password)
        {
            // Specify connection options and open an connection
            connectionString = String.Format("Server={0};User Id={2}; Password={3};Database={1};",
                                             ip, dbName,username, password);
        }

        /// <typeparam name="T">A generic object, can be int, can typeof Contacts</typeparam>
        /// <param name="req">Contains the SQL Statement</param>
        /// <returns>A list that contains the result of the SQL SELECT statement</returns>
        public List<T> Select<T>(string req)
        {
            using (dbConnection = new NpgsqlConnection())
            {
                List<T> list = new List<T>();
                T obj = default(T);

                dbConnection.ConnectionString = connectionString;
                dbConnection.Open();
                DbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandText = req;
                dbCommand.CommandType = CommandType.Text;
                dbCommand.Connection = dbConnection;
                DbDataReader reader;
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
                dbConnection.Close();
                dbConnection.Dispose(); /// !!
                return list;
            }
        }

        /// <typeparam name="T">A generic object, can be int, can typeof Contacts</typeparam>
        /// <param name="req">Contains the SQL statement</param>
        /// <param name="listOfParameters">Contains the value of the parameters in the SQL Statement</param>
        /// <returns>It either returns a list that contains the result of a SELECT Sql Statement, or it returns an empty list</returns>
        public List<T> Execute<T>(string req, List<DbParameter> listOfParameters)
        {
            List<T> list = new List<T>();
            using (dbConnection = new NpgsqlConnection())
            {
                dbConnection.ConnectionString = connectionString;
                DbCommand dbCommand = new NpgsqlCommand();
                dbCommand.CommandText = req;
                dbCommand.CommandType = CommandType.Text;
                dbCommand.Connection = dbConnection;
                foreach (DbParameter param in listOfParameters)
                {
                    DbParameter npgSqlParameter = dbCommand.CreateParameter();
                    npgSqlParameter.ParameterName = param.ParameterName;
                    npgSqlParameter.Value = param.Value;
                    dbCommand.Parameters.Add(npgSqlParameter);
                }
                dbConnection.Open();
                var i = 0;
                if (req.IndexOf("SELECT", StringComparison.Ordinal) == 0)
                {
                    DbDataReader dbDataReader;
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
                    else
                    {
                        Console.WriteLine("No rows were found.");
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
                dbConnection.Close();
                dbConnection.Dispose();
                return list;
            }
        }
    }
}

