using Godot;

public class Chunk : MeshInstance
{
    [Export]
    private Material material;

    [Export]
    private int width = 64;

    [Export]
    private int height = 64;

    [Export]
    private int size = 1;

    private readonly SurfaceTool tool = new SurfaceTool();

    public override void _Ready()
    {
        Mesh = CreateMesh();

        CreateTrimeshCollision();
    }

    private Mesh CreateMesh()
    {
        tool.Clear();

        tool.Begin(Mesh.PrimitiveType.Triangles);

        var mesh = new ArrayMesh();

        for (int x = 0; x < width * size; x += size)
        {
            for (int z = 0; z < height * size; z += size)
            {
                var y = 0f;

                var vertexLT = new Vector3(x, y, z);

                var vertexRT = new Vector3(x + size, y, z);

                var vertexRB = new Vector3(x + size, y, z + size);

                var vertexLB = new Vector3(x, y, z + size);

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
