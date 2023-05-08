using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtendedMath
{
    public static Vector2 RotateVector2(Vector2 u, float angle)
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
    public static float RadToDeg(float radians)
    {
        return radians*180f/Mathf.PI;
    }

    /*public static Quaternion QuaternionRotation(Quaternion quat, Vector3 rot) //I fucking hate quaternions
    {
        quat = Quaternion.AngleAxis(rot.x,Vector3.right);
        quat = Quaternion.AngleAxis(rot.y,Vector3.up);
        quat = Quaternion.AngleAxis(rot.z,Vector3.forward);

        return quat;
    }*/
}
