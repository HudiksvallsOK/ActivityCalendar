namespace ActivityCalendarCore.Core
{
	public static class DoubleExtensions
	{
		public static string AsPosition(this double position)
		{
			return position.ToString().Replace(',', '.');
		}
	}
}
