using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Interpolator
{
    private const float MIN_DIST = 0.01f;
    private const int PARAMETER = 1;
    public static void IDW(ColorPoint[] inputPoints, ref ColorPoint[] outputPoints) //Inverse Distance Weighting
    {
        for(var i=0;i<outputPoints.Length;i++)
        {
            outputPoints[i].colorHSV = Interpolate(outputPoints[i]);
        }

        float InvertedDistanceAdjusted(float distance)
        {
            return 1f/Mathf.Pow(distance,PARAMETER);
        }
        ColorHSV Interpolate(ColorPoint Point)
        {
            float[] distances = new float[inputPoints.Length];
            for(var i=0;i<distances.Length;i++)
            {
                distances[i] = new float();
                distances[i] = GetDistance(Point.position,inputPoints[i].position);
                if(distances[i]<MIN_DIST)
                    return inputPoints[i].colorHSV;
            }
            var interpolatedHSV = new ColorHSV(0,0,0);
            var denominator = 0f;
            for(var i=0;i<distances.Length;i++)
            {
                //inputPoints[i].colorHSV.DebugHSV();
                var invertedDistance = InvertedDistanceAdjusted(distances[i]);
                var iHSV = inputPoints[i].colorHSV*invertedDistance;
                //iHSV.DebugHSV();
                //Debug.Log(invertedDistance);
                denominator += invertedDistance;
                interpolatedHSV+=iHSV;
            }
            interpolatedHSV/=denominator;
            return interpolatedHSV;
        }
    }

    private static float GetDistance(Vector3 P, Vector3 Q)
    {
        var x = Q.x-P.x;
        var y = Q.y-P.y;
        var z = Q.z-P.z;
        return Mathf.Sqrt(x*x+y*y+z*z);
    }
}
