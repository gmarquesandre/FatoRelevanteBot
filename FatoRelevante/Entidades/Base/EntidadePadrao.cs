using System.ComponentModel.DataAnnotations;

namespace FatoRelevante.Entidades.Base
{
    public class EntidadePadrao
    {
        public EntidadePadrao()
        {
            Id = new Guid();
        }

        [Key]
        public Guid Id { get; set; }
    }
}