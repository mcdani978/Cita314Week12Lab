using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SimpleSliderControl : MonoBehaviour
{
    public UnityEvent OnSliderActive;

    
    Slider slider;

    [SerializeField]
    float minValue = 0f;

    [SerializeField]
    float maxValue = 20f;

    [SerializeField]
    Light libraryLight;

    private void OnEnable()
    {
        slider = GetComponent<Slider>();

        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = minValue;
        slider.onValueChanged.AddListener(OnValueChanged);
        libraryLight.intensity = minValue;
    }

    private void OnValueChanged(float arg0)
    {
        if (arg0 >= maxValue)
        {
            OnSliderActive?.Invoke();
        }
        if (libraryLight != null)
        {
            libraryLight.intensity = arg0;
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
