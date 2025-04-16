using MediaHub.API.Controllers;
using MediaHub.DAL.FS.Model;
using MediaHub.DAL.Model;
using MediaHub.DAL.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaHub.Test.API.Controllers;

[TestClass]
public class MediaControllerTests
{
    [TestMethod]
    public void GetMedia_ReturnsMedia_WhenServiceReturnsMedia()
    {
        // Arrange
        var media = new List<Media>
        {
            new Media { Id = Guid.Empty, Name = "folder1", Path = (RelativePath)"", Type = MediaType.DIRECTORY},
            new Media { Id = Guid.Empty, Name = "file1.txt", Path = (RelativePath)"", Type = MediaType.FILE },
            new Media { Id = Guid.Empty, Name = "file2.txt", Path = (RelativePath)"", Type = MediaType.FILE }
        };
        var mockService = new Mock<IMediaService>();
        mockService.Setup(service => service.GetMedia()).Returns(media);
        var controller = new MediaController(new Mock<ILogger<MediaController>>().Object, mockService.Object);

        // Act
        IEnumerable<Media> result = controller.GetMedia(null);

        // Assert
        Assert.AreEqual(media, result);
    }
    
    [TestMethod]
    public void GetMedia_ReturnsMedia_WhenServiceReturnsMediaForPath()
    {
        // Arrange
        var media = new List<Media>
        {
            new Media { Id = Guid.Empty, Name = "folder1", Path = (RelativePath)"", Type = MediaType.DIRECTORY},
            new Media { Id = Guid.Empty, Name = "file1.txt", Path = (RelativePath)"", Type = MediaType.FILE },
            new Media { Id = Guid.Empty, Name = "file2.txt", Path = (RelativePath)"", Type = MediaType.FILE }
        };
        var mockService = new Mock<IMediaService>();
        mockService.Setup(service => service.GetMedia("path")).Returns(media);
        var controller = new MediaController(new Mock<ILogger<MediaController>>().Object, mockService.Object);

        // Act
        IEnumerable<Media> result = controller.GetMedia("path");

        // Assert
        Assert.AreEqual(media, result);
    }
}