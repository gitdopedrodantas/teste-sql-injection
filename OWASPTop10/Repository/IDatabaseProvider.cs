using System.Data;

namespace OWASPTop10.Repository
{
        public interface IDatabaseProvider
        {
            IDbConnection GetConnection();
            void SetStringConnection();
        }
}