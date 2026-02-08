using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Charawalk : MonoBehaviour
{
    // ===============================
    // 外部公開プロパティ
    // ===============================
    public int CharaId { get; private set; }
    public CharaStatus Status { get; private set; }
   public bool IsWalkingDone { get; private set; } = false;

    // 初期化済みか
    public bool isInitialized = false;

    // EventCanvas の複製元プレハブ
    [SerializeField] private GameObject eventCanvasPrefab;

    [SerializeField] private UnityEngine.UI.Button targetButton;
    [SerializeField] public GameObject EndEvent;



    // ===============================
    // 内部状態
    // ===============================
    private bool isWalking = false;

    private void Awake()
    {
        Debug.Log($"Awake: {gameObject.name} / instance {GetInstanceID()}");
        targetButton.onClick.AddListener(OnButtonClicked);
       
    }

    // ===============================
    // 初期化（生成時に必ず呼ばれる想定）
    // ===============================
    public void Init(int charaId, StartJson.CharaData charaData)
    {
        Debug.Log("Start");
        Debug.Log("Button: " + targetButton);
        CharaId = charaId;
        Status = new CharaStatus(charaData);
        isInitialized = true;
    }

    void OnEnable()
    {
        ButtonSet.OnWalkRequested += OnButtonClicked;
        Debug.Log($"OnEnable {gameObject.name} frame:{Time.frameCount}");
    }

    void OnDisable()
    {
        ButtonSet.OnWalkRequested -= OnButtonClicked;
        Debug.Log($"OnButtonClicked {gameObject.name} frame:{Time.frameCount}");
    }


    // ===============================
    // Update
    // ===============================



    // ===============================
    // メイン移動ルーチン
    // ===============================
    private IEnumerator WalkRoutine()
    {
        // スタート直後の縦移動
        yield return MoveY(-0.04f, 40);

        // 横線段（固定数）
        for (int i = 0; i < 6; i++)
        {
            yield return HandleYokoObjInteraction();
            yield return MoveY(-0.04f, 20);
        }
        IsWalkingDone = true;
        VerticalLineEndManager.Instance.Decrement();

    }

    // ===============================
    // 縦移動
    // ===============================
    private IEnumerator MoveY(float deltaY, int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            transform.position += Vector3.up * deltaY;
            yield return null;
        }
    }

    // ===============================
    // 横移動
    // ===============================
    private IEnumerator MoveX(Vector2 target, int frames)
    {
        Vector2 start = transform.position;
        Vector2 delta = (target - start) / frames;

        for (int i = 0; i < frames; i++)
        {
            transform.position += new Vector3(delta.x, 0f, 0f);
            yield return null;
        }

        // 誤差補正
        transform.position = new Vector3(
            target.x,
            transform.position.y,
            transform.position.z
        );
    }

    // ===============================
    // 横線チェック＆イベント通知
    // ===============================
    private IEnumerator HandleYokoObjInteraction()
    {
        // 物理更新と同期
        yield return new WaitForFixedUpdate();

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (var col in hits)
        {
            if (!col.CompareTag("YokoObj"))
                continue;

            // 横線コンポーネント取得
            LoadJson.YokoObjComponent yoko =
                col.GetComponent<LoadJson.YokoObjComponent>();

            if (yoko != null)
            {
                EventCanvasManager.TryRequestEventCanvas(
                    yoko.horLineId,
                    eventCanvasPrefab,
                    yoko.info.eventText
                );
            }

            // ===============================
            // 横移動処理
            // ===============================
            LineRenderer lr = col.GetComponent<LineRenderer>();
            if (lr == null)
                yield break;

            Vector2 start = lr.GetPosition(0);
            Vector2 end = lr.GetPosition(1);

            Vector2 current = transform.position;
            Vector2 target =
                Vector2.Distance(current, start) >
                Vector2.Distance(current, end)
                    ? start
                    : end;

           

            yield return MoveX(target, 180);
            for (int i = 0; i < 60; i++)
                yield return null;
            yield break;
        }

        // 横線がなければ待機
        for (int i = 0; i < 240;i++)
            yield return null;
    }

   

    private void OnButtonClicked()
    {
        Debug.Log("ボタン押下を受信");
        Debug.Log($"Charawalk {CharaId} received");
        if (!isInitialized) return;

        if (!isWalking)
        {
            isWalking = true;
            StartCoroutine(WalkRoutine());
        }


    }
}


