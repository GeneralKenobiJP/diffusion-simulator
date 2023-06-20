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
        Initialize();

        //LabelItemSelected();
        //dropdown.onValueChanged.AddListener(delegate {LabelItemSelected();});
    }
    public void Initialize()
    {
        dropdown = transform.GetComponent<Dropdown>();
        
        dropdown.options.Clear();

        dropdown.AddOptions(thisOptions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
