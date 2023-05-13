using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class jsonSerializer
{
    //public string jsonText;

    public class ParticleType
    {
        public ParticleType(string typeName)
        {
            type = typeName;
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

    [System.Serializable]
    public class ParticleTypeSerializer
    {
        public ParticleType[] _particleList;
    }
    //public ParticleTypeSerializer jsonParticleType = new ParticleTypeSerializer();

    public static void ParticleCreateFromJSON()
    {
        var jsonText = File.ReadAllText("Assets/Scripts/Example.json");
        var jsonParticleType = new ParticleTypeSerializer();
        jsonParticleType = JsonUtility.FromJson<ParticleTypeSerializer>(jsonText);
    }
}
