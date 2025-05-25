using OpenCvSharp;
using RudolphNoseDetector.Models;
using System.Collections.Generic;
using System;
using System.IO;

namespace RudolphNoseDetector.Services
{
    public class FaceDetectionService : IFaceDetectionService, IDisposable
    {
        private readonly CascadeClassifier _faceCascade;
        private bool _disposed = false;

        public FaceDetectionService()
        {
            // Haar Cascade 파일 경로 (OpenCV에 내장된 것 사용)
            _faceCascade = new CascadeClassifier();
            
            // 기본 얼굴 검출기 로드
            if (!_faceCascade.Load("haarcascade_frontalface_alt.xml"))
            {
                // 대안 경로들 시도
                var possiblePaths = new[]
                {
                    @"C:\opencv\data\haarcascades\haarcascade_frontalface_alt.xml",
                    @"haarcascade_frontalface_alt.xml",
                    @"opencv_data\haarcascade_frontalface_alt.xml"
                };

                bool loaded = false;
                foreach (var path in possiblePaths)
                {
                    if (_faceCascade.Load(path))
                    {
                        loaded = true;
                        break;
                    }
                }

                if (!loaded)
                {
                    throw new FileNotFoundException("Haar Cascade 파일을 찾을 수 없습니다.");
                }
            }
        }

        public List<FaceDetectionResult> DetectFaces(Mat image)
        {
            var results = new List<FaceDetectionResult>();
            
            try
            {
                // 그레이스케일 변환
                using var grayImage = new Mat();
                Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
                
                // 얼굴 검출
                var faces = _faceCascade.DetectMultiScale(
                    grayImage,
                    scaleFactor: 1.1,
                    minNeighbors: 3,
                    flags: HaarDetectionTypes.ScaleImage,
                    minSize: new Size(30, 30)
                );

                foreach (var face in faces)
                {
                    var nosePos = DetectNosePosition(image, face);
                    results.Add(new FaceDetectionResult
                    {
                        FaceRect = face,
                        NosePosition = nosePos,
                        Confidence = 0.8 // Haar Cascade는 confidence를 제공하지 않으므로 기본값
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"얼굴 검출 오류: {ex.Message}");
            }

            return results;
        }

        public Point DetectNosePosition(Mat image, Rect faceRect)
        {
            // 간단한 추정: 얼굴 중앙 하단 부분을 코로 가정
            var noseX = faceRect.X + faceRect.Width / 2;
            var noseY = faceRect.Y + (int)(faceRect.Height * 0.6); // 얼굴의 60% 지점

            return new Point(noseX, noseY);
        }

        public Mat AddRudolphNose(Mat image, Point nosePosition, int radius = 15)
        {
            // 붉은색 원 그리기 (루돌프 코)
            Cv2.Circle(image, nosePosition, radius, new Scalar(0, 0, 255), -1);
            
            // 하이라이트 효과 (선택사항)
            Cv2.Circle(image, new Point(nosePosition.X - 5, nosePosition.Y - 5), 
                      radius / 3, new Scalar(100, 100, 255), -1);

            return image;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _faceCascade?.Dispose();
                _disposed = true;
            }
        }
    }
}