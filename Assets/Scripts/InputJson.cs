using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class InputJson : MonoBehaviour
{
    // ----------------------------
    // JSONクラス
    // ----------------------------
    [System.Serializable]
    public class SaveData
    {
        public int VarticalLineNumber;
        public List<StartJson.CharaData> charaList;
        public List<StartJson.FlagData> flagList;
        public List<StartJson.HorLineInfo> HorLineInfo;
        public List<StartJson.VarLineInfo> VarLineInfo;
    }

    [Header("UI References")]
    public TMP_InputField VarticalLineNumberInput;
    public GameObject VarticalLineNumberAlart;

    [Header("Prefab References")]
    public GameObject VarticalLinePrefab;
    public GameObject HorizontalLinePrefab;

    [Header("Collider Reference")]
    public GameObject CharaColliderPrefab;
    public GameObject EventColliderPrefab;

    [Header("Horizontal Line Info InputFields")]
    public TMP_InputField MoneyValueInput;
    public TMP_InputField HeartValueInput;
    public TMP_InputField EventContentInput;



    private string jsonFilePath;

    // 現在描画中の線を管理
    private List<GameObject> spawnedLines = new List<GameObject>();

    // シーン内で TateObj や CharaCollision を表す小さな MonoBehaviour を定義
    // （Prefab にそれぞれアタッチされていることを想定）
    public class CharaCollision : MonoBehaviour
    {
        public int varLineId;
    }

    public class EventCollision : MonoBehaviour
    {
        public int varLineId;
    }

    public class TateObj : MonoBehaviour
    {
        public int varLineId;
    }

    void Start()
    {
        if (VarticalLineNumberAlart != null)
            VarticalLineNumberAlart.SetActive(false);

        // JSON パス
        string sceneName = SceneManager.GetActiveScene().name;
        jsonFilePath = Path.Combine(Application.dataPath, $"Json/{sceneName}.json");

        // InputField の Enter 押下イベント登録
        if (VarticalLineNumberInput != null)
            VarticalLineNumberInput.onEndEdit.AddListener(OnInputEnd);
        if (MoneyValueInput != null) MoneyValueInput.onEndEdit.AddListener(OnMoneyValueEdited);
        if (HeartValueInput != null) HeartValueInput.onEndEdit.AddListener(OnHeartValueEdited);
        if (EventContentInput != null) EventContentInput.onEndEdit.AddListener(OnEventContentEdited);
    

    // 初回描画
    SaveData data = null;
        try
        {
            data = LoadJson();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadJson failed: " + ex.Message);
            data = new SaveData
            {
                VarticalLineNumber = 1,
                HorLineInfo = new List<StartJson.HorLineInfo>(),
                VarLineInfo = new List<StartJson.VarLineInfo>(),
                charaList = new List<StartJson.CharaData>()
            };
        }

        RedrawLines(data);

        if (VarticalLineNumberInput != null && data != null)
            VarticalLineNumberInput.text = data.VarticalLineNumber.ToString();
    }

    // ----------------------------
    // 入力確定時処理
    // ----------------------------
    private void OnInputEnd(string input)
    {
        if (!int.TryParse(input, out int num))
        {
            ShowAlert();
            return;
        }

        if (num < 1 || num > 6)
        {
            ShowAlert();
            return;
        }

        HideAlert();

        // JSON 更新・保存
        SaveData data = LoadJson();
        data.VarticalLineNumber = num;

        SaveJson(data);

        // 線を描き直す
        RedrawLines(data);
    }

    private void ShowAlert()
    {
        if (VarticalLineNumberAlart != null)
            VarticalLineNumberAlart.SetActive(true);
    }

    private void HideAlert()
    {
        if (VarticalLineNumberAlart != null)
            VarticalLineNumberAlart.SetActive(false);
    }

    public bool ToggleHorLine(int index)
    {
        SaveData data = LoadJson();

        if (index < 0 || index >= data.HorLineInfo.Count)
            return false;

        // トグル
        data.HorLineInfo[index].isActive = !data.HorLineInfo[index].isActive;

        // 保存
        SaveJson(data);

        return data.HorLineInfo[index].isActive;
    }

    // ----------------------------
    // 線を描き直す
    // ----------------------------
    private void RedrawLines(SaveData data)
    {
        // 既存線を削除
        foreach (var obj in spawnedLines)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedLines.Clear();

        if (data == null)
            data = LoadJson();

        if (data.HorLineInfo == null)
            data.HorLineInfo = new List<StartJson.HorLineInfo>();
        else
        {
            data.HorLineInfo.Clear();
            if (MoneyValueInput != null) MoneyValueInput.text = "";
            if (HeartValueInput != null) HeartValueInput.text = "";
            if (EventContentInput != null) EventContentInput.text = "";
            currentHorLineId = -1;
        }

        if (data.VarLineInfo == null)
            data.VarLineInfo = new List<StartJson.VarLineInfo>();
        else
            data.VarLineInfo.Clear();

        int count = Mathf.Max(1, data.VarticalLineNumber);
        float lineSpace = 1.0f;

        // 縦線描画（黒）
        for (int i = 0; i < count; i++)
        {
            float x = -3.5f - (count - 1) * 0.5f + i * lineSpace; // JSON の値に基づいて均等配置

            // --- 先に VarLineInfo を作って id を確定 ---
            StartJson.VarLineInfo varInfo = new StartJson.VarLineInfo();
            varInfo.varLineId = data.VarLineInfo.Count;
            varInfo.eventId = -1;
            varInfo.startCharaId = -1;
            data.VarLineInfo.Add(varInfo);
            int assignedvarLineId = varInfo.varLineId;

            // 縦線 Prefab を複製
            GameObject obj = Instantiate(VarticalLinePrefab);
            spawnedLines.Add(obj);

            int assignedVarLineId = assignedvarLineId;
            // TateObj があれば varLineId を渡す（安全チェック）
            TateObj tateComp = obj.GetComponent<TateObj>();
            if (tateComp == null)
            {
                // もし子に入っている可能性があるなら GetComponentInChildren も試す
                tateComp = obj.GetComponentInChildren<TateObj>();
            }
            if (tateComp != null)
            {
                tateComp.varLineId = assignedVarLineId;
            }

            LineRenderer lrTate = obj.GetComponent<LineRenderer>() ?? obj.AddComponent<LineRenderer>();
            lrTate.material = new Material(Shader.Find("Sprites/Default"));
            lrTate.startColor = Color.black;
            lrTate.endColor = Color.black;
            lrTate.startWidth = 0.06f;
            lrTate.endWidth = 0.06f;

            lrTate.positionCount = 2;
            lrTate.SetPosition(0, new Vector3(x, 0.6f, -1));
            lrTate.SetPosition(1, new Vector3(x, -3, -1));

            // CharaCollider を複製して varLineId を設定
            GameObject charaColliderObj = Instantiate(CharaColliderPrefab);
            spawnedLines.Add(charaColliderObj);
            charaColliderObj.transform.position = new Vector3(x, 1.1f, -2);

            // CharaCollision コンポーネントが prefab についている想定 → 値を渡す
            CharaCollision chComp = charaColliderObj.GetComponent<CharaCollision>();
            if (chComp == null)
            {
                chComp = charaColliderObj.GetComponentInChildren<CharaCollision>();
            }
            if (chComp != null)
            {
                chComp.varLineId = assignedVarLineId;
            }

            // EventCollider を複製（位置は縦線の終点 -0.5）
            GameObject eventColliderObj = Instantiate(EventColliderPrefab);
            spawnedLines.Add(eventColliderObj);
            eventColliderObj.transform.position = new Vector3(x, -3.5f, -2);

            // --- 横線の生成（既存の処理） ---
            if (i != count - 1)
            {
                if (i % 2 == 0)
                {
                    // 横線描画（半透明灰色）
                    for (int j = 0; j < 3; j++)
                    {
                        GameObject Yokoobj = Instantiate(HorizontalLinePrefab);
                        spawnedLines.Add(Yokoobj);

                        LineRenderer lrYoko = Yokoobj.GetComponent<LineRenderer>() ?? Yokoobj.AddComponent<LineRenderer>();
                        lrYoko.material = new Material(Shader.Find("Sprites/Default"));

                        lrYoko.startWidth = 0.06f;
                        lrYoko.endWidth = 0.06f;

                        float y = -0.3f - j * 0.9f;
                        lrYoko.positionCount = 2;
                        lrYoko.SetPosition(0, new Vector3(x, y, 0));
                        lrYoko.SetPosition(1, new Vector3(x + lineSpace, y, 0));

                        StartJson.HorLineInfo info = new StartJson.HorLineInfo();
                        info.horLineId = data.HorLineInfo.Count;
                        info.isActive = false;
                        info.moneyValue = 0;
                        info.heartValue = 0;
                        info.eventText = null;
                        data.HorLineInfo.Add(info);

                        bool isActive = info.isActive;
                        if (isActive)
                        {
                            lrYoko.startColor = Color.black;
                            lrYoko.endColor = Color.black;
                        }
                        else
                        {
                            lrYoko.startColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
                            lrYoko.endColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
                        }

                        YokoLineClick click = Yokoobj.AddComponent<YokoLineClick>();
                        click.Init(this, info.horLineId, lrYoko);
                        click.UpdateColor(isActive);
                    }
                }
                else if (i % 2 == 1)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        GameObject Yokoobj = Instantiate(HorizontalLinePrefab);
                        spawnedLines.Add(Yokoobj);

                        LineRenderer lrYoko = Yokoobj.GetComponent<LineRenderer>() ?? Yokoobj.AddComponent<LineRenderer>();
                        lrYoko.material = new Material(Shader.Find("Sprites/Default"));

                        lrYoko.startWidth = 0.06f;
                        lrYoko.endWidth = 0.06f;

                        float y = -0.75f - j * 0.9f;
                        lrYoko.positionCount = 2;
                        lrYoko.SetPosition(0, new Vector3(x, y, 0));
                        lrYoko.SetPosition(1, new Vector3(x + lineSpace, y, 0));

                        StartJson.HorLineInfo info = new StartJson.HorLineInfo();
                        info.horLineId = data.HorLineInfo.Count;
                        info.isActive = false;
                        data.HorLineInfo.Add(info);
                        bool isActive = info.isActive;

                        if (isActive)
                        {
                            lrYoko.startColor = Color.black;
                            lrYoko.endColor = Color.black;
                        }
                        else
                        {
                            lrYoko.startColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
                            lrYoko.endColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
                        }

                        YokoLineClick click = Yokoobj.AddComponent<YokoLineClick>();
                        click.Init(this, info.horLineId, lrYoko);
                        click.UpdateColor(isActive);
                    }
                }
            }
        }

        // 生成が終わったら保存（VarLineInfo などが更新されている）
        SaveJson(data);
    }

    // ----------------------------
    // JSON読み込み/保存
    // ----------------------------
    private SaveData LoadJson()
    {
        if (!File.Exists(jsonFilePath)) return new SaveData { HorLineInfo = new List<StartJson.HorLineInfo>(), VarLineInfo = new List<StartJson.VarLineInfo>() };
        string json = File.ReadAllText(jsonFilePath);
        SaveData data = JsonConvert.DeserializeObject<SaveData>(json);

        if (data.HorLineInfo == null) data.HorLineInfo = new List<StartJson.HorLineInfo>();
        if (data.VarLineInfo == null) data.VarLineInfo = new List<StartJson.VarLineInfo>();
        if (data.charaList == null) data.charaList = new List<StartJson.CharaData>();

        return data;
    }

    private void SaveJson(SaveData data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(jsonFilePath, json);
    }

    public void SetCharaStartPos(int charaIndex, int newStartPosition)
    {
        SaveData data = LoadJson();

        if (data.charaList == null || charaIndex < 0 || charaIndex >= data.charaList.Count)
            return;

        // 値が変わった時だけ保存（無駄な書き込みを防ぐ）
        if (data.charaList[charaIndex].StartPosition != newStartPosition)
        {
            data.charaList[charaIndex].StartPosition = newStartPosition;
            SaveJson(data);
            Debug.Log($"[InputJson] Saved: chara {charaIndex} StartPosition = {newStartPosition}");
        }
    }

    public void OnCharaAttached(int charaIndex, int newStartPosition)
    {
        SetCharaStartPos(charaIndex, newStartPosition);
    }

    // ----------------------------
    // DragToEvent から呼ばれる
    // ----------------------------
    public void OnEventAttached(int eventId, int varLineId)
    {
        SaveData data = LoadJson();

        if (data.VarLineInfo == null)
            data.VarLineInfo = new List<StartJson.VarLineInfo>();

        // varLineId に対応する VarLineInfo を探す
        StartJson.VarLineInfo varInfo = data.VarLineInfo.Find(v => v.varLineId == varLineId);
        if (varInfo != null)
        {
            // eventId を設定
            varInfo.eventId = eventId;
        }
        else
        {
            // 万が一存在しなければ新規作成して追加
            varInfo = new StartJson.VarLineInfo()
            {
                varLineId = varLineId,
                eventId = eventId
            };
            data.VarLineInfo.Add(varInfo);
        }

        // 即保存
        SaveJson(data);
    }

    public void OnEventSpriteChanged(int eventId, int varLineId)
    {
        SaveData data = LoadJson();

        if (data.VarLineInfo == null)
            data.VarLineInfo = new List<StartJson.VarLineInfo>();

        StartJson.VarLineInfo varInfo = data.VarLineInfo.Find(v => v.varLineId == varLineId);
        if (varInfo != null)
        {
            varInfo.eventId = eventId;
        }
        else
        {
            varInfo = new StartJson.VarLineInfo()
            {
                varLineId = varLineId,
                eventId = eventId
            };
            data.VarLineInfo.Add(varInfo);
        }

        SaveJson(data);
    }

    public void OnYokoLineRightClicked(int horLineId)
    {
        SaveData data = LoadJson();
        StartJson.HorLineInfo info = data.HorLineInfo.Find(h => h.horLineId == horLineId);
        if (info == null) return;
        currentHorLineId = horLineId;
        if (MoneyValueInput != null) MoneyValueInput.text = info.moneyValue.ToString();
        if (HeartValueInput != null) HeartValueInput.text = info.heartValue.ToString();
        if (EventContentInput != null) EventContentInput.text = info.eventText ?? "";
    }

    // ----------------------------
    // InputField編集時
    // ----------------------------
    private void OnMoneyValueEdited(string value)
    {
        if (!int.TryParse(value, out int result)) return;
        UpdateCurrentHorLineValue(money: result);
    }

    private void OnHeartValueEdited(string value)
    {
        if (!int.TryParse(value, out int result)) return;
        UpdateCurrentHorLineValue(heart: result);
    }

    private void OnEventContentEdited(string value)
    {
        UpdateCurrentHorLineValue(eventText: value);
    }

    private int currentHorLineId = -1;

    // 横線クリックでどの HorLineId に対応しているかセット
    public void SetCurrentHorLineId(int horLineId)
    {
        currentHorLineId = horLineId;
    }

    private void UpdateCurrentHorLineValue(int? money = null, int? heart = null, string eventText = null)
    {
        if (currentHorLineId < 0) return;

        SaveData data = LoadJson();
        StartJson.HorLineInfo info = data.HorLineInfo.Find(h => h.horLineId == currentHorLineId);
        if (info == null) return;

        if (money.HasValue) info.moneyValue = money.Value;
        if (heart.HasValue) info.heartValue = heart.Value;
        if (eventText != null) info.eventText = eventText;

        SaveJson(data);
    }
}
