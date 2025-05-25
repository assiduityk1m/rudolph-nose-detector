using Microsoft.Win32;
using OpenCvSharp;
using RudolphNoseDetector.Helpers;
using RudolphNoseDetector.Services;
using System.IO;
using System.Windows.Media.Imaging;
using System;
using System.Threading.Tasks;
using System.Linq;



namespace RudolphNoseDetector.ViewModels
{
    public class MainViewModel : BaseViewModel, IDisposable
    {
        #region AS-IS
        //private readonly IFaceDetectionService _faceDetectionService;
        //private BitmapImage? _originalImage;
        //private BitmapImage? _processedImage;
        //private string _statusMessage = "이미지를 선택하거나 웹캠을 시작하세요.";
        //private bool _isProcessing = false;

        //// Haar Cascade 파일 경로
        //public string HaarCascadeFilePath { get; private set; }

        //public MainViewModel()
        //{
        //    _faceDetectionService = new FaceDetectionService();
        //    LoadImageCommand = new RelayCommand(ExecuteLoadImage);
        //    ProcessImageCommand = new RelayCommand(ExecuteProcessImage, CanProcessImage);
        //    StartWebcamCommand = new RelayCommand(ExecuteStartWebcam);
        //    ClearCommand = new RelayCommand(ExecuteClear);

        //    // Haar Cascade 파일 경로 설정
        //    HaarCascadeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_alt.xml");

        //    if (!File.Exists(HaarCascadeFilePath))
        //    {
        //        throw new FileNotFoundException($"The required file 'haarcascade_frontalface_alt.xml' was not found at {HaarCascadeFilePath}. Please ensure it is placed in the application's base directory.");
        //    }
        //}


        //#region Properties

        //public BitmapImage? OriginalImage
        //{
        //    get => _originalImage;
        //    set => SetProperty(ref _originalImage, value);
        //}

        //public BitmapImage? ProcessedImage
        //{
        //    get => _processedImage;
        //    set => SetProperty(ref _processedImage, value);
        //}

        //public string StatusMessage
        //{
        //    get => _statusMessage;
        //    set => SetProperty(ref _statusMessage, value);
        //}

        //public bool IsProcessing
        //{
        //    get => _isProcessing;
        //    set => SetProperty(ref _isProcessing, value);
        //}

        //#endregion

        //#region Commands

        //public RelayCommand LoadImageCommand { get; }
        //public RelayCommand ProcessImageCommand { get; }
        //public RelayCommand StartWebcamCommand { get; }
        //public RelayCommand ClearCommand { get; }

        //#endregion

        //#region Command Implementations

        //private void ExecuteLoadImage(object? parameter)
        //{
        //    var openFileDialog = new OpenFileDialog
        //    {
        //        Title = "이미지 파일 선택",
        //        Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp;*.tiff|모든 파일|*.*",
        //        FilterIndex = 1
        //    };

        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        try
        //        {
        //            var bitmap = new BitmapImage();
        //            bitmap.BeginInit();
        //            bitmap.UriSource = new Uri(openFileDialog.FileName);
        //            bitmap.CacheOption = BitmapCacheOption.OnLoad;
        //            bitmap.EndInit();
        //            bitmap.Freeze();

        //            OriginalImage = bitmap;
        //            ProcessedImage = null;
        //            StatusMessage = $"이미지 로드 완료: {Path.GetFileName(openFileDialog.FileName)}";
        //        }
        //        catch (Exception ex)
        //        {
        //            StatusMessage = $"이미지 로드 실패: {ex.Message}";
        //        }
        //    }
        //}

        //private async void ExecuteProcessImage(object? parameter)
        //{
        //    if (OriginalImage == null) return;

        //    IsProcessing = true;
        //    StatusMessage = "얼굴 검출 중...";

        //    try
        //    {
        //        await Task.Run(() =>
        //        {
        //            // BitmapImage를 Mat로 변환
        //            using var originalMat = BitmapConverter.BitmapImageToMat(OriginalImage);
        //            using var processedMat = originalMat.Clone();

