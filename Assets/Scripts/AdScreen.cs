using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class AdScreen : MonoBehaviour
{
	private bool m_showAd;
	private ShowOptions m_adOptions;
	private float m_secsSinceLastConnection;

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
}