using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    private RectTransform reticle;
    [SerializeField, Range(50, 250f)]
    private float currentSize;

    // TODO change on isMoving state
    private float maxSize;
    private float restingSize;

    private void Start()
    {
        reticle = GetComponent<RectTransform>();
        currentSize = 100;

    }

    private void Update()
    {
        reticle.sizeDelta = new Vector2(currentSize, currentSize);
    }
}
