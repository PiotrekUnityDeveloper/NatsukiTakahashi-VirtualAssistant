using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMenuItem : MonoBehaviour
{
    private Animator animator;
    public bool isAContainer = false;
    public CMenu containedMenu = null;
    public GameObject containedPositionObject = null;

    private CMenu parent;

    private void Awake()
    {
        AssignAnimator();
    }

    private void AssignAnimator()
    {
        animator = this.GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitMenuItem(CMenu parent)
    {
        if(this.animator == null)
            AssignAnimator();

        this.parent = parent;

        if (isAContainer)
        {
            parent.subMenus.Add(containedMenu);
        }

        //print(this.animator.gameObject.name);
        this.animator.SetTrigger("show");
    }

    public void HideMenuItem()
    {
        if (this.animator == null)
            AssignAnimator();

        this.animator.SetTrigger("hide");
    }

    public void ExtendContainedMenu()
    {
        if (isAContainer)
        {
            //parent.StopCoroutine(parent.HideMenuEnumerator());
            //parent.StopAllHideCoroutines();
            containedMenu.StopAllHideCoroutines();
            if (!containedMenu.gameObject.activeInHierarchy) { containedMenu.gameObject.SetActive(true); }
            containedMenu.MoveContextPanelRelativeToHostMenu(this);
            if(!containedMenu.isOpened) containedMenu.ShowMenu();
        }
    }

    public void CollapseContainedMenu()
    {
        if (isAContainer)
        {
            containedMenu.HideMenu();
        }
    }

    public void CollapseOpenedSubMenusAndOpenNew()
    {
        if(containedMenu == null)
            return;

        parent.CollapseOpenedSubMenusAndIgnore(this.containedMenu);
        //parent.CollapseOpenedSubMenus(); // collapses itself when hovered twice ;i
        ExtendContainedMenu();
    }

    public void UpdateContextSubMenuPosition()
    {
        if(isAContainer && containedMenu != null)
        {
            containedMenu.MoveContextPanelRelativeToHostMenu(this);
        }
    }
}
