using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resizable : MonoBehaviour
{
    public GameObject objectToResize;
    private Vector3 originalScale;
    private bool isResizing = false;
    private Vector2 initialMousePosition;

    void Start()
    {
        originalScale = objectToResize.transform.localScale;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(SystemManager.instance.displayCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isResizing = true;
                initialMousePosition = SystemManager.instance.displayCamera.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isResizing = false;
        }

        if (isResizing)
        {
            Vector2 currentMousePosition = SystemManager.instance.displayCamera.ScreenToWorldPoint(Input.mousePosition);
            float scaleX = originalScale.x + (currentMousePosition.x - initialMousePosition.x) * 0.95f;
            float scaleY = originalScale.y - (currentMousePosition.y - initialMousePosition.y) * 0.95f;
            Vector3 newScale = new Vector3(scaleX, scaleY, 1f);
            objectToResize.transform.localScale = newScale;
        }
    }
}
