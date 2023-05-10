using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtendedMath
{
    public static Vector2 RotateVector2(Vector2 u, float angle)//radians
    {
        //Rotation matrix multiplication
        return new Vector2(u.x*Mathf.Cos(angle)-u.y*Mathf.Sin(angle),u.x*Mathf.Sin(angle)+u.y*Mathf.Cos(angle));
    }
    public static Vector3 RotateVector2(Vector3 u, float angle)
    {
        //Rotation matrix multiplication
        //Currently considering only y=0
        return new Vector3(u.x*Mathf.Cos(angle)-u.z*Mathf.Sin(angle),0,u.x*Mathf.Sin(angle)+u.z*Mathf.Cos(angle));
    }
    public static Vector3 RotateVector2(Vector3 u, float angle, float yPos)
    {
        //Rotation matrix multiplication
        //Currently considering only y=yPos
        return new Vector3(u.x*Mathf.Cos(angle)-u.z*Mathf.Sin(angle),yPos,u.x*Mathf.Sin(angle)+u.z*Mathf.Cos(angle));
    }
    public static float RadToDeg(float radians)
    {
        return radians*180f/Mathf.PI;
    }
    public static void ReflectVector(ref Vector3 u,Vector3 v /*Normal of hte surface hit*/)
    {
        //Debug.DrawRay(GameObject.Find("Particle").GetComponent<Transform>().position, u* 10f, new Color(0,0,255), 10f);
        u = u-2*Vector3.Dot(u,v)*v/Mathf.Pow(v.magnitude,2);
        //Debug.DrawRay(GameObject.Find("Particle").GetComponent<Transform>().position, u* 10f, new Color(255,0,0), 10f);
    }
}
