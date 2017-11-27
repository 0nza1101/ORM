using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMLibTest.Modele
{
    [Serializable]
    public class Contacts
    {
        public int contactid { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string email { get; set; }

        public Contacts(int contacId, string name, string address, string email)
        {
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
}
