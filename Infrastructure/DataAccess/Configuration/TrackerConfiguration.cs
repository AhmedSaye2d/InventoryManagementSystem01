using Domain.Entities.ActivityTracker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccess.Configuration
{
    internal class TrackerConfiguration : IEntityTypeConfiguration<Tracker>
    {
        public void Configure(EntityTypeBuilder<Tracker> builder)
        {
            builder.ToTable("ActivityTrackers");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Operation)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.Date)
                .IsRequired();

            builder.Property(t => t.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(t => t.OperationStatus)
                .IsRequired();
        }
    }
}
