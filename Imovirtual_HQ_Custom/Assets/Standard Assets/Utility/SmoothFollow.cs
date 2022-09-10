﻿using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class SmoothFollow : MonoBehaviour
    {

        // The target we are following
        public Transform target;
        // The distance in the x-z plane to the target
        public float distance = 10.0f;
        // the height we want the camera to be above the target
        [SerializeField]
        private float height = 5.0f;

        [SerializeField]
        private float rotationDamping;
        [SerializeField]
        private float heightDamping;


        private float targetRotation;
        float wantedRotationAngle;
        [SerializeField] float MaxAngle;

        // Use this for initialization
        void Start() { }

        // Update is called once per frame
        void Update()
        {
            // Early out if we don't have a target
            if (!target)
                return;
            
            // Calculate the current rotation angles
            targetRotation = target.eulerAngles.y;
            var wantedHeight = target.position.y + height;

            var currentRotationAngle = transform.eulerAngles.y;
            var currentHeight = transform.position.y;

            if (Mathf.Abs(targetRotation - wantedRotationAngle) > MaxAngle)
                wantedRotationAngle = targetRotation;

            // Damp the rotation around the y-axis
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, Time.deltaTime/ rotationDamping);

            // Damp the height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Convert the angle into a rotation
            var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            transform.position = target.position;
            transform.position -= currentRotation * Vector3.forward * distance;

            // Set the height of the camera
            transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

            // Always look at the target
            transform.LookAt(target);
        }
    }
}