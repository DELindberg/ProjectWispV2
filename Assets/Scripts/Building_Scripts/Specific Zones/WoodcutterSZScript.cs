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

    protected void Awake()
    {
        //Extend Awake from SmartZoneParentScript
        PreBuildInAwake();

        ZoneType = "woodcutter";
        
        //Establish resources and find the references
        LocalResources = new ResourceType[2];   //Adjust this number if resources are added or removed to the building's design
        PopulateResourceList(LocalResources);
        InitializeResources();
        FindResourceID(out InputResourceID, out OutputResourceID);

        WispLimit = 2;

        //Check if the building can produce its output resource(s)
        InvokeRepeating("Produce", 10f, 10f);
    }

    //Production-related functions
    void InitializeResources() //WATCH OUT FOR TYPOS IN HERE
    {
        //Input-type resources
        LocalResources[0].Name = "forest";
        LocalResources[0].Type = "input";

        //Output-type resources
        LocalResources[1].Name = "wood";
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
            //TODO: Look through the list of Smart Zones for someplace that serves the input resource, call Wisp Fetch behavior on it
        }
    }


    //Logic handling Wisps entering the Smart Zone
    private void OnTriggerEnter(Collider other)
    {
        //TODO: REMEMBER to add the "wisp" tag to the Wisp prefab
        if(other.tag == "wisp")
        {
            WispScript TempWispRef = other.GetComponent<WispScript>();

            //If Wisp is not busy, was sent to work here and has its default role "builder"
            if(TempWispRef.IsBusy == false && 
                TempWispRef.AssignedBuildingID == ZoneID && 
                TempWispRef.RoleValue == "builder")
            {
                //Add the Wisp to the local list of assigned Wisps
                AssignWisp(TempWispRef);
                TempWispRef.RoleValue = "woodcutter";
                TempWispRef.IsPresent = true;


                //TODO: FIND SOME WAY TO INTEGRATE SMART ZONE DESIGN WITH NAVMESH BEHAVIOR
                //Temporarily disable the NavMesh Agent
                TempWispRef.gameObject.GetComponent<NavMeshAgent>().enabled = false;

                //TODO: Call the check to return the first available animation ID, assign appropriate name and begin animation
                switch (AnimateCount)
                {
                    //If nothing is animating
                    case 0:
                        //THE ANIMATION COMPONENT IS INITIALIZED HERE, because if it's initialized in a different block of code,
                        //Unity bugs out and delays the parenting process which causes the animation to keel over with a weird intestinal hernia
                        Animation Animat1 = Animator1.GetComponent<Animation>();
                        SetParent(Animator1, TempWispRef); //Set this building as the new parent
                        TempWispRef.gameObject.name = "Wisp";   //Change the object name so the animator can use it

                        Animat1.Play();
                        AnimateCount++;
                        break;
                    
                    //If one Wisp is already animating
                    case 1:
                        Animation Animat2 = Animator2.GetComponent<Animation>();
                        SetParent(Animator2, TempWispRef);
                        TempWispRef.gameObject.name = "Wisp (1)";
                        Animat2.Play();
                        break;
                }
            }
            //If the Wisp works here, is fetching and carries the input resource
            else if(TempWispRef.AssignedBuildingID == ZoneID && TempWispRef.RoleValue == "woodcutter" && TempWispRef.IsFetching)
            {
                for(int i = 0; i < TempWispRef.ResourcesCarried.Count; i++)
                {
                    //It the name matches the input resource - ATTENTION!: MORE THAN ONE OF THESE ARE NEEDED IF IT CAN FETCH MORE THAN ONE RESOURCE TYPE
                    if(TempWispRef.ResourcesCarried[i].Name == LocalResources[0].Name && TempWispRef.ResourcesCarried[i].Amount > 0)
                    {
                        //TODO: Perform resource transfer from Wisp to local pool
                    }
                }
            }
            //If the Wisp is busy, fetching, it doesn't work here and the FetchID it has been biven matches that of the building
            else if(TempWispRef.IsBusy && TempWispRef.IsFetching && TempWispRef.AssignedBuildingID != ZoneID && TempWispRef.FetchID == ZoneID)
            {
                //TODO: Perform transfer from output pool to the Wisp, remove the resource reservation and send the Wisp back home
            }
        }
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
