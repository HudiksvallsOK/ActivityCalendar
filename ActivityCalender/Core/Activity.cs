using ActivityCalender.Data;
using Ical.Net.CalendarComponents;
using SimpleFeedReader;
using System.Drawing;
using System.Globalization;

namespace ActivityCalender.Core
{
	public class Activity
	{
		private Activity(ActivityTypes activityType)
		{
			this.ActivityType = activityType;
			this.LocalCultureInfo = new CultureInfo("sv-SE");
		}

		public static Activity Empty => new(ActivityTypes.Other, calendarEvent: null);

		public Activity(ActivityTypes activityType, CalendarEvent? calendarEvent) : this(activityType)
		{
			this.CalendarEvent = calendarEvent;

			this.Summary = this.CalendarEvent?.Summary;
			this.Description = activityType == ActivityTypes.Holidays ? null : calendarEvent?.Description;
			var now = DateTime.UtcNow;
			this.CalendarDateTime = this.CalendarEvent?.Start.AsSystemLocal ?? now;
			this.Week = this.Calendar.GetWeekOfYear(this.CalendarEvent?.Start.Date ?? now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
			this.Times = this.GetActivityTimes();

			this.HasLocationCoordinates = this.CalendarEvent?.Location != null && this.CalendarEvent.Location.Contains("°");
			if (activityType != ActivityTypes.Holidays)
			{
				this.Location = this.HasLocationCoordinates
					? "Karta..."
					: this.CalendarEvent?.Location;
			}

			this.CalenderEventLink = this.GetCalenderEventLink();

			this.Initialize();
		}

		public Activity(ActivityTypes activityType, DateTime date) : this(activityType)
		{
			this.CalendarDateTime = date;
			this.Summary = date.ToString("dddd", this.LocalCultureInfo);
			this.Week = this.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
			this.Times = this.GetActivityTimes();

			this.Initialize();
		}

		public Activity(ActivityTypes activityType, Exception e) : this(activityType)
		{
			this.Summary = e.Message;
			this.Description = e.ToString();

			var now = DateTime.UtcNow;

			this.CalendarDateTime = now;
			this.Week = this.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
			this.Times = this.GetActivityTimes();

			this.Initialize();
		}

		public Activity(ActivityTypes activityType, FeedItem feedItem) : this(activityType)
		{
			this.FeedItem = feedItem;

			this.Summary = feedItem.Title;
			this.Description = feedItem.Content ?? feedItem.Summary;
			this.CalendarDateTime = feedItem.LastUpdatedDate.Date;
			this.Week = this.Calendar.GetWeekOfYear(this.CalendarDateTime.Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
			this.Times = feedItem.LastUpdatedDate.ToString("HH:mm", this.LocalCultureInfo);

			this.CalenderEventLink = feedItem.Uri.ToString() ?? feedItem.Id;

			this.Initialize();
		}

		private void Initialize()
		{
			this.BackGroundColor = this.ActivityType.GetBackGroundColor();
		}

		public string Id
		{
			get
			{
				var lastSegment = this.Uri?.Segments.Last();

				if(long.TryParse(lastSegment, out var id2))
				{
					return this.CalendarDateTime.Date.Date.ToShortDateString() + ":" + lastSegment;
				}

				return this.FeedItem?.Id ?? CalendarDateTime.Ticks.ToString();
			}
		}

		private string GetActivityTimes()
		{
			var times = String.Empty;

			if (this.CalendarEvent == null)
			{
				return times;
			}

			if (this.CalendarEvent.IsAllDay || this.CalendarEvent.Duration.Hours >= 23)
			{
				return times + "Hela dagen";
			};

			times += this.CalendarDateTime.ToString("HH:mm", this.LocalCultureInfo);

			if (this.CalendarDateTime != this.CalendarEvent.End.AsSystemLocal)
			{
				times += "-" + this.CalendarEvent.End.AsSystemLocal.ToString("HH:mm", this.LocalCultureInfo);
			}

			return times;
		}

		public Calendar Calendar => this.LocalCultureInfo.Calendar;

		public string? Description { get; }

		public DateTimeOffset CalendarDateTime { get; }

		public string YearMonth => this.CalendarDateTime.ToString("yyyy MMMM", this.LocalCultureInfo);
		public int Week { get; }
		public string Day => this.CalendarDateTime.ToString("ddd d", this.LocalCultureInfo);
		public string? Times { get; }
		public string? Summary { get; }

		public bool HasLocationCoordinates { get; }

		public bool HasLocation => !string.IsNullOrEmpty(this.GeoLocationLink) || !string.IsNullOrEmpty(this.Location);

		public string? Location { get; }

		public string? GeoLocationLink
		{
			get
			{
				if (this.CalendarEvent?.GeographicLocation == null && !this.HasLocationCoordinates)
				{
					return null;
				}

				var href = "https://www.google.com/maps/search/?api=1&query=";

				var location = this.CalendarEvent?.GeographicLocation == null
					? this.CalendarEvent?.Location
					: $"E{this.CalendarEvent.GeographicLocation.Longitude.AsPosition()},N{this.CalendarEvent.GeographicLocation.Latitude.AsPosition()}";

				return string.IsNullOrEmpty(location) ? null : $"{href}{location}";
			}
		}

		public string? CalenderEventLink { get; }

		private string? GetCalenderEventLink()
		{
			if (this.CalendarEvent?.Uid == null)
			{
				return null;
			}

			var uid = this.CalendarEvent.Uid.Replace("Activity", "").Replace("@idrottonline.se", "");

			if (long.TryParse(uid, out var id))
			{
				var href = $"activity.idrottonline.se/Activities/Edit/{id}?referrer=calendar&calendarId=41602";

				return href;
			}

			return null;
		}

		public Uri? Uri => this.CalendarEvent?.Url;

		public ActivityTypes ActivityType { get; }
		public FeedItem? FeedItem { get; }
		public CalendarEvent? CalendarEvent { get; }
		public CultureInfo LocalCultureInfo { get; }

		public Color BackGroundColor { get; set; }


		public bool IsToday => this.CalendarDateTime.Date == DateTime.UtcNow.Date;
	}
}