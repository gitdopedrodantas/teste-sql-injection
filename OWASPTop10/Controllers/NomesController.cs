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
    public class NomesController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
           "Frio", "Calor", "Chuva", "Nublado" 
        };

        IDatabaseProvider _databaseProvider;

        public NomesController(IDatabaseProvider database)
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
        
    
        [HttpGet("{nome}/last-name")]
        public async Task<IActionResult> GetLastNameByName([FromRoute] string nome)
        {
            var repo = new NomesRepositorio(_databaseProvider);
            var result = await repo.GetLastNameByNameAsync(nome);

            return Ok(result);
        }
    }
}
