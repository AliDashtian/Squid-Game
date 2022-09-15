using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperModeEnemy : MonoBehaviour
{
    public bool isDancing;
    public Rigidbody[] bodyParts;

    private void Start()
    {
        int random = Random.Range(1, 3);

        if (random == 2)
        {
            isDancing = true;
            GetComponent<Animator>().SetBool("Idle", true);
        }
    }

    public void DisableKinematics()
    {
        GetComponent<Animator>().enabled = false;
        foreach (var item in bodyParts)
        {
            item.isKinematic = false;
        }
    }

    public void MakeHimDance()
    {
        isDancing = true;
        GetComponent<Animator>().SetBool("Idle", true);
    }
}
