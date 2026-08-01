# Hero Knight

Unity로 만든 2D 액션 플랫포머. 스테이지마다 몰려오는 적을 처치하고, 콤보를 이어가며 최고 기록에 도전합니다.

**▶ [플레이하기](https://lkm0222.itch.io/hero-knight)** — 브라우저에서 바로 실행됩니다

> ⌨️ **키보드 전용 / 데스크톱 권장** — 모바일 브라우저에서는 조작할 수 없습니다.

---

## 조작

| 키 | 동작 |
|---|---|
| `←` `→` | 이동 |
| `X` | 점프 (2단 점프) |
| `C` | 공격 |
| `↓` | 방어 |
| `Z` | 구르기 |
| `A` | 회복 |
| `D` | 러시 어택 |
| `S` | 다운 스매시 (공중에서) |

`Z` `A` `D` `S`는 마나를 소모하며 쿨타임이 있습니다.

## 게임 개요

- **스테이지 진행** — 스테이지마다 정해진 적이 소환되고, 전멸시키면 다음 스테이지로 넘어갑니다
- **적 2종** — 근접형 고블린, 원거리 투사체를 쏘는 이블 위저드
- **콤보 시스템** — 연속 타격이 끊기지 않으면 콤보가 누적되고, 최고 콤보와 최고 스테이지가 기록으로 남습니다

## 구현

| 항목 | 내용 |
|---|---|
| **오브젝트 풀링** | `EnemyPoolManager`가 적을 타입별 큐로 관리. 스테이지 전환마다 생성·파괴하지 않고 재사용 |
| **입력** | New Input System (`PlayerAction.inputactions`). `PlayerInput` 메시지 방식으로 `OnMove`/`OnJump` 등 수신 |
| **스킬** | `SkillController`가 쿨타임과 마나 소비를 스킬 데이터로 분리 관리. 사용 실패 시 원인(쿨타임/마나 부족)을 구분해 반환 |
| **스테이지 진행** | `StageManager`의 코루틴이 `소환 → 전멸 대기 → 다음 스테이지`를 순차 처리 |
| **타격 연출** | Cinemachine `CinemachineImpulseSource`로 피격 시 화면 흔들림 |
| **공격 판정** | `Physics2D.Raycast`를 레이어 마스크와 함께 사용해 전방 사거리 내 적을 판정 |
| **매니저 구조** | 제네릭 싱글턴 `MonoSingleton<T>`를 상속한 Game / Sound / Stage / EnemyPool 매니저 |

### WebGL 빌드 최적화

배포 과정에서 빌드 리포트로 용량을 측정하고 병목을 제거했습니다.

| 대상 | 조치 | 결과 |
|---|---|---|
| BGM (22MB WAV) | Vorbis 압축 + `Compressed In Memory`로 전환 | 빌드 내 1.9MB |
| TMP 폰트 아틀라스 6개 | Static 4096×4096 → Dynamic 1024×1024 | **99.4MB → 약 6MB** |

전체 에셋 121.8MB → 30.2MB, **최종 다운로드 약 17MB**.

WebGL 환경에 맞춰 브라우저 자동재생 정책(BGM을 사용자 입력 시점에 재생)과 `PlayerPrefs` 영속성(IndexedDB flush를 위한 명시적 `Save()`)도 함께 처리했습니다.

## 개발 환경

- Unity **6000.3.9f1**
- Universal Render Pipeline (2D Renderer)
- Cinemachine 3.1 / Input System 1.18 / TextMesh Pro

## 에셋 출처

직접 제작하지 않은 리소스의 출처입니다. 상세 목록은 [`Assets/무료 에셋 출처.txt`](Assets/무료%20에셋%20출처.txt) 참조.

| 분류 | 출처 |
|---|---|
| 폰트 | [Mona12](https://noonnu.cc/font_page/1792) |
| 플레이어 | [Hero Knight - Pixel Art](https://assetstore.unity.com/packages/2d/characters/hero-knight-pixel-art-165188) |
| 몬스터 | [Monsters Creatures Fantasy](https://assetstore.unity.com/packages/2d/characters/monsters-creatures-fantasy-167949), [Evil Wizard](https://assetstore.unity.com/packages/2d/characters/evil-wizard-168007) |
| 효과음 | [효과음 연구소](https://soundeffect-lab.info/) |
| BGM | [Udio](https://www.udio.com/) (AI 생성) |
