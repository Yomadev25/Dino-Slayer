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

    public void Init(string name, Color color)
    {
        _nameText.text = name;
        _profileImage.color = color;       
    }
}
