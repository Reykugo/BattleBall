using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour {


    //public bool isModal = false;
    private bool isOpen = false;
    public delegate void OpenObserver();
    public event OpenObserver OnOpen;
    public event OpenObserver OnClose;

    public Text title;
    public Text content;

    void Start()
    {
        Close();
    }

    public void UpdateTitle(string newText)
    {
        if (title)
        {
            title.text = newText;
        }
    }

    public void UpdateContent(string newText)
    {
        if (content)
        {
            content.text = newText;
        }
    }

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
