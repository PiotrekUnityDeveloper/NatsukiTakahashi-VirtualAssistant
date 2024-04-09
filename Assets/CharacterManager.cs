using Live2D.Cubism.Framework.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        if(mainContextM.isOpened) HideMainContextMenu();
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

    public static void MoveContextMenuToCursor(CMenu cmenu)
    {
        // Get the RectTransform of the menu
        RectTransform menuRectTransform = cmenu.gameObject.GetComponent<RectTransform>();

        // Convert mouse position to local position within canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cmenu.GetComponentInParent<Canvas>().transform as RectTransform, Input.mousePosition, cmenu.GetComponentInParent<Canvas>().worldCamera, out Vector2 localPoint);

        // Set the local position of the menu within the canvas
        menuRectTransform.localPosition = localPoint;
    }

    public float currentCharacterScale = 1f;
    private float scaleChange = 0.25f;
    public Slider characterScaleSlider;

    public void ModifyCharacterScale(float value)
    {
        if (characterScaleSlider.value + (value) <= characterScaleSlider.maxValue &&
            characterScaleSlider.value + (value) >= characterScaleSlider.minValue)
        {
            characterScaleSlider.value += value;
            UpdateCharacterScale();
        }
        else if(value > 0) //positive, round up
        {
            characterScaleSlider.value = characterScaleSlider.maxValue;
            UpdateCharacterScale();
        }
        else if(value < 0) //negative, round down
        {
            characterScaleSlider.value = characterScaleSlider.minValue;
            UpdateCharacterScale();
        }
    }

    public void ModifyCharacterScale(bool negative)
    {
        float value = negative ? scaleChange* -1 : scaleChange;

        if (characterScaleSlider.value + (value) <= characterScaleSlider.maxValue &&
            characterScaleSlider.value + (value) >= characterScaleSlider.minValue)
        {
            characterScaleSlider.value += value;
            UpdateCharacterScale();
        }
        else if (value > 0) //positive, round up
        {
            characterScaleSlider.value = characterScaleSlider.maxValue;
            UpdateCharacterScale();
        }
        else if (value < 0) //negative, round down
        {
            characterScaleSlider.value = characterScaleSlider.minValue;
            UpdateCharacterScale();
        }
    }

    public void UpdateCharacterScale()
    {
        this.gameObject.transform.localScale = new Vector3(characterScaleSlider.value,
            characterScaleSlider.value, characterScaleSlider.value);
    }
}
