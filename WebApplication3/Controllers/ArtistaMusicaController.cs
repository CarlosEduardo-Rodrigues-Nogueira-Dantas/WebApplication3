using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using WebApplication3.Model;
using WebApplication3.Options;

namespace WebApplication3.Controllers
{
    [Route("api/artistaMusica")]
    [ApiController]
    public class ArtistaMusicaController : ControllerBase
    {
        [HttpGet]
        [Route("{idArtista}")]
        public IActionResult GetQtdMusicaArtista([FromServices] IOptions<ConnectionStringOptions> options, [FromRoute] int idArtista)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"select COUNT(*) from Musica where IdArtista = @IdArtista";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("IdArtista", idArtista));

                int qtdMusicas = (int)command.ExecuteScalar(); 

                return Ok(qtdMusicas);
            }
        }
    }
}
