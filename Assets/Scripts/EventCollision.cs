using UnityEngine;

public class EndCollision : MonoBehaviour
{
    public int varLineId;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Chara")) return;

        Charawalk chara = other.GetComponent<Charawalk>();
        if (chara == null) return;

        triggered = true;

        VerticalLineEndManager.Instance.NotifyArrived(
            varLineId,
            chara.CharaId
        );
    }
}
