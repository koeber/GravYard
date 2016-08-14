using UnityEngine;
using System.Collections;
//using ExtensionMethods;

public class HookShotHook : MonoBehaviour {

    /*
    *   Attach to hook gameObject, detects hook collisions and assigns that object as parent of hook
    *
    *   requires character object be assigned to myChar object
    *       if myChar not assigned, will try to assign it to 
    *   also anything that shouldn't be considered for the hook collision (like the rope) should be tagged with "no_collision"
    *
    *   ExtensionMethods.cs is used for asVector3() and toStringPrecise(). 
    */

    public GameObject myChar;

    private Vector2 lastPos;
    private float lastDistanceToChar = 0;

	// Use this for initialization
	void Start () {
        lastPos = this.transform.position;
        if (myChar == null)
        {   
            myChar = GameObject.Find("CharacterRobotBoy");
        }
    }
	
	// Update is called once per frame
	void Update () {
        //debugging
        /*if (this.transform.parent == null)
        {
            Debug.Log("hook is ein orphan");
        }
        else
        {
            Debug.Log("hook's parent is now " + this.transform.parent.name);
        }*/



        //temporarily removing this part, it would hold the rope down to a certain distance
        /*if (Input.GetKey("w") && myChar.GetComponent<HookShot>().getHookState() == HookShot.HOOK_HOOKED && lastDistanceToChar != 0f)
        {
            //float lastDistance = Vector2.Distance(lastPos, myChar.transform.position);
            float thisDistance = Vector2.Distance(this.transform.position, myChar.transform.position);
            if (thisDistance > lastDistanceToChar)
            {
                Vector2 newPos = this.transform.position - myChar.transform.position;
                Vector2.ClampMagnitude(newPos, lastDistanceToChar);
                this.transform.position = myChar.transform.position + newPos.asVector3();
            }
            lastDistanceToChar = thisDistance;
        }
        else
        {
            lastDistanceToChar = Vector2.Distance(this.transform.position, myChar.transform.position);
        }
        lastPos = this.transform.position;*/
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //make sure the hook is being thrown, and the object it collided with isn't the player or the rope 
        if (myChar.GetComponent<HookShot>().getHookState() == HookShot.HOOK_THROWN && other != myChar && !other.transform.CompareTag("no_collision")) {
            
            Debug.Log("hit object: " + other.ToString());
            Debug.Log("hit object with name: " + other.name);

            // Disable collisions with the object being attached
            this.GetComponent<Collider2D>().enabled = false;


            // Don't allow physics to affect the object
            this.GetComponent<Rigidbody2D>().isKinematic = true;
        

            // Attach hook to object it collided with
            this.transform.parent = other.transform;
            Debug.Log("just assigned new parent");
        }
    }

    
}
