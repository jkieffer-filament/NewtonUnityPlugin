using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newton;

public class Motor : MonoBehaviour {

	public NewtonHingeActuator actuator;


	
	// Update is called once per frame
	void FixedUpdate () {
		actuator.TargetAngle += actuator.AngularRate * Time.fixedDeltaTime;
	}
}
