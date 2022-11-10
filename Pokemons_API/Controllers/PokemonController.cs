using Microsoft.AspNetCore.Mvc;
using Pokemons_API.Mapper;
using Pokemons_API.Models;
using Pokemons_API.Models.Forms;
using Tools;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Pokemons_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly Connection _connection;

        public PokemonController(ILogger<PokemonController> logger, Connection connection)
        {
            _connection = connection;
        }


        [HttpGet("GetALL")]
        public IActionResult GetALL()
        {

            Command command = new Command("SELECT Id, FrenchName, EnglishName FROM [Pokemon];", false);

            try
            {
                return Ok(_connection.ExecuteReader(command, dr => dr.ToPokemon()).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Un probleme est survenu lors de la recuperation des pokemons" });
            }
        }

        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            Command command = new Command("SELECT Id, FrenchName, EnglishName FROM [Pokemon] WHERE Id = @Id;", false);
            command.AddParameter("Id", id);
            try
            {
                Pokemon? pokemon = _connection.ExecuteReader(command, dr => dr.ToPokemon()).SingleOrDefault();

                return (pokemon == null) ? BadRequest(new { Message = "le pokemon n'a pas été trouvé" }) : Ok(pokemon);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<PokemonController>/5
        [HttpGet("GetByChar/{name?}")]
        public IActionResult GetByChar(string? name)
        {
            Command command = new Command($"SELECT Id, FrenchName, EnglishName FROM [Pokemon] WHERE FrenchName LIKE @Name OR EnglishName LIKE @Name ;", false);
            command.AddParameter("Name", $"%{name}%" ?? "");
            try
            {
                return Ok(_connection.ExecuteReader(command, dr => dr.ToPokemon()).ToList());

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       

        [HttpPost("Insert")]
        public IActionResult Insert(AddPokemon form)
        {
            Command command = new Command("INSERT INTO Pokemon(FrenchName, EnglishName) OUTPUT inserted.id VALUES(@FrenchName, @EnglishName)", false);
            command.AddParameter("FrenchName", form.FrenchName);
            command.AddParameter("EnglishName", form.EnglishName);

            int? id = (int?)_connection.ExecuteScalar(command); // recuperer l'id du contact inseré

            if (id.HasValue)
            {
                Pokemon pokemon = new Pokemon()
                {
                    Id = id.Value,
                    FrenchName = form.FrenchName,
                    EnglishName = form.EnglishName,
                };
                return Ok(pokemon);
            }
            else
            {
                return BadRequest(new { Message = "Un probleme est survenu, l'insertion d'un pokemon dans la base de donnée a échouée" });
            }
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            Command command = new Command("DELETE FROM Pokemon WHERE Id = @Id", false);
            command.AddParameter("Id", id);

            try
            {
                int? nbRow = _connection.ExecuteNonQuery(command);
                return nbRow == 0 ? BadRequest(new { Message = "Aucune ligne n'a ete supprimer le pokemon n'existe pas dans la base de données !" }) : Ok(nbRow);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
