﻿using UnityEngine;
using System.Collections;

//using Assets.Scripts;

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

	private Rigidbody rb;
	private float nextFire;
	private AudioSource audioSource;
	private int i;

	public GameObject myo = null;

private Pose _lastPose = Pose.Unknown;


private Vector2 referenceVector;
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
			if(i<2){
							i++;
						}
						else if(i>=2){
							i=0;
						}
		}

        ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo>();
        bool updateReference = false;
		if (thalmicMyo.pose != _lastPose){
		_lastPose = thalmicMyo.pose;

		// Vibrate the Myo armband when a fist is made.
		if (thalmicMyo.pose == Pose.WaveIn || Input.GetButton("fire1") && Time.time > nextFire){
				thalmicMyo.Vibrate(VibrationType.Medium);
				nextFire = Time.time + fireRate;

				Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
				audioSource.Play();

				ExtendUnlockAndNotifyUserAction(thalmicMyo);

				// Change material when wave in, wave out or double tap poses are made.
		} else if (thalmicMyo.pose == Pose.WaveOut) {
                //GetComponent<Renderer>().material = waveInMaterial;
								if(i<2){
												i++;
											}
											else if(i>=2){
												i=0;
											}

                ExtendUnlockAndNotifyUserAction (thalmicMyo);
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
	}
/*
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
	*/
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
