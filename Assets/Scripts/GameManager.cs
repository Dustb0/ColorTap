using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    #region " Resources "

    // Button colors
    public Color TapRed;
    public Color TapBlue;
    public Color TapGreen;
    public Color TapMagenta;
    public Color TapYellow;
    public Color TapCyan;

    #endregion

    #region " Fields "

    private enum GamePhase
    {
        Menu,
        Tutorial,
        SelectColor,
        Game,
        Highscore
    }

    public RectTransform ProgressRect;
    public RectTransform FullRect;
    public List<ColorTapButton> SelectButtons;
    public Animator TapBoardAnimator;

    // Menu Controls
    public Text TitleText;
    public InputField EnterScoreInput;
    public Text AchievedPointsText;
    public Button SubmitScoreButton;
    public Image ProgressbarBG;
    public GameObject GameMenuPanel;
    public Text HighscoreListeText;

    private const float TOTAL_TIME = 30;

    private float m_progressFullWidth;
    private GamePhase m_currentPhase;
    private float m_remainingTime;
    private int m_levelIndex;
    private TapColor m_selectedColor;
    private Image m_progressImageBar;
    private Image m_progressImageBG;

    // Highscore data
    private int m_pointCount;
    private int m_comboCount;
    
    #endregion

    #region " Engine Callbacks "

    void Start()
    {
        // Save progressrect
        m_progressFullWidth = FullRect.rect.width;

        PrepareMainMenu();

        m_currentPhase = GamePhase.SelectColor;
        m_levelIndex = 1;
        PrepareColorSelection();
    }

    private void PrepareMainMenu()
    {
        // Hide all game buttons
        foreach (ColorTapButton btn in ButtonPool) btn.enabled = false;

        // Hide progressbar
        m_progressImageBar = ProgressRect.GetComponent<Image>();
        m_progressImageBG = FullRect.GetComponent<Image>();
        m_progressImageBar.enabled = false;
        m_progressImageBG.enabled = false;
        ProgressbarBG.enabled = false;

        // Hide highscore stuff
        TitleText.enabled = false;
        AchievedPointsText.enabled = false;
        SubmitScoreButton.gameObject.SetActive(false);
        EnterScoreInput.gameObject.SetActive(false);
        GameMenuPanel.SetActive(false);
        HighscoreListeText.enabled = false;
    }

    void Update()
    {
        switch(m_currentPhase)
        {
            case GamePhase.SelectColor:
                
                break;

            case GamePhase.Game:
                // Set time
                m_remainingTime -= Time.deltaTime;

                // Set width
                ProgressRect.sizeDelta = new Vector2((m_remainingTime / TOTAL_TIME) * m_progressFullWidth, ProgressRect.sizeDelta.y);

                // Check if time's over
                if(m_remainingTime <= 0)
                {
                    // Add current combopoint amount
                    m_pointCount += m_comboCount;

                    // Switch to highscore-screen,
                    PrepareHighscore();
                }

                break;
        }
    }

    #endregion

    #region " Color Selection "

    private void PrepareColorSelection()
    {
        switch (m_levelIndex)
        {
            case 1:
                SelectButtons[0].TColor = TapColor.Red;
                SelectButtons[1].TColor = TapColor.Green;
                SelectButtons[2].TColor = TapColor.Blue;
                SetupButtonColor(SelectButtons[0]);
                SetupButtonColor(SelectButtons[1]);
                SetupButtonColor(SelectButtons[2]);
                break;
        }
    }

    private void PrepareHighscore()
    {
        m_currentPhase = GamePhase.Highscore;

        // Hide progressbar & menu
        m_progressImageBar.enabled = false;
        m_progressImageBG.enabled = false;
        ProgressbarBG.enabled = false;
        GameMenuPanel.SetActive(false);

        // Show title text
        TitleText.enabled = true;
        TitleText.text = "Highscore";
        AchievedPointsText.enabled = true;

        // Only allow to submit highscore if it was better 
        if(Highscore.AddNewScore(m_pointCount, EnterScoreInput.text))
        {
            SubmitScoreButton.gameObject.SetActive(true);
            EnterScoreInput.gameObject.SetActive(true);
            AchievedPointsText.text = (Highscore.AchievedHighscorePlace + 1) + "th Place ~ Achieved " + m_pointCount + " points!";
        }
        else
        {
            AchievedPointsText.text = "Achieved " + m_pointCount + " points!";    
        }

        // Display current Highscore positions
        HighscoreListeText.enabled = true;

        // Load highscore
        Highscore list = Highscore.Get();
        string scoreList = ""; 

        // Display entries
        for (int i = 0; i < Highscore.MAX_COUNT; ++i)
        {
            // Check how many entries there are
            if (list.Count > i)
            {
                scoreList += (i + 1) + "th " + list[i].PlayerName + " - " + list[i].ScorePoints + " Points \n";
            }
            else
            {
                // Empty entry
                scoreList += (i + 1) + ". --- \n";
            }
        }

        // Set text
        HighscoreListeText.text = scoreList;

        // Hide game screen
        foreach (ColorTapButton b in ButtonPool) b.animator.SetTrigger("HideButton");
    }

    public void SelectColor()
    {
        // Determine clicked button
        ColorTapButton btn = EventSystem.current.currentSelectedGameObject.GetComponent<ColorTapButton>();
        m_selectedColor = btn.TColor;

        foreach (ColorTapButton b in SelectButtons) b.animator.SetTrigger("HideButton");

        // Set game phase
        m_currentPhase = GamePhase.Game;

        // Init combocount
        m_comboCount = 0;

        // Set time & Show timebar
        m_remainingTime = TOTAL_TIME;
        m_progressImageBar.enabled = true;
        m_progressImageBG.enabled = true;
        ProgressbarBG.enabled = true;
        GameMenuPanel.SetActive(true);

        SetupNewScreen();
    }

    #endregion

    #region " Tap Button Managing "

    public List<ColorTapButton> ButtonPool;

    private void SetupButtonColor(ColorTapButton button)
    {
        // Set color
        switch (button.TColor)
        {
            case TapColor.Red:
                button.image.color = TapRed;
                break;

            case TapColor.Blue:
                button.image.color = TapBlue;
                break;

            case TapColor.Green:
                button.image.color = TapGreen;
                break;

            case TapColor.Magenta:
                button.image.color = TapMagenta;
                break;

            case TapColor.Yellow:
                button.image.color = TapYellow;
                break;

            case TapColor.Cyan:
                button.image.color = TapCyan;
                break;
        }

    }

    public void ColorTapClick()
    {
        // Determine clicked button
        ColorTapButton btn = EventSystem.current.currentSelectedGameObject.GetComponent<ColorTapButton>();   

        // Check if it's selected color
        switch(m_levelIndex)
        {
            case 1:
                // Simple 1/1 color 
                if(btn.TColor == m_selectedColor)
                {
                    // Made it!
                    m_pointCount += 1;

                    // Increase combocount 
                    m_comboCount += 1;

                    SetupNewScreen();
                }
                else
                {
                    // Shake screen!
                    TapBoardAnimator.SetTrigger("Wrong");

                    m_remainingTime -= 5.0f;

                    // Add & Reset combo count
                    m_pointCount += m_comboCount;
                    m_comboCount = 0;
                }
                break;
        }
    }

    #endregion

    #region " Screen Managing "

    private void SetupNewScreen()
    {
        // Collect possible colors
        List<TapColor> possibleColors = new List<TapColor>(ButtonPool.Count);
        possibleColors.Add(TapColor.Blue);
        possibleColors.Add(TapColor.Cyan);
        possibleColors.Add(TapColor.Green);
        possibleColors.Add(TapColor.Magenta);
        possibleColors.Add(TapColor.Red);
        possibleColors.Add(TapColor.Yellow);

        for (int i = 0; i < ButtonPool.Count; ++i)
        {
            // Get one random button out of the collection and display it
            int index = Random.Range(0, possibleColors.Count - 1);
            
            // Assign the color
            ButtonPool[i].enabled = true;
            ButtonPool[i].TColor = possibleColors[index];
            SetupButtonColor(ButtonPool[i]);

            // Remove the assigned color from the list of possible choices
            possibleColors.RemoveAt(index);

            // Play Entrance animation
            ButtonPool[i].animator.SetTrigger("ShowButton");
        }

    }

    #endregion

    #region " Highscore "

    public void SubmitScore()
    {
        if(EnterScoreInput.text.Trim().Length > 0)
        {
            Highscore.PreparedEntry.PlayerName = EnterScoreInput.text;
            Highscore.Save();
            m_currentPhase = GamePhase.SelectColor;
            PrepareMainMenu();
        }
    }

    #endregion

}
