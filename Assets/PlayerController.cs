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
        } else if (rb.velocity.y == 0) {
            jumping = false;
        }
        print (rb.velocity.y);
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
            floorHolder.transform.rotation = floorRotation;
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
            floorHolder.transform.rotation = floorRotation;
            //StartCoroutine(RotateFloor(Vector3.forward * -90, 0.4f));
        }

        float jumpTransition = Mathf.InverseLerp(-14f, -5f, transform.localPosition.y);
        PostProcessingProfile postProfile = mainCamera.GetComponent<PostProcessingBehaviour>().profile;
        float newHue = Mathf.Lerp(0, 180, jumpTransition);
        var grading = postProfile.colorGrading.settings;
        grading.basic.hueShift = newHue;
        postProfile.colorGrading.settings = grading;
    }

    // Rotates floor in desired direcion smoothly
    IEnumerator RotateFloor(Vector3 byAngles, float inTime)
    {
        var fromAngle = floorHolder.transform.rotation;
        var toAngle = Quaternion.Euler(floorHolder.transform.eulerAngles + byAngles);
        for (var t = 0f; t < 1f; t += Time.deltaTime / inTime)
        {
            floorHolder.transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }
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