        //            // 얼굴 검출
        //            var faces = _faceDetectionService.DetectFaces(originalMat);

        //            if (faces.Any())
        //            {
        //                foreach (var face in faces)
        //                {
        //                    // 루돌프 코 추가
        //                    _faceDetectionService.AddRudolphNose(processedMat, face.NosePosition);

        //                    // 얼굴 영역 표시 (선택사항)
        //                    Cv2.Rectangle(processedMat, face.FaceRect, new Scalar(0, 255, 0), 2);
        //                }

        //                // 결과 이미지 변환
        //                ProcessedImage = BitmapConverter.MatToBitmapImage(processedMat);
        //                StatusMessage = $"처리 완료: {faces.Count}개의 얼굴에서 루돌프 코 적용";
        //            }
        //            else
        //            {
        //                ProcessedImage = BitmapConverter.MatToBitmapImage(originalMat);
        //                StatusMessage = "얼굴을 찾을 수 없습니다.";
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        StatusMessage = $"처리 오류: {ex.Message}";
        //    }
        //    finally
        //    {
        //        IsProcessing = false;
        //    }
        //}

        //private bool CanProcessImage(object? parameter)
        //{
        //    return OriginalImage != null && !IsProcessing;
        //}

        //private void ExecuteStartWebcam(object? parameter)
        //{
        //    StatusMessage = "웹캠 기능은 현재 개발 중입니다.";
        //    // TODO: 웹캠 기능 구현
        //}

        //private void ExecuteClear(object? parameter)
        //{
        //    OriginalImage = null;
        //    ProcessedImage = null;
        //    StatusMessage = "이미지가 초기화되었습니다.";
        //}

        //#endregion

        //public void Dispose()
        //{
        //    _faceDetectionService.Dispose();
        //}
        #endregion

        #region TO-BE
        private readonly IFaceDetectionService _faceDetectionService;
        private BitmapImage? _originalImage;
        private BitmapImage? _processedImage;
        private string _statusMessage = "이미지를 선택하거나 웹캠을 시작하세요.";
        private bool _isProcessing = false;
        private int _noseSize = 25;
        private int _detectedFaceCount = 0;
        private bool _showFaceRectangles = true; // 얼굴 영역 표시 여부
        private int _faceCount = 0; // 검출된 얼굴 수

        public MainViewModel()
        {
            try
            {
                _faceDetectionService = new FaceDetectionService();
                InitializeCommands();
                StatusMessage = "애플리케이션이 준비되었습니다.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"초기화 오류: {ex.Message}";
            }
        }

        private void InitializeCommands()
        {
            LoadImageCommand = new RelayCommand(ExecuteLoadImage);
            ProcessImageCommand = new RelayCommand(ExecuteProcessImage, CanProcessImage);
            StartWebcamCommand = new RelayCommand(ExecuteStartWebcam);
            ClearCommand = new RelayCommand(ExecuteClear);
            SaveImageCommand = new RelayCommand(ExecuteSaveImage, CanSaveImage);
        }

        #region Properties

        public BitmapImage? OriginalImage
        {
            get => _originalImage;
            set => SetProperty(ref _originalImage, value);
        }

