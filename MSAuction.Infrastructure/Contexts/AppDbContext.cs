namespace MSAuction.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using MSAuction.Domain.Entities;
using MSAuction.Infrastructure.Settings;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Auction> Auctions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AuctionConfiguration());

        modelBuilder.Entity<Auction>(entity =>
        {
            entity.ToTable("subastas");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProductId).HasColumnName("producto_id").IsRequired();
            entity.Property(e => e.UserId).HasColumnName("usuario_id").IsRequired();
            entity.Property(e => e.Title).HasColumnName("titulo").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("descripcion");
            entity.Property(e => e.InitialPrice).HasColumnName("precio_inicial").HasColumnType("decimal(12,2)").IsRequired();
            entity.Property(e => e.MinIncrement).HasColumnName("incremento_min").HasColumnType("decimal(12,2)").IsRequired();
            entity.Property(e => e.ReservePrice).HasColumnName("precio_reserva").HasColumnType("decimal(12,2)");
            entity.Property(e => e.StartDate).HasColumnName("fecha_inicio").IsRequired().HasColumnType("timestamp with time zone");
            entity.Property(e => e.EndDate).HasColumnName("fecha_fin").IsRequired().HasColumnType("timestamp with time zone");
            entity.Property(e => e.Status).HasColumnName("estado").HasMaxLength(20).HasDefaultValue("pending");
            entity.Property(e => e.Conditions).HasColumnName("condiciones");
            entity.Property(e => e.Type).HasColumnName("tipo_subasta").HasMaxLength(20).HasDefaultValue("normal");
        });
    }
}