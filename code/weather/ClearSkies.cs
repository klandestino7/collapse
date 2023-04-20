using Sandbox;

namespace NxtStudio.Collapse;

public class ClearSkies : WeatherCondition
{
	private TimeUntil NextAttemptToChange { get; set; }

	public override void OnStarted()
	{
		if ( Game.IsServer )
		{
			NextAttemptToChange = Game.Random.Float( 30f, 60f );
		}

		base.OnStarted();
	}

	public override void ServerTick()
	{
		if ( NextAttemptToChange )
		{
			if ( Game.Random.Float() < 0.3f )
			{
				WeatherSystem.Change( new RainyWeather
				{
					Intensity = Game.Random.Int( 1, 3 )
				} );
			}

			NextAttemptToChange = Game.Random.Float( 30f, 60f );
		}

		base.ServerTick();
	}
}

