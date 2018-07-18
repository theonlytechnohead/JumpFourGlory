using System.Collections;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

public class NewPlayerController : MonoBehaviour {

	public GameObject cameraHolder;
	public GameObject mainCamera;
	public GameObject fallHolder;
	public GameObject fallLocationGameObject;

	[Range(0.0f, 100f)]
	public float forwardSpeed = 10f;
	public float sideSpeed = 5f;

	[Range(1f, 50f)]
	public float jumpVelocity = 25f;

	private bool jumping = false;
	private bool falling = true;
	private float jumpTransition;
	float jumpDistance;
	float timeScale;

	private Quaternion worldRotation;

	public PostProcessingProfile PPP;
	public PostProcessingProfile PPPLow;
	private bool lowSettings = false;

	void Awake() {
		worldRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			Scene scene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(scene.name);
		}
		fallHolder.transform.localRotation = worldRotation;
		if (jumping) {
			transform.position = Vector3.Lerp(transform.position, new Vector3(0f, 0f, transform.position.z), 2f * Time.deltaTime);
			mainCamera.transform.localRotation = Quaternion.Slerp(mainCamera.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), 2f * Time.deltaTime);
			if (Input.GetKeyDown(KeyCode.UpArrow)) {
				worldRotation = Quaternion.Euler(0f, 0f, worldRotation.eulerAngles.z + 180f);
				StartCoroutine(disableDelay());
			}
			if (Input.GetKeyDown(KeyCode.RightArrow)) {
				worldRotation = Quaternion.Euler(0f, 0f, worldRotation.eulerAngles.z + 90f);
				StartCoroutine(disableDelay());
			}
			if (Input.GetKeyDown(KeyCode.DownArrow)) {
				StartCoroutine(disableDelay());
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				worldRotation = Quaternion.Euler(0f, 0f, worldRotation.eulerAngles.z - 90f);
				StartCoroutine(disableDelay());
			}
		} else {
			transform.position = Vector3.Lerp(transform.position, fallLocationGameObject.transform.position, 2f * Time.deltaTime);
			mainCamera.transform.localRotation = Quaternion.Slerp(mainCamera.transform.localRotation, Quaternion.Euler(30f, 0f, 0f), 2f * Time.deltaTime);

		}
		if (falling) {
			if (Input.GetKeyDown(KeyCode.UpArrow)) {
				jumping = true;
				falling = false;
			}
			if (Input.GetKey(KeyCode.RightArrow)) {
				transform.Translate(Vector3.right * sideSpeed * Time.deltaTime, Space.Self);
				fallLocationGameObject.transform.Translate(Vector3.right * sideSpeed * Time.deltaTime, Space.Self);
			}
			if (Input.GetKey(KeyCode.LeftArrow)) {
				transform.Translate(Vector3.left * sideSpeed * Time.deltaTime, Space.Self);
				fallLocationGameObject.transform.Translate(Vector3.left * sideSpeed * Time.deltaTime, Space.Self);
			}
		}

		cameraHolder.transform.rotation = Quaternion.Slerp(cameraHolder.transform.rotation, worldRotation, 3f * Time.deltaTime);
		transform.rotation = Quaternion.Slerp(transform.rotation, worldRotation, 4f * Time.deltaTime);

		if (Input.GetKeyDown(KeyCode.G)) {
			lowSettings = !lowSettings;
		}
		if (lowSettings) {
			mainCamera.GetComponent<PostProcessingBehaviour>().profile = PPPLow;
		} else {
			mainCamera.GetComponent<PostProcessingBehaviour>().profile = PPP;
		}

		jumpDistance = Vector3.Distance(transform.position, fallHolder.transform.position);
		jumpTransition = Mathf.InverseLerp(14f, 0f, jumpDistance);
		timeScale = Mathf.InverseLerp(0f, 14f, jumpDistance);
		// Post processing coolio stuffs
		PostProcessingProfile postProfile = mainCamera.GetComponent<PostProcessingBehaviour>().profile;
		float newHue = Mathf.Lerp(0, 270, jumpTransition);
		var grading = postProfile.colorGrading.settings;
		grading.basic.hueShift = newHue;
		postProfile.colorGrading.settings = grading;
		// DoF stuff for coolio post proccessing stuff and to cope with camera push in
		var dof = postProfile.depthOfField.settings;
		dof.focusDistance = Mathf.Lerp(12f, 8f, jumpTransition);
		dof.focalLength = Mathf.Lerp(75, 10, jumpTransition);
		postProfile.depthOfField.settings = dof;
		// FoV lerpy-derp
		mainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(55f, 120f, jumpTransition);
	}

	void FixedUpdate() {
		// Move stuff forward - progression!
		transform.Translate(0, 0, forwardSpeed * timeScale * Time.deltaTime);
		cameraHolder.transform.Translate(0, 0, forwardSpeed * timeScale * Time.deltaTime);
		fallHolder.transform.Translate(0, 0, forwardSpeed * timeScale * Time.deltaTime);
		// Camera push in when jumping - compensate for crazy FoV
		Vector3 newPos = mainCamera.transform.localPosition;
		newPos.z = Mathf.Lerp(-12f, -8f, jumpTransition);
		mainCamera.transform.localPosition = newPos;
		//Time.timeScale = Mathf.Lerp(1f, 0.25f, jumpTransition);
	}

	IEnumerator disableDelay() {
		jumping = false;
		yield return new WaitForSeconds(0.5f);
		falling = true;
		StopAllCoroutines();
	}
}