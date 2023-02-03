using UnityEngine;

public static class DebugGizmos
{
	public static void DrawDebugBox(Vector3 center, Quaternion orientation, Vector3 halfExtents, Color color)
	{
		// Calculate the corners of the box
		Vector3 frontTopLeft = center + orientation * new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);
		Vector3 frontTopRight = center + orientation * new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
		Vector3 frontBottomLeft = center + orientation * new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
		Vector3 frontBottomRight = center + orientation * new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
		Vector3 backTopLeft = center + orientation * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
		Vector3 backTopRight = center + orientation * new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
		Vector3 backBottomLeft = center + orientation * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
		Vector3 backBottomRight = center + orientation * new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

		// Draw the lines connecting the corners
		Debug.DrawLine(frontTopLeft, frontTopRight, color);
		Debug.DrawLine(frontTopRight, frontBottomRight, color);
		Debug.DrawLine(frontBottomRight, frontBottomLeft, color);
		Debug.DrawLine(frontBottomLeft, frontTopLeft, color);
		Debug.DrawLine(backTopLeft, backTopRight, color);
		Debug.DrawLine(backTopRight, backBottomRight, color);
		Debug.DrawLine(backBottomRight, backBottomLeft, color);
		Debug.DrawLine(backBottomLeft, backTopLeft, color);
		Debug.DrawLine(frontTopLeft, backTopLeft, color);
		Debug.DrawLine(frontTopRight, backTopRight, color);
		Debug.DrawLine(frontBottomRight, backBottomRight, color);
		Debug.DrawLine(frontBottomLeft, backBottomLeft, color);
	}

}
