using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene1Story : MonoBehaviour
{
    public static Scene1Story Instance { get; private set; }
    public List<GameObject> toDestroyList;
    public string[] chatOutro = { "", "", "" };
    public string characterName = "ADMIN";
    [SerializeField] private string nextSceneName = "Scene 2";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public Camera mainCamera;
    public Camera introCamera;
    // Start is called before the first frame update
    void Start()
    {
        QuestManager.Instance.OnQuestCompleted += HandleQuestCompleted;
        SetUp();
        StartCoroutine(ShowIntroText());
    }

    void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestCompleted -= HandleQuestCompleted;
        }
    }

    private void HandleQuestCompleted(Quest quest)
    {
        if (quest.questID == "Q1")
        {
            UIChatManager.Instance.SendChat("Kỹ năng cần thời gian mài dũa, thời gian có hạn....", "Tôi", ChatPosition.Right);
            StartCoroutine(LoadSceneAfterDelay());
        }
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(nextSceneName);
    }

    void SetUp()
    {
        mainCamera.gameObject.SetActive(false);
        introCamera.gameObject.SetActive(true);
    }

    IEnumerator ShowIntroText()
    {
        yield return new WaitForSeconds(1);
        if (UINoticeManager.Instance != null)
        {
            UINoticeManager.Instance.ShowNotice(
                "Hắc Thành – nơi ánh sáng không bao giờ chạm tới đáy của bóng tối.",
                UINoticeManager.NoticeType.JustTalk
            );
            UINoticeManager.Instance.ShowNotice(
                "Mafia kiểm soát đường phố. Cắm rễ trong những ngóc ngách sâu nhất",
                UINoticeManager.NoticeType.JustTalk
            );
            UINoticeManager.Instance.ShowNotice(
                "Bạn quay trở lại cố thành - không với tư cách anh hùng \n Nhưng tâm huyết đánh tan bọn Mafia đang nuốt chửng quê hương mình",
                UINoticeManager.NoticeType.JustTalk,
                UINoticeManager.NoticeSpeed.Normal,
                () =>
                {
                    mainCamera.gameObject.SetActive(true);
                    introCamera.gameObject.SetActive(false);
                    QuestManager.Instance.StartQuest("Q1");
                    toDestroyList.ForEach(item => Destroy(item));
                }
            );
        }
    }

    public void RunOutro()
    {
        foreach (string chat in chatOutro)
        {
            UIChatManager.Instance.SendChat(chat, characterName);
        }
    }
}
