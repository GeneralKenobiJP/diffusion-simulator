using UnityEngine;
using System.IO;

[System.Serializable]
public class ParticleType
{
    public ParticleType(string typeName)
    {
        type = typeName;
    }
    public ParticleType()
    {
        type = "Oxygen";
    }

    public string type;
    public string molarMass;
    public string meltingPoint;
    public string boilingPoint;
    public string molarHeatCapacity;
    public string bondType;
    public string dipoleMoment;
    public string colorGas;
    public string color;

    public static ParticleType CreateFromJSON(string particleType)
    {
        var jsonString = File.ReadAllText("Assets/Scripts/ParticleType.json");
        return JsonUtility.FromJson<ParticleType>(jsonString);
    }
    public static void SaveIntoJSON(string particleType)
    {
        var jsonString = File.OpenWrite("Assets/Scripts/Example.json");
        //jsonString. (JsonUtility.ToJson(jsonString));
    }

    public void ToStringDebug()
    {
        Debug.Log(type);
        Debug.Log(molarMass);
        Debug.Log(meltingPoint);
        Debug.Log(boilingPoint);
        Debug.Log(molarHeatCapacity);
        Debug.Log(bondType);
        Debug.Log(dipoleMoment);
        Debug.Log(colorGas);
        Debug.Log(color);
    }
}