        public BitmapImage? ProcessedImage
        {
            get => _processedImage;
            set => SetProperty(ref _processedImage, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        /// <summary>
        /// 루돌프 코 크기 (픽셀)
        /// </summary>
        public int NoseSize
        {
            get => _noseSize;
            set => SetProperty(ref _noseSize, value);

            //get => _noseSize;
            //set
            //{
            //    if (SetProperty(ref _noseSize, value))
            //    {
            //        // 코 크기가 변경되면 자동으로 다시 처리
            //        if (OriginalImage != null && !IsProcessing)
            //        {
            //            ExecuteProcessImage(null);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 얼굴 영역 사각형 표시 여부
        /// </summary>
        public bool ShowFaceRectangles
        {
            get => _showFaceRectangles;
            set => SetProperty(ref _showFaceRectangles, value);
        }

        /// <summary>
        /// 검출된 얼굴 수
        /// </summary>
        public int FaceCount
        {
            get => _faceCount;
            set => SetProperty(ref _faceCount, value);
        }

        public int DetectedFaceCount
        {
            get => _detectedFaceCount;
            set => SetProperty(ref _detectedFaceCount, value);
        }

        #endregion

        #region Commands

        public RelayCommand LoadImageCommand { get; private set; }
        public RelayCommand ProcessImageCommand { get; private set; }
        public RelayCommand StartWebcamCommand { get; private set; }
        public RelayCommand ClearCommand { get; private set; }
        public RelayCommand SaveImageCommand { get; private set; }

        #endregion

        #region Command Implementations

        private void ExecuteLoadImage(object? parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "이미지 파일 선택",
                Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp;*.tiff;*.gif|모든 파일|*.*",
                FilterIndex = 1,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // 메모리 효율적인 이미지 로딩
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.DecodePixelWidth = 800; // 메모리 절약을 위한 크기 제한
                    bitmap.EndInit();
                    bitmap.Freeze();

                    OriginalImage = bitmap;
                    ProcessedImage = null;
                    DetectedFaceCount = 0;

                    StatusMessage = $"이미지 로드 완료: {Path.GetFileName(openFileDialog.FileName)} ({bitmap.PixelWidth}x{bitmap.PixelHeight})";

                    // 자동으로 얼굴 검출 시작
                    //ExecuteProcessImage(null);
                }
                catch (Exception ex)
                {
                    StatusMessage = $"이미지 로드 실패: {ex.Message}";
                    OriginalImage = null;
                    ProcessedImage = null;
                }
            }
        }

        private async void ExecuteProcessImage(object? parameter)
        {
            if (OriginalImage == null) return;

            IsProcessing = true;
            StatusMessage = "🎅 얼굴을 찾는 중...";
            FaceCount = 0;

            try
            {
                await Task.Run(() =>
                {
                    // BitmapImage를 Mat로 변환
                    using var originalMat = BitmapConverter.BitmapImageToMat(OriginalImage);
                    using var processedMat = originalMat.Clone();

                    // 얼굴 검출
                    var faces = _faceDetectionService.DetectFaces(originalMat);
                    FaceCount = faces.Count;

                    if (faces.Any())
                    {
                        foreach (var face in faces)
                        {
                            // 루돌프 코 추가 (사용자 설정 크기 적용)
                            _faceDetectionService.AddRudolphNose(processedMat, face.NosePosition, NoseSize);

                            // 얼굴 영역 표시 (옵션)
                            if (ShowFaceRectangles)
                            {
                                Cv2.Rectangle(processedMat, face.FaceRect, new Scalar(0, 255, 0), 2);
                            }
                        }

                        // 결과 이미지 변환
                        ProcessedImage = BitmapConverter.MatToBitmapImage(processedMat);
                        StatusMessage = $"🎄 완료! {faces.Count}개의 얼굴에 루돌프 코 적용됨";
                    }
                    else
                    {
                        ProcessedImage = BitmapConverter.MatToBitmapImage(originalMat);
                        StatusMessage = "😞 얼굴을 찾을 수 없습니다. 다른 이미지를 시도해보세요.";
                    }
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ 처리 오류: {ex.Message}";
                FaceCount = 0;
            }
            finally
            {
                IsProcessing = false;
            }
            //if (OriginalImage == null || IsProcessing) return;

            //IsProcessing = true;
            //StatusMessage = "얼굴 검출 및 루돌프 코 적용 중...";

            //try
            //{
            //    var result = await Task.Run(() =>
            //    {
            //        // BitmapImage를 Mat로 변환
            //        using var originalMat = BitmapConverter.BitmapImageToMat(OriginalImage);
            //        using var processedMat = originalMat.Clone();

            //        Console.WriteLine($"이미지 크기: {originalMat.Width}x{originalMat.Height}");

            //        // 얼굴 검출
            //        var faces = _faceDetectionService.DetectFaces(originalMat);
            //        Console.WriteLine($"검출된 얼굴 수: {faces.Count}");

            //        if (faces.Any())
            //        {
            //            foreach (var face in faces)
            //            {
            //                // 얼굴 크기에 비례한 코 크기 계산
            //                var adaptiveNoseSize = Math.Max(NoseSize, face.FaceRect.Width / 8);

            //                // 루돌프 코 추가
            //                _faceDetectionService.AddRudolphNose(processedMat, face.NosePosition, adaptiveNoseSize);

            //                // 얼굴 영역 표시 (반투명 테두리)
            //                Cv2.Rectangle(processedMat, face.FaceRect, new Scalar(0, 255, 0), 2);

            //                // 신뢰도 표시
            //                var confidenceText = $"{face.Confidence:P0}";
            //                Cv2.PutText(processedMat, confidenceText,
            //                    new Point(face.FaceRect.X, face.FaceRect.Y - 10),
            //                    HersheyFonts.HersheySimplex, 0.5, new Scalar(0, 255, 0), 1);
            //            }

            //            return new { Image = BitmapConverter.MatToBitmapImage(processedMat), FaceCount = faces.Count };
            //        }
            //        else
            //        {
            //            return new { Image = BitmapConverter.MatToBitmapImage(originalMat), FaceCount = 0 };
            //        }
            //    });

            //    ProcessedImage = result.Image;
            //    DetectedFaceCount = result.FaceCount;

            //    if (result.FaceCount > 0)
            //    {
            //        StatusMessage = $"처리 완료: {result.FaceCount}개의 얼굴에 루돌프 코 적용 (코 크기: {NoseSize}px)";
            //    }
            //    else
            //    {
            //        StatusMessage = "얼굴을 찾을 수 없습니다. 이미지를 다른 각도로 시도해보세요.";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    StatusMessage = $"처리 오류: {ex.Message}";
            //    Console.WriteLine($"상세 오류: {ex}");
            //}
            //finally
            //{
            //    IsProcessing = false;
            //}
        }

        private bool CanProcessImage(object? parameter)
        {
            return OriginalImage != null && !IsProcessing;
        }

        private void ExecuteStartWebcam(object? parameter)
        {
            StatusMessage = "웹캠 기능은 현재 개발 중입니다. 곧 업데이트될 예정입니다.";
            // TODO: 웹캠 기능 구현
        }

        private void ExecuteClear(object? parameter)
        {
            OriginalImage = null;
            ProcessedImage = null;
            DetectedFaceCount = 0;
            StatusMessage = "이미지가 초기화되었습니다.";
        }

        private void ExecuteSaveImage(object? parameter)
        {
            if (ProcessedImage == null) return;

            var saveFileDialog = new SaveFileDialog
            {
                Title = "처리된 이미지 저장",
                Filter = "PNG 이미지|*.png|JPEG 이미지|*.jpg|BMP 이미지|*.bmp",
                DefaultExt = "png",
                FileName = $"rudolph_nose_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var encoder = GetEncoder(Path.GetExtension(saveFileDialog.FileName));
                    encoder.Frames.Add(BitmapFrame.Create(ProcessedImage));

                    using var stream = new FileStream(saveFileDialog.FileName, FileMode.Create);
                    encoder.Save(stream);

                    StatusMessage = $"이미지 저장 완료: {Path.GetFileName(saveFileDialog.FileName)}";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"이미지 저장 실패: {ex.Message}";
                }
            }
        }

        private BitmapEncoder GetEncoder(string extension)
        {
            return extension.ToLower() switch
            {
                ".png" => new PngBitmapEncoder(),
                ".jpg" or ".jpeg" => new JpegBitmapEncoder { QualityLevel = 95 },
                ".bmp" => new BmpBitmapEncoder(),
                _ => new PngBitmapEncoder()
            };
        }

        private bool CanSaveImage(object? parameter)
        {
            return ProcessedImage != null;
        }

        #endregion

        public void Dispose()
        {
            try
            {
                _faceDetectionService?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dispose 오류: {ex.Message}");
            }
        }

        #endregion

    }
}