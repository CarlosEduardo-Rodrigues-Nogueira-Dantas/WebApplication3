using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using WebApplication3.Model;
using WebApplication3.Options;

namespace WebApplication3.Controllers
{
    [Route("api/playlists")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        [HttpPost]
        [Route("")]

        public IActionResult CreatePlaylist([FromServices] IOptions<ConnectionStringOptions> options, [FromBody] Playlists playlists)
        {

            bool playListExistente = VerificarPlaylistExistente(options,playlists.IdUsuario);

            if(playListExistente == true) 
            {
                return BadRequest("Uma playlist não pode ter o nome de outra playlist existente para um mesmo usuário.");
            }

           

            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"insert into Playlist (Nome,DataCriacao,IdUsuario) values (@Nome,@DataCriacao,@IdUsuario)";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("Nome", playlists.Nome));
                command.Parameters.Add(new SqlParameter("DataCriacao", playlists.DataCriacao));
                command.Parameters.Add(new SqlParameter("IdUsuario", playlists.IdUsuario));

                command.ExecuteNonQuery();


            }
            return Ok();
        }


        [HttpPut]
        [Route("{idPlayList}")]

        public IActionResult UpdatePlaylist([FromServices] IOptions<ConnectionStringOptions> options, [FromBody] Playlists playlists, [FromRoute] int idPlayList)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"UPDATE Playlist set NomeMusic = @NomeMusic, DataCriacao = @DataCriacao where Id = @Id";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("NomeMusic", playlists.Nome));
                command.Parameters.Add(new SqlParameter("DataCriacao", playlists.DataCriacao));
                command.Parameters.Add(new SqlParameter("Id", idPlayList));

                command.ExecuteNonQuery();


            }
            return Ok();
        }

        [HttpDelete]
        [Route("{idPlayList}")]

        public IActionResult DeletePlaylist([FromServices] IOptions<ConnectionStringOptions> options, [FromRoute] int idPlayList)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"delete from Playlist where Id = @Id ";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("id", idPlayList));

                command.ExecuteNonQuery();


            }
            return Ok();
        }

        [HttpGet]
        [Route("{idPlayList}")]
        public IActionResult GetPlaylist([FromServices] IOptions<ConnectionStringOptions> options, [FromQuery] int idPlayList)
        {
            Playlists playlists = null;

            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"select * from Playlist where Id = @Id";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("Id", idPlayList));

                using (SqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        playlists = new Playlists()
                        {
                            DataCriacao = dr.GetDateTime(4),
                            Nome = dr.GetString(0),
                            
                        };
                    }
                }

                return Ok(playlists);
            }
        }

        private bool VerificarPlaylistExistente(IOptions<ConnectionStringOptions>options,int idUsuario) 
        {
            Playlists playlists = new(); 

            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection)) 
            {
                connection.Open();
                SqlCommand command = new();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text; 
                command.CommandText = @"select NomeMusic from Playlist where IdUsuario = @IdUsuario";

                command.Parameters.Add(new SqlParameter("IdUsuario", idUsuario));

                int? id = (int?)command.ExecuteScalar();

                return id != null;

            }
        }

    }

}

