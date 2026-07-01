using FitCore.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Configurations.MemberConfiguration
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.HasKey(m => m.MembershipID);
            builder.Property(m => m.Status).IsRequired().HasMaxLength(20);

            builder.HasOne(m => m.MemberProfile)
                .WithMany(mp => mp.Memberships)
                .HasForeignKey(m => m.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.SubscriptionPlan)
                .WithMany(p => p.Memberships)
                .HasForeignKey(m => m.PlanID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
