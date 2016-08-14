using UnityEngine;
using System.Collections;

public class SpaceObjectGeneration : MonoBehaviour {


    public GameObject meteorSprite;
    public float distributionRadius = 100;
    //public int num

    private float initialForce = 100f;
    private float initialTorque = 20f;

	// Use this for initialization
	void Start () {

        for (int x = 0; x < 100; x++)
        {
            //GameObject cube = GameObject.ins
            //cube.AddComponent<Rigidbody>();
            //Random
            //Vector2.
            float mySize = Mathf.Pow(Random.value * 2, 2f);

            GameObject newMeteor = (GameObject)Instantiate(meteorSprite, 200*Random.insideUnitCircle, Quaternion.identity);
            newMeteor.GetComponent<Rigidbody2D>().mass = Mathf.Pow(mySize, 2f);         //setting mass to 2x diameter
            newMeteor.transform.localScale = new Vector2(mySize, mySize);
            newMeteor.GetComponent<Rigidbody2D>().AddForce(initialForce * Random.insideUnitCircle);
            newMeteor.GetComponent<Rigidbody2D>().AddTorque(Random.value*initialTorque);          //pr
            
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
