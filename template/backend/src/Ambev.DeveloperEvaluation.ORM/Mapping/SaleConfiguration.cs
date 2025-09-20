using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

            builder.Property(s => s.SaleNumber).IsRequired().HasMaxLength(50);
            builder.Property(s => s.SaleDate).IsRequired();
            builder.Property(s => s.CustomerId).IsRequired();
            builder.Property(s => s.CustomerName).IsRequired().HasMaxLength(150);
            builder.Property(s => s.BranchId).IsRequired();
            builder.Property(s => s.BranchName).IsRequired().HasMaxLength(150);
            builder.Property(s => s.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(s => s.IsCancelled).IsRequired();

            // Configuração do relacionamento: Uma Venda (Sale) tem muitos Itens (SaleItem)
            builder.HasMany(s => s.Items)
                   .WithOne() // Um Item pertence a uma Venda
                   .HasForeignKey(i => i.SaleId) // A chave estrangeira é SaleId na tabela de Itens
                   .OnDelete(DeleteBehavior.Cascade); // Se uma venda for deletada, seus itens também serão.
        }
    }
}