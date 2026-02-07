using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VerticalLineEndManager : MonoBehaviour
{
    public static VerticalLineEndManager Instance { get; private set; }
     public List<GameObject> allCharas=new List<GameObject>();
    public List<GameObject> verticalLines= new List<GameObject>(); // varLineId順に並べる
    [SerializeField] private GameObject EndChara;
    [SerializeField] private TextMeshProUGUI EndText;
    [SerializeField] private GameObject Fukidashi;
    [SerializeField] private Sprite Icon0, Icon1, Icon2, Icon3, Icon4, Icon5;

    private const int WAIT_FRAMES_AFTER_WALK = 150;

    private void Awake()
       
    {
        Instance = this;
    }


    private void Start()
    {
        Fukidashi.SetActive(false);
        StartEndProcess(new Dictionary<int, string>());
    }

    public void StartEndProcess(Dictionary<int, string> textsPerLine)
    {
        StartCoroutine(EndRoutine(textsPerLine));
    }

    private IEnumerator EndRoutine(Dictionary<int, string> textsPerLine)
    {
        Debug.Log("EndRoutine 開始");

        // 1. 全キャラ終了待機
        bool allDone = false;
        while (!allDone)
        {

            allDone = true; // まず全員終わったと仮定
            foreach (var go in allCharas)
            {
                Charawalk c = go.GetComponent<Charawalk>();
                if (c == null) continue;

                if (!c.IsWalkingDone)
                {
                    allDone = false; // 1人でも終わってなければ false
                    break;
                }
            }
            yield return null;
        }

       
           
        // 2. 150フレーム待機
        for (int i = 0; i < WAIT_FRAMES_AFTER_WALK; i++)
            yield return null;
        Debug.Log("全キャラの移動終了を確認");

        if (allDone)
        {
            // 3. 縦線ID順に End 処理
            foreach (var line in verticalLines)
            {
                // eventColliderObjに接触しているキャラを取得
                Collider2D[] hits = Physics2D.OverlapCircleAll(line.transform.position, 0.1f);
                foreach (var hit in hits)
                {
                    if (!hit.CompareTag("Chara")) continue;

                    // キャラIDに応じてアイコンを設定
                    var spriteRenderer = EndChara.GetComponent<SpriteRenderer>();
                    int id = hit.GetComponent<Charawalk>().CharaId;
                    switch (id)
                    {
                        case 0: spriteRenderer.sprite = Icon0; break;
                        case 1: spriteRenderer.sprite = Icon1; break;
                        case 2: spriteRenderer.sprite = Icon2; break;
                        case 3: spriteRenderer.sprite = Icon3; break;
                        case 4: spriteRenderer.sprite = Icon4; break;
                        case 5: spriteRenderer.sprite = Icon5; break;
                    }
                }

                foreach (var go in verticalLines)
                {
                    LoadJson.YokoObjComponent lineComp = go.GetComponent<LoadJson.YokoObjComponent>();
                    if (lineComp == null) continue; // 念のため null チェック

                    int id = lineComp.varLineId;

                    // horLineIdによってでテキストを設定
                    switch (id)
                    {
                        case 0:
                            EndText.text = textsPerLine.ContainsKey(0) ? textsPerLine[0] : "遊園地に遊びに行って、親友ができた！これからもよろしくね";
                            break;
                        case 1:
                            EndText.text = textsPerLine.ContainsKey(1) ? textsPerLine[1] : "バイト代をゲット！頑張った甲斐があった";
                            break;
                        case 2:
                            EndText.text = textsPerLine.ContainsKey(2) ? textsPerLine[2] : "多忙で体調を崩してしまった…";
                            break;
                        case 3:
                            EndText.text = textsPerLine.ContainsKey(3) ? textsPerLine[3] : "友達とご飯を食べに行って、親友になった！これからもよろしくね";
                            break;
                        case 4:
                            EndText.text = textsPerLine.ContainsKey(4) ? textsPerLine[4] : "今月のお小遣いが手に入った！やった～";
                            break;
                        case 5:
                            EndText.text = textsPerLine.ContainsKey(5) ? textsPerLine[5] : "温泉に入って、日々の疲れを癒した";
                            break;
                    }
                }

                Fukidashi.SetActive(true);
                Debug.Log("Fukidashi を表示");


                for (int i = 0; i < 150; i++)
                    yield return null;
            }
        }
    }
}
