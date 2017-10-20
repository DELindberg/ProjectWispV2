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
    
    protected void PopulateResourceList(ResourceType[] LocalResources)
    {
        for (int i = 0; i < LocalResources.Length; i++)
        {
            LocalResources[i] = new ResourceType();
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
}
