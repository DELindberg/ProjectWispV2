using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class ResourceNodeParentScript : SmartZoneParentScript {

    //Holds a reference to all of our reservation-objects
    public List<Reservation> ReservationList = new List<Reservation>();

    protected RNodeSpawnpoint[] ListOfSpawnpoints;
    protected int ResourceLimit;

    protected void Awake()
    {
        //Extend Awake from SmartZoneParentScript
        PreBuildInAwake();
    }

    //The name of the resource given and the amount given for each pickup
    protected void CustomUpdate(string ResourceName, int AmountToGive)
    {
        if (ReservationList.Count > 0)
        {
            foreach (Reservation Element in ReservationList)
            {
                //Debug.Log("Distance is: " + Vector3.Distance(Element.RNodeListSpawnpointList[0].SpawnpointObject.transform.position, Element.WispScriptRef.transform.position));
                if (Element.RNodeListSpawnpointList.Count > 0)
                {
                    //If the distance between the last reserved spawnpoint and the Wisp is small enough, and the Reservation is executing
                    if (Vector3.Distance(Element.RNodeListSpawnpointList[0].SpawnpointObject.transform.position, Element.WispScriptRef.transform.position) < 0.6
                        && Element.ReservationExecuting)
                    {
                        //Destroy mesh after specified delay
                        Element.RNodeListSpawnpointList[0].DestroyMesh();
                        //Remove the last spawnpoint from the list after a delay
                        Element.RemoveFirstReservation();

                        //Add to inventory
                        Element.WispScriptRef.AddResource(ResourceName, AmountToGive);
                        Element.WispScriptRef.CurrentlyCarrying++;
                        LocalResources[0].Amount--;

                        //Order the Wisp to move on to the next spawnpoint
                        FetchNext(Element.WispScriptRef);

                        //If we're reaching the last reservation in the list
                        if (Element.RNodeListSpawnpointList.Count == 0)
                        {
                            Element.ReservationExecuting = false;
                        }

                    }

                }
                //TODO: This solution is a bit wonky, would perhaps be better to have it all in the same statement
                else
                {
                    //Destroy mesh after specified delay
                    FetchNext(Element.WispScriptRef);
                    ReservationList.RemoveAt(GetReservationIndex(Element.WispScriptRef));
                    Debug.Log("ReservationList count at: " + ReservationList.Count);
                    break;
                }
            }
        }
    }
    
    //Runs through the list of spawnpoints and randomly picks one of the available spots to generate a resource on
    protected void GenerateResource(string ModelDirectory, float MinimumScale)
    {
        //TODO: This might need some tweaking, it only every takes up the first 9-10 spawnpoints, but that's ok mechanically for now
        int ToSubtract = (int)(RandomDouble(ResourceLimit - ReturnSpawned()));
        //Debug.Log("ToSubtract at: " + ToSubtract);
        foreach (RNodeSpawnpoint Spawnpoint in ListOfSpawnpoints)
        {
            if (!Spawnpoint.HasSpawned)
            {
                ToSubtract--;
                if (ToSubtract == 0)
                {
                    Spawnpoint.LoadMesh(ModelDirectory, MinimumScale);
                    Spawnpoint.HasSpawned = true;
                    Spawnpoint.IsReserved = false;
                    LocalResources[0].Amount++;

                    //Debug.Log("Tree spawned,  total resources at: " + LocalResources[0].Amount);
                    break;
                }
            }
        }
    }

    //Overload to handle multiple models for GenerateResource, randomly selects between the given models
    protected void GenerateResource(string ModelDirectory, string ModelDirectory2, float MinimumScale)
    {
        string ToLoad;

        if ((RandomDouble(1)) < 0.5)
        {
            ToLoad = ModelDirectory;
        }
        else
        {
            ToLoad = ModelDirectory2;
        }

        //TODO: This might need some tweaking, it only every takes up the first 9-10 spawnpoints, but that's ok mechanically for now
        int ToSubtract = (int)(RandomDouble(ResourceLimit - ReturnSpawned()));
        //Debug.Log("ToSubtract at: " + ToSubtract);
        foreach (RNodeSpawnpoint Spawnpoint in ListOfSpawnpoints)
        {
            if (!Spawnpoint.HasSpawned)
            {
                ToSubtract--;
                if (ToSubtract == 0)
                {
                    Spawnpoint.LoadMesh(ToLoad, MinimumScale);
                    Spawnpoint.HasSpawned = true;
                    Spawnpoint.IsReserved = false;
                    LocalResources[0].Amount++;

                    //Debug.Log("Tree spawned,  total resources at: " + LocalResources[0].Amount);
                    break;
                }
            }
        }
    }

    //Check if there's at least one tree available to be reserved
    public bool CanReserve()
    {
        bool ToReturn = false;
        foreach (RNodeSpawnpoint Element in ListOfSpawnpoints)
        {
            if (Element.HasSpawned && !Element.IsReserved)
            {
                ToReturn = true;
                break;
            }
        }
        return ToReturn;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag == "wisp")
        {
            WispScript TempWispRef = other.GetComponent<WispScript>();
            //TODO: Check if there's any references to the Wisp in the local list of reservations - if so, send it around to pick up the reserved trees
            for (int i = 0; i < ReservationList.Count; i++)
            {
                if (TempWispRef.WispName == ReservationList[i].WispScriptRef.WispName &&
                    TempWispRef.IsFetching)
                {
                    BeginFetching(TempWispRef);
                }
            }
        }
    }

    //Initialize the elements of the Spawnpoint array
    protected void PopulateListOfSpawnpoints()
    {
        for (int i = 0; i < ListOfSpawnpoints.Length; i++)
        {
            ListOfSpawnpoints[i] = new RNodeSpawnpoint();
        }
    }

    //Takes the Wisp reference, reserves up to the amount that the Wisp can carry, or less if less are available
    public void MakeReservation(WispScript WispScriptRef)
    {
        Reservation NewReservation = new Reservation();

        //Write the Wisp into the reservation
        NewReservation.WispScriptRef = WispScriptRef;

        //If there's any Wisps available
        if (WispScriptRef != null)
        {
            //Iterate through the Spawnpoints of the resource node
            for (int i = 0; i < ListOfSpawnpoints.Length; i++)
            {
                if (ReturnSpawned() >= 1 &&              //If there's one or more trees left
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
        else
        {
            Debug.Log("No reservation was made due to no Wisps being available");
        }
    }

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
            HasSpawned = false;
            Destroy(ResourceModel);
        }
        //I wanted to add a delayed call here, but it goes bonkers because it needs to inherit from Monobehavior, which hates instantiating with the New keyword
        //public IEnumerator DelayedDestroyMesh(int DelayTime)
        //{
        //    yield return new WaitForSeconds(DelayTime);
        //    DestroyMesh();
        //}
        //public void CallDelayedDestroyMesh(int DelayTime)
        //{
        //    StartCoroutine(DelayedDestroyMesh(DelayTime));
        //}


        Vector3 RandomScale(float MinScale)
        {
            Vector3 ToReturn = new Vector3(
                (float)RandomDouble(0.5f) + MinScale,
                (float)RandomDouble(0.5f) + MinScale, //Height
                (float)RandomDouble(0.5f) + MinScale);

            return ToReturn;
        }
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

    //Order the Wisp to move to the first spawnpoint it has reserved
    protected void FetchNext(WispScript WispScriptRef)
    {
        int ReservationNumber = GetReservationIndex(WispScriptRef);

        //If there's any next to fetch
        if (ReservationList[ReservationNumber].RNodeListSpawnpointList.Count > 0)
        {
            //Assign the location of the last spawnpoint found in the list of spawnpoints inside of the Reservation
            Vector3 SpawnpointLocation = ReservationList[ReservationNumber].RNodeListSpawnpointList[0].SpawnpointObject.transform.position;
            Debug.Log("Fetch next executing!");
            //Go to the location of the last spawnpoint in the list
            WispScriptRef.gameObject.GetComponent<NavMeshAgent>().SetDestination(SpawnpointLocation);
        }
        else //Send it back
        {
            WispScriptRef.gameObject.GetComponent<NavMeshAgent>().SetDestination(WispScriptRef.WorkLocation);
        }
    }
    public int GetReservationIndex(WispScript WispScriptRef)
    {
        int ReservationNumber = 0;


        //Find the reservation for the current Wisp
        for (int i = 0; i < ReservationList.Count; i++)
        {
            if (ReservationList[i].WispScriptRef.WispName == WispScriptRef.WispName)
            {
                ReservationNumber = i;
                break;
            }
        }

        return ReservationNumber;
    }

    //Call FetchNext after a delay
    public IEnumerator DelayedFetchNext(WispScript WispScriptRef, int DelayTime, Reservation ToExecute)
    {
        //Delay timer should AT MINIMUM match up with IEnumerator DelayedDestroyMesh
        yield return new WaitForSeconds(DelayTime);

        FetchNext(WispScriptRef);

        //Indicate that the Wisp is now moving again and that we should begin performing our distance checks
        ToExecute.ReservationExecuting = true;
    }
    //Access function used to call the IEnumerator from outside the script
    public void CallDelayedFetchNext(WispScript WispScriptRef, int DelayTime, Reservation ToExecute)
    {
        StartCoroutine(DelayedFetchNext(WispScriptRef, DelayTime, ToExecute));
        Debug.Log("Calling Delayed Fetch Next");
    }

    //Function to start the Wisp towards a resource node Spawnpoint when it first enters the trigger area
    protected void BeginFetching(WispScript WispScriptRef)
    {
        //Get the location of the last spawnpoint on the list related to the Wisp's reservation
        FetchNext(WispScriptRef);
    }

    protected bool CheckFetchDistance(WispScript WispScriptRef, Vector3 SpawnpointLocation)
    {
        bool ToReturn = false;
        if (Vector3.Distance(WispScriptRef.gameObject.transform.position, SpawnpointLocation) > 0.6)
        {
            ToReturn = true;
        }
        return ToReturn;
    }

    public class Reservation
    {
        //The Wisp who made the reservation
        public WispScript WispScriptRef;
        //A list of the resource node spawnpoints whose resources were reserved to this wisp
        public List<RNodeSpawnpoint> RNodeListSpawnpointList = new List<RNodeSpawnpoint>();

        //Used to indicate whether a Wisp is currently in transition between resource spawnpoints
        public bool ReservationExecuting = true;    //Initialized to true, as we know someone made the reservation

        //Add the passed Spawnpoint to the Reservation's list of spawnpoints
        public void MakeReservation(RNodeSpawnpoint SpawnpointToReserve)
        {
            RNodeListSpawnpointList.Add(SpawnpointToReserve);
        }

        //Remove the first element in the list
        public void RemoveFirstReservation()
        {
            RNodeListSpawnpointList.RemoveAt(0);
        }
    }
}
