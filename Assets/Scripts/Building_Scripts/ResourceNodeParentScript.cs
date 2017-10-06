using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNodeParentScript : SmartZoneParentScript {
    
    //Holds a reference to all of our reservation-objects
    protected List<Reservation> ReservationList = new List<Reservation>();

    protected RNodeSpawnpoint[] ListOfSpawnpoints;
    protected int ResourceLimit;

    //Class representing the Spawnpoints and the trees
    public class RNodeSpawnpoint
    {
        public bool HasSpawned = false;     //Whether there's a resource or not
        public bool IsReserved = false;     //Used to determine whether a Wisp has been sent for it or not
        public Vector3 SpawnpointLocation;  //TODO: OBSOLETE AT THE MOMENT, REMOVE LATER IF STILL NOT RELEVANT
        public GameObject SpawnpointObject; //A reference to the spawnpoint object itself
        public GameObject ResourceModel;    //A reference to the resource prefab we're instantiating

        //Functions for loading and destroying meshes on the Resource Node Spawnpoint
        public void LoadMesh(string ResourceDirectory, float MinimumScale)
        {
            //Instantiate prefab from Resources-folder
            var temp = Instantiate(Resources.Load(ResourceDirectory) as GameObject);
            ResourceModel = temp;  //Store reference

            Vector3 RandomRotation = new Vector3(0f, (float)RandomDouble(360f), 0f);

            HasSpawned = true;

            //Set parent and configure transform for the instantiated prefab
            temp.transform.parent = SpawnpointObject.transform;
            temp.transform.position = SpawnpointObject.transform.position;
            temp.transform.rotation = SpawnpointObject.transform.rotation;    //Match rotation on all three axis with spawnpoint, necessary on incline surfaces
            temp.transform.Rotate(RandomRotation);
            temp.transform.localScale = RandomScale(MinimumScale);


        }
        public void DestroyMesh()
        {
            Destroy(ResourceModel);
        }

        Vector3 RandomScale(float MinScale)
        {
            Vector3 ToReturn = new Vector3(
                (float)RandomDouble(0.5f) + MinScale,
                (float)RandomDouble(0.5f) + MinScale, //Height
                (float)RandomDouble(0.5f) + MinScale);

            return ToReturn;
        }
    }
    
    //Included here again because of access problems
    public static double RandomDouble(double max)
    {
        //Seed randomizer from time
        int seed = (int)System.DateTime.Now.Ticks;
        System.Random r = new System.Random(seed);

        return (r.NextDouble() * max);
    }

    //Goes through the list of Spawnpoints and returns how many has a Resource on them
    public int ReturnSpawned()
    {
        int ToReturn = 0;

        foreach (RNodeSpawnpoint Spawnpoint in ListOfSpawnpoints)
        {
            if (Spawnpoint.HasSpawned)
            {
                ToReturn++;
            }
        }

        return ToReturn;
    }

    public class Reservation
    {
        //The Wisp who made the reservation
        public WispScript WispScriptRef;
        //A list of the resource node spawnpoints whose resources were reserved to this wisp
        public List<RNodeSpawnpoint> RNodeListSpawnpointList = new List<RNodeSpawnpoint>();

        //Add the passed Spawnpoint to the Reservation's list of spawnpoints
        public void MakeReservation(RNodeSpawnpoint SpawnpointToReserve)
        {
            RNodeListSpawnpointList.Add(SpawnpointToReserve);
        }
    }
}
