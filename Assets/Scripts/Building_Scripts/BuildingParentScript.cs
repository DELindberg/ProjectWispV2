using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingParentScript : SmartZoneParentScript {
    /*
     * WHEN DESIGNING ADDITIONAL SMART ZONES LATER:
     * If any of the functionality found in here might be used in other Smart Zones (eg. pollinary with the bees, perhaps),
     * simply cut the code from this class and paste it into the SmartZoneParentScript to make it more widely available
     */

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
            if (!WasFound)
            {
                ToReturn = AnimationID;
                break;  //Ends the for-loop running through AnimationID values
            }
        }

        //If this returns zero, no spaces were available (it shouldn't return zero)
        return ToReturn;
    }

    //Cycle through the list of assigned Wisps, return reference to the first one available
    protected WispScript ReturnAssignedAvailable(List<WispScript> AssignedWisps)
    {
        //Initialized to null to prevent errors
        WispScript ToReturn = null;

        for (int i = 0; i < AssignedWisps.Count; i++)
        {
            if (AssignedWisps[i].IsPresent && !AssignedWisps[i].IsBusy)
            {
                ToReturn = AssignedWisps[i];
                break;
            }
        }
        return ToReturn;
    }

    //Return the total number of Wisps currently present in the Smart Zone
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
        for (int i = 0; i < AssignedWisps.Count; i++)
        {
            if (WispScriptRef.WispName == AssignedWisps[i].WispName)
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

        if (AssignedWisps.Count < WispLimit)
        {
            IfRoom = true;
        }

        return IfRoom;
    }
}
