using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;


namespace Assets.Scripts
{
    class MyoController : MonoBehaviour
    {
        public GameObject myo = null;

        void Update() {

            ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo>();







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
}
