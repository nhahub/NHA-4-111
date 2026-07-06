using FitCore.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FitCore.DAL.Configurations
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.HasKey(x => x.MembershipID);

            builder.HasOne(m => m.MemberProfile)
                   .WithOne(p => p.Membership)
                   .HasForeignKey<Membership>(m => m.MemberProfileId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
