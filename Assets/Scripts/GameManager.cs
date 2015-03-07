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
        Game
    }

    public RectTransform ProgressRect;
    public RectTransform FullRect;
    public List<ColorTapButton> SelectButtons;
    public Animator TapBoardAnimator;

    private const float TOTAL_TIME = 30;

    private float m_progressFullWidth;
    private GamePhase m_currentPhase;
    private float m_remainingTime;
    private int m_levelIndex;
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
        foreach (ColorTapButton btn in ButtonPool) btn.enabled = false;

        // Hide progressbar
        m_progressImageBar = ProgressRect.GetComponent<Image>();
        m_progressImageBG = FullRect.GetComponent<Image>();
        m_progressImageBar.enabled = false;
        m_progressImageBG.enabled = false;

        m_currentPhase = GamePhase.SelectColor;
        m_levelIndex = 1;
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

                // Set width
                ProgressRect.sizeDelta = new Vector2((m_remainingTime / TOTAL_TIME) * m_progressFullWidth, ProgressRect.sizeDelta.y);
                break;
        }
    }

    #endregion

    #region " Color Selection "

    public void PrepareColorSelection()
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

    public void SelectColor()
    {
        // Determine clicked button
        ColorTapButton btn = EventSystem.current.currentSelectedGameObject.GetComponent<ColorTapButton>();
        m_selectedColor = btn.TColor;

        foreach(ColorTapButton b in SelectButtons)
        {
            b.animator.SetTrigger("HideButton");
        }

        // Set game phase
        m_currentPhase = GamePhase.Game;
        m_levelIndex = 1;

        // Set time & Show timebar
        m_remainingTime = TOTAL_TIME;
        m_progressImageBar.enabled = true;
        m_progressImageBG.enabled = true;

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
                    SetupNewScreen();
                }
                else
                {
                    // Shake screen!
                    TapBoardAnimator.SetTrigger("Wrong");
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

        foreach (ColorTapButton btn in ButtonPool) btn.enabled = true;

    }

    #endregion

}
