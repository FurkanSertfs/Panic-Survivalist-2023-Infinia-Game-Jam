using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.IO.Ports;
using System;
public class PlayerController : MonoBehaviour
{

    [SerializeField]
    AudioSource levelUpSound, experienceSound;

    FireManager fireManager;
    // public static PlayerController instance;

    [SerializeField]
    Image healthBar;

    [SerializeField]
    float speed;

    [SerializeField]
    float health, maxHealth;

    [SerializeField]
    float invincibilityTime;

    float invincibilityCounter;

    Animator animator;

    private Rigidbody2D rb;

    [SerializeField] bool isOtherController;

    SerialPort serialPort;

    bool isShieldActive;

    [SerializeField]
    GameObject shield;

    [SerializeField]
    Image buttonImage;

    [SerializeField]
    TMP_Text buttonText;

    string serialData;

    [SerializeField] TMP_Text eventTimerText;

    bool isStopped;

    float startSpeed;

    [SerializeField]
    Image eventTimeBar;

    [SerializeField]
    Sprite greenButton, redButton, orangeButton;

    [SerializeField] float xp, xpToNextLevel = 40;

    [SerializeField] TMP_Text levelUpText, levelText;

    int level = 1;

    int directionPenalty = 1;


    [SerializeField]
    PlayerController otherPlayer;

    [SerializeField]
    PunchScreen punchScreen;

    [SerializeField]
    Image blackScreen;

    Tween currentTwwen;

    private void OnEnable()
    {
        GameEvents.instance.OnEventAction += OnStopEvent;
    }
    private void OnDisable()
    {
        GameEvents.instance.OnEventAction -= OnStopEvent;
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        rb = GetComponent<Rigidbody2D>();

        fireManager = GetComponent<FireManager>();
    }

    private void Start()
    {
        startSpeed = speed;

        if (isOtherController)
        {
            Debug.Log("Is Other Controller" + isOtherController, gameObject);

            serialPort = new SerialPort("COM4", 9600); // Arduino'nun bağlı olduğu seri port ve baud rate


            serialPort.Open();
            serialPort.ReadTimeout = 5000;

            if (serialPort.IsOpen)
            {
                Debug.Log("Serial Port Açıldı", gameObject);
            }
            else
            {
                Debug.Log("Serial Port Açılamadı");
            }


        }
    }

    public void EarnXp(float xp)
    {
        this.xp += xp;
        if (this.xp >= xpToNextLevel)
        {
            LevelUp();
            xpToNextLevel *= 1.5f;
        }
    }

    void LevelUp()
    {
        levelUpSound.Play();

        level++;

        xp = 0;

        fireManager.damage += fireManager.damage * 0.1f;

        fireManager.reloudTime -= fireManager.reloudTime * 0.05f;

        StartCoroutine(LevelUpText());


    }

    IEnumerator LevelUpText()
    {
        levelUpText.color = Color.white;
        levelUpText.gameObject.SetActive(true);
        levelUpText.DOColor(Color.green, 0.125f).SetLoops(10, LoopType.Yoyo);
        yield return new WaitForSeconds(1.25f);
        levelText.text = "Lvl: " + level;
        levelUpText.gameObject.SetActive(false);


    }

    private void Update()
    {
        float moveHorizontal = 0;
        float moveVertical = 0;


        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(ButtonClickEvent());
        }


