﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using WebApplication3.Model;
using WebApplication3.Options;
using WebApplication3.Validações;

namespace WebApplication3.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        [HttpPost]
        [Route("")]

        public IActionResult PostUsuario([FromServices] IOptions<ConnectionStringOptions> options, [FromBody] Usuario usuarios)
        {
            if (usuarios.DataNasc.Year < 1900)
                return BadRequest("Data de nascimento nao pode ser menor que 1900");

            if (usuarios.DataCriacao.Year < 1900)
                return BadRequest("Data de criação nao pode ser menor que 1900");

            if (string.IsNullOrEmpty(usuarios.Nome))
                return BadRequest("Nome nao pode ser nulo");

            if (string.IsNullOrEmpty(usuarios.CPF))
                return BadRequest("CPF nao pode ser nulo");
            
            bool Usuario = VerificarUsuarioExistente(options, usuarios.CPF, usuarios.Email);

            bool cpfValidar = ValidarCPF.Validar(usuarios.CPF);

            if(cpfValidar == false) 
            {
                return BadRequest("CPF invalido");
            }

            if (Usuario == true)
            {
                return BadRequest("Não pode haver usuario com o mesmo CPF e Email");
            }

            int qtdPlayList = QuantidadePlayListUsuario(options, usuarios.Id,usuarios.IdContrato);

            if (qtdPlayList > 5 && usuarios.IdContrato == 6)
            {
                return BadRequest("Um usuário pode ter no máximo 5 playlists no contrato free e ilimitadas playlists no contrato premium.");
            }

            using (SqlConnection Connection = new SqlConnection(options.Value.MyConnection))
            {
                Connection.Open();

                SqlCommand command = new();
                command.Connection = Connection;
                command.CommandText = @"insert into Usuario (CPF,Nome,Email,DataNasc,DataCriacao,IdContrato) values (@CPF,@Nome,@Email,@DataNasc,@DataCriacao,@IdContrato)";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("Nome", usuarios.Nome));
                command.Parameters.Add(new SqlParameter("CPF", usuarios.CPF));
                command.Parameters.Add(new SqlParameter("Email", usuarios.Email));
                command.Parameters.Add(new SqlParameter("DataNasc", usuarios.DataNasc));
                command.Parameters.Add(new SqlParameter("DataCriacao", usuarios.DataCriacao));
                command.Parameters.Add(new SqlParameter("TipoContrato", usuarios.NomeContrato));
                command.Parameters.Add(new SqlParameter("IdContrato", usuarios.IdContrato));

                int IdadeAtual = Convert.ToInt32(DateTime.Today.Subtract(usuarios.DataNasc).TotalDays / 365);

                command.ExecuteNonQuery();

            }

            return Ok();
        }

        [HttpPut]
        [Route("{IdUsuario}")]

        public IActionResult PutUsuario([FromServices] IOptions<ConnectionStringOptions> options, [FromRoute] int IdUsuario, [FromBody] Usuario usuarios)
        {
            using (SqlConnection Connection = new SqlConnection(options.Value.MyConnection))
            {
                Connection.Open();

                SqlCommand command = new();
                command.Connection = Connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = @"UPDATE Usuario SET CPF = @CPF,Nome = @Nome,Email = @Email,DataNasc = @DataNasc,DataCriacao = @DataCriacao, IdContrato = @IdContrato where Id = @Id";

                command.Parameters.Add(new SqlParameter("Nome", usuarios.Nome));
                command.Parameters.Add(new SqlParameter("CPF", usuarios.CPF));
                command.Parameters.Add(new SqlParameter("Email", usuarios.Email));
                command.Parameters.Add(new SqlParameter("DataNasc", usuarios.DataNasc));
                command.Parameters.Add(new SqlParameter("DataCriacao", usuarios.DataCriacao));
                command.Parameters.Add(new SqlParameter("IdContrato", usuarios.IdContrato));

                command.Parameters.Add(new SqlParameter("Id", IdUsuario));


                command.ExecuteNonQuery();
            }
            return Ok();
        }

        [HttpDelete]
        [Route("{IdUsuario}")]

        public IActionResult DeleteUsuario([FromServices] IOptions<ConnectionStringOptions> options, [FromRoute] int IdUsuario)
        {
            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"delete from Usuario where Id = @Id";

                command.Parameters.Add(new SqlParameter("Id", IdUsuario));

                command.ExecuteNonQuery();
            }

            return Ok();
        }


        [HttpGet]
        [Route("{idUsuario}")]
        public IActionResult GetUsuario([FromServices] IOptions<ConnectionStringOptions> options, [FromQuery] int idUsuario)
        {
            Usuario usuario = null;

            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = @"select * from Usuario where Id = @Id";
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SqlParameter("Id", idUsuario));

                using (SqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        usuario = new Usuario()
                        {
                            CPF = dr.GetString(1),
                            DataCriacao = dr.GetDateTime(5),
                            DataNasc = dr.GetDateTime(2),
                            Email = dr.GetString(4),
                            Id = dr.GetInt32(3),
                            Nome = dr.GetString(0),
                            IdContrato = dr.GetInt32(5)
                        };
                    }
                }
            }
            return Ok(usuario);
        }


        private bool VerificarUsuarioExistente(IOptions<ConnectionStringOptions> options, string CPF, string Email)
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

        private int QuantidadePlayListUsuario(IOptions<ConnectionStringOptions> options, int idUsuario, int idContrato)
        {
            Playlists playlists = new();

            using (SqlConnection connection = new SqlConnection(options.Value.MyConnection))
            {
                connection.Open();
                SqlCommand command = new();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = @"select count(*) from Playlist where IdUsuario = @IdUsuario";

                command.Parameters.Add(new SqlParameter("IdUsuario", idUsuario));

                int idQtdMusica = (int)command.ExecuteScalar();

                return idQtdMusica;

            }
        }
    }


}
