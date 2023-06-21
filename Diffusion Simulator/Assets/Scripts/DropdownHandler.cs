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
    private bool isBottom; //checks if it is a dropdown B
    public Button button;
    private ButtonHandler buttonHandler;
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

        if(this.name=="Dropdown B")
            isBottom = true;
        else
            isBottom = false;

        buttonHandler = button.GetComponent<ButtonHandler>();
    }

    public void SelectParticle()
    {
        var index = dropdown.value;
        var particle = dropdown.options[index].text;
        Debug.Log("Le select: " + particle);
        //GameObject.Find("Setup").GetComponent<Setup>().ReInitialize(particleA,particleB);
        buttonHandler.UpdateParticleSelection(particle,isBottom);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
