using UnityEngine;

public class Quest
{
    public string questName;
    public string questDescription;
    public Vector3 questTarget;
    public GameObject questDirector;

    public Quest(string questName, string questDescription, Vector3 questTarget)
    {
        this.questName = questName;
        this.questDescription = questDescription;
        this.questTarget = questTarget;
    }
    
}

