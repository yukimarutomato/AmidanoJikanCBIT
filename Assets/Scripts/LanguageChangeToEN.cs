using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LanguageChangeToEN : MonoBehaviour
{
    //     クリックしたらシーンを変えるスクリプト
    [SerializeField] private Sprite hoverSprite;   // カーソルが乗っているときの sprite
    [SerializeField]private Sprite selectSprite;
    [SerializeField] private GameObject JpBotton;
   

    private SpriteRenderer spriteRenderer;

    private Sprite originalSprite;
    private Sprite previousSprite;

    public bool isEN = false; 

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;   // 元の sprite を保存
    }

    private void Update()
    {
        if (isEN)
        {
            spriteRenderer.sprite = selectSprite;

        }
        else
        {
            spriteRenderer.sprite = originalSprite;
        }
    }

     void OnMouseOver()
    {
        if (hoverSprite != null)
        {
            previousSprite = spriteRenderer.sprite; // 現在の sprite を保存
            spriteRenderer.sprite = hoverSprite;
        }
    }

    void OnMouseExit()
    {
        spriteRenderer.sprite = previousSprite;   // 元に戻す
    }

     void OnMouseDown()
    {
        // 左クリックされたとき
        isEN = true;
        JpBotton.GetComponent<LanguageChangeToJP>().isJP = false;
    }
}
