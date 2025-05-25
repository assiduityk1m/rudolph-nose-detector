# 🎄 Rudolph Nose Detector Made By Kim YoungHwan

## 프로젝트 개요
웹캠 또는 이미지에서 얼굴을 자동으로 검출하고, 검출된 얼굴의 코 부위에 루돌프의 빨간 코를 합성하는 WPF 애플리케이션입니다. (이미지만 구현됌)

## 🛠 기술 스택
- **Language**: C# (.NET 8.0)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM (Model-View-ViewModel) 패턴
- **Computer Vision**: OpenCV 4.11
- **Face Detection**: Haar Cascade Classifier
- **Development Environment**: Microsoft Visual Studio, GitHub Codespaces

## ✨ 주요 기능
- 이미지 파일 로드 (JPG, PNG, BMP, TIFF 지원)
- 실시간 얼굴 검출
- 코 위치 자동 인식
- 루돌프 코 효과 적용
- 처리 전후 이미지 비교 표시
- 직관적인 사용자 인터페이스

## 실행방법

1. 애플리케이션 시작 후
- "이미지 선택" 버튼 클릭
- 얼굴이 포함된 이미지 파일 선택 (JPG, PNG, BMP 등)
- "얼굴 검출" 버튼 클릭
- 처리 결과 확인 (오른쪽 패널에 루돌프 코가 적용된 이미지 표시)

2. 지원 파일 형식
- JPG, JPEG, PNG, BMP, TIFF

3. 기능 설명
- 이미지 선택: 로컬 파일에서 이미지 로드
- 얼굴 검출: 선택된 이미지에서 얼굴 찾기 및 루돌프 코 적용
- 웹캠 시작: 실시간 처리 (개발 중)
- 초기화: 이미지 초기화

## 🎯 주의사항
- Windows 환경에서만 실행 가능: WPF는 Windows 전용 기술
- Haar Cascade 파일 필수: 얼굴 검출을 위해 반드시 다운로드 필요
- 이미지 크기 제한: 너무 큰 이미지는 처리 시간이 오래 걸릴 수 있음
- 얼굴 검출 정확도: 조명과 각도에 따라 검출 성능 차이 발생 가능
