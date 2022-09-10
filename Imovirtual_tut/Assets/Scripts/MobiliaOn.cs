using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobiliaOn : MonoBehaviour {

	public GameObject mobilia;

    private void Start()
    {
        Disable();
    }

    public void Enable(){
		mobilia.SetActive (true);
	}
	public void Disable(){
		mobilia.SetActive (false);
	}

}