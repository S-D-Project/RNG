# GitHub PLAN 관찰 수집기

이 도구는 이슈를 읽고 PLAN 후보·중복·보완 대상을 분류하며 도구 전용 기록에 관찰 결과를 저장한다. 기본 주기는 60초다. 자동 본문 갱신·인수와 실제 AO 전달·배정은 차단돼 있다. [승인된 PLAN](../../Docs/Plans/plan-20261111-8d2dcd9d-github-issue-polling-automation.md)은 로컬 원본이며 현재 active다.

## 지원 범위와 차단

| 경로 | 현재 결과 |
|---|---|
| gh 전체 페이지 GET, PR 제외, PLAN 파싱·로컬 원본 대조 | 구현, 실제 저장소 dry-run 확인 |
| SQLite 관찰·인수 의도·논리 배정·시도·전달 기록, OS 잠금 | 구현, 임시 상태 저장소에서 집중 시험 |
| 신규 ID 예약, 인수·전달·배정·재개·응답 유실 복구 게이트 | 전송 대역을 사용하는 계약 시험 통과; 실제 외부 효과 미실행 |
| GitHub 본문 갱신·ID 원격 초기화·active 전환 | P0-1 미충족으로 비활성, 변경 API 미구현 |
| 실제 ao send/spawn, 워커 작업 시작·재개, HEAD/워크트리 증명 | P0-3 실제 계약 미입증으로 비활성 |
| 불명확한 재시도·DB 복원·잠금 강제 회수 | 실행 명령·승인 우회 옵션 없음; D4 사용자 건별 승인 필요 |
| 안전한 시험 PLAN과 상시 자동 인수 | 미실행, 선행 조건 통과 및 D5 후속 결정 대기 |

CLI의 정상 종료는 관찰·분류 성공을 뜻한다. 자동 인수 성공이나 PLAN 전체 완료를 뜻하지 않는다. 출력의 automatic_intake와 external_writes는 항상 false다. 설정으로 외부 효과 차단을 해제할 수 없다.

## 선행 검증 근거

