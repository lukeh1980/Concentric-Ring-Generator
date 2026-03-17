using UnityEngine;

namespace DigitalHorde.Examples {

    public class SimpleMouseLook : MonoBehaviour {

        [SerializeField] float m_SmoothDampTime = 0.3f;

        public float mouseSensitivity = 100.0f;
        public float clampAngle = 80.0f;

        private float rotY = 0.0f; // rotation around the up/y axis
        private float rotX = 0.0f; // rotation around the right/x axis

        private float lastRotX = 0.0f;
        private float lastRotY = 0.0f;

        // smooth damp:
        float yVelocity = 0.0f;
        float xVelocity = 0.0f;

        void Start() {

            Vector3 rot = transform.localRotation.eulerAngles;
            rotY = rot.y;
            rotX = rot.x;

            lastRotX = rotX;
            lastRotY = rotY;

        }

        void Update() {

            if (Input.GetMouseButton(1)) {

                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = -Input.GetAxis("Mouse Y");

                rotY += mouseX * mouseSensitivity * Time.deltaTime;
                rotX += mouseY * mouseSensitivity * Time.deltaTime;
                rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

                // smooth damp:
                float newRotY = Mathf.SmoothDamp(lastRotY, rotY, ref yVelocity, m_SmoothDampTime);
                float newRotX = Mathf.SmoothDamp(lastRotX, rotX, ref xVelocity, m_SmoothDampTime);

                Quaternion localRotation = Quaternion.Euler(newRotX, newRotY, 0.0f);
                transform.rotation = localRotation;

                lastRotX = newRotX;
                lastRotY = newRotY;

            }

        }

    }

}