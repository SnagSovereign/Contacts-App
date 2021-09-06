using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddressPanel : MonoBehaviour {

	// all of the input fields required for the address
	public TMP_InputField street1InputField;
	public TMP_InputField street2InputField;
	public TMP_InputField suburbInputField;
	public TMP_InputField stateInputField;
	public TMP_InputField postcodeInputField;
	public TMP_InputField countryInputField;

	// stores the Detail ID
	[HideInInspector]
	public int id = -1;

	public void DeleteButton()
    {
		// if the data in the panel is from the DB
		if(id != -1)
        {
			// Add the id to the list of addresses to delete
			ContactsDBManager.manager.addressesToDelete.Add(id);
        }

		// Remove this panel from the list of spawned address panels
		ContactsDBManager.manager.spawnedAddressPanels.Remove(this);

		// Destory this panel
		Destroy(gameObject);
    }
}
