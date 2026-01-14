using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene2Story : MonoBehaviour
{
    public static Scene2Story Instance { get; private set; }
    public List<GameObject> toDestroyList;
    public string npcName = "Bình Minh";
    public string you = "Tôi";
    public string yourName = "Hoàng";
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        StartCoroutine(ShowIntroText());
    }

    IEnumerator ShowIntroText()
    {
        yield return new WaitForSeconds(0f);
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
