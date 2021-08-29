using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContactPanel : MonoBehaviour {

    // Unity linking variables for ContactPanel object
    public TextMeshProUGUI contactNameText;

    private int personID;

    public void SetPersonID(int person)
    {
        personID = person;
    }

    public int GetPersonID()
    {
        return personID;
    }

    public void ShowContactDetails()
    {
        Debug.Log("You clicked on " + contactNameText.text + ". ID = " + GetPersonID());
    }
}