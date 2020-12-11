using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PinchZoom : MonoBehaviour {

	public float orthoZoomSpeed = 0.5f;

	private CinemachineVirtualCamera playerCam;

	//Find the camera component on this game object. 
	void Start () {
		playerCam = GetComponent<CinemachineVirtualCamera>();
	}
	
	//Every frame, the class will check whether there are two fingers touching the screen of the device, it there are
	//it will do a couple of calculations to measure the difference between them and check if the user is trying to 
	//zoom out or in and the camera size will change depending on the touch inputs. 
	void Update () {
		if (Input.touchCount == 2) {
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
			float deltaMagnitudeDif = prevTouchDeltaMag - touchDeltaMag;
			playerCam.m_Lens.OrthographicSize += deltaMagnitudeDif * orthoZoomSpeed;
			playerCam.m_Lens.OrthographicSize = Mathf.Max(playerCam.m_Lens.OrthographicSize, 2.5f);
		}
	}
}
