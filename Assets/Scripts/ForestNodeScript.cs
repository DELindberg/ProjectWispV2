using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestNodeScript : MonoBehaviour {

    RNodeSpawnpoint[] ListOfSpawnpoints;
    public int ResourceLimit;

    //Goes through the list of Spawnpoints and returns how many has a Resource on them
    int ReturnSpawned()
    {
        int ToReturn = 0;

        foreach(RNodeSpawnpoint Spawnpoint in ListOfSpawnpoints)
        {
            if(Spawnpoint.HasSpawned)
            {
                ToReturn++;
            }
        }

        return ToReturn;
    }

	// Use this for initialization
	void Start () {
        ResourceLimit = 10;
        ListOfSpawnpoints = new RNodeSpawnpoint[15];
        GetSpawnPoints();

        //InvokeRepeating("SpamTreesEverywhere", 0.01f, 1f);

        InvokeRepeating("UpdateNode", 0.1f , 1f);
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
                    //ListOfSpawnpoints[0].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[0].SpawnpointObject = t.gameObject;
                    //ListOfSpawnpoints[0].LoadMesh();
                    break;
                case "Spawnpoint (1)":
                    //ListOfSpawnpoints[1].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[1].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (2)":
                    //ListOfSpawnpoints[2].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[2].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (3)":
                    //ListOfSpawnpoints[3].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[3].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (4)":
                    //ListOfSpawnpoints[4].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[4].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (5)":
                    //ListOfSpawnpoints[5].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[5].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (6)":
                    //ListOfSpawnpoints[6].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[6].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (7)":
                    //ListOfSpawnpoints[7].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[7].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (8)":
                    //ListOfSpawnpoints[8].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[8].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (9)":
                    //ListOfSpawnpoints[9].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[9].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (10)":
                    //ListOfSpawnpoints[10].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[10].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (11)":
                    //ListOfSpawnpoints[11].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[11].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (12)":
                    //ListOfSpawnpoints[12].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[12].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (13)":
                    //ListOfSpawnpoints[13].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[13].SpawnpointObject = t.gameObject;
                    break;
                case "Spawnpoint (14)":
                    //ListOfSpawnpoints[14].SpawnpointLocation = t.transform.position;
                    ListOfSpawnpoints[14].SpawnpointObject = t.gameObject;
                    break;
            }               
        }
    }

    //FOR TESTING, CYCLES TREES ON ALL THE SPAWNPOINTS
    void SpamTreesEverywhere()
    {
        for(int i = 0; i < ListOfSpawnpoints.Length; i++)
        {
            ListOfSpawnpoints[i].LoadMesh();
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
        Debug.Log("ToSubtract at: " + ToSubtract);
        foreach(RNodeSpawnpoint Spawnpoint in ListOfSpawnpoints)
        {
            if(!Spawnpoint.HasSpawned)
            {
                ToSubtract--;
                if(ToSubtract == 0)
                {
                    Spawnpoint.LoadMesh();
                    Spawnpoint.HasSpawned = true;
                    break;
                }
            }
        }
    }
    //A randomizer outputting between zero and max, copied from original project
    public double RandomDouble(double max)
    {
        //Seed randomizer from time
        int seed = (int)System.DateTime.Now.Ticks;
        System.Random r = new System.Random(seed);

        return (r.NextDouble() * max);
    }

    public class RNodeSpawnpoint
    {
        public bool HasSpawned = false;     //Whether there's a resource or not
        public Vector3 SpawnpointLocation;  //TODO: OBSOLETE AT THE MOMENT, REMOVE LATER IF STILL NOT RELEVANT
        public GameObject SpawnpointObject; //A reference to the spawnpoint object itself
        public GameObject ResourceModel;   //A reference to the resource prefab we're instantiating


        public void LoadMesh()
        {
            Debug.Log("Running LoadMesh()...");
            //Instantiate prefab from Resources-folder
            var temp = Instantiate(Resources.Load("LowPolyTree2") as GameObject);
            ResourceModel = temp;  //Store reference

            Vector3 RandomRotation = new Vector3(0f, (float)RandomDouble(360f), 0f);

            HasSpawned = true;

            //Set parent and configure transform for the instantiated prefab
            temp.transform.parent = SpawnpointObject.transform;
            temp.transform.position = SpawnpointObject.transform.position;
            temp.transform.rotation = SpawnpointObject.transform.rotation;    //Match rotation on all three axis with spawnpoint, necessary on incline surfaces
            temp.transform.Rotate(RandomRotation);
            temp.transform.localScale = RandomScale(); ;
        }
        public void DestroyMesh()
        {
            Destroy(ResourceModel);
        }

        Vector3 RandomScale()
        {
            float MinScale = 0.6f;

            Vector3 ToReturn = new Vector3(
                (float)RandomDouble(0.5f) + MinScale, 
                (float)RandomDouble(0.5f) + MinScale, //Height
                (float)RandomDouble(0.5f) + MinScale);

            return ToReturn;
        }
        public double RandomDouble(double max)
        {
            //Seed randomizer from time
            int seed = (int)System.DateTime.Now.Ticks;
            System.Random r = new System.Random(seed);

            return (r.NextDouble() * max);
        }
    }

    //Periodically called to update the Resource node
    void UpdateNode()
    {
        Debug.Log(ReturnSpawned() + " trees have been spawned.");

        //If there's less resources than the upper limit
        if(ReturnSpawned() < ResourceLimit)
        {
            GenerateResource();
        }
    }


}
