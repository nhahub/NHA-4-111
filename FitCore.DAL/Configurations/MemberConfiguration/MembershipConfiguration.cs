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

            builder.HasOne(m => m.User)
                .WithMany(mp => mp.Memberships)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.GymService).WithMany(mp => mp.Memberships).HasForeignKey(x => x.GymServiceId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
