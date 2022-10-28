using FatoRelevante.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FatoRelevante.Context
{
    public class FatosRelevantesContext : DbContext
    {
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<RegistroFatoRelevante> FatosRelevantes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Server=localhost;Database=FatoRelevanteDb;Trusted_Connection=True;");
        }


    }
}
