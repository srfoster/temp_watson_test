using UnityEngine;
using System.Collections;

public class LightChange : MonoBehaviour {
	public GameObject lightCube;

	void OnTriggerEnter(Collider other) {
		lightCube.SetActive (true);
	}

	void OnTriggerExit(Collider other) {
		lightCube.SetActive (false);
	}
}
