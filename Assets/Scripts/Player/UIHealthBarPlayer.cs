using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHealthBarPlayer : MonoBehaviour
{
    private float maxHealthBarValue;

    // Start is called before the first frame update
    void Start()
    {
        maxHealthBarValue = GetComponent<RectTransform>().sizeDelta.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateHealth(float percentage)
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(percentage * maxHealthBarValue, GetComponent<RectTransform>().sizeDelta.y);
    }
}
