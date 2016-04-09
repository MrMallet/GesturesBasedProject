using UnityEngine;
using System.Collections;

using Assets.Scripts;

using Pose = Thalmic.Myo.Pose;

[System.Serializable]
public class Boundary{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {
	public float speed;	
	public Boundary boundary;
	public float tilt;

	public GameObject shot;
    public GameObject myo = null;
    public Transform shotSpawn;
	public float fireRate;
	
	private Rigidbody rb;
	private float nextFire;
	private AudioSource audioSource;
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}

	void Update(){
        Debug.Log(GlobalVars.playerXpos);
        Debug.Log(GlobalVars.playerYpos);
        Debug.Log(GlobalVars.playerZpos);
        // ThalmicMyo thalmicMyo = shot.GetComponent<ThalmicMyo>();

        //if (thalmicMyo.pose == Pose.Fist){
        //      nextFire = Time.time + fireRate;
        //      Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        // }
        if (Input.GetButton ("Fire1") && Time.time > nextFire) {
			nextFire = Time.time + fireRate;

			Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
			audioSource.Play();
		}
	}

	void FixedUpdate(){
		//float moveHorizontal = Input.GetAxis ("Horizontal");
		//float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3(GlobalVars.playerXpos, 0.0f ,GlobalVars.playerZpos);

		rb.velocity = movement* speed;
		
		rb.position = new Vector3 
			(Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
			 0, 
			 Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
			 );
		rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);
	}
}
