<div align="center">

# 🎬 Dual-View ROI Comparator

### 공간 밀도 히트맵 기반 ROI · 추정 비트량 동기 시각화 도구
### (Dual-View ROI Comparator with Spatial Density Heatmap)

Windows Forms · OpenCvSharp4 · Frame Differencing · Spatial Density Heatmap · raw .yuv (I420)

<p>
<img src="https://img.shields.io/badge/.NET%20Framework-4.8-512BD4"/>
<img src="https://img.shields.io/badge/C%23-7.3-239120"/>
<img src="https://img.shields.io/badge/UI-Windows%20Forms-0078D6"/>
<img src="https://img.shields.io/badge/OpenCvSharp4-4.9.0-orange"/>
<img src="https://img.shields.io/badge/License-MIT-green"/>
<img src="https://img.shields.io/badge/Status-Completed-success"/>
</p>

<!-- 추천: docs/demo.gif 추가 -->
<!-- <img src="docs/demo.gif" width="900"/> -->

</div>

---

## 🏆 핵심 요약

| 항목 | 내용 |
|------|------|
| 입력 | 컨테이너 영상(`.mp4 .avi .mov .mkv .wmv`) + **헤더 없는 raw `.yuv`(I420 4:2:0)** |
| 동시 시각화 | **3-Split View** — 원본 / ROI Overlay / Spatial Density Heatmap |
| ROI 검출 | Frame Differencing → 임계화 → 팽창 → 연결 컴포넌트(4-connectivity) |
| 비트 비교 | 분산 기반 **Estimated Bits**(ROI vs Background) 실시간 그래프 |
| 영속화 | 분석 세션을 **JSON + PNG**로 저장하고 CRUD 관리 |
| 복원력 | OpenCV 네이티브 부재 시 **순수 관리 코드 백엔드로 자동 폴백** |
| 동시성 모델 | 단일 `System.Windows.Forms.Timer` (멀티스레드 미사용) |

---

## 🚀 프로젝트 소개

영상 코덱·영상 분석 학습 과정에서 **"영상의 어디에서 움직임이 발생하는가(Spatial Activity)"** 와
**"그 움직임이 부호화 비용(Coding Cost)에 어떻게 기여하는가"** 를 한 화면에서 동시에 보기는 어렵습니다.
일반 플레이어는 재생만, 분석 도구는 수치만 제공하기 때문에, 학습자는 공간적 활동 영역과 비트 소비
경향을 머릿속에서 따로 결합해야 하는 인지 부하를 안게 됩니다.

본 프로그램은 단일 입력 영상을 **원본 · ROI Overlay · Spatial Density Heatmap · Estimated Bits 그래프**로
동기적으로 제시하여 이 문제를 직접 해소하는 Windows Forms 데스크톱 애플리케이션입니다. 특히 실습에서
헤더가 없어 일반 플레이어로 열 수 없는 raw `.yuv` 시퀀스를 직접 입력받아 분석할 수 있습니다.

### 주요 기술

- Frame Differencing 기반 ROI(움직임 영역) 검출 및 BoundingBox Overlay
- 픽셀 분산(Variance) 기반 Estimated Bits 근사 모델 (ROI vs Background)
- 누적 움직임 밀도의 Spatial Density Heatmap (Blue → Green → Red)
- raw `.yuv`(I420 4:2:0) 디코딩 + 컨테이너 포맷 통합 입력
- Strategy 패턴 기반 OpenCvSharp ↔ 순수 관리 코드 백엔드 자동 전환
- 분석 세션 JSON/PNG 영속화 및 CRUD
- GDI Handle 누수 방지 결정적(Deterministic) 자원 해제

### 전체 파이프라인 (타이머 틱 1회)

```text
ReadNextFrame
    ↓
RoiAnalyzer.Analyze(prev, cur)         (백엔드 자동 선택)
    ↓
Frame Differencing → Threshold → Dilate → Connected Components
    ↓
┌──────────────┬───────────────────┬─────────────────────┐
원본 패널 갱신   ROI Overlay 패널     Heatmap 누적·갱신
              (OrangeRed Box)       (64×36 Grid 정규화)
    ↓
ChartManager.AddPoint(frame, bitsRoi, bitsBg)
    ↓
Seek 바 / 프레임 라벨 갱신  →  (EOF 시 자동 Pause)
```

---

## ⚡ Quick Start

```text
1. Visual Studio 2022에서 DualViewRoiComparator.sln 열기
2. 솔루션 우클릭 → "NuGet 패키지 복원"
3. F5 (디버그 시작)으로 빌드 & 실행
4. 메뉴 [파일] → [영상 열기]로 분석할 영상(.mp4 / .yuv) 선택
5. [재생] → 3-Split View와 Estimated Bits 그래프가 실시간 갱신
```

