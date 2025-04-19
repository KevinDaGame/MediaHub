namespace MediaHub.DAL.Model;

public class RelativePath
{
    public string Value { get; set; }
    
    public RelativePath(string path)
    {
        Value = path;
    }
    
    public RelativePath()
    {
        Value = string.Empty;
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

    public override bool Equals(object? obj)
    {
        if (obj is RelativePath other)
        {
            return Value == other.Value;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(RelativePath left, RelativePath right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(RelativePath left, RelativePath right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return Value;
    }
}
