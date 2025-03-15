using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoteSystem.Entities;

namespace VoteSystem.Configurations;

public class VoteEntityConfiguration
{
    public void Configure(EntityTypeBuilder<VoteEntity> builder)
    {
        builder.ToTable("votes");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .UseIdentityByDefaultColumn()
            .IsRequired();

        builder.HasKey(v => v.Id);

        builder.Property(v => v.PollId)
            .HasColumnName("poll_id")
            .IsRequired();

        builder.Property(v => v.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(v => v.ChoiceId)
            .HasColumnName("choice_id")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();
    }
}