> raw `.yuv`를 선택하면 헤더가 없으므로 **해상도(Width, Height)와 FPS를 입력**하는 대화상자가 표시됩니다.
> (8비트 I420 / YUV420p 가정)

---

## 🎥 Demo

재생을 시작하면 동일한 프레임 타이밍에 세 패널이 동기적으로 갱신됩니다.

- **원본 패널** — Zoom 모드로 종횡비 유지
- **ROI Overlay** — 움직임 영역에 OrangeRed 2px BoundingBox
- **Heatmap** — 누적 활동 밀도를 Blue(낮음) → Green → Red(높음)로 표현

하단의 **Estimated Bits 그래프**에서 ROI(주황)와 Background(파랑)의 비트 비용이 프레임마다 비교되며,
움직임이 많은 구간에서 ROI 비트 비용이 급증하는 경향을 직접 확인할 수 있습니다.

---

## 🏗️ 시스템 아키텍처

Layered Architecture를 기반으로 **Facade · Strategy · Adapter** 패턴을 적용했습니다. View 계층(`MainForm`)은
이벤트 수집·렌더링·자원 수명만 담당하고, 도메인 로직은 `Core / Heatmap / Persistence` 계층의 독립
클래스가 담당하여 단방향 의존(View → Domain → Infrastructure)을 형성합니다.

```text
┌──────────────────────────────────────────────────────────────┐
│                           MainForm                            │
│   (UI 이벤트 오케스트레이션 · 자원 소유 · 프레임 파이프라인)   │
└───────┬───────────┬────────────┬───────────┬─────────┬────────┘
        │           │            │           │         │
        ▼           ▼            ▼           ▼         ▼
 VideoSource    Playback     RoiAnalyzer   Heatmap   Chart / Session
   Manager      Controller   (Facade)     Accumulator  Manager
        │                        │
        │                        ▼
        │                   IRoiBackend  ◀── Strategy
        │              ┌─────────┴─────────┐
        │      OpenCvRoiBackend     BitmapRoiBackend
        │       (OpenCvSharp)      (managed, LockBits)
        │              └─────────┬─────────┘
        ▼                        ▼
   (Mat → Bitmap)        RoiPostProcessor (공통: 연결요소·분산·비트 추정)
```

| 패턴 | 적용 클래스 | 역할 |
|------|------------|------|
| **Facade** | `RoiAnalyzer` | 백엔드 선택·파라미터 보관을 캡슐화 — View는 `Analyze()` 하나만 호출 |
| **Strategy** | `IRoiBackend` | ROI 검출 알고리즘을 추상화 — OpenCV 미설치 시 자동 교체 |
| **Adapter** | `ChartManager` | Chart 컨트롤 저수준 API를 도메인 친화 메서드로 래핑 |

---

## 📚 상세 문서

아래부터는 환경 설정, 기능 상세, 실행 방법, 데이터 구조, 프로젝트 구조를 포함한 전체 문서입니다.

---

## 1. 환경 설정

### 요구 사항

| 항목 | 내용 |
|------|------|
| OS | Windows 10/11 (x64) |
| IDE | Visual Studio 2022 |
| 프레임워크 | .NET Framework **4.8** (WinForms) |
| 언어 | C# **7.3** |

### NuGet 패키지 (csproj에 명시, 복원만 하면 됨)

| 패키지 | 버전 | 용도 |
|--------|------|------|
| `OpenCvSharp4` | 4.9.0.20240103 | 영상 디코딩 · 프레임 차분 분석 |
| `OpenCvSharp4.runtime.win` | 4.9.0.20240103 | Windows 네이티브 런타임 |
| `OpenCvSharp4.Extensions` | 4.9.0.20240103 | `Mat ↔ Bitmap` 변환 |
| `Newtonsoft.Json` | 13.0.3 | 세션 직렬화(다차원 배열 포함) |

> `System.Windows.Forms.DataVisualization`(Chart)는 .NET Framework에 포함되어 별도 설치가 필요 없습니다.

### 빌드 & 실행

```text
git clone <repo-url>
cd DualViewRoiComparator
# Visual Studio 2022에서 DualViewRoiComparator.sln 열기
# → 솔루션 우클릭 → NuGet 패키지 복원 → F5
```

---

## 2. 주요 기능

### 2.1. 3-Split View 동기 시각화

단일 입력 영상을 ① 원본 frame, ② ROI BoundingBox Overlay, ③ 누적 Spatial Density Heatmap으로
같은 프레임 타이밍에 동시 표시합니다. 원본은 Zoom(종횡비 유지), Heatmap은 StretchImage 모드로 렌더링됩니다.

