using UnityEngine;

public class AlongQuest : MonoBehaviour
{
    Transform playerTransform;
    public string questName = "Quest1";
    public bool isPlayerReach = false;
    public float reachArea = 3f;
    public int segments = 64;
    public ParticleSystem ps;
    LineRenderer lr;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = segments;
        lr.widthMultiplier = 0.05f;

        ps = GetComponentInChildren<ParticleSystem>();
        ps.gameObject.SetActive(false);
    }

    public void DrawDirectorArea()
    {
        DrawCircle(lr);
        ps.gameObject.SetActive(true);
        ps.Play();
    }

    void FixedUpdate()
    {
        if (isPlayerReach == true) return;

        if (Vector2.Distance(playerTransform.position, transform.position) < reachArea)
        {
            isPlayerReach = true;
            Destroy(lr);
            Destroy(ps);
            QuestManager.Instance.RemoveQuest(questName);
        }
    }


    void DrawCircle(LineRenderer lr)
    {
        float angle = 0f;
        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(angle) * reachArea;
            float y = Mathf.Sin(angle) * reachArea;
            lr.SetPosition(i, new Vector3(x, y, 0));
            angle += 2 * Mathf.PI / segments;
        }
    }
}