namespace ActivityCalendar.Data
{
	[AttributeUsage(AttributeTargets.All)]
	public class TranslationAttribute : Attribute
	{
		public TranslationAttribute(string translation)
		{
			this.Translation = translation;
		}

		public string Translation { get; }
	}
}