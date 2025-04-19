namespace MediaHub.DAL.Model;

public class WatchedMedia
{
    public required Guid Id { get; set; }
    public required Guid MediaId { get; set; }
    public required string UserId { get; set; }
    public required DateTime WatchedAt { get; set; }
    public required bool Watched { get; set; }
    
    public Media Media { get; set; }
}