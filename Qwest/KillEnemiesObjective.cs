using UnityEngine;

public class KillEnemiesObjective : MiniObjective
{
    public int targetKills = 20;
    int current;

    void OnEnable()
    {
        EnemyHealth.OnAnyEnemyKilled += OnKill;
    }

    void OnDisable()
    {
        EnemyHealth.OnAnyEnemyKilled -= OnKill;
    }

    void OnKill()
    {
        current++;
        if (current >= targetKills)
            Finish(true);
    }

    public override string GetProgressText()
    {
        return $"{current}/{targetKills}";
    }
}
