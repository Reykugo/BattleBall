using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour {

    //public bool isModal = false;
    private bool isOpen = false;
    public delegate void OpenObserver();
    public event OpenObserver OnOpen;
    public event OpenObserver OnClose;

    public virtual void Open()
    {
        if (isOpen == true)
            Debug.LogWarning("Two call on open");
        gameObject.SetActive(true);
        isOpen = true;
        if (OnOpen != null)
            OnOpen();
    }

    public virtual void Close()
    {
        if (isOpen == false)
            Debug.LogWarning("Two call on close");
        isOpen = false;
        gameObject.SetActive(false);
        if (OnClose != null)
            OnClose();
    }


}
