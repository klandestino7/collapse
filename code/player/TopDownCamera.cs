using Sandbox;

namespace NxtStudio.Collapse;

public partial class TopDownCamera
{
    public float ZoomLevel { get; set; }
    public Vector3 LookAt { get; set; }
    
	public float Height { get; set; } = 650f;
	public float MoveSpeed { get; set; } = 20f;

	private float Scale { get; set; } = 1.5f;

    public float MinZoom = 10f;
    public float MaxZoom = 200f;

    public void Update()
    {
        
		var pawn = CollapsePlayer.Me;

        if ( pawn.IsValid() ) 
        {
			ZoomLevel += Input.MouseWheel * Time.Delta * 8f;
			ZoomLevel = ZoomLevel.Clamp( 0f, 1f );

			Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 30f );

			var velocity = Vector3.Zero;
			var panSpeed = MaxZoom - (MaxZoom * ZoomLevel * 0.6f);

            if ( pawn.Velocity.Length > 0 )
            {
                velocity = pawn.Velocity * panSpeed;
            }

			var lookAtPosition = (LookAt + 0.1f * Time.Delta);

            var center = pawn.Position + Vector3.Up * 50;

			lookAtPosition.x = center.x;
			lookAtPosition.y = center.y;

			LookAt = center;

			Vector3 eyePos;

			eyePos = LookAt + Vector3.Backward * (MinZoom - (MinZoom * ZoomLevel * 0.6f));
			eyePos += Vector3.Left * (MinZoom - (MinZoom * ZoomLevel * 0.6f));
			eyePos += Vector3.Up * (MaxZoom - (MaxZoom * ZoomLevel * 0.6f));

            var camPosition = Camera.Position.LerpTo( eyePos, Time.Delta * 2f );
            
            var tr = Trace.Ray(LookAt, eyePos)
                .WithAnyTags("world")
                .WithoutTags("wall")
                .Radius(8)
                .Run();

			var difference = LookAt - eyePos;
            
			Camera.Rotation = Rotation.Slerp( Camera.Rotation, Rotation.LookAt( difference, Vector3.Up ), Time.Delta * 8f );

			Sound.Listener = new Transform()
			{
				Position = lookAtPosition,
				Rotation = Camera.Rotation
			};

			Camera.Position = tr.EndPosition;

			Camera.FirstPersonViewer = null;
        }
    }

    //  dont touch here

	// public void Update()
	// {
	// 	var pawn = CollapsePlayer.Me;

	// 	if ( pawn.IsValid() )
	// 	{

    //         if (Input.MouseWheel > 0 && Height >= 190f)
    //         {
    //             Height = MathX.LerpTo(Height, 190f, Time.Delta * 10f);
    //         }
    //         if (Input.MouseWheel < 0 && Height <= 650f)
    //         {
    //             Height = MathX.LerpTo(Height, 650f, Time.Delta * 10f);
    //         }

    //         if (Height >= 650f)
    //         {
    //             Height = 650f;
    //         }
    //         else if (Height <= 190f)
    //         {
    //             Height = 190f;
    //         }

    //         Sound.Listener = new Transform(pawn.EyePosition, Camera.Rotation);

	// 		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 50f );
	// 		Camera.FirstPersonViewer = null;

    //         var center = pawn.Position + Vector3.Up * 50;

    //         var pos = center;
    //         var rot = Rotation.LookAt(pawn.Rotation.Down);

    //         var target = pawn.Position.WithZ(pawn.Position.z + Height);            
    //         var newCamPosition = new Vector3(Camera.Position.LerpTo(target, Time.Delta * MoveSpeed));

    //         var newRotationCam = Rotation.Lerp(rot, Rotation.LookAt(Vector3.Down), Time.Delta * 10f);

    //         var tr = Trace.Ray(pos, newCamPosition)
    //             .WithAnyTags("world")
    //             .WithoutTags("wall")
    //             .Radius(8)
    //             .Run();

    //         Camera.Rotation = Rotation.LookAt(Vector3.Down);
    //         Camera.Position = tr.EndPosition;
    //     }
    // }
}
