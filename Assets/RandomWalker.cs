using UnityEngine;
using System.Collections;

public class RandomWalker : MonoBehaviour
{
    public int length = 100;
    public float stride = 0.1f;
    public int trial = 20;
    public Vector3 prevMove;
    public float inertia = 0.1f;
	public float thresh = 0.3f;
    WebCamHandler webcam;

	Vector3 lastPoint;
    Mesh mesh;

    void Start ()
    {
        webcam = FindObjectOfType (typeof(WebCamHandler)) as WebCamHandler;

        mesh = new Mesh ();
        mesh.MarkDynamic ();
        GetComponent <MeshFilter> ().mesh = mesh;

        mesh.vertices = new Vector3[length];

        var indices = new int[length];
        for (var i = 0; i < length; i++)
        {
            indices[i] = i;
        }
        mesh.SetIndices (indices, MeshTopology.LineStrip, 0);
		lastPoint = new Vector3 (Random.Range (-0.5f, 0.5f), Random.Range (-0.5f, 0.5f));;
	}
    
    void Update ()
    {
        if (!webcam.Ready) return;


        var vertices = new Vector3[length];
		var position = lastPoint;

        for (var vi = 0; vi < length; vi++)
        {
            var max = 0.0f;
            var move = Vector3.zero;
            
            for (var i = 0; i < trial; i++)
            {
				Vector3 delta = Random.insideUnitCircle * stride;
				delta += prevMove * stride * inertia;
                var newPosition = position + delta;
                var level = webcam.GetLevel(newPosition);
                if (level == 0.0f) continue;
                
                if (level > max)
                {
                    move = delta;
                    max = level;
                }

				if (max > thresh) break;
            }
            position += move;
            prevMove = move.normalized;

            vertices[vi] = position;
        }

        mesh.vertices = vertices;
		lastPoint = position;
    }
}
