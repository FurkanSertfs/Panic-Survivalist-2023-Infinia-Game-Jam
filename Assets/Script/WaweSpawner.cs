using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaweSpawner : MonoBehaviour
{
    public static WaweSpawner instance;

    [SerializeField]
    public Transform[] spawnPoints;

    [SerializeField]
    List<GameObject> enemyPrefabs = new List<GameObject>();
    float spawnInterval;

    [SerializeField] bool isStopped;

    public List<GameObject> enemyList = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnWave());

    }

    private void OnEnable()
    {
        GameEvents.instance.OnEventAction += OnEventStarted;
    }

    private void OnDisable()
    {
        GameEvents.instance.OnEventAction -= OnEventStarted;
    }

    void OnEventStarted(bool isStarted)
    {
        if (isStarted)
        {
            isStopped = true;
        }
        else
        {
            isStopped = false;
        }

    }

    IEnumerator SpawnWave()
    {

        while (true)
        {
            // wait until isStopped is false or enemyList is < 50

            yield return new WaitUntil(() => !isStopped || enemyList.Count < 50);

            int randomSpawnPoint = Random.Range(0, spawnPoints.Length);

            int minute = Mathf.CeilToInt((Time.time + 0.1f - GameManager.instance.startTime) / 60);

            spawnInterval = 2 / Mathf.Pow(minute, 4.7f) / 2 + 2 + (Mathf.Sin(minute) / 2);

            spawnInterval = Mathf.Clamp(spawnInterval, 0.5f, 2);

            yield return new WaitForSeconds(spawnInterval);

            int randomEnemyType = Random.Range(0, enemyPrefabs.Count);


            yield return new WaitUntil(() => !isStopped || enemyList.Count < 50);

            enemyList.Add(Instantiate(enemyPrefabs[randomEnemyType], spawnPoints[randomSpawnPoint].position, Quaternion.identity));






        }


    }






}


