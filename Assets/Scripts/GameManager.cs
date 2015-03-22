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
    public List<ColorTapButton> SelectButtons;
    public Animator TapBoardAnimator;

    // Canvas Groups
    public CanvasGroup StartMenuGroup;
    public CanvasGroup GameGroup;
    public CanvasGroup SubMenuGroup;
    public CanvasGroup HighscoreGroup;
    public CanvasGroup TutorialGroup;
    public CanvasGroup EnterScoreGroup;

    // Menu Controls
    public Text TitleText;
    public InputField EnterScoreInput;
    public Text AchievedPointsText;
    public Text HighscoreListeText;
    public Text CurrentScoreText;
    public Button BackToMenuButton;
    public Text ComboText;
	public Image ProgressImage;
    public Sprite ButtonImageNormal;
    public Sprite ButtonImageMinus;

    public Color ComboTextColorPlus;
    public Color ComboTextColorMinus;

    // Constants
    private const float TOTAL_TIME = 30;
    private const int TOTAL_TAPBUTTONS = 6;
	
    private GamePhase m_currentPhase;
    private float m_remainingTime;
    private TapColor m_selectedColor;
    private Animator m_comboAnimator;
    private List<TapColor> m_possibleColors;
    private List<int> m_availableIndices;
    private int m_roundSeq;

    // Highscore data
    private int m_pointCount;
    private int m_comboLevel;
    
    #endregion

    #region " Engine Callbacks "

	void Awake()
	{
		// Force binary writer to use a different encoding mode on iOS
		System.Environment.SetEnvironmentVariable ("MONO_REFLECTION_SERIALIZER", "yes");
	}

    void Start()
    {
        m_comboAnimator = ComboText.gameObject.GetComponent<Animator>();

        // Initialize lists needed for screen setup
        m_possibleColors = new List<TapColor>(TOTAL_TAPBUTTONS);
        m_availableIndices = new List<int>(TOTAL_TAPBUTTONS);

        // Hide everything else but the start menu
        SetupCanvasGroup(GameGroup, false);
        SetupCanvasGroup(SubMenuGroup, false);

        ShowStartMenu();

        m_currentPhase = GamePhase.SelectColor;
        PrepareColorSelection();
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

                // Set width & position to align
				ProgressImage.fillAmount = m_remainingTime / TOTAL_TIME;

                // Check if time's over & switch to highscore-screen,
                if (m_remainingTime <= 0) PrepareHighscore();

                break;
        }
    }

    #endregion

    #region " Menu Handling "

    private void SetupCanvasGroup(CanvasGroup group, bool visible)
    {
        group.alpha = visible ? 1 : 0;
        group.blocksRaycasts = visible;
        group.interactable = visible;
    }

    private void PrepareColorSelection()
    {
        SelectButtons[0].TColor = TapColor.Red;
        SelectButtons[1].TColor = TapColor.Green;
        SelectButtons[2].TColor = TapColor.Blue;
        SelectButtons[3].TColor = TapColor.Magenta;
        SelectButtons[4].TColor = TapColor.Yellow;
        SelectButtons[5].TColor = TapColor.Cyan;
        SetupButtonColor(SelectButtons[0]);
        SetupButtonColor(SelectButtons[1]);
        SetupButtonColor(SelectButtons[2]);
        SetupButtonColor(SelectButtons[3]);
        SetupButtonColor(SelectButtons[4]);
        SetupButtonColor(SelectButtons[5]);
    }

    public void ShowHighscore(bool showBackToMenuButton)
    {
        m_currentPhase = GamePhase.Highscore;

        // Hide progressbar & menu
        SetupCanvasGroup(GameGroup, false);
        CurrentScoreText.enabled = false;

        // Show title text
        SetupCanvasGroup(SubMenuGroup, true);
        SetupCanvasGroup(TutorialGroup, false);
        SetupCanvasGroup(HighscoreGroup, true);
        TitleText.text = "Highscore";

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

        BackToMenuButton.image.enabled = showBackToMenuButton;
        AchievedPointsText.enabled = !showBackToMenuButton;
        SetupCanvasGroup(EnterScoreGroup, !showBackToMenuButton);
    }

    private void PrepareHighscore()
    {
        ShowHighscore(false);
        
		// Always show achieved points
		AchievedPointsText.enabled = true;

        // Only allow to submit highscore if it was better 
		if(m_pointCount > 0 && Highscore.AddNewScore(m_pointCount, EnterScoreInput.text))
        {
            BackToMenuButton.image.enabled = false;
            SetupCanvasGroup(EnterScoreGroup, true);
            EnterScoreInput.text = "";
            AchievedPointsText.text = (Highscore.AchievedHighscorePlace + 1) + "th Place" + System.Environment.NewLine +  "Achieved " + m_pointCount + " points!";
        }
        else
        {
            SetupCanvasGroup(EnterScoreGroup, false);
            AchievedPointsText.text = "Achieved " + m_pointCount + " points!";
            BackToMenuButton.image.enabled = true;
        }

        // Hide game screen
        foreach (ColorTapButton b in ButtonPool) b.animator.SetTrigger("HideButton");
    }

    public void SelectColor()
    {
        // Determine clicked button
        ColorTapButton btn = EventSystem.current.currentSelectedGameObject.GetComponent<ColorTapButton>();
        m_selectedColor = btn.TColor;

        HideStartMenu();

        // Set game phase
        m_currentPhase = GamePhase.Game;
        m_comboLevel = 0;

        // Set time & Show timebar
        m_remainingTime = TOTAL_TIME;
        SetupCanvasGroup(GameGroup, true);
        CurrentScoreText.enabled = true;
        CurrentScoreText.text = "";
        ComboText.enabled = true;
        ComboText.text = "";

        // Init points & other game values
        m_pointCount = 0;
        m_comboLevel = 0;
        m_roundSeq = 0;

        SetupNewScreen();
    }

    public void HideStartMenu()
    {
        // Hide main menu elements
        foreach (ColorTapButton b in SelectButtons) b.animator.SetTrigger("HideButton");
        SetupCanvasGroup(StartMenuGroup, false);
    }

    public void ShowStartMenu()
    {
        SetupCanvasGroup(StartMenuGroup, true);

        // Show menu
        foreach (ColorTapButton b in SelectButtons) b.animator.SetTrigger("ShowButton");
    }

    public void ShowTutorial()
    {
        // Show title text
        SetupCanvasGroup(SubMenuGroup, true);
        SetupCanvasGroup(HighscoreGroup, false);
        SetupCanvasGroup(TutorialGroup, true);
        TitleText.text = "How to Play";

        BackToMenuButton.image.enabled = true;
    }

    public void HideSubMenu()
    {
        SetupCanvasGroup(HighscoreGroup, false);
        SetupCanvasGroup(TutorialGroup, false);
        SetupCanvasGroup(SubMenuGroup, false);
    }
    public void SubmitScore()
    {
        if(EnterScoreInput.text.Trim().Length > 0)
        {
            Highscore.PreparedEntry.PlayerName = EnterScoreInput.text;
            Highscore.Save();
            m_currentPhase = GamePhase.SelectColor;

            HideSubMenu();
            ShowStartMenu();
        }
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

        // Check if color matches
        if (btn.TColor == m_selectedColor)
        {
            // Check if it's a minus button!
            if(btn.IsMinusButton)
            {
                // Oh shoot :S
                m_pointCount -= 5;

                // Make sure points don't go below 0
                if (m_pointCount < 0) m_pointCount = 0;

                // Show text
                ComboText.color = ComboTextColorMinus;
                ComboText.text = "-5";
                m_comboAnimator.SetTrigger("ShowCombo");

                // Update score
                CurrentScoreText.text = (m_pointCount).ToString();
            }
            else
            {
                // Made it!
                m_pointCount += 1;
                if (m_comboLevel < 5) m_comboLevel += 1;

                // If combo, add another point
                if (m_comboLevel > 1)
                {
                    m_pointCount += (m_comboLevel - 1);

                    // Show text
                    ComboText.color = ComboTextColorPlus;
                    ComboText.text = "+" + m_comboLevel;
                    m_comboAnimator.SetTrigger("ShowCombo");
                }

                // Update score
                CurrentScoreText.text = (m_pointCount).ToString();

                SetupNewScreen();
            }
        }
        else
        {
            WrongButtonFeedback();
        }
    }

    private void WrongButtonFeedback()
    {
        // Shake screen!
        TapBoardAnimator.SetTrigger("Wrong");
        m_comboLevel = 0;
        m_remainingTime -= 5.0f;
    }

    #endregion

    #region " Screen Managing "

    private void SetupNewScreen()
    {
        // Collect possible colors
        m_possibleColors.Clear();
        m_possibleColors.Add(TapColor.Blue);
        m_possibleColors.Add(TapColor.Cyan);
        m_possibleColors.Add(TapColor.Green);
        m_possibleColors.Add(TapColor.Magenta);
        m_possibleColors.Add(TapColor.Red);
        m_possibleColors.Add(TapColor.Yellow);

        // Advance roundnumber and determine what screen type to show
        m_roundSeq += 1;
        switch(m_roundSeq)
        {
            case 1:
                NormalScreen();
                break;

            case 2:
                ScoreModifierScreen(true);
                m_roundSeq = 0; // Reset
                break;
        }

    }

    private void NormalScreen()
    {
        for (int i = 0; i < TOTAL_TAPBUTTONS; ++i)
        {
            // Get one random button out of the collection and display it
            int index = Random.Range(0, m_possibleColors.Count - 1);

            // Assign the color
            ButtonPool[i].Init();
            ButtonPool[i].image.sprite = ButtonImageNormal;
            ButtonPool[i].TColor = m_possibleColors[index];
            SetupButtonColor(ButtonPool[i]);

            // Remove the assigned color from the list of possible choices
            m_possibleColors.RemoveAt(index);

            // Play Entrance animation
            ButtonPool[i].animator.SetTrigger("FlipButton");
        }
    }

    private void SetupButton(int index, TapColor tcolor, bool modifierMinus)
    {
        ButtonPool[index].Init();

        // Check for special modifier 
        if(modifierMinus)
        {
            ButtonPool[index].IsMinusButton = true;
            ButtonPool[index].image.sprite = ButtonImageMinus;
        }
        else
        {
            ButtonPool[index].image.sprite = ButtonImageNormal;
        }
        ButtonPool[index].TColor = tcolor;
        SetupButtonColor(ButtonPool[index]);
    }

    private void ScoreModifierScreen(bool minusButton)
    {
        // Create an array with all the remaining indices 
        for (short i = 0; i < TOTAL_TAPBUTTONS; ++i) m_availableIndices.Add(i);

        // Get an index for the modifier button
        int modifierIndex = m_availableIndices[Random.Range(0, m_availableIndices.Count - 1)];
        m_availableIndices.Remove(modifierIndex);

        // Setup the modifier button
        SetupButton(modifierIndex, m_selectedColor, true);

        // Get an index for the normal button
        int normalIndex = m_availableIndices[Random.Range(0, m_availableIndices.Count - 1)];
        m_availableIndices.Remove(normalIndex);

        // Set up the obligatory, selected "normal" color button
        SetupButton(normalIndex, m_selectedColor, false);
        m_possibleColors.Remove(m_selectedColor);

        // Clear pool of available indices
        m_availableIndices.Clear();

        // Move through all the other buttons and regularly assign their colors
        for (int i = 0; i < ButtonPool.Count; ++i)
        {
            // Only setup button if it wasn't already setup 
            if(i != modifierIndex && i != normalIndex)
            { 
                // Get one random button out of the collection and display it
                int index = Random.Range(0, m_possibleColors.Count - 1);

                // Assign the color etc.
                SetupButton(i, m_possibleColors[index], false);

                // Remove the assigned color from the list of possible choices
                m_possibleColors.RemoveAt(index);
            }

            // Alawys play the entrance animation
            ButtonPool[i].animator.SetTrigger("FlipButton");
        }


    }

    #endregion

}
