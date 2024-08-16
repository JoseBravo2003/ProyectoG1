using Microsoft.EntityFrameworkCore;
using Proyecto.Models;

namespace Proyecto.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<PurchaseHistory> PurchaseHistories { get; set; }
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Comida> Comidas { get; set; } // Agregado

        public DbSet<Cart> Carts { get; set; } // Agregado


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
                tb.Property(col => col.Role).HasMaxLength(50);
                tb.ToTable("Usuario");
            });

            modelBuilder.Entity<PurchaseHistory>(tb =>
            {
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id)
                  .UseIdentityColumn()
                  .ValueGeneratedOnAdd();

                tb.Property(col => col.PaymentType).HasMaxLength(50);
                tb.Property(col => col.MaskedCardNumber).HasMaxLength(19);
                tb.Property(col => col.ProductName).HasMaxLength(100);
                tb.Property(col => col.UnitPrice).HasColumnType("decimal(18,2)");
                tb.Property(col => col.Qty).HasColumnType("int");
                tb.Property(col => col.TotalPrice).HasColumnType("decimal(18,2)");
                tb.Property(col => col.OrderId).HasMaxLength(50);
                tb.Property(col => col.Status).HasMaxLength(20);

                // Configuración de la relación con Usuario
                tb.Property(col => col.UserId).IsRequired();
                tb.HasOne(col => col.Usuario)
                  .WithMany(u => u.PurchaseHistory)
                  .HasForeignKey(col => col.UserId);
            });

            modelBuilder.Entity<Comida>(tb =>
            {
                tb.HasKey(col => col.IdComida);
                tb.Property(col => col.IdComida)
                  .UseIdentityColumn()
                  .ValueGeneratedOnAdd();

                tb.Property(col => col.Nombre).HasMaxLength(100).IsRequired();
                tb.Property(col => col.Descripcion).HasMaxLength(255);
                tb.Property(col => col.Precio).HasColumnType("decimal(18,2)").IsRequired();
                tb.Property(col => col.Categoria).HasMaxLength(50);
                tb.Property(col => col.ImagenUrl).HasMaxLength(255);
                tb.Property(col => col.Cantidad).HasColumnType("int").IsRequired();

                tb.ToTable("Comida");
            });

            // Configuración de la entidad ShoppingCartItem
            modelBuilder.Entity<Cart>(tb =>
            {
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id)
                  .UseIdentityColumn()
                  .ValueGeneratedOnAdd();

                tb.Property(col => col.Quantity).HasColumnType("int").IsRequired();
                tb.Property(col => col.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();

                tb.HasOne(col => col.Usuario)
                  .WithMany()
                  .HasForeignKey(col => col.UserId);

                tb.HasOne(col => col.Comida)
                  .WithMany()
                  .HasForeignKey(col => col.IdComida);

                tb.ToTable("Cart");
            });
        }
       
    }
}
