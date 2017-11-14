using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

using ORMLib.Constants;
using ORMLib.Database;

namespace ORMLib.Generics
{
    public class DboMapper
    {
        private IDatabase m_database;

        public IDatabase database
        {
            get { return m_database; }
            private set { m_database = value; }
        }

        public DboMapper(string ip, string port, string dbName, string username, string password, DatabaseType databaseType)
        {
            //Create the correct Database
            DatabaseFactory databaseFactory = new DatabaseFactory();
            this.database = databaseFactory.Create(ip, port, dbName, username, password, databaseType);
        }

        //Mapping
        public List<T> List<T>(string request)
		{
            List<T> list = new List<T>();
            //Check if request contain SELECT words at index 0
            if(request.IndexOf("SELECT", StringComparison.CurrentCulture) == 0){
                list = database.Select<T>(request);
            }
			return list;
		}


           
        public void Execute<T>(string request, List<DboParameter<T>> parameters)
		{
            
		}



        public void ExecuteInsert<T>(T obj)
        {
            
        }
	}
}
