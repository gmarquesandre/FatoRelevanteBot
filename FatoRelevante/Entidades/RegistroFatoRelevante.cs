using FatoRelevante.Entidades.Base;

namespace FatoRelevante.Entidades
{
    public class RegistroFatoRelevante : EntidadePadrao
    {
        public string Tipo { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Assunto { get; set; } = string.Empty;
        public DateTime DataEntrega { get; set; }
        public string Status { get; set; } = string.Empty; 
        public string Modalidade { get; set; } = string.Empty;
        public DateTime DataReferencia { get; set; }      
        public string NumeroProtocoloEntrega { get; set; } = string.Empty;
        public string NumeroSequencia { get; set; } = string.Empty;
        public string NumeroProtocolo { get; set; } = string.Empty;
        public string CodigoTipoInstituicao { get; set; } = string.Empty;
        public Empresa Empresa { get; set; }
        public Guid EmpresaId { get; set; }
    }
}
