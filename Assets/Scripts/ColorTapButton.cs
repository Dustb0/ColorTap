using UnityEngine;
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

public enum TapType
{
    Full,
    Half
}

public class ColorTapButton : Button
{

    public TapColor TColor;
    public TapType TType;

	void OnDisable()
    {
        image.enabled = false;
    }

    void OnEnable()
    {
        image.enabled = true;
    }
}
