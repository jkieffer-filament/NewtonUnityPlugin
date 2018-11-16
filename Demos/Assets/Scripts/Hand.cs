using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newton;

public class Hand : MonoBehaviour {

	public NewtonWorld World;
	public Camera Camera;

	private NewtonBallAndSocket m_Joint;
	private NewtonBody m_Body;

	private float m_PlaneDistance;

	// Use this for initialization
	void Start () {
		m_Body = new GameObject("Hand").AddComponent<NewtonBody>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetMouseButtonDown(0)) {
			var ray = Camera.ScreenPointToRay(Input.mousePosition);
			NewtonRayHitInfo info;
			if (World.Raycast(ray.origin, ray.direction, 1000, out info)) {
				


				var toHit = Camera.transform.InverseTransformPoint(info.position);
				m_PlaneDistance = Vector3.Project(toHit, Vector3.forward).magnitude;

				m_Body.SleepState = false;
				info.body.SleepState = false;
				m_Body.GetBody().SetPosition(info.position.x, info.position.y, info.position.z);
				m_Body.transform.position = info.position;

				m_Joint = m_Body.gameObject.AddComponent<NewtonBallAndSocket>();
				m_Joint.OtherBody = info.body;
				m_Joint.Stiffness = 10;
			}
		}

		else if (Input.GetMouseButton(0) && m_Joint) {
			var ray = Camera.ScreenPointToRay(Input.mousePosition);
			var direction = Camera.transform.InverseTransformDirection(ray.direction);

			m_PlaneDistance = Mathf.Clamp(m_PlaneDistance + Input.mouseScrollDelta.y * 0.5f, 3f, 100f);

			Vector3 pos;

			NewtonRayHitInfo info;
			if (World.Raycast(ray.origin, ray.direction, 1000, out info, 1 << LayerMask.NameToLayer("Environment")) &&
			(ray.origin - info.position).sqrMagnitude < m_PlaneDistance * m_PlaneDistance) {
				pos = info.position + info.normal * 0.5f;
			} else { 
				var inPlane = LineToPlane(Vector3.zero, direction, Vector3.back, Vector3.forward * m_PlaneDistance);
				pos = Camera.transform.TransformPoint(inPlane);
			}

			m_Body.SleepState = false;
			m_Joint.OtherBody.SleepState = false;
			m_Body.GetBody().SetPosition(pos.x, pos.y, pos.z);

			m_Body.transform.position = pos;
		}

		else if (Input.GetMouseButtonUp(0) && m_Joint) {
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
