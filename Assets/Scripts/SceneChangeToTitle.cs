using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeToTitle : MonoBehaviour
{
    //     クリックしたらシーンを変えるスクリプト
    [SerializeField] private Sprite hoverSprite;   // カーソルが乗っているときの sprite
    [SerializeField]private Sprite selectSprite;
    [SerializeField] private string sceneToLoad;


    private SpriteRenderer spriteRenderer;


    private Sprite originalSprite;




    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;   // 元の sprite を保存
    }


     void OnMouseOver()
    {
        if (hoverSprite != null)
        {

            spriteRenderer.sprite = hoverSprite;
        }
    }

    void OnMouseExit()
    {
        spriteRenderer.sprite = originalSprite;   // 元に戻す
    }

     void OnMouseDown()
    {
        spriteRenderer.sprite = selectSprite;
        SceneManager.LoadScene(sceneToLoad);
    }
}
