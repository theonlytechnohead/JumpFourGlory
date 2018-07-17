using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionHandler : MonoBehaviour {

	public GameObject explosion;

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			Instantiate(explosion, other.transform.position, Quaternion.identity);
			Destroy(other.gameObject);
		}
	}
}