using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ContactsDBManager : MonoBehaviour {

	// screens
	[Header("Screens")]
	[SerializeField] GameObject mainScreen;
	[SerializeField] GameObject viewContactScreen;
	[SerializeField] GameObject editAddScreen;

	// main screen
	[Header("Main Screen Variables")]
	[SerializeField] GameObject contactPanel;
	[SerializeField] GameObject msContent;

	// view contact screen

	// edit/add contact screen
	[Header("Edit/Add Screen Variables")]
	[SerializeField] GameObject deleteContactButton;

	// class level variables
	int personID = -1;
	string myQuery;
	bool isEditScreen = false;

	// Use this for initialization
	void Start () 
	{
		DisplayContactPanels();
	}

	void DisplayContactPanels()
    {
		// Get the contacts IDs and names
		myQuery = "SELECT ID, FirstName, LastName, OtherName FROM Person;";
		RunMyQuery(myQuery);
		Debug.Log("My Query = " + myQuery);

		// Create a contact panel for each result in the reader
		while(DB.reader.Read())
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
