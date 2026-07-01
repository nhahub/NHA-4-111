using FitCore.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Configurations.InvoiceConfiguration
{
    public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
    {
        public void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            builder.HasKey(ii => ii.InvoiceItemID);
            builder.Property(ii => ii.SellPrice).HasColumnType("decimal(18,2)");
            builder.Property(ii => ii.LineTotal).HasColumnType("decimal(18,2)");

            builder.HasOne(ii => ii.Invoice)
                .WithMany(i => i.InvoiceItems)
                .HasForeignKey(ii => ii.InvoiceID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ii => ii.Product)
                .WithMany(p => p.InvoiceItems)
                .HasForeignKey(ii => ii.ProductID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
