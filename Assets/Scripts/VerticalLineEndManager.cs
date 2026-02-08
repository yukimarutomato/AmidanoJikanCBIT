using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VerticalLineEndManager : MonoBehaviour
{
    public static VerticalLineEndManager Instance { get; private set; }

    // ===============================
    // Inspector 設定
    // ===============================
    [Header("End表示用")]
    [SerializeField] private GameObject EndChara;
    [SerializeField] private TextMeshProUGUI EndText;
    [SerializeField] private GameObject Fukidashi;

    [Header("アイコン")]
    [SerializeField] private Sprite Icon0;
    [SerializeField] private Sprite Icon1;
    [SerializeField] private Sprite Icon2;
    [SerializeField] private Sprite Icon3;
    [SerializeField] private Sprite Icon4;
    [SerializeField] private Sprite Icon5;

    [Header("VarLine 表示順")]
    [Tooltip("VarLineId を表示したい順に並べる")]
    [SerializeField] private List<int> varLineOrder = new();

    [Header("表示待機時間")]
    [SerializeField] private float waitSeconds = 2.5f;

    public int WalkEndCounter = 6;

    // ===============================
    // 内部状態
    // ===============================

    // varLineId → charaId
    private Dictionary<int, int> arrivedCharas = new();

    private bool isSequenceStarted = false;

    // ===============================
    // Unity Lifecycle
    // ===============================
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Fukidashi.SetActive(false);
    }

    public void Update()
    {
        // デバッグ用：Eキーで演出開始
        if (WalkEndCounter==0)
        {
            if (!isSequenceStarted)
            {
                isSequenceStarted = true;
                StartCoroutine(EndSequence());
            }
        }
    }
    // ===============================
    // EndCollision からの通知口
    // ===============================
    public void NotifyArrived(int varLineId, int charaId)
    {
        // 重複通知防止
        if (arrivedCharas.ContainsKey(varLineId))
            return;

        arrivedCharas[varLineId] = charaId;

        Debug.Log($"Arrived : varLineId={varLineId}, charaId={charaId}");

        // 全ライン到達したら演出開始
        if (WalkEndCounter==0)
        {
            isSequenceStarted = true;
            StartCoroutine(EndSequence());
        }
    }

    // ===============================
    // End 演出本体
    // ===============================
    private IEnumerator EndSequence()
    {
        Debug.Log("EndSequence Start");
        Debug.Log($"設定された表示順リストの数: {varLineOrder.Count}");
        Debug.Log($"到着済みキャラデータの数: {arrivedCharas.Count}");

        foreach (var pair in arrivedCharas)
        {
            Debug.Log($"辞書の中身 -> Key(VarLineId): {pair.Key}, Value(CharaId): {pair.Value}");
        }

        foreach (int lineId in varLineOrder)
        {
            if (!arrivedCharas.TryGetValue(lineId, out int charaId))
                continue;

            SetEndCharaSprite(charaId);
            SetEndText(lineId);

            Fukidashi.SetActive(true);
            Debug.Log("吹き出しを表示");

            yield return new WaitForSeconds(waitSeconds);
        }

        Debug.Log($"[FUKIDASHI CHECK] name={Fukidashi.name} instanceID={Fukidashi.GetInstanceID()} activeSelf={Fukidashi.activeSelf}");



        Debug.Log("EndSequence Finished");
    }

    // ===============================
    // 表示制御
    // ===============================
    private void SetEndCharaSprite(int charaId)
    {
        var sr = EndChara.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        sr.sprite = charaId switch
        {
            0 => Icon0,
            1 => Icon1,
            2 => Icon2,
            3 => Icon3,
            4 => Icon4,
            5 => Icon5,
            _ => sr.sprite
        };
    }

    private void SetEndText(int varLineId)
    {
        EndText.text = varLineId switch
        {
            0 => "遊園地に遊びに行って、親友ができた！",
            1 => "バイト代をゲット！頑張った甲斐があった",
            2 => "多忙で体調を崩してしまった…",
            3 => "友達とご飯を食べに行って、親友になった！",
            4 => "今月のお小遣いが手に入った！やった～",
            5 => "温泉に入って、日々の疲れを癒した",
            _ => ""
        };
    }

    public void Decrement() { WalkEndCounter--; }
}
