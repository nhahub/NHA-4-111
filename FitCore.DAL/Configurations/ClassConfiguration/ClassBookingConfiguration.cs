using FitCore.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCore.DAL.Configurations.ClassConfiguration
{
    public class ClassBookingConfiguration : IEntityTypeConfiguration<ClassBooking>
    {
        public void Configure(EntityTypeBuilder<ClassBooking> builder)
        {
            builder.HasKey(b => b.BookingID);

            builder.Property(b => b.SessionDate).IsRequired();
            builder.Property(b => b.CreatedAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();

            builder.HasOne(b => b.ClassSchedule)
                .WithMany(cs => cs.Bookings)
                .HasForeignKey(b => b.ClassScheduleID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.MemberProfile)
                .WithMany(mp => mp.ClassBookings)
                .HasForeignKey(b => b.MemberUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
