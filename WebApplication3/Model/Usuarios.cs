using System.Data;

namespace WebApplication3.Model
{
    public class Usuario
    {
        public string CPF { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataNasc { get; set; }
        public DateTime DataCriacao { get; set; }
        public int Id { get; internal set; }
    }
}
