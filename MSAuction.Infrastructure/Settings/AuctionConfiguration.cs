namespace MSAuction.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MSAuction.Domain.Entities;

public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
{
    public void Configure(EntityTypeBuilder<Auction> builder)
    {
        builder.ToTable("subastas");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.ProductId).HasColumnName("producto_id").IsRequired();
        builder.Property(a => a.UserId).HasColumnName("usuario_id").IsRequired();
        builder.Property(a => a.Title).HasColumnName("titulo").HasMaxLength(100).IsRequired();
        builder.Property(a => a.Description).HasColumnName("descripcion");
        builder.Property(a => a.InitialPrice).HasColumnName("precio_inicial").HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(a => a.MinIncrement).HasColumnName("incremento_min").HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(a => a.ReservePrice).HasColumnName("precio_reserva").HasColumnType("decimal(12,2)");
        builder.Property(a => a.StartDate).HasColumnName("fecha_inicio").IsRequired().HasColumnType("timestamp with time zone");
        builder.Property(a => a.EndDate).HasColumnName("fecha_fin").IsRequired().HasColumnType("timestamp with time zone");
        builder.Property(a => a.Status).HasColumnName("estado").HasMaxLength(20).HasDefaultValue("pending");
        builder.Property(a => a.Conditions).HasColumnName("condiciones");
        builder.Property(a => a.Type).HasColumnName("tipo_subasta").HasMaxLength(20).HasDefaultValue("normal");
    }
}