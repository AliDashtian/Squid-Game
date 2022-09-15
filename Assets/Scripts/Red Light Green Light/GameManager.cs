using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using YsoCorp.GameUtils;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public GameObject doll;
    public ParticleSystem bloodPS;
    public Text timerText, startCounterText;
    public GameObject redLight, winPanel, losePanel, gamePanel;
    
    public int timerTime, otherPlayersKillRate;

    public float forwardSpeed/*, maxSpeed = 2*/, sideSpeed, maxDelta;

    public List<OtherPlayersScript> ops = new List<OtherPlayersScript>();
    public ParticleSystem[] wallRifle;

    public AudioClip[] dollSoundClips;

    public static bool isPlayerMoving, dollIsLooking;

    public static bool isFinished, won, timeIsUp;

    private List<OtherPlayersScript> killedPlayers = new List<OtherPlayersScript>();

    private Animator playerAnim, dollAnim;
    private AudioSource dollAS;
    private Image redLightImage;

    private Vector2 pos1, pos2;
    private float deltaX;

    private int startCounterTime = 3;

    Rigidbody playerRB;
    AudioSource gunSound;

    Vector3 oldPos;

    float t = 0, maxValue = 0;

    private void Awake()
    {
        Time.timeScale = 0.0f;
    }

    private void Start()
    {
        ops.AddRange(FindObjectsOfType<OtherPlayersScript>());

        GameStartYSO();
        SetStaticVariables();

        redLightImage = redLight.GetComponent<Image>();
        playerAnim = player.GetComponent<Animator>();
        dollAS = doll.GetComponent<AudioSource>();
        dollAnim = doll.GetComponent<Animator>();

        playerRB = player.GetComponent<Rigidbody>();
        gunSound = GetComponent<AudioSource>();

        bloodPS = player.GetChild(0).GetComponent<ParticleSystem>();

        StartCoroutine(StartCounter());

        for (int i = 0; i < ops.Count; i++)
        {
            ops[i].index = i;
        }
    }

    void GameStartYSO()
    {
        int currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
        //YCManager.instance.OnGameStarted(currentLevel);
    }
    public void GameFinish(bool win)
    {
        //YCManager.instance.OnGameFinished(win);
    }

    void SetStaticVariables()
    {
        isPlayerMoving = false;
        dollIsLooking = false;
        isFinished = false;
        won = false;
        timeIsUp = false;
    }

    IEnumerator StartCounter()
    {
        for (int i = 0; i < 3; i++)
        {
            startCounterText.text = startCounterTime.ToString();

            yield return new WaitForSecondsRealtime(1f);

            startCounterTime--;
        }

        startCounterText.gameObject.SetActive(false);

        Time.timeScale = 1f;

        StartCoroutine(RedLightGreenLight());
        StartCoroutine(Timer());
    }
    
    private void Update()
    {
        CheckIfPlayerIsMoving();

        if (!isFinished)
        {
            MoveTouch();
            MoveButtons();
        }

        if (won)
        {
            playerAnim.Play("Victory Idle");
            Invoke("ActiveWinPanel", 2f);
        }

        if (!isFinished)
        {
            if (dollIsLooking)
            {
                if (isPlayerMoving)
                {
                    DieAndLose();
                    Invoke("ActiveLosePanel", 2f);
                }
            }

            if (timeIsUp)
            {
                DieAndLose();
            }
        }

        if (!dollIsLooking)
        {
            RedLightSliderFill();
        }
    }

    void DieAndLose()
    {
        gunSound.Play();
        playerAnim.Play("Flying Back Death");
        bloodPS.Play();
        StopCoroutine(RedLightGreenLight());
        player.GetChild(5).gameObject.SetActive(false);
        isFinished = true;
    }

    void RedLightSliderFill()
    {
        t += Time.deltaTime;

        redLightImage.fillAmount = Mathf.Lerp(0, 1, t / maxValue);
    }

    IEnumerator RedLightGreenLight()
    {
        while (!isFinished)
        {
            dollAS.clip = dollSoundClips[Random.Range(0, dollSoundClips.Length)];
            maxValue = dollAS.clip.length;
            dollAS.Play();

            yield return new WaitForSecondsRealtime(dollAS.clip.length);
            dollIsLooking = true;

            int howManyAreAlive = HowManyAreAlive();
            ChooseKillRate();

            if (howManyAreAlive >= otherPlayersKillRate)
            {
                SelectPlayersForKilling(otherPlayersKillRate);
            }
            else
            {
                SelectPlayersForKilling(howManyAreAlive);
            }

            dollAnim.SetBool("Rotate", true);
            yield return new WaitForSeconds(1.5f);


            StartCoroutine(KillSelectedPlayers());

            float redLightTime = Random.Range(2f, 3f);
            t = 0;
            yield return new WaitForSeconds(redLightTime);
            dollIsLooking = false;

            dollAnim.SetBool("Rotate", false);
        }
    }

    private void MoveTouch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //isPlayerMoving = true;

            pos1 = Input.mousePosition;
            playerAnim.SetBool("Run", true);
        }
        else if (Input.GetMouseButton(0))
        {
            pos2 = Input.mousePosition;
            deltaX = pos2.x - pos1.x;
            DeltaLimiter();

            player.Translate(deltaX * sideSpeed * Time.deltaTime, 0, forwardSpeed * Time.deltaTime);
            //if (playerRB.velocity.magnitude < maxSpeed)
            //{
            //    playerRB.AddForce(deltaX * sideSpeed * Time.deltaTime, 0, forwardSpeed * Time.deltaTime);
            //}

            pos1 = pos2;
        }
        if (Input.GetMouseButtonUp(0))
        {
            //isPlayerMoving = false;

            playerAnim.SetBool("Run", false);
        }
    }

    private void MoveButtons()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            playerAnim.SetBool("Run", true);
            //if (playerRB.velocity.magnitude < maxSpeed)
            //    playerRB.AddForce(0, 0, forwardSpeed * Time.deltaTime);

            player.Translate(0, 0, forwardSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerAnim.SetBool("Run", true);
            //if (playerRB.velocity.magnitude < maxSpeed)
            //playerRB.AddForce(-1 * sideSpeed * Time.deltaTime, 0, forwardSpeed * Time.deltaTime);

            player.Translate(-sideSpeed * Time.deltaTime, 0, forwardSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            playerAnim.SetBool("Run", true);
            //if (playerRB.velocity.magnitude < maxSpeed)
            //playerRB.AddForce(1 * sideSpeed * Time.deltaTime, 0, forwardSpeed * Time.deltaTime);

            player.Translate(sideSpeed * Time.deltaTime, 0, forwardSpeed * Time.deltaTime);
        }
        if (!Input.anyKey)
        {
            playerAnim.SetBool("Run", false);
        }
    }

    void CheckIfPlayerIsMoving()
    {
        //if (Mathf.Abs(playerRB.velocity.magnitude) > 1.3f)
        //{
        //    isPlayerMoving = true;
        //}
        //else
        //{
        //    isPlayerMoving = false;
        //}

        if (!player.position.Equals(oldPos))
        {
            isPlayerMoving = true;
        }
        else
        {
            isPlayerMoving = false;
        }

        oldPos = player.position;
    }

    void DeltaLimiter()
    {
        if (deltaX > maxDelta)
        {
            deltaX = maxDelta;
        }
        else if (deltaX < -maxDelta)
        {
            deltaX = -maxDelta;
        }
    }

    void KillRandomPlayers(int killCount)
    {
        for (int i = 0; i < killCount; i++)
        {
            int randomAvailablePlayer = PickRandomPlayer();

            ops[randomAvailablePlayer].Die();
            RemoveCertainPlayer(randomAvailablePlayer);

            WallRifleFire();
        }
    }

    void SelectPlayersForKilling(int killCount)
    {
        killedPlayers.Clear();

        for (int i = 0; i < killCount; i++)
        {
            List<int> availablePlayersIndex = new List<int>();

            for (int j = 0; j < ops.Count; j++)
            {
                if (ops[j] != null)
                {
                    availablePlayersIndex.Add(ops[j].index);
                }
            }

            int rand = availablePlayersIndex[Random.Range(0, availablePlayersIndex.Count)];
            ops[rand].Stumble();
            killedPlayers.Add(ops[rand]);
            RemoveCertainPlayer(rand);
        }
    }

    IEnumerator KillSelectedPlayers()
    {
        for (int i = 0; i < killedPlayers.Count; i++)
        {
            killedPlayers[i].Die();
            WallRifleFire();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void WallRifleFire()
    {
        int wallRifleIndex = Random.Range(0, 3);
        wallRifle[wallRifleIndex].Play();

        gunSound.Play();
    }

    public void RemoveCertainPlayer(int index)
    {
        ops[index] = null;
    }

    int PickRandomPlayer()
    {
        List<int> availablePlayersIndex = new List<int>();

        for (int i = 0; i < ops.Count; i++)
        {
            if (ops[i] != null)
            {
                availablePlayersIndex.Add(ops[i].index);
            }
        }

        int rand = availablePlayersIndex[Random.Range(0, availablePlayersIndex.Count)];

        return rand;
    }

    int HowManyAreAlive()
    {
        int c = 0;

        for (int i = 0; i < ops.Count; i++)
        {
            if (ops[i] != null)
            {
                c++;
            }
        }

        return c;
    }

    void ChooseKillRate()
    {
        int killRate = Random.Range(0, 4);
        otherPlayersKillRate = killRate;
    }

    IEnumerator Timer()
    {
        while (timerTime >= 0)
        {
            var timeSpan = System.TimeSpan.FromSeconds(timerTime);
            timerText.text = timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");

            yield return new WaitForSeconds(1f);

            timerTime--;
        }

        timeIsUp = true;
        //StopCoroutine(RedLightGreenLight());
        StartCoroutine(KillTheRemaining());
    }

    IEnumerator KillTheRemaining()
    {
        for (int i = 0; i < ops.Count; i++)
        {
            if (ops[i] != null)
            {
                ops[i].Die();
                RemoveCertainPlayer(i);
                WallRifleFire();

                yield return new WaitForSeconds(0.5f);
            }
        }

        Invoke("ActiveLosePanel", 2f);
    }

    void ActiveWinPanel()
    {
        Invoke("SetTimeScaleToZero", 1);

        GameFinish(true);

        dollAS.Stop();
        gamePanel.SetActive(false);
        winPanel.SetActive(true);
    }

    void ActiveLosePanel()
    {
        Invoke("SetTimeScaleToZero", 3);

        GameFinish(false);

        dollAS.Stop();
        gamePanel.SetActive(false);
        losePanel.SetActive(true);
    }

    void SetTimeScaleToZero()
    {
        Time.timeScale = 0.0f;
    }
}
