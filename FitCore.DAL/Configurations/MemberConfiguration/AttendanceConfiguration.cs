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
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.HasKey(a => a.AttendanceID);
            builder.Property(a => a.Status).HasMaxLength(20);
            builder.Property(a => a.Type).HasMaxLength(20);

            builder.HasOne(a => a.MemberProfile)
                .WithMany(mp => mp.Attendances)
                .HasForeignKey(a => a.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Class)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.ClassID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
