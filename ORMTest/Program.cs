using ORMLib.Constants;
using ORMLib.Generics;
using System;
using System.Collections.Generic;

namespace ORMTest
{
    public class Contacts
    {
        public int contactid { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string email { get; set; }

        public Contacts(int contacId, string name, string address, string email){
            this.contactid = contacId;
            this.name = name;
            this.address = address;
            this.email = email;
        }

        public Contacts()
        {
            this.contactid = 0;
            this.name = "";
            this.address = "";
            this.email = "";
        }
    }

    class Program
    {
        static string team = "ME";
        static void Main(string[] args)
        {
            Console.WriteLine($"Test cases for ORMLib made by { team }");

            DboMapper database = new DboMapper("localhost", "1433", "devdb", "sa", "P@55w0rd", DatabaseType.MSSql);

            //Simple mapping takes only SELECT
            string selectReq = "SELECT * FROM Contacts";
            List<Contacts> l = new List<Contacts>();
            l = database.List<Contacts>(selectReq);

            foreach(var item in l){
                Console.WriteLine(item.contactid);
                Console.WriteLine(item.name);
                Console.WriteLine(item.address);
                Console.WriteLine(item.email);
            }
            Console.ReadKey();
            //Params query takes SELECT INSERT UPDATE DELETE
            //string insertReq = @"INSERT INTO contact (nom, prenom) VALUES (@nom, @prenom)";
            /*List<string> p = new List<string>();
            p.Add(new DboParameter("@nom", obj.Nom));
            p.Add(new DboParameter("@prenom", obj.Prenom));
            database.Execute(req, p);*/

        }
    }
}