namespace NxtStudio.Collapse
{
	public static class Vector3Extension
	{
		public static string ToCSV( this Vector3 self )
		{
			return (self.x + "," + self.y + "," + self.z);
		}
	}
}
