using UnityEngine;
using System;
using System.IO;

public class StraightRoadsGenerator : MonoBehaviour
{
	public string straightRoads, gradient;
	private StreamReader file, file2;

	private GameObject obj;
	private MeshFilter mf;
	private MeshRenderer mr;
	private Material mat;
	private Mesh mesh;

	float x1, x2, x3, x4, z1, z2, z3, z4, offset, grad;
	int id;

	void Start()
	{
		straightRoads = "cleaf/StraightRoads.txt";
		gradient = "cleaf/Gradient.txt";
		file = new StreamReader(straightRoads);

		id = int.Parse(file.ReadLine());
		x1 = Convert.ToSingle(file.ReadLine());
		z1 = Convert.ToSingle(file.ReadLine());
		x2 = Convert.ToSingle(file.ReadLine());
		z2 = Convert.ToSingle(file.ReadLine());

		bool newRoadSegment = false;
		string str;

		int count = 1;
		while (!file.EndOfStream)
		{
			file2 = new StreamReader(gradient);

			str = file.ReadLine();
			if (str.Equals("Right edge"))
			{
				x3 = Convert.ToSingle(file.ReadLine());
				z3 = Convert.ToSingle(file.ReadLine());
				x4 = Convert.ToSingle(file.ReadLine());
				z4 = Convert.ToSingle(file.ReadLine());
				newRoadSegment = true;
			}
			else
			{
				x3 = float.Parse(str);
				z3 = Convert.ToSingle(file.ReadLine());
				x4 = Convert.ToSingle(file.ReadLine());
				z4 = Convert.ToSingle(file.ReadLine());
			}

			Vector3 p0, p1, p2, p3;
			float y = 0.1f;
			if (id == 31 || id == 22) {
				p0 = new Vector3(x1, y, z1);
				p1 = new Vector3(x2, 10.1f, z2);
				p2 = new Vector3(x4, 10.1f, z4);
				p3 = new Vector3(x3, y, z3);
			} else if (id == 32 || id == 23) {
				p0 = new Vector3(x1, 10.1f, z1);
				p1 = new Vector3(x2, 10.1f, z2);
				p2 = new Vector3(x4, 10.1f, z4);
				p3 = new Vector3(x3, 10.1f, z3);
			} else if (id == 33 || id == 24) {
				p0 = new Vector3(x1, 10.1f, z1);
				p1 = new Vector3(x2, y, z2);
				p2 = new Vector3(x4, y, z4);
				p3 = new Vector3(x3, 10.1f, z3);
			} else {
				p0 = new Vector3(x1, y, z1);
				p1 = new Vector3(x2, y, z2);
				p2 = new Vector3(x4, y, z4);
				p3 = new Vector3(x3, y, z3);
			}

			Vector3 heading = p1 - p0;
			float distance = heading.magnitude;
			heading = heading.normalized;
			float _grad = 0;

			/*while (!file2.EndOfStream)
            {
                offset = Convert.ToSingle(file2.ReadLine());
                grad = Convert.ToSingle(file2.ReadLine());
               
                float rise = _grad * offset / 100;
                Vector3 q1 = p0 + heading * offset;
                q1.y += rise;

                Vector3 q2 = p3 + heading * offset;
                q2.y += rise;

                Vector3 q0 = p0;
                Vector3 q3 = p3;

                makeMeAMeshOhGenie(q0, q1, q2, q3);

                mesh.RecalculateBounds();
                mesh.Optimize();

                obj.AddComponent<MeshCollider>();

                p0 = q1;
                p3 = q2;
                _grad = grad;
            }*/

			makeMeAMeshOhGenie(p0, p1, p2, p3);

		    mesh.RecalculateBounds();
			mesh.Optimize();

			obj.AddComponent<MeshCollider>();

			if (!file.EndOfStream && newRoadSegment)
			{
				id = int.Parse(file.ReadLine());
				x1 = Convert.ToSingle(file.ReadLine());
				z1 = Convert.ToSingle(file.ReadLine());
				x2 = Convert.ToSingle(file.ReadLine());
				z2 = Convert.ToSingle(file.ReadLine());
				newRoadSegment = false;
			}
			else
			{
				x1 = x3;
				z1 = z3;
				x2 = x4;
				z2 = z4;
			}
			count++;
			file2.Close();
		}
		file.Close();
	}

