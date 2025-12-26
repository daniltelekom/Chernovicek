using UnityEngine;

public class SlamVFX : MonoBehaviour
{
    public void SetRadius(float r)
    {
        float s = r * 2f;
        transform.localScale = new Vector3(s, s, s);
        Destroy(gameObject, 0.7f);
    }
}
