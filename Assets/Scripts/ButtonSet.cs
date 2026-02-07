using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ButtonSet : MonoBehaviour
{
    [SerializeField] Button button;

    public static event Action OnWalkRequested;

    void Awake()
    {
        button.onClick.AddListener(() =>
        {
            Debug.Log("Button Clicked");
            OnWalkRequested?.Invoke();
        });
    }
}