	private void makeMeAMeshOhGenie (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		obj = new GameObject("Segment " + id);
		mf = obj.AddComponent<MeshFilter>() as MeshFilter;
		mr = obj.AddComponent<MeshRenderer>() as MeshRenderer;
		mat = Resources.Load("Materials/1RoadMob", typeof(Material)) as Material;
		mat.mainTexture = Resources.Load("Textures/Road_1lane_dark01") as Texture;
		mr.material = mat;
		mesh = new Mesh();
		mf.mesh = mesh;

		Vector3 p4 = new Vector3(p0.x, p0.y - 0.1f, p0.z);
		Vector3 p5 = new Vector3(p1.x, p1.y - 0.1f, p1.z);
		Vector3 p6 = new Vector3(p2.x, p2.y - 0.1f, p2.z);
		Vector3 p7 = new Vector3(p3.x, p3.y - 0.1f, p3.z);

		Vector3[] vertices = new Vector3[16]
		{
			// Top
			p0, p1, p2, p3,

			// Left
			p0, p1, p5, p4,

			// Right
			p2, p3, p7, p6,

			// Bottom
			p4, p5, p6, p7
		};

		float height = Vector3.Distance(p0, p1);
		float width = Vector3.Distance(p0, p3);

		float tile_y = height / width;
		mr.material.mainTextureScale = new Vector2(1.0f, tile_y);

		int[] tri = new int[24];

		tri[0] = 3;
		tri[1] = 1;
		tri[2] = 0;
		tri[3] = 3;
		tri[4] = 2;
		tri[5] = 1;
		///////////
		tri[6] = 4;
		tri[7] = 5;
		tri[8] = 6;
		tri[9] = 6;
		tri[10] = 7;
		tri[11] = 4;
		////////////
		tri[12] = 11;
		tri[13] = 8;
		tri[14] = 9;
		tri[15] = 9;
		tri[16] = 10;
		tri[17] = 11;
		////////////
		tri[18] = 12;
		tri[19] = 13;
		tri[20] = 15;
		tri[21] = 13;
		tri[22] = 14;
		tri[23] = 15;

		Vector3 side1 = p1 - p0;
		Vector3 side2 = p4 - p0;
		Vector3 perp1 = Vector3.Cross(side1, side2);
		perp1 = perp1.normalized;

		side1 = p2 - p3;
		side2 = p7 - p3;
		Vector3 perp2 = Vector3.Cross(side1, side2);
		perp2 = perp2.normalized;

		Vector3[] normals = new Vector3[16];

		normals[0] = Vector3.up;
		normals[1] = Vector3.up;
		normals[2] = Vector3.up;
		normals[3] = Vector3.up;
		normals[4] = perp1;
		normals[5] = perp1;
		normals[6] = perp1;
		normals[7] = perp1;
		normals[8] = perp2;
		normals[9] = perp2;
		normals[10] = perp2;
		normals[11] = perp2;
		normals[12] = Vector3.down;
		normals[13] = Vector3.down;
		normals[14] = Vector3.down;
		normals[15] = Vector3.down;

		Vector2[] uv = new Vector2[16];
		uv[0] = new Vector2(0, 0);
		uv[1] = new Vector2(0, 1);
		uv[2] = new Vector2(1, 1);
		uv[3] = new Vector2(1, 0);
		uv[4] = new Vector2(0.1f, 0);
		uv[5] = new Vector2(0.1f, 1);
		uv[6] = new Vector2(0.12f, 1);
		uv[7] = new Vector2(0.12f, 0);
		uv[8] = new Vector2(0.12f, 1);
		uv[9] = new Vector2(0.12f, 0);
		uv[10] = new Vector2(0.1f, 0);
		uv[11] = new Vector2(0.1f, 1);
		uv[12] = new Vector2(0.9f, 0);
		uv[13] = new Vector2(0.9f, 1);
		uv[14] = new Vector2(0.1f, 1);
		uv[15] = new Vector2(0.1f, 0);

		mesh.vertices = vertices;
		mesh.triangles = tri;
		mesh.normals = normals;
		mesh.uv = uv;
	}
}