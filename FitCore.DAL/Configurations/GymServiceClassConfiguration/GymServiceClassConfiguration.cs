using FitCore.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Configurations.GymServiceClassConfiguration
{
    public class GymServiceClassConfiguration : IEntityTypeConfiguration<GymServiceClass>
    {
        public void Configure(EntityTypeBuilder<GymServiceClass> builder)
        {
            builder.HasKey(sc => new { sc.ServiceId, sc.ClassId });

            
            builder.HasOne(sc => sc.GymService)
                .WithMany(s => s.GymServiceClasses)
                .HasForeignKey(sc => sc.ServiceId);

            
            builder.HasOne(sc => sc.Class)
                .WithMany(c => c.GymServiceClasses)
                .HasForeignKey(sc => sc.ClassId);
        }
    }
}
