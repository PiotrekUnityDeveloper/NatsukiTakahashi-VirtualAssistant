using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMenu : MonoBehaviour
{
    private Animator animator;
    [HideInInspector] public List<CMenu> subMenus = new List<CMenu>();

    [HideInInspector] public bool isOpened = false;

    public CMenuItem subMenuItemParent;

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

    public void OnCollisionStay(Collision collision)
    {
        
    }

    public void ShowMenu()
    {
        StartCoroutine(ShowMenuEnumerator());
    }

    private IEnumerator ShowMenuEnumerator()
    {
        if(this.animator == null)
            AssignAnimator();

        this.animator.SetTrigger("show");
        this.isOpened = true;
        this.animator.SetBool("opened", true);

        
        CMenuItem[] menuItems = GetComponentsInChildren<CMenuItem>();

        foreach (CMenuItem menuitem in menuItems)
        {
            yield return new WaitForSeconds(0.1f);
            menuitem.InitMenuItem(this);
            menuitem.UpdateContextSubMenuPosition();
        }
    }

    public void HideMenu()
    {
        if (this.isActiveAndEnabled)
        {
            hideWindowCoroutines.Add(StartCoroutine(HideMenuEnumerator()));
        }
    }

    private List<Coroutine> hideWindowCoroutines = new List<Coroutine>();

    public IEnumerator HideMenuEnumerator()
    {
        CMenuItem[] menuItems = GetComponentsInChildren<CMenuItem>();

        this.isOpened = false;
        this.animator.SetBool("opened", false);

        foreach (CMenuItem menuitem in menuItems)
        {
            yield return new WaitForSeconds(0.1f);
            menuitem.HideMenuItem();
        }

        this.animator.SetTrigger("hide");
        //this.animator.SetBool("opened", false);
        yield return new WaitForSeconds(1.5f);
        this.gameObject.SetActive(false);
    }

    public void StopAllHideCoroutines()
    {
        foreach(Coroutine cor in hideWindowCoroutines)
        {
            StopCoroutine(cor);
        }

        hideWindowCoroutines.Clear();
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
    }

    public void MoveContextPanelRelativeToHostMenu(CMenuItem cmenu)
    {
        this.GetComponent<RectTransform>().position = cmenu.containedPositionObject.GetComponent<RectTransform>().position;
    }

    public void CollapseOpenedSubMenus()
    {
        foreach(CMenu menu in subMenus)
        {
            if (menu.isOpened)
            {
                menu.HideMenu();
            }
        }
    }

    public void CollapseOpenedSubMenusAndIgnore(CMenu menuToIgnore)
    {
        foreach (CMenu menu in subMenus)
        {
            if (menu.isOpened && menu.gameObject.activeInHierarchy && menu != menuToIgnore)
            {
                menu.HideMenu();
            }
        }
    }

    public void UpdateSubMenusPositions()
    {
        foreach(CMenu menu in subMenus)
        {
            if (menu.subMenuItemParent != null)
            {
                menu.MoveContextPanelRelativeToHostMenu(menu.subMenuItemParent);
            }
        }
    }
}
