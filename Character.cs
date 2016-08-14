using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
	// Use this for initialization
	public Rigidbody2D myrigidbody2D;
	public GameObject mystandingobject;
	public float torquemultiplier = 1f;
	public float jumpforce;
	public bool isFacingRight = true;
	// Update is called once per frame

	public void Move (float h,  bool jump){
		if (mystandingobject == null) {
			if (h != 0) {
				myrigidbody2D.AddTorque (h*myrigidbody2D.mass*torquemultiplier);
			}
			if (jump) {
				myrigidbody2D.drag = 100;
				myrigidbody2D.angularDrag = 100;
			} else {
				myrigidbody2D.drag = 0;
				myrigidbody2D.angularDrag = 10;
			}
		} else {
			if (mystandingobject.tag == "circularObject") {
				if (h != 0) {
					//Debug.Log (mystandingobject.transform.position + "," + Vector3.forward + "," + (h * 5) / mystandingobject.transform.lossyScale.z);
					transform.RotateAround (mystandingobject.transform.position, Vector3.forward, (-h * 2) / mystandingobject.transform.lossyScale.z);

				}
			}
			else if (mystandingobject.tag == "flatObject"){
				}
		

			if (jump) {
				myrigidbody2D.AddRelativeForce (new Vector2 (0, jumpforce*myrigidbody2D.mass));
				mystandingobject = null;
			}
		}
	}
	public void Kill(){
		Debug.Log("DEAD");
	}

	public void Update(){
		if (mystandingobject != null) {
			myrigidbody2D.AddForce (Vector2.ClampMagnitude(new Vector2((mystandingobject.transform.position.x-transform.position.x), (mystandingobject.transform.position.y-transform.position.y)),.01f));
			mystandingobject.GetComponent<Rigidbody2D>().AddForce (Vector2.ClampMagnitude(new Vector2((transform.position.x-mystandingobject.transform.position.x), (transform.position.y-mystandingobject.transform.position.y)),.01f));
			Lookat2D(mystandingobject);
		}
	}
	public void Lookat2D(GameObject Target){
		Vector3 dir = Target.transform.position - transform.position;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
	}
}
