using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemHud : MonoBehaviour
{
    [SerializeField]
    private Image _profileImage;
    [SerializeField]
    private TMP_Text _nameText;
    [SerializeField]
    private TMP_Text _scoreText;
    [SerializeField]
    private GameObject _readyIcon;

    public void Init(string name, Color color, bool ready)
    {
        _nameText.text = name;
        _profileImage.color = color;       
        _readyIcon.SetActive(ready);
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.text = score.ToString();
    }
}
