using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is used as the base class for all of the game's Smart Zones, containing the common
/// elements needed for all of the Smart Zones to work. Additional functionality is created in the
/// subsequently derived classes, one for each type of Smart Zone
/// </summary>
public class SmartZoneParentScript : MonoBehaviour {

    //For identification:
    public string ZoneID, ZoneType;

    //Used to set the upper limit for number of acquirable Wisps/actors in the Smart Zone:
    public int WispLimit, AnimateCount;
    public List<WispScript> AssignedWisps = new List<WispScript>();

    //For injecting animations:
    protected Animator SmartZoneAnimator;

    //Used to communicate with the Smart Zone Controller:
    protected GameObject SmartZoneController;
    protected SmartZoneControllerScript SmartZoneControllerRef;

    //Used for communicating with the Resource Controller
    protected GameObject ResourceController;
    protected ResourceControllerScript ResourceControllerRef;

    //An array of the local resources, if any are defined
    public ResourceType[] LocalResources;

    //CALL THIS AT THE TOP OF AWAKE ON INHERITED CLASSES!!!!
    protected void PreBuildInAwake () {
        /*
            The reason we're calling this in Awake on inherited classes, is to extend
            the functionality of Awake before runtime, to make sure important elements
            are initialized before the game starts. The reason we're not simply extending
            Awake with an override in inherited classes, is because of an access conflict
            arising in Unity which I am uncertain how to resolve - the forums, they do nothing!
         */
        

        //Connect to the Smart Zone Controller:
        SmartZoneController = GameObject.Find("SmartZoneController"); //:REMEMBER TO ENSURE THAT THIS NAME IS CORRECT IN THE EDITOR
        SmartZoneControllerRef = SmartZoneController.GetComponent<SmartZoneControllerScript>();

        //Retrieves a name and adds this instance of the Smart Zone to the list in the Smart Zone Controller
        ZoneID = SmartZoneControllerRef.GetName();
        SmartZoneControllerRef.ListOfSmartZones.Add(this);

        ResourceController = GameObject.Find("ResourceController");
        ResourceControllerRef = ResourceController.GetComponent<ResourceControllerScript>();

        SmartZoneAnimator = this.GetComponent<Animator>();

        //Initialize number of currently animating Wisps
        AnimateCount = 0;
	}

    //Return the first available AnimationID - MIGHT WANT TO RUN AN ISOLATED TEST ON THIS SYSTEM
    protected int ReturnFirstAvailable(int WispLimit, List<WispScript> AssignedWisps)
    {
        int ToReturn = 0;
        bool WasFound;
        //For each Animation ID number up until the maximum number of assignable Wisps
        for (int AnimationID = 1; AnimationID <= WispLimit; AnimationID++)
        {
            WasFound = false;
            //Run through the list to see if the ID is
            for (int i = 0; i < AssignedWisps.Count; i++)
            {
                //If the animation ID matches one of our assigned Wisps and said Wisp is present (which means it's animating)
                if (AssignedWisps[i].AnimationID == AnimationID && AssignedWisps[i].IsPresent)
                {
                    WasFound = true;
                }
            }
            if(!WasFound)
            {
                ToReturn = AnimationID;
                break;  //Ends the for-loop running through AnimationID values
            }
        }

        //If this returns zero, no spaces were available (it shouldn't return zero)
        return ToReturn;
    }

    protected int PresentWispAmount()
    {
        int ToReturn = 0;

        for (int i = 0; i < AssignedWisps.Count; i++)
        {
            if (AssignedWisps[i].IsPresent == true)
            {
                ToReturn++;
            }

        }

        return ToReturn;
    }

    //Set this object as a parent to the Wisp
    protected void SetParent(GameObject ToParent, WispScript WispScriptRef)
    {
        WispScriptRef.SetParent(ToParent);
    }

    //Assigns given Wisp to list of assigned Wisps
    protected void AssignWisp(WispScript WispScriptRef)
    {
        AssignedWisps.Add(WispScriptRef);
        Debug.Log("Wisp assigned");
    }

    //Remove the given wisp from the list of assigned Wisps
    protected void RemoveAssignedWisp(WispScript WispScriptRef)
    {
        for(int i = 0; i < AssignedWisps.Count; i++)
        {
            if(WispScriptRef.WispName == AssignedWisps[i].WispName)
            {
                AssignedWisps.RemoveAt(i);
                break;
            }
        }
    }

    //Return true if there's still room for more Wisps
    protected bool CheckIfRoom()
    {
        bool IfRoom = false;

        if(AssignedWisps.Count < WispLimit)
        {
            IfRoom = true;
        }

        return IfRoom;
    }

    protected void PopulateResourceList(ResourceType[] LocalResources)
    {
        for (int i = 0; i < LocalResources.Length; i++)
        {
            LocalResources[i] = new ResourceType();
        }
    }
}
