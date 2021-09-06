using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DetailPanel : MonoBehaviour {

    public TMP_InputField inputField;

    // stores the Detail ID
    [HideInInspector]
    public int id = -1;

    // stores the type of detail
    [HideInInspector]
    public string type = "";

    public void DeleteButton()
    {
        // if the data in the panel is from the DB
        if(id != -1)
        {
            // Add the id to the list of details to delete
            ContactsDBManager.manager.detailsToDelete.Add(id);
        }

        // Remove this panel from the list of spawned details panels
        ContactsDBManager.manager.spawnedDetailsPanels.Remove(this);

        // Destroy this panel
        Destroy(gameObject);
    }
}
