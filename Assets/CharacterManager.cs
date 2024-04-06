using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        
    }

    private void OnMouseOver()
    {
        //print("mouse down!");
        if (Input.GetMouseButtonDown(1)) // Right click
        {
            //print("right click!");
            ShowMainContextMenu();
        }
    }

    public CMenu mainContextM;
    public void ShowMainContextMenu()
    {
        mainContextM.gameObject.SetActive(true);

        if(!mainContextM.isOpened)
        {
            mainContextM.ShowMenu();
            MoveContextMenuToCursor(mainContextM);
        }
        else
        {
            MoveContextMenuToCursor(mainContextM);
            mainContextM.UpdateSubMenusPositions();
        }

    }

    public void HideMainContextMenu()
    {
        mainContextM.HideMenu();
    }

    public void MoveContextMenuToCursor(CMenu cmenu)
    {
        // Get the RectTransform of the menu
        RectTransform menuRectTransform = cmenu.gameObject.GetComponent<RectTransform>();

        // Convert mouse position to local position within canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cmenu.GetComponentInParent<Canvas>().transform as RectTransform, Input.mousePosition, cmenu.GetComponentInParent<Canvas>().worldCamera, out Vector2 localPoint);

        // Set the local position of the menu within the canvas
        menuRectTransform.localPosition = localPoint;
    }
}
