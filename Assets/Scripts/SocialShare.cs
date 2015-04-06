using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SocialShare : MonoBehaviour {

    public string ShareText;
    public Text ShareLabel;
    public Text URLLabel;
    public CanvasGroup Group;

    private const string SHARE_URL = "https://itunes.apple.com/us/app/coltap/id979054662";

    public void InitMenu()
    {
        // Set texts
        ShareLabel.text = ShareText;
        URLLabel.text = SHARE_URL;

        Group.alpha = 1;   
    }

    public void HideMenu()
    {
        Group.alpha = 0;
    }

    public void ShareTwitter()
    {

    }

    public void ShareFB()
    {

    }

}
