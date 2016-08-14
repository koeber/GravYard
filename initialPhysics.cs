using UnityEngine;
using System.Collections;

public class initialPhysics : MonoBehaviour {

    /*
    *   attach to objects to give them initial force
    */

    public float initialForceX = 0;
    public float initialForceY = 0;

	// Use this for initialization
	void Start () {
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(initialForceX, initialForceY));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
