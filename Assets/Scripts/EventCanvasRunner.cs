using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventCanvasRunner : MonoBehaviour
{
    public static EventCanvasRunner Instance { get; private set; }



    private const float START_Y = 710f;
    private const float STEP_Y = 260f;
    private const float MIN_Y = -300f;

    private const int WAIT_BEFORE_NEXT = 300;
    private const int SHIFT_FRAMES = 90;

    private float nextTextY = START_Y;

    private readonly List<TextMeshProUGUI> activeTexts
        = new List<TextMeshProUGUI>();

    private readonly Queue<(GameObject prefab, string text, Transform parent)>
        queue = new();

    private bool isProcessing = false;

    private void Awake()
    {
        Instance = this;
    }

    public void Enqueue(
        GameObject prefab,
        string eventText,
        Transform parent = null
    )
    {
        queue.Enqueue((prefab, eventText, parent));

        if (!isProcessing)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (queue.Count > 0)
        {
            var (prefab, text, parent) = queue.Dequeue();

            // --- 生成 ---
            GameObject canvas = Instantiate(prefab, parent);
            TextMeshProUGUI tmp =
                canvas.GetComponentInChildren<TextMeshProUGUI>(true);

            tmp.text = text;

            RectTransform rt = tmp.rectTransform;
            rt.anchoredPosition =
                new Vector2(rt.anchoredPosition.x, nextTextY);

            activeTexts.Add(tmp);
            nextTextY -= STEP_Y;

            // --- 待機 ---
            for (int i = 0; i < WAIT_BEFORE_NEXT; i++)
                yield return null;

            // --- 下限チェック ---
            if (nextTextY <= MIN_Y)
            {
                yield return ShiftAllTextsUp();
                nextTextY += STEP_Y;
            }
        }

        isProcessing = false;
    }

    // ===============================
    // ★ 全体シフト
    // ===============================
    private IEnumerator ShiftAllTextsUp()
    {
        var snapshot = activeTexts.ToArray();
        int count = snapshot.Length;

        Vector2[] start = new Vector2[count];
        for (int i = 0; i < count; i++)
        {
            if (!snapshot[i]) continue;
            if (!snapshot[i].rectTransform) continue;

            start[i] = snapshot[i].rectTransform.anchoredPosition;
        }

        for (int f = 0; f < SHIFT_FRAMES; f++)
        {
            float t = (float)f / SHIFT_FRAMES;

            for (int i = 0; i < count; i++)
            {
                if (!snapshot[i]) continue;
                if (!snapshot[i].rectTransform) continue;

                snapshot[i].rectTransform.anchoredPosition =
                    Vector2.Lerp(start[i], start[i] + Vector2.up * STEP_Y, t);
            }

            yield return null;
        }

        for (int i = 0; i < count; i++)
        {
            if (!snapshot[i]) continue;
            if (!snapshot[i].rectTransform) continue;

            snapshot[i].rectTransform.anchoredPosition =
                start[i] + Vector2.up * STEP_Y;
        }

        // ★ ここで死んでる参照を掃除
        activeTexts.RemoveAll(t => !t || !t.rectTransform);
    }
}


