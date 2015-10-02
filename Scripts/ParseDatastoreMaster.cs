using UnityEngine;
using System.Collections;
using Parse;
using System.Threading.Tasks;
using UnityEngine.UI;

public class ParseDatastoreMaster : MonoBehaviour {

	public UserAddPanel addPanel;
	public UserLoginPanel loginPanel;
	public RectTransform logoutPanel;

	public RectTransform blockPanel;

	public Button loginButton;

	public bool updateLoginStateFlag = true;

	public CommentDelegateParseStore commentDelegate;

	// Use this for initialization
	void Start () {
		
		UpdateLoginState ();

	}

	public void UpdateLoginState(){
		addPanel.gameObject.SetActive (false);
		loginPanel.gameObject.SetActive (false);
		blockPanel.gameObject.SetActive (false);
		logoutPanel.gameObject.SetActive (false);
		
		if (ParseUser.CurrentUser != null && ParseUser.CurrentUser.IsAuthenticated) {
			loginButton.GetComponentInChildren<Text> ().text = "Welcome: " + ParseUser.CurrentUser.Username;
		} else {
			loginButton.GetComponentInChildren<Text> ().text = "Login";
		}
		
		loginPanel.username.text = "";
		loginPanel.password.text = ""; 
		addPanel.username.text = "";
		addPanel.password.text = "";
		addPanel.email.text = "";

		commentDelegate.LoadData ();
	}
	
	// Update is called once per frame
	void Update () {
		if (updateLoginStateFlag) {
			UpdateLoginState ();
			updateLoginStateFlag = false;
		}
	}

	public void LoginButtonClicked(){

		blockPanel.gameObject.SetActive (true);

		if (ParseUser.CurrentUser != null && ParseUser.CurrentUser.IsAuthenticated) {
			logoutPanel.gameObject.SetActive(true);
		} else {
			loginPanel.gameObject.SetActive(true);
		}
	}

	public void Dismiss(){
		logoutPanel.gameObject.SetActive(false);
		addPanel.gameObject.SetActive (false);
		loginPanel.gameObject.SetActive (false);
		blockPanel.gameObject.SetActive (false);
	}


	public void StoreTestData(){
		ParseObject testObject = new ParseObject("TestObject");
		testObject["foo"] = "bar";
		testObject.SaveAsync();
	}

	public Task StoreData(ParseObject obj){
		if (ParseUser.CurrentUser != null)
		{
			obj["user"] = ParseUser.CurrentUser;
			// do stuff with the user
			return obj.SaveAsync();
		}
		else
		{
			return null;
		}

	}

	public void Logout(){
		Dismiss ();
		ParseUser.LogOutAsync().ContinueWith(t=>
		{
			if (t.IsFaulted || t.IsCanceled) {
				// The login failed. Check the error to see why.
				print ("logout failed");
				print (t.Exception);
				
				updateLoginStateFlag = true;
			} else {
				// Login was successful.
				print ("logout success");
				
				updateLoginStateFlag = true;
				
			}
		});
	}

	public void Login(){
		Login (loginPanel.username.text, loginPanel.password.text);
	}


	public void Login(string name, string password){
		Dismiss ();
		ParseUser.LogInAsync (name, password).ContinueWith (t =>
		{
			if (t.IsFaulted || t.IsCanceled) {
				// The login failed. Check the error to see why.
				print ("login failed");
				
				updateLoginStateFlag = true;
			} else {
				// Login was successful.
				print ("login success");

				updateLoginStateFlag = true;

			}
		});
	}
	
	public void AddUser(){
		AddUser (addPanel.username.text, addPanel.password.text, addPanel.email.text);
	}
	public void AddUser(string name, string password, string email){
		Dismiss ();
		var user = new ParseUser()
		{
			Username = name,
			Password = password,
			Email = email
		};

		user.SignUpAsync().ContinueWith (t =>
		{
			if (t.IsFaulted || t.IsCanceled)
			{
				// The login failed. Check the error to see why.
				print ("signup failed");
				
				updateLoginStateFlag = true;
			}
			else
			{
				// Login was successful.
				print ("signup success");
				
				updateLoginStateFlag = true;
			}
		});


	}
}
