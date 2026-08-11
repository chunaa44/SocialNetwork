using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ImageProcessorLibrary;


public class ImageProcessor
{
    private const int TargetSize = 512;

    /// <summary>
    /// Processes the provided <see cref="Bitmap"/> by validating minimum dimensions,
    /// cropping to a centered square, and resizing down to <see cref="TargetSize"/> when required.
    /// </summary>
    /// <param name="image">The source <see cref="Bitmap"/> to process. Must not be <c>null</c>.</param>
    /// <returns>
    /// A <see cref="Bitmap"/> that is square. If the input already equals <see cref="TargetSize"/> for both
    /// width and height, the same instance is returned; otherwise a new <see cref="Bitmap"/> is returned.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="image"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the input image width or height is smaller than <see cref="TargetSize"/>
    /// </exception>
    public Bitmap ProcessImage(Bitmap image)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));
      
        int width = image.Width;
        int height = image.Height;

        if (width < TargetSize)
            throw new ArgumentException("Image width too small");

        if (height < TargetSize)
            throw new ArgumentException("Image height too small");

        if (width == TargetSize && height == TargetSize)
            return image;

        Bitmap cropped = CropToSquare(image);

        if (cropped.Width > TargetSize)
            return ResizeToTarget(cropped);

        return cropped;
    }

    /// <summary>
    /// Crops the provided <see cref="Bitmap"/> to a centered square using the smaller of the image's width and height.
    /// </summary>
    /// <param name="image">Source image to crop. Must be non-null and have positive dimensions.</param>
    /// <returns>A new <see cref="Bitmap"/> instance that is square and contains the centered crop.</returns>
    private Bitmap CropToSquare(Bitmap image)
    {
        int size = Math.Min(image.Width, image.Height);

        int x = (image.Width - size) / 2;
        int y = (image.Height - size) / 2;

        Rectangle cropArea = new Rectangle(x, y, size, size);

        Bitmap result = new Bitmap(size, size);

        using (Graphics g = Graphics.FromImage(result))
        {
            g.DrawImage(image, 
               new Rectangle(0, 0, size, size),
               cropArea,
               GraphicsUnit.Pixel);
        }

        return result;
    }

    /// <summary>
    /// Resizes the provided square <see cref="Bitmap"/> to the fixed <see cref="TargetSize"/>.
    /// </summary>
    /// <param name="image">A square <see cref="Bitmap"/> whose width/height are greater than <see cref="TargetSize"/>.</param>
    /// <returns>A new <see cref="Bitmap"/> instance sized to <see cref="TargetSize"/> × <see cref="TargetSize"/>.</returns>
    private Bitmap ResizeToTarget(Bitmap image)
    {
        Bitmap resized = new Bitmap(TargetSize, TargetSize);

        using (Graphics g = Graphics.FromImage(resized))
        {
            g.DrawImage(image, 0, 0, TargetSize, TargetSize);
        }

        return resized;
    }
}
