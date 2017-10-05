using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceControllerScript : MonoBehaviour
{

    //This list is not meant to be manipulated by the controller beyond its initialisation
    //It is used by the Wisps to retrieve a list of all the resource types they can carry

    /* List of current resources to keep track of array length
     * 
     *      1. Water
     *      2. Seeds
     *      3. Grain
     *      4. Runes
     *      5. Honey
     *      6. Flour
     *      7. Beer
     *      8. Chili
     *      9. Flowers
     *      10. Forest
     *      11. Wildlife
     *      12. Weed
     *      13. Mineral
     *      14. Wood
     *      15. Rocks
     *      16. Leather
     *      17. Glass
     *      18. Fuel
     *      19. Bread
     *      20. Meat
     *      21. Antibear
     *      22. Sleep
     *      23. Happiness
     * 
     */

    public ResourceType[] ResourceList;

    // Use this for initialization
    void Start()
    {
        /*
         * Because of the way the other objects in the game handle their individual initialization of their resource lists,
         * it is possible to completely reorder this list without having to worry about index-numbers. HOWEVER, the names 
         * should not be changed - if they have to be changed, make sure the change is also reflected in the code of the
         * objects which utilizes the resource in question (except for the Wisps, as they simply copy this list directly)! 
         */


        ResourceList = new ResourceType[23];

        for (int i = 0; i < ResourceList.Length; i++)
        {
            ResourceList[i] = new ResourceType();
        }

        ResourceList[0].Name = "water";
        ResourceList[1].Name = "seeds";
        ResourceList[2].Name = "grain";
        ResourceList[3].Name = "runes";
        ResourceList[4].Name = "honey";
        ResourceList[5].Name = "flour";
        ResourceList[6].Name = "beer";
        ResourceList[7].Name = "chili";
        ResourceList[8].Name = "flowers";
        ResourceList[9].Name = "wildlife";
        ResourceList[10].Name = "weed";
        ResourceList[11].Name = "mineral";
        ResourceList[12].Name = "wood";
        ResourceList[13].Name = "rocks";
        ResourceList[14].Name = "leather";
        ResourceList[15].Name = "glass";
        ResourceList[16].Name = "fuel";
        ResourceList[17].Name = "bread";
        ResourceList[18].Name = "meat";
        ResourceList[19].Name = "antibear";
        ResourceList[20].Name = "sleep";
        ResourceList[21].Name = "happiness";
        ResourceList[22].Name = "forest";


    }

}

//This class must be serializable because of the way we list it
[System.Serializable]
public class ResourceType
{
    public string Name;
    public int Amount = 0;
    //Type is used by Smart Zones to indicate whether the resource is input or output
    public string Type = "undeclared"; //Is later set as Input or Output
}