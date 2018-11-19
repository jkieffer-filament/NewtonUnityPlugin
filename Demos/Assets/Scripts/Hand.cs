using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newton;

public class Hand : MonoBehaviour {

	public NewtonWorld World;
	public Camera Camera;

	private NewtonBallAndSocket m_Joint;
	private NewtonKinematicBody m_Body;

	private Vector3 m_AngularDamping;
	private float m_PlaneDistance;


	// Use this for initialization
	void Start () {
		m_Body = new GameObject("Hand").AddComponent<NewtonKinematicBody>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetMouseButtonDown(0)) {
			var ray = Camera.ScreenPointToRay(Input.mousePosition);
			NewtonRayHitInfo info;
			if (World.Raycast(ray.origin, ray.direction, 1000, out info)) {
				var toHit = Camera.transform.InverseTransformPoint(info.Position);
				m_PlaneDistance = Vector3.Project(toHit, Vector3.forward).magnitude;

				m_Body.SleepState = false;
				info.Body.SleepState = false;
				m_Body.Position = info.Position;
				m_Body.transform.position = info.Position;

				m_AngularDamping = info.Body.AngularDamping;
				info.Body.AngularDamping = new Vector3 (15f, 15f, 15f);
				m_Joint = m_Body.gameObject.AddComponent<NewtonBallAndSocket>();
				m_Joint.OtherBody = info.Body;
				m_Joint.Stiffness = 1;
			}
		}

		else if (Input.GetMouseButton(0) && m_Joint) {
			var ray = Camera.ScreenPointToRay(Input.mousePosition);
			var direction = Camera.transform.InverseTransformDirection(ray.direction);

			m_PlaneDistance = Mathf.Clamp(m_PlaneDistance + Input.mouseScrollDelta.y * 0.5f, 3f, 100f);

			Vector3 pos;

			NewtonRayHitInfo info;
			if (World.Raycast(ray.origin, ray.direction, 1000, out info, 1 << LayerMask.NameToLayer("Environment")) &&
			(ray.origin - (info.Position + info.Normal * 0.5f)).sqrMagnitude < m_PlaneDistance * m_PlaneDistance) {
				pos = info.Position + info.Normal * 0.5f;
			} else { 
				var inCameraPlane = LineToPlane(Vector3.zero, direction, Vector3.back, Vector3.forward * m_PlaneDistance);
				
				var inGroundPlane = LineToPlane(Vector3.zero, direction, Camera.transform.InverseTransformDirection(Vector3.up), Camera.transform.InverseTransformPoint(new Vector3(0, 0.5f, 0)));

				pos = inCameraPlane.sqrMagnitude < inGroundPlane.sqrMagnitude ? Camera.transform.TransformPoint(inCameraPlane) : Camera.transform.TransformPoint(inGroundPlane);
			}

			m_Body.SleepState = false;
			m_Joint.OtherBody.SleepState = false;
			m_Body.Position = pos;
			m_Body.transform.position = pos;
		}

		else if (Input.GetMouseButtonUp(0) && m_Joint) {
			m_Joint.OtherBody.AngularDamping = m_AngularDamping;
			Destroy(m_Joint);
			m_Joint = null;
		}
	}


			/// <summary>
		/// Get the intersection point of line and plane
		/// </summary>
		public static Vector3 LineToPlane(Vector3 origin, Vector3 direction, Vector3 planeNormal, Vector3 planePoint) {
			float dot = Vector3.Dot(planePoint - origin, planeNormal);
			float normalDot = Vector3.Dot(direction, planeNormal);
			
			if (normalDot == 0.0f) return Vector3.zero;
			
			float dist = dot / normalDot;
			return origin + direction.normalized * dist;
		}
}
