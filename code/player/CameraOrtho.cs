using Sandbox;

namespace NxtStudio.Collapse;

public partial class TopDownCamera
{
    public float MinZoom = 200f;//     
    public float MaxZoom = 900f;
    public float ZoomLevel { get; set; }

    public Vector3 LookAt { get; set; }

    public Vector3 camOffset { get; set; }

    public Entity lastEntityDeleted {get; set;}

    public PhysicsBody lastEntityTouch {get; set;}

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
            .WithoutTags("solid")
            .Radius(2)
            .Run();


        var roof = Trace.Ray(LookAt, camPos)
            .WithAnyTags("roof")
            .WithoutTags("hidden")
            .Radius(2)
            .Run();
        
        if ( roof.Hit || roof.Entity.IsValid() )
        {   
            if (roof.Entity is Roof )
            {
                if ( lastEntityTouch == null )
                {
                    // var cacheEntity = roof.Entity;
                    
                    var cacheEntity = (ModelEntity)roof.Entity;
                    
                    lastEntityTouch = roof.Body;
                    Log.Info($"EU SETEI A Entidade { cacheEntity}"); 
                    cacheEntity.Tags.Add( "hidden" );
                }
            }
        }

        if (!roof.Hit && lastEntityTouch != null )
        {
            var cacheEntity = lastEntityTouch.GetEntity();
            
            if (cacheEntity.IsValid())
            {
                Log.Info($" NÃO TENHO MAIS A ENTIDADE {cacheEntity}");
                cacheEntity.Tags.Remove( "hidden" );
                lastEntityTouch = null;
            }
        }

        camOffset = LookAt - camPos;
    
        Camera.Rotation = Rotation.Slerp( Camera.Rotation, Rotation.LookAt( camOffset, Vector3.Up ), Time.Delta * 8f );

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
}