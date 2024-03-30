using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static GameManager instance;

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
    private int minimumPlayer;
    [SerializeField]
    private int maximumPlayer;
    [SerializeField]
    private bool test;

    [Header("Audios")]
    [SerializeField]
    private AudioSource _bgm;

    private Coroutine _startCoutdownCoroutine;

    public List<Character> characters => _characters;

    public static UnityAction<List<Character>> onUpdateCharacters;
    public static UnityAction<State> onUpdateState;
    public static UnityAction<string> onCreatedRoom;
    public static UnityAction onCountdown;
    public UnityAction<int> onTimeUpdate;

    public const byte GAME_START = 1;

    private void Awake()
    {
        instance = this;
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
        Debug.Log("Connected");
        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maximumPlayer + 1;
        roomOptions.IsVisible = true;

        string room = test ? "TEST" : Random.Range(1000, 10000).ToString();
        PhotonNetwork.CreateRoom(room, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
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

        if (_characters.Count == 0 && _currentState == State.GAMEPLAY)
        {
            PhotonNetwork.Disconnect();
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
        if (_characters.Count >= minimumPlayer && _characters.All(x => x.isReady))
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

                foreach (Character character in _characters)
                {
                    character.isReady = false;
                }
                onUpdateCharacters?.Invoke(_characters);

                _bgm.Play();
                break;
            case State.RESULT:
                onTimeUpdate?.Invoke((int)_duration);
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies)
                {
                    Destroy(enemy);
                }

                _bgm.Stop();
                break;
            default:
                break;
        }

        _currentState = state;
        onUpdateState?.Invoke(_currentState);
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
                    onTimeUpdate?.Invoke((int)_duration);
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        Transition.instance.FadeIn(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }
}
