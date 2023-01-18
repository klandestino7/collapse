using Sandbox;

namespace NxtStudio.Collapse;

public partial class TopDownCamera
{

    public float MinZoom = 200f;//     
    public float MaxZoom = 900f;
    
    public float Radius = 50f;
    public float ZoomLevel { get; set; }

    public Vector3 LookAt { get; set; }

    public Vector3 camOffset { get; set; }

    public float cos { get; set; }
    public float sin { get; set; }


    public void Update()
    {
        var player = CollapsePlayer.Me;
        ZoomLevel += Input.MouseWheel * Time.Delta * 8f;
        ZoomLevel = ZoomLevel.Clamp( 0f, 1f );

        Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 30f );

        var lookAtPosition = (LookAt * Time.Delta);

        var center = player.Position + Vector3.Up * 50;

        lookAtPosition.x = center.x;
        lookAtPosition.y = center.y;

        LookAt = center;

        Vector3 camPos;

        camPos = LookAt + Vector3.Backward * (MinZoom - (MinZoom * ZoomLevel * 0.6f));
        camPos += (Vector3.Right - 1f) * (MinZoom - (MinZoom * ZoomLevel * 0.6f));
        camPos += Vector3.Up * (MaxZoom - (MaxZoom * ZoomLevel * 0.6f));
        
        var tr = Trace.Ray(LookAt, camPos)
            .WithAnyTags("world")
            .WithoutTags("wall")
            .Radius(2)
            .Run();

        camOffset = LookAt - camPos;
    
        Camera.Rotation = Rotation.Slerp( Camera.Rotation, Rotation.LookAt( camOffset, Vector3.Up ), Time.Delta * 8f );

        // Camera.Rotation = Rotation.LookAt( Vector3.Down ); 

        Sound.Listener = new Transform()
        {
            Position = lookAtPosition,
            Rotation = Camera.Rotation
        };

        DebugOverlay.Line(LookAt, tr.EndPosition, Color.Blue, 0);

        Camera.Position = tr.EndPosition;

    
        var target = player.Position.WithZ( player.Position.z + 450f );
        // Camera.Position = Camera.Position.LerpTo( target, Time.Delta * 20f );
    
        Camera.FirstPersonViewer = null;



    }
    //     ZoomLevel += Input.MouseWheel * Time.Delta * 8f;
    //     ZoomLevel = ZoomLevel.Clamp( 0f, 1f );

    //     Radius = 50f;
    //     MinZoom = 70f;

    //     var player = CollapsePlayer.Me;

    //     var center = player.Position;

    //     var lookAtX = center.x;
    //     var lookAtY = center.y;
    //     var zPos = center.z + MinZoom;
    
    //     // Log.Info( $"cos {cos} sin {sin}");

    //     var X_deg0 = lookAtX + ( Radius * cos  );
    //     var Y_deg0 = lookAtY + ( Radius * sin  );

    //     var camPosition = new Vector3(X_deg0, Y_deg0, zPos + ( Radius * ZoomLevel ));

    //     Sound.Listener = new Transform( center, Camera.Rotation );

    //     Camera.Position = camPosition;
	// 	Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 50f );
	// 	Camera.FirstPersonViewer = null; 

    //     // Camera.Rotation = Rotation.Slerp( Camera.Rotation, Rotation.LookAt( center, Vector3.Up ), Time.Delta * 8f );

    //     var rotationPos = new Vector3(Vector3.Down.x, Vector3.Down.x, Vector3.Down.z);

    //     Camera.Rotation = Rotation.LookAt( Vector3.Down );
    // }

    

    // public void FrameSimulate()
	// {
    //     for (float i = 0; i < 360; i += 0.5f ) 
    //     {
    //         double radians = (Math.PI / 180) * i;
    //         cos = (float)Math.Cos(radians);
    //         sin = (float)Math.Sin(radians);
    //     }
	// }
}