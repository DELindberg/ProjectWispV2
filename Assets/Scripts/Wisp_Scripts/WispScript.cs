using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class WispScript : MonoBehaviour
{
    //Identification variables
    public string WispName, //The ID used to identify the Wisp
        OriginalObjectName, //The name originally assigned to the duplicate
        AssignedBuildingID, //The ID of the Smart Zone where the Wisp is working
        FetchID;            //The ID of the Smart Zone where the Wisp has been sent by another Smart Zone to fetch something,
                            //or someplace it has chosen to go on its own accord that is not the Smart Zone it works in
    
    //Coordinate storage to prevent the Wisps from making unnecessary queries when pathfinding
    public Vector3 WorkLocation;
    
    //Counting-variable used to determine querying priority by the Wisp Controller
    public float TimeSinceQuery;

    //Variables used to establish connection to the Wisp Controller object
    GameObject WispController;
    WispControllerScript WCReference;

    NavMeshAgent NMAgent;

    //Possible states for the Wisp to exist in
    public bool IsBusy, //If it can't be queried 
        IsFetching,     //If it's fetching something
        IsPresent;      //If it's present in the building

    //Variable used in conjunction with Smart Zone design to inject the proper animations
    public int AnimationID;

    //Integers used to determine how many resources the Wisps are allowed to carry at once
    public int CarryCapacity, CurrentlyCarrying, CurrentlyReserved;

    //Variables used to establish connection to the Resource Controller object
    public ResourceType[] ResourcesCarried;
    GameObject ResourceController;
    public ResourceControllerScript ResourceControllerRef;

    //Used to communicate with the Smart Zone Controller:
    GameObject SmartZoneController;
    public SmartZoneControllerScript SmartZoneControllerRef;

    /// <summary>
    /// Dual Utility reasoning components below:
    /// </summary>
    double[] UtilityValue, ProbabilityValue;
    int[] CategoryValue;
    //These values are the ones given at "birth" when the Wisp is first created
    double RandomValue, Hunger = 0, Sleep = 0, Happiness = 0, Morale;
    public string RoleValue = "builder"; //Role value indicates which roles are assigned to the individual

    // Use this for initialization
    void Start()
    {
        UtilityValue = new double[4];
        ProbabilityValue = new double[4];
        CategoryValue = new int[4];

        //Store the original name of the gameobject, not sure if we need this, but since we modify the names in the smart zones
        //it might be a good idea to save the name given when the duplicate is instantiated
        OriginalObjectName = this.gameObject.name;

        //REMEMBER TO ENSURE THE NAME IS CORRECT IN THE EDITOR
        WispController = GameObject.Find("WispController");
        WCReference = WispController.GetComponent<WispControllerScript>();

        WispName = WCReference.GetName();
        WCReference.ListOfWisps.Add(this);
        WCReference.Announce(); //TODO: For debugging, remove in final build

        //Copy list of resources possible to carry, from the resource controller
        ResourceController = GameObject.Find("ResourceController");
        ResourceControllerRef = ResourceController.GetComponent<ResourceControllerScript>();
        
        //Copy list from resource controller to Wisp
        ResourcesCarried = new ResourceType[ResourceControllerRef.ResourceList.Length];
        for(int i = 0; i < ResourcesCarried.Length; i++)
        {
            ResourcesCarried[i] = new ResourceType();
            ResourcesCarried[i].Name = ResourceControllerRef.ResourceList[i].Name;
        }


        //Connect to the Smart Zone Controller:
        SmartZoneController = GameObject.Find("SmartZoneController"); //:REMEMBER TO ENSURE THAT THIS NAME IS CORRECT IN THE EDITOR
        SmartZoneControllerRef = SmartZoneController.GetComponent<SmartZoneControllerScript>();

        //Connect to the Navmesh agent attached to this instance of the Wisp
        NMAgent = this.gameObject.GetComponent<NavMeshAgent>();

        //Using positive infinity to indicate that no place has been assigned
        //WorkLocation = Vector3.positiveInfinity;

        IsBusy = false;
        IsFetching = false;
        IsPresent = false;

        CarryCapacity = 3;
        CurrentlyCarrying = 0;

        //FOR TESTING!!!
        AssignWorkplace();

        //Every second with 1 second delay, call the function that adds 1 to the timer
        InvokeRepeating("CountSinceQuery", 1f, 1f);

        //Every 5 seconds, update the internal needs of the Wisp
        InvokeRepeating("UpdateNeeds", 10f, 10f);
    }

    //Increments the timer to track how long it's been since last query
    void CountSinceQuery()
    {
        TimeSinceQuery++;
        //Debug.Log(WispName + " 1s added to TimeSinceQuery");
    }

    private void OnDestroy()
    {
        //Remove the Wisp from the WispController's list
        WCReference.WispDestroyer(WispName);
    }

    //Update the behavior of the Wisp - called when queried by the Wisp controller
    public void UpdateBehavior()
    {
        //Convert utility values into probability values for weight-based random
        WeightBasedRandom(UtilityValue, ProbabilityValue);
        //Find the random value to subtract from
        RandomValue = FindRandom(ProbabilityValue);
        //Subtract to zero from the found random value, and execute the relevant behavior
        //TODO: Uncomment this when behaviors need to be updated again
        //ExecuteBehavior(SubtractToZero(RandomValue, ProbabilityValue));

        //Debug.Log(WispName + "'s behavior was queried!");
        TimeSinceQuery = 0;
    }

    //Frequently updates the utility values of the Wisp's needs
    void UpdateNeeds()
    {
        Hunger += 0.05;
        Sleep += 0.02;
        Happiness += 0.01;
        Morale = CalculateMorale(Hunger, Sleep, Happiness);

        //TODO: Create a check to see if any of these values exceed a threshold which causes the Wisp to die

        //TODO: Consider potential modifiers in the design in different contexts, such as different buildings affecting the Wisps differently (eg. bakers never get hungry at work)

        //Insert updated values into utility array
        UpdateUtility(Hunger, Sleep, Happiness, Morale);

        //FOR TESTING
        //FindFood();
    }

    //Set new parent
    public void SetParent(GameObject NewParent)
    {
        this.transform.parent = NewParent.transform;
        //Debug.Log("Parent updated!");
    }

    //Disassociate from the parent object
    public void DetachParent()
    {
        this.transform.parent = null;
    }

    //Calculate the value of Morale based on the values of the other utility parameters
    double CalculateMorale(double Hunger, double Sleep, double Happiness)
    {
        double Morale = (2 - Hunger) + (2 - Sleep) + (2 - Happiness);
        return Morale;
    }

    //Update the utility array with the utility values squared
    void UpdateUtility(double Hunger, double Sleep, double Happiness, double Morale)
    {
        UtilityValue[0] = (double)Mathf.Pow((float)Hunger, 2);
        UtilityValue[1] = (double)Mathf.Pow((float)Sleep, 2);
        UtilityValue[2] = (double)Mathf.Pow((float)Happiness, 2);
        UtilityValue[3] = (double)Mathf.Pow((float)Morale, 2);
    }

    //Calculates probability values
    void WeightBasedRandom(double[] ArrayUtility, double[] ArrayProbability)
    {
        for (int i = 0; i < ArrayUtility.Length; i++)
        {
            double SumOfOthers = 0;
            //Add all of the utility values together
            for (int j = 0; j < ArrayUtility.Length; j++)
            {
                SumOfOthers += ArrayUtility[j];
            }
            //Subtract own utility value
            SumOfOthers -= ArrayUtility[i];

            //Calculate and assign probability value
            ArrayProbability[i] = (ArrayUtility[i] / SumOfOthers);
            //Debug.Log("Probability value index " + i + " is at; " + ArrayProbability[i] + "\n");
            //Debug.Log("SumOfOthers value index " + i + " is at; " + SumOfOthers + "\n");
        }
    }

    //Accumulate probability values to find the random number within the given range (zero to sum of probability values)
    double FindRandom(double[] ArrayProbability)
    {
        double TempSum = 0;
        double ToReturn;

        //Add all of the probability values together in TempSum
        for (int i = 0; i < ArrayProbability.Length; i++)
        {
            TempSum += ArrayProbability[i];
            //Debug.Log("TempSum temporary value: " + TempSum);
        }

        //Call the randomizer with the given max value of TempSum
        ToReturn = RandomDouble(TempSum);

        //Debug.Log("The result of FindRandom is: " + ToReturn);

        return ToReturn;
    }

    //An extension of FindRandom which handles the randomization
    double RandomDouble(double max)
    {
        //Seed randomizer from time
        int seed = (int)System.DateTime.Now.Ticks;
        System.Random r = new System.Random(seed);

        return (r.NextDouble() * max);
    }

    //Cycle through probability-values subtracting from the random value, until zero or below-zero is reached
    int SubtractToZero(double RandomVal, double[] ArrayProbability)
    {
        int i = 0;
        while (RandomVal > 0)
        {
            RandomVal -= ArrayProbability[i];


            if (RandomVal < 0)
            {
                //Debug.Log("Index returned is at: " + i + "\n");
                //Debug.Log("RandomVal found is at: " + RandomVal);
                return i;
            }
            i++;
        }

        return 0;
    }

    void ExecuteBehavior(int BehaviorID)
    {
        //Determine what behavior to execute
        switch (BehaviorID)
        {
            case 0:
                FindFood();
                break;
            case 1:
                FindSleep();
                break;
            case 2:
                FindHappiness();
                break;
            //Case 4 will execute differently, depending on what role has been assigned to the NPC
            case 3:
                GoWork();
                break;
        }
    }

    void FindFood()
    {
        //Query list of Smart Zones for nearest place to eat

        //Initialized to prevent errors
        Vector3 FinalCoordinate = this.transform.position;
        float ShortestDistance = float.MaxValue;
        //int IndexToReserve = 0;

        //For each Smart Zone in the list
        for(int i = 0; i < SmartZoneControllerRef.ListOfSmartZones.Count; i++)
        {
            //For each of the resource types inside the Smart Zone
            for(int j = 0;j < SmartZoneControllerRef.ListOfSmartZones[i].LocalResources.Length; j++)
            {
                if (SmartZoneControllerRef.ListOfSmartZones[i].LocalResources[j].Type == "output" &&    //If the resource is an output-type
                    SmartZoneControllerRef.ListOfSmartZones[i].LocalResources[j].Amount > 0 &&          //If there's more than zero of the resource
                    SmartZoneControllerRef.ListOfSmartZones[i].LocalResources[j].Name == "meat" ||      //If the resource is meat
                    SmartZoneControllerRef.ListOfSmartZones[i].LocalResources[j].Name == "bread")       //Or if it's bread
                {
                    //If the distance between here and the building we're examining, is shorter than the shortest distance observed so far
                    if(Vector3.Distance(this.transform.position, SmartZoneControllerRef.ListOfSmartZones[i].transform.position) < ShortestDistance )
                    {
                        //Set the new final coordinate
                        FinalCoordinate = SmartZoneControllerRef.ListOfSmartZones[i].transform.position;
                        //Update shortest distance to the newly observed shortest distance
                        ShortestDistance = Vector3.Distance(this.transform.position, SmartZoneControllerRef.ListOfSmartZones[i].transform.position);

                       // IndexToReserve = i;   //Previously used to identify which index in the List of Smart Zones the output building was in
                    }
                }
            }
        }
        GotoLocation(FinalCoordinate);
    }

    void FindSleep()
    {
        //TODO: Query list of Smart Zones for nearest place to sleep
    }

    void FindHappiness()
    {
        //TODO: Query list of Smart Zones for nearest place to get happy
    }

    public void GoWork()
    {
        //TODO: If a workplace has been assigned, go there. If already there, do nothing. If no workplace has been assigned (the Wisp is a builder) look for Smart Zones which require building
        //if(WorkLocation != Vector3.positiveInfinity)
        //{
            GotoLocation(WorkLocation);
        //}
        //else
        //{
        //    //TODO: Look for construction sites
        //}
    }

    public void GotoLocation(Vector3 DestinationCoordinate)
    {
        //Update navmesh agent to the destination coordinate.
        NMAgent.destination = DestinationCoordinate;
    }

    public void AddResource(string ResourceID, int Amount)
    {
        for(int i = 0; i < ResourcesCarried.Length; i++)
        {
            if(ResourcesCarried[i].Name == ResourceID)
            {
                ResourcesCarried[i].Amount += Amount;
                break;
            }
        }
    }

    //FOR TESTING!! Automatic assignment of Wisps to a workplace
    void AssignWorkplace()
    {

        //TODO: Rewrite this to assign the closest woodcutter, so that we can test having multiple instances
        //for (int i = 0; i < SmartZoneControllerRef.ListOfSmartZones.Count; i++)
        //{
        //    Debug.Log("For loop run");
        //    //Change the string when matching against other Smart Zone types
        //    if (SmartZoneControllerRef.ListOfSmartZones[i].ZoneType == "woodcutter" && SmartZoneControllerRef.ListOfSmartZones[i].AssignedWisps.Count < SmartZoneControllerRef.ListOfSmartZones[i].WispLimit)
        //    {
        //        Debug.Log("Workplace assigned");
        //        AssignedBuildingID = SmartZoneControllerRef.ListOfSmartZones[i].ZoneID;
        //        WorkLocation = SmartZoneControllerRef.ListOfSmartZones[i].gameObject.transform.position;
        //        GoWork();       //Go to the work location we've found
        //        break;  //Only assign to the first building we find
        //    }
        //}

        //Initialized to prevent errors
        Vector3 FinalCoordinate = this.transform.position;
        float ShortestDistance = float.MaxValue;

        //Used to refer to the most optimal Forest node we'll find
        WoodcutterSZScript WoodcutterRef = null;

        //Look through the list of Smart Zones for someplace that serves the input resource, call Wisp Fetch behavior on it
        for (int i = 0; i < SmartZoneControllerRef.ListOfSmartZones.Count; i++)
        {
            //Debug.Log("Iterating through: " + SmartZoneControllerRef.ListOfSmartZones[i].ZoneType);

            //If the Smart Zone checked is a forest node
            if (SmartZoneControllerRef.ListOfSmartZones[i].ZoneType == "woodcutter")
            {
                //Connect to the forest node
                WoodcutterSZScript WoodcutterTempRef = SmartZoneControllerRef.ListOfSmartZones[i].GetComponent<WoodcutterSZScript>();

                //If there's one or more of the resource available and it's the closest observed node so far
                if (Vector3.Distance(this.transform.position, SmartZoneControllerRef.ListOfSmartZones[i].transform.position) < ShortestDistance)
                {
                    //Set the new final coordinate
                    FinalCoordinate = SmartZoneControllerRef.ListOfSmartZones[i].transform.position;
                    //Update shortest distance to the newly observed shortest distance
                    ShortestDistance = Vector3.Distance(this.transform.position, SmartZoneControllerRef.ListOfSmartZones[i].transform.position);

                    //Update the reference to the newly found best candidate
                    WoodcutterRef = WoodcutterTempRef;
                }
            }
        }
        AssignedBuildingID = WoodcutterRef.ZoneID;
        WorkLocation = WoodcutterRef.gameObject.transform.position;
        GoWork();
    }

}


