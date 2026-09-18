namespace Ara.Domain.Entities;

public class Destination
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Island { get; set; }
    public required string Description { get; set; }
    public double Rating { get; set; }
    public string? ImageUrl { get; set; }
}
