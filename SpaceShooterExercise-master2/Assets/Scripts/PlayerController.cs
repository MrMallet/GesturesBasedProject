using UnityEngine;
using System.Collections;
using Assets.Scripts;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

[System.Serializable]
public class Boundary{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {
	public float speed;
	public Boundary boundary;
	public float tilt;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;

	private Rigidbody rb;
	private float nextFire;
	private AudioSource audioSource;

	public GameObject myo = null;

private Pose _lastPose = Pose.Unknown;


private Vector2 referenceVector;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}

	void Update(){

		if (Input.GetButton ("Fire1") && Time.time > nextFire) {
			nextFire = Time.time + fireRate;

			Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
			audioSource.Play();
		}
		ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo>();

bool updateReference = false;


if (thalmicMyo.pose != _lastPose)
{
		_lastPose = thalmicMyo.pose;

		// Vibrate the Myo armband when a fist is made.
		if (thalmicMyo.pose == Pose.Fist || Input.GetButton("fire1") && Time.time > nextFire)
		{
				thalmicMyo.Vibrate(VibrationType.Medium);
				nextFire = Time.time + fireRate;

				Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
				audioSource.Play();

				ExtendUnlockAndNotifyUserAction(thalmicMyo);

				// Change material when wave in, wave out or double tap poses are made.
		}

}

if (updateReference)
{
		referenceVector = new Vector2(myo.transform.forward.x*10, myo.transform.forward.y*5);

		rb.velocity = referenceVector * speed;

			rb.position = new Vector3
			(Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
			 0,
			Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
			 );
		rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);

}
transform.position = new Vector2((myo.transform.forward.x*10) - referenceVector.x, myo.transform.forward.y*5 - referenceVector.y);
	}//end Updated

	void FixedUpdate(){
        float moveHorizontal = Input.GetAxis ("Horizontal");
        float moveVertical = Input.GetAxis ("Vertical");
   //     float moveHorizontal = Global_PlayerPos.playerXpos;
     //   float moveVertical = Global_PlayerPos.playerZpos;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f ,moveVertical);

		rb.velocity = movement* speed;

		rb.position = new Vector3
			(Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
			 0,
			 Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
			 );
		rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);
	}
	void ExtendUnlockAndNotifyUserAction(ThalmicMyo myo)
{
		ThalmicHub hub = ThalmicHub.instance;

		if (hub.lockingPolicy == LockingPolicy.Standard)
		{
				myo.Unlock(UnlockType.Timed);
		}

		myo.NotifyUserAction();
}
}
