# 🎮 Strike 90 (Unity FPS Project)
> **CS2(Counter-Strike 2)를 레퍼런스로 한 90초 타임 어택 FPS 시스템 모작**

## 📺 프로젝트 자료 및 영상
* [cite_start]**상세 발표 자료 (Google Drive)**: [미니프로젝트_김승원.pptx 바로가기](https://drive.google.com/open?id=1j5Di-CkP02YThjiQ7nPpfkwEprKaWgPj) 
* **시연 영상 (YouTube)**: [![Strike_90_Demo](http://img.youtube.com/vi/유튜브_영상_아이디/0.jpg)](https://www.youtube.com/watch?v=유튜브_영상_아이디)
  *이미지를 클릭하면 유튜브 시연 영상으로 이동합니다.*

---

## 🏗️ 시스템 설계 및 기술적 특징
본 프로젝트는 **'확장 가능한 무기 시스템'**과 **'객체 간 결합도 완화'**를 목표로 설계되었습니다. [cite_start]자세한 클래스 구조와 레이어 설계는 상단의 **발표 자료(PPTX)**를 통해 확인하실 수 있습니다. 

### 1. 계층형 아키텍처 (Layered Architecture)
* [cite_start]**Manager Layer**: 게임 흐름 및 전체 시스템 통제 (씬 흐름, 게임 플로우 매니저) 
* [cite_start]**Gameplay Layer**: 실제 전투 및 물리 로직 (플레이어 컨트롤러, 총기 작동, 적 AI) 
* [cite_start]**UI/Data Layer**: 플레이어 UI, 결과 화면 및 정적 데이터를 활용한 정보 저장 

### 2. 모듈형 총기 시스템 (SRP 적용)
`CGun` 클래스를 퍼사드(Facade)로 두어 각 기능을 컴포넌트 단위로 분리했습니다.
* **사격 로직**: 오브젝트 풀링(`Queue`) 기반 탄환 발사 및 연사 속도 제어
* **절차적 반동**: `AnimationCurve`를 활용한 리얼한 반동 구현
* **피드백 분리**: 사운드 및 파티클 시스템을 독립된 모듈로 관리

### 3. 인터페이스를 통한 유연한 확장
* [cite_start]**무기 인터페이스**: 다양한 무기 타입 확장을 위한 공통 규격 정의 
* **피격 시스템 추상화**: 공격자와 피격자 간의 의존성을 제거하여 유지보수성 향상

---

## 📝 알려진 이슈 및 개선 사항 (Known Issues)
* **맵 텍스처 백화 현상**: 외부 에셋(Sketchfab) 임포트 시 URP 환경에서의 쉐이더 호환성 이슈로 인해 맵 아틀라스 매핑이 비정상적으로 출력되는 현상이 있습니다. 현재 쉐이더 재할당 및 UV 리베이킹을 통한 기술적 수정을 계획 중입니다.
* **자막 가독성**: 시연 영상 내 자막 속도가 다소 빠른 문제는 본 README의 기술 명세로 보완하고 있습니다.

---

## 📜 Credits
This project uses the following assets:

* **"De_Dust 2 with real light"** ([https://skfb.ly/oq6KL](https://skfb.ly/oq6KL)) by Neo_minigan
* **"AK47 Counter Strike 2"** ([https://skfb.ly/oII8N](https://skfb.ly/oII8N)) by blazitt
* **"Cs2 Terrorist"** ([https://skfb.ly/oLLuY](https://skfb.ly/oLLuY)) by Toast
* **"Low - High poly bullet with sleeve (game ready)"** ([https://skfb.ly/osHoU](https://skfb.ly/osHoU)) by JakOuNien
