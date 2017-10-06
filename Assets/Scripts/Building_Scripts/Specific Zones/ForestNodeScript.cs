using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestNodeScript : ResourceNodeParentScript {
    /// <summary>
    /// Since this Smart Zone is a resource node, I do not believe it is necessary to assign a limit
    /// to how many NPCs can interact with it at once. However, I do believe that it is important to
    /// still keep an indication of how many resources are available, as well as allowing us for making
    /// reservations on the individual resources inside of the node.
    /// </summary>


    // Use this for initialization
    void Start()
    {
        ZoneType = "forestnode";

        //I know this is weird, but I'm keeping with a standard,
        //we'll just have to live with an array that only has ONE element, lol
        LocalResources = new ResourceType[1];
        LocalResources[0] = new ResourceType();

        ResourceLimit = 10;
        ListOfSpawnpoints = new RNodeSpawnpoint[15];
        GetSpawnPoints();

        //InvokeRepeating("SpamTreesEverywhere", 0.01f, 1f);

        InvokeRepeating("UpdateNode", 0.1f, 1f);
    }

    void InitializeResources()
    {
        LocalResources[0].Name = "forest";
        LocalResources[0].Type = "output";
    }




    //Initialize the elements of the Spawnpoint array
    void PopulateListOfSpawnpoints()
    {
        for(int i = 0; i < ListOfSpawnpoints.Length; i++)
        {
            ListOfSpawnpoints[i] = new RNodeSpawnpoint();
        }
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

    //FOR TESTING
    void SpamTreesEverywhere()
    {
        for(int i = 0; i < ListOfSpawnpoints.Length; i++)
        {
            ListOfSpawnpoints[i].LoadMesh("LowPolyTree2", 0.5f);
        }
        Invoke("DestroyAllTrees", 0.9f);
    }
    void DestroyAllTrees()
    {
        for (int i = 0; i < ListOfSpawnpoints.Length; i++)
        {
            ListOfSpawnpoints[i].DestroyMesh();
        }
    }

    //Runs through the list of spawnpoints and randomly picks one of the available spots to generate a resource on
    void GenerateResource()
    {
        //TODO: This might need some tweaking, it only every takes up the first 9-10 spawnpoints, but that's ok mechanically for now
        int ToSubtract = (int)(RandomDouble(ResourceLimit - ReturnSpawned()));
        //Debug.Log("ToSubtract at: " + ToSubtract);
        foreach(RNodeSpawnpoint Spawnpoint in ListOfSpawnpoints)
        {
            if(!Spawnpoint.HasSpawned)
            {
                ToSubtract--;
                if(ToSubtract == 0)
                {
                    Spawnpoint.LoadMesh("LowPolyTree2", 0.5f);
                    Spawnpoint.HasSpawned = true;
                    LocalResources[0].Amount++;

                    Debug.Log("Tree spawned,  total resources at: " + LocalResources[0].Amount);
                    break;
                }
            }
        }
    }

    //Takes the Wisp reference, reserves the number of
    public void MakeReservation(WispScript WispScriptRef)
    {
        Reservation NewReservation = new Reservation();
        
        //Write the Wisp into the reservation
        NewReservation.WispScriptRef = WispScriptRef;

        //Iterate through the Spawnpoints of the resource node
        for(int i = 0; i < ListOfSpawnpoints.Length; i++)
        {
            if(ReturnSpawned() >= 1 &&              //If there's one or more trees left
                ListOfSpawnpoints[i].HasSpawned &&  //If the currently checked node has a tree on it
                !ListOfSpawnpoints[i].IsReserved && //If the currently checked node isn't already reserved
                WispScriptRef.CurrentlyReserved < WispScriptRef.CarryCapacity)  //If the Wisp can still carry more resources
            {
                //Reserve the resource, add one to the currently reserved number in the Wisp
                ListOfSpawnpoints[i].IsReserved = true;
                WispScriptRef.CurrentlyReserved++;
                //Add the spawnpoint to the Reservation object
                NewReservation.MakeReservation(ListOfSpawnpoints[i]);
            }
        }
        //File the Reservation object to the Resource Node's list of reservations
        ReservationList.Add(NewReservation);
    }

    //Periodically called to update the Resource node
    void UpdateNode()
    {
        //Debug.Log(ReturnSpawned() + " trees have been spawned.");

        //If there's less resources than the upper limit
        if(ReturnSpawned() <= ResourceLimit)
        {
            GenerateResource();
        }
    }

}
