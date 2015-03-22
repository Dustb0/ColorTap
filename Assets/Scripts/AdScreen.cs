using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdScreen : MonoBehaviour
{

	private bool m_showAd;
	private ShowOptions m_adOptions;

	#region " Engine Callbacks "
    
	void Awake() 
	{
		// Check if we can show an ad
		if (Advertisement.isSupported) 
		{
			Advertisement.debugLevel = Advertisement.DebugLevel.NONE;
			Advertisement.Initialize ("24971", false);

			// Setup show options
			m_adOptions = new ShowOptions();
			m_adOptions.pause = true;
			m_adOptions.resultCallback += CloseAd;
		} 
		else 
		{
			// Ads not supported, delete component
			Destroy(this);
		}
    }

	void Update()
	{
		if (m_showAd && Advertisement.isReady("pictureZone")) 
		{
			Advertisement.Show("pictureZone", m_adOptions);
			m_showAd = false;
		}
	}

	void CloseAd(ShowResult result)
	{
		// Check if user has finished the add...
	}

	#endregion

	#region " Public Access "

	public void ShowAd()
	{
		// Set flag to show ads
		m_showAd = true;
	}

	#endregion

//    void OnGUI()
//    {
//        if (GUI.Button(new Rect(10, 10, 150, 50), Advertisement.isReady("pictureZone") ? "Show Ad" : "Waiting..."))
//        {
//            // Show with default zone, pause engine and print result to debug log
//            Advertisement.Show("pictureZone", new ShowOptions
//            {
//                pause = true,
//                resultCallback = result =>
//                {
//                    Debug.Log(result.ToString());
//                }
//            });
//        }
//    }
}