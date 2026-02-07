using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LanguageChangeToJP : MonoBehaviour
{
    //     クリックしたらシーンを変えるスクリプト
    [SerializeField] private Sprite hoverSprite;   // カーソルが乗っているときの sprite
    [SerializeField]private Sprite selectSprite;
    [SerializeField] private GameObject EnBotton;
    [SerializeField] private string sceneToLoadJP;


    private SpriteRenderer spriteRenderer;

    private Sprite originalSprite;
    private Sprite previousSprite;



    public bool isJP = true; // 日本語がデフォルト

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;   // 元の sprite を保存
    }

    void Update()
    {
        if (isJP)
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
            SceneManager.LoadScene(sceneToLoadJP);

        }
    }

    void OnMouseExit()
    {
        spriteRenderer.sprite = previousSprite;   // 元に戻す
    }

     void OnMouseDown()
    {
        // 左クリックされたとき
        isJP = true;
        EnBotton.GetComponent<LanguageChangeToEN>().isEN = false;
    }
}
