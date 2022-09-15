using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TOWEnemy : MonoBehaviour
{
    public Rigidbody[] bodyParts;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Falling Point"))
        {
            TugOfWarScript manager = FindObjectOfType<TugOfWarScript>();

            if (gameObject.CompareTag("Player"))
            {
                manager.fallenPlayers++;
                manager.GameEndCheck();
            }
            else if (gameObject.CompareTag("Enemy"))
            {
                manager.fallenEnemies++;
                manager.GameEndCheck();
            }



            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Animator>().enabled = false;

            transform.parent = null;

            foreach (var item in bodyParts)
            {
                item.isKinematic = false;
            }
        }
    }
}
