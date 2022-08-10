using ActivityCalender.Pages;
using System.Drawing;

namespace ActivityCalender.Data
{
	public static class ActivityTypesExtensions
	{
		public static string? GetTranslation(this ActivityTypes activityType)
		{
			return activityType.GetAttributeValue<TranslationAttribute, string>(x => x.Translation);
		}

		public static bool IsRss(this ActivityTypes activityType)
		{
			return activityType == ActivityTypes.NewsOrientation 
				|| activityType == ActivityTypes.NewsRF
				|| activityType == ActivityTypes.NewsHOK
				;
		}

		public static Color GetBackGroundColor(this ActivityTypes activityType)
		{
			var color = Color.Transparent;

			switch (activityType)
			{
				case ActivityTypes.Training:
					color = Color.LightGreen;
					break;
				case ActivityTypes.Eventor:
					color = Color.LightBlue;
					break;
				case ActivityTypes.Education:
					color = Color.Orange;
					break;
				case ActivityTypes.Camp:
					color = Color.MediumPurple;
					break;
				case ActivityTypes.Other:
					color = Color.DarkOliveGreen;
					break;
				case ActivityTypes.Meetings:
				case ActivityTypes.AnnualMeeting:
				case ActivityTypes.BoardMeeting:
					color = Color.LightSlateGray;
					break;
				case ActivityTypes.Holidays:
				case ActivityTypes.SaturDayAndSundays:
					color = Color.Red;
					break;
				case ActivityTypes.Today:
					color = Color.Transparent;
					break;
				case ActivityTypes.NewsHOK:
					color = Color.LightSkyBlue;
					break;
				case ActivityTypes.NewsOrientation:
					color = Color.LightPink;
					break;
				case ActivityTypes.NewsRF:
					color = Color.LightSalmon;
					break;
				default:
					break;
			}

			return color;
		}

	}
}