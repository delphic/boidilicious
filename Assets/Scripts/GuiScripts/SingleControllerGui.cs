using UnityEngine;
using System.Collections;
using SteeringBehaviour = SingleBehaviourSteeringController.SteeringBehaviour;
using Deceleration = SteeringBehaviours.Deceleration;

[RequireComponent(typeof(BoxCollider))]
public class SingleControllerGui : MonoBehaviour {
	
	public SingleBehaviourSteeringController boidController;
	public GameObject controlledBoid;
	public GameObject boidPrefab;
	public GUISkin guiSkin;
	
	private Rect _guiArea = new Rect(10,10,500,500);
	private bool _waitingForClick = false;
	private System.Action<Vector3> _onClickDelegate = (x) => {};
	
	private GameObject _otherBoid = null;
	
	private int _selectedBehaviour = 0;
	private string[] _behaviourStrings = new [] { "Wander", "Arrive", "Evade", "Flee", "Pursuit", "Seek" };
	private float _panicSliderValue = 0.0f;
	private float _wanderRadiusSliderValue = 0.0f;
	private float _wanderDistanceSliderValue = 0.0f;
	private float _wanderJitterSliderValue = 0.0f;
	private int _selectedDeceleration = 2;
	private string[] _decelerationStrings = new [] { "Slow", "Normal", "Fast" };
	
	void Awake(){
		var collider = this.GetComponent<BoxCollider>();
		collider.isTrigger = true;
		collider.transform.position = Vector3.zero;
		collider.size = new Vector3(1000,1000,1);
	}
	
	void Start(){
		this.boidController = this.controlledBoid.GetComponent<Boid>().Controller as SingleBehaviourSteeringController;
		_panicSliderValue = Mathf.Log10(this.boidController.steeringBehaviours.panicDistance); 				// Using 10 as baseValue, hence log10
		_wanderRadiusSliderValue = Mathf.Log10(this.boidController.steeringBehaviours.wanderRadius+1);		// Using offset for wander values, hence +1
		_wanderDistanceSliderValue = Mathf.Log10(this.boidController.steeringBehaviours.wanderDistance+1);
		_wanderJitterSliderValue = Mathf.Log10(this.boidController.steeringBehaviours.wanderJitter+1);
	}
	
	void OnGUI() {
		GUI.skin = guiSkin;
		GUILayout.BeginArea(_guiArea);
		var previousBehaviour = _selectedBehaviour;
		_selectedBehaviour = GUILayout.SelectionGrid(_selectedBehaviour, _behaviourStrings, _behaviourStrings.Length);
		var selectedBehaviour = (SteeringBehaviour)System.Enum.Parse(typeof(SteeringBehaviour), _behaviourStrings[_selectedBehaviour]);
		
		switch(selectedBehaviour){
		case SteeringBehaviour.Arrive:
			_waitingForClick = true;
			_onClickDelegate = (pos) => { this.boidController.targetPosition = pos; };
			GuiLayoutVectorLabel("Target Position (Click to Set)", this.boidController.targetPosition);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Deceleration");
			if(GuiLayoutMultiButton(ref _selectedDeceleration, _decelerationStrings)) {
				this.boidController.deceleration = (Deceleration)System.Enum.Parse(typeof(Deceleration), _decelerationStrings[_selectedDeceleration]);
			}
			GUILayout.EndHorizontal();
			break;
		case SteeringBehaviour.Evade:
			_waitingForClick = false;
			this.boidController.steeringBehaviours.panicDistance = GuiLayoutLabelledPowerBar("Panic Distance", ref _panicSliderValue, 10, 3, false);
			break;
		case SteeringBehaviour.Flee:
			_waitingForClick = true;
			_onClickDelegate = (pos) => { this.boidController.fleePosition = pos; };
			GuiLayoutVectorLabel("Scary Point (Click to Set)", this.boidController.fleePosition);
			this.boidController.steeringBehaviours.panicDistance = GuiLayoutLabelledPowerBar("Panic Distance", ref _panicSliderValue, 10, 3, false);
			break;
		case SteeringBehaviour.Seek:
			_waitingForClick = true;
			_onClickDelegate = (pos) => { this.boidController.targetPosition = pos; };
			GuiLayoutVectorLabel("Target Position (Click to Set)", this.boidController.targetPosition);
			break;
		case SteeringBehaviour.Wander:
			_waitingForClick = false;
			this.boidController.steeringBehaviours.wanderRadius = GuiLayoutLabelledPowerBar("Wander Radius", ref _wanderRadiusSliderValue, 10, 2, true);
			this.boidController.steeringBehaviours.wanderDistance = GuiLayoutLabelledPowerBar("Wander Distance", ref _wanderDistanceSliderValue, 10, 2, true);
			this.boidController.steeringBehaviours.wanderJitter = GuiLayoutLabelledPowerBar("Wande Jitter", ref _wanderJitterSliderValue, 10, 2, true);
			break;
		default:
			_waitingForClick = false;
			break;
		}
		GUILayout.EndArea();
		
		if(previousBehaviour != _selectedBehaviour)
		{
			if((selectedBehaviour.Equals(SteeringBehaviour.Evade) || selectedBehaviour.Equals(SteeringBehaviour.Pursuit))) {
				if(_otherBoid == null) {
					_otherBoid = (GameObject)Instantiate(boidPrefab);
					boidController.boidToEvade = _otherBoid.GetComponent<Boid>();
					boidController.boidToPursue = _otherBoid.GetComponent<Boid>();
				}
			} else if (_otherBoid != null) {
				Destroy(_otherBoid);
				_otherBoid = null;
				boidController.boidToEvade = null;
				boidController.boidToPursue = null;
			}
			this.boidController.currentBehaviour = selectedBehaviour;		

		}
	}
	
	#region Gui Helper Functions
	
	bool GuiLayoutMultiButton(ref int selected, string[] descriptions) 
	{
		if(GUILayout.Button(descriptions[selected])) {
			selected = (selected+1)%descriptions.Length;
			return true;
		}
		return false;
	}
	
	void GuiLayoutVectorLabel(string label, Vector2 vector) {
		GUILayout.BeginVertical();
		GUILayout.Label(label);
		GUILayout.BeginHorizontal();
		GUILayout.Label("x");
		GUILayout.Label(Mathf.RoundToInt(vector.x).ToString());
		GUILayout.Label("y");
		GUILayout.Label(Mathf.RoundToInt(vector.y).ToString());
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
	
	float GuiLayoutLabelledPowerBar(string label, ref float sliderValue, float baseValue, float maxPower, bool offset, params GUILayoutOption[] layoutOptions)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label);
		var result = GuiLayoutPowerBar(ref sliderValue, baseValue, maxPower, offset);
		GUILayout.Label(Mathf.RoundToInt(result).ToString());
		GUILayout.EndHorizontal();
		return result;
	}
	
	float GuiLayoutPowerBar(ref float sliderValue, float baseValue, float maxPower, bool offset, params GUILayoutOption[] layoutOptions)
	{	
		sliderValue = GUILayout.HorizontalSlider(sliderValue, 0, maxPower, layoutOptions);
		var result = Mathf.Pow(baseValue, sliderValue);
		if (offset) { result-=1.0f; }
		return result;
	}
	
	#endregion
	
	void OnMouseUp () {
		if(_waitingForClick) {
			_onClickDelegate(GetCursorWorldPosition());
		}
	}
	
	Vector3 GetCursorWorldPosition() {
		RaycastHit hit;
		Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
		return hit.point;
	}
}
