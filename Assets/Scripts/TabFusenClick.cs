using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabFusenClick : MonoBehaviour
{
    // タブ切り替え付箋がクリックされたときに表示順を変えるスクリプト

    private void OnMouseDown()
    {
        // Tag "SceneFusen" の全オブジェクトを取得
        GameObject[] fusenObjects = GameObject.FindGameObjectsWithTag("TabFusen");

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
    }
}