- **P0-1:** GitHub 공식 문서는 별도 명시가 없는 PATCH 등 변경 요청의 조건부 처리를 지원하지 않는다고 설명한다. [조건부 요청](https://docs.github.com/en/rest/using-the-rest-api/best-practices-for-using-the-rest-api#use-conditional-requests-if-appropriate), [이슈 갱신](https://docs.github.com/en/rest/issues/issues#update-an-issue). 따라서 GET → PATCH → 재조회나 임의 If-Match 헤더를 CAS로 사용하지 않는다.
- GitHub의 읽기 전용 GraphQL introspection으로 UpdateIssueInput 필드를 조회했다. body와 clientMutationId 등은 있으나 기대 본문·버전 일치 입력은 없었다. 이 조회는 query였으며 mutation은 수행하지 않았다. 문서와 스키마로 차단 근거를 확인할 수 있어 별도 fixture 이슈를 만들지 않았다.
- **P0-3:** 설치 AO 0.14.10의 CLI 도움말·동봉 send/spawn 안내와 실제 세션 조회를 확인했다. 일반 send에는 내구성 있는 재전달 식별 계약이 없고, client-message-id/recover-only는 steering용이다. spawn은 branch를 받지만 기준 SHA·멱등 키·실행 전 대기 옵션이 입증되지 않았다. 세션 조회는 ID·프로젝트·역할·브랜치를 제공하지만 워크트리/HEAD 검증 영수증을 제공한다는 계약은 확인하지 못했다.
- 위 AO 계약을 실제 새 세션으로 시험하지 않았다. GitHub 인수 관문이 닫힌 상태에서 기존 작업을 깨울 필요가 없으며, 모의 백엔드의 기능을 실제 AO 기능으로 간주하지 않는다.
- 프로젝트의 trackerIntake는 활성 설정이 없는 상태로 조회됐으며 동봉 프로젝트 안내의 기본값은 비활성이다. 프로젝트 설정·데몬 환경은 변경하지 않았다.

## 설치와 순수 dry-run

Python 3.13, gh CLI 인증, 동일 Windows 계정의 AO 실행이 필요하다. Unity 실행·빌드·패키지 변경은 필요하지 않다. venv와 상태 파일은 모든 Git 워크트리 밖에 둔다.

아래 경로는 운영자가 선택하는 예시다. 이 작업에서는 temp venv만 만들었으며 운영 venv·예약 작업·상시 수집기를 설치하지 않았다.

~~~powershell
$taskVenv = Join-Path $env:LOCALAPPDATA 'RNG\PlanIntake\venv'
python -m venv $taskVenv
$taskPython = Join-Path $taskVenv 'Scripts\python.exe'
& $taskPython -m pip install -r Tools/PlanIntake/requirements.txt
& $taskPython -B Tools/PlanIntake/main.py --repo S-D-Project/RNG --project rng inspect
~~~

inspect는 DB·바인딩을 생성하거나 갱신하지 않는다. GitHub GET과 AO 조회만 수행한다. 오케스트레이터는 프로젝트·역할·활성 상태로 조회하며 특정 세션 ID를 영구 고정하지 않는다. --orchestrator로 기대 ID를 지정해도 실제 조회를 통과해야 한다.

오프라인 시험은 전체 페이지를 합친 이슈 JSON 배열을 temp에 준비하고 다음과 같이 실행한다. --skip-ao는 inspect에서 조회를 생략할 뿐 외부 효과를 허용하지 않는다.

~~~powershell
& $taskPython -B Tools/PlanIntake/main.py --repo S-D-Project/RNG inspect --input "$env:TEMP\rng-issues.json" --skip-ao
~~~

## 파싱·승인 내용·증거

- PLAN metadata 제목 아래 단일 yaml 코드 블록에 id/status/current_step/source 네 문자열 필드가 모두 있어야 한다. 별칭·앵커·태그·중복 키·복수 블록·누락·다중 문서는 보완 대상이다. 본문은 UTF-8 64 KiB, 메타데이터는 8 KiB와 제한된 토큰 수를 상한으로 둔다.
- 작업 내용·문제 내용·리팩토링 내용·목표와 범위 중 하나와 완료 조건 영역이 각각 하나 있고 내용이 있어야 한다. 기존 저장소의 Feature/Bugfix/Refactor/Documents 양식에 맞춘다. source의 다른 원본은 자동 덮어쓰지 않는다.
- 다른 필드가 보완 대상이어도 일관되게 읽은 메타데이터의 유일한 유효 ID는 오류 관찰·중복 색인에 보존한다. 중복 ID 키처럼 식별이 모호한 경우는 ID를 임의 선택하지 않는다. 필수 작업·완료 조건 영역이 관리 영역과 겹치면 승인 내용이 해시에서 빠지지 않도록 거절한다.
- 정확히 ready인 후보만 분류한다. 라벨·담당자·GitHub open/closed는 PLAN 상태를 대신하지 않는다. 기존 active·완료 기록·이관 전 로컬 source·ID 변경은 신규 인수하지 않는다.
- 승인 해시는 제목과 메타데이터·명시적 PLAN progress/PLAN intake 관리 영역을 제외한 본문에서 만든다. 본문 끝의 구분용 줄바꿈만 정리하며 목표 텍스트 내부의 공백·문구 변경은 감지한다. 이 구분은 도구의 계약이며 공통 저장소 지침은 수정하지 않았다.
- D1의 신규 증거는 사용자의 신규 선언과 전체 기록 대조를 참조하는 신뢰된 로컬 기록이다. 이슈 생성자 이름만으로 현재 본문의 작성자·승인자를 추정하지 않는다. inspect --evidence의 JSON 배열 필드는 issue_url, body_hash, submitted_by, confirmed_at(시간대 포함), reference, comparison_complete=true, new_submission=true다. 본문 버전이 다르거나 대조가 끝나지 않으면 초기화 후보로 분류하지 않는다.
- 분류가 initialization_required여도 현재는 ID 원격 초기화·인수를 하지 않는다. ID 예약·재시작 시 재사용은 게이트의 격리 계약 시험에서만 확인했다.
- D3 재개 코어는 승인 참조·현재 본문 버전·이전 담당자·기존 실행 정리 증거를 요구하고, 소비한 승인 참조는 재사용하지 않는다. 기존 인수 토큰·논리 키를 유지한다. 실제 사용자 승인 수집·AO 워커 재개는 CLI에 연결하지 않았다.

## 관찰 기록과 시작·중지

관찰 기록이 필요할 때만 운영 계정에서 신규 설치를 명시적으로 초기화한다. 기본 경로는 LOCALAPPDATA/RNG/PlanIntake/state/저장소이며, 별도 bindings 폴더에 저장소별 고정 경로·세대·감사 체인의 마지막 확인값을 둔다. --state-dir는 모든 등록 워크트리 밖의 절대 경로여야 하며 이미 바인딩한 경로를 임의 변경할 수 없다.

~~~powershell
& $taskPython -B Tools/PlanIntake/main.py --repo S-D-Project/RNG --project rng init-state
& $taskPython -B Tools/PlanIntake/main.py --repo S-D-Project/RNG --project rng poll --cycles 1
& $taskPython -B Tools/PlanIntake/main.py --repo S-D-Project/RNG --project rng status
~~~

init-state는 비어 있는 신규 설치만 허용한다. DB·신원·바인딩의 유실, 기존/부분 설치, 과거 DB 복원·경로 불일치를 새 설치로 처리하지 않는다. 별도 바인딩은 DB 백업 디렉터리 밖에 둔다. 상태 DB와 바인딩을 모두 과거 사본으로 교체한 경우는 로컬 자료만으로 탐지할 수 없으므로 모든 복원은 D4 승인·외부 전체 대조를 필요로 한다.

연속 관찰은 poll --cycles 0 --interval 60으로 실행하고 Ctrl+C 또는 종료 신호로 중지한다. 전체 페이지를 생성 순서로 매번 읽는다. since 필터로 범위를 줄이지 않아 매 조회가 전체 범위를 중첩하며, 체크포인트는 완전한 조회 결과의 기록이다. 마지막 상태가 계속 ready인 이슈의 재발견을 다루며 폴링 사이의 모든 순간적 상태 변경을 관찰한다고 보장하지 않는다.

GitHub의 x-poll-interval·Retry-After·rate limit reset과 지수 지연을 존중한다. 읽기 오류의 다음 허용 시각을 영속 저장하므로 재시작도 대기 시간을 우회하지 않는다. 연속 5회 읽기 실패 시 종료한다. 기본 1회 실행의 오류는 즉시 종료하며 종료 코드는 성공 0, 오류·보류 2다. 긴 대기는 짧게 나누어 중지 가능하게 한다.

Windows 작업 스케줄러를 사용할 경우 다음 조건으로 **관찰 전용** 작업을 운영자가 검토한다. 이번 PR에서는 작업을 등록하거나 켜지 않았다.

| 설정 | 적용 조건 |
|---|---|
| 계정 | gh 인증·AO를 사용하는 단일 Windows 계정; 다계정·다중 호스트 운영은 지원 범위 밖 |
| 동작 | venv Python 절대 경로, main.py 절대 경로와 --repo/--project 및 poll --cycles 0 인수 |
| 시작 위치 | 도구가 있는 워크트리; 새 버전 적용 시 작업 중지 후 경로·버전 확인 |
| 트리거 | 사용자 로그온 이후 AO 데몬·기존 오케스트레이터가 사용 가능할 때 |
| 중복 실행 | 새 인스턴스 시작 금지와 저장소별 커널 잠금을 함께 사용 |
| 실패·복구 | 종료 코드·JSON 오류 확인, 최소 1분 후 재시작, 영속 지연·기존 기록 유지 |
| PC 조건 | 전원·절전 해제·네트워크 유지. 중단 중 즉시 수집을 보장하지 않고 다음 전체 조회로 대조 |

잠금 파일을 삭제하거나 TTL/PID만 보고 회수하지 않는다. 정상 종료·프로세스 종료 후 커널이 잠금을 해제한다. SQL 커밋 직후 확인 파일 쓰기만 중단된 경우에는 감사 체인의 일치하는 접두부가 입증될 때 확인값만 전진시킨다. 이는 DB 복원·외부 요청 재시도가 아니며 이력은 삭제하지 않는다.

## D4 복구 운영

최종 복구 책임자는 사용자다. 운영자는 조사·증거 수집·보류를 맡는다. 불명확한 재시도·DB 복원·잠금 회수는 대상 PLAN·배정/시도 ID·원본 버전·조회 결과·진행 중 요청·제안 조치를 제출하고 **사용자의 건별 명시 승인 후** 수행한다. 승인 후에도 실패 확정·지연 실행 부재 등 안전 조건이 확인되지 않으면 보류한다.

- 성공 응답을 잃었지만 사전 상관 키에 대응하는 기존 실행이 정확히 하나 입증되면 기존 배정에 연결한다. 현재 실제 AO에서는 필요한 증명 계약이 미입증이므로 모의 시험 결과만 있다.
- 조회 0개는 실패 증명이 아니다. 복수 일치도 자동 선택·종료하지 않는다. 새 spawn, ready 역전환, 기록 삭제로 해소하지 않는다.
- DB가 없거나 과거 사본이면 init-state를 다시 실행해 지우지 않는다. DB·신원·별도 바인딩과 AO/GitHub의 대조 근거를 보존하고 사용자의 복구 승인을 기다린다. 이 PR은 DB 복원·잠금 회수 실행 명령을 제공하지 않는다.
- 사용자 승인 참조를 입력 모델에 담는 것과 실제 사용자 신원을 인증하는 것은 별개다. 실제 승인 수집 통합은 미지원이며 CLI에 --approved 같은 우회 옵션을 제공하지 않는다.

## 집중 테스트와 확인 결과

~~~powershell
& $taskPython -B -m unittest discover -s Tools/PlanIntake/tests -v
~~~

초기 검증은 Windows Python 3.13.16에서 54개를 통과했고, 1차 리뷰의 오류 ID 보존·실제 Refactor 양식·관리 영역 중첩 회귀를 추가한 뒤 58개를 통과했다. CI는 같은 집중 시험을 Windows·Ubuntu Python 3.13에서 수행한다. 로컬 결과를 다른 OS의 통과로 표현하지 않는다.

| 검증 층 | 수행 결과 |
|---|---|
| 순수 파서·분류 | 상태·오류 0회 인수, 신규 증거, 같은 ID·기존 로컬 원본·완료 상태 보존 |
| 전송 대역을 쓰는 게이트 | ready 1회 인수, 동일 논리 작업 중복 차단, 의도적 복수 하위 작업, 승인 재개·참조 재사용 차단 |
| 장애 주입 | active 직후 중단, send/spawn 응답 유실·지연 성공·SQL 확인 실패, 0/복수 대조 보류, source 편집·취소·HEAD 불일치 |
| 실제 OS/임시 SQLite | 다중 프로세스 잠금·동시 게이트, DB 유실·과거 복원 탐지, 롤백·확인 파일 중단·지속 backoff |
| 실제 저장소 읽기 | 전체 이슈 15개(PR 제외), 모두 metadata 보완; #15/#27/#28 무배정, 기존 오케스트레이터 조회 |
| 미실행 | GitHub 변경 요청·새 fixture 이슈, AO send/spawn·새 시험 세션, 스케줄러 등록·상시 실행, Unity 빌드 |

## D5 후속 결정과 PR

사용자 선택은 아직 없다. 임시로 기존 수동 인수 절차와 관찰 도구를 사용하거나, 안전한 갱신 경로를 확보하기 위한 별도 설계를 검토할 수 있다. 후자는 이번 PR에 추가 구현하지 않는다. 어느 경우에도 원자적 조건을 낮추거나 구성 값으로 자동 인수를 켜지 않는다.

현재 결과는 draft PR로 검토하며 원래 자동 인수·분배 목표는 미완료다. 실제 외부 계약·안전한 시험·사용자 선택·코드리뷰·필요 PR의 사용자 merge를 확인하기 전 PLAN을 completed로 바꾸지 않는다. 공통 지침 변경은 없으며 지침 후보는 PLAN에 남겨 둔다.
