using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {

	public GameObject[] bolts;
	public float speed;
	public Boundary boundary;
	public float tilt;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;

	private Rigidbody rb;
	private float nextFire;
	private AudioSource audioSource;
	private int i;
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		i=0;

	}

	void Update(){
		shot = bolts[i];
		if (Input.GetButton ("Fire1") && Time.time > nextFire) {
			nextFire = Time.time + fireRate;

			//changing the weapon will be done through the MYO here.
			// or possibly bubbles with II III symbols or x2 x3 firerate
			//shot = bolts[0];

			Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
			audioSource.Play();
		}
		if(Input.GetKeyDown(KeyCode.Space)){
			if(i<3)
				i++;
			else{
				i=0;
			}
		}
	}

	void FixedUpdate(){
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3(moveHorizontal, 0.0f ,moveVertical);

		rb.velocity = movement* speed;

		rb.position = new Vector3
			(Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
			 0,
			 Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
			 );
		rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);
	}
}
