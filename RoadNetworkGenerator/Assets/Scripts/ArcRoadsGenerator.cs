using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class ArcRoadsGenerator : MonoBehaviour
{
	public string arcs_RightEdge, arcs_LeftEdge;
	private StreamReader file1;
	private StreamReader file2;

	private GameObject obj;
	private MeshFilter mf;
	private MeshRenderer mr;
	private Material material;
	private Texture texture;
	private Mesh mesh;

	// Mutable lists for all the vertices and triangles of the mesh
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();

	private List<Vector3> normals = new List<Vector3>();
	private List<Vector2> uv = new List<Vector2>();

	void Start()
	{
		
		arcs_RightEdge = "cleaf/Arcs_RightEdge.txt";
		arcs_LeftEdge = "cleaf/Arcs_LeftEdge.txt";
		file1 = new StreamReader(arcs_RightEdge);
		file2 = new StreamReader(arcs_LeftEdge);

		obj = new GameObject("Arcs");
		mf = obj.AddComponent<MeshFilter>() as MeshFilter;
		mr = obj.AddComponent<MeshRenderer>() as MeshRenderer;
		material = Resources.Load("Materials/1RoadMob", typeof(Material)) as Material;
		material.mainTexture = Resources.Load("Textures/Road_1lane_dark01") as Texture;
		mr.material = material;
		mesh = new Mesh();
		mf.mesh = mesh;

		string str;
		while (!file1.EndOfStream)
		{
			str = file1.ReadLine();
			if (str.Equals("end of arc"))
			{
				// Assign vertices and triangles to mesh
				mesh.vertices = vertices.ToArray();
				mesh.triangles = triangles.ToArray();
				mesh.normals = normals.ToArray();
				mesh.uv = uv.ToArray();
				obj.AddComponent<MeshCollider>();

				if (!file1.EndOfStream)
				{
					obj = new GameObject("Arcs");
					mf = obj.AddComponent<MeshFilter>() as MeshFilter;
					mr = obj.AddComponent<MeshRenderer>() as MeshRenderer;
					mr.material = material;
					mesh = new Mesh();
					mf.mesh = mesh;

					vertices.Clear();
					triangles.Clear();
					normals.Clear();
					uv.Clear();

					AddTerrainPoint(new Vector3(Convert.ToSingle(file1.ReadLine()), 0.1f, Convert.ToSingle(file1.ReadLine())));
				}
			}
			else
			{
				AddTerrainPoint(new Vector3(Convert.ToSingle(str), 0.1f, Convert.ToSingle(file1.ReadLine())));
			}
		}

		file1.Close();
		file2.Close();
	}

	void AddTerrainPoint(Vector3 point)
	{
		if (vertices.Count == 0)
		{
			vertices.Add(point);
			vertices.Add(new Vector3(point.x, 0.0f, point.z));//////////////////////////////////////////////////////////////////////////////////////////

			float _x = Convert.ToSingle(file2.ReadLine());
			float _z = Convert.ToSingle(file2.ReadLine());
			vertices.Add(new Vector3(_x, 0.1f, _z));
			vertices.Add(new Vector3(_x, 0.0f, _z));

			normals.Add(Vector3.up);
			normals.Add(Vector3.up);
			normals.Add(Vector3.up);
			normals.Add(Vector3.up);

			uv.Add(new Vector2(0, 1));
			uv.Add(new Vector2(1, 1));
			uv.Add(new Vector2(0, 1));
			uv.Add(new Vector2(1, 1));
			//uv.Add(new Vector2(0, 0));
			//uv.Add(new Vector2(0, 1));
			//uv.Add(new Vector2(0, 0));
			//uv.Add(new Vector2(0, 1));
		}

		else
		{
			// Corresponding edge point
			float x = Convert.ToSingle(file2.ReadLine());
			float z = Convert.ToSingle(file2.ReadLine());
			Vector3 point2 = new Vector3(x, 0.1f, z);

			// Our top side
			Vector3 p0 = vertices[vertices.Count - 4];
			Vector3 p1 = vertices[vertices.Count - 2];
			Vector3 p2 = point;
			Vector3 p3 = point2;

			vertices.Add(p0);
			vertices.Add(p1);
			vertices.Add(p2);
			vertices.Add(p3);

			vertices.Add(point);
			vertices.Add(new Vector3(point.x, 0.0f, point.z));

			vertices.Add(point2);
			vertices.Add(new Vector3(x, 0.0f, z));

			int start = vertices.Count - 12;

			// We have completed a section (3 sides), create 6 triangles
			// Top
			triangles.Add(start + 7);
			triangles.Add(start + 5);
			triangles.Add(start + 4);
			triangles.Add(start + 4);
			triangles.Add(start + 6);
			triangles.Add(start + 7);
			//  Right
			triangles.Add(start + 9);
			triangles.Add(start + 8);
			triangles.Add(start + 0);
			triangles.Add(start + 0);
			triangles.Add(start + 1);
			triangles.Add(start + 9);
			// Left
			triangles.Add(start + 2);
			triangles.Add(start + 10);
			triangles.Add(start + 11);
			triangles.Add(start + 11);
			triangles.Add(start + 3);
			triangles.Add(start + 2);

			normals.Add(Vector3.up);
			normals.Add(Vector3.up);
			normals.Add(Vector3.up);
			normals.Add(Vector3.up);

			normals.Add(Vector3.up);
			normals.Add(Vector3.up);
			normals.Add(Vector3.up);
			normals.Add(Vector3.up);

			uv.Add(new Vector2(1, 0));
			uv.Add(new Vector2(0, 0));
			uv.Add(new Vector2(1, 1));
			uv.Add(new Vector2(0, 1));

			uv.Add(new Vector2(0, 1));
			uv.Add(new Vector2(1, 1));
			uv.Add(new Vector2(0, 1));
			uv.Add(new Vector2(1, 1));
		}
	}
}
