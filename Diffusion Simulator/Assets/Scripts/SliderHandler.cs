using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderHandler : MonoBehaviour
{
    private Slider slider;
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        slider = this.GetComponent<Slider>();
        text.text = "300 K (27°C | 80.6°F)";
    }

    public void SetTemperatureLabel()
    {
        var kelvin = slider.value;
        var celsius = kelvin-273;
        var fahrenheit = celsius*9f/5f+32f;
        text.text = slider.value.ToString() + " K (" + celsius.ToString() + "°C | " + fahrenheit.ToString() + "°F)";
        Debug.Log((140-273)*9f/5f+32f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
