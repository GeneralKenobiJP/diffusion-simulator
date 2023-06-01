using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class jsonSerializer
{
    [System.Serializable]
    private class ParticleWrapper {
        public ParticleType[] Items;
    }

    public static ParticleType[] FromJson()
    {
        var jsonFile = File.ReadAllText("Assets/Scripts/ParticleType.json");
        return JsonUtility.FromJson<ParticleWrapper>(jsonFile).Items;
    }

    public static ParticleType SearchForParticle(string searchType)
    {
        var particleList = FromJson();
        var num = 0;
        var i=0;
        foreach(var item in particleList)
        {
            if(item.type==searchType)
                num=i;
            i++;
        }
        return particleList[num]; //emergency return
    }
}
