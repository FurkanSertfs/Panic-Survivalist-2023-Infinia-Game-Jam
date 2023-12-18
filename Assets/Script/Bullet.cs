using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Enemy enemy;
    
    [SerializeField]
    private float bulletSpeed;
    private Vector3 direction = Vector3.right;
    public float damage;

    public PlayerController playerController;

   
    public void SetUp(Vector3 direction)
    {
        this.direction = direction;
        
    }

    private void Update()
    {
        transform.Translate(Vector3.right * bulletSpeed * Time.deltaTime);
    }

     private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.GetComponent<Enemy>() != null)
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(damage,playerController);

            Destroy(gameObject);

        }

        if(other.gameObject.GetComponent<PlayerController>() != null)
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
