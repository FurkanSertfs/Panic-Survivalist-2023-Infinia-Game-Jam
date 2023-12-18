using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFinder : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Enemy>() != null)
        {
            GameEvents.instance.OnEnemyEntered?.Invoke(other.transform, true);
        }

        // try get component other
        if (other.TryGetComponent(out PlayerController _playerController))
        {

            GameEvents.instance.OnEnemyEntered?.Invoke(other.transform, true);

        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.GetComponent<EnemyFinder>() != null)
        {
            GameEvents.instance.OnEnemyEntered?.Invoke(other.transform, false);
        }

        // try get component other
        if (other.TryGetComponent(out PlayerController _playerController))
        {

            GameEvents.instance.OnEnemyEntered?.Invoke(other.transform, false);

        }

    }




}
