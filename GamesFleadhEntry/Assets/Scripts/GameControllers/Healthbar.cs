using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider;

    // Start is called before the first frame update
    public void SetCurrent(float Health)
        {
            slider.value = Health;
        }

    public void SetMaxBar(float Health)
    {
        slider.maxValue = Health;
    }
}
