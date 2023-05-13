using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class ParticleJSONSerializer
{
    public List<ParticleType> _particleTypes;

    public static ParticleJSONSerializer CreateFromJSON(string particleType)
    {
        var jsonString = File.ReadAllText("Assets/Scripts/Example.json");
        return JsonUtility.FromJson<ParticleJSONSerializer>(jsonString);
    }
    public void ToStringDebug()
    {
        foreach(var x in _particleTypes)
            x.ToStringDebug();
    }
}
