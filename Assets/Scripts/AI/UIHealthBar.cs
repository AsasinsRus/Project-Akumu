using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
    }

    public void UpdateHealth(float percentage)
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(percentage * 100, GetComponent<RectTransform>().sizeDelta.y);
    }
}
