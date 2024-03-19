using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public string id;
    public Color color;

    [Header("JOIN HUD")]
    [SerializeField]
    private GameObject _joinHud;
    [SerializeField]
    private TMP_InputField _nameInput;
    [SerializeField]
    private Button _joinButton;
    [SerializeField]
    private Image _thumbnail;
    [SerializeField]
    private Button[] _colorButtons;

    [Header("GAMEPLAY HUD")]
    [SerializeField]
    private GameObject _gameplayHud;
    [SerializeField]
    private FixedJoystick _moveJoystick;
    [SerializeField]
    private FixedJoystick _attackJoystick;

    [Header("Ready HUD")]
    [SerializeField]
    private GameObject _readyHud;
    [SerializeField]
    private Button _readyButton;
    [SerializeField]
    private Button _unreadyButton;

    [Header("NOTIFICATION HUD")]
    [SerializeField]
    private GameObject _notificationHud;
    [SerializeField]
    private TMP_Text _notificationText;

    [Header("OTHER HUD")]
    [SerializeField]
    private GameObject _loadingPanel;

    public const byte REGISTER_PLAYER = 10;
    public const byte READY = 11;
    public const byte MOVE_CHARACTER = 12;
    public const byte ATTACK = 13;

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {
        _joinHud.SetActive(true);
        _gameplayHud.SetActive(false);
        foreach (Button button in _colorButtons)
        {
            Color color = button.image.color;
            button.onClick.AddListener(() =>
            {
                _thumbnail.color = color;
                this.color = color;
            });
        }
        _joinButton.onClick.AddListener(JoinGame);
        _readyButton.onClick.AddListener(() =>
        {
            Ready(true);
            _unreadyButton.gameObject.SetActive(true);
        });
        _unreadyButton.onClick.AddListener(() =>
        {
            Ready(false);
            _unreadyButton.gameObject.SetActive(false);
        });

        _loadingPanel.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        _loadingPanel.SetActive(false);
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            Move();
            Shoot();
        }
    }

    #region CONNECTING
    private void JoinGame()
    {
        PhotonNetwork.NickName = _nameInput.text;
        string room = "TEST";

#if UNITY_WEBGL && !UNITY_EDITOR
        int pm = Application.absoluteURL.IndexOf("?");
        if (pm != -1)
        {
            room = Application.absoluteURL.Split("?"[0])[1];
        }
#endif

        PhotonNetwork.JoinRoom(room);

        id = PhotonNetwork.NickName;
        _loadingPanel.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        _joinHud.SetActive(false);
        _gameplayHud.SetActive(true);
        _loadingPanel.SetActive(false);

        object[] content = new object[] { id, color.r, color.g, color.b, color.a };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(REGISTER_PLAYER, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogFormat("error {0}, {1}", returnCode, message);
        _loadingPanel.SetActive(false);

        if (returnCode == 32758)
        {
            Notification("Please wait for next game.");
        }
        else
        {
            Notification(message);
        }
    }
#endregion

    private void Notification(string message)
    {
        _notificationText.text = message;
        _notificationHud.SetActive(true);
    }

    private void Ready(bool ready)
    {
        object[] content = new object[] { id, ready };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(READY, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void GameStarted()
    {
        _readyHud.SetActive(false);
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (_moveJoystick != null)
        {
            horizontal = _moveJoystick.Horizontal;
            vertical = _moveJoystick.Vertical;
        }

        object[] content = new object[] { id, horizontal, vertical };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(MOVE_CHARACTER, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void Shoot()
    {
        object[] content = new object[] { id, _attackJoystick.Horizontal, _attackJoystick.Vertical };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(ATTACK, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == GameManager.GAME_START)
        {
            GameStarted();
        }
    }
}
