using UnityEngine;
using System.Collections;

public class bodyCollisionDetect : MonoBehaviour {
	public Character myCharacter;
	//This script determines whether the character is standing,
	//and what the character is standing on.
	void OnCollisionEnter2D(Collision2D coll){
		Debug.Log ("COLLISION");
		myCharacter.mystandingobject = coll.gameObject;
		coll.gameObject.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		//myCharacter.gameObject.transform.SetParent (coll.gameObject.transform);

		
	}

}
