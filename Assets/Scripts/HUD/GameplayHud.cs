using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayHud : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private TMP_Text _timeText;
    [SerializeField]
    private GameObject _gameoverPopup;
    [SerializeField]
    private PlayerItemHud[] _playerPanels;   

    private void Awake()
    {
        GameManager.onUpdateCharacters += UpdatePlayerPanels;
        GameManager.onUpdateState += CheckGameState;
        GameManager.instance.onTimeUpdate += UpdateTimeText;
        Character.onScoreUpdate += UpdateScore;
    }

    private void OnDestroy()
    {
        GameManager.onUpdateCharacters -= UpdatePlayerPanels;
        GameManager.onUpdateState -= CheckGameState;
        GameManager.instance.onTimeUpdate -= UpdateTimeText;
        Character.onScoreUpdate -= UpdateScore;
    }

    private void UpdatePlayerPanels(List<Character> characters)
    {
        foreach (PlayerItemHud item in _playerPanels)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < characters.Count; i++)
        {
            _playerPanels[i].Init(characters[i].id, characters[i].color, characters[i].isReady);
            _playerPanels[i].UpdateScoreText(characters[i].score);
            _playerPanels[i].gameObject.SetActive(true);
        }
    }

    private void UpdateScore()
    {
        List<Character> characters = GameManager.instance.characters;

        for (int i = 0; i < characters.Count; i++)
        { 
            _playerPanels[i].UpdateScoreText(characters[i].score);
        }
    }

    private void CheckGameState(GameManager.State state)
    {
        if (state == GameManager.State.GAMEPLAY)
        {
            _timeText.gameObject.SetActive(true);
        }
        if (state == GameManager.State.RESULT)
        {
            _timeText.gameObject.SetActive(false);
            StartCoroutine(GameoverCoroutine());
        }
    }

    IEnumerator GameoverCoroutine()
    {
        _gameoverPopup.SetActive(true);
        yield return new WaitForSeconds(2f);

        _canvasGroup.LeanAlpha(0f, 0.5f);
    }

    private void UpdateTimeText(int time)
    {
        _timeText.text = time.ToString();
    }
}
