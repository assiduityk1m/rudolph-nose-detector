using OpenCvSharp;
using RudolphNoseDetector.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DlibDotNet;
using DlibDotNet.Extensions;
using System.Drawing; // Point, Size 등

namespace RudolphNoseDetector.Services
{
    #region AS-IS
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
                    @"opencv_data\haarcascade_frontalface_alt.xml",
                    @"C:\Users\assid\rudolph-nose-detector\RudolphNoseDetector\Resources\haarcascade_frontalface_alt.xml"
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

        //OpenCVSharp를 사용한 얼굴 검출
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
                    minSize: new OpenCvSharp.Size(30, 30)
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

        public OpenCvSharp.Point DetectNosePosition(Mat image, Rect faceRect)
        {
            // 간단한 추정: 얼굴 중앙 하단 부분을 코로 가정
            var noseX = faceRect.X + faceRect.Width / 2;
            var noseY = faceRect.Y + (int)(faceRect.Height * 0.6); // 얼굴의 60% 지점

            return new OpenCvSharp.Point(noseX, noseY);
        }

        public Mat AddRudolphNose(Mat image, OpenCvSharp.Point nosePosition, int radius = 15)
        {
            //// 붉은색 원 그리기 (루돌프 코)
            //Cv2.Circle(image, nosePosition, radius, new Scalar(0, 0, 255), -1);

            //// 하이라이트 효과 (선택사항)
            //Cv2.Circle(image, new Point(nosePosition.X - 5, nosePosition.Y - 5), 
            //          radius / 3, new Scalar(100, 100, 255), -1);

            var dynamicRadius = Math.Max(15, radius);

            // 메인 루돌프 코 (더 큰 빨간 원)
            Cv2.Circle(image, nosePosition, dynamicRadius, new Scalar(0, 0, 220), -1, LineTypes.AntiAlias);

            // 외곽선 추가 (더 진한 빨간색)
            Cv2.Circle(image, nosePosition, dynamicRadius, new Scalar(0, 0, 180), 3, LineTypes.AntiAlias);

            // 하이라이트 효과 (왼쪽 위)
            var highlightPos = new OpenCvSharp.Point(nosePosition.X - dynamicRadius / 3, nosePosition.Y - dynamicRadius / 3);
            var highlightRadius = Math.Max(5, dynamicRadius / 3);
            Cv2.Circle(image, highlightPos, highlightRadius, new Scalar(100, 100, 255), -1, LineTypes.AntiAlias);

            // 추가 광택 효과 (더 작은 하이라이트)
            var smallHighlightPos = new OpenCvSharp.Point(nosePosition.X - dynamicRadius / 5, nosePosition.Y - dynamicRadius / 5);
            var smallHighlightRadius = Math.Max(2, dynamicRadius / 6);
            Cv2.Circle(image, smallHighlightPos, smallHighlightRadius, new Scalar(200, 200, 255), -1, LineTypes.AntiAlias);

            return image;
        }

