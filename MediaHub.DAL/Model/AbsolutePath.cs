namespace MediaHub.DAL.Model;

public class AbsolutePath
{
    public string Value { get; set; }
    
    public AbsolutePath(string path)
    {
        Value = path;
    }
    
    public AbsolutePath()
    {
        Value = string.Empty;
    }
    
    public static implicit operator string(AbsolutePath path)
    {
        return path.Value;
    }
    
    public static explicit operator AbsolutePath(string path)
    {
        return new AbsolutePath(path);
    }
}