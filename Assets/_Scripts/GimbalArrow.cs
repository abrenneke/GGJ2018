﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gimbalScript : MonoBehaviour {


	// Use this for initialization
	void Start () {


	}



    // Update is called once per frame
    void Update () {
        transform.rotation = Quaternion.LookRotation(Vector3.left);
	}
}