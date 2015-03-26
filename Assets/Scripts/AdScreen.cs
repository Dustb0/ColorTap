using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class AdScreen : MonoBehaviour
{
	private bool m_showAd;
	private ShowOptions m_adOptions;
	private float m_secsSinceLastConnection;
	public Text DebugText;

	#region " Engine Callbacks "
    
	void Awake() 
	{
		// Check if we can show an ad
		if (Advertisement.isSupported) ConnectToAdSys (); 
		else Destroy(this); 
    }

	private void ConnectToAdSys()
	{
		//Advertisement.debugLevel = Advertisement.DebugLevel.NONE;
		Advertisement.Initialize ("24971", false);
		
		// Setup show options
		m_adOptions = new ShowOptions();
		m_adOptions.pause = true;
		m_adOptions.resultCallback += CloseAd;
	}

	private void Reconnect()
	{
		// Try to reload connection
		m_secsSinceLastConnection += Time.deltaTime;
		
		if(m_secsSinceLastConnection >= 30)
		{
			m_secsSinceLastConnection = 0;
			ConnectToAdSys();
		}
	}

	void Update()
	{
		DebugText.text = Advertisement.isReady().ToString();

		if (Advertisement.isReady ()) 
		{
			if (m_showAd) 
			{
				Advertisement.Show (null, m_adOptions);
				m_showAd = false;
			}
		}
		else Reconnect();
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