using UnityEngine;

public class FukidashiDebug : MonoBehaviour
{
    private void OnDisable()
    {
        Debug.Log($"Fukidashi disabled by {Time.frameCount}", this);
    }

    private void OnEnable()
    {
        Debug.Log($"Fukidashi enabled by {Time.frameCount}", this);
    }
}
