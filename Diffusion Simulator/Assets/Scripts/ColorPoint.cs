using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPoint
{
    public Vector3 position;
    public Color colorRGB;
    public ColorHSV colorHSV;

    public ColorPoint()
    {
        position = new Vector3();
        //colorRGB = new Color();
        colorHSV = new ColorHSV();
    }
}