### 2.2. Estimated Bits 실시간 비교 그래프

매 프레임마다 ROI / Background 영역의 픽셀 분산을 기반으로 추정 비트량을 산출하여 Chart에 플로팅합니다.

```text
EstimatedBits = Variance × BitWeight × √(pixelFactor + 1)
```

ROI(OrangeRed)와 Background(DodgerBlue) 두 Series가 한 그래프에 겹쳐 표시됩니다. 화면에는 **최근 300
포인트**만 렌더링하여 긴 영상에서도 성능을 유지하고, 전체 로그는 내부에 보관하여 세션 저장 시 함께
영속화됩니다.

> ⚠️ 이 값은 실제 코덱 비트스트림이 아닌 **분산 기반 근사(heuristic) 모델**입니다.

### 2.3. raw `.yuv`(I420) 지원 + 백엔드 자동 Fallback

헤더가 없는 raw `.yuv`를 열면 `YuvSpecDialog`가 표시되어 해상도/FPS를 입력받습니다. `RawYuvReader`는
`frameSize = width × height × 3 / 2` 공식으로 프레임 수를 계산하고, 지정 offset에서 버퍼를 읽어
`CvtColor(YUV2BGR_I420)`로 BGR Bitmap을 반환합니다.

OpenCvSharp 네이티브 런타임이 없는 환경에서는 `RoiAnalyzer`가 프로빙 후 `BitmapRoiBackend`(순수 관리
코드, `LockBits` 기반)로 **자동 전환**되어 프로그램이 항상 실행 가능합니다. 현재 백엔드 이름은 상태바에
표시됩니다.

### 2.4. 분석 세션 CRUD 관리

현재 Heatmap Grid와 Chart 로그를 JSON + PNG로 저장(Create)하고, 목록 조회(Read)·수정(Update)·삭제(Delete)할
수 있습니다. 불러오기 시 저장된 Heatmap Grid와 Chart 로그가 복원되어 이전 분석 결과를 재확인할 수
있습니다. 세션 이름의 공백·중복은 `SessionManager`의 `ValidateName`에서 차단됩니다.

### 기타

- 재생 / 일시정지 / 정지 / 이전·다음 프레임 / Seek 슬라이더
- **Threshold(0–255) 실시간 조절** — 슬라이더 조작 즉시 ROI 검출에 반영 (기본값 25)
- 영상 미로드 시 컨트롤 비활성 가드(오조작 방지), 절대 경로 ToolTip
- Seek 직후 기준 프레임 재설정(`resetPrev`)으로 불연속 점프 시 ROI 과검출 방지

---

## 3. 데이터 저장 구조

세션은 실행 파일 폴더 하위 `Sessions/` 디렉터리에 영속화됩니다.

```text
<실행 파일 폴더>/
└── Sessions/
    ├── index.json                 # List<SessionSummary> (목록 캐시)
    ├── 20260614-002648.json        # SessionData (SessionId = 파일명)
    └── 20260614-002648.png         # Heatmap PNG Export
```

`SessionData` 주요 필드:

| 필드 | 타입 | 설명 |
|------|------|------|
| `SessionId` | `string` | `yyyyMMdd-HHmmss` (파일명과 동일) |
| `Name`, `Memo` | `string` | 세션 이름 및 메모 |
| `VideoPath` | `string` | 원본 영상 절대 경로 |
| `CreatedAt`, `UpdatedAt` | `DateTime` | 생성 / 수정 시각 |
| `HeatmapGrid` | `int[,]` | 누적 Heatmap Grid (다차원 배열, Newtonsoft.Json 직렬화) |
| `HeatmapImagePath` | `string` | Export된 PNG 절대 경로 |
| `ChartLogRoi`, `ChartLogBg` | `List<BitPoint>` | ROI / 배경 Estimated Bits 로그(Frame, Bits 쌍) |

손상된 세션 파일은 Skip 후 `index.json`을 재생성하는 복원 로직이 포함되어 있습니다.

---

## 4. 프로젝트 구조

