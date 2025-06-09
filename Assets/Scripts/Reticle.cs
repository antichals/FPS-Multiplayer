using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    private RectTransform reticle;
    [SerializeField, Range(50, 250f)]
    private float size;

    private void Start()
    {
        reticle = GetComponent<RectTransform>();
        size = 100;

    }

    private void Update()
    {
        reticle.sizeDelta = new Vector2(size, size);
    }
}
