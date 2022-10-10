using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SpatialPartitionPattern
{
    public class GameController : MonoBehaviour
    {
        public GameObject friendlyObj;
        public GameObject enemyObj;

        //Change materials to detect which enemy is the closest
        public Material enemyMaterial;
        public Material closestEnemyMaterial;

        //To get a cleaner workspace, parent all soldiers to these empty gameobjects
        public Transform enemyParent;
        public Transform friendlyParent;
        public bool spatialPartition = false;
        public int Direction = 1;
        public int a = 1;
        //Store all soldiers in these lists
        List<Soldier> enemySoldiers = new List<Soldier>();
        List<Soldier> friendlySoldiers = new List<Soldier>();

        //Save the closest enemies to easier change back its material
        List<Soldier> closestEnemies = new List<Soldier>();

        //Grid data
        float mapWidth = 50f;
        int cellSize = 10;

        //Number of soldiers on each team
        public int numberOfSoldiers = 200;
        public Text timeText;
        public Text onOffText;
        Toggle sp_toggle;

        //The Spatial Partition grid
        Grid grid;
        public void SPToggleChange(Toggle change)
        {
            if (change.isOn)
                spatialPartition = true;
            else
                spatialPartition = false;
        }

        public void ToggleChange(Toggle change)
        {
            if (change.isOn)
                a = -1;
            else
                a = 1;
        }

        public void ButtonAPressed()
        {
            numberOfSoldiers += 100;
            UpdateCount();
        }

        public void ButtonBPressed()
        {
            if (numberOfSoldiers > 100)
            {
                numberOfSoldiers -= 100;
            }
            UpdateCount();
        }

        public void UpdateCount()
        {
            GameObject[] soldier = GameObject.FindGameObjectsWithTag("Soldier");
            foreach (GameObject child in soldier)
                Destroy(child);
            // Destroy(friendlyObj);

            enemySoldiers.Clear();
            friendlySoldiers.Clear();
            Start();
        }

        void Start()
        {
            //Create a new grid
            grid = new Grid((int)mapWidth, cellSize);

            //Add random enemies and friendly and store them in a list
            for (int i = 0; i < numberOfSoldiers; i++)
            {
                //Give the enemy a random position
                Vector3 randomPos = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth));

                //Create a new enemy
                GameObject newEnemy = Instantiate(enemyObj, randomPos, Quaternion.identity) as GameObject;

                //Add the enemy to a list
                enemySoldiers.Add(new Enemy(newEnemy, mapWidth, grid));

                //Parent it
                newEnemy.transform.parent = enemyParent;


                //Give the friendly a random position
                randomPos = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth));

                //Create a new friendly
                GameObject newFriendly = Instantiate(friendlyObj, randomPos, Quaternion.identity) as GameObject;

                //Add the friendly to a list
                friendlySoldiers.Add(new Friendly(newFriendly, mapWidth));

                //Parent it 
                newFriendly.transform.parent = friendlyParent;
            }
        }


        void Update()
        {
            float startTime = Time.realtimeSinceStartup;

            if (spatialPartition)
            {
                for (int i = 0; i < enemySoldiers.Count; i++)
                {
                    enemySoldiers[i].Move();
                }

                for (int i = 0; i < closestEnemies.Count; i++)
                {
                    closestEnemies[i].soldierMeshRenderer.material = enemyMaterial;
                }

                closestEnemies.Clear();

                for (int i = 0; i < friendlySoldiers.Count; i++)
                {
                    //Soldier closestEnemy = FindClosestEnemySlow(friendlySoldiers[i]);

                    Soldier closestEnemy = grid.FindClosestEnemy(friendlySoldiers[i]);

                    if (closestEnemy != null)
                    {
                        closestEnemy.soldierMeshRenderer.material = closestEnemyMaterial;

                        closestEnemies.Add(closestEnemy);

                        friendlySoldiers[i].Move(closestEnemy,a);
                    }
                }
            }
            else {
                for (int i = 0; i < enemySoldiers.Count; i++)
                {
                    enemySoldiers[i].Move();
                }

                for (int i = 0; i < closestEnemies.Count; i++)
                {
                    closestEnemies[i].soldierMeshRenderer.material = enemyMaterial;
                }

                closestEnemies.Clear();

                for (int i = 0; i < friendlySoldiers.Count; i++)
                {
                    Soldier closestEnemy = FindClosestEnemySlow(friendlySoldiers[i]);

                    //Soldier closestEnemy = grid.FindClosestEnemy(friendlySoldiers[i]);

                    if (closestEnemy != null)
                    {
                        closestEnemy.soldierMeshRenderer.material = closestEnemyMaterial;

                        closestEnemies.Add(closestEnemy);

                        friendlySoldiers[i].Move(closestEnemy,a);
                    }
                }
            }
            float elapsedTime = 1f/(Time.realtimeSinceStartup - startTime);
            timeText.text = "FPS: " + elapsedTime ;
        }


        //Find the closest enemy - slow version
        Soldier FindClosestEnemySlow(Soldier soldier)
        {
            Soldier closestEnemy = null;

            float bestDistSqr = Mathf.Infinity;

            //Loop thorugh all enemies
            for (int i = 0; i < enemySoldiers.Count; i++)
            {
                //The distance sqr between the soldier and this enemy
                float distSqr = (soldier.soldierTrans.position - enemySoldiers[i].soldierTrans.position).sqrMagnitude;

                //If this distance is better than the previous best distance, then we have found an enemy that's closer
                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;

                    closestEnemy = enemySoldiers[i];
                }
            }
            return closestEnemy;
        }
    }
}