using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class AimButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static bool isShooting;

    public Animator playerAnim;

    public GameObject gun;

    public void OnPointerDown(PointerEventData eventData)
    {
        isShooting = true;

        gun.SetActive(false);

        playerAnim.SetBool("Shooting", true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StartCoroutine(PlayPlayerAnimationWithDelay(0.9f));

    }

    IEnumerator PlayPlayerAnimationWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gun.SetActive(true);

        playerAnim.SetBool("Shooting", false);
    }
}
