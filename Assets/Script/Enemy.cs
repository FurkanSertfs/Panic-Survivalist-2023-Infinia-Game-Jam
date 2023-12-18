using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    EnemyTypes enemyTypes;
    [SerializeField]
    Transform player;

    [SerializeField]
    float health;
    [SerializeField]
    float speed;

    [SerializeField]
    float damage;

    [SerializeField]
    float attackCoolDown;

    [SerializeField]
    bool hit;
    bool isAttacking;

    Vector3 direction;

    int money;

    Animator animator;

    float startHealth;


    bool isStopped;

    [SerializeField]
    TextMesh damageText;

    [SerializeField]
    Transform damageTextTransform, damageTextFinalPosition;

    [SerializeField]
    Experience experience;

    [SerializeField]
    Chest chest;


    private void SetUp()
    {

        int minute = Mathf.CeilToInt((Time.time - GameManager.instance.startTime) / 60);
        //Debug.Log(Time.time - startTime/60 );   
        health = enemyTypes.healthMultiplier * (Mathf.Pow(minute, 3.2f) + 5 + Mathf.Sin(minute));
        startHealth = health;
        speed = enemyTypes.speed;
        damage = enemyTypes.damage;
        attackCoolDown = enemyTypes.attackCoolDown;
        money = enemyTypes.money;
        player = GameManager.instance.GetPlayer();

       
        
    }

    void PauseOnEvent(bool isStarted)
    {
        if (isStarted)
        {
            isStopped = true;
            animator.speed = 0;
            speed = 0;
        }
        else
        {
            isStopped = false;
            speed = enemyTypes.speed;
            animator.speed = 1;
        }
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

    }

    private void Start()
    {

        SetUp();


    }

    private void OnEnable()
    {
        GameEvents.instance.OnEventAction += PauseOnEvent;
    }

    private void OnDestroy()
    {
      
        GameEvents.instance.OnEventAction -= PauseOnEvent;
    }

    public void TakeDamage(float _damage, PlayerController playerController)
    {
        float randomDamage = Random.Range(_damage * 0.8f, _damage * 1.2f);

        StartCoroutine(TextAnim(randomDamage));

        health -= randomDamage;

        if (health > 0)
        {
            animator.SetBool("isDamaged", true);
        }
        else
        {
            int randomNumber = Random.Range(0, 10);

            if (randomNumber < 8)
            {
                Experience experience = Instantiate(this.experience, transform.position, Quaternion.identity);

                experience.experienceValue = startHealth;


            }

            else
            {
                Chest chest = Instantiate(this.chest, transform.position, Quaternion.identity);

            }


            WaweSpawner.instance.enemyList.Remove(gameObject);

            Death();
        }

    }

    IEnumerator TextAnim(float damage = 0)
    {
        TextMesh textMesh = Instantiate(damageText, damageTextTransform.position, Quaternion.identity);

        textMesh.text = damage.ToString("0.0");

        float timer = 0;

        textMesh.transform.DOScale(0, 1.3f).OnComplete(() => Destroy(textMesh.gameObject));


        while (timer < 1.24)
        {
            timer += Time.deltaTime;

            textMesh.color = Color.Lerp(Color.white, Color.clear, timer);

            yield return new WaitForEndOfFrame();
        }



    }

    public void Death()
    {
        GameEvents.instance.OnEnemyDestroy?.Invoke(transform);
        animator.SetBool("isDeath", true);


    }

    public void EndHitEnemyAnimation()
    {
        animator.SetBool("isDamaged", false);

    }

    public void EndDeathAnimation()
    {
        Destroy(gameObject);
    }

    private void FollowPlayer()
    {
        if (player != null)
        {
            direction = (player.position - transform.position).normalized;

            transform.Translate(direction * speed * Time.deltaTime);




        }

    }

    private void Update()
    {
        FollowPlayer();
        ChangeDirection();

    }

    private void ChangeDirection()
    {
        Vector2 tempDirection = transform.localScale;

        if (direction.x > 0)
        {
            tempDirection.x = 1f;
        }
        else if (direction.x < 0)
        {
            tempDirection.x = -1f;
        }

        transform.localScale = tempDirection;

    }


    IEnumerator AttackingRoutine(PlayerController playerController)
    {
        isAttacking = true;
        while (hit && !isStopped)
        {


            playerController.TakeDamage(damage);
            yield return new WaitForSeconds(attackCoolDown);


        }
        isAttacking = false;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            hit = true;
            if (!isAttacking)
            {
                StartCoroutine(AttackingRoutine(other.gameObject.GetComponent<PlayerController>()));

            }

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            hit = false;
        }
    }
}
