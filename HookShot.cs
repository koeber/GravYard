using UnityEngine;
using System.Collections;

// PrototypeWhite4x1 is actually 1.25 x 3.3333333333 wtf

/// <summary>
/// Requirements for HookShot to work:
/// Hierarchy has:
///     *"hook_rope" object
///     and tagged with "no_collision"
///         *child  object with collider2d(isTrigger==true) & renderer
///             
///     *"hook" object with collider2d, renderer, rigidbody2d, and *HookShotHook*
/// 
///     *character object with collider2d, rigidbody, and *HookShot*
///        
///         
/// </summary>






public class HookShot : MonoBehaviour {

    public const int HOOK_HOLSTERED = -1;
    public const int HOOK_HELD = 0;
    public const int HOOK_THROWN = 1;
    public const int HOOK_HOOKED = 2;
    public const int HOOK_PULLED_ATTACHED = 3;
    public const int HOOK_PULLED_EMPTY = 4;
    private int hookState;
    public GameObject myHook;
    public GameObject myRope;
    public float hookShootPower = 5.1f;
    public float hookPullPower = 0.1f;
    public float ropePrefabX = 5.5f;
    public float ropePrefabY = .66666666666666f;

    private float lastDistanceToHook = 0;
    private Vector2 lastCharPos;


