using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChattingManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button sendButton;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private Transform chatContent;
    [SerializeField] private GameObject messageMe;
    [SerializeField] private GameObject messageYou;

    PhotonView pv;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        sendButton.onClick.AddListener(() => OnSubmit(chatInput.text));

        chatInput.onEndEdit.AddListener((inputText) =>
        {
            if (!string.IsNullOrWhiteSpace(inputText))
            {
                OnSubmit(inputText);
            }
        });
    }


    void Update()
    {
    }


    [PunRPC]
    void RpcAddChat(string sender, string chat, string time)
    {
        // 메시지가 로컬 플레이어로부터 온 것인지 확인
        bool isLocalPlayer = sender == PhotonNetwork.NickName;

        // messageMe 또는 messageYou 인스턴스 생성
        GameObject item = Instantiate(isLocalPlayer ? messageMe : messageYou, chatContent);

        // ChattingBox 찾기
        Transform chattingBox_Transform = item.transform.Find("ChattingBox");
        if (chattingBox_Transform == null)
        {
            Debug.LogError("ChattingBox not found in prefab.");
            return;
        }

        // Message_Text 찾기
        Transform message_Transform = chattingBox_Transform.Find("Message_Text");
        if (message_Transform == null)
        {
            Debug.LogError("Message_Text not found in ChattingBox.");
            return;
        }
        TextMeshProUGUI messageText = message_Transform.GetComponent<TextMeshProUGUI>();
        messageText.text = chat;

        // NickName_Text 설정 (로컬 플레이어가 아닌 경우만)
        if (!isLocalPlayer)
        {
            Transform name_Transform = chattingBox_Transform.Find("NickName_Text");
            if (name_Transform == null)
            {
                Debug.LogError("NickName_Text not found in ChattingBox.");
                return;
            }
            TextMeshProUGUI nameText = name_Transform.GetComponent<TextMeshProUGUI>();
            nameText.text = sender;
        }

        // avatar 찾기
        Transform avatar_Transform = chattingBox_Transform.Find("avatar");
        if (avatar_Transform == null)
        {
            Debug.LogError("avatar not found in ChattingBox.");
            return;
        }

        // Time_Text 설정
        Transform time_Transform = avatar_Transform.Find("Time_Text");
        if (time_Transform == null)
        {
            Debug.LogError("Time_Text not found in avatar.");
            return;
        }
        TextMeshProUGUI timeText = time_Transform.GetComponent<TextMeshProUGUI>();
        timeText.text = time;
    }

    void OnSubmit(string s)
    {
        Debug.Log("OnSubmit called with: " + s);

        if (string.IsNullOrWhiteSpace(chatInput.text))
        {
            Debug.Log("Input is empty or whitespace.");
            return;
        }

        string currentTime = System.DateTime.Now.ToString("HH:mm");
        pv.RPC("RpcAddChat", RpcTarget.All, PhotonNetwork.NickName, chatInput.text, currentTime);

        chatInput.text = "";
        chatInput.ActivateInputField();
    }
}
