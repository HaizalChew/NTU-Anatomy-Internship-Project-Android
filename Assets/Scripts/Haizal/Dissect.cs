using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissect : MonoBehaviour
{
	Vector3[] edgeVerts;
	Vector3[] vertices;
	int[] tris;
	int pointSpacing = 5;
	Vector3 mousePos;
	List<Vector3> edges = new List<Vector3>();
	int maxDist = 100;

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			//Array.Clear(edges, 0, edges.Length);
			edges.Clear();
			SaveEdge();
		}
		if (Input.GetMouseButton(0))
		{
			if ((mousePos - Input.mousePosition).sqrMagnitude >= pointSpacing * pointSpacing)
			{
				SaveEdge();
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			edges.Add(Vector3.zero);
			tris = CreateTris();
			StartCoroutine(CreateMesh(edges.ToArray(), tris));
		}
	}

	void SaveEdge()
	{
		mousePos = Input.mousePosition;
		edges.Add(Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, maxDist)));
	}

	int[] CreateTris()
	{
		List<int> trisList = new List<int>();
		for (var i = 0; i < edges.Count; i++)
		{
			trisList.Add(i);
			trisList.Add((i + 1) % (edges.Count - 1));
			trisList.Add(edges.Count - 1);
		}
		return trisList.ToArray();
	}

	IEnumerator CreateMesh(Vector3[] verts, int[] tris)
	{
		Mesh mesh = new Mesh();
		mesh.vertices = verts;
		mesh.triangles = tris;

		MeshCollider col = (MeshCollider)gameObject.AddComponent(typeof(MeshCollider));
		col.isTrigger = true;
		col.sharedMesh = mesh;
		col.convex = true;
		Rigidbody rb = (Rigidbody)gameObject.AddComponent(typeof(Rigidbody));
		rb.isKinematic = true;
		GetComponent<Rigidbody>().position = transform.position;
		yield return new WaitForFixedUpdate();

		Destroy(rb);
		Destroy(col);
		Destroy(mesh);
	}

	void OnTriggerEnter(Collider other)
	{
		print(other.name);
	}
}
