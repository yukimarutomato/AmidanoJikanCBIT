using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBack : MonoBehaviour
{
   

    [SerializeField] GameObject Memo;

    // Start is called before the first frame update
    void Start()
    {
       Memo.SetActive(false); 
    }

    //Escapeキーでタイトルバックを表示にする
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Memo.activeSelf)
            {
                Debug.Log("Escapeキーが押されました");
                Memo.SetActive(true);
            }
        }

}

    
}
