using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            // Calculate the new position of the object based on the mouse position
            Vector3 newPosition = SystemManager.instance.displayCamera.ScreenToWorldPoint(Input.mousePosition) + offset;
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
    }

    private void OnMouseDown()
    {
        offset = transform.position - SystemManager.instance.displayCamera.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }


}
