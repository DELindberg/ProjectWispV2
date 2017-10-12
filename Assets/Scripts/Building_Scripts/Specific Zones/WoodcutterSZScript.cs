using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class WoodcutterSZScript : SmartZoneParentScript
{
    //Add more of these if you ever add more input or output resources to the building
    int InputResourceID;
    int OutputResourceID;

    //SET VIA INSPECTOR
    public GameObject Animator1;
    public GameObject Animator2;

    Animation Animat1;
    Animation Animat2;

    string ResourceName1;
    string ResourceName2;

    protected void Awake()
    {
        //Extend Awake from SmartZoneParentScript
        PreBuildInAwake();

        //Define identifiers for Zone- and Resource-types
        ZoneType = "woodcutter";
        ResourceName1 = "forest";
        ResourceName2 = "wood";
        
        //Establish resources and find the references
        LocalResources = new ResourceType[2];   //Adjust this number if resources are added or removed to the building's design
        PopulateResourceList(LocalResources);
        InitializeResources();
        FindResourceID(out InputResourceID, out OutputResourceID);

        WispLimit = 2;

        //Check if the building can produce its output resource(s)
        InvokeRepeating("Produce", 10f, 10f);

        //FOR TESTING!!
        Invoke("SendToFetch", 20f);
    }

    //Production-related functions
    void InitializeResources() //WATCH OUT FOR TYPOS IN HERE
    {
        //Input-type resources
        LocalResources[0].Name = ResourceName1; //"forest"
        LocalResources[0].Type = "input";

        //Output-type resources
        LocalResources[1].Name = ResourceName2; //"wood"
        LocalResources[1].Type = "output";
    } 
    void Produce()
    {
        //If the required input is above zero and there's Wisps present
        if(LocalResources[0].Amount > 0 && PresentWispAmount() > 0)
        {
            //If there's only ONE unit of forest available
            if(LocalResources[0].Amount == 1)
            {
                LocalResources[0].Amount -= 1;
                LocalResources[1].Amount += 2;
            }
            else
            {
                //Produce up to double depending on number of Wisps present
                LocalResources[0].Amount -= (1 * PresentWispAmount());
                LocalResources[1].Amount += (2 * PresentWispAmount());
            }
        }
        else
        {
            //Send out a Wisp to fetch more resources
            SendToFetch();
        }
    }

    //Function used to 
    void SendToFetch()
    {
        //Used for referring to the Wisp we'll send out to fetch materials
        WispScript WispScriptRef = null;

        //Call a function here that returns a reference to the first available Wisp of the assigned Wisps, could be written in the Smart Zone parent script
        if (ReturnAssignedAvailable(AssignedWisps) != null)//If there's any available Assigned Wisps
        {
            //Get the first available assigned Wisp from the building
            WispScriptRef = ReturnAssignedAvailable(AssignedWisps);

            if(AnimateCount < 0)
            {
                AnimateCount--;
            }
        }
        else
        {
            Debug.Log("No Wisps could be added from the list of assigned Wisps");
        }

        //Initialized to prevent errors
        Vector3 FinalCoordinate = this.transform.position;
        float ShortestDistance = float.MaxValue;

        //Used to refer to the most optimal Forest node we'll find
        ForestNodeScript ForestNodeRef = null;

        //Look through the list of Smart Zones for someplace that serves the input resource, call Wisp Fetch behavior on it
        for (int i = 0; i < SmartZoneControllerRef.ListOfSmartZones.Count; i++)
        {
            //Debug.Log("Iterating through: " + SmartZoneControllerRef.ListOfSmartZones[i].ZoneType);

            //If the Smart Zone checked is a forest node
            if (SmartZoneControllerRef.ListOfSmartZones[i].ZoneType == "forestnode")
            {
                //Connect to the forest node
                ForestNodeScript ForestNodeTempRef = SmartZoneControllerRef.ListOfSmartZones[i].GetComponent<ForestNodeScript>();

                //If there's one or more of the resource available and it's the closest observed node so far
                if (ForestNodeTempRef.ReturnSpawned() >= 1 &&
                    Vector3.Distance(this.transform.position, SmartZoneControllerRef.ListOfSmartZones[i].transform.position) < ShortestDistance)
                {
                    //Set the new final coordinate
                    FinalCoordinate = SmartZoneControllerRef.ListOfSmartZones[i].transform.position;
                    //Update shortest distance to the newly observed shortest distance
                    ShortestDistance = Vector3.Distance(this.transform.position, SmartZoneControllerRef.ListOfSmartZones[i].transform.position);

                    //Update the reference to the newly found best candidate
                    ForestNodeRef = ForestNodeTempRef;
                }
            }
        }

        //Only proceed if there's actually a Wisp available to recieve these commands
        if (WispScriptRef != null)
        {
            switch(WispScriptRef.gameObject.name)
            {
                case "Wisp":
                    Animat1.Stop();
                    break;
                case "Wisp (1)":
                    Animat2.Stop();
                    break;
            }
            //Reserve resources at the forest node
            ForestNodeRef.MakeReservation(WispScriptRef);

            //Set the reservation object to "executing" to indicate that it's being processed
            ForestNodeRef.ReservationList[0].ReservationExecuting = true;

            //Reenable the Navmesh Agent on the Wisp, order it to move to the found node and set it to busy and fetching
            WispScriptRef.gameObject.GetComponent<NavMeshAgent>().enabled = true;
            WispScriptRef.IsFetching = true;
            WispScriptRef.IsBusy = true;
            WispScriptRef.FetchID = ForestNodeRef.ZoneID;
            WispScriptRef.GotoLocation(ForestNodeRef.transform.position);
            WispScriptRef.transform.parent = null;

            Debug.Log("Wisp has been sent to fetch");
        }
    }

    //Logic handling Wisps entering the Smart Zone
    private void OnTriggerEnter(Collider other)
    {
        //REMEMBER to add the "wisp" tag to the Wisp prefab
        if(other.tag == "wisp")
        {
            WispScript TempWispRef = other.GetComponent<WispScript>();

            //If Wisp is not busy, was sent to work here and has its default role "builder"
            if(TempWispRef.IsBusy == false && //TODO: When adding the manual role-assignment, this should be changed to true to prevent "distracting" query before role is taken 
                TempWispRef.AssignedBuildingID == ZoneID && 
                TempWispRef.RoleValue == "builder")
            {
                //Add the Wisp to the local list of assigned Wisps
                AssignWisp(TempWispRef);
                TempWispRef.RoleValue = "woodcutter";
                TempWispRef.IsPresent = true;

                //Temporarily disable the NavMesh Agent
                TempWispRef.gameObject.GetComponent<NavMeshAgent>().enabled = false;

                ParentAndAnimate(TempWispRef);

            }
            //If the Wisp works here, is fetching and carries the input resource
            else if(TempWispRef.AssignedBuildingID == ZoneID && TempWispRef.RoleValue == "woodcutter" && TempWispRef.IsFetching && TempWispRef.CurrentlyCarrying > 0)
            {
                TempWispRef.IsPresent = true;

                //Temporarily disable the NavMesh Agent
                TempWispRef.gameObject.GetComponent<NavMeshAgent>().enabled = false;
                Debug.Log("Return fetching code triggering...");
                ParentAndAnimate(TempWispRef);

                for(int i = 0; i < TempWispRef.ResourcesCarried.Length; i++)
                {
                    //It the name matches the input resource - ATTENTION!: MORE THAN ONE OF THESE ARE NEEDED IF IT CAN FETCH MORE THAN ONE RESOURCE TYPE
                    if(TempWispRef.ResourcesCarried[i].Name == LocalResources[0].Name && TempWispRef.ResourcesCarried[i].Amount > 0)
                    {
                        //Perform resource transfer from Wisp to local pool
                        LocalResources[0].Amount += TempWispRef.ResourcesCarried[InputResourceID].Amount;
                        TempWispRef.ResourcesCarried[InputResourceID].Amount = 0;
                        TempWispRef.CurrentlyCarrying = 0;
                    }
                }
            }
            //If the Wisp is busy, fetching, it doesn't work here and the FetchID it has been biven matches that of the building
            else if(TempWispRef.IsBusy && TempWispRef.IsFetching && TempWispRef.AssignedBuildingID != ZoneID && TempWispRef.FetchID == ZoneID)
            {
                //TODO: Perform transfer from output pool to the Wisp, remove the resource reservation and send the Wisp back home

                //TEST!
                TempWispRef.IsPresent = true;
                TempWispRef.gameObject.GetComponent<NavMeshAgent>().enabled = false;
                Debug.Log("Return fetching code triggering...");
                ParentAndAnimate(TempWispRef);
            }
        }
    }

    void AnimateOnTriggerEnter(WispScript WispScriptRef)
    {
        if (WispScriptRef.AssignedBuildingID == ZoneID)
        {
            //Call the check to return the first available animation ID, assign appropriate name and begin animation
            switch (AnimateCount)
            {
                //If nothing is animating
                case 0:
                    //THE ANIMATION COMPONENT IS INITIALIZED HERE, because if it's initialized in a different block of code,
                    //Unity bugs out and delays the parenting process which causes the animation to fail in finding the object to animate
                    Animat1 = Animator1.GetComponent<Animation>();
                    SetParent(Animator1, WispScriptRef); //Set this building as the new parent
                    WispScriptRef.gameObject.name = "Wisp";   //Change the object name so the animator can use it

                    if(WispScriptRef.IsFetching && WispScriptRef.CurrentlyCarrying > 0)
                    {
                        //TODO: Load the offloading animation
                        Animat1.Play("WoodcutterOffload1");
                    }
                    else
                    {
                        Animat1.Play("WoodcutterAnim1");
                    }

                    Debug.Log("Animating 1");
                    break;

                //If one Wisp is already animating
                case 1:
                    Animat2 = Animator2.GetComponent<Animation>();
                    SetParent(Animator2, WispScriptRef);
                    WispScriptRef.gameObject.name = "Wisp (1)";

                    if (WispScriptRef.IsFetching && WispScriptRef.CurrentlyCarrying > 0)
                    {
                        //TODO: Load the offloading animation 
                    }
                    else
                    {
                        Animat2.Play("WoodcutterAnim2");
                    }

                    Debug.Log("Animating 2");
                    break;
            }
        }
    }
    void ParentAndAnimate(WispScript WispScriptRef)
    {
        bool ExecuteThis;
        foreach(Transform child in transform)
        {
            ExecuteThis = true;
            if (child.name == "Anim1")
            {
                //If a child exists under Anim1
                foreach (Transform subchild in child.transform)
                {
                    ExecuteThis = false;
                }
                //If no child was found
                if (ExecuteThis)
                {
                    AnimateCount = 0;
                    break;
                }
            }
            else if (child.name == "Anim2")
            {
                //If a child exists under Anim1
                foreach (Transform subchild in child.transform)
                {
                    ExecuteThis = false;
                }
                //If no child was found
                if (ExecuteThis)
                {
                    AnimateCount = 1;
                    break;
                }
            }
        }
        AnimateOnTriggerEnter(WispScriptRef);
    }

    //Returns the index numbers equivalent to the resource IDs, modify this function if you ever add more input or output resources to the building
    void FindResourceID(out int InputResourceID, out int OutputResourceID)
    {
        //Defined to avoid errors in handling out-parameters
        InputResourceID = 0;
        OutputResourceID = 0;

        //Find the index numbers for the input and output resources
        for(int i = 0; i < ResourceControllerRef.ResourceList.Length; i++)
        {
            if(LocalResources[0].Name == ResourceControllerRef.ResourceList[i].Name)
            {
                InputResourceID = i;
            }
            else if(LocalResources[1].Name == ResourceControllerRef.ResourceList[i].Name)
            {
                OutputResourceID = i;
            }
        }
    }

}
