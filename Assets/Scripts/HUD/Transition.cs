using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Transition : MonoBehaviour
{
    public static Transition instance;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        FadeOut();
    }

    public void FadeIn(UnityAction callback = null)
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.LeanAlpha(1, 0.5f).setOnComplete(() => callback?.Invoke());
    }

    public void FadeOut(UnityAction callback = null)
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.LeanAlpha(0, 0.5f).setOnComplete(() => callback?.Invoke());
    }
}
