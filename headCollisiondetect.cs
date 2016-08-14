using UnityEngine;
using System.Collections;

public class headCollisionDetect : MonoBehaviour {
	public Character myCharacter;

	void OnCollisionEnter2D(Collision2D coll){
		myCharacter.Kill();

	}
}


