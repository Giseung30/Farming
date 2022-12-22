## Farming

+ **프로젝트 소개**
  + 각기 다른 능력을 갖춘 세 가지 영웅 중 하나를 선택하여 최대한 많은 농작물을 처치하는 게임이다.

- **프로젝트 계기**
  - 학과 내에서 주최하는 **2020년도 소프트웨어 전시회**에 출품하기 위해서 프로젝트를 수행하게 되었다. 평소에 AOS 장르 게임을 많이 즐겼던 터라 플레이 경험을 바탕으로 개발하고자 했다.
  
+ **프로젝트 기간**
  + 2019.09~2019.11
    
- **프로젝트 기획**
  - 유저 스타일에 따라 플레이할 수 있도록 세 가지의 영웅을 개발한다.
  - 영웅이 기본 공격과 스킬, 궁극기를 사용할 수 있도록 구현한다.
  - 스킬 사용 여부를 파악하기 위해 특수 효과(Particle system) 기능을 적극적으로 활용한다.
  - 게임 진행 시간, 적 생성 주기 등 전체적인 게임의 흐름을 총괄하는 스크립트를 작성한다.
  - 각 영웅과 적의 종류가 다양하므로 부모 클래스를 기반으로 파생된 자식 클래스들이 존재한다.
  - 부모 클래스의 추상 함수를 통해 영웅과 적 오브젝트가 상호 작용한다.
  - 유저들의 실력을 객관적으로 판단할 수 있는 랭킹 기능을 구현한다.
  
+ **개발 환경**
  + 개발 툴 : **Unity 2019.2.10f1**
  + 이미지 편집 : **Adobe Photoshop**
  + 오디오 편집 : **GoldWave**
  + 애니메이션 편집 : **3ds Max**
  
- **프로젝트 평가**
  - 최적화를 위해 스크립트를 최대한 독립적으로 분리했고, 컴포넌트를 캐싱해서 사용했다.
  - 필요한 리소스를 찾을 수 없을 때, 리소스 제작 툴을 공부해야겠다는 생각이 들었다.
  - 이전 전시회에 비해 개발 시간이 단축되어서 피드백을 반영할 시간이 있었다. 이 기간에 이펙트와 맵 디테일을 향상시키고, 부드러운 카메라 움직임을 구현하였으며, 밸런스 조절을 하였다.
  - 조작감을 향상시켜서 모바일 버전으로 출시하면 좋을 것 같다.
  - 이러한 큰 프로젝트를 경험해볼수록 많은 것을 배운다는 것을 항상 느낀다.
  - 전시회는 1위로 마무리했다.
