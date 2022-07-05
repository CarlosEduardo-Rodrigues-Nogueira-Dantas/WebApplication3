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
        public IActionResult PostAlbum([FromServices] IOptions<ConnectionStringOptions> options, [FromBody] Album album)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"insert into Album (NomeAlbum,DataCriacaoAlbum,DataLancamentoAlbum) values (@NomeAlbum,@DataCriacaoAlbum,@DataLancamentoAlbum)";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("NomeAlbum", album.NomeAlbum));
                command.Parameters.Add(new SqlParameter("DataCriacaoAlbum", album.DataCriacaoAlbum));
                command.Parameters.Add(new SqlParameter("DataLancamentoAlbum", album.DataLancamentoAlbum));


                command.ExecuteNonQuery();
            }

            return Ok();
        }

        [HttpPut]
        [Route("{idAlbum}")]
        public IActionResult UpdateAlbum([FromServices] IOptions<ConnectionStringOptions> options, [FromBody] Album album, [FromRoute] int idAlbum) 
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection)) 
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"UPDATE Usuario SET NomeAlbum = @NomeAlbum,DataCriacaoAlbum = @DataCriacaoAlbum,DataLancamentoAlbum = @DataLancamentoAlbum where Id = @Id";

                command.Parameters.Add(new SqlParameter("NomeAlbum", album.NomeAlbum));
                command.Parameters.Add(new SqlParameter("DataCriacaoAlbum", album.DataCriacaoAlbum));
                command.Parameters.Add(new SqlParameter("DataLancamentoAlbum", album.DataLancamentoAlbum));

                command.Parameters.Add(new SqlParameter("Id", idAlbum));

                command.ExecuteNonQuery();

            }

            return Ok();
        }

        [HttpDelete]
        [Route("{idAlbum}")]
        public IActionResult DeleteAlbum([FromServices] IOptions<ConnectionStringOptions> options, [FromRoute] int idAlbum) 
        { 
            using(SqlConnection connection = new SqlConnection(options.Value.MyConnection)) 
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
    }
}
