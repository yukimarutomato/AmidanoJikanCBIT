using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;


public class LoadJson : MonoBehaviour
{
    public class YokoObjComponent : MonoBehaviour
    {
        public int horLineId; // 横線の累計ID
        public int varLineId;
        public StartJson.HorLineInfo info; // 紐付けるJSONデータ
        public bool isEventCanvasSpawned = false;
        public bool isEventRegistered = false;
        public bool isEventRegistering = false;
    }

    [System.Serializable]
    public class SaveData
    {
        public int VarticalLineNumber;
        public List<StartJson.CharaData> charaList;
        public List<StartJson.FlagData> flagList;
        public List<StartJson.HorLineInfo> HorLineInfo;
        public List<StartJson.VarLineInfo> VarLineInfo;
    }
    private string jsonFilePath;
    [SerializeField] private TextAsset JsonJp;
    [SerializeField] private TextAsset JsonEn;
    [SerializeField] public GameObject YokoFramePrefab;
    [SerializeField] public GameObject YokoBotton;
    [SerializeField] public GameObject VarticalLinePrefab;
    [SerializeField] public GameObject HorizontalLinePrefab;
    [SerializeField] public GameObject CharaColliderPrefab;
    [SerializeField] public GameObject EventColliderPrefab;

    [SerializeField] private GameObject CharaPrefab0;
    [SerializeField] private GameObject CharaPrefab1;
    [SerializeField] private GameObject CharaPrefab2;
    [SerializeField] private GameObject CharaPrefab3;
    [SerializeField] private GameObject CharaPrefab4;
    [SerializeField] private GameObject CharaPrefab5;

    [SerializeField] private GameObject EventPrefab0;
    [SerializeField] private GameObject EventPrefab1;
    [SerializeField] private GameObject EventPrefab2;

    // 現在描画中の線を管理
    private List<GameObject> spawnedLines = new List<GameObject>();
    private int currentHorLineId = -1;
    
    // Start is called before the first frame update
    void Start()
    {
        EventCanvasManager.Reset();

        SaveData data;
        if (JsonJp == null)
        {
            Debug.LogError("JsonJp がインスペクタで設定されていません");
            data = CreateEmptyData();
        }
        else
        {
            data = LoadJsonFromTextAsset(JsonJp);
        }

        RedrawLines(data);

    }

 
   

    // Update is called once per frame
    void Update()
    {
        
    }

    private SaveData LoadJsonFromTextAsset(TextAsset jsonAsset)
    {
        SaveData data = JsonConvert.DeserializeObject<SaveData>(jsonAsset.text);

        if (data.HorLineInfo == null) data.HorLineInfo = new List<StartJson.HorLineInfo>();
        if (data.VarLineInfo == null) data.VarLineInfo = new List<StartJson.VarLineInfo>();
        if (data.charaList == null) data.charaList = new List<StartJson.CharaData>();

        return data;
    }

    private SaveData CreateEmptyData()
    {
        return new SaveData
        {
            VarticalLineNumber = 1,
            HorLineInfo = new List<StartJson.HorLineInfo>(),
            VarLineInfo = new List<StartJson.VarLineInfo>(),
            charaList = new List<StartJson.CharaData>()
        };
    }

