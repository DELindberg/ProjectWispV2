using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispControllerScript : MonoBehaviour
{
    //List of all of the Wisps in the game
    public List<WispScript> ListOfWisps = new List<WispScript>();
    
    //NOT SURE IF WE NEED THIS CONNECTION, MORE MANAGEABLE TO KEEP IT DIRECTLY ON THE WISPS TO AVOID CHAIN-DEPENDENCIES
    //Used to communicate with the Smart Zone Controller:
    //GameObject SmartZoneController;
    //public SmartZoneControllerScript SmartZoneControllerRef;

    WispScript WispScriptRef;
    int WispCounter;
    // Use this for initialization
    void Start()
    {
        ////Connect to the Smart Zone Controller:
        //SmartZoneController = GameObject.Find("SmartZoneController"); //:REMEMBER TO ENSURE THAT THIS NAME IS CORRECT IN THE EDITOR
        //SmartZoneControllerRef = SmartZoneController.GetComponent<SmartZoneControllerScript>();

        WispCounter = 0;
        InvokeRepeating("WispQuery", 5f, 5f);
    }

    public void Announce()
    {
        Debug.Log("Item was added to the list");
    }

    //Returns a unique Wisp name
    public string GetName()
    {
        string WispName = "Wisp" + WispCounter;
        WispCounter++;

        return WispName;
    }

    //Removes the Wisp with the requested ID from the list of Wisps
    public void WispDestroyer(string WispName)
    {
        for (int i = 0; i < ListOfWisps.Count; i++)
        {
            //If the name in the current index matches the name passed to it        
            if (ListOfWisps[i].GetComponent<WispScript>().WispName == WispName)
            {
                //Remove element at the found index
                ListOfWisps.RemoveAt(i);
                break;  //Stop the for-loop
            }
            else
            {
                //WE SHOULD NEVER ARRIVE HERE
                Debug.Log("Wisp removal failed - ID not found in the list!");
            }
        }
    }

    //Run through the list of Wisps, randomly query those above the threshold - the further above, the bigger the chance
    void WispQuery()
    {
        //Set this variable to configure the earliest moment a Wisp can be queried since last query (or birth)
        int TimeThreshold = 5;

        for (int i = 0; i < ListOfWisps.Count; i++)
        {
            if (RandomDouble(ListOfWisps[i].TimeSinceQuery) > TimeThreshold)
            {
                ListOfWisps[i].UpdateBehavior();
            }
        }
    }

    //Get random number between zero and the number of seconds since last time a Wisp was queried
    double RandomDouble(double max)
    {
        //Seed randomizer from time
        int seed = (int)System.DateTime.Now.Ticks;
        System.Random r = new System.Random(seed);

        return (r.NextDouble() * max);
    }

    WispScript ReturnWispRefByName(string WispName)
    {
        //Initialized to avoid syntax error
        WispScript ToReturn = null;

        for(int i = 0; i < ListOfWisps.Count; i++)
        {
            if(ListOfWisps[i].WispName == WispName)
            {
                ToReturn = ListOfWisps[i];
                break;
            }
        }
        return ToReturn;
    }
}
