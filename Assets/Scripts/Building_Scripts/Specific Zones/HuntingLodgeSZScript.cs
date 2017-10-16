using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingLodgeSZScript : BuildingParentScript
{
    //Add more of these if you ever add more input or output resources to the building
    int InputResourceID;
    int OutputResourceID, OutputResourceID2;

    //Initialization
    private void Awake()
    {
        PreBuildInAwake();
        ZoneType = "huntinglodge";

        //Create and define the resources for the Hunting Lodge building
        LocalResources = new ResourceType[2];
        PopulateResourceList(LocalResources);
        InitializeResources();

        FindResourceID(out InputResourceID, out OutputResourceID, out OutputResourceID2);
        WispLimit = 2;


    }

    //WATCH OUT FOR TYPOS IN HERE
    void InitializeResources()
    {
        LocalResources[0].Name = "wildlife";
        LocalResources[0].Type = "input";

        LocalResources[1].Name = "meat";
        LocalResources[1].Type = "output";

    }
    // Update is called once per frame
    void Update()
    {

    }

    //Returns the index numbers equivalent to the resource IDs, modify this function if you ever add more input or output resources to the building
    void FindResourceID(out int InputResourceID, out int OutputResourceID, out int OutputResourceID2)
    {
        //Defined to avoid errors in handling out-parameters
        InputResourceID = 0;
        OutputResourceID = 0;
        OutputResourceID2 = 0;

        //Find the index numbers for the input and output resources
        for (int i = 0; i < ResourceControllerRef.ResourceList.Length; i++)
        {
            if (LocalResources[0].Name == ResourceControllerRef.ResourceList[i].Name)
            {
                InputResourceID = i;
            }
            else if (LocalResources[1].Name == ResourceControllerRef.ResourceList[i].Name)
            {
                OutputResourceID = i;
            }
            else if (LocalResources[2].Name == ResourceControllerRef.ResourceList[i].Name)
            {
                OutputResourceID2 = i;
            }
        }
    }
}
