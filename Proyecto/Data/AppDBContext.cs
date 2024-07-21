
using Microsoft.EntityFrameworkCore;
using Proyecto.Models;

namespace Proyecto.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext>options): base(options)
        {
            
        }
        public DbSet<Usuario> Usuarios { get; set; }
        //ACa tienen que ir todas las tablas del documento
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(tb =>
            {
                tb.HasKey(col => col.IdUsuario);
                tb.Property(col => col.IdUsuario)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd();

                tb.Property(col => col.NombreCompleto).HasMaxLength(50);
                tb.Property(col => col.Correo).HasMaxLength(50);
                tb.Property(col => col.clave).HasMaxLength(50);
            });

            modelBuilder.Entity<Usuario>().ToTable("Usuario");
                
                
        }

        
    }
}