        // 얼굴 크기에 따른 동적 루돌프 코 크기 계산
        public Mat AddDynamicRudolphNose(Mat image, OpenCvSharp.Point nosePosition, Rect faceRect)
        {
            // 얼굴 크기 기반 동적 크기 계산
            var faceSize = Math.Min(faceRect.Width, faceRect.Height);
            var dynamicRadius = Math.Max(12, faceSize / 8); // 얼굴 크기의 1/8

            return AddRudolphNose(image, nosePosition, dynamicRadius);
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
    #endregion

    #region TO-BE
    //public class FaceDetectionService : IFaceDetectionService, IDisposable
    //{
    //    private readonly CascadeClassifier _faceCascade;
    //    private readonly CascadeClassifier _noseCascade;
    //    private readonly CascadeClassifier _eyeCascade;
    //    private bool _disposed = false;

    //    public FaceDetectionService()
    //    {
    //        // 여러 Cascade 분류기 초기화
    //        _faceCascade = new CascadeClassifier();
    //        _noseCascade = new CascadeClassifier();
    //        _eyeCascade = new CascadeClassifier();

    //        // 얼굴 검출기 로드 (더 정확한 모델 사용)
    //        LoadCascadeClassifier(_faceCascade, "haarcascade_frontalface_default.xml", "얼굴");

    //        // 코 검출기 로드 (선택사항)
    //        try
    //        {
    //            LoadCascadeClassifier(_noseCascade, "haarcascade_mcs_nose.xml", "코");
    //        }
    //        catch
    //        {
    //            // 코 검출기가 없어도 계속 진행
    //        }

    //        // 눈 검출기 로드 (코 위치 추정에 도움)
    //        try
    //        {
    //            LoadCascadeClassifier(_eyeCascade, "haarcascade_eye.xml", "눈");
    //        }
    //        catch
    //        {
    //            // 눈 검출기가 없어도 계속 진행
    //        }
    //    }

    //    private void LoadCascadeClassifier(CascadeClassifier classifier, string filename, string description)
    //    {
    //        var possiblePaths = new[]
    //        {
    //            filename,
    //            Path.Combine("opencv_data", filename),
    //            Path.Combine("data", "haarcascades", filename),
    //            Path.Combine(Environment.CurrentDirectory, filename)
    //        };

    //        // GitHub에서 직접 다운로드 URL들
    //        var downloadUrls = new Dictionary<string, string>
    //        {
    //            ["haarcascade_frontalface_default.xml"] = "https://raw.githubusercontent.com/opencv/opencv/master/data/haarcascades/haarcascade_frontalface_default.xml",
    //            ["haarcascade_frontalface_alt.xml"] = "https://raw.githubusercontent.com/opencv/opencv/master/data/haarcascades/haarcascade_frontalface_alt.xml",
    //            ["haarcascade_mcs_nose.xml"] = "https://raw.githubusercontent.com/opencv/opencv/master/data/haarcascades/haarcascade_mcs_nose.xml",
    //            ["haarcascade_eye.xml"] = "https://raw.githubusercontent.com/opencv/opencv/master/data/haarcascades/haarcascade_eye.xml"
    //        };

    //        bool loaded = false;

    //        // 로컬 파일들 먼저 시도
    //        foreach (var path in possiblePaths)
    //        {
    //            if (File.Exists(path) && classifier.Load(path))
    //            {
    //                loaded = true;
    //                Console.WriteLine($"{description} 검출기 로드 성공: {path}");
    //                break;
    //            }
    //        }

    //        // 로컬에 없으면 다운로드 시도
    //        if (!loaded && downloadUrls.ContainsKey(filename))
    //        {
    //            try
    //            {
    //                using var client = new System.Net.Http.HttpClient();
    //                var xmlContent = client.GetStringAsync(downloadUrls[filename]).Result;
    //                File.WriteAllText(filename, xmlContent);

    //                if (classifier.Load(filename))
    //                {
    //                    loaded = true;
    //                    Console.WriteLine($"{description} 검출기 다운로드 및 로드 성공");
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                Console.WriteLine($"{description} 검출기 다운로드 실패: {ex.Message}");
    //            }
    //        }

    //        if (!loaded)
    //        {
    //            throw new FileNotFoundException($"{description} Haar Cascade 파일을 찾거나 로드할 수 없습니다: {filename}");
    //        }
    //    }

    //    public List<FaceDetectionResult> DetectFaces(Mat image)
    //    {
    //        var results = new List<FaceDetectionResult>();

    //        try
    //        {
    //            // 그레이스케일 변환
    //            using var grayImage = new Mat();
    //            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

    //            // 히스토그램 균등화로 조명 개선
    //            using var equalizedImage = new Mat();
    //            Cv2.EqualizeHist(grayImage, equalizedImage);

    //            using var resized = new Mat();
    //            Cv2.Resize(image, resized, new Size(image.Width / 2, image.Height / 2));


    //            // 얼굴 검출 (더 정확한 파라미터 사용)
    //            //var faces = _faceCascade.DetectMultiScale(
    //            //    equalizedImage,
    //            //    scaleFactor: 1.05,      // 더 세밀한 스케일
    //            //    minNeighbors: 5,        // 더 엄격한 검증
    //            //    flags: HaarDetectionTypes.ScaleImage,
    //            //    minSize: new Size(50, 50),  // 최소 크기 증가
    //            //    maxSize: new Size(300, 300) // 최대 크기 제한
    //            //);

    //            var faces = _faceCascade.DetectMultiScale(
    //                equalizedImage,
    //                scaleFactor: 1.10,               // 스케일 단계를 넓혀 과검출 방지
    //                minNeighbors: 7,                 // 검출 신뢰도 강화
    //                flags: HaarDetectionTypes.ScaleImage,
    //                minSize: new Size(50, 50),     // 너무 작은 얼굴은 무시
    //                maxSize: new Size(300, 300)      // 필요에 따라 조절
    //            );

    //            Console.WriteLine($"검출된 얼굴 수: {faces.Length}");

    //            foreach (var face in faces)
    //            {
    //                var nosePos = DetectNosePosition(equalizedImage, face);
    //                var confidence = CalculateFaceConfidence(equalizedImage, face);

    //                results.Add(new FaceDetectionResult
    //                {
    //                    FaceRect = face,
    //                    NosePosition = nosePos,
    //                    Confidence = confidence
    //                });
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"얼굴 검출 오류: {ex.Message}");
    //        }

    //        return results;
    //    }

    //    public Point DetectNosePosition(Mat image, Rect faceRect)
    //    {
    //        try
    //        {
    //            // 얼굴 영역만 추출
    //            using var faceRegion = new Mat(image, faceRect);

    //            // 실제 코 검출 시도
    //            if (_noseCascade != null)
    //            {
    //                var noses = _noseCascade.DetectMultiScale(
    //                    faceRegion,
    //                    scaleFactor: 1.1,
    //                    minNeighbors: 3,
    //                    minSize: new Size(10, 10)
    //                );

