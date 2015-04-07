using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Soomla.Profile;

public class SocialShare : MonoBehaviour {

    public string ShareText;
    public Text ShareLabel;
    public Text URLLabel;
    public CanvasGroup Group;

    private const string SHARE_URL = "https://itunes.apple.com/us/app/coltap/id979054662";

	public void Start()
	{
		// Initialize soomla
		SoomlaProfile.Initialize ();
	}

    public void InitMenu()
    {
        // Set texts
        ShareLabel.text = ShareText;
        URLLabel.text = SHARE_URL;

        Group.alpha = 1;   
		Group.blocksRaycasts = true;
		Group.interactable = true;
    }

    public void HideMenu()
    {
        Group.alpha = 0;
		Group.blocksRaycasts = false;
		Group.interactable = false;
    }

    public void ShareTwitter()
    {
		StartCoroutine ("PostTwitter");
		HideMenu ();
    }

	private IEnumerator PostTwitter()
	{
		SoomlaProfile.Login (Provider.TWITTER);

		yield return new WaitForSeconds(1);

		SoomlaProfile.UpdateStatus(Provider.TWITTER, ShareText + " " + SHARE_URL);
	}

    public void ShareFB()
    {

    }

}
