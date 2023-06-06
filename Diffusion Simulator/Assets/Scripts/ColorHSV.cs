using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorHSV
{
    public float h; //[0,1] (normally [0,360])
    public float s; //[0,1]
    public float v; //[0,1]

    public ColorHSV(float hue, float saturation, float value)
    {
        h=hue;
        s=saturation;
        v=value;
    }
    public ColorHSV()
    {
        h = new float();
        s = new float();
        v = new float();
    }

    public void SetHSVFromRGB(Color rgb)
    {
        Color.RGBToHSV(rgb,out h,out s,out v);
    }

    public void GetRGBFromHSV(out Color rgb)
    {
        rgb = Color.HSVToRGB(h,s,v);
    }

    public void DebugHSV()
    {
        Debug.Log(h);
        Debug.Log(s);
        Debug.Log(v);
    }

    public static ColorHSV operator +(ColorHSV a, ColorHSV b)
        => new ColorHSV(a.h+b.h,a.s+b.s,a.v+b.v);
    public static ColorHSV operator -(ColorHSV a, ColorHSV b)
        => new ColorHSV(a.h-b.h,a.s-b.s,a.v-b.v);
    public static ColorHSV operator *(ColorHSV a, float b)
        => new ColorHSV(a.h*b,a.s*b,a.v*b);
    public static ColorHSV operator /(ColorHSV a, float b)
        => new ColorHSV(a.h/b,a.s/b,a.v/b);
}
