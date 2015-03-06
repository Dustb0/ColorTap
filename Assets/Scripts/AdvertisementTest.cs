using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdvertisementTest : MonoBehaviour
{
    void Awake() {
        if (Advertisement.isSupported) {
            Advertisement.allowPrecache = true;
            Advertisement.Initialize("24971", true);
        } else {
            Debug.Log("Platform not supported");
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 50), Advertisement.isReady("pictureZone") ? "Show Ad" : "Waiting..."))
        {
            // Show with default zone, pause engine and print result to debug log
            Advertisement.Show("pictureZone", new ShowOptions
            {
                pause = true,
                resultCallback = result =>
                {
                    Debug.Log(result.ToString());
                }
            });
        }
    }
}