using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayersScript : MonoBehaviour
{
    public float moveSpeed, maxSpeed = 2f;
    public bool isAlive = true, didFinish = false;

    private Animator anim;
    private ParticleSystem bloodPS;

    public int index;

    private void Start()
    {
        anim = GetComponent<Animator>();
        bloodPS = transform.GetChild(0).GetComponent<ParticleSystem>();

        StartCoroutine(SneakWalk());
    }

    IEnumerator SneakWalk()
    {
        while (isAlive && !didFinish)
        {
            if (!GameManager.dollIsLooking)
            {
                anim.SetBool("Run", true);

                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            else
            {
                anim.SetBool("Run", false);
            }

            yield return null;
        }
    }

    public void Die()
    {
        isAlive = false;
        StopCoroutine(SneakWalk());
        bloodPS.Play();
        anim.SetBool("Die", true);
    }

    public void Stumble()
    {
        isAlive = false;

        anim.SetBool("Stumble", true);
    }

    public void Win()
    {
        didFinish = true;
        anim.Play("Victory Idle");
        StopCoroutine(SneakWalk());
        FindObjectOfType<GameManager>().RemoveCertainPlayer(index);
    }
}
