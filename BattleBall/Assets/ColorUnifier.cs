using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorUnifier : MonoBehaviour {

    public delegate void ColorObserver(Color color);
    public event ColorObserver OnColorChanged;

    private Color _Color = Color.black;
	// Use this for initialization

    public void SetColor(Color color)
    {
        _Color = color;
        if(color != _Color && OnColorChanged != null)
        {
            OnColorChanged(color);
        }
    }

    public Color GetColor()
    {
        return _Color;
    }
}
