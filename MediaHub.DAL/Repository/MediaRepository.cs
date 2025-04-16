using MediaHub.DAL.FS.Model;

namespace MediaHub.DAL.FS.Repository;

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
    
    public Media? GetMedia(string path)
    {
        return _context.Media
            .FirstOrDefault(media => media.Path == path);
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

    public void AddMediaMultiple(IEnumerable<Media> media)
    {
        _context.Media.AddRange(media);
        _context.SaveChanges();
    }
}