namespace ActivityCalendarCore.Data
{
	public enum ActivityTypes
	{
		[Translation("Träning")]
		Training = 1,
		[Translation("Eventor")]
		Eventor = 2,
		[Translation("Alla sorters möten")]
		Meetings = 3,
		[Translation("Utbildning")]
		Education = 4,
		[Translation("Läger")]
		Camp = 5,
		[Translation("Övrigt")]
		Other = 6,
		[Translation("Årsmöte")]
		AnnualMeeting = 7,
		[Translation("Styrelsemöte")]
		BoardMeeting = 8,
		[Translation("Lördag/Söndag")]
		SaturDayAndSundays = 94,
		[Translation("Idag")]
		Today = 95,
		[Translation("Helgdagar")]
		Holidays = 96,
		[Translation("Nyheter från förbundet")]
		NewsRF = 97,
		[Translation("Nyheter orientering")]
		NewsOrientation = 98,
		[Translation("Nyheter HOK")]
		NewsHOK = 99,
	}
}