namespace WebApplication3.Model
{
    public class Musicas
    {
        public string NomeMusica { get; set; }
        public TimeSpan TempDuracao { get; set; }
        public DateTime DataLancamento { get; set; }
        public int IdArtista { get; set; }
        public int IdAlbum { get; set; }
    }
}
