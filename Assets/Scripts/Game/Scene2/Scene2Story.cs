using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene2Story : MonoBehaviour
{
    public static Scene2Story Instance { get; private set; }
    public List<GameObject> toDestroyList;
    public string npcName = "Bình Minh";
    public string you = "Tôi";
    public string yourName = "Hoàng";
    [SerializeField] private string nextSceneName = "Scene 3";
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        QuestManager.Instance.OnQuestCompleted += HandleQuestCompleted;
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
        if (quest.questID == "Q2.1")
        {
            UIChatManager.Instance.SendChat("Mọi bí mật đều ở đây, tôi đã sẵn sàng, bạn thì sao", you, ChatPosition.Right);
            StartCoroutine(LoadSceneAfterDelay());
        }
    }
    
    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator ShowIntroText()
    {
        yield return new WaitForSeconds(3f);
        UIChatManager.Instance.SendChat("Chúng ta đã ở đây trước cả cảnh sát.", you, ChatPosition.Right);
        UIChatManager.Instance.SendChat(
            $"Thôi nào {yourName}? Cảnh sát mà có tác dụng thì mafia đâu lộng hành",
            npcName, ChatPosition.Left);
        UIChatManager.Instance.SendChat(
            $"Chia nhau ra tìm manh mối, tôi sẽ đi về phía Tây.",
            npcName, ChatPosition.Left,
            null,
            () =>
            {
                QuestManager.Instance.StartQuest("Q2.1");
            });
    }

    public void RunOutro()
    {

    }
}
