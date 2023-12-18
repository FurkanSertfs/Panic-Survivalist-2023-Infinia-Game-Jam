using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTheTarget : MonoBehaviour
{
    [SerializeField]
    Transform gunPoint1, gunPoint2, gunPoint;



    [SerializeField]
    FireManager fireManager;



    Vector3 startLocalScale;

    private void Start()
    {
        startLocalScale = transform.localScale;
    }
    private void Update()
    {

        transform.position = gunPoint.position;

        Vector3 enemyPoint = fireManager.ClosestEnemy(); // yakin enemyPoint

        Vector3 direction = enemyPoint - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);



        if (angle > -90 && angle < 90)
        {
            transform.localScale = startLocalScale;
            
        }
        else
        {
            transform.localScale = new Vector3(startLocalScale.x, -startLocalScale.y, startLocalScale.z);
            
        }



    }
}