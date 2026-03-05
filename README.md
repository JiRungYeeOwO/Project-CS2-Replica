🎮 Strike 90 (Unity FPS Project)CS2(Counter-Strike 2)를 레퍼런스로 한 90초 타임 어택 FPS 시스템 모작
📺 시연 영상이미지를 클릭하면 유튜브 시연 영상으로 이동합니다.
🏗️ 시스템 아키텍처 및 설계 의도본 프로젝트는 **'확장 가능한 무기 시스템'**과 **'객체 간 결합도 완화'**를 목표로 설계되었습니다.
1. 계층형 아키텍처 (Layered Architecture)Manager Layer: 게임 흐름 및 전체 시스템 통제Gameplay Layer: 실제 전투 및 물리 로직 (CGun, CBullet 등)UI/Data Layer: 점수 합산 및 정적 데이터 관리
2. 모듈형 총기 시스템 (SRP 적용)CGun 클래스를 퍼사드(Facade)로 두어 각 기능을 컴포넌트 단위로 분리했습니다.CGunFire: 오브젝트 풀링(Queue) 기반 탄환 발사 및 연사 속도 제어CGunRecoil: AnimationCurve를 활용한 절차적 반동 구현CGunSound / CGunParticle: 시청각 피드백 분리
3. 인터페이스를 통한 유연한 확장IWeapon: 다양한 무기(나이프, 투척무기 등) 추가에 유연한 인터페이스 정의IHit: 피격 대상을 추상화하여 공격자와 피격자 간의 의존성 제거
🖼️ 주요 설계 자료 (PPT 슬라이드)클래스 구조도총기 모듈화 상세
📝 알려진 이슈 및 개선 사항 (Known Issues)맵 텍스처 백화 현상: 외부 에셋(Sketchfab) 임포트 시 URP 환경에서의 쉐이더 호환성 이슈로 인해 맵 아틀라스 매핑이 비정상적으로 출력되는 현상이 있습니다. 현재 쉐이더 재할당 및 UV 리베이킹을 통한 기술적 수정을 계획 중입니다.
자막 가독성: 시연 영상 내 자막 속도가 다소 빠른 문제는 본 README의 기술 명세로 보완하고 있습니다.
📜 Credits
This project uses the following assets:
"De_Dust 2 with real light" (https://skfb.ly/oq6KL) by Neo_minigan is licensed under Creative Commons Attribution.
"AK47 Counter Strike 2" (https://skfb.ly/oII8N) by blazitt is licensed under Creative Commons Attribution.
"Cs2 Terrorist" (https://skfb.ly/oLLuY) by Toast is licensed under Creative Commons Attribution.
"Low - High poly bullet with sleeve (game ready)" (https://skfb.ly/osHoU) by JakOuNien is licensed under Creative Commons Attribution.
