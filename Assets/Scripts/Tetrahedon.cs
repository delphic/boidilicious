using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Tetrahedon : MonoBehaviour
{
	public float baseEdgeLength = 1.0f;
	public float heightRatio = 2.0f;	
	
	void Awake()
	{
		this.gameObject.GetComponent<MeshFilter>().mesh = CreateMesh(baseEdgeLength, heightRatio);
		
	}
	
	Mesh CreateMesh(
		float baseEdgeLength,
		float heightRatio)
	{
		// CF: http://en.wikipedia.org/wiki/Tetrahedron
		// Note not sure 0,0,0 is not at C.O.M. which it should be really.
		var height = heightRatio*Mathf.Sqrt(6)*baseEdgeLength/3; 
		var halfDepth = Mathf.Sqrt(3)*baseEdgeLength/4;
		
		var mesh = new Mesh();
		
		var vertices = new List<Vector3>();
				
		vertices.Add(new Vector3(
			-baseEdgeLength/2,
			-halfDepth,
			-height/3));
		vertices.Add(new Vector3(
			0,
			halfDepth,
			-height/3));
		vertices.Add(new Vector3(
			baseEdgeLength/2,
			-halfDepth,
			-height/3));
		vertices.Add(new Vector3(
			0,
			0,
			2*height/3));
		
		var uvs = new List<Vector2>();
		foreach(var vertex in vertices)
		{
			uvs.Add(new Vector2(vertex.x, vertex.z)); // HACK doesn't work for the most part.
		}
		
		var triangles = new List<int> { 0,2,1, 0,3,2, 2,3,1, 1,3,0 };
		
		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = triangles.ToArray();
		
		mesh.RecalculateNormals();
		
		return mesh;
	}
}
