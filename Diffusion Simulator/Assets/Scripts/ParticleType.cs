using UnityEngine;

[System.Serializable]
public class ParticleType
{
    public string type;
    public float molarMass;
    //possibly density is needed
    public float meltingPoint;
    public float boilingPoint;
    public float molarHeatCapacity;
    public string bondType;
    public float dipoleMoment;
    public byte[] colorGas;
    public byte[] color;
    public float normalDensity;
}
