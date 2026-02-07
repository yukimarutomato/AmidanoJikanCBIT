using UnityEngine;

public class DragToChara : MonoBehaviour
{
    [SerializeField] private int charaId = 0;  // Inspector で設定する 0〜5
    [SerializeField] private int charaIndex = -1; // 0~5 を Inspectorで設定
    [SerializeField] private InputJson inputJson; // Inspector で設定


    private bool isDragging = false;
    private Collider2D currentHit;
    private Vector3 offset;
    private Vector3 originalPos;

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

    private void OnMouseUp()
    {
        isDragging = false;

        if (currentHit != null && currentHit.CompareTag("Chara"))
        {
            SpriteRenderer targetSR = currentHit.GetComponent<SpriteRenderer>();
            SpriteRenderer mySR = GetComponent<SpriteRenderer>();
            if (targetSR != null && mySR != null)
            {
                targetSR.sprite = mySR.sprite;
            }

            if (inputJson != null)
            {
                var col = currentHit.GetComponent<InputJson.CharaCollision>();
                if (col != null)
                {
                    inputJson.OnEventSpriteChanged(charaId, col.varLineId);
                }
            }
        }
        transform.position = originalPos;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = transform.position.z; // ドラッグ中 z 固定
        transform.position = mouse + offset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Chara"))
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
