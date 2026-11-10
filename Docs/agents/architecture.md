# 코드 구조와 책임

구현 위치를 정하거나 책임, 의존성, 객체 수명을 변경할 때 적용한다.

## 책임 위치

| 경로 | 현재 책임 |
|---|---|
| `Assets/Game/Scripts/Core/` | 공통 기반 클래스, 씬 로딩 도구, 충돌 영역 도구 |
| `Assets/Game/Scripts/Data/` | 데이터 로딩과 변환, 리소스 정의, 기본 데이터와 실행 상태, 무기 제작 에디터 |
| `Assets/Game/Scripts/Gameflow/` | 부팅, 데이터 저장소, 씬 전환, 스테이지와 웨이브 진행 |
| `Assets/Game/Scripts/Gameplay/` | 플레이어, 적, 무기와 공격, 공간 검색, 맵 생성 |
| `Assets/Game/Scripts/Presentation/` | UI, 팝업, 로딩 화면과 카메라 |
| `Assets/Scenes/` | 실행 씬과 컴포넌트 연결 |
| `Assets/Game/Prefabs/`, `Assets/Game/SO/` | 게임 오브젝트 구성과 직렬화된 설정 |

## 의존성과 초기화

- `Bootstrap`은 공통 시스템과 데이터를 초기화하고 타이틀로 진입한다.
- `StageInitializer`는 플레이어, 적, 공격, 카메라, 맵과 스테이지 진행의 참조를 연결한다.
- 의존성은 Inspector 참조, 초기화 메서드와 싱글턴 접근으로 연결되어 있다. 기존 연결과 수명을 확인하고 변경한다.
- `Data`에는 `PlayerRuntime`, `EnemyRuntime` 같은 `MonoBehaviour`와 실행 상태가 있다. 무기 리소스 설정은 `Gameplay`의 실행 객체를 생성한다.
- 현재 프로젝트 코드는 전역 namespace와 Unity 기본 어셈블리를 사용한다.

## 변경 기준

- 비슷한 기능과 호출 경로를 찾아 기존 책임 소유자를 확장한다.
- 객체의 생성, 초기화, 사용과 해제를 담당하는 위치를 명확히 한다.
- 책임 이동, 공용 인터페이스, namespace와 어셈블리 변경 시 호출자와 직렬화 에셋의 영향을 확인한다.
- 새 공유 객체나 추상화는 실제 사용처와 책임을 근거로 도입한다.
- 승인된 변경 범위에 맞춰 구조를 변경하고, 관련 문서의 변경은 [guideline-maintenance.md](guideline-maintenance.md)를 따른다.
