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
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        boxCollider = this.gameObject.GetComponent<BoxCollider2D>();
        StartCoroutine(UpdateWindowPosition());
    }

    private void Update()
    {
        UpdateWindowCollision();
    }

    public void UpdateWindowCollision()
    {
        if(boxCollider != null)
        {
            if (spriteRenderer.bounds.size.x > (SystemManager.Xwidth / 5) * 4 ||
            spriteRenderer.bounds.size.y > (SystemManager.Yheight / 5) * 4)
            // we check, if the window's width OR height is greater than 4/5 of your native display size (w/h),
            // if yes, then disable the collider, so your character doesnt get 'pushed out'
            {
                this.boxCollider.isTrigger = true;
            }
            else
            {
                this.boxCollider.isTrigger = false;
            }
        }
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
