using UnityEngine;
using System.Collections;


/*
GlobalPhysics:
1) attach to main camera
2) works with objects that have the rigidbody2d and transform components
3) turn gravity scale to 0 in the rigidbody2d component on all objects you want to work with this
4) use the gravConstant variable to adjust gravity
5) use mass on the rigidbody2d to adjust how much pull an object has
6) for some reason, with the robotboy prefabs, it only works in the y direction and occasionally gives divide by 0 errors. 


    */





public class GlobalPhysics : MonoBehaviour {

    public float gravConstant = 1;

    private static GameObject[] allMySpaceTrash;
    private static Transform[] allMySpaceTrans;
    //private static ObjectPhysics[] allMySpacePhysics;

    // Use this for initialization
    void Start()
    {
        allMySpaceTrash = FindObjectsOfType<GameObject>();
        for (int i = 0; i < allMySpaceTrash.Length; i++)
        {
            if (allMySpaceTrash[i].GetComponent<Rigidbody2D>() == null)
            {
                allMySpaceTrash[i] = null;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        Transform myTransform;
        //ObjectPhysics myPhysics;
        Rigidbody2D myBody;
        float[] allMyForcesX = new float[allMySpaceTrash.Length];
        float[] allMyForcesY = new float[allMySpaceTrash.Length];
        for (int i = 0; i < allMySpaceTrash.Length; i++)
        {
            if (allMySpaceTrash[i] != null)
            {
                myTransform = allMySpaceTrash[i].GetComponent<Transform>();
                myBody = allMySpaceTrash[i].GetComponent<Rigidbody2D>();
                for (int j = 0; j < allMySpaceTrash.Length; j++)
                {

                    if (i != j && allMySpaceTrash[j] != null) //if the other object isn't the same, and both exist
                    {
                        Transform yourTransform = allMySpaceTrash[j].GetComponent<Transform>();
                        Rigidbody2D yourBody = allMySpaceTrash[j].GetComponent<Rigidbody2D>();

                        float xDiff = (myTransform.position.x - yourTransform.position.x);
                        float yDiff = (myTransform.position.y - yourTransform.position.y);
                        //check for object having the same x or y coordinate, otherwise divide by 0 errors
                        if (xDiff == 0) { xDiff = .000001f; }
                        if (yDiff == 0) { yDiff = .000001f; }

                        float xDir = (xDiff / Mathf.Abs(xDiff)) * -1;
                        float yDir = (yDiff / Mathf.Abs(yDiff)) * -1;

                        //(myTransform.position.x - yourTransform.position.x)
                        float a2plusb2 = xDiff * xDiff + yDiff * yDiff;

                        //Debug.Log("pathagerusss:" + a2plusb2);

                        float totalForce = (yourBody.mass * myBody.mass) / Mathf.Sqrt(a2plusb2);
                        
                        float massProportion = .5f;// yourBody.mass / (yourBody.mass + myBody.mass);            hmmmmmm
                        //allMyForcesX[i] += totalForce * massProportion * ((yDiff * yDiff) / a2plusb2) * xDir;
                        //allMyForcesY[i] += totalForce * massProportion * ((xDiff * xDiff) / a2plusb2) * yDir;
                        allMyForcesX[i] += totalForce * massProportion * ((xDiff * xDiff) / a2plusb2) * xDir;
                        allMyForcesY[i] += totalForce * massProportion * ((yDiff * yDiff) / a2plusb2) * yDir;
                        //Debug.Log("tf:" + totalForce + " xDiff:" + xDiff + " a2+b2:" + a2plusb2 + " xDir:" + xDir);
                        //Debug.Log("amfX:" + allMyForcesX[i]);
                        if (float.IsNaN(allMyForcesX[i]) || float.IsNaN(allMyForcesY[i]))
                        {
                            Debug.Log("+X == " + allMyForcesX[i]);
                            Debug.Log("So tf=" + totalForce + " mp=" + massProportion + " xdif=" + xDiff + " a2b2=" + a2plusb2 + " xdir=" + xDir);
                            Debug.Log("+Y == " + allMyForcesY[i]);
                        }
                    }
                }
            }

        }
        //apply calculated forces
        for (int i = 0; i < allMySpaceTrash.Length; i++)
        {
            
            if (allMySpaceTrash[i] != null)
            {
                //Debug.Log(allMySpaceTrash[i].name + "XForce: " + allMyForcesX[i] + ", YForce: " + allMyForcesY[i]);
                //myBody = allMySpaceTrash[i].GetComponent<Rigidbody2D>();
                //myTransform = allMySpaceTrash[i].GetComponent<Transform>();
                allMySpaceTrash[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(allMyForcesX[i] * gravConstant, allMyForcesY[i] * gravConstant));
            }
        }
    }
}
