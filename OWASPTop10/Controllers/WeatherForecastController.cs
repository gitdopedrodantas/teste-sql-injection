using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OWASPTop10.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWASPTop10.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
           "Teste", "retorno"
        };

        IDatabaseProvider _databaseProvider;

        public WeatherForecastController(IDatabaseProvider database)
        {
            _databaseProvider = database;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        } 
        
    
        [HttpGet("{nome}/id")]
        public async Task<IActionResult> GetIdByName([FromRoute] string nome)
        {
            var repo = new NomesRepositorio(_databaseProvider);
            var id = await repo.GetIdByNameAsync(nome);

            return Ok(id);
        }
    }
}
