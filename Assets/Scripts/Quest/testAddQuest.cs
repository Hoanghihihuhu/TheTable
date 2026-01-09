using UnityEngine;

public class testAddQuest: MonoBehaviour
{
    public Transform questTarget;
    private bool isQuestAdded = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isQuestAdded)
        {
            QuestManager.Instance.AddQuest(new Quest("Test Quest", "This is a test quest", questTarget.position));
            isQuestAdded = true;
        }
    }
}
