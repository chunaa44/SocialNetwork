using ImageProcessorLibrary;
using System.Drawing;

namespace TestImageProcessorLibrary;

[TestClass]
public class ImageProcessorTests
{
    private ImageProcessor processor;

    [TestInitialize]
    public void TestInit()
    {
        processor = new ImageProcessor();
    }

    [TestMethod]
    public void ProcessImage_NullImage_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            processor.ProcessImage(null));
    }

    [TestMethod]
    public void ProcessImage_WidthTooSmall_ThrowsException()
    {
        Bitmap image = new Bitmap(128, 512);
        Assert.Throws<ArgumentException>(() => 
            processor.ProcessImage(image));
    }

    [TestMethod]
    public void ProcessImage_HeightTooSmall_ThrowsException()
    {
        Bitmap image = new Bitmap(512, 128);
        Assert.Throws<ArgumentException>(() =>
            processor.ProcessImage(image));
    }

    [TestMethod]
    public void ProcessImage_Already512x512_ReturnsSameSize()
    {
        Bitmap image = new Bitmap(512, 512);
        Bitmap result = processor.ProcessImage(image);

        Assert.AreSame(image, result);

        // old
        //Assert.AreEqual(512, result.Width);
        //Assert.AreEqual(512, result.Height);
    }

    [TestMethod]
    public void ProcessImage_TallImage_CropsToSquare()
    {
        Bitmap image = new Bitmap(512, 600);
        Bitmap result = processor.ProcessImage(image);

        Assert.AreEqual(512, result.Width);
        Assert.AreEqual(512, result.Height);
    }

    [TestMethod]
    public void ProcessImage_WideImage_CropsToSquare()
    {
        Bitmap image = new Bitmap(600, 512);
        Bitmap result = processor.ProcessImage(image);

        Assert.AreEqual(512, result.Width);
        Assert.AreEqual(512, result.Height);
    }

    [TestMethod]
    public void ProcessImage_LargeImage_ResizesToTarget()
    {
        Bitmap image = new Bitmap(4000, 6000);
        Bitmap result = processor.ProcessImage(image);

        Assert.AreEqual(512, result.Width);
        Assert.AreEqual(512, result.Height);
    }
}
