using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// This Component takes care of scaling children 3D Objects with a Renderer-Derived Component
/// (Mesh Renderer/Skinned Mesh Renderer) to fit a Rect Transform. 
/// The 3D Objects will be uniformly scaled around the ParentTransform and can
/// either fit the height, width or bounds of the parent Rect Transform
/// </summary>
[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class CanvasElement3D : UIBehaviour{
	public RectTransform parentTransform;
	public bool autoUpdate = true;
	public bool scaleOnStart = true;
	[Tooltip("Scale children to fit either rect height, width or within bounds?")]
	public Fit fit = Fit.best;
	public Renderer[] rendererToScale = new Renderer[0];
	private Vector3[] cornersArray = new Vector3[4];

	public enum Fit{
		height, width, best
	}
	protected override void Start()
	{
		if(scaleOnStart) 
			ScaleRenderer();
	}
	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();

		if(autoUpdate) 
			ScaleRenderer();
	}
	public void ScaleRenderer(float scaleFactor = 1f)
	{
		#if UNITY_EDITOR
		//common error
		if(parentTransform == (transform as RectTransform))
		{
			Debug.LogWarning("The parent transform can't be on the same GameObject" +
				"as the GameObject that defines the scale");
			return;	
		}
		#endif
		parentTransform.localScale = TargetScale(scaleFactor);
	}
	public Vector3 TargetScale(float scaleFactor = 1f)
	{
		return parentTransform.localScale * FittingScaleFactor() * scaleFactor;
	}
	public float FittingScaleFactor()
	{
		Vector2 scaleFactor = CalculateScaleFactor();
		float factor = 1f;
		switch (fit) {
		case Fit.height:
			factor = scaleFactor.y;
			break;
		case Fit.width:
			factor = scaleFactor.x;
			break;
		case Fit.best:
			factor = Mathf.Min(scaleFactor.x, scaleFactor.y);
			break;
		}
		//When called at Awake, the Rect Transform Bounds are not properly initialized resulting in NaN Scale at this point
		if(float.IsNaN(factor) || float.IsInfinity(factor)) 
			return 1f;
		//the factor should also never become zero
		else if(FloatNullCheck(factor)) 
			return 0.01f;
		else return 
			factor;
	}
	public Vector2 CalculateScaleFactor()
	{
		Vector3 meshBoundSize = CreateUnifiedBounds(rendererToScale).size;
		Vector3 desiredBoundSize = GetBoundsForRectTransform().size;
		return new Vector2(
			desiredBoundSize.x/ meshBoundSize.x, 
			desiredBoundSize.y/meshBoundSize.y
		); 
	}
	public Bounds GetBoundsForRectTransform()
	{
		Bounds bounds = new Bounds(transform.position, Vector3.zero);
		RectTransform rectTransform = transform as RectTransform;
		rectTransform.GetWorldCorners(cornersArray);
		foreach(Vector3 point in cornersArray)
		{
			bounds.Encapsulate(point);	
		}
		return bounds;
	}
	protected Bounds CreateUnifiedBounds(Renderer[] rendererToEncompass)
	{
		Bounds bounds = new Bounds(parentTransform.position, Vector3.zero);
		foreach(Renderer ren in rendererToEncompass)
		{
			if(ren.enabled) 
				bounds.Encapsulate(ren.bounds);
		}
		return bounds;
	}
	bool VectorNullCheck(Vector3 v3)
	{
		return FloatNullCheck(v3.x) && FloatNullCheck(v3.y) && FloatNullCheck(v3.z);
	}
	bool VectorNullCheck(Vector2 v2)
	{
		return FloatNullCheck(v2.x) && FloatNullCheck(v2.y);
	}
	bool FloatNullCheck(float f)
	{
		return Mathf.Approximately(f, 0f);
	}
}
