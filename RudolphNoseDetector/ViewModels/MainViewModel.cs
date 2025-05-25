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
        private readonly IFaceDetectionService _faceDetectionService;
        private BitmapImage? _originalImage;
        private BitmapImage? _processedImage;
        private string _statusMessage = "이미지를 선택하거나 웹캠을 시작하세요.";
        private bool _isProcessing = false;

        public MainViewModel()
        {
            _faceDetectionService = new FaceDetectionService();
            LoadImageCommand = new RelayCommand(ExecuteLoadImage);
            ProcessImageCommand = new RelayCommand(ExecuteProcessImage, CanProcessImage);
            StartWebcamCommand = new RelayCommand(ExecuteStartWebcam);
            ClearCommand = new RelayCommand(ExecuteClear);
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

        #endregion

        #region Commands

        public RelayCommand LoadImageCommand { get; }
        public RelayCommand ProcessImageCommand { get; }
        public RelayCommand StartWebcamCommand { get; }
        public RelayCommand ClearCommand { get; }

        #endregion

        #region Command Implementations

        private void ExecuteLoadImage(object? parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "이미지 파일 선택",
                Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp;*.tiff|모든 파일|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    OriginalImage = bitmap;
                    ProcessedImage = null;
                    StatusMessage = $"이미지 로드 완료: {Path.GetFileName(openFileDialog.FileName)}";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"이미지 로드 실패: {ex.Message}";
                }
            }
        }

        private async void ExecuteProcessImage(object? parameter)
        {
            if (OriginalImage == null) return;

            IsProcessing = true;
            StatusMessage = "얼굴 검출 중...";

            try
            {
                await Task.Run(() =>
                {
                    // BitmapImage를 Mat로 변환
                    using var originalMat = BitmapConverter.BitmapImageToMat(OriginalImage);
                    using var processedMat = originalMat.Clone();

                    // 얼굴 검출
                    var faces = _faceDetectionService.DetectFaces(originalMat);

                    if (faces.Any())
                    {
                        foreach (var face in faces)
                        {
                            // 루돌프 코 추가
                            _faceDetectionService.AddRudolphNose(processedMat, face.NosePosition);

                            // 얼굴 영역 표시 (선택사항)
                            Cv2.Rectangle(processedMat, face.FaceRect, new Scalar(0, 255, 0), 2);
                        }

                        // 결과 이미지 변환
                        ProcessedImage = BitmapConverter.MatToBitmapImage(processedMat);
                        StatusMessage = $"처리 완료: {faces.Count}개의 얼굴에서 루돌프 코 적용";
                    }
                    else
                    {
                        ProcessedImage = BitmapConverter.MatToBitmapImage(originalMat);
                        StatusMessage = "얼굴을 찾을 수 없습니다.";
                    }
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"처리 오류: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private bool CanProcessImage(object? parameter)
        {
            return OriginalImage != null && !IsProcessing;
        }

        private void ExecuteStartWebcam(object? parameter)
        {
            StatusMessage = "웹캠 기능은 현재 개발 중입니다.";
            // TODO: 웹캠 기능 구현
        }

        private void ExecuteClear(object? parameter)
        {
            OriginalImage = null;
            ProcessedImage = null;
            StatusMessage = "이미지가 초기화되었습니다.";
        }

        #endregion

        public void Dispose()
        {
            _faceDetectionService.Dispose();
        }
        
        
    }
}