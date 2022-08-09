using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using WebApplication3.Model;
using WebApplication3.Options;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuantidadeMusicasDeterminadaPlaylistController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public IActionResult GetQuantidadePlalistMusica([FromServices] IOptions<ConnectionStringOptions> options, [FromQuery] Musicas musica)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"select * 
                                        from Usuario u
                                        where exists (
	                                      select 1 
	                                      from 
		                                    Playlist p 
		                                    join PlayListMusica pm on p.Id = pm.IdPlayList 
                                       	where 
		                                    u.Id = p.IdUsuario
		                                    and pm.IdMusica = 1)";
                command.CommandType = System.Data.CommandType.Text;


                command.ExecuteNonQuery();
            }
            return Ok();
        }
    }
}
