//using System.Collections;
using System.Collections.Generic;
public class CalculationColumn
{
    public CalculationColumn()
    {
        probeList = new List<CalculationProbe>();
    }
    public List<CalculationProbe> probeList;
    public int columnID;

    public List<CalculationProbe> GetProbesColumn(float posY, string mode) //getprobescolumn higher/lower than posY
    {
        var modeList = new List<CalculationProbe>();
        if(mode=="lower")
        {
            foreach(var item in probeList)
            {
                if(item.transform.position.y<posY)
                    modeList.Add(item);
            }
        }
        else
        {
            foreach(var item in probeList)
            {
                if(item.transform.position.y>posY)
                    modeList.Add(item);
            }
        }
        return probeList;
    }
    public List<CalculationProbe> GetProbesColumn() //getprobescolumn
    {
        return probeList;
    }
}
