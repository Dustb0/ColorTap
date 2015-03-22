﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum TapColor
{
    Red,
    Green,
    Blue,
    Magenta,
    Yellow,
    Cyan
}

public class ColorTapButton : Button
{

    public TapColor TColor;
    public bool IsMinusButton;

    /// <summary>
    /// Initializes the button and removes any modifier-flags for gameplay
    /// </summary>
    public void Init()
    {
        enabled = true;
        IsMinusButton = false;
    }

	void OnDisable()
    {
        animator.enabled = false;
        image.enabled = false;
    }

    void OnEnable()
    {
        animator.enabled = true;
        image.enabled = true;
    }
}
