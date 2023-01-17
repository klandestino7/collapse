using Sandbox;

namespace NxtStudio.Collapse;

public partial class TopDownCamera
{
	public float Height { get; set; } = 650f;
	public float MoveSpeed { get; set; } = 20f;

	private float Scale { get; set; } = 1.5f;

	public void Update()
	{
		var pawn = CollapsePlayer.Me;

		if ( pawn.IsValid() )
		{

            // if (Input.MouseWheel > 0 && Height >= 190f)
            // {
            //     Height = MathX.LerpTo(Height, 190f, Time.Delta * 10f);
            // }
            // if (Input.MouseWheel < 0 && Height <= 650f)
            // {
            //     Height = MathX.LerpTo(Height, 650f, Time.Delta * 10f);
            // }

            // if (Height >= 650f)
            // {
            //     Height = 650f;
            // }
            // else if (Height <= 190f)
            // {
            //     Height = 190f;
            // }

            // Sound.Listener = new Transform(pawn.EyePosition, Camera.Rotation);

			// Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 50f );
			// Camera.FirstPersonViewer = null;

            // var center = pawn.Position + Vector3.Up * 50;

            // var pos = center;
            // var rot = Rotation.LookAt(pawn.Rotation.Down);

            // var target = pawn.Position.WithZ(pawn.Position.z + Height);            
            // var newCamPosition = new Vector3(Camera.Position.LerpTo(target, Time.Delta * MoveSpeed));

            // var newRotationCam = Rotation.Lerp(rot, Rotation.LookAt(Vector3.Down), Time.Delta * 10f);

            // var tr = Trace.Ray(pos, newCamPosition)
            //     .WithAnyTags("world")
            //     .WithoutTags("wall")
            //     .Radius(8)
            //     .Run();

            // Camera.Rotation = Rotation.LookAt(Vector3.Down);
            // Camera.Position = tr.EndPosition;



        }
    }
}