    // Use this for initialization
    void Start()
    {
        hookState = HOOK_HOLSTERED;
        myHook = GameObject.Find("hook");
        myRope = GameObject.Find("hook_rope");
        lastCharPos = new Vector2(0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("^^^^^^^^^^^^^^^^^^^^^^^^^^^State:^^^^^^^: " + hookState);

        switch (hookState){
            case HOOK_HOLSTERED:
                if (Input.GetMouseButton(0))
                {
                    hookState = HOOK_HELD;
                    myHook.GetComponent<SpriteRenderer>().enabled = true;
                    myRope.GetComponentInChildren<SpriteRenderer>().enabled = true;
                }
                else
                {
                    myHook.GetComponent<SpriteRenderer>().enabled = false;
                    myRope.GetComponentInChildren<SpriteRenderer>().enabled = false;
                }
                break;
            case HOOK_HELD:
                if (Input.GetMouseButton(2))
                {   //right click to cancel
                    hookState = HOOK_HOLSTERED;
                }
                else
                {
                    if (Input.GetMouseButton(0))
                    {   //still holding
                        hookAim();
                    }
                    else
                    {   //released
                        throwHook(hookShootPower);
                        hookState = HOOK_THROWN;
                    }
                }
                break;
            case HOOK_THROWN:
                if (myHook.transform.parent != null)
                {   //hook is attached
                    Debug.Log("setting HookState to hooked, parent is " + myHook.transform.parent.name);
                    hookState = HOOK_HOOKED;
                }
                if (Input.GetKey("e"))
                {   //hook is pulled back
                    Debug.Log("e pressed while hook not hooked");
                    pullHook(hookPullPower);
                }
                break;
            case HOOK_HOOKED:
                if (Input.GetKey("e"))
                {   //hook pulled back
                    Debug.Log("e pressed while hook hooked");
                    pullHook(hookPullPower);
                }
                if (Input.GetKey("q"))
                {   //unhook hook
                    unHookHook();
                    hookState = HOOK_HOLSTERED;
                    //hookState = HOOK_PULLED_EMPTY;
                    //pullHook(hookPullPower * 20f);
                }
                if (Input.GetKey("w"))
                {   //clamp rope
                    clampRope();
                }
                if (isHookRetracted())
                {
                    hookState = HOOK_HOLSTERED;
                }
                break;
            case HOOK_PULLED_EMPTY:
                pullHook(hookPullPower/30f);
                if (isHookRetracted())
                {
                    hookState = HOOK_HOLSTERED;
                }
                break;
            default:
                break;
        }

        //adjust rope
        if (hookState != HOOK_HOLSTERED)
        {
            drawRope();
        }



        if (Input.GetMouseButtonDown(1))
            Debug.Log("Pressed right click.");

        if (Input.GetMouseButtonDown(2))
            Debug.Log("Pressed middle click.");
    }

    public float getHookState()
    {
        return hookState;
    }


    //what is this function supposed to do? why is it here?
    /*void OnTriggerEnter(Collider other)
    {
        if (hookState == HOOK_THROWN)
        {   //might take away collider stuff while hook isn't in this state later, so the if may not be nessesary
            myHook.transform.SetParent(other.transform);
            Debug.Log("onTrigger activated");
            //myHook.transform.rigidbody2D.
        }
    }*/

    /// <summary>
    /// if you press "/" three times it automatically makes this
    /// but this sets the rope's position using the hook/character's position
    /// </summary>
    public void drawRope()
    {
        Vector2 charPos = this.transform.position;
        Vector2 hookPos = myHook.transform.position;

        //position
        Vector2 middlePos = (charPos + hookPos) / 2f;
        myRope.transform.position = middlePos;

        //rope scale
        float myDistance = Vector3.Distance(hookPos, charPos);
        //Debug.Log("ropeDistance: " + myDistance);
        myRope.transform.localScale = new Vector2(myDistance / ropePrefabX, .2f);

        //rope rotation
        float myAngle = Mathf.Atan2(hookPos.y - charPos.y, hookPos.x - charPos.x) * 180 / Mathf.PI;
        //Debug.Log("angle: " + myAngle + " when charPos=" + charPos.ToString() + " and hookPos=" + hookPos.ToString());
        myRope.transform.eulerAngles = new Vector3(0f, 0f, myAngle);
    }

    public void hookAim()
    {
        //get mousePos as world vector
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        //Debug.Log("MouseWorldPoint: " + mousePos.ToString());

        //get difference between mousePos and character
        Vector3 mouseCharDiff = mousePos - this.GetComponent<Transform>().position;
        mouseCharDiff.z = 0;
        //Debug.Log("mouseCharDiff(unchanged):" + mouseCharDiff.ToString());

        //scale difference and apply to hook position
        mouseCharDiff.Normalize();
        //mouseCharDiff.Scale(new Vector3(3f, 3f, 3f));         //keep
        //Debug.Log("thisPosition(un) + mouseCharDiff ==:" + this.GetComponent<Transform>().position.ToString() + " + " + mouseCharDiff.ToString());
        myHook.GetComponent<Transform>().position = this.GetComponent<Transform>().position + mouseCharDiff;
    }

    public void throwHook(float magnitude)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector3 mouseCharDiff = mousePos - this.GetComponent<Transform>().position;
        mouseCharDiff.z = 0;
        mouseCharDiff.Normalize();
        myHook.GetComponent<Rigidbody2D>().AddForce(new Vector2(mouseCharDiff.x * magnitude, mouseCharDiff.y * magnitude));
    }

    private void pullHook(float magnitude)
    {
        float charMass = this.GetComponent<Rigidbody2D>().mass;
        Vector2 charPos = this.transform.position;
        float otherMass;
        Vector2 otherPos;
        Rigidbody2D otherRB3d;
        if (myHook.transform.parent != null)
        {   //if hook is hooked
            otherMass = myHook.GetComponentInParent<Rigidbody2D>().mass;
            otherPos = myHook.GetComponentInParent<Transform>().position;
            otherRB3d = myHook.GetComponentInParent<Rigidbody2D>();
        }
        else
        {   //if hook isn't hooked, you still gotta pull it back
            otherMass = myHook.GetComponent<Rigidbody2D>().mass;
            otherPos = myHook.GetComponent<Transform>().position;
            otherRB3d = myHook.GetComponent<Rigidbody2D>();
        }
        float totalMass = otherMass + charMass;

        //apply force to other
        Vector2 forceForOther = charPos - otherPos;
        forceForOther.Normalize();
        forceForOther.Scale(new Vector2(magnitude, magnitude));     //I feel like magnitude should be divided by 2 since the force is split between objects, but magnitude is just a random var so it shouldn't really matter
        otherRB3d.AddForce(forceForOther);

        Debug.Log("forceforeother:" + forceForOther);

        //apply force to character
        Vector2 forceForChar = otherPos - charPos;
        forceForChar.Normalize();
        forceForChar = forceForChar * (magnitude);                  //I feel like magnitude should be divided by 2 since the force is split between objects, but magnitude is just a random var so it shouldn't really matter
        this.GetComponent<Rigidbody2D>().AddForce(forceForChar);        

        Debug.Log("forceforeChar:" + forceForChar);
    }
    
