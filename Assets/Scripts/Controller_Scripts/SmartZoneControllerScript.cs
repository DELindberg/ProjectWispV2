using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the controller component to aid in the tracking and referencing to the Smart Zones
/// </summary>

public class SmartZoneControllerScript : MonoBehaviour {

    public List<SmartZoneParentScript> ListOfSmartZones = new List<SmartZoneParentScript>();

    SmartZoneParentScript SmartZoneParentReference;
    int SmartZoneCounter;   //Used for naming

    //Returns a unique Smart Zone name
    public string GetName()
    {
        string SmartZoneName = "SZone " + SmartZoneCounter;
        SmartZoneCounter++;

        return SmartZoneName;
    }

    public void SmartZoneDestroyer(string SmartZoneID)
    {
        for(int i = 0; i < ListOfSmartZones.Count; i++)
        {
            //If the name in the current index matches the name passed to it
            if (ListOfSmartZones[i].GetComponent<SmartZoneParentScript>().ZoneID == SmartZoneID)
            {
                //Remove the element at the found index
                ListOfSmartZones.RemoveAt(i);
                break;  //Stop the for-loop
            }
            else
            {
                Debug.Log("ERROR!: The deletion of a Smart Zone from the Smart Zone Controller list has failed!");
            }
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
