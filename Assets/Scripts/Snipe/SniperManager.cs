using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using YsoCorp.GameUtils;

public class SniperManager : MonoBehaviour
{
    public Transform hor, ver, playerSpine;

    public GameObject winPanel, losePanel, sniperScopeImage, crosshair, fireEffect, bloodEffect;

    public float sniperZoomSpeed, horizontalSpeed, verticalSpeed, sniperHorSpeed, sniperVerSpeed, threshold, sniperFireRate = 1, sniperImpactForce;
    public float rightBound, leftBound, upBound, downBound, maxDelta;

    public int defaultFOV = 60, sniperFOV = 30, timerTime;

    private int killedDancers;

    private Vector2 pos1, pos2;

    private float deltaX, deltaY, nextTimeToFire;
    private bool timeIsUp;

    private AudioSource gunSound;
    private Animator camAnim;

    public Text timerText;

    public LayerMask layerMask;

    RaycastHit hit;
    Camera mainCamera;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        gunSound = GetComponent<AudioSource>();
        mainCamera = Camera.main;

        camAnim = hor.GetComponent<Animator>();

        GameStartYSO();

        StartCoroutine(Timer());
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

    private void Update()
    {
        Sniper();
    }

    void Sniper()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.touchCount > 0)
            {
                pos1 = Input.GetTouch(0).position;
            }
            else
            {
                pos1 = Input.mousePosition;
            }

            if (AimButton.isShooting)
            {
                crosshair.SetActive(false);
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (Input.touchCount > 0)
            {
                pos2 = Input.GetTouch(0).position;
            }
            else
            {
                pos2 = Input.mousePosition;
            }

            deltaX = pos2.x - pos1.x;
            deltaY = pos2.y - pos1.y;

            CamDeltaLimiter();

            if (AimButton.isShooting)
            {
                if (Camera.main.fieldOfView > sniperFOV)
                {
                    Camera.main.fieldOfView -= sniperZoomSpeed;
                }
                else
                {
                    sniperScopeImage.SetActive(true);
                }

                if (Camera.main.fieldOfView < sniperFOV)
                {
                    Camera.main.fieldOfView = sniperFOV;
                }

                playerSpine.LookAt(hor.forward);
            }

            if (deltaX > threshold)
            {
                if (hor.rotation.y * Mathf.Rad2Deg <= rightBound)
                {
                    if (AimButton.isShooting)
                    {
                        hor.Rotate(0, deltaX * sniperVerSpeed * Time.smoothDeltaTime, 0);
                    }
                    else
                    {
                        hor.Rotate(0, deltaX * verticalSpeed * Time.smoothDeltaTime, 0);
                    }
                }
            }
            else if (deltaX < -threshold)
            {
                if (hor.rotation.y * Mathf.Rad2Deg >= -leftBound)
                {
                    if (AimButton.isShooting)
                    {
                        hor.Rotate(0, deltaX * sniperVerSpeed * Time.smoothDeltaTime, 0);
                    }
                    else
                    {
                        hor.Rotate(0, deltaX * verticalSpeed * Time.smoothDeltaTime, 0);
                    }
                }
            }
            if (deltaY > threshold)
            {
                if (ver.localRotation.x * Mathf.Rad2Deg >= -upBound)
                {
                    if (AimButton.isShooting)
                    {
                        ver.Rotate(-deltaY * sniperHorSpeed * Time.smoothDeltaTime, 0, 0);
                    }
                    else
                    {
                        ver.Rotate(-deltaY * horizontalSpeed * Time.smoothDeltaTime, 0, 0);
                    }
                }
            }
            else if (deltaY < -threshold)
            {
                if (ver.localRotation.x * Mathf.Rad2Deg <= downBound)
                {
                    if (AimButton.isShooting)
                    {
                        ver.Rotate(-deltaY * sniperHorSpeed * Time.smoothDeltaTime, 0, 0);
                    }
                    else
                    {
                        ver.Rotate(-deltaY * horizontalSpeed * Time.smoothDeltaTime, 0, 0);
                    }
                }
            }
            pos1 = pos2;
        }
        if (Input.GetMouseButtonUp(0) && AimButton.isShooting)
        {
            SniperFire();
        }
    }

    void SniperFire()
    {
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / sniperFireRate;
            
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 1000, layerMask))
            {
                if (hit.rigidbody != null)
                {
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);

                        if (hit.transform.GetComponentInParent<SniperModeEnemy>() != null)
                        {
                            SniperModeEnemy sme = hit.transform.GetComponentInParent<SniperModeEnemy>();

                            sme.DisableKinematics();

                            if (sme.isDancing)
                            {
                                killedDancers++;

                                if (killedDancers >= SniperEnemyManager.dancersNum)
                                {
                                    if (!timeIsUp)
                                    {
                                        Invoke("Win", 1);
                                    }
                                }
                            }
                            else
                            {
                                Invoke("Lose", 1);
                            }

                            hit.rigidbody.AddForce(-hit.normal * sniperImpactForce);
                        }
                    }
                }
                if (hit.transform.CompareTag("Concrete"))
                {
                    Instantiate(fireEffect, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }

            crosshair.SetActive(false);
            camAnim.Play("Cam Kick");
            gunSound.Play();
            StartCoroutine(SetFOV(1f));   // return the sniper camera back to its place
        }

        AimButton.isShooting = false;
    }

    IEnumerator SetFOV(float time)
    {
        yield return new WaitForSeconds(time);
        Camera.main.fieldOfView = defaultFOV;
        sniperScopeImage.SetActive(false);
        crosshair.SetActive(true);
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
        Lose();
    }

    void CamDeltaLimiter()
    {
        if (deltaX > maxDelta)
        {
            deltaX = maxDelta;
        }
        else if (deltaX < -maxDelta)
        {
            deltaX = -maxDelta;
        }
        if (deltaY > maxDelta)
        {
            deltaY = maxDelta;
        }
        else if (deltaY < -maxDelta)
        {
            deltaY = -maxDelta;
        }
    }

    void UpgradeValueAndSave(string valueKey, int defaultValue, int upgradeAmount)
    {
        int valueToUpgrade = PlayerPrefs.GetInt(valueKey, defaultValue);
        valueToUpgrade += upgradeAmount;
        PlayerPrefs.SetInt(valueKey, valueToUpgrade);
    }

    void Win()
    {
        UpgradeValueAndSave("currentLevel", 1, 1);
        UpgradeValueAndSave("coin", 0, 50);

        GameFinish(true);
        winPanel.SetActive(true);
        Time.timeScale = 0.0f;
    }
    void Lose()
    {
        GameFinish(false);
        losePanel.SetActive(true);
        Time.timeScale = 0.0f;
    }
}
