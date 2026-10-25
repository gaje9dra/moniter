namespace Moniter.Api.Data.Entities;

public interface ITimestampedEntity
{
    DateTimeOffset CreatedAt { get; set; }

    DateTimeOffset UpdatedAt { get; set; }
}
