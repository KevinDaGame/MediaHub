namespace MediaHub.DAL.FS.Model;

public class RelativePath
{
    public string Value { get; set; }
    
    public RelativePath(string path)
    {
        Value = path;
    }
    
    public static implicit operator string(RelativePath path)
    {
        return path.Value;
    }
    
    public static explicit operator RelativePath(string path)
    {
        return new RelativePath(path);
    }
    
    public static RelativePath operator +(RelativePath left, string right)
    {
        return (RelativePath)string.Concat(left.Value, right);
    }
}