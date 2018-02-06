using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public GameObject cameraHolder;
    public GameObject mainCamera;
    public GameObject floorHolder;

    [Range(0.0f, 100f)]
    public float forwardSpeed = 10f;
    [Range(0.0f, 1000f)]
    public float sideSpeed = 100f;
    [Range(0.0f, 1f)]
    public float slowTimeBy = 0.01f;

    public bool jumping = false;

    public iTween.EaseType easetype;
    Hashtable jump = new Hashtable();
    private void Awake()
    {
        jump.Add("y", 6);
        jump.Add("time", 1);
        jump.Add("oncomplete", "AfterJump");
        jump.Add("onupdate", "SlowTime");
        jump.Add("easetype", easetype);
    }

    private void Start()
    {
        Physics.gravity = new Vector3(0, -30f, 0);
    }

    void Update () {
        // Makes main camera look at the player ALWAYS!!!
        mainCamera.transform.LookAt(transform);
        // Jump when up is pressed
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            jumping = true;
            iTween.MoveAdd(gameObject, jump);
            Physics.gravity = new Vector3(0, 0f, 0);
        }

        // Move left when not jumping
        if (Input.GetKey(KeyCode.LeftArrow) && !jumping)
        {
            GetComponent<Rigidbody>().AddForce(-sideSpeed * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }
        // Rotate floor left when jumping
        if (Input.GetKeyDown(KeyCode.LeftArrow) && jumping)
        {
            StartCoroutine(RotateFloor(Vector3.forward * 90, 0.4f));
        }

        // Move right when not jumping
        if (Input.GetKey(KeyCode.RightArrow) && !jumping)
        {
            GetComponent<Rigidbody>().AddForce(sideSpeed * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }
        // Rotate floor right when jumping
        if (Input.GetKeyDown(KeyCode.RightArrow) && jumping)
        {
            StartCoroutine(RotateFloor(Vector3.forward * -90, 0.4f));
        }
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
        Time.timeScale -= slowTimeBy * Time.deltaTime;
        print(Time.timeScale);
    }

    void AfterJump ()
    {
        Physics.gravity = new Vector3(0, -30f, 0);
        Time.timeScale = 1f;
        jumping = false;
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
