using UnityEngine;
using System.Collections;
using Shiva.Commenting;
using Parse;
using System.Threading.Tasks;
using System.Collections.Generic;

public class CommentDelegateParseStore : MonoBehaviour {

	public CommentingMaster master;
	public ParseDatastoreMaster parseMaster;

	private IEnumerable<ParseObject> pos;
	private bool dataLoad;

	private Vector3 ToVec3(string str){
		string[] strs = str.Split (',');
		return new Vector3 (float.Parse (strs [0]), float.Parse (strs [1]), float.Parse (strs [2]));
	}

	// Use this for initialization
	void Start () {
		master.onCommentAdded += OnCommentAdded;
		master.onCommentDeleted += OnCommentDeleted;
	}

	public void LoadData(){
		ParseObject.GetQuery ("CommentData").Limit (100).OrderByDescending ("updatedAt").FindAsync ().ContinueWith (t =>
		                                                                                                            {
			if (t.IsFaulted || t.IsCanceled) {
				// The login failed. Check the error to see why.
				print ("load failed");
				print(t.Exception);
			} else {
				// Login was successful.
				print ("save success");
				pos = t.Result;
				dataLoad = true;
			}
		});
	}
	
	// Update is called once per frame
	void Update () {
		if (dataLoad) {
			master.DeleteAll();

			foreach(ParseObject po in pos){

				CommentingMaster.CommentData cd = new CommentingMaster.CommentData();
				cd.objectId = po.ObjectId;
				cd.comment = (string)po["comment"];
				cd.loc = ToVec3((string)po["position"]);
				
				CommentObject co = master.AddComment(cd);

				ParseUser pu = po["user"] as ParseUser;
				if(ParseUser.CurrentUser == null || pu.ObjectId != ParseUser.CurrentUser.ObjectId){

					Renderer[] rs = co.GetComponentsInChildren<Renderer>();
					foreach(Renderer r in rs){
						if(r.gameObject.name == "Sphere"){
							r.material.color = Color.gray;
						}
					}

					Collider[] cs = co.GetComponentsInChildren<Collider>();
					foreach(Collider c in cs){
						c.enabled = false;
					}

				}
			}

			dataLoad = false;
		}
	
	}

	public void OnCommentAdded (CommentObject co){
		CommentingMaster.CommentData data = CommentingMaster.ToCommentData (co);
		print ("Added");
		print (data.comment);
		
		ParseObject po = new ParseObject("CommentData");
		po["comment"] = data.comment;
		po["position"]=""+data.loc.x+","+data.loc.y+","+data.loc.z;
		if(co.objectId.Length > 0)
			po.ObjectId = co.objectId;

		Task task = parseMaster.StoreData (po);
		if (task != null) {
			task.ContinueWith(t=>
			{
				if (t.IsFaulted || t.IsCanceled) {
					// The login failed. Check the error to see why.
					print ("save failed");
					print(t.Exception);
				} else {
					// Login was successful.
					print ("save success");
					print(po.ObjectId);
					co.objectId = po.ObjectId;
				}
			});
		}

	}

	public void OnCommentDeleted (CommentObject co){
		CommentingMaster.CommentData data = CommentingMaster.ToCommentData (co);
		print ("Delete");
		print (data.comment);
	}
}
