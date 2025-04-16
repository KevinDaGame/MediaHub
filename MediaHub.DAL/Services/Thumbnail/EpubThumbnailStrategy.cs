using System.IO.Abstractions;
using System.IO.Compression;
using System.Net.Mime;
using System.Xml;
using MediaHub.DAL.FS.Model;
using MediaHub.DAL.FS.Services.MediaPath;
using Xabe.FFmpeg;

namespace MediaHub.DAL.FS.Services.Thumbnail;

public class EpubThumbnailStrategy : ThumbnailStrategy
{
    public EpubThumbnailStrategy(RootPathService rootPath, ThumbnailPathService thumbnailPath, IFileSystem fileSystem) :
        base(rootPath, thumbnailPath, fileSystem)
    {
    }

    public override IEnumerable<string> SupportedExtensions { get; } = new[] { "epub" };

    public override async Task ExtractThumbnail(RelativePath path)
    {
        // Construct the full path of the media file
        AbsolutePath mediaFilePath = RootPath.CombineRootPath(path);

        // Construct the output thumbnail file path
        AbsolutePath thumbnailFilePath = ThumbnailPath.CombineRootPath(path + ".webp");

        using ZipArchive zip = ZipFile.OpenRead(mediaFilePath);

        // open META-INF/container.xml
        ZipArchiveEntry containerEntry = zip.GetEntry("META-INF/container.xml") ??
                                         throw new Exception("Container.xml not found in epub");

        string rootFilePath;

        await using (Stream containerStream = containerEntry.Open())
        {
            using var reader = XmlReader.Create(containerStream);
            reader.ReadToFollowing("rootfile");
            rootFilePath = reader.GetAttribute("full-path") ??
                           throw new Exception("Rootfile path not found in container.xml");
        }

        // open rootfile as xml
        ZipArchiveEntry rootFileEntry = zip.GetEntry(rootFilePath) ??
                                        throw new Exception("Rootfile not found in epub");

        string? imagePath = null;
        await using (Stream rootFileStream = rootFileEntry.Open())
        {
            using var rootFileReader = XmlReader.Create(rootFileStream);

            // find first image in manifest
            rootFileReader.ReadToFollowing("manifest");
            while (rootFileReader.ReadToFollowing("item"))
            {
                string? mediaType = rootFileReader.GetAttribute("media-type");
                if (mediaType == null || !mediaType.StartsWith("image/")) continue;
                imagePath = rootFileReader.GetAttribute("href")!;
                break;
            }
        }
        
        if (imagePath == null)
        {
            throw new Exception("No image found in epub");
        }

        // open image
        ZipArchiveEntry imageEntry =
            zip.GetEntry(rootFilePath.Split("/")[0..^1].Aggregate((a, b) => a + "/" + b) + "/" + imagePath) ??
            throw new Exception("Image not found in epub");

        await using Stream imageStream = imageEntry.Open();
        string tempImagePath = Path.GetTempFileName();

        try
        {
            await using (FileSystemStream fileStream = FileSystem.File.Create(tempImagePath))
            {
                imageStream.CopyTo(fileStream);
            }

            IConversion conversion =
                await FFmpeg.Conversions.FromSnippet.Snapshot(tempImagePath, thumbnailFilePath,
                    TimeSpan.FromSeconds(0));
            //set image resolution to 480p
            conversion.AddParameter("-vf scale=480:-1");
            await conversion.Start();
        }
        finally
        {
            FileSystem.File.Delete(tempImagePath);
        }
    }
}