using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperEnemyManager : MonoBehaviour
{
    public Transform[] enemyPlaces;
    public SniperModeEnemy[] enemies;

    //public List<SniperModeEnemy> dancingEnemies = new List<SniperModeEnemy>();

    public static int dancersNum;

    List<SniperModeEnemy> spawnedEnemies = new List<SniperModeEnemy>();

    private void Awake()
    {
        dancersNum = 0;

        for (int i = 0; i < enemyPlaces.Length; i++)
        {
            int random = Random.Range(1, 3);

            if (random == 1)
            {
                SniperModeEnemy obj = Instantiate(enemies[Random.Range(0, enemies.Length)], enemyPlaces[i].position, enemyPlaces[i].rotation);
                spawnedEnemies.Add(obj);
            }
        }

        int c = 0;

        foreach (var item in spawnedEnemies)
        {
            if (!item.isDancing)
            {
                c++;
            }
        }

        if (c == spawnedEnemies.Count)
        {
            spawnedEnemies[spawnedEnemies.Count - 1].MakeHimDance();
        }

        Invoke("CalculateDancingEnemies", 0.5f);
    }

    private void CalculateDancingEnemies()
    {
        foreach (var item in spawnedEnemies)
        {
            if (item.isDancing)
            {
                //dancingEnemies.Add(item);
                dancersNum++;
            }
        }
    }
}
