using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuHud : MonoBehaviour
{
    [Header("Lobby")]
    [SerializeField]
    private RawImage _qrCode;
    [SerializeField]
    private GameObject _qrLoading;
    [SerializeField]
    private Animator _anim;

    private void Awake()
    {
        GameManager.onCreatedRoom += GenerateQrCode;
        GameManager.onCountdown += Countdown;
    }

    private void OnDestroy()
    {
        GameManager.onCreatedRoom -= GenerateQrCode;
        GameManager.onCountdown -= Countdown;
    }

    private void GenerateQrCode(string room)
    {
        _qrLoading.SetActive(false);
        _qrCode.texture = QRCode.TextToQR(room);
    }

    private void Countdown()
    {
        _anim.SetBool("Start", true);
    }
}
