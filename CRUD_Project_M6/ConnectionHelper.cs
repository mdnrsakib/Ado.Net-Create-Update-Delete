using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD_Project_M6
{
    public static class ConnectionHelper
    {
        public static string ConString
        {
            get
            {
                string dataPath = Path.Combine(Path.GetFullPath(@"..\..\"), "EmployeeDb.mdf");
                return $@"Data Source=(localdb)\mssqllocaldb;AttachDbFilename={dataPath};Initial Catalog=EmployeeDb;Trusted_Connection=True";
            }
        }
    }
}
