using System.IO.Abstractions;
using MediaHub.DAL.Model;
using MediaHub.DAL.Services.MediaPath;
using Xabe.FFmpeg;

namespace MediaHub.DAL.Services.Thumbnail;

public class VideoThumbnailStrategy: ThumbnailStrategy
{
    
    public VideoThumbnailStrategy(RootPathService rootPath, ThumbnailPathService thumbnailPath, IFileSystem fileSystem) : base(rootPath, thumbnailPath, fileSystem)
    {
    }
    
    public override async Task ExtractThumbnail(RelativePath path)
    {
        // Construct the full path of the media file
        AbsolutePath mediaFilePath = RootPath.CombineRootPath(path);

        // Construct the output thumbnail file path
        AbsolutePath thumbnailFilePath = ThumbnailPath.CombineRootPath(path + ".webp");

        // Extract the thumbnail halfway through the video
        var halfway = TimeSpan.FromSeconds(FFmpeg.GetMediaInfo(mediaFilePath).Result.Duration.TotalSeconds / 2);
        IConversion conversion =
            await FFmpeg.Conversions.FromSnippet.
                Snapshot(mediaFilePath, thumbnailFilePath, halfway);
        conversion.AddParameter("-vf scale=480:-1");
        conversion.SetOutputFormat(Format.webp);
        // Start the conversion
        await conversion.Start();
    }

    public override IEnumerable<string> SupportedExtensions => new[] {"mkv", "mp4"};
  
}