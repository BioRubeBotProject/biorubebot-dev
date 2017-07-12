using UnityEngine;
using System.Collections;

//Script courtesy of Slavish Unity Tutorials (http://www.savalishunitytutorials.com)
//Script can be found at: http://www.savalishunitytutorials.com/updatedtouchmovement.cs
//The website hosts numerous tutorials that you may find helpful

public class PanAndZoom : MonoBehaviour
{
	public float moveSensitivityX = 10.0f;
	public float moveSensitivityY = 10.0f;
	public bool updateZoomSensitivity = true;
	public float orthoZoomSpeed = 1f;
	public float minZoom = 25.0f;
	public float maxZoom = 50.0f;
	public bool invertMoveX = false;
	public bool invertMoveY = false;
	public bool zooming = false;
	public float mapWidth = 60.0f;
	public float mapHeight = 40.0f;
	public Canvas _canvas;

	public GameObject menuBtn;
	public GameObject resetBtn;
	public GameObject playBtn;
	public GameObject pauseBtn;
	public GameObject ffwBtn;
	public GameObject tutorialBtn;

	private float lastZoomDistance = float.PositiveInfinity; //should be reset to infinite on touch end
	private float maxZoomOut = 50;
	private float maxZoomIn = 16.7f;
	private float zoomSpeed = 10f;

	private Camera _camera;
	
	private float minX, maxX, minY, maxY;
	private float horizontalExtent, verticalExtent;
	

	void Start () 
	{
		_camera = Camera.main;
		CalculateLevelBounds ();
	}
	
	void Update () 
	{
		menuBtn.SetActive (true);
		tutorialBtn.SetActive (true);
		resetBtn.SetActive (true);
		playBtn.SetActive (true);
		pauseBtn.SetActive (true);
		ffwBtn.SetActive (true);
		if (updateZoomSensitivity)
		{
			moveSensitivityX = _camera.orthographicSize / 2.7f;
			moveSensitivityY = _camera.orthographicSize / 2.7f;
		}
		
		Touch[] touches = Input.touches;

		
		if (touches.Length > 0) {
			if (touches.Length == 1 && Spawner.panning) {//Single touch --> pan
				if (touches [0].phase == TouchPhase.Moved) {
					Vector2 delta = touches [0].deltaPosition;
					
					float positionX = delta.x * moveSensitivityX * 0.015f;
					positionX = invertMoveX ? positionX : positionX * -1;
					
					float positionY = delta.y * moveSensitivityY * 0.015f;
					positionY = invertMoveY ? positionY : positionY * -1;
					
					_camera.transform.position += new Vector3 (positionX, positionY, 0);
				}
			}
			if (touches.Length == 2) {//--> zoom
					
				//disable GUI controls while zooming (if they're not disabled jump around while zooming)
				menuBtn.SetActive (false);
				tutorialBtn.SetActive (false);
				resetBtn.SetActive (false);
				playBtn.SetActive (false);
				pauseBtn.SetActive (false);
				ffwBtn.SetActive (false);

				Vector2 cameraViewsize = new Vector2 (_camera.pixelWidth, _camera.pixelHeight);
					
				Touch touchOne = touches [0];
				Touch touchTwo = touches [1];
					
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
				Vector2 touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;
					
				float prevTouchDeltaMag = (touchOnePrevPos - touchTwoPrevPos).magnitude;
				float touchDeltaMag = (touchOne.position - touchTwo.position).magnitude;
					
				float deltaMagDiff = prevTouchDeltaMag - touchDeltaMag;
					
				_camera.transform.position += _camera.transform.TransformDirection ((touchOnePrevPos + touchTwoPrevPos - cameraViewsize) * _camera.orthographicSize / cameraViewsize.y);
					
				_camera.orthographicSize += deltaMagDiff * orthoZoomSpeed;
				_camera.orthographicSize = Mathf.Clamp (_camera.orthographicSize, minZoom, maxZoom) - 0.001f;
					
				_camera.transform.position -= _camera.transform.TransformDirection ((touchOne.position + touchTwo.position - cameraViewsize) * _camera.orthographicSize / cameraViewsize.y);
					
				CalculateLevelBounds ();
			}
		}
	}
	
	void CalculateLevelBounds ()
	{
		verticalExtent = _camera.orthographicSize;
		horizontalExtent = _camera.orthographicSize * Screen.width / Screen.height;
		minX = horizontalExtent - mapWidth / 2.0f;
		maxX = mapWidth / 2.0f - horizontalExtent;
		minY = verticalExtent - mapHeight / 2.0f;
		maxY = mapHeight / 2.0f - verticalExtent;
	}
	
	void LateUpdate ()
	{
		Vector3 limitedCameraPosition = _camera.transform.position;
		limitedCameraPosition.x = Mathf.Clamp (limitedCameraPosition.x, minX, maxX);
		limitedCameraPosition.y = Mathf.Clamp (limitedCameraPosition.y, minY, maxY);
		_camera.transform.position = limitedCameraPosition;
	}
}

