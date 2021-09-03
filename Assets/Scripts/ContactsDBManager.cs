using System.Collections;
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
				
	// edit/add contact screen
	[Header("Edit/Add Screen")]
	[SerializeField] GameObject deleteContactButton;
				
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

			// display the contact name (First name, Other name, Last name)
			panel.contactNameText.text = DB.reader.GetString(1) + " " +
										 DB.reader.GetString(3) + " " +
										 DB.reader.GetString(2);
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
				// fill in existing contact details

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

	public void AddButton()
    {
		Debug.Log("Add Button Pressed");
    }

	public void SaveButton()
    {
		// Validate the data

		if(isEditScreen) // Edit Contact Details
        {
			
        }
		else // Add New Contact
        {

        }
    }

	void ClearScreenData()
    {
		// destroy all of the panels on the main screen
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

		// Clear the list containing the spawned objects
		vsSpawnedObjects.Clear();
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
