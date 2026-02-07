using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SpriteChange : MonoBehaviour
{
    //     クリックしたらシーンを変えるスクリプト
    [SerializeField] private Sprite hoverSprite;   // カーソルが乗っているときの sprite
    [SerializeField] private Sprite selectSprite;
    [SerializeField] private Button targetButton;


    private Sprite originalSprite;
    SpriteState spritestate = new SpriteState();




    private void Awake()
    {
        spritestate = targetButton.spriteState;
        originalSprite = targetButton.GetComponent<Image>().sprite;   // 元の sprite を保存
        targetButton.onClick.AddListener(OnButtonClicked);
    }

    public void PointerEnter()
    {
        Debug.Log("ボタンにマウスが乗った！");
        targetButton.GetComponent<Image>().sprite =  hoverSprite;
    }

    public void PointerExit()
    {
        Debug.Log("ボタンからマウスが離れた！");
        targetButton.GetComponent<Image>().sprite = originalSprite;   // 元に戻す
    }




    private void OnButtonClicked()
    {
        targetButton.GetComponent<Image>().sprite = selectSprite;
    }

}
