using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    TMP_Text whoWinsText;

    [SerializeField]
    GameObject gameOverPanel;

    private void OnEnable()
    {
        GameEvents.instance.OnSomeOneWin += OnGameEnd;
    }
    private void OnDisable()
    {
        GameEvents.instance.OnSomeOneWin -= OnGameEnd;
    }

    void OnGameEnd(string playerID)
    {
        gameOverPanel.SetActive(true);
        whoWinsText.text = playerID + " Won The Game!";
    }

}
