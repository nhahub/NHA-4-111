using FitCore.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FitCore.DAL.Configurations.InvoiceConfiguration
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(i => i.InvoiceID);
            builder.Property(i => i.TotalAmount).HasColumnType("decimal(18,2)");
            builder.Property(i => i.InvoiceStatus).IsRequired().HasMaxLength(20);

            builder.HasOne(i => i.User)
                .WithMany(mp => mp.Invoices)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}
