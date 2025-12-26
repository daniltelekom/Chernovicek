using UnityEngine;

public abstract class MiniObjective : MonoBehaviour
{
    public string title;
    public float timeLimit = 20f;

    protected float timer;
    protected bool completed;

    public System.Action<bool> OnFinished;

    public virtual void StartObjective()
    {
        timer = timeLimit;
        completed = false;
    }

    public virtual void UpdateObjective()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            Finish(false);
    }

    protected void Finish(bool success)
    {
        if (completed) return;
        completed = true;
        OnFinished?.Invoke(success);
        Destroy(this);
    }

    public abstract string GetProgressText();
}
