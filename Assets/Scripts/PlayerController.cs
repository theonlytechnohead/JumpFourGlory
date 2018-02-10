using System.Collections;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PlayerController : MonoBehaviour {

    public GameObject cameraHolder;
    public GameObject mainCamera;
    public GameObject floorHolder;

    [Range(0.0f, 100f)]
    public float forwardSpeed = 10f;
    [Range(0.0f, 1000f)]
    public float sideSpeed = 100f;
    [Range(0.0f, 1f)]
    public float slowTimeTo = 0.25f;

    [Range(1f, 50f)]
    public float jumpVelocity = 25f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private Rigidbody rb;
    public bool jumping = false;

    public Quaternion floorRotation;


    public PostProcessingProfile PPP;
    public PostProcessingProfile PPPLow;
    private bool lowSettings = false;

    void Awake () {
        rb = GetComponent<Rigidbody>();
        floorRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void Update () {
        // Makes main camera look at the player ALWAYS!!!
        mainCamera.transform.LookAt(transform);

        if (rb.velocity.y < 0) {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.UpArrow)) {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        if (transform.position.y < -13.5f) {
            jumping = false;
        }
        // Jump when up is pressed
        if (Input.GetKeyDown(KeyCode.UpArrow) && jumping == false)
        {
            jumping = true;
            rb.velocity = Vector3.up * jumpVelocity;
        }

        // Move left when not jumping
        if (Input.GetKey(KeyCode.LeftArrow) && !jumping)
        {
            GetComponent<Rigidbody>().AddForce(-sideSpeed * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }
        // Rotate floor left when jumping
        if (Input.GetKeyDown(KeyCode.LeftArrow) && jumping)
        {
            floorRotation = Quaternion.Euler(0f, 0f, floorRotation.eulerAngles.z - 90f);
            //StartCoroutine(RotateFloor(Vector3.forward * 90, 0.4f));
        }

        // Move right when not jumping
        if (Input.GetKey(KeyCode.RightArrow) && !jumping)
        {
            GetComponent<Rigidbody>().AddForce(sideSpeed * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }
        // Rotate floor right when jumping
        if (Input.GetKeyDown(KeyCode.RightArrow) && jumping)
        {
            floorRotation = Quaternion.Euler(0f, 0f, floorRotation.eulerAngles.z + 90f);
            //StartCoroutine(RotateFloor(Vector3.forward * -90, 0.4f));
        }

        floorHolder.transform.rotation = Quaternion.Lerp(floorHolder.transform.rotation, floorRotation, 10f * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.G)) {
            lowSettings = !lowSettings;
        }
        if (lowSettings) {
            mainCamera.GetComponent<PostProcessingBehaviour>().profile = PPPLow;
        } else {
            mainCamera.GetComponent<PostProcessingBehaviour>().profile = PPP;
        }

        float jumpTransition = Mathf.InverseLerp(-14f, -5f, transform.localPosition.y);
        PostProcessingProfile postProfile = mainCamera.GetComponent<PostProcessingBehaviour>().profile;
        float newHue = Mathf.Lerp(0, 180, jumpTransition);
        var grading = postProfile.colorGrading.settings;
        grading.basic.hueShift = newHue;
        postProfile.colorGrading.settings = grading;

    }

    void SlowTime ()
    {
        Time.timeScale = slowTimeTo;
    }

    void AfterJump ()
    {
        Time.timeScale = 1f;
    }

    private void OnCollisionEnter (Collision collision)
    {
        //print("Hit " + collision.gameObject.name);
        //jumping = false;
    }

    // Move forward
    void FixedUpdate () {
        transform.Translate(0, 0, forwardSpeed * Time.deltaTime);
        cameraHolder.transform.Translate(0, 0, forwardSpeed * Time.deltaTime);
	}
}
