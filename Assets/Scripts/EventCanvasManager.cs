using System.Collections.Generic;
using UnityEngine;

public static class EventCanvasManager
{
    private static readonly HashSet<int> spawnedHorLineIds
        = new HashSet<int>();

    public static bool TryRequestEventCanvas(
        int horLineId,
        GameObject prefab,
        string eventText,
        Transform parent = null
    )
    {
        if (!spawnedHorLineIds.Add(horLineId))
            return false;

        if (string.IsNullOrEmpty(eventText))
            return false;

        EventCanvasRunner.Instance.Enqueue(
            prefab,
            eventText,
            parent
        );

        return true;
    }

    public static void Reset()
    {
        spawnedHorLineIds.Clear();
    }
}
