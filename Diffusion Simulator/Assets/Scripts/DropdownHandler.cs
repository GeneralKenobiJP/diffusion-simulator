using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownHandler : MonoBehaviour
{
    /*DropdownHandler(List<string> opt)
    {
        thisOptions = opt;
    }*/
    public List<string> thisOptions;
    private Dropdown dropdown;
    public Text text;
    void Start()
    {
        dropdown = transform.GetComponent<Dropdown>();
        
        dropdown.options.Clear();

        foreach(var item in thisOptions)
        {
            dropdown.options.Add(new Dropdown.OptionData(){text = item});
        }

        LabelItemSelected();
        dropdown.onValueChanged.AddListener(delegate {LabelItemSelected();});
    }


    private void LabelItemSelected()
    {
        var index = dropdown.value;
        text.text = dropdown.options[index].text;
        Debug.Log(text.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
