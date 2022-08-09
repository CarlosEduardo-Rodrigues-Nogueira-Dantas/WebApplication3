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
        public IActionResult CreateMusica([FromServices] IOptions<ConnectionStringOptions> options, [FromBody] Musicas musicas)
        {
            if (musicas.TempDuracao.TotalSeconds <= 0)
                return BadRequest("O tempo de uma música deve ser maior que 0 segundoss");

            bool musicaExistente = VerificarMusicaExistente(options, musicas.NomeMusica);

            if (musicaExistente == true)
            {
                return BadRequest("Não podem haver músicas com nomes iguais para um mesmo artista"); 
            }

            bool musicaAlbumExistente = VerificarAlbumExistente(options,musicas.NomeMusica);

            if(musicaAlbumExistente == true) 
            {
                return BadRequest("Não podem haver músicas com nomes iguais para um mesmo album.");
            }

            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"insert into Musica (Nome,TempoDuracao,DataLancamento,IdArtista,IdAlbum) values (@Nome,@TempoDuracao,@DataLancamento,@IdArtista,@IdAlbum)";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("Nome", musicas.NomeMusica));
                command.Parameters.Add(new SqlParameter("TempoDuracao", musicas.TempDuracao));
                command.Parameters.Add(new SqlParameter("DataLancamento", musicas.DataLancamento));
                command.Parameters.Add(new SqlParameter("IdArtista", musicas.IdArtista));
                command.Parameters.Add(new SqlParameter("IdAlbum", musicas.IdAlbum));   

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

        private bool VerificarMusicaExistente(IOptions<ConnectionStringOptions> options, string nomeMusica) 
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection)) 
            {
                connection.Open();

                Musicas musicas = new();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"select IdArtista from Musica where Nome = @Nome";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("Nome", nomeMusica));

                int? id = (int?)command.ExecuteScalar();


                return id != null;
            }
        }


        private bool VerificarAlbumExistente(IOptions<ConnectionStringOptions> options, string nomeMusica)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                Musicas musicas = new();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"select IdAlbum from Musica where Nome = @Nome";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("Nome", nomeMusica));

                int? id = (int?)command.ExecuteScalar();


                return id != null;
            }
        }

    }
}
