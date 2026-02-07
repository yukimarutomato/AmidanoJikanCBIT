using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFusenClick : MonoBehaviour
{
    // シーン切り替え付箋がクリックされたときに表示順を変えるスクリプト
    [SerializeField] private string sceneToLoad;

    // インスペクタで設定するSprite
    [SerializeField] private Sprite Chara0;
    [SerializeField] private Sprite Chara1;
    [SerializeField] private Sprite Chara2;
    [SerializeField] private Sprite Chara3;
    [SerializeField] private Sprite Chara4;
    [SerializeField] private Sprite Chara5;

    private void OnMouseDown()
    {
        // Tag "SceneFusen" の全オブジェクトを取得
        GameObject[] fusenObjects = GameObject.FindGameObjectsWithTag("SceneFusen");

        // すべての SceneFusen の Z を 1 にする
        foreach (GameObject obj in fusenObjects)
        {
            Vector3 pos = obj.transform.position;
            pos.z = 1f;
            obj.transform.position = pos;
        }

        // クリックされたこのオブジェクトだけ Z を -1 にする
        Vector3 myPos = transform.position;
        myPos.z = -1f;
        transform.position = myPos;

        // シーン遷移前のチェック処理
        CheckAndSetStartCharaId();

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("SceneToLoad が設定されていません！");
        }
    }

    private void CheckAndSetStartCharaId()
    {
        // CharaCollisionを持つすべてのオブジェクトを取得
        InputJson.CharaCollision[] charaColliders = FindObjectsOfType<InputJson.CharaCollision>();

        foreach (var charaCollider in charaColliders)
        {
            SpriteRenderer spriteRenderer = charaCollider.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) continue;

            // Spriteに応じてstartCharaIdを設定
            int startCharaId = -1;
            if (spriteRenderer.sprite == Chara0) startCharaId = 0;
            else if (spriteRenderer.sprite == Chara1) startCharaId = 1;
            else if (spriteRenderer.sprite == Chara2) startCharaId = 2;
            else if (spriteRenderer.sprite == Chara3) startCharaId = 3;
            else if (spriteRenderer.sprite == Chara4) startCharaId = 4;
            else if (spriteRenderer.sprite == Chara5) startCharaId = 5;

            if (startCharaId != -1)
            {
                // varLineIdに対応するvarInfoを取得して更新
                InputJson inputJson = FindObjectOfType<InputJson>();
                if (inputJson != null)
                {
                    inputJson.OnEventSpriteChanged(startCharaId, charaCollider.varLineId);
                }
            }
        }
    }
}

