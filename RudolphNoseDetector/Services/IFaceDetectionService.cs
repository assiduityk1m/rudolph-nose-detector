using OpenCvSharp;
using RudolphNoseDetector.Models;
using System.Collections.Generic;

namespace RudolphNoseDetector.Services
{
    public interface IFaceDetectionService
    {
        List<FaceDetectionResult> DetectFaces(Mat image);
        Point DetectNosePosition(Mat image, Rect faceRect);
        Mat AddRudolphNose(Mat image, Point nosePosition, int radius = 15);
        
        void Dispose();
    }
}