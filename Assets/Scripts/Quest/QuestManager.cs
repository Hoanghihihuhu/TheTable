using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    public Transform playerTransform;
    public GameObject questDirectorPrefab;
    public List<Quest> quests = new List<Quest>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddQuest(Quest quest)
    {
        quests.Add(quest);
        GameEvents.ShowNotice($"Đã nhận nhiệm vụ: {quest.questDescription}");
        quest.questDirector = Instantiate(questDirectorPrefab, new Vector2(-999f, -999f), Quaternion.identity);
        GetPlayerTransform();
    }

    public void RemoveQuest(Quest quest)
    {
        Destroy(quest.questDirector);
        quests.Remove(quest);
        GetPlayerTransform();
    }

    public void RemoveQuest(string questName)
    {
        quests.FindAll(quest => quest.questName == questName).ForEach(quest => RemoveQuest(quest));
    }

    void Update()
    {
        if (quests.Count == 0) return;
        if (playerTransform == null)
        {
            return;
        }
        Vector2 direction = playerTransform.position - quests[0].questTarget;
        direction.Normalize();
        quests[0].questDirector.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
        quests[0].questDirector.transform.position = playerTransform.position - new Vector3(direction.x, direction.y, 0);
    }

    void GetPlayerTransform()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
    }
}
