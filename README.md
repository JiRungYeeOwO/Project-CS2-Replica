# 🎮 Strike 90 (Unity FPS Project)
> **CS2(Counter-Strike 2)를 레퍼런스로 한 90초 타임 어택 FPS 시스템 모작**

## 📺 프로젝트 자료 및 영상
* **상세 발표 자료 (Google Drive)**: [미니프로젝트_김승원.pptx](https://docs.google.com/presentation/d/1j5Di-CkP02YThjiQ7nPpfkwEprKaWgPj/edit?usp=sharing&ouid=113630341990529298262&rtpof=true&sd=true) 
* **시연 영상**:

[![Strike_90_Demo](http://img.youtube.com/vi/vxjFW06ClMk/0.jpg)](https://www.youtube.com/watch?v=vxjFW06ClMk)

  *이미지를 클릭하면 유튜브 시연 영상으로 이동합니다.*

---

## 🏗️ 시스템 설계 및 기술적 특징
본 프로젝트는 **확장 가능한 무기 시스템**과 **객체 간 결합도 완화**를 목표로 설계되었습니다. 자세한 내용은 상단의 **발표 자료**를 확인해 주세요.

### 1. 계층형 아키텍처 (Layered Architecture)
- **Manager Layer**: 게임 흐름 및 전체 시스템 통제
- **Gameplay Layer**: 실제 전투 및 물리 로직
- **UI/Data Layer**: 플레이어 UI 및 정적 데이터 관리

### 2. 모듈형 총기 시스템 (SRP 적용)
`CGun` 클래스를 퍼사드로 설계하여 각 기능을 컴포넌트 단위로 분리했습니다.
- **사격 로직**: 오브젝트 풀링 기반 탄환 발사 제어
- **절차적 반동**: AnimationCurve를 활용한 리얼한 반동 구현
- **피드백 분리**: 사운드 및 파티클 시스템의 독립 모듈화

### 3. 인터페이스를 통한 유연한 확장
- **무기 인터페이스**: 다양한 무기 타입 확장을 위한 규격 정의
- **피격 시스템 추상화**: 공격자와 피격자 간 의존성 제거

---

## 📝 알려진 이슈 (Known Issues)
- **맵 텍스처 백화 현상**: URP 환경에서의 쉐이더 호환성 이슈로 인해 맵 아틀라스 매핑 오류 발생. 현재 기술적 수정을 계획 중입니다.

---

## 📜 Credits
This project uses the following assets:

* **"De_Dust 2 with real light"** ([https://skfb.ly/oq6KL](https://skfb.ly/oq6KL)) by Neo_minigan
* **"AK47 Counter Strike 2"** ([https://skfb.ly/oII8N](https://skfb.ly/oII8N)) by blazitt
* **"Cs2 Terrorist"** ([https://skfb.ly/oLLuY](https://skfb.ly/oLLuY)) by Toast
* **"Low - High poly bullet with sleeve (game ready)"** ([https://skfb.ly/osHoU](https://skfb.ly/osHoU)) by JakOuNien
