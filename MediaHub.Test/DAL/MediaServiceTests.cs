using System.IO.Abstractions.TestingHelpers;
using MediaHub.DAL.Model;
using MediaHub.DAL.Services;

namespace MediaHub.Test.DAL;

[TestClass]
public class MediaServiceTests
{
    [TestMethod]
    public void GetMedia_ReturnsMediaFoldersAndFiles_WhenDirectoryContainsBoth()
    {
        // Arrange
        string root = Path.Combine("root");
        string folder1 = Path.Combine(root, "folder1");
        string file1 = Path.Combine(root, "file1.txt");
        string file2 = Path.Combine(root, "file2.txt");
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { folder1, new MockDirectoryData() },
            { file1, new MockFileData("file1") },
            { file2, new MockFileData("file2") }
        });
        var service = new MediaService(root, mockFileSystem);

        // Act
        List<Media> result = service.GetMedia().ToList();

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.IsTrue(result.Any(media => media.Name == "folder1" && media.Type == MediaType.DIRECTORY));
        Assert.IsTrue(result.Any(media => media.Name == "file1.txt" && media.Type == MediaType.FILE));
        Assert.IsTrue(result.Any(media => media.Name == "file2.txt" && media.Type == MediaType.FILE));
    }

    [TestMethod]
    public void GetMedia_ReturnsEmpty_WhenDirectoryIsEmpty()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { @"c:\empty", new MockDirectoryData() }
        });
        var service = new MediaService(@"c:\empty", mockFileSystem);

        // Act
        var result = service.GetMedia();

        // Assert
        Assert.IsFalse(result.Any());
    }

    [TestMethod]
    public void MediaSerivce_Creates_RootPath_When_It_Does_Not_Exist()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var service = new MediaService(@"c:\nonexistent", mockFileSystem);

        // Act
        service.GetMedia();

        // Assert
        Assert.IsTrue(mockFileSystem.Directory.Exists(@"c:\nonexistent"));
    }
    
    [TestMethod]
    public void StripRootPath_ReturnsPathWithoutRootPath()
    {
        // Arrange
        var service = new MediaService(@"c:\root");

        // Act
        var result = service.StripRootPath(@"c:\root\path");

        // Assert
        Assert.AreEqual("path", result);
    }
    
    [TestMethod]
    public void CombineRootPath_ReturnsRootPathCombinedWithPath()
    {
        // Arrange
        var service = new MediaService(@"c:\root");

        // Act
        var result = service.CombineRootPath(@"path");

        // Assert
        Assert.AreEqual(@"c:\root\path", result);
    }
    
    [TestMethod] 
    public void CombineRootPath_ReturnsRootPath_WhenPathIsEmpty()
    {
        // Arrange
        var service = new MediaService(@"c:\root");

        // Act
        var result = service.CombineRootPath("");

        // Assert
        Assert.AreEqual(@"c:\root", result);
    }
    
    [TestMethod]
    public void GetMediaFile_ReturnsFileInfo_WhenFileExists()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { @"c:\root\file.txt", new MockFileData("file") }
        });
        var service = new MediaService(@"c:\root", mockFileSystem);

        // Act
        var result = service.GetMediaFile("file.txt");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("file.txt", result.Name);
    }
    
    [TestMethod]
    public void GetMediaFile_ReturnsNull_WhenFileDoesNotExist()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var service = new MediaService(@"c:\root", mockFileSystem);

        // Act
        var result = service.GetMediaFile("file.txt");

        // Assert
        Assert.IsNull(result);
    }
}