```text
DualViewRoiComparator/
├── DualViewRoiComparator.sln
├── README.md                         이 파일
├── LICENSE                           MIT
├── .gitignore
└── DualViewRoiComparator/
    ├── Program.cs                     STAThread 진입점 + 전역 Exception Handling
    ├── App.config                     .NET Framework 4.8 런타임 지정
    ├── MainForm.cs / .Designer.cs     View + Orchestration, 3-Split Layout
    │
    ├── Core/                          입력 · 분석 도메인
    │   ├── VideoSourceManager.cs      단일 영상/.yuv 로드·읽기·Seek, 처리 해상도 산정
    │   ├── RawYuvReader.cs            I420 raw YUV 디코딩(Mat 반환)
    │   ├── PlaybackController.cs      단일 Timer 래퍼(Tick · SetFps · Play/Pause/Toggle)
    │   ├── RoiAnalyzer.cs             Facade + 백엔드 자동 선택(probing)
    │   ├── IRoiBackend.cs             Strategy 인터페이스
    │   ├── OpenCvRoiBackend.cs        OpenCvSharp (native) 백엔드
    │   ├── BitmapRoiBackend.cs        Managed Bitmap (LockBits) 폴백 백엔드
    │   ├── RoiPostProcessor.cs        공통 Connected Components · Variance · 비트 추정
    │   └── RoiResult.cs              분석 결과 Model + Empty 팩토리
    │
    ├── Heatmap/                       히트맵
    │   ├── HeatmapAccumulator.cs      64×36 Grid binning 누적 · 정규화
    │   └── ColorMapHelper.cs          Blue → Green → Red Piecewise 색상 매핑
    │
    ├── UI/
    │   └── ChartManager.cs            Chart Adapter (ROI / Background Series)
    │
    ├── Persistence/                   세션 영속화
    │   ├── SessionData.cs             SessionData + BitPoint + SessionSummary
    │   ├── SessionManager.cs          JSON+PNG CRUD · Index 관리 · 예외 정규화
    │   └── SessionPersistenceException.cs   도메인 예외
    │
    ├── Forms/                         대화상자
    │   ├── YuvSpecDialog.cs           raw .yuv 해상도/FPS 입력
    │   └── SessionEditForm.cs / .Designer.cs   세션 이름·메모 편집
    │
    └── Properties/
        └── AssemblyInfo.cs
```

### 제어 흐름

```text
Program.Main(STAThread)
    → 전역 Exception Handler 등록 → Application.Run(new MainForm())

MainForm_Load
    → 협력 객체 생성 + Timer.Tick 구독 + RefreshSessionList

[영상 열기]
    → 확장자 분기 (.yuv → YuvSpecDialog + LoadYuv / 그 외 → LoadVideo) → 첫 frame 렌더링

[재생]
    → Timer.Tick → ReadAndProcess → 3패널/그래프 갱신 → (EOF 시 자동 Pause)

[세션 CRUD]
    → SessionManager → ListView 재바인딩

종료(FormClosing)
    → Timer / Capture / Bitmap 결정적 해제
```

---

## 5. 핵심 구현 포인트

| 주제 | 구현 위치 | 핵심 내용 |
|------|----------|----------|
| **단일 스레드 실시간 처리** | `MainForm.ReadAndProcess`, `PlaybackController` | 단일 WinForms Timer가 한 번의 Tick으로 frame 읽기 → 분석 → 3패널 렌더 → 차트·Seek 갱신을 순차 수행 |
| **GDI Handle 누수 방지** | `MainForm` (SetPictureImage) | 매 frame 새 Bitmap 할당 전 이전 `Image`/`Bitmap`을 명시적 `Dispose` |
| **ROI 검출** | `OpenCvRoiBackend`, `RoiPostProcessor` | Grayscale → GaussianBlur(3×3) → AbsDiff → Threshold → Dilate → 4-connectivity Stack 플러드필 → MinBoxArea 필터 |
| **Seek 후 과검출 방지** | `MainForm` | Seek 시 `ReadAndProcess(resetPrev: true)`로 기준 frame 재설정, 첫 결과는 `RoiResult.Empty`로 대체 |
| **Heatmap** | `HeatmapAccumulator`, `ColorMapHelper` | 64×36 Grid 누적 → 최댓값 정규화 → Blue→Green→Red 선형 보간, 0 활동 배경은 dark navy `(0,0,64)` |
| **3중 예외 방어** | `Program`, `SessionManager` | 지역 try-catch → 도메인 예외 정규화 → 전역 Exception Handler |

---

## 6. 한계 및 개선 방향

- **Estimated Bits 정확도** — 현재는 분산 기반 근사 모델. 향후 VVC/HEVC bitstream 파싱 기반 실측 Bits 측정으로 대체 예정.
- **Object Tracking** — 현재는 frame 단위 ROI 검출만 수행. 향후 객체 단위 추적 및 궤적 시각화 추가 예정.
- **Schema Versioning** — 데이터 모델 변경 시 구버전 JSON 호환을 위해 스키마 버전 필드 도입 예정.

---

## 7. 라이선스

- 본 프로젝트 코드 — **MIT License** (`LICENSE` 참조)
- 의존 라이브러리 — OpenCvSharp4(Apache-2.0), Newtonsoft.Json(MIT)는 각 라이선스를 따릅니다.

---

<div align="center">

윈도우프로그래밍 최종 프로젝트 · 소프트웨어학부 · 신효승

</div>