    //                if (noses.Length > 0)
    //                {
    //                    var nose = noses[0]; // 첫 번째 검출된 코 사용
    //                    return new Point(
    //                        faceRect.X + nose.X + nose.Width / 2,
    //                        faceRect.Y + nose.Y + nose.Height / 2
    //                    );
    //                }
    //            }

    //            // 눈을 이용한 코 위치 추정
    //            if (_eyeCascade != null)
    //            {
    //                var eyes = _eyeCascade.DetectMultiScale(
    //                    faceRegion,
    //                    scaleFactor: 1.1,
    //                    minNeighbors: 3,
    //                    minSize: new Size(10, 10)
    //                );

    //                if (eyes.Length >= 2)
    //                {
    //                    // 두 눈의 중점 아래쪽을 코로 추정
    //                    var eye1Center = new Point(eyes[0].X + eyes[0].Width / 2, eyes[0].Y + eyes[0].Height / 2);
    //                    var eye2Center = new Point(eyes[1].X + eyes[1].Width / 2, eyes[1].Y + eyes[1].Height / 2);

    //                    var eyesCenterX = (eye1Center.X + eye2Center.X) / 2;
    //                    var eyesCenterY = (eye1Center.Y + eye2Center.Y) / 2;

    //                    return new Point(
    //                        faceRect.X + eyesCenterX,
    //                        faceRect.Y + eyesCenterY + (int)(faceRect.Height * 0.25)
    //                    );
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"코 위치 검출 오류: {ex.Message}");
    //        }

    //        // 기본 추정: 얼굴 중앙 하단
    //        var noseX = faceRect.X + faceRect.Width / 2;
    //        var noseY = faceRect.Y + (int)(faceRect.Height * 0.65); // 65% 지점으로 조정

    //        return new Point(noseX, noseY);
    //    }

    //    public Mat AddRudolphNose(Mat image, Point nosePosition, int radius = 25)
    //    {
    //        try
    //        {
    //            // 더 큰 루돌프 코 그리기
    //            var baseRadius = Math.Max(radius, 20); // 최소 20픽셀

    //            // 메인 코 (진한 빨간색)
    //            Cv2.Circle(image, nosePosition, baseRadius, new Scalar(0, 0, 200), -1);

    //            // 테두리 (더 진한 빨간색)
    //            Cv2.Circle(image, nosePosition, baseRadius, new Scalar(0, 0, 150), 3);

    //            // 하이라이트 (왼쪽 위)
    //            var highlightPos = new Point(
    //                nosePosition.X - baseRadius / 3,
    //                nosePosition.Y - baseRadius / 3
    //            );
    //            Cv2.Circle(image, highlightPos, baseRadius / 3, new Scalar(100, 100, 255), -1);

    //            // 작은 하이라이트 (더 밝게)
    //            var smallHighlightPos = new Point(
    //                nosePosition.X - baseRadius / 4,
    //                nosePosition.Y - baseRadius / 4
    //            );
    //            Cv2.Circle(image, smallHighlightPos, baseRadius / 6, new Scalar(200, 200, 255), -1);

    //            // 그림자 효과 (오른쪽 아래)
    //            var shadowPos = new Point(
    //                nosePosition.X + baseRadius / 4,
    //                nosePosition.Y + baseRadius / 4
    //            );
    //            Cv2.Circle(image, shadowPos, baseRadius / 4, new Scalar(0, 0, 120), -1);

    //            Console.WriteLine($"루돌프 코 적용 완료: 위치({nosePosition.X}, {nosePosition.Y}), 반지름: {baseRadius}");
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"루돌프 코 적용 오류: {ex.Message}");
    //        }

    //        return image;
    //    }

    //    private double CalculateFaceConfidence(Mat grayImage, Rect faceRect)
    //    {
    //        try
    //        {
    //            // 간단한 신뢰도 계산 (얼굴 영역의 대비 기반)
    //            using var faceRegion = new Mat(grayImage, faceRect);

    //            var mean = new Scalar();
    //            var stddev = new Scalar();
    //            Cv2.MeanStdDev(faceRegion, out mean, out stddev);

    //            // 표준편차가 클수록 더 선명한 얼굴 (높은 신뢰도)
    //            var confidence = Math.Min(stddev.Val0 / 50.0, 1.0);
    //            return Math.Max(confidence, 0.3); // 최소 30% 신뢰도
    //        }
    //        catch
    //        {
    //            return 0.5; // 기본값
    //        }
    //    }

    //    public void Dispose()
    //    {
    //        if (!_disposed)
    //        {
    //            _faceCascade?.Dispose();
    //            _noseCascade?.Dispose();
    //            _eyeCascade?.Dispose();
    //            _disposed = true;
    //            GC.SuppressFinalize(this);
    //        }
    //    }
    //}
    #endregion
}