    private void RedrawLines(SaveData data)
    {
        int count = Mathf.Max(1, data.VarticalLineNumber);
        float lineSpace = 2.0f;
        int horLineCounter = 0;

        // 縦線描画（黒）
        for (int i = 0; i < count; i++)
        {
            float x = -5.0f - (count - 1) * 0.5f + i * lineSpace; // JSON の値に基づいて均等配置

            // --- VarLineInfo を取得 ---
            StartJson.VarLineInfo varInfo = data.VarLineInfo[i];

            // 縦線 Prefab を複製
            GameObject obj = Instantiate(VarticalLinePrefab);
            spawnedLines.Add(obj);

            LineRenderer lrTate = obj.GetComponent<LineRenderer>() ?? obj.AddComponent<LineRenderer>();
            lrTate.material = new Material(Shader.Find("Sprites/Default"));
            lrTate.startColor = Color.black;
            lrTate.endColor = Color.black;
            lrTate.startWidth = 0.06f;
            lrTate.endWidth = 0.06f;

            lrTate.positionCount = 2;
            lrTate.SetPosition(0, new Vector3(x, 3.2f, -1));
            lrTate.SetPosition(1, new Vector3(x, -3.2f, -1));

            // CharaCollider を複製して varLineId を設定
            GameObject charaColliderObj = Instantiate(CharaColliderPrefab);
            SpriteRenderer charaSpr = charaColliderObj.GetComponent<SpriteRenderer>();
            spawnedLines.Add(charaColliderObj);
            charaColliderObj.transform.position = new Vector3(x, 3.8f, -1);
            VerticalLineEndManager.Instance.allCharas.Add(charaColliderObj);

            // startCharaId に応じて Sprite を変更
            if (charaSpr != null)
            {
                switch (varInfo.startCharaId)
                {
                    case 0:
                        charaSpr.sprite = CharaPrefab0.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 1:
                        charaSpr.sprite = CharaPrefab1.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 2:
                        charaSpr.sprite = CharaPrefab2.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 3:
                        charaSpr.sprite = CharaPrefab3.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 4:
                        charaSpr.sprite = CharaPrefab4.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 5:
                        charaSpr.sprite = CharaPrefab5.GetComponent<SpriteRenderer>().sprite;
                        break;
                    default:
                        // -1 の場合はデフォルトのまま
                        break;
                }
            }

            
            int charaId = varInfo.startCharaId;
            StartJson.CharaData charaData =
            data.charaList.Find(c => c.id == charaId);
           
            Charawalk walk = charaColliderObj.GetComponent<Charawalk>();
            walk.Init(charaId, charaData);

            // EventCollider を複製（位置は縦線の終点 -0.5）
            GameObject eventColliderObj = Instantiate(EventColliderPrefab);
            SpriteRenderer eventSpr = eventColliderObj.GetComponent<SpriteRenderer>();
            spawnedLines.Add(eventColliderObj);
            eventColliderObj.transform.position = new Vector3(x, -3.5f, -1);
            if (eventSpr != null)
            {
                switch (varInfo.eventId)
                {
                    case 0:
                        eventSpr.sprite = EventPrefab0.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 1:
                        eventSpr.sprite = EventPrefab1.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 2:
                        eventSpr.sprite = EventPrefab2.GetComponent<SpriteRenderer>().sprite;
                        break;
                }
            }
            VerticalLineEndManager.Instance.verticalLines.Add(eventColliderObj);

            // --- 横線の生成（既存の処理） ---
            if (i != count - 1)
            {
                int linesThisSegment = (i % 2 == 0) ? 3 : 2;

                for (int j = 0; j < linesThisSegment; j++)
                {
                    GameObject Yokoobj = Instantiate(HorizontalLinePrefab);
                    spawnedLines.Add(Yokoobj);

                    // 横線の見た目は使わないので非表示
                    Yokoobj.SetActive(false);

                    LineRenderer lrYoko = Yokoobj.GetComponent<LineRenderer>() ?? Yokoobj.AddComponent<LineRenderer>();
                    lrYoko.material = new Material(Shader.Find("Sprites/Default"));
                    lrYoko.startColor = Color.black;
                    lrYoko.endColor = Color.black;
                    lrYoko.startWidth = 0.06f;
                    lrYoko.endWidth = 0.06f;

                    float y = (i % 2 == 0) ? 1.6f - j * 1.6f : 0.8f - j * 1.6f;
                    lrYoko.positionCount = 2;
                    lrYoko.SetPosition(0, new Vector3(x, y, -1));
                    lrYoko.SetPosition(1, new Vector3(x + lineSpace, y, -1));

                    EdgeCollider2D edge = Yokoobj.GetComponent<EdgeCollider2D>();
                    if (edge != null)
                    {
                        Vector3 p0 = lrYoko.GetPosition(0);
                        Vector3 p1 = lrYoko.GetPosition(1);

                        // EdgeCollider2D はローカル座標
                        Vector2 localP0 = Yokoobj.transform.InverseTransformPoint(p0);
                        Vector2 localP1 = Yokoobj.transform.InverseTransformPoint(p1);

                        edge.points = new Vector2[] { localP0, localP1 };
                    }

                    // --- HorLineInfo 紐付け ---
                    if (horLineCounter < data.HorLineInfo.Count)
                    {
                        StartJson.HorLineInfo info = data.HorLineInfo[horLineCounter];

                        YokoObjComponent yokoComp = Yokoobj.AddComponent<YokoObjComponent>();
                        yokoComp.horLineId = horLineCounter;
                        yokoComp.info = info;

                        // ===== YokoFramePrefab を複製 =====
                        GameObject yokoFrameObj = Instantiate(YokoFramePrefab);
                        spawnedLines.Add(yokoFrameObj);

                        yokoFrameObj.transform.position = new Vector3(
                            x + lineSpace * 0.5f,
                            y,
                            -1
                        );

                        // サイズ調整（横線と同じ長さにする想定）
                        yokoFrameObj.transform.localScale = new Vector3(3f, 1f, 1.8f);

                        // 見た目を消す（クリックは有効）
                        SpriteRenderer frameSpr = yokoFrameObj.GetComponent<SpriteRenderer>();
                        if (frameSpr != null)
                        {
                            frameSpr.enabled = false; // これが重要
                        }

                        YokoFrame frame = yokoFrameObj.GetComponent<YokoFrame>() ?? yokoFrameObj.AddComponent<YokoFrame>();
                        frame.linkedYokoLine = Yokoobj;

                        // ===== 左端ボタン =====
                        GameObject leftButton = Instantiate(YokoBotton);
                        leftButton.transform.position = new Vector3(x, y, -1);
                        spawnedLines.Add(leftButton);
                        leftButton.SetActive(true);
                        SpriteRenderer leftSpr = leftButton.GetComponent<SpriteRenderer>();
                        if (leftSpr != null)
                        {
                            leftSpr.enabled = true;
                        }



                        // ===== 右端ボタン =====
                        GameObject rightButton = Instantiate(YokoBotton);
                        rightButton.transform.position = new Vector3(x + lineSpace, y, -1);
                        spawnedLines.Add(rightButton);
                        rightButton.SetActive(true);
                        SpriteRenderer rightSpr = rightButton.GetComponent<SpriteRenderer>();
                        if (rightSpr != null)
                        {
                            rightSpr.enabled = true;
                        }


                    }

                    horLineCounter++;

                }
            }
        }
    }
    private SaveData LoadJsonFile()
    {
        if (!File.Exists(jsonFilePath))
            return new SaveData
            {
                HorLineInfo = new List<StartJson.HorLineInfo>(),
                VarLineInfo = new List<StartJson.VarLineInfo>(),
                charaList = new List<StartJson.CharaData>()
            };

        string json = File.ReadAllText(jsonFilePath);
        SaveData data = JsonConvert.DeserializeObject<SaveData>(json);

        if (data.HorLineInfo == null) data.HorLineInfo = new List<StartJson.HorLineInfo>();
        if (data.VarLineInfo == null) data.VarLineInfo = new List<StartJson.VarLineInfo>();
        if (data.charaList == null) data.charaList = new List<StartJson.CharaData>();

        return data;
    }
}
