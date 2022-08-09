using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using WebApplication3.Model;
using WebApplication3.Options;

namespace WebApplication3.Controllers
{
    [Route("api/playListMusica")]
    [ApiController]
    public class PlayListMusicaController : ControllerBase
    {
        [HttpPost]
        [Route("")]
        public IActionResult CreatePlaylistMusica([FromServices] IOptions<ConnectionStringOptions> options, [FromBody] PlayListMusica playListMusica)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"insert into PlayListMusica (IdMusica,IdPlayList) values (@IdMusica,@IdPlayList)";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("IdMusica", playListMusica.IdMusica));
                command.Parameters.Add(new SqlParameter("IdPlayList", playListMusica.IdPlayList));

                command.ExecuteNonQuery();
            }
            return Ok();
        }


        [HttpDelete]
        [Route("{idPlayList}")]
        public IActionResult DeletePlaylistMusica([FromServices] IOptions<ConnectionStringOptions> options, [FromBody] PlayListMusica playListMusica, [FromRoute] int idPlayList)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"Delete from PlayListMusica where idPlayList = @idPlayList";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("IdPlayList", playListMusica.IdPlayList));

                command.ExecuteNonQuery();
            }
            return Ok();
        }
    }
}
