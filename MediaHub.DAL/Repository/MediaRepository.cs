using MediaHub.DAL.Model;

namespace MediaHub.DAL.Repository;

public class MediaRepository
{
    private readonly MediaHubDBContext _context;

    public MediaRepository(MediaHubDBContext context)
    {
        _context = context;
    }

    /**
     * Get a list of media
     *
     * @param id The parent id of the media you want to get
     */
    public IEnumerable<Media> getSubMedia(Guid? parentId)
    {
        return _context.Media
            .Where(media => media.ParentId == parentId)
            .ToList();
    }
    
    public Media? GetMediaByPath(string path)
    {
        return _context.Media
            .FirstOrDefault(media => media.Path == path);
    }

    public Media? GetMediaById(Guid id)
    {
        return _context.Media
            .SingleOrDefault(media => media.Id == id);
    }
    
    public IEnumerable<Media> GetAllMediaQuery()
    {
        return _context.Media;
    }
    
    public void DeleteMedia(Media media)
    {
        _context.Media.Remove(media);
        _context.SaveChanges();
    }
    
    public void DeleteMediaMultiple(IEnumerable<Media> media)
    {
        _context.Media.RemoveRange(media);
        _context.SaveChanges();
    }

    public void AddMedia(Media media)
    {
        _context.Media.Add(media);
        _context.SaveChanges();
    }
    
    public void AddMediaMultiple(IEnumerable<Media> media)
    {
        _context.Media.AddRange(media);
        _context.SaveChanges();
    }

    public List<Media> GetMediaByPathMultiple(IEnumerable<RelativePath> paths)
    {
        return _context.Media
            .Where(media => paths.Contains(media.Path))
            .ToList();
    }
}