using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class AdSpace : MonoBehaviour 
{
	void Start () 
    {
        // Initialize Ad-system
        Advertisement.Initialize("24971", true);

	    // Show that ad!
        if (Advertisement.isReady())
        {
            Debug.Log("Hallo");
            Advertisement.Show();
        }
	}

}
