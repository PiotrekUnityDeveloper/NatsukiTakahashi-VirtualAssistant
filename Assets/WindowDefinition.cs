using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
//using UnityEditor.MPE;
using UnityEngine;

public class WindowDefinition : MonoBehaviour
{
    [HideInInspector] public SystemManager systemManager;
    [HideInInspector] public Process myProcess;
    public float updateDelay = 0.1f; //update the window position every 0.5 seconds :333

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateWindowPosition());
    }

    private IEnumerator UpdateWindowPosition()
    {
        if (myProcess != null)
        {
            systemManager.UpdateBounds(myProcess, this.gameObject);
        }

        yield return new WaitForSeconds(updateDelay);
        StartCoroutine(UpdateWindowPosition());
    }

}
