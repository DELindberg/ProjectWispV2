using UnityEngine;
using System.Collections;

public class DayNightScript : MonoBehaviour
{
    public Vector3 rotateVal;

    //The light variable used to hold the reference to the light component of the object
    private Light sun;

    Color DaytimeTarget = new Color(255f, 255f, 255f, 255f);
    Color NighttimeTarget = new Color(255f, 20f, 0f, 255f);

    bool IsDay = true;

    // Use this for initialization
    void Start()
    {
        //Define rotation in degrees per second
        rotateVal = new Vector3 (3, 0, 0); //3 degrees per second = one full rotation every two minutes

        //Get the reference to the light component of the object the script is attached to
        sun = gameObject.GetComponent<Light>();
        sun.colorTemperature = 1800;

    }


    // Update is called once per frame
    void Update()
    {

        //Apply the vector3 rotateVal onto the Rotate transform of the sun component
        sun.transform.Rotate(rotateVal * Time.deltaTime);

        //if (false) ;

    }

    void MakeDay()
    {
        if(DaytimeTarget.g < 255f)
        {
            DaytimeTarget.g += (1 * Time.deltaTime);
        }

        if (DaytimeTarget.b < 255f)
        {
            DaytimeTarget.b += (1 * Time.deltaTime);
        }

    }

    void MakeNight()
    {
        if (DaytimeTarget.g > 20f)
        {
            DaytimeTarget.g -= (1 * Time.deltaTime);
        }

        if (DaytimeTarget.b > 0f)
        {
            DaytimeTarget.b -= (1 * Time.deltaTime);
        }
    }
}
