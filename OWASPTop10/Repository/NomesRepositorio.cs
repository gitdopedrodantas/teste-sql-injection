using System;
using System.Threading.Tasks;

namespace OWASPTop10.Repository
{
    public class NomesRepositorio : BaseSqlRepository
    {
        public NomesRepositorio(IDatabaseProvider databaseProvider) : base(databaseProvider)
        {

        }

        public async Task<int> GetIdByNameAsync(string nome)
        {
            try
            {
                return await QueryFirstOrDefaultAsync<int>($"SELECT Id FROM SQLInjectionTest WHERE PrimeiroNome='{nome}';");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, innerException: e);
            }
        }
    }
}
