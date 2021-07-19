using System.Data;
using System.Data.SqlClient;

namespace OWASPTop10.Repository
{
    public class DatabaseProvider : IDatabaseProvider
    {
        private string _connectionStringName;
        private IDbConnection _connection;

        public DatabaseProvider()
        {
        }

        public IDbConnection GetConnection()
        {
            if (_connection == null || _connection.State == ConnectionState.Closed)
            {
                _connection = new SqlConnection("Server=localhost;Database=master;Trusted_Connection=True;");
            }
            return _connection;
        }

        public void SetStringConnection()
        {
            _connectionStringName = "owasp_vuln_dev";
        }
    }
}