        if (!isOtherController)
        {
            // wasd 
            if (Input.GetKey(KeyCode.W))
            {
                moveVertical = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                moveVertical = -1;
            }
            else
            {
                moveVertical = 0;
            }

            if (Input.GetKey(KeyCode.A))
            {
                moveHorizontal = -1;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                moveHorizontal = 1;
            }
            else
            {
                moveHorizontal = 0;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isShieldActive)
                    StartCoroutine(ActivateShield());
            }



        }

        else
        {
            // arrow keys


            try
            {
                serialData = serialPort.ReadLine();





                if (serialData[0] == '0')
                {
                    moveVertical = 1;


                }

                if (serialData[1] == '0')
                {
                    moveVertical = -1;

                }

                if (serialData[2] == '0')
                {
                    moveHorizontal = 1;


                }

                if (serialData[3] == '0')
                {
                    moveHorizontal = -1;

                }
                if (serialData[4] == '0')
                {
                    GameEvents.instance.OnFire?.Invoke();

                }


                if (serialData[5] == '0')
                {


                    if (!isShieldActive)
                        StartCoroutine(ActivateShield());
                }

            }

            catch (System.Exception)
            {
                Debug.Log("Serial Port Hatası");
                // Hata yönetimi
            }



        }



        invincibilityCounter -= Time.deltaTime;
        ChangeDirection();



        Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized * directionPenalty;
        rb.velocity = movement * speed;

        if ((rb.velocity.magnitude) > 0)
        {
            animator.SetBool("isMove", true);
        }
        else
        {
            animator.SetBool("isMove", false);
        }


    }


    void OnStopEvent(bool isStarted)
    {
        if (isStarted)
        {
            speed = 0;
            animator.speed = 0;
            isStopped = true;

        }
        else
        {
            animator.speed = 1;
            isStopped = false;
            speed = startSpeed;

        }


    }

    IEnumerator ActivateShield()
    {
        if (isStopped)
        {
            yield break;
        }

        isShieldActive = true;
        shield.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < 3; i++)
        {
            shield.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            shield.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
        shield.SetActive(false);
        isShieldActive = false;

    }

    IEnumerator ButtonClickEvent(int Count = 1)
    {
        GameEvents.instance.OnEventAction?.Invoke(true);

        eventTimeBar.fillAmount = 1;
        buttonImage.sprite = orangeButton;

        if (Count == 1)
        {

            int eventTimer = 2;
            eventTimeBar.transform.parent.gameObject.SetActive(true);

            DOTween.To(() => eventTimeBar.fillAmount, x => eventTimeBar.fillAmount = x, 0, 3);


            while (eventTimer > -1)
            {

                eventTimerText.text = "You must click the button on the screen in " + eventTimer + " seconds";
                yield return new WaitForSeconds(1);
                eventTimer--;
            }

            eventTimeBar.transform.parent.gameObject.SetActive(false);

        }



        else
        {
            eventTimeBar.transform.parent.gameObject.SetActive(true);

            currentTwwen.Kill();

            yield return new WaitForSeconds(0.125f);

            currentTwwen = DOTween.To(() => eventTimeBar.fillAmount, x => eventTimeBar.fillAmount = x, 0, 2f).SetEase(Ease.Linear);

        }

        buttonImage.color = Color.white;

        buttonImage.gameObject.SetActive(true);
        String[] buttonTexts = { "Top", "Bottom", "Right", "Left", "Shoot", "Shield" };
        int randomIndex = UnityEngine.Random.Range(0, buttonTexts.Length);
        buttonText.text = buttonTexts[randomIndex];
        bool isButtonClicked = false;
        float timer = 0;

        while (timer < 2)
        {
            timer += Time.deltaTime;
            yield return null;

            if (isOtherController)
            {
                try
                {


                    if (serialData[randomIndex] == '0')
                    {
                        isButtonClicked = true;
                        buttonImage.sprite = greenButton;
                        break;

                    }
                    bool isBreak = false;

                    for (int i = 0; i < 6; i++)
                    {
                        if (serialData[i] == '0')
                        {
                            buttonImage.sprite = redButton;
                            isBreak = true;
                            break;
                        }

                    }

                    if (isBreak)
                    {
                        break;
                    }

                }

                catch (System.Exception)
                {
                    Debug.Log("Serial Port Hatası");
                    // Hata yönetimi
                }
            }

            else
            {
                if (Input.GetKeyDown(KeyCode.W) && randomIndex == 0)
                {
                    isButtonClicked = true;
                    buttonImage.sprite = greenButton;
                    break;
                }
                else if (Input.GetKeyDown(KeyCode.S) && randomIndex == 1)
                {
                    isButtonClicked = true;
                    buttonImage.sprite = greenButton;
                    break;
                }
                else if (Input.GetKeyDown(KeyCode.D) && randomIndex == 2)
                {
                    isButtonClicked = true;
                    buttonImage.sprite = greenButton;
                    break;
                }
                else if (Input.GetKeyDown(KeyCode.A) && randomIndex == 3)
                {
                    isButtonClicked = true;
                    buttonImage.sprite = greenButton;
                    break;
                }
                else if (Input.GetKeyDown(KeyCode.Space) && randomIndex == 4)
                {
                    isButtonClicked = true;
                    buttonImage.sprite = greenButton;
                    break;
                }
                else if (Input.GetKeyDown(KeyCode.E) && randomIndex == 5)
                {
                    isButtonClicked = true;
                    buttonImage.sprite = greenButton;
                    break;
                }
                else if (Input.anyKeyDown)
                {
                    buttonImage.sprite = redButton;
                    isButtonClicked = false;
                    break;
                }

            }


        }

        if (isButtonClicked && Count > 5)
        {
            eventTimeBar.fillAmount = 0;
            eventTimeBar.transform.parent.gameObject.SetActive(true);
            eventTimerText.text = "You won the event";
            buttonImage.gameObject.SetActive(false);


            yield return new WaitForSeconds(1.5f);

            eventTimeBar.transform.parent.gameObject.SetActive(false);

            int randomNumber = UnityEngine.Random.Range(0, 5);

            if (randomNumber == 0)
            {
                otherPlayer.StartCoroutine(otherPlayer.DirectionPenaltyRoutine());
            }

            else if (randomNumber == 1)
            {
                otherPlayer.StartCoroutine(otherPlayer.ShaceScreenRoutine());

            }

            else if (randomNumber == 2)
            {
                otherPlayer.StartCoroutine(otherPlayer.BlackScreenRoutine());
            }

            else if (randomNumber == 3)
            {
                eventTimerText.text = "Your health is restored";

                health = maxHealth;
            }

            GameEvents.instance.OnEventAction?.Invoke(false);

            yield return new WaitForSeconds(1.5f);

            eventTimeBar.transform.parent.gameObject.SetActive(false);


        }

        else if (isButtonClicked && Count <= 5)
        {
            yield return new WaitForSeconds(0.25f);
            buttonImage.gameObject.SetActive(false);
            eventTimeBar.transform.parent.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);

            StartCoroutine(ButtonClickEvent(Count + 1));
        }

        else
        {
            eventTimeBar.transform.parent.gameObject.SetActive(true);
            eventTimeBar.fillAmount = 0; ;

            eventTimerText.text = "You lost the event";
            buttonImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(1.5f);
            eventTimeBar.transform.parent.gameObject.SetActive(false);
            GameEvents.instance.OnEventAction?.Invoke(false);

        }




    }



    public IEnumerator ShaceScreenRoutine()
    {
        eventTimeBar.transform.parent.gameObject.SetActive(true);
        eventTimeBar.fillAmount = 1;
        eventTimerText.text = "Your screen is shaking";
        punchScreen.Punch();
        yield return new WaitForSeconds(3);
        eventTimeBar.transform.parent.gameObject.SetActive(false);


    }


    public IEnumerator BlackScreenRoutine()
    {
        eventTimeBar.transform.parent.gameObject.SetActive(true);
        eventTimeBar.fillAmount = 1;
        eventTimerText.text = "Blindness";
        blackScreen.gameObject.SetActive(true);
        blackScreen.DOFade(0, 1).SetLoops(7, LoopType.Yoyo);
        yield return new WaitForSeconds(7);
        eventTimeBar.transform.parent.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(false);



    }

    public IEnumerator DirectionPenaltyRoutine()
    {
        eventTimeBar.transform.parent.gameObject.SetActive(true);
        eventTimeBar.fillAmount = 1;
        eventTimerText.text = "Opposite Buttons";
        directionPenalty = -1;
        yield return new WaitForSeconds(6);
        directionPenalty = 1;
        eventTimeBar.transform.parent.gameObject.SetActive(false);


    }



    public void TakeDamage(float damage)
    {
        if (invincibilityCounter <= 0)
        {

            health -= damage;
            DOTween.To(() => healthBar.fillAmount, x => healthBar.fillAmount = x, health / maxHealth, 0.25f);
            if (health > 0)
            {
                invincibilityCounter = invincibilityTime;
                animator.SetBool("isDamage", true);
            }
            else
            {
                if (isOtherController)
                {
                    GameEvents.instance.OnSomeOneWin?.Invoke("Player 1");
                }
                else
                {
                    GameEvents.instance.OnSomeOneWin?.Invoke("Player 2");
                }

                GameEvents.instance.OnFinishGame.Invoke();

            }
        }
    }



    public void EndHitPAnimation()
    {
        animator.SetBool("isDamage", false);
    }


    private void ChangeDirection()
    {
        Vector2 tempDirection = transform.localScale;



        if (rb.velocity.x > 0)
        {
            tempDirection.x = 1f;
        }
        else if (rb.velocity.x < 0)
        {
            tempDirection.x = -1f;
        }

        transform.localScale = tempDirection;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Experience")
        {

            Experience exp = other.GetComponent<Experience>();

            EarnXp(exp.experienceValue);

            other.gameObject.tag = "Untagged";


            other.transform.DOMove(other.transform.position + Vector3.up * 5, 0.35f).OnComplete(() =>
            {
                StartCoroutine(FollowPlayerRoutine(other.gameObject));
            });
        }

        if (other.tag == "Chest")
        {
            Chest chest = other.GetComponent<Chest>();

            chest.OpenChest();

            other.gameObject.tag = "Untagged";

            other.transform.DOMove(other.transform.position + Vector3.up * 5, 0.35f).OnComplete(() =>
            {
                StartCoroutine(FollowPlayerRoutine(other.gameObject));

                int randomNumber = UnityEngine.Random.Range(0, 5);

                if (randomNumber == 0)
                {
                    StartCoroutine(ButtonClickEvent());
                }
                else
                {
                    LevelUp();
                }


            });
        }
    }

    IEnumerator FollowPlayerRoutine(GameObject gameO)
    {
        while (Vector3.Distance(gameO.transform.position, transform.position) > 0.1f)
        {
            gameO.transform.position = Vector3.MoveTowards(gameO.transform.position, transform.position, 0.5f);
            yield return null;
        }

        experienceSound.Play();

        Destroy(gameO);
    }

    private void OnApplicationQuit()
    {
        if (isOtherController)
        {
            serialPort.Close();
        }
    }
}