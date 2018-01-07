using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skud;
using System.Data.SqlClient;

namespace SkudTests
{
    [TestFixture]
    public class Test
    {
        [TestCase]
        public void TestConnection()
        {
            SqlConnectionStringBuilder sqlConnection = new SqlConnectionStringBuilder();
            sqlConnection.DataSource = "dfgdfgdf";
            sqlConnection.InitialCatalog = "Journal";
            sqlConnection.IntegratedSecurity = true;
            bool result = Main.IsServerConnected(sqlConnection.ConnectionString);
            Assert.AreEqual(true, result);
        }
    }
}
