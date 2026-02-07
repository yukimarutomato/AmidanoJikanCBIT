using UnityEngine;

public class DragToEvent : MonoBehaviour
{
    [SerializeField] private InputJson inputJson; // Inspectorで設定
    [SerializeField] private int eventId = -1;     // このオブジェクト固有のeventId

    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPos;

    private Collider2D currentHit = null;           // ぶつかった EventCollider

    void Start()
    {
        originalPos = transform.position;
    }

    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = transform.position.z;
        offset = transform.position - mouse;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = transform.position.z; // ドラッグ中 z 固定
        transform.position = mouse + offset;
    }

    void OnMouseUp()
    {
        isDragging = false;

        // 衝突している "Event" タグのオブジェクトがあれば Sprite を置き換え
        if (currentHit != null && currentHit.CompareTag("Event"))
        {
            SpriteRenderer targetSR = currentHit.GetComponent<SpriteRenderer>();
            SpriteRenderer mySR = GetComponent<SpriteRenderer>();
            if (targetSR != null && mySR != null)
            {
                targetSR.sprite = mySR.sprite;
            }

            if (inputJson != null)
            {
                var col = currentHit.GetComponent<InputJson.EventCollision>();
                if (col != null)
                {
                    inputJson.OnEventSpriteChanged(eventId, col.varLineId);
                }
            }
        }

        // 元の位置に必ず戻す
        transform.position = originalPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Event"))
        {
            currentHit = collision;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == currentHit)
        {
            currentHit = null;
        }
    }
}
