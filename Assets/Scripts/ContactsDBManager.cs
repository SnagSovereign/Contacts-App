using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class ContactsDBManager : MonoBehaviour {

	//create a static link to this class
	//so that other classes can access its public methods / variables
	public static ContactsDBManager manager;

	// screens
	[Header("Screens")]
	[SerializeField] GameObject mainScreen;
	[SerializeField] GameObject viewContactScreen;
	[SerializeField] GameObject editAddScreen;
	
	// main screen (ms = main screen)
	[Header("Main Screen")]
	[SerializeField] GameObject msContent;
	[SerializeField] GameObject contactPanel;

	// view contact screen (vs = view screen)
	[Header("View Contact Screen")]
	[SerializeField] GameObject vsContent;
	[SerializeField] TextMeshProUGUI detailText;
	[SerializeField] TextMeshProUGUI addressText;
	[SerializeField] GameObject spacer;
	[SerializeField] TextMeshProUGUI nameText;
	[SerializeField] TextMeshProUGUI nicknameText;
	[SerializeField] GameObject companyLabel;
	[SerializeField] TextMeshProUGUI companyText;
	[SerializeField] GameObject departmentLabel;
	[SerializeField] TextMeshProUGUI departmentText;
	[SerializeField] GameObject jobTitleLabel;
	[SerializeField] TextMeshProUGUI jobTitleText;
	[SerializeField] GameObject phoneLabel;
	[SerializeField] GameObject emailLabel;
	[SerializeField] GameObject addressLabel;
	[SerializeField] GameObject dobLabel;
	[SerializeField] TextMeshProUGUI dobText;

	// list containing instantiated objects on the view screen (vs)
	List<GameObject> vsSpawnedObjects = new List<GameObject>();

	// edit/add contact screen (ea = edit/add)
	[Header("Edit/Add Screen")]
	[SerializeField] GameObject eaContent;
	[SerializeField] AddressPanel editAddressPanel;
	[SerializeField] DetailPanel editDetailPanel;
	[SerializeField] GameObject addPhoneButton;
	[SerializeField] GameObject addEmailButton;
	[SerializeField] GameObject addAddressButton;
	[SerializeField] TMP_InputField firstNameInputField;
	[SerializeField] TMP_InputField lastNameInputField;
	[SerializeField] TMP_InputField otherNameInputField;
	[SerializeField] TMP_InputField nicknameInputField;
	[SerializeField] TMP_InputField companyInputField;
	[SerializeField] TMP_InputField departmentInputField;
	[SerializeField] TMP_InputField jobTitleInputField;
	[SerializeField] TMP_InputField dobInputField;
	[SerializeField] GameObject deleteContactButton;

	[Header("Message Objects")]
	[SerializeField] GameObject darkOverlay;
	[SerializeField] TextMeshProUGUI messageText;
	[SerializeField] GameObject okButton;

	// list containing all of the spawned Details panels on the edit/add screen (ea)
	[HideInInspector]
	public List<DetailPanel> spawnedDetailsPanels = new List<DetailPanel>();

	// list containing all of the spawned Address panels on the edit/add screen (ea)
	[HideInInspector]
	public List<AddressPanel> spawnedAddressPanels = new List<AddressPanel>();

	// list that contains any individual details the user wants to delete from the DB
	[HideInInspector]
	public List<int> detailsToDelete = new List<int>();

	// list that contains any individual addresses the user wants to delete from the DB
	[HideInInspector]
	public List<int> addressesToDelete = new List<int>();
				
	// class level variables
	int personID = -1;
	string myQuery;
	bool isEditScreen = false;

	// Use this for initialization
	void Start () 
	{
		//set manager to the instance of itself for ContactsDBManager
		manager = this;

        DisplayContactPanels();
    }

	public void SetPersonID(int person)
    {
		personID = person;
    }

	void DisplayContactPanels()
	{
		// Get the contacts IDs and names
		myQuery = "SELECT ID, FirstName, LastName, OtherName FROM Person ORDER BY FirstName;";
		RunMyQuery(myQuery);
		Debug.Log("My Query = " + myQuery);

		// Create a contact panel for each result in the reader
		while (DB.reader.Read())
		{
			// instantiate a new contact panel
			GameObject newPanel = Instantiate(contactPanel, msContent.transform);

			// access the script on the contact panel
			ContactPanel panel = newPanel.GetComponent<ContactPanel>();
			// set the personID of the contact panel
			panel.SetPersonID(DB.reader.GetInt32(0));

			// display the FirstName
			panel.contactNameText.text = DB.reader.GetString(1);

			// if there is an OtherName, then display it
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(3)))
            {
				panel.contactNameText.text += " " + DB.reader.GetString(3);
            }

			// if there is a LastName, then display it
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(2)))
            {
				panel.contactNameText.text += " " + DB.reader.GetString(2);
			}
		}
		// close the DB
		DB.CloseDB();
	}

	public void MenuGoTo(int screenIndex)
    {
		////////////////////////////////////////////////////////
		// changes the canvas for the user					  //
		// each screen is numbered with an index as follows:  //
		//		0 = Main Screen (Default)					  //
		//		1 = View Contact Screen						  //
		//		2 = Edit Contact Screen						  //
		//		3 = Add Contact Screen						  //
		////////////////////////////////////////////////////////

		// Clear all of the text off of all the screens
		ClearScreenData();

		// Turn off all the screens
		mainScreen.SetActive(false);
		viewContactScreen.SetActive(false);
		editAddScreen.SetActive(false);

		// set isEditScreen to false
		isEditScreen = false;

		// Turn on the screen that was requested
		switch (screenIndex)
        {
			case 1: // View Contact Screen
				viewContactScreen.SetActive(true);
				// display all of the data for the chosen contact
				DisplayContactDetails();
				break;
			case 2: // Edit Contact Screen
				editAddScreen.SetActive(true);
				isEditScreen = true;
				// enable the delete contact button
				deleteContactButton.SetActive(true);
				// fill in existing contact details in the input fields
				FillExistingDetails();
				break;
			case 3: // Add Contact Screen
				editAddScreen.SetActive(true);
				//disable the delete contact button
				deleteContactButton.SetActive(false);
				break;
			default: // Main Screen
				mainScreen.SetActive(true);
				DisplayContactPanels();
				break;
        }
    }

	void DisplayContactDetails()
    {
		// Run a query to select the data from the 'Person' table
		myQuery = "SELECT * FROM Person WHERE ID = " + personID + ";";
		RunMyQuery(myQuery);
		Debug.Log("My Query = " + myQuery);

		// check that there is data in the reader
		if(DB.reader.Read())
        {
			// Display the FirstName
			nameText.text = DB.reader.GetString(1);

			// If there is an OtherName, then display it
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(3)))
			{
				nameText.text += " " + DB.reader.GetString(3);
			}

			// If there is a LastName, then display it
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(2)))
			{
				nameText.text += " " + DB.reader.GetString(2);
			}

			// If there is a Nickname, then display it
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(4)))
			{
				nicknameText.gameObject.SetActive(true);
				nicknameText.text = '"' + DB.reader.GetString(4) + '"';
			}

			// If there is a company, then display it
			if (!string.IsNullOrWhiteSpace(DB.reader.GetString(5)))
			{
				companyLabel.SetActive(true);
				companyText.gameObject.SetActive(true);
				companyText.text = DB.reader.GetString(5);
			}

			// If there is department, then display it
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(6)))
			{
				departmentLabel.SetActive(true);
				departmentText.gameObject.SetActive(true);
				departmentText.text = DB.reader.GetString(6);
			}

			// If there is a Job Title, then display it
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(7)))
			{
				jobTitleLabel.SetActive(true);
				jobTitleText.gameObject.SetActive(true);
				jobTitleText.text = DB.reader.GetString(7);
			}

			// If there is a DOB, then display it
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(8)))
			{
				dobLabel.SetActive(true);
				dobText.gameObject.SetActive(true);
				dobText.text = DB.reader.GetString(8);
			}
        }

		// close the DB
		DB.CloseDB();

		// Run a query to select the data from the 'Details' table
		myQuery = "SELECT * FROM Details WHERE Person = " + personID + " ORDER BY ID DESC;";
		RunMyQuery(myQuery);
		Debug.Log("My Query = " + myQuery);

		// loop through all of the rows in the 'Details' table for the specific Person
		while(DB.reader.Read())
        {
			// store the type and contact of each detail
			string type = DB.reader.GetString(2);
			string contact = DB.reader.GetString(1);

			// Instantiate a new detail text
			TextMeshProUGUI newDetailText = Instantiate(detailText, vsContent.transform);
			// Add the new object to the list of spawned objects
			vsSpawnedObjects.Add(newDetailText.gameObject);

			if (type == "Phone") // If the detail is a phone number
			{
				// Ensure that the phone label is enabled
				phoneLabel.SetActive(true);
				// Position the instantiated textbox underneath the phone label
				newDetailText.transform.SetSiblingIndex(phoneLabel.transform.GetSiblingIndex() + 1);
			}
			else if (type == "Email") // If the detail is an email
			{
				// Ensure that the email label is enabled
				emailLabel.SetActive(true);
				// Position the instantiated textbox underneath the email label
				newDetailText.transform.SetSiblingIndex(emailLabel.transform.GetSiblingIndex() + 1);
				// Set the font size to 50 (Default is 75)
				newDetailText.fontSize = 50f;
			}

			// set the textbox equal to the contact variable
			newDetailText.text = contact;
		}

		// Close the DB
		DB.CloseDB();

		// Run a query to select data from the 'Address' table
		myQuery = "SELECT * FROM Address WHERE Person = " + personID + " ORDER BY ID DESC";
		RunMyQuery(myQuery);
		Debug.Log("My Query = " + myQuery);

		// loop through all of the rows in the 'Address' table for the specific Person
		while(DB.reader.Read())
        {
			// Ensure that the address label is enabled
			addressLabel.SetActive(true);

			// Instantiate a spacer underneath the address label
			GameObject newSpacer = Instantiate(spacer, vsContent.transform);
			newSpacer.transform.SetSiblingIndex(addressLabel.transform.GetSiblingIndex() + 1);
			vsSpawnedObjects.Add(newSpacer);

			TextMeshProUGUI newAddressText;

			//if there is a Country, then display it
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(6)))
            {
				newAddressText = Instantiate(addressText, vsContent.transform);
				newAddressText.transform.SetSiblingIndex(addressLabel.transform.GetSiblingIndex() + 1);
				newAddressText.text = DB.reader.GetString(6);
				vsSpawnedObjects.Add(newAddressText.gameObject);
            }

			// string that will store the suburb, state and postcode
			string suburbStatePostcode = "";

			// if there is a suburb, add it to the string
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(3)))
            {
				suburbStatePostcode += DB.reader.GetString(3) + " ";
            }

			// if there is a state, add it to the string
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(4)))
            {
				suburbStatePostcode += DB.reader.GetString(4) + " ";
            }

			// if there is a postcode, add it to the string
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(5)))
            {
				suburbStatePostcode += DB.reader.GetString(5);
            }

			// if the string containing the suburb, state, and postcode is not empty, then display it
			if(!string.IsNullOrWhiteSpace(suburbStatePostcode))
            {
				newAddressText = Instantiate(addressText, vsContent.transform);
				newAddressText.transform.SetSiblingIndex(addressLabel.transform.GetSiblingIndex() + 1);
				newAddressText.text = suburbStatePostcode;
				vsSpawnedObjects.Add(newAddressText.gameObject);
            }

			// if there is a Street2, then display it
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(2)))
            {
				newAddressText = Instantiate(addressText, vsContent.transform);
				newAddressText.transform.SetSiblingIndex(addressLabel.transform.GetSiblingIndex() + 1);
				newAddressText.text = DB.reader.GetString(2);
				vsSpawnedObjects.Add(newAddressText.gameObject);
            }

			// if there is a Street1, then display it
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(1)))
            {
				newAddressText = Instantiate(addressText, vsContent.transform);
				newAddressText.transform.SetSiblingIndex(addressLabel.transform.GetSiblingIndex() + 1);
				newAddressText.text = DB.reader.GetString(1);
				vsSpawnedObjects.Add(newAddressText.gameObject);
            }
        }

		// close the DB
		DB.CloseDB();
	}

	void FillExistingDetails()
    {
		// Run a query to select all of the data from the 'Person' table
		myQuery = "SELECT * FROM Person WHERE ID = " + personID + ";";
		RunMyQuery(myQuery);
		Debug.Log("My Query = " + myQuery);

		// check that there is data in the reader
		if(DB.reader.Read())
        {
			// Fill in all the data
			firstNameInputField.text = DB.reader.GetString(1);
			lastNameInputField.text = DB.reader.GetString(2);
			otherNameInputField.text = DB.reader.GetString(3);
			nicknameInputField.text = DB.reader.GetString(4);

			companyInputField.text = DB.reader.GetString(5);
			departmentInputField.text = DB.reader.GetString(6);
			jobTitleInputField.text = DB.reader.GetString(7);

			dobInputField.text = DB.reader.GetString(8);
        }

		// close the DB
		DB.CloseDB();

		// Run a query to select all of the data from the 'Details' table
		myQuery = "SELECT * FROM Details WHERE Person = " + personID + ";";
		RunMyQuery(myQuery);
		Debug.Log("My Query = " + myQuery);

		// check that there is data in the reader and loop through the rows
		while(DB.reader.Read())
        {
			// Instantiate a new Detail Panel
			DetailPanel newDetailPanel = Instantiate(editDetailPanel, eaContent.transform);

			// Set the ID and type of the new panel
			newDetailPanel.id = DB.reader.GetInt32(0);
			newDetailPanel.type = DB.reader.GetString(2);

			// Fill in the contact to the input field
			newDetailPanel.inputField.text = DB.reader.GetString(1);

			// Position the Panel above either the Add Phone or Add Email button
			if(newDetailPanel.type == "Phone")
            {
				newDetailPanel.transform.SetSiblingIndex(addPhoneButton.transform.GetSiblingIndex());
				// Set the placeholder to 'phone number'
				newDetailPanel.placeholder.text = "phone number";
            }
			else if (newDetailPanel.type == "Email")
            {
				newDetailPanel.transform.SetSiblingIndex(addEmailButton.transform.GetSiblingIndex());
				// Set the font size to 45
				newDetailPanel.inputField.textComponent.fontSize = 45f;
				// Set the placeholder to 'email address'
				newDetailPanel.placeholder.text = "email address";
			}

			// add the new panel to the list of spawned details panels
			spawnedDetailsPanels.Add(newDetailPanel);
		}

		// close the DB
		DB.CloseDB();

		// Run a query to select all of the data from the 'Address' table
		myQuery = "SELECT * FROM Address WHERE Person = " + personID + ";";
		RunMyQuery(myQuery);
		Debug.Log("My Query = " + myQuery);

		// check that there is data in the reader and loop through all the rows
		while(DB.reader.Read())
        {
			// Instantiate a new address panel
			AddressPanel newAddressPanel = Instantiate(editAddressPanel, eaContent.transform);

			// set the ID of the new panel
			newAddressPanel.id = DB.reader.GetInt32(0);

			// Position the new panel above the Add Address button
			newAddressPanel.transform.SetSiblingIndex(addAddressButton.transform.GetSiblingIndex());

			// add the panel to the list of spawned address panels
			spawnedAddressPanels.Add(newAddressPanel);

			// Fill in all of the input fields with data
			if(!string.IsNullOrWhiteSpace(DB.reader.GetString(1)))
            {
				newAddressPanel.street1InputField.text = DB.reader.GetString(1);
            }
			if (!string.IsNullOrWhiteSpace(DB.reader.GetString(2)))
			{
				newAddressPanel.street2InputField.text = DB.reader.GetString(2);
			}
			if (!string.IsNullOrWhiteSpace(DB.reader.GetString(3)))
			{
				newAddressPanel.suburbInputField.text = DB.reader.GetString(3);
			}
			if (!string.IsNullOrWhiteSpace(DB.reader.GetString(4)))
			{
				newAddressPanel.stateInputField.text = DB.reader.GetString(4);
			}
			if (!string.IsNullOrWhiteSpace(DB.reader.GetString(5)))
			{
				newAddressPanel.postcodeInputField.text = DB.reader.GetString(5);
			}
			if (!string.IsNullOrWhiteSpace(DB.reader.GetString(6)))
			{
				newAddressPanel.countryInputField.text = DB.reader.GetString(6);
			}
        }

		// close the DB
		DB.CloseDB();
    }

	void EditContact()
	{
		// Run a query thats updates the 'Person' table
		myQuery = "UPDATE Person SET " +
				  "FirstName = '" + firstNameInputField.text + 
				  "', LastName = '" + lastNameInputField.text + 
				  "', OtherName = '" + otherNameInputField.text + 
				  "', Nickname = '" + nicknameInputField.text + 
				  "', Company = '" + companyInputField.text + 
				  "', Department = '" + departmentInputField.text + 
				  "', JobTitle = '" + jobTitleInputField.text + 
				  "', DOB = '" + dobInputField.text +  
				  "' WHERE ID = " + personID + ";";
		RunMyQuery(myQuery);

		// Close the DB
		DB.CloseDB();

		// Loop through all of the detail panels
		foreach(DetailPanel detailPanel in spawnedDetailsPanels)
		{
			if(detailPanel.id != -1) // if the data is in the DB, and needs to be UPDATED
			{
				// Run a query to UPDATE the detail in the DB
				myQuery = "UPDATE Details SET Contact = '" + detailPanel.inputField.text + "' WHERE ID = " + detailPanel.id + ";";
				RunMyQuery(myQuery);
				DB.CloseDB();
			}
			else // if the data is not in the DB, and needs to be INSERTED
			{
				// Run a query to INSERT a new detail into the DB
				myQuery = "INSERT INTO Details (Contact, Type, Person) " +
						  "VALUES ('" + detailPanel.inputField.text + "', '" + detailPanel.type + "', " + personID + ");";
				RunMyQuery(myQuery);
				DB.CloseDB();
			}
		}

		// Loop through all of the address panels
		foreach(AddressPanel addressPanel in spawnedAddressPanels)
		{
			if(addressPanel.id != -1) // if the data is in the DB, and needs to be UPDATED
			{
				// Run a query to UPDATE the address in the DB
				myQuery = "UPDATE Address SET " +
						  "Street1 = '" + addressPanel.street1InputField.text + 
						  "', Street2 = '" + addressPanel.street2InputField.text + 
						  "', Suburb = '" + addressPanel.suburbInputField.text + 
						  "', State = '" + addressPanel.stateInputField.text + 
						  "', PostCode = '" + addressPanel.postcodeInputField.text + 
						  "', Country = '" + addressPanel.countryInputField.text + 
						  "' WHERE ID = " + addressPanel.id + ";";
				RunMyQuery(myQuery);
				DB.CloseDB();
			}
			else // if the data is not in the DB, and needs to be INSERTED
			{
				// Run a query to INSERT a new address into the DB
				myQuery = "INSERT INTO Address (Street1, Street2, Suburb, State, PostCode, Country, Person) " +
						  "VALUES ('" + addressPanel.street1InputField.text + "', '" 
									  + addressPanel.street2InputField.text + "', '" 
									  + addressPanel.suburbInputField.text + "', '" 
									  + addressPanel.stateInputField.text + "', '" 
									  + addressPanel.postcodeInputField.text + "', '" 
									  + addressPanel.countryInputField.text + "', " 
									  + personID + ");";
				RunMyQuery(myQuery);
				DB.CloseDB();
			}
		}

		// Loop through all of the details that the user wants to DELETE
		foreach(int detailID in detailsToDelete)
        {
			// Run a query to DELETE the detail from the DB
			myQuery = "DELETE FROM Details WHERE ID = " + detailID + ";";
			RunMyQuery(myQuery);
			DB.CloseDB();
        }

		// Loop through all of the address that the user wants to DELETE
		foreach(int addressID in addressesToDelete)
        {
			// Run a query to DELETE the address from the DB
			myQuery = "DELETE FROM Address WHERE ID = " + addressID + ";";
			RunMyQuery(myQuery);
			DB.CloseDB();
        }
	}

	void AddContact()
    {
		// Run a query that adds a new person to the DB
		myQuery = "INSERT INTO Person (FirstName, LastName, OtherName, Nickname, Company, Department, JobTitle, DOB) " +
				  "VALUES ('" + firstNameInputField.text + "', '"
							  + lastNameInputField.text + "', '"
							  + otherNameInputField.text + "', '"
							  + nicknameInputField.text + "', '"
							  + companyInputField.text + "', '"
							  + departmentInputField.text + "', '"
							  + jobTitleInputField.text + "', '"
							  + dobInputField.text + "');";
		RunMyQuery(myQuery);

		//close the DB
		DB.CloseDB();

		// Run a query to find the ID of the newly added Person
		myQuery = "SELECT MAX(ID) FROM Person;";
		RunMyQuery(myQuery);

		// check that there is data in the reader
		if (DB.reader.Read())
		{
			// set the personID to the ID of the newly added Person
			personID = DB.reader.GetInt32(0);
		}

		// Close the DB
		DB.CloseDB();

		// Loop through every detail panel
		foreach (DetailPanel panel in spawnedDetailsPanels)
		{
			// Run a query to insert the data into the DB
			myQuery = "INSERT INTO Details (Contact, Type, Person) " +
					  "VALUES ('" + panel.inputField.text + "', '" + panel.type + "', " + personID + ");";
			RunMyQuery(myQuery);

			// Close the DB
			DB.CloseDB();
		}

		// Loop through every address panel
		foreach (AddressPanel panel in spawnedAddressPanels)
		{
			// Run a query to insert the data into the DB
			myQuery = "INSERT INTO Address (Street1, Street2, Suburb, State, PostCode, Country, Person) " +
						  "VALUES ('" + panel.street1InputField.text + "', '"
									  + panel.street2InputField.text + "', '"
									  + panel.suburbInputField.text + "', '"
									  + panel.stateInputField.text + "', '"
									  + panel.postcodeInputField.text + "', '"
									  + panel.countryInputField.text + "', "
									  + personID + ");";
			RunMyQuery(myQuery);

			//Close the DB
			DB.CloseDB();
		}
	}

	bool InputIsValid()
    {
		// if the first name input field is blank
		if(string.IsNullOrWhiteSpace(firstNameInputField.text))
        {
			// Notify the user
			messageText.text = "First name is a required field";
			SwitchDarkOverlay();
			return false;
        }

		// temp list of details panels to be removed after the foreach loop
		List<DetailPanel> tempDetails = new List<DetailPanel>();

        // loop through all of the detail panels
        foreach (DetailPanel panel in spawnedDetailsPanels)
        {
            // if the detail panel is empty then it can be assumed that the user
            // doesn't want that data in the DB
            if (string.IsNullOrWhiteSpace(panel.inputField.text))
            {
                // If the detail panel contained data from the DB
                if (panel.id != -1)
                {
                    // add the id to the list of details to delete
                    detailsToDelete.Add(panel.id);
                }

				// Add this panel to the temp list containing details panels to be deleted
				tempDetails.Add(panel);

                // Destroy the panel
                Destroy(panel.gameObject);
            }
            else if (panel.type == "Phone") // If the detail panel is not empty and it is a Phone
            {
                // loop through every character in the phone number
                foreach (char character in panel.inputField.text)
                {
                    // if the character is not a digit
                    if (character < '0' || character > '9')
                    {
						//Notify the user
						messageText.text = "Phone numbers can only contain digits";
						SwitchDarkOverlay();
                        return false;
                    }
                }
            }
        }

		// Remove any details in the temp list
		foreach(DetailPanel panel in tempDetails)
        {
			spawnedDetailsPanels.Remove(panel);
        }

		// temp list of details panels to be removed after the foreach loop
		List<AddressPanel> tempAddresses = new List<AddressPanel>();

		// loop through all of the address panels
		foreach (AddressPanel panel in spawnedAddressPanels)
        {
            // if none of the address fields have anything in them
            if (string.IsNullOrWhiteSpace(panel.street1InputField.text)
            && string.IsNullOrWhiteSpace(panel.street2InputField.text)
            && string.IsNullOrWhiteSpace(panel.suburbInputField.text)
            && string.IsNullOrWhiteSpace(panel.stateInputField.text)
            && string.IsNullOrWhiteSpace(panel.postcodeInputField.text)
            && string.IsNullOrWhiteSpace(panel.countryInputField.text))
            {
				if(panel.id != -1) // if the address panel did contain data from the DB
                {
					// add the id to the list of addresses to delete
					addressesToDelete.Add(panel.id);
                }

				// Add this panel to the temp list containing address panels to be deleted
				tempAddresses.Add(panel);

				// Destroy the panel
				Destroy(panel.gameObject);
			}
			else if (!string.IsNullOrWhiteSpace(panel.postcodeInputField.text)) // if the postcode is not empty
            {
				// The Postcode is first validated via rules set on the InputField
				// that only allow alphanumeric characters, and a max of 10 characters

				// Validate the postcode further by changing any letters to Upper Case
				panel.postcodeInputField.text = panel.postcodeInputField.text.ToUpper();
            }
        }

		// Remove any addresses in the temp list
		foreach(AddressPanel panel in tempAddresses)
        {
			spawnedAddressPanels.Remove(panel);
        }

		// If the DOB is not empty
		if(!string.IsNullOrWhiteSpace(dobInputField.text))
		{
			// Stores the DateTime of the DOB if the date can be parsed
			DateTime parsedDate;

			// if the DOB the user has input can be parsed to a DateTime
			if(DateTime.TryParse(dobInputField.text, out parsedDate))
            {
				// Set the DOB to the Date of the parsedDate
				dobInputField.text = parsedDate.ToShortDateString();
            }
			else // if the date is invalid
            {
				// Notify the user
				messageText.text = "DOB must follow the format: dd/mm/yyy";
				SwitchDarkOverlay();
				return false;
            }
        }

        // if the users data has passed all of the validation checks, then return true
        return true;
    }

	public void SaveButton()
    {
		// if the user has input invalid data, then exit this method
		if(!InputIsValid()) { return; }

		if (isEditScreen) // Edit Contact Details
		{
			EditContact();
		}
		else // Add New Contact
		{
			AddContact();
		}

		// View the contact
		MenuGoTo(1);
	}

	public void CancelButton()
    {
		if(isEditScreen)
		{
			MenuGoTo(1);
		}
		else
        {
			MenuGoTo(0);
        }
    }

	public void AddPhoneButton()
    {
		// Instatiate a new detail panel
		DetailPanel newPanel = Instantiate(editDetailPanel, eaContent.transform);
		// Set the type to Phone
		newPanel.type = "Phone";
		// Position the panel above the Add Phone Button
		newPanel.transform.SetSiblingIndex(addPhoneButton.transform.GetSiblingIndex());
		// Add the panel to the list of spawned Details Panels
		spawnedDetailsPanels.Add(newPanel);
		// Set the placeholder to 'phone number'
		newPanel.placeholder.text = "phone number";
    }

	public void AddEmailButton()
    {
		// Instatiate a new detail panel
		DetailPanel newPanel = Instantiate(editDetailPanel, eaContent.transform);
		// Set the type to Email
		newPanel.type = "Email";
		// Position the panel above the Add Email button
		newPanel.transform.SetSiblingIndex(addEmailButton.transform.GetSiblingIndex());
		// Add the panel to the list of spawned Details Panels
		spawnedDetailsPanels.Add(newPanel);
		// Set the font size to 45
		newPanel.inputField.textComponent.fontSize = 45f;
		// Set the placeholder to 'email address'
		newPanel.placeholder.text = "email address";
	}

	public void AddAddressButton()
    {
		// Instantiate a new address panel
		AddressPanel newPanel = Instantiate(editAddressPanel, eaContent.transform);
		// Position the panel above the Add Address Button
		newPanel.transform.SetSiblingIndex(addAddressButton.transform.GetSiblingIndex());
		// Add the panel to the list of spawned Address Panels
		spawnedAddressPanels.Add(newPanel);
    }

	public void DeleteContact()
    {
		// Delete all details related to the Person
		myQuery = "DELETE FROM Details WHERE Person = " + personID + ";";
		RunMyQuery(myQuery);
		Debug.Log("My Query = " + myQuery);

		// Delete all addresses related to the Person
		myQuery = "DELETE FROM Address WHERE Person = " + personID + ";";
		RunMyQuery(myQuery);
		Debug.Log("My Query = " + myQuery);

		// Delete the Person from the DB
		myQuery = "DELETE FROM Person WHERE ID = " + personID + ";";
		RunMyQuery(myQuery);
		Debug.Log("My Query = " + myQuery);

		// Go back to the main screen
		MenuGoTo(0);
	}

	void ClearScreenData()
    {
		// destroy all of the contact panels on the main screen
		foreach(Transform panel in msContent.transform)
        {
			Destroy(panel.gameObject);
        }

		// clear all of the textboxes on the view contact screen
		nameText.text = "";
		nicknameText.text = "";
		companyText.text = "";
		departmentText.text = "";
		jobTitleText.text = "";
		dobText.text = "";

		// disable all of the labels and textboxes on the view contact screen
		nicknameText.gameObject.SetActive(false);
		companyLabel.SetActive(false);
		companyText.gameObject.SetActive(false);
		departmentLabel.SetActive(false);
		departmentText.gameObject.SetActive(false);
		jobTitleLabel.SetActive(false);
		jobTitleText.gameObject.SetActive(false);
		phoneLabel.SetActive(false);
		emailLabel.SetActive(false);
	    addressLabel.SetActive(false);
		dobLabel.SetActive(false);
		dobText.gameObject.SetActive(false);

		// Destroy all of the spawned objects on the view contact screen
		foreach(GameObject obj in vsSpawnedObjects)
        {
			Destroy(obj);
        }

		// Clear the list containing the spawned objects on the view contact screen
		vsSpawnedObjects.Clear();

		// clear all of the input fields in the edit/add screens
		firstNameInputField.text = "";
		lastNameInputField.text = "";
		otherNameInputField.text = "";
		nicknameInputField.text = "";
		companyInputField.text = "";
		departmentInputField.text = "";
		jobTitleInputField.text = "";
		dobInputField.text = "";

		// Destory all of the spawned panels on the edit/add screens
		foreach(DetailPanel panel in spawnedDetailsPanels)
        {
			Destroy(panel.gameObject);
        }
		foreach(AddressPanel panel in spawnedAddressPanels)
        {
			Destroy(panel.gameObject);
        }

		// clear the lists containing the spawned detail and address panels
		spawnedDetailsPanels.Clear();
		spawnedAddressPanels.Clear();

		// clear the lists containing ID's to delete
		detailsToDelete.Clear();
		addressesToDelete.Clear();

		// disable the Confirmation Overlay
		darkOverlay.SetActive(false);

		// enable the OK button
		okButton.SetActive(true);
    }

	public void DeleteContactButton()
    {
		messageText.text = "Are you sure you want to delete this contact?";
		SwitchDarkOverlay();
		okButton.SetActive(false);
    }

	public void SwitchDarkOverlay()
    {
		darkOverlay.SetActive(!darkOverlay.activeSelf);
		okButton.SetActive(true);
    }

	void RunMyQuery(string myQuery)
	{
		//set up the path to the db file
		string dbPath = "URI=file:"
			+ Path.Combine(Application.streamingAssetsPath, "productivity.db");

		//set up and open the connection to the database
		DB.Connect(dbPath);

		//run the query the fill the data reader 
		DB.RunQuery(myQuery);
	}
}