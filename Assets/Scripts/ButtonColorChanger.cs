using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChanger : MonoBehaviour
{
    // Start is called before the first frame update
    public Text t;

    public void ChangeColour()
    {
        t.color = Color.red;
    }
}
