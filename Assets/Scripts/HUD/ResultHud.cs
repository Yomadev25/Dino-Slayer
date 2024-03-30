using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultHud : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private TMP_Text _restartText;
    [SerializeField]
    private Scrollbar _scrollBar;

    [Header("Ranking Template")]
    [SerializeField]
    private GameObject _rankingTemplate;
    [SerializeField]
    private TMP_Text _nameText;
    [SerializeField]
    private TMP_Text _scoreText;
    [SerializeField]
    private Image _profileImage;
    [SerializeField]
    private Transform _contentRoot;

    private void Awake()
    {
        GameManager.onUpdateState += CheckGameState;
    }

    private void OnDestroy()
    {
        GameManager.onUpdateState -= CheckGameState;
    }

    private void CheckGameState(GameManager.State state)
    {
        if (state == GameManager.State.RESULT)
        {
            StartCoroutine(GameoverCoroutine());
        }
    }

    IEnumerator GameoverCoroutine()
    {
        yield return new WaitForSeconds(2.5f);        
        _canvasGroup.LeanAlpha(1f, 0.5f);

        Character[] characters = FindObjectsOfType<Character>();
        var rankOrder = characters.OrderByDescending(x => x.score).ToArray();

        for (int i = 0; i < rankOrder.Length; i++)
        {
            _nameText.text = rankOrder[i].id;
            _scoreText.text = rankOrder[i].score.ToString();
            _profileImage.color = rankOrder[i].color;

            GameObject GO = Instantiate(_rankingTemplate, _contentRoot);
            GO.SetActive(true);

            yield return new WaitForSeconds(0.2f);
        }

        LeanTween.value(0, 1, 5f).setOnUpdate((x) =>
        {
            _scrollBar.value = x;
        }).setLoopPingPong();

        float duration = 10f;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            _restartText.text = $"RESTART IN {duration.ToString("0")} SECONDS";
            yield return null;
        }

        PhotonNetwork.Disconnect();
    }
}
