namespace WebApplication3.Model
{
    public class Artista
    {
        //nome, email, data nascimento, cpf, data de criacao

        public string Nome { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataNasc { get; set; }
        public int Id { get; set; }
    }
}
