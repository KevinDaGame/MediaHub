using System.Text.RegularExpressions;

namespace MediaHub.DAL.Model;

public partial class Media
{
    public required Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public required string Name { get; set; }
    public required RelativePath Path { get; set; }
    public string? ThumbnailUrl { get; set; }
    public required MediaType Type { get; set; }
    public IEnumerable<Media> Children { get; set; }

    public int ExtractNumericValueFromName()
    {
        // Regular expression to match numeric sequences
        var match = NumericValueRegex().Match(Name);
        return match.Success
            ? int.Parse(match.Value)
            :
            // Handle cases where there's no numeric value in the name
            int.MaxValue; // Or any other suitable default value
    }

    [GeneratedRegex("\\d+")]
    private static partial Regex NumericValueRegex();
}