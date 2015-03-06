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
    private const int TOTAL_BUTTONS = 20;

    private float m_progressFullWidth;
    private GamePhase m_currentPhase;
    private float m_remainingTime;
    private int m_screenIndex;
    private TapColor m_selectedColor;
    
    #endregion

    #region " Engine Callbacks "

    void Start()
    {
        // Save progressrect
        m_progressFullWidth = FullRect.sizeDelta.x;

        // Set remaining time
        m_remainingTime = TOTAL_TIME;

        // Hide all game buttons
        foreach(ColorTapButton btn in ButtonPool)
        {
            btn.enabled = false;
        }

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
        int visibleButtons = 4 + m_screenIndex;

        // Collect possible button spots
        

        for(int i = 0; i < visibleButtons; ++i)
        {

        }

    }

    #endregion

}
