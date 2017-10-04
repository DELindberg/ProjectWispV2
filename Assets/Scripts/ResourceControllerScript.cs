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
    //TODO: REWRITE THIS AS AN ARRAY, there's no reason to have this as a list
    //public List<ResourceType> ResourceList = new List<ResourceType>();

    //ResourceType Water = new ResourceType(),
    //    Seeds = new ResourceType(),
    //    Grain = new ResourceType(),
    //    Runes = new ResourceType(),
    //    Honey = new ResourceType(),
    //    Flour = new ResourceType(),
    //    Beer = new ResourceType(),
    //    Chili = new ResourceType(),
    //    Flowers = new ResourceType(),
    //    Forest = new ResourceType(),
    //    Wildlife = new ResourceType(),
    //    Weed = new ResourceType(),
    //    Mineral = new ResourceType(),
    //    Wood = new ResourceType(),
    //    Rocks = new ResourceType(),
    //    Leather = new ResourceType(),
    //    Glass = new ResourceType(),
    //    Fuel = new ResourceType(),
    //    Bread = new ResourceType(),
    //    Meat = new ResourceType(),
    //    Antibear = new ResourceType(),
    //    Sleep = new ResourceType(),
    //    Happiness = new ResourceType();
        
        

    // Use this for initialization
    void Start()
    {
        /*
         * Because of the way the other objects in the game handle their individual initialization of their resource lists,
         * it is possible to completely reorder this list without having to worry about index-numbers. HOWEVER, the names 
         * should not be changed - if they have to be changed, make sure the change is also reflected in the code of the
         * objects which utilizes the resource in question (except for the Wisps, as they simply copy this list directly)! 
         */

        ////Attach the appropriate tags
        //Water.Name = "water";
        //Seeds.Name = "seeds";
        //Grain.Name = "grain";
        //Runes.Name = "runes";
        //Honey.Name = "honey";
        //Flour.Name = "flour";
        //Beer.Name = "beer";
        //Chili.Name = "chili";
        //Flowers.Name = "flowers";
        //Forest.Name = "forest";
        //Wildlife.Name = "wildlife";
        //Weed.Name = "weed";
        //Mineral.Name = "mineral";
        //Wood.Name = "wood";
        //Rocks.Name = "rocks";
        //Leather.Name = "leather";
        //Glass.Name = "glass";
        //Fuel.Name = "fuel";
        //Bread.Name = "bread";
        //Meat.Name = "meat";
        //Antibear.Name = "antibear";
        //Sleep.Name = "sleep";
        //Happiness.Name = "happiness";

        ////Add the resources to the ResourceList
        //ResourceList.Add(Water);
        //ResourceList.Add(Seeds);
        //ResourceList.Add(Grain);
        //ResourceList.Add(Runes);
        //ResourceList.Add(Honey);
        //ResourceList.Add(Flour);
        //ResourceList.Add(Beer);
        //ResourceList.Add(Chili);
        //ResourceList.Add(Flowers);
        //ResourceList.Add(Forest);
        //ResourceList.Add(Wildlife);
        //ResourceList.Add(Weed);
        //ResourceList.Add(Mineral);
        //ResourceList.Add(Wood);
        //ResourceList.Add(Rocks);
        //ResourceList.Add(Leather);
        //ResourceList.Add(Glass);
        //ResourceList.Add(Fuel);
        //ResourceList.Add(Bread);
        //ResourceList.Add(Meat);
        //ResourceList.Add(Antibear);
        //ResourceList.Add(Sleep);
        //ResourceList.Add(Happiness);

        ResourceList = new ResourceType[23];

        for(int i = 0; i < ResourceList.Length; i++)
        {
            ResourceList[i] = new ResourceType();
        }
    }

}

[System.Serializable]
public class ResourceType
{
    public string Name;
    public int Amount = 0;
    //Type is used by Smart Zones to indicate whether the resource is input or output
    public string Type = "undeclared";
}