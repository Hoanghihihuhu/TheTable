using System.Collections;
using UnityEngine;

public class AutoQuest1 : MonoBehaviour
{
    public GameObject questTarget;
    void Start () {
        StartCoroutine("AddQuest");
    }

    IEnumerator AddQuest() {
        if (QuestManager.Instance == null) yield break;
        yield return new WaitForSeconds(2f);
        if (UIChatManager.Instance != null) {
            UIChatManager.Instance.SendChat("A Long đang bị bao vây, tôi cần tìm và hỗ trợ anh ấy", "Player", ChatPosition.Right);
        }

        yield return new WaitForSeconds(5f);
        string questName = "Quest1";
        Quest quest = new Quest(questName, "Tìm và hỗ trợ A Long", questTarget.transform.position);
        QuestManager.Instance.AddQuest(quest);
        AlongQuest alongQuest = questTarget.AddComponent<AlongQuest>();
        alongQuest.questName = questName;


        yield return new WaitForSeconds(1f);
        alongQuest.DrawDirectorArea();
    }
}