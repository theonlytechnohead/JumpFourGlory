using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class collisionHandler : MonoBehaviour {

	public GameObject explosion;

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			Instantiate(explosion, other.transform.position, Quaternion.identity);
			Invoke("reset", 10f);
			other.gameObject.GetComponent<NewPlayerController>().bloomDestroy();
			Destroy(other.gameObject.GetComponent<MeshRenderer>());
			other.gameObject.GetComponent<NewPlayerController>().destroyed = true;
		}
	}

	void reset() {
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.name);
	}
}