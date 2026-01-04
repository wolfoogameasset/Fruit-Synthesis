using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageScroller : MonoBehaviour
{
    [SerializeField] RawImage _rawImage;
    [SerializeField] float _x, _y;

    public float X { get => _x; set => _x = value; }
    public float Y { get => _y; set => _y = value; }

    private void Start()
    {
        StartCoroutine(ScrollImage());
    }

    private void OnEnable()
    {
        StartCoroutine(ScrollImage());
    }
    private void OnDisable()
    {
        StopCoroutine(ScrollImage());
    }

    IEnumerator ScrollImage()
    {
        while (true)
        {
            _rawImage.uvRect = new Rect(_rawImage.uvRect.position + new Vector2(X, Y) * Time.deltaTime, _rawImage.uvRect.size);
            yield return null;
        }
    }

}
