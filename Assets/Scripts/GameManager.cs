using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    #region " Resources "

    // Buttonsprites
    public Sprite TapFullSprite;
    public Sprite TapHalfSprite;

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
        Game
    }

    public RectTransform ProgressRect;
    public RectTransform FullRect;
    public List<ColorTapButton> SelectButtons;

    private const float TOTAL_TIME = 60;
    private const int TOTAL_BUTTONS = 15;

    private float m_progressFullWidth;
    private GamePhase m_currentPhase;
    private float m_remainingTime;
    private int m_screenIndex;
    private TapColor m_selectedColor;
    private Image m_progressImageBar;
    private Image m_progressImageBG;
    
    #endregion

    #region " Engine Callbacks "

    void Start()
    {
        // Save progressrect
        m_progressFullWidth = FullRect.rect.width;

        // Hide all game buttons
        foreach(ColorTapButton btn in ButtonPool)
        {
            btn.enabled = false;
        }

        // Hide progressbar
        m_progressImageBar = ProgressRect.GetComponent<Image>();
        m_progressImageBG = FullRect.GetComponent<Image>();
        m_progressImageBar.enabled = false;
        m_progressImageBG.enabled = false;

        m_currentPhase = GamePhase.SelectColor;
        PrepareColorSelection(1);
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
                break;
        }
    }

    #endregion

    #region " Color Selection "

    public void PrepareColorSelection(int level)
    { 
        switch(level)
        {
            case 1:
                SelectButtons[0].TColor = TapColor.Red;
                SelectButtons[1].TColor = TapColor.Green;
                SelectButtons[2].TColor = TapColor.Blue;
                SetupButton(SelectButtons[0]);
                SetupButton(SelectButtons[1]);
                SetupButton(SelectButtons[2]);
                break;
        }
    }

    public void SelectColor()
    {
        // Determine clicked button
        ColorTapButton btn = EventSystem.current.currentSelectedGameObject.GetComponent<ColorTapButton>();
        
        foreach(ColorTapButton b in SelectButtons)
        {
            b.animator.SetTrigger("SelectedButton");
        }

        // Set game phase
        m_currentPhase = GamePhase.Game;
        m_screenIndex = 0;

        // Set time & Show timebar
        m_remainingTime = TOTAL_TIME;
        m_progressImageBar.enabled = true;
        m_progressImageBG.enabled = true;

        SetupNewScreen();
    }

    #endregion

    #region " Tap Button Managing "

    public List<ColorTapButton> ButtonPool;

    private void SetupButton(ColorTapButton button)
    {
        // Setup sprite
        button.image.sprite = button.TType == TapType.Full ? TapFullSprite : TapHalfSprite;

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
    }

    #endregion

    #region " Screen Managing "

    private void SetupNewScreen()
    {
        // Set amount of current visible tap buttons to press
        int visibleButtons = Mathf.Max(4 + m_screenIndex);

        // Collect possible button spots
        List<ColorTapButton> possibleButtons = new List<ColorTapButton>(ButtonPool.Count);
        possibleButtons.AddRange(ButtonPool);

        for(int i = 0; i < visibleButtons; ++i)
        {
            // Get one random button out of the collection and display it
            int index = Random.Range(0, possibleButtons.Count - 1);
            ColorTapButton current = possibleButtons[index];

            // Always assign the TO-TIP-color first
            if(i == 1) current.TColor = m_selectedColor;
            else
            {
                
            }

        }

    }

    #endregion

}
