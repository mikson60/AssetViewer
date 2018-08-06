using UnityEngine;

public class PinchZoom : MonoBehaviour {

    [SerializeField] RectTransform targetRect;
    [SerializeField] Camera camera3D;
    [SerializeField] Rigidbody targetRB;

    [SerializeField] float perspectiveZoomSpeed = .5f;
    [SerializeField] float orthoZoomSpeed = .5f;
    [SerializeField] float rotationSpeed = 4f;
	
	void Update () {
		if (Input.touchCount == 2)
        {
            Debug.Log("Got 2 touches!");
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (RectTransformUtility.RectangleContainsScreenPoint(targetRect, touchZero.position) && 
                RectTransformUtility.RectangleContainsScreenPoint(targetRect, touchOne.position))
            {
                Debug.Log("Both are in!");

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                float deltaMagnitudediff = prevTouchDeltaMag - touchDeltaMag;

                if (camera3D.orthographic)
                {
                    camera3D.orthographicSize += deltaMagnitudediff * orthoZoomSpeed;
                    camera3D.orthographicSize = Mathf.Max(camera3D.orthographicSize, .1f);
                }
                else
                {
                    camera3D.fieldOfView += deltaMagnitudediff * perspectiveZoomSpeed;
                    camera3D.fieldOfView = Mathf.Clamp(camera3D.fieldOfView, .1f, 179.9f);
                }
            }
        }

        else if (Input.touchCount == 1)
        {
            Touch touchZero = Input.GetTouch(0);

            if (RectTransformUtility.RectangleContainsScreenPoint(targetRect, touchZero.position))
            {
                Debug.Log(touchZero.deltaPosition);
                targetRB.AddTorque(new Vector3(touchZero.deltaPosition.y, -touchZero.deltaPosition.x, 0) * rotationSpeed, ForceMode.Force);
            }
        }
    }
}
