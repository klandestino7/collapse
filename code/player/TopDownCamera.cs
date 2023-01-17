using Sandbox;

namespace Facepunch.Collapse;

public partial class TopDownCamera
{
	public float Height { get; set; } = 450f;
	public float MoveSpeed { get; set; } = 20f;

	private float Scale { get; set; } = 1.5f;

	public void Update()
	{
		var pawn = CollapsePlayer.Me;

		if ( pawn.IsValid() )
		{
            var target = pawn.Position.WithZ(pawn.Position.z + Height);

            Sound.Listener = new Transform(pawn.EyePosition, Camera.Rotation);

            //Camera.Rotation = Rotation.LookAt(new Vector3(150.0f, 80.0f, -220.0f));
            Camera.Rotation = Rotation.LookAt(Vector3.Down);


            var newCamPosition = new Vector3(Camera.Position.LerpTo(target, Time.Delta * MoveSpeed));
            //Camera.Position = new Vector3(newCamPosition.x, newCamPosition.y, newCamPosition.z);

            Camera.FieldOfView = Screen.CreateVerticalFieldOfView(50f);
            Camera.FirstPersonViewer = null;

            //Input.MouseWheel

            //if (Input.MouseWheel < 0 && Scale <= 3.0f)
            //    Scale += 0.1f;
            //else if (Input.MouseWheel > 0 && Scale >= 1.0f)
            //    Scale -= 0.1f;


            //Camera.FirstPersonViewer = null;

            Vector3 targetPos;
            var center = pawn.Position + Vector3.Up * 50;

            var pos = center;
            var rot = Rotation.LookAt(Vector3.Down);

            //float distance = 180.0f * Scale;
            //targetPos = pos + rot.Right * ((pawn.CollisionBounds.Mins.x + 50) * Scale);
            //targetPos += rot.Forward * -distance - 40;

            //targetPos += Vector3.Backward * 100.0f;

            //Camera.Rotation = rot;

            var tr = Trace.Ray(pos, newCamPosition)
                .WithAnyTags("world")
                .WithoutTags("wall")
                .Radius(8)
                .Run();

            Camera.Position = tr.EndPosition;

        }
    }
}
