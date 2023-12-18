using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;

    public delegate void OnEnemyEnteredDelegate(Transform enemy, bool x);

    public delegate void OnEnemyDestroyDelegate(Transform enemyTransform);
    
    public delegate void OnFinishGameDelegate();

    public delegate void OnFireDelegate();

    public delegate void OnEventActionDelegate(bool isStarted);

    public OnEventActionDelegate OnEventAction;
    public OnEnemyEnteredDelegate OnEnemyEntered;

    public OnFireDelegate OnFire;

    public OnEnemyDestroyDelegate OnEnemyDestroy;

    public OnFinishGameDelegate OnFinishGame;

    public delegate void OnSomeOneWinDelegate(string winnerName);

    public OnSomeOneWinDelegate OnSomeOneWin;

    
    private void Awake()
    {
        instance = this;
    }
}
