using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using WebApplication3.Model;
using WebApplication3.Options;

namespace WebApplication3.Controllers
{
    [Route("api/albuns")]
    [ApiController]
    public class AlbumController : ControllerBase
    {

        [HttpPost]
        [Route("")]
        //[FromServices] = sobrescreva a fonte de associação injetando os valores via injeção de dependência em um método Action específico.
        public IActionResult PostAlbum([FromServices] IOptions<ConnectionStringOptions> options, [FromBody] Album album)
        {
            //Este metodo, cria um album  nome
            bool albumExistente = VerificarAlbumExistente(options, album.NomeAlbum);

            //verificar se ja nao existe um album com o mesmo 

            if (albumExistente == true) 
            {
                return BadRequest("Não podem haver albuns com nomes repetidos para um mesmo artista.");
            }

            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"insert into Album (Nome,DataCriacao,DataLancamento,IdArtista) values (@Nome,@DataCriacao,@DataLancamento,@IdArtista)";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("Nome", album.NomeAlbum));
                command.Parameters.Add(new SqlParameter("DataCriacao", album.DataCriacao));
                command.Parameters.Add(new SqlParameter("DataLancamento", album.DataLancamento));
                command.Parameters.Add(new SqlParameter("IdArtista", album.IdArtista));


                command.ExecuteNonQuery();
            }

            return Ok();
        }
        //Este metodo atualiza um album
        [HttpPut]
        [Route("{idAlbum}")]
        public IActionResult UpdateAlbum([FromServices] IOptions<ConnectionStringOptions> options, [FromBody] Album album, [FromRoute] int idAlbum)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"UPDATE Album SET Nome = @Nome,DataCriacao = @DataCriacao,DataLancamento = @DataLancamento where Id = @Id";

                command.Parameters.Add(new SqlParameter("Nome", album.NomeAlbum));
                command.Parameters.Add(new SqlParameter("DataCriacao", album.DataCriacao));
                command.Parameters.Add(new SqlParameter("DataLancamento", album.DataLancamento));

                command.Parameters.Add(new SqlParameter("Id", idAlbum));

                command.ExecuteNonQuery();

            }

            return Ok();
        }
        //Este metodo deleta um album
        [HttpDelete]
        [Route("{idAlbum}")]
        public IActionResult DeleteAlbum([FromServices] IOptions<ConnectionStringOptions> options, [FromRoute] int idAlbum)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();
                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"delete from Album where Id = @Id";

                command.Parameters.Add(new SqlParameter("Id", idAlbum));

                command.ExecuteNonQuery();
            }
            return Ok();
        }

        //Este metodo Get um album e volta todos os dados do album
        [HttpGet]
        [Route("{idAlbum}")]
        public IActionResult GetAlbum([FromServices] IOptions<ConnectionStringOptions> options, [FromQuery] int idAlbum)
        {
            Album album = null;

            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"select * from Album where Id = @Id";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("Id", idAlbum));

                using (SqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        album = new Album
                        {
                            DataCriacao = dr.GetDateTime(1),
                            DataLancamento = dr.GetDateTime(4),
                            NomeAlbum = dr.GetString(0)
                        };
                    }
                }
                return Ok(album);
            }
        }

        //Metodo que verifica um album existente
        private bool VerificarAlbumExistente(IOptions<ConnectionStringOptions> options, string nomeAlbum)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                Album album = new Album();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = @"select IdArtista from Album where Nome = @Nome";

                command.Parameters.Add(new SqlParameter("Nome", nomeAlbum));

                int? id = (int?)command.ExecuteScalar();


                return id != null;

            }
        }
    }
}
    