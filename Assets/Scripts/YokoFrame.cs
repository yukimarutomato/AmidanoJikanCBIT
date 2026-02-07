using UnityEngine;

public class YokoFrame : MonoBehaviour
{
    public GameObject linkedYokoLine; // 紐づく横線

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 初期状態は非表示（Collider は有効）
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    // カーソルが重なったら表示
    void OnMouseEnter()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
    }

    // カーソルが離れたら非表示
    void OnMouseExit()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    // 左クリックで横線をトグル
    void OnMouseDown()
    {
        if (linkedYokoLine == null) return;

        bool isActive = linkedYokoLine.activeSelf;
        linkedYokoLine.SetActive(!isActive);
    }
}
