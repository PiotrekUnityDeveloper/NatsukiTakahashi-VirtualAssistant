using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppLocalizer : MonoBehaviour
{
    public List<RectTransform> vapplications = new List<RectTransform>();

    // Start is called before the first frame update
    void Start()
    {
        InitApplications();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject appLocalizerPrefab;

    public void InitApplications()
    {
        foreach(RectTransform rectt in vapplications)
        {
            Localizator local = Instantiate(appLocalizerPrefab, transform.position, Quaternion.identity).GetComponent<Localizator>();
            local.canvasCamera = Camera.main;
            local.target = rectt;
        }
    }
}
