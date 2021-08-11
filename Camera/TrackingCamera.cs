using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingCamera : MonoBehaviour
{
	Transform targetObj;
	[SerializeField]
	Rect targetBounds;
	bool trackHeight = false;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if(targetObj != null)
			TrackTarget();
	}

	void TrackTarget()
	{
		Vector3 offset = Vector3.zero;
		if(targetObj.position.x < (transform.position.x + targetBounds.x + targetBounds.xMin))
		{
			offset.x = targetObj.position.x - (transform.position.x + targetBounds.x + targetBounds.xMin);
		}
		if(targetObj.position.x > (transform.position.x + targetBounds.x + targetBounds.xMax))
		{
			offset.x = targetObj.position.x - (transform.position.x + targetBounds.x + targetBounds.xMax);
		}
		if(trackHeight)
		{
			if(targetObj.position.y < (transform.position.y + targetBounds.y + targetBounds.yMin))
			{
				offset.y = targetObj.position.y - (transform.position.y + targetBounds.y + targetBounds.yMin);
			}
			if(targetObj.position.y > (transform.position.y + targetBounds.y + targetBounds.yMax))
			{
				offset.y = targetObj.position.y - (transform.position.y + targetBounds.y + targetBounds.yMax);
			}
		}
		transform.position += offset;
	}

	public void SetTarget(Transform target)
	{
		targetObj = target;
	}

	private void OnDrawGizmos() {
		Color c = Color.magenta;
		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
		c.a = 0.125f;
		Gizmos.color = c;
		Gizmos.DrawCube(targetBounds.position, targetBounds.size);
		c.a = 1;
		Gizmos.color = c;
		Gizmos.DrawWireCube(targetBounds.position, targetBounds.size);
	}
}
