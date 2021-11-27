using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace BeltsPack.Utils
{
    public static class ImageHelper
    {
        // EXIF related constants
        private const int OrientationKey = 0x0112;
        private const int NotSpecified = 0;
        private const int NormalOrientation = 1;
        private const int MirrorHorizontal = 2;
        private const int UpsideDown = 3;
        private const int MirrorVertical = 4;
        private const int MirrorHorizontalAndRotateRight = 5;
        private const int RotateLeft = 6;
        private const int MirorHorizontalAndRotateLeft = 7;
        private const int RotateRight = 8;

        public static Size ResizeKeepAspect(this Size src, int maxWidth, int maxHeight, bool enlarge = false)
        {
            maxWidth = enlarge ? maxWidth : Math.Min(maxWidth, src.Width);
            maxHeight = enlarge ? maxHeight : Math.Min(maxHeight, src.Height);

            decimal rnd = Math.Min(maxWidth / (decimal)src.Width, maxHeight / (decimal)src.Height);
            return new Size((int)Math.Round(src.Width * rnd), (int)Math.Round(src.Height * rnd));
        }

        /// <summary>
        /// Resize the image to the specified width and height, keeping aspect ratio.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="maxWidth">The max width to resize to.</param>
        /// <param name="maxHeight">The max height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            var resizedSize = image.Size.ResizeKeepAspect(maxWidth, maxHeight);

            var destRect = new Rectangle(0, 0, resizedSize.Width, resizedSize.Height);
            var destImage = new Bitmap(resizedSize.Width, resizedSize.Height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            // this is really important when you have image with EXIF data saying you need to rotate/mirror the image
            FixExifRotation(image, destImage);

            return destImage;
        }

        private static void FixExifRotation(Image image, Bitmap newBitmap)
        {
            // Fix orientation if needed.
            if (image.PropertyIdList.Contains(OrientationKey))
            {
                var orientation = (int)image.GetPropertyItem(OrientationKey).Value[0];
                switch (orientation)
                {
                    case NotSpecified: // Assume it is good.
                    case NormalOrientation:
                        // No rotation required.
                        break;
                    case MirrorHorizontal:
                        newBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case UpsideDown:
                        newBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case MirrorVertical:
                        newBitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case MirrorHorizontalAndRotateRight:
                        newBitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case RotateLeft:
                        newBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case MirorHorizontalAndRotateLeft:
                        newBitmap.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case RotateRight:
                        newBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    default:
                        // we should throw exception, because this data is not managed: meanwhile do nothing
                        // throw new NotImplementedException("An orientation of " + orientation + " isn't implemented.");
                        break;
                }
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static string CreateThumbnailFilename(string Fullpath, int width, int height, long quality = 80L)
        {
            FileInfo info = new FileInfo(Fullpath);

            string thumbnailPath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + info.Extension;

            using (Image image = Image.FromFile(Fullpath))
            {
                using (Bitmap resizedImage = ImageHelper.ResizeImage(image, width, height))
                {
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                    ImageCodecInfo jpgEncoder = ImageHelper.GetEncoder(ImageFormat.Jpeg);

                    resizedImage.Save(thumbnailPath, jpgEncoder, myEncoderParameters);
                }
            }

            return thumbnailPath;
        }
    }
}
