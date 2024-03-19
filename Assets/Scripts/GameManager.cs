using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;
using UnityEngine.Events;

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public enum State
    {
        MENU,
        GAMEPLAY,
        RESULT
    }
    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private List<Character> _characters = new List<Character>();

    [Header("Game Setting")]
    [SerializeField]
    private State _currentState;
    [SerializeField]
    private float _duration;

    [Header("Server Setting")]
    [SerializeField]
    private string endpoint;
    [SerializeField]
    private bool test;

    private Coroutine _startCoutdownCoroutine;

    public static UnityAction<List<Character>> onUpdateCharacters;
    public static UnityAction<State> onUpdateState;
    public static UnityAction<string> onCreatedRoom;
    public static UnityAction onCountdown;

    public const byte GAME_START = 1;

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Transition.instance.FadeOut();
    }

    private void Update()
    {
        StateUpdate();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        CreateRoom();
    }

    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        roomOptions.IsVisible = true;

        string room = test ? "TEST" : Random.Range(1000, 10000).ToString();
        PhotonNetwork.CreateRoom(room, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        string url = $"https://{endpoint}/?{PhotonNetwork.CurrentRoom.Name}";
        onCreatedRoom?.Invoke(url);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} joined game");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Character character = _characters.First(x => x.id == otherPlayer.NickName);
        _characters.Remove(character);
        if (character.gameObject != null)
        {
            Destroy(character.gameObject);
        }

        if (_characters.Count == 0)
        {
            //Restart Game
        }

        onUpdateCharacters?.Invoke(_characters);
        CheckAllPlayerIsReady();
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == PlayerController.REGISTER_PLAYER)
        {
            GameObject Go = Instantiate(_playerPrefab);
            object[] data = (object[])photonEvent.CustomData;

            if (Go.TryGetComponent(out Character character))
            {
                character.id = (string)data[0];
                character.color = new Color((float)data[1], (float)data[2], (float)data[3], (float)data[4]);
                _characters.Add(character);
            }

            onUpdateCharacters?.Invoke(_characters);
        }
        else if (eventCode == PlayerController.READY)
        {
            object[] data = (object[])photonEvent.CustomData;
            string id = (string)data[0];

            Character character = _characters.First(x => x.id == id);
            character.isReady = (bool)data[1];

            onUpdateCharacters?.Invoke(_characters);
            CheckAllPlayerIsReady();           
        }
    }

    private void CheckAllPlayerIsReady()
    {
        if (_characters.Count > 1 && _characters.All(x => x.isReady))
        {
            if (_startCoutdownCoroutine != null)
            {
                StopCoroutine(_startCoutdownCoroutine);
                _startCoutdownCoroutine = null;
            }

            _startCoutdownCoroutine = StartCoroutine(StartCountdown());
        }
        else
        {
            if (_startCoutdownCoroutine != null)
            {
                StopCoroutine(_startCoutdownCoroutine);
                _startCoutdownCoroutine = null;
            }
        }
    }

    private IEnumerator StartCountdown()
    {
        float duration = 3f;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        onCountdown?.Invoke();
        yield return new WaitForSeconds(4.5f);
        ChangeState(State.GAMEPLAY);
    }

    private void ChangeState(State state)
    {
        if (_currentState == state) return;

        switch (state)
        {
            case State.MENU:
                break;
            case State.GAMEPLAY:
                object[] content = new object[] {};
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(GAME_START, content, raiseEventOptions, SendOptions.SendReliable);
                break;
            case State.RESULT:
                break;
            default:
                break;
        }

        _currentState = state;
    }

    private void StateUpdate()
    {
        switch (_currentState)
        {
            case State.MENU:
                break;
            case State.GAMEPLAY:
                if (_duration > 0)
                {
                    _duration -= Time.deltaTime;
                }
                else
                {
                    ChangeState(State.RESULT);
                }
                break;
            case State.RESULT:
                break;
            default:
                break;
        }
    }
}
