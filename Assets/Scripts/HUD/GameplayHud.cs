using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayHud : MonoBehaviour
{
    [SerializeField]
    private PlayerItemHud[] _playerPanels;

    private void Awake()
    {
        GameManager.onUpdateCharacters += UpdatePlayerPanels;
    }

    private void OnDestroy()
    {
        GameManager.onUpdateCharacters -= UpdatePlayerPanels;
    }

    private void UpdatePlayerPanels(List<Character> characters)
    {
        foreach (PlayerItemHud item in _playerPanels)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < characters.Count; i++)
        {
            _playerPanels[i].Init(characters[i].id, characters[i].color);
            _playerPanels[i].gameObject.SetActive(true);
        }
    }
}
