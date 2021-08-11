using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class CollisionBox : MonoBehaviour
{
	protected BoxCollider _collider{
		get { return GetComponent<BoxCollider>(); }
	}

	public Vector2 boxPos = Vector2.zero;
	public Vector2 boxSize = Vector2.one;
	protected Vector2 halfSize = Vector2.one;
	public EntityController owner;

	private void Awake() {
		if(owner == null)
		{
			owner = GetComponentInParent<EntityController>();
		}
	}

	public void UpdateColBox(Vector3 pos, Vector3 size)
	{
		boxPos = pos;
		boxSize = size;
		halfSize = 0.5f*boxSize;

		_collider.center = new Vector3(boxPos.x , boxPos.y, 0);
		_collider.size = new Vector3(boxSize.x, boxSize.y, 1);
	}
	
	

	protected void DrawColBox(Color c)
	{
		Vector3 pos = new Vector3(boxPos.x, boxPos.y, 0);
		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
		c.a = 0.125f;
		Gizmos.color = c;
		Gizmos.DrawCube(pos, boxSize);
		c.a = 1;
		Gizmos.color = c;
		Gizmos.DrawWireCube(pos, boxSize);
	}
}
