using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using WebApplication3.Model;
using WebApplication3.Options;

namespace WebApplication3.Controllers
{
    [Route("api/musicas")]
    [ApiController]
    public class MusicaController : ControllerBase
    {

        [HttpPost]
        [Route("")]
        public IActionResult CreateMusica([FromServices] IOptions<ConnectionStringOptions> options, [FromQuery] Musicas musicas)
        {
            //if(musicas.TempDuracao < 0)

            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"insert into Musica (NomeMusica,TempDuracao,DataLancamento) values (@NomeMusica,@TempDuracao,@DataLancamento)";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("NomeMusica", musicas.NomeMusica));
                command.Parameters.Add(new SqlParameter("TempDuracao", musicas.TempDuracao));
                command.Parameters.Add(new SqlParameter("DataLancamento", musicas.DataLancamento));

                command.ExecuteNonQuery();

            }

            return Ok();
        }

        [HttpPut]
        [Route("{idMusica}")]
        public IActionResult UpdateMusica([FromServices] IOptions<ConnectionStringOptions> options, [FromQuery] Musicas musicas, [FromRoute] int idMusica)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"UPDATE Musica Set NomeMusica = @NomeMusica,TempDuracao = @TempDuracao,DataLancamento = @DataLancamento where Id = @Id";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("NomeMusica", musicas.NomeMusica));
                command.Parameters.Add(new SqlParameter("TempDuracao", musicas.TempDuracao));
                command.Parameters.Add(new SqlParameter("DataLancamento", musicas.DataLancamento));


                command.Parameters.Add(new SqlParameter("Id", idMusica));

                command.ExecuteNonQuery();
            }

            return Ok();
        }

        [HttpDelete]
        [Route("{idMusica}")]
        public IActionResult DeleteMusica([FromServices] IOptions<ConnectionStringOptions> options, [FromRoute] int idMusica)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @" delete from Musica where Id = @Id";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("Id", idMusica));

                command.ExecuteNonQuery();
            }

            return Ok();
        }


        [HttpGet]
        [Route("{idMusica}")]
        public IActionResult GetPlaylist([FromServices] IOptions<ConnectionStringOptions> options, [FromQuery] int idMusica) 
        {
            Musicas musicas = null;

            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection)) 
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"select * from Musicat where Id = @Id";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("Id", idMusica));

                using (SqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        musicas = new Musicas()
                        {
                            DataLancamento = dr.GetDateTime(1),
                            NomeMusica = dr.GetString(0),
                            TempDuracao = dr.GetTimeSpan(4)
                        };
                    }
                }

                return Ok(musicas);
            }
        }
    }
}
