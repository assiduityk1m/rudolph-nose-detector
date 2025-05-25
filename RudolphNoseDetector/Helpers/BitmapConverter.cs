using OpenCvSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;


namespace RudolphNoseDetector.Helpers
{
    public static class BitmapConverter
    {
        public static BitmapImage MatToBitmapImage(Mat mat)
        {
            using var bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat);
            using var memory = new MemoryStream();

            bitmap.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = memory;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        public static Mat BitmapImageToMat(BitmapImage bitmapImage)
        {
            using var outStream = new MemoryStream();

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(outStream);

            var bitmap = new Bitmap(outStream);
            return OpenCvSharp.Extensions.BitmapConverter.ToMat(bitmap);
        }
    }
}