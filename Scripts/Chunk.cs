using Godot;
using System;

public class Chunk : MeshInstance
{
    [Export]
    private Material material;

    [Export]
    private int width = 64;

    [Export]
    private int height = 64;

    [Export]
    private int size = 16;

    private readonly SurfaceTool tool = new SurfaceTool();

    private readonly RandomNumberGenerator random = new RandomNumberGenerator();

    public override void _Ready()
    {
        Mesh = CreateMesh();

        CreateTrimeshCollision();
    }

    private OpenSimplexNoise CreateNoise()
    {
        var noise = new OpenSimplexNoise();

        random.Randomize();

        noise.Seed = random.RandiRange(0, int.MaxValue);

        noise.Octaves = 3;

        noise.Period = 64f;

        noise.Persistence = 0.5f;

        noise.Lacunarity = 2f;

        return noise;
    }

    private static Vector3 GetVertex(int x, int z, OpenSimplexNoise noise)
    {
        var y = noise.GetNoise2d(x, z) * 0.5f + 1.5f;

        return new Vector3(x, (Mathf.Pow(y, 4f) - 1f), z);
    }

    private Mesh CreateMesh()
    {
        tool.Clear();

        tool.Begin(Mesh.PrimitiveType.Triangles);

        var mesh = new ArrayMesh();

        var noise = CreateNoise();

        for (int x = 0; x < width * size; x += size)
        {
            for (int z = 0; z < height * size; z += size)
            {
                var vertexLT = GetVertex(x, z, noise);

                var vertexRT = GetVertex(x + size, z, noise);

                var vertexRB = GetVertex(x + size, z + size, noise);

                var vertexLB = GetVertex(x, z + size, noise);

                var uvLT = new Vector2(0f, 0f);

                var uvRT = new Vector2(1f, 0f);

                var uvRB = new Vector2(1f, 1f);

                var uvLB = new Vector2(0f, 1f);

                tool.AddTriangleFan(
                    new Vector3[] {
                        vertexLT,
                        vertexRT,
                        vertexRB,
                    },
                    new Vector2[] {
                        uvLT,
                        uvRT,
                        uvRB,
                    }
                );

                tool.AddTriangleFan(
                   new Vector3[] {
                        vertexLT,
                        vertexRB,
                        vertexLB,
                   },
                   new Vector2[] {
                        uvLT,
                        uvRB,
                        uvLB,
                   }
               );
            }
        }

        tool.SetMaterial(material);

        tool.GenerateNormals();

        tool.Commit(mesh);

        return mesh;
    }
}
