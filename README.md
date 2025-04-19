# 📄 목차
[1. 개요](#개요)

[2. 게임 설명](#게임-설명)

[3. 게임 플레이 방식](#게임-플레이-방식)

## 개요

### 📌 프로젝트 이름

앗!뜨거 원시인(So-Hot)

플레이 영상
[![썸네일](https://github.com/user-attachments/assets/f8c072c2-dbf9-4e67-b5be-5fb62bdf2ec6)](https://youtu.be/46StBkKMiuE)


### 💡 장르

3D 어드벤쳐 플랫폼

### ⏰ 개발 기간

2025.02.14~2025.03.18


### ⚙️ **언어 및 게임 엔진**

<img src="https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white">

<img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white">


### 🛠️ **협업 툴**

<img src="https://img.shields.io/badge/Jira-0052CC?style=for-the-badge&logo=Jira&logoColor=white">

<img src="https://img.shields.io/badge/Discord-7289DA?style=for-the-badge&logo=discord&logoColor=white">

<img src="https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white">

### 👩‍💻  **팀 구성원 및 역할**

| **이름** | 권동현 | 장조성 | 정기주 | 하민정 | 정윤지 |
| --- | --- | --- | --- | --- | --- |
| **역할1** | Photon Engine을 이용한 멀티 플레이 환경 구축 | 플레이어 구현 및 맵 기믹 | 맵 수정 | 아이템 및 인벤토리 구현 | Photon Engine을 활용한 멀티 방 생성, 채팅창 구현 |
| **역할2** | 스킬 구현 | 캐릭터 선택창 구현 | 게이지 시스템 구현 | 세이브포인트 구현 | Photon Voice2를 이용한 보이스챗 구현 |
| **GitHub** | [@kdsh627](https://github.com/kdsh627) | [@hipop1109](https://github.com/hipop1109) |[@JungKiJoo777](https://github.com/JungKiJoo777) | [@pipirongcha](https://github.com/pipirongcha) | [@Yj621](https://github.com/Yj621) |
| **코드 구현** | [Network](https://github.com/Yj621/So-hot/tree/main/Assets/WorkSpace/DH) | [Player](https://github.com/Yj621/So-hot/tree/main/Assets/WorkSpace/JS) | [Map](https://github.com/Yj621/So-hot/tree/main/Assets/WorkSpace/KJ) | [SavePoint](https://github.com/Yj621/So-hot/tree/main/Assets/WorkSpace/MJ) | [PhotonVoice&Chat](https://github.com/Yj621/So-hot/tree/main/Assets/WorkSpace/YJ) |

**📁 디렉토리 구조**

> 깃허브를 통한 협업 과정에서 Assets 폴더 하위에 본인의 워크스페이스를 형성하여 작업 진행

```csharp
SoHot
├── Assets
│   ├── WorkSpace
│   │   ├── DH    // 권동현 작업 공간
│   │   ├── JS    // 장조성 작업 공간
│   │   ├── KJ    // 정기주 작업 공간
│   │   ├── MJ    // 하민정 작업 공간
│   │   ├── YJ    // 정윤지 작업 공간
├── Packages
├── ProjectSettings
```

## 게임 설명
<table>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/9e40e968-73a2-4565-8ce2-12040dacc963" width="300"></td>
    <td><img src="https://github.com/user-attachments/assets/cc453303-0486-4e3a-a6fb-55d88f23fdde" width="300"></td>
    <td><img src="https://github.com/user-attachments/assets/682d4926-a92d-495e-a3e0-c0e2a1c6e1ef" width="300"></td>
    <td><img src="https://github.com/user-attachments/assets/f9ae14d1-4186-4cc3-8aab-17aa723f0f97" width="300"></td>
  </tr>
  <tr>
    <td align="center">시작화면</td>
    <td align="center">닉네임 설정</td>
    <td align="center">방 만들기</td>
    <td align="center">클리어</td>
  </tr>
</table>


🤼  우연히 친 번개로 인해 발생한 불씨를 발견한 4명의 원시인들이 그들의 거처까지 안전하게 불씨를 옮기는 “앗!뜨거 원시인”을 제작했습니다.

- 멀티 플레이어 💡
    
    **최대 4명의 플레이어**가 협력하여 불씨를 제단까지 옮겨야 합니다.
    **뜨거움 게이지**는 불을 들고 있는 동안 게이지가 차오르므로, **다른 플레이어에게 전달**하며 진행하세요.
    

- 불을 놓치면 안돼요! 🔥
    
    **불씨를 떨어뜨리면** 타이머가 줄어들며, **0초가 되면 세이브포인트로 돌아갑니다.**
    타이머가 부족할 땐 **아이템을 활용**하여 시간을 늘려보세요!
    
- 세이브포인트 🌱
    
    **중간중간 저장 가능한 포인트**에서 불씨를 던져 저장하세요.
    **모든 플레이어가 죽거나** 불씨를 잃으면 저장된 세이브포인트로 돌아갑니다.
    
- 유령 👻
    
    장애물에 맞으면 플레이어는 **유령**이 되어 구천을 떠돌게 됩니다.
    **유령 상태에서는** 불씨를 잡을 수 없지만, 장애물에 영향을 받지 않습니다.
    

**뜨거운 불씨를 지키며 협동의 재미를 느껴보세요!**
장애물을 극복하고, 팀워크로 제단까지 도달하세요! 🎮✨

## 게임 플레이 방식
![image](https://github.com/user-attachments/assets/a2f9567d-fe12-4d64-a5ee-5340eae56c50)

- 캐릭터 이동 방법

| 이동방향 | 좌(왼쪽) | 우(오른쪽) | 점프 | 대쉬 | 던지기 | 아이템 | 스킬 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 키보드 | A | D | SpaceBar | LeftShift | 우클릭 꾹 차징 후 떼기 | G | E |
- 맵 순서

| 캐릭터 선택창 | 플레이 | 스킬 사용 | 죽을시 |
| :---: | :---: | :---: | :---: |
| ![image](https://github.com/user-attachments/assets/0a70fde7-3bf7-4121-a6e5-426e582193b3) |![image](https://github.com/user-attachments/assets/97bfe26c-54ad-414c-8bfb-0a7cf79c9138) |![image](https://github.com/user-attachments/assets/8d79207b-7fb4-4ec5-ad11-6733b15a2269)| ![image](https://github.com/user-attachments/assets/d15986fd-58d9-45ae-bff2-53ea9859b03e)
| 캐릭터, 스킬 선택, 채팅 및 보이스 가능 | 불을 바닥에 떨어뜨렸을시 | 아이템/스킬 사용시 텍스트 애니메이션 | 유령으로 변함 |


## ⏩ 게임 실행 방법

1. [게임 다운로드 링크](https://drive.google.com/file/d/1Jc499aamxR2MqfaHD2eJCcMS9OT7HFPy/view?usp=sharing) 다운로드
2. 압축 해제 후, Absorber.exe 실행
