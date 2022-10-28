namespace FatoRelevante.Entidades
{
    public class Empresa
    {
        public Empresa()
        {
            Id = new Guid();
        }
        public Guid Id { get; set; }
        public string CodigoCvm { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
    }
}
