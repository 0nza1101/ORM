using System;
using System.Collections.Generic;
using System.Text;

using ORMLib.Constants;
using ORMLib.Database;

namespace ORMLib.Generics
{
    class DboMapper
    {
        private IDatabase m_database;

        public IDatabase database
        {
            get { return m_database; }
            private set { m_database = value; }
        }

        public DboMapper(string ip, string dbName, string username, string password, DatabaseType databaseType)
        {
            //Connect to the Database
            DatabaseFactory databaseFactory = new DatabaseFactory();
            this.database = databaseFactory.Create(ip, dbName, username, password, databaseType);
        }

		public List<T> List<T>(string request)
		{
			//Check if request contain SELECT words

			//Loop through the response array of the database
			//Use reflections to get property and fields
			//For each val in response
			//List.add

			return null;
		}
           
		public void Execute<T>(string request, List<T> parameters)
		{
            
		}

        public void ExecuteInsert<T>(T obj)
        {
            
        }
	}
}
