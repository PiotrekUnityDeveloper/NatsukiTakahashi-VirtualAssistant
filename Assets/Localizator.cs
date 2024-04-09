using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localizator : MonoBehaviour
{
    public RectTransform target;
    public Camera canvasCamera;

    private BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && canvasCamera != null)
        {

            Vector3 targetViewportPosition = canvasCamera.WorldToViewportPoint(target.position);
            Vector3 targetWorldPosition = canvasCamera.ViewportToWorldPoint(targetViewportPosition);

            transform.position = new Vector3(targetWorldPosition.x, targetWorldPosition.y, transform.position.z);

            //transform.localScale = target.localScale;
            Vector2 sizeDelta = target.sizeDelta / target.lossyScale;
            boxCollider.size = sizeDelta;
        }
    }
}
