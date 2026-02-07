using UnityEngine;

public class YokoLineClick : MonoBehaviour
{
    public int index;
    public InputJson inputJson;
    private LineRenderer lr;

    public void Init(InputJson json, int id, LineRenderer line)
    {
        inputJson = json;
        index = id;
        lr = line;

        AutoFitCollider2D(lr);
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bool newValue = inputJson.ToggleHorLine(index);
            UpdateColor(newValue);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (inputJson != null)
            {
                inputJson.OnYokoLineRightClicked(index);
                inputJson.SetCurrentHorLineId(index); // 選択中の横線IDを更新
            }
        }
    }

    public void UpdateColor(bool isActive)
    {
        if (lr == null)
        {
            Debug.LogError("LineRenderer がセットされていません：" + gameObject.name);
            return;
        }

        if (isActive)
        {
            lr.startColor = Color.black;
            lr.endColor = Color.black;
        }
        else
        {
            lr.startColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            lr.endColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }

    public void AutoFitCollider2D(LineRenderer lr)
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) col = gameObject.AddComponent<BoxCollider2D>();

        Vector3 p0 = lr.GetPosition(0);
        Vector3 p1 = lr.GetPosition(1);

        float length = Vector3.Distance(p0, p1);

        col.size = new Vector2(length, 0.15f);  
        col.offset = (p0 + p1) / 2f - transform.position;
    }
}
