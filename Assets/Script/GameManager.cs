using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    Transform[] players;

    int playerIndex;

    public float startTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        startTime = Time.time;
    }

    public Transform GetPlayer()
    {
        playerIndex++;

        if (playerIndex >= players.Length)
        {
            playerIndex = 0;
        }

        return players[playerIndex];

    }





    private void OnEnable()
    {

        GameEvents.instance.OnFinishGame += FinishGame;

    }

    private void OnDisable()
    {

        GameEvents.instance.OnFinishGame += FinishGame;
    }





    void FinishGame()
    {

        Time.timeScale = 0;
    }









}
