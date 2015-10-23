using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Parse;
using System;

public class UserAddPanel : MonoBehaviour {


	public InputField username;
	public InputField password;
	public InputField email;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void AddUser(ParseDatastoreMaster pdm ){

		pdm.Dismiss ();

		var user = new ParseUser()
		{
			Username = username.text,
			Password = password.text,
			Email = email.text
		};


		user.SignUpAsync().ContinueWith (t =>
		                                 {
			if (t.IsFaulted || t.IsCanceled)
			{
				// The login failed. Check the error to see why.
				print ("signup failed!");
				print (t.Exception.Message);
				pdm.updateLoginStateFlag = true;
			}
			else
			{
				// Login was successful.
				print ("signup success");
				pdm.updateLoginStateFlag = true;
			}
		});
		
		
	}

}
