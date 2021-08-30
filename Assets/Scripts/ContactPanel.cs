using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ContactPanel : MonoBehaviour {

    // Unity linking variables for ContactPanel object
    public TextMeshProUGUI contactNameText;

    private int personID;

    public void SetPersonID(int person)
    {
        personID = person;
    }

    public void ContactPanelButton()
    {
        Debug.Log("You clicked on " + contactNameText.text + ". ID = " + personID);

        // change the personID for the manager to equal the ID attached to this panel
        ContactsDBManager.manager.SetPersonID(personID);

        // change the screen to View Contact
        ContactsDBManager.manager.MenuGoTo(1);
    }
}