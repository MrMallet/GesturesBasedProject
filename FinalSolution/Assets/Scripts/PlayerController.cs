using UnityEngine;
using System.Collections;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;


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

	private bool shooting = false;
	private Rigidbody rb;
	private float nextFire;
	private AudioSource audioSource;
	private int i;

	public ThalmicMyo thalmicMyo;
	public GameObject myo = null;
	private Pose _lastPose = Pose.Unknown;
	private Vector2 referenceVector;
	private Vector3 startPosition;


	void Start()
	{
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		i=0;
		thalmicMyo = myo.GetComponent<ThalmicMyo>();
		startPosition = transform.position;
	}

	void shoot(bool shooting ){
		if ((shooting) && Time.time > nextFire){
			nextFire = Time.time + fireRate;
			Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
		}
	}

	void Update(){


		bool updateReference = false;
		shoot(shooting);
		shot = bolts[i];

			if (Input.GetButton ("Fire1") && Time.time > nextFire) {
				nextFire = Time.time + fireRate;

				Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
				audioSource.Play();
			}
			if(Input.GetKeyDown(KeyCode.Space)){
				if(i<2){
					i++;
				}
				else if(i>=2){
					i=0;
				}
			}

		if (thalmicMyo.pose != _lastPose){
				_lastPose = thalmicMyo.pose;
				shooting = false;

				if (thalmicMyo.pose == Pose.FingersSpread) {
						//updateReference = true;
						thalmicMyo.Vibrate(VibrationType.Medium);
						rb.position = startPosition;
						ExtendUnlockAndNotifyUserAction(thalmicMyo);
				}
				else if (thalmicMyo.pose == Pose.Fist)
				{
						shooting = true;
						thalmicMyo.Vibrate(VibrationType.Medium);
						nextFire = Time.time + fireRate;

						Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
						audioSource.Play();

						ExtendUnlockAndNotifyUserAction(thalmicMyo);
				}
				else if (thalmicMyo.pose == Pose.WaveOut)
				{
						thalmicMyo.Vibrate(VibrationType.Long);
						if(i<2){
							i++;
						}
						else if(i>=2){
							i=0;
						}
						ExtendUnlockAndNotifyUserAction(thalmicMyo);
				}
				else if (thalmicMyo.pose == Pose.WaveIn)
				{
						thalmicMyo.Vibrate(VibrationType.Long);
						if(i>0){
							i--;
						}
						else if(i ==0){
							i=2;
						}
						ExtendUnlockAndNotifyUserAction(thalmicMyo);
				}
		}

		if (updateReference)
		{

			Vector3 movement = new Vector3(myo.transform.forward.x*10, 0.0f , 0.0f); //myo.transform.forward.z * 5
			rb.velocity = movement * speed;

				rb.position = new Vector3
				(Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
				 0,
				 Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
				);
			rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);

		}
		//rb.position = new Vector3((myo.transform.eulerAngles.x*10), 0.0f , 0.0f);// myo.transform.forward.z

		rb.position = new Vector3((myo.transform.forward.x*10) , 0.0f , 0.0f);// myo.transform.forward.z

}

	void FixedUpdate(){
		float moveHorizontal = Input.GetAxis ("Horizontal"); //myo.transform.forward.x * 10 ;
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

	void ExtendUnlockAndNotifyUserAction (ThalmicMyo myo)
	{
			ThalmicHub hub = ThalmicHub.instance;

			if (hub.lockingPolicy == LockingPolicy.Standard) {
					myo.Unlock (UnlockType.Timed);
			}

			myo.NotifyUserAction ();
	}
}
