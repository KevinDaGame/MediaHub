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

    public override bool Equals(object? obj)
    {
        if (obj is AbsolutePath other)
        {
            return Value == other.Value;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(AbsolutePath? left, AbsolutePath? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(AbsolutePath? left, AbsolutePath? right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return Value;
    }
}
