미니게임 제작 실습 프로젝트
=============
***
_UnityEngine을 기반으로 한 미니게임을 구현하는 과정_ 을 기록하는 레포지토리입니다.

  * __선택한 장르__ : 액션 어드벤처 / 플랫포머
  * __기능을 모작할 게임__ : 원더포션 개발의 로프 액션 플랫폼 게임 '산나비'
  * __사용 가능한 컨트롤러__ : 키보드
  * __제작 기한__ : 5일
  * __핵심 구현 기능__ : 플레이어 조작 형식과 로프 액션
  
   
***

# 구현할 기능

***

 ## 1. 플레이어 캐릭터 조작 - 클래식
> #### WASD 이동방식
> >   AD 방향키를 입력해 이동할 수 있다.
> > 
> >   캐릭터가 벽에 닿은 상태로 WS 방향키를 입력하면, 벽을 타고 이동할 수 있다.
> #### 스페이스 바 를 입력해 점프할 수 있다.
>
> #### 캐릭터의 체력은 총 4칸으로, 0칸이 되면 저장된 값과 위치의 Scene을 불러온다.
>
> #### 캐릭터가 데미지 오브젝트에 닿으면, 체력 1칸을 감소하고, 해당 해당 위치에서부터 캐릭터의 뒷 방향으로 튕겨나가게 한다. 튕겨나가는 애니메이션 종료 후 다시 조작이 가능하도록 한다.

***

 ## 2. 플레이어 캐릭터 조작 - 로프 액션
> #### 좌클릭 입력 시, 캐릭터가 위치한 곳에서 -> 커서가 위치한 방향을 향해 사슬팔을 발사한다.
> > 일정 거리 내에 오브젝트가 존재할 시, 사슬팔은 레이캐스트가 인식한 위치에 고정된다.
> > 
> > 일정 거리 내에 오브젝트가 존재하지 않을 시, 사슬팔은 뻗어나가다가 캐릭터에게로 돌아온다.
> > 
> > 좌클릭을 떼면 사슬팔 고정이 false가 된다.
> #### 사슬팔이 고정된 상태에서 왼쪽 Shift를 입력하면, 공중에서 대쉬할 수 있다.

***

 ## 3. 튜토리얼 맵 구현
> #### 플레이어가 닿어도 문제가 없는 오브젝트 구성
> > 아무런 이벤트도 없는 오브젝트 (ex : 바닥, 벽, 기타 사물)
> > 
> > 위와 동일한 조건이나, 사슬팔이 닿아도 고정되지 않는 오브젝트
> #### 플레이어가 닿으면 체력이 깎이는 장애물 오브젝트

***

 ## 4. 적 에너미 구현 ( 선택사항 )
> #### 날아다니는 적 에너미 (바공격)
> > 사슬팔을 통해 붙잡아 올라타면, 일정 시간동안 원하는 방향으로 이동시킬 수 있다.
> > 
> > 일정시간이 지나면 적 에너미는 자폭한다. (노 데미지)
> > 
> > 플레이어의 그립 상태가 해제되면, 적 에너미는 자폭한다. (노 데미지)
> > 
> > 일정 시간이 지나면 해당 적 에너미는 초기 위치에서 재생성된다.
> #### 움직임이 고정된 채 공격하는 적 에너미 (ex : 터렛)
> > 캐릭터가 일정 범위 내로 들어오면, 해당 적 에너미는 캐릭터를 향해 총알 세례를 발사한다.
> > 
> > 한 번의 공격 함수가 끝나면, 다음 함수 실행 까지 약간의 텀이 존재한다.
> > 
> > 사슬팔로 붙잡고-해제 하는 것으로 해치울 수 있다.
> #### 움직임이 존재하는 적 에너미
> > 적 에너미의 앞 방향에서 캐릭터가 일정 범위 내로 들어오면, 해당 적 에너미는 캐릭터를 향해 총알 세례를 발사한다.
> > 
> > 한 번의 공격 함수가 끝나면, 다음 함수 실행 까지 약간의 텀이 존재한다.
> > 
> > 사슬팔로 붙잡고-해제 하는 것으로 해치울 수 있다.
> #### 특정 조건을 통해서만 없앨 수 있는 에너미 - 방패막 적 에너미
> > 적 에너미의 앞 방향에서 캐릭터가 일정 범위 내로 들어오면, 해당 적 에너미는 캐릭터를 향해 총알 세례를 발사한다.
> > 
> > 한 번의 공격 함수가 끝나면, 다음 함수 실행 까지 약간의 텀이 존재한다.
> > 
> > 해당 적 에너미의 앞 방향에서는 적 에너미를 사슬팔로 붙잡을 수 없다.
> > 
> > 해당 적 에너미의 뒷 방향에서부터 사슬팔로 붙잡고-해제 하는 것으로 해치울 수 있다.

***

 ## 5. 게임 시작 UI 구현 ( 선택사항 )
> #### 게임 시작 버튼
> > 해당 버튼을 누를 시, 게임의 난이도를 설정하는 창을 띄운다.
> > 
> > 쉬움, 보통, 어려움, 매우 어려움으로 구성된다.
> > 
> > 쉬움 난이도 버튼을 누를 시, 장애물 오브젝트에 닿아도 체력 칸이 닳지 않는 조건을 가지고 게임이 시작된다.
> > 
> > 이번에 진행될 프로젝트에서는 쉬움 난이도를 기준으로 기능을 제작하기 때문에, 다른 난이도의 버튼은 구성만 맞춰둔다.
> #### 설정 버튼
> > 볼륨 설정 슬라이드 바, 효과음 설정 슬라이드 바, 해상도 설정 버튼으로 나뉜다.
> > 
> > 해당 설정은 게임 플레이 중에도 ESC 를 입력하면 언제든 변경이 가능하도록 한다.
> #### 게임 종료 버튼
> > 게임을 종료하시겠습니까? 창과 함께 예, 아니오 버튼 UI 창이 생겨난다.
> > 예 버튼 클릭 시, 게임은 종료된다.
> > 아니오 버튼 클릭 시, 해당 UI창이 닫힌다.

***
