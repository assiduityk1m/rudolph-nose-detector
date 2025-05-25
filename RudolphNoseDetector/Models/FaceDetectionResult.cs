using OpenCvSharp;

namespace RudolphNoseDetector.Models
{
    public class FaceDetectionResult
    {
        public Rect FaceRect { get; set; }
        public Point NosePosition { get; set; }
        public double Confidence { get; set; }
        public bool IsValid => FaceRect.Width > 0 && FaceRect.Height > 0;
    }
}