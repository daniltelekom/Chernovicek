using UnityEngine;

public class SurviveObjective : MiniObjective
{
    public override string GetProgressText()
    {
        return Mathf.CeilToInt(timer).ToString();
    }
}
