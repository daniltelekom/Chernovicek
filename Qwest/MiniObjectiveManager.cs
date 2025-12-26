using UnityEngine;

public class MiniObjectiveManager : MonoBehaviour
{
    public float interval = 45f;

    float timer;
    MiniObjective current;

    void Start()
    {
        timer = interval;
    }

    void Update()
    {
        if (current != null)
        {
            current.UpdateObjective();
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            StartRandomObjective();
            timer = interval;
        }
    }

    void StartRandomObjective()
    {
        int roll = Random.Range(0, 2);

        if (roll == 0)
        {
            current = gameObject.AddComponent<KillEnemiesObjective>();
            current.title = "Kill enemies";
            ((KillEnemiesObjective)current).targetKills = 25;
        }
        else
        {
            current = gameObject.AddComponent<SurviveObjective>();
            current.title = "Survive";
            current.timeLimit = 20f;
        }

        current.OnFinished += OnObjectiveFinished;
        current.StartObjective();
    }

    void OnObjectiveFinished(bool success)
    {
        if (success)
        {
            Debug.Log("Objective completed!");
            // НАГРАДА
            FindObjectOfType<PlayerStats>()?.AddExp(50);
        }
        else
        {
            Debug.Log("Objective failed");
        }

        current = null;
    }

    public MiniObjective GetCurrent() => current;
}
