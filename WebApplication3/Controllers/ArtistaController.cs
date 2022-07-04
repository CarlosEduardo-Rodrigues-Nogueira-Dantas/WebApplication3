using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using WebApplication3.Model;
using WebApplication3.Options;

namespace WebApplication3.Controllers
{
    [Route("api/artistas")]
    [ApiController]
    public class ArtistaController : ControllerBase
    {
        [HttpPost]
        [Route("")]
        public IActionResult CreateArtista([FromServices] IOptions<ConnectionStringOptions> options, [FromBody] Artista artista)
        {
            if (string.IsNullOrEmpty(artista.Nome))
                return BadRequest("Nome nao pode ser nulo");

            if (string.IsNullOrEmpty(artista.CPF))
                return BadRequest("CPF nao pode ser nulo");

            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"insert into Usuario (CPF,Nome,Email,DataNasc,DataCriacao) values (@CPF,@Nome,@Email,@DataNasc,@DataCriacao)";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("CPF", artista.CPF));
                command.Parameters.Add(new SqlParameter("Nome", artista.Nome));
                command.Parameters.Add(new SqlParameter("Email", artista.Email));
                command.Parameters.Add(new SqlParameter("DataNasc", artista.DataNasc));
                command.Parameters.Add(new SqlParameter("DataCriacao", artista.DataCriacao));

                command.ExecuteNonQuery();
            }

            return Ok();
        }


        [HttpPut]
        [Route("{idArtista}")]
        public IActionResult UpdateArtista([FromServices] IOptions<ConnectionStringOptions> options, [FromBody] Artista artista, [FromRoute] int idArtista) 
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection)) 
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = @"UPDATE Artista SET CPF = @CPF,Nome = @Nome,Email = @Email,DataNasc = @DataNasc,DataCriacao = @DataCriacao where Id = @Id";

                command.Parameters.Add(new SqlParameter("Nome", artista.Nome));
                command.Parameters.Add(new SqlParameter("CPF", artista.CPF));
                command.Parameters.Add(new SqlParameter("Email", artista.Email));
                command.Parameters.Add(new SqlParameter("DataNasc", artista.DataNasc));
                command.Parameters.Add(new SqlParameter("DataCriacao", artista.DataCriacao));

                command.Parameters.Add(new SqlParameter("Id", idArtista));


                command.ExecuteNonQuery();

            }
            return Ok();
        }

        [HttpDelete]
        [Route("{idArtista}")]

        public IActionResult DeleteUsuario([FromServices] IOptions<ConnectionStringOptions> options, [FromRoute] int idArtista)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"delete from Artista where Id = @Id";

                command.Parameters.Add(new SqlParameter("Id", idArtista));

                command.ExecuteNonQuery();
            }

            return Ok();
        }

        [HttpGet]
        [Route("{idArtista}")]
        public IActionResult GetUsuario([FromServices] IOptions<ConnectionStringOptions> options, [FromQuery] int idArtista)
        {
            Artista artista = null;

            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"select * from Artista where Id = @Id";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("Id", idArtista));

                using (SqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        artista = new Artista()
                        {
                            CPF = dr.GetString(1),
                            DataCriacao = dr.GetDateTime(5),
                            DataNasc = dr.GetDateTime(2),
                            Email = dr.GetString(4),
                            Id = dr.GetInt32(3),
                            Nome = dr.GetString(0)
                        };
                    }
                }
            }
            return Ok(artista);
        }


        private bool VerificarArtistaExistente(IOptions<ConnectionStringOptions> options, string CPF, string Email)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                Usuario usuario = new Usuario();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = @"select Id from Usuario where CPF = @CPF and Email = @Email";

                command.Parameters.Add(new SqlParameter("Email", Email));
                command.Parameters.Add(new SqlParameter("CPF", CPF));

                int? id = (int?)command.ExecuteScalar();


                return id != null;

            }
        }

    }
}