    private bool isHookRetracted()
    {
        Vector2 charPos = this.transform.position;
        Vector2 otherPos = myHook.GetComponentInParent<Transform>().position;               //use the hook instead of the parent, won't work with big objects otherwise
        Vector2 distanceToHook = charPos - otherPos;
        
        if (distanceToHook.magnitude < 1.0)
        {
            unHookHook();

            return true;
        }
        return false;
    }

    private void unHookHook()
    {
        myHook.GetComponent<Collider2D>().enabled = true;


        // allow physics to affect the object
        myHook.GetComponent<Rigidbody2D>().isKinematic = false;


        // Attach object 1 to object 2
        myHook.transform.parent = null;
        myHook.transform.localScale = new Vector3(.1f, .1f, .1f);
        myHook.transform.rotation = Quaternion.identity;
        //Debug.Log("hook's parent is now " + myHook.transform.parent.name);
    }

    private void clampRope()
    {
        Debug.Log("starting clampRope Method");
        if (lastDistanceToHook > 0f)
        {
            Debug.Log("lastdistance > 0");
            //float lastDistance = Vector2.Distance(lastPos, myChar.transform.position);
            float thisDistance = Vector2.Distance(this.transform.position, myHook.transform.position);
            if (thisDistance > lastDistanceToHook)
            {
                Vector2 newPos = myHook.transform.position - this.transform.position;
                Debug.Log("inital diff: " + newPos);
                newPos = Vector2.ClampMagnitude(newPos, lastDistanceToHook);
                Debug.Log("initialCharPos: " + this.transform.position.ToString());
                Debug.Log("clamped newPos: " + newPos.ToString());
                newPos = myHook.transform.position - newPos.asVector3();

                //use three points to determine new velocity
                //Vector2 oldVelocity = this.GetComponent<Rigidbody2D>().velocity;
                //Vector2 oldVelocity = this.GetComponent<Rigidbody2D>().v
                //myHook.transform.
                //
                //newPos.no

                //so just set velocity to normalized(vector between old and new points) * magnitude of velocity?
                Vector2 newVelVector = newPos - lastCharPos;
                newVelVector.Normalize();
                Debug.Log("normalized vector between old and new" + newVelVector);
                Debug.Log("old Velocity: " + this.GetComponent<Rigidbody2D>().velocity.toStringPrecise() + " has magnitude of " + this.GetComponent<Rigidbody2D>().velocity.magnitude);
                
                float velocityMag = this.GetComponent<Rigidbody2D>().velocity.magnitude;
                Debug.Log("new velocity: " + (velocityMag * newVelVector));
                this.GetComponent<Rigidbody2D>().velocity = velocityMag * newVelVector;
                Debug.Log("new velocity(assignment check): " + GetComponent<Rigidbody2D>().velocity.toStringPrecise() + " has magnitude of " + this.GetComponent<Rigidbody2D>().velocity.magnitude);




                this.transform.position = newPos;
                Debug.Log("lastDist " + lastDistanceToHook + ", thisDist: " + thisDistance + ", CharPos: " + this.transform.position.ToString() + 
                    ", hookPos: " + myHook.transform.position);
                lastCharPos = this.transform.position;

            }
            else
            {   //if getting closer, update the lastDistance
                lastDistanceToHook = thisDistance;
                lastCharPos = this.transform.position;
            }

            
        }
        else
        {
            lastDistanceToHook = Vector2.Distance(this.transform.position, myHook.transform.position);
            lastCharPos = this.transform.position;
        }
    }
}
