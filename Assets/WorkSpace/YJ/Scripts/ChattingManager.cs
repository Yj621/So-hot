using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChattingManager : MonoBehaviourPunCallbacks
{
    [Header ("채팅 관련")]
    [SerializeField] private Button sendButton;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private Transform chatContent;
    [SerializeField] private GameObject messageMe;
    [SerializeField] private GameObject messageYou;

    [Header("채팅 활성화 관련")]
    [SerializeField] private CanvasGroup chatPanelCanvasGroup; // 채팅 패널의 CanvasGroup
    [SerializeField] private GameObject chatPanel; // 채팅 패널 (전체 채팅 UI)
    [SerializeField] private float chatVisibilityDuration = 1f; // 채팅 활성화 지속 시간
    private Coroutine chatPanelCoroutine; // 코루틴을 관리하기 위한 변수


    PhotonView pv;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        //전송 버튼 클릭시 메서드 동적 연결
        sendButton.onClick.AddListener(() => OnSubmit(chatInput.text));

        // 채팅 입력 필드에서 엔터 키를 눌렀을 때도 메시지 전송
        chatInput.onEndEdit.AddListener((inputText) =>
        {
            if (!string.IsNullOrWhiteSpace(inputText))
            {
                OnSubmit(inputText);
            }
        });
        // Chat InputField는 항상 활성화
        chatInput.gameObject.SetActive(true);
    }


    void Update()
    {
    }

    // Photon RPC로 호출되는 메서드, 모든 클라이언트에서 메시지를 표시하도록 처리
    [PunRPC]
    void RpcAddChat(string sender, string chat, string time)
    {
        // 메시지가 로컬 플레이어로부터 온 것인지 확인
        bool isLocalPlayer = sender == PhotonNetwork.NickName;

        // 로컬 플레이어와 상대방에 따라 메시지 객체 선택
        GameObject item = Instantiate(isLocalPlayer ? messageMe : messageYou, chatContent);

        // 채팅 박스 구성 요소 찾기
        Transform chattingBoxTransform = item.transform.Find("ChattingBox");
        Transform messageTextTransform = chattingBoxTransform?.Find("Message_Text");
        Transform avatarTransform = chattingBoxTransform?.Find("avatar");

        if (messageTextTransform == null || avatarTransform == null)
        {
            Debug.LogError("Message_Text나 avatar가 없음");
            return;
        }

        // 메시지 텍스트 설정
        messageTextTransform.GetComponent<TextMeshProUGUI>().text = chat;

        // 상대방의 이름을 설정 (로컬 플레이어가 아닌 경우만)
        if (!isLocalPlayer)
        {
            Transform nameTextTransform = chattingBoxTransform.Find("NickName_Text");

            // TextMeshProUGUI 컴포넌트를 찾아서 텍스트 설정
            TextMeshProUGUI nameText = nameTextTransform.GetComponent<TextMeshProUGUI>();
            nameText.text = sender;

            //방장만 빨간색으로 표시
            if (sender == PhotonNetwork.MasterClient.NickName)
            {
                nameText.color = Color.red;
            }
        }

        // 시간 설정
        TextMeshProUGUI timeText = avatarTransform.Find("Time_Text").GetComponent<TextMeshProUGUI>();
        timeText.text = time;

        // 채팅 메시지가 표시되면 채팅 UI 활성화
        StartCoroutine(ActivateChatPanel());
    }

    // 채팅 패널 서서히 사라지는 처리
    private IEnumerator FadeOutChatPanel()
    {
        // 채팅 패널 활성화
        chatPanel.SetActive(true);
        chatPanelCanvasGroup.alpha = 1f;

        // 3초 동안 유지
        yield return new WaitForSeconds(3f);

        float startAlpha = chatPanelCanvasGroup.alpha;
        float endAlpha = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < chatVisibilityDuration)
        {
            elapsedTime += Time.deltaTime;
            // elapsedTime / chatVisibilityDuration 값에 따라 alpha 값 선형 보간
            chatPanelCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / chatVisibilityDuration);
            yield return null;
        }

        // 최종적으로 완전히 비활성화
        chatPanelCanvasGroup.alpha = endAlpha;

        // 채팅 패널 비활성화
        chatPanel.SetActive(false);

        // 코루틴 변수 초기화
        chatPanelCoroutine = null;
    }
    // 채팅 메시지가 표시되면 채팅 UI 활성화
    private IEnumerator ActivateChatPanel()
    {
        // 코루틴이 실행 중이면 중지
        if (chatPanelCoroutine != null)
        {
            StopCoroutine(chatPanelCoroutine);
        }

        // 새 코루틴 시작
        chatPanelCoroutine = StartCoroutine(FadeOutChatPanel());
        yield return null;
    }

    // 메시지 전송 함수
    void OnSubmit(string s)
    {
        // 입력이 비어 있거나 공백만 있는 경우 메시지 전송하지 않음
        if (string.IsNullOrWhiteSpace(chatInput.text))
        {
            Debug.Log("인풋이 없음");
            return;
        }

        // 현재 시간 가져오기 (메시지 전송 시간)
        string currentTime = System.DateTime.Now.ToString("HH:mm");

        // RPC 호출하여 모든 클라이언트에 메시지 전송
        pv.RPC("RpcAddChat", RpcTarget.All, PhotonNetwork.NickName, chatInput.text, currentTime);

        // 입력 필드 초기화 및 활성화
        chatInput.text = "";
        chatInput.ActivateInputField();
    }

}
