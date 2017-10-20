using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningNodeScript : ResourceNodeParentScript
{

    // Use this for initialization
    void Start()
    {
        ZoneType = "miningnode";

        //I know this is weird, but I'm keeping with a standard,
        //we'll just have to live with an array that only has ONE element, lol
        LocalResources = new ResourceType[1];
        LocalResources[0] = new ResourceType();

        ResourceLimit = 10;
        ListOfSpawnpoints = new RNodeSpawnpoint[15];
        GetSpawnPoints();

        InvokeRepeating("UpdateNode", 0.1f, 2f);
    }

    // Update is called once per frame
    protected void Update()
    {
        CustomUpdate("rocks", 1);
    }

    void InitializeResources()
    {
        LocalResources[0].Name = "minerals";
        LocalResources[0].Type = "output";
    }

    void GetSpawnPoints()
    {
        //Initialize each element in the list
        PopulateListOfSpawnpoints();

        //Iterate through all of the child-objects
        foreach (Transform t in transform)
        {
            switch (t.gameObject.name)
            {
                case "Spawnpoint":
                    ListOfSpawnpoints[0].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (1)":
                    ListOfSpawnpoints[1].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (2)":
                    ListOfSpawnpoints[2].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (3)":
                    ListOfSpawnpoints[3].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (4)":
                    ListOfSpawnpoints[4].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (5)":
                    ListOfSpawnpoints[5].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (6)":
                    ListOfSpawnpoints[6].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (7)":
                    ListOfSpawnpoints[7].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (8)":
                    ListOfSpawnpoints[8].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (9)":
                    ListOfSpawnpoints[9].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (10)":
                    ListOfSpawnpoints[10].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (11)":
                    ListOfSpawnpoints[11].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (12)":
                    ListOfSpawnpoints[12].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (13)":
                    ListOfSpawnpoints[13].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (14)":
                    ListOfSpawnpoints[14].SpawnpointObject = t.gameObject;
                    break;
            }
        }
    }
    //Periodically called to update the Resource node
    void UpdateNode()
    {
        //Debug.Log(ReturnSpawned() + " trees have been spawned.");

        //If there's less resources than the upper limit
        if (ReturnSpawned() <= ResourceLimit)
        {
            GenerateResource("LowPolyRock1","LowPolyRock3", 0.5f);
        }
    }
}
