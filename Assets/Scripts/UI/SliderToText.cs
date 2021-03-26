using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderToText : MonoBehaviour
{
    private Text text;
    public Slider slider;

    public void Start()
    {
        text = GetComponent<Text>();
        text.text = slider.value.ToString("F2");
    }

    public void UpdateValue(System.Single newValue)
    {
        text.text = newValue.ToString("F2");
    }
}
