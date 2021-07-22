using System;
using System.Threading.Tasks;

namespace OWASPTop10.Repository
{
    public class NomesRepositorio : BaseSqlRepository
    {
        public NomesRepositorio(IDatabaseProvider databaseProvider) : base(databaseProvider)
        {

        }

        public async Task<object> GetLastNameByNameAsync(string nome)
        {
            try
            {
                return await QueryFirstOrDefaultAsync<object>($"SELECT UltimoNome FROM SQLInjectionTest WHERE PrimeiroNome='{nome}';");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, innerException: e);
            }
        }
    }
}
