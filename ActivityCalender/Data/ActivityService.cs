using ActivityCalender.Core;
using Ical.Net.CalendarComponents;
using SimpleFeedReader;
using System.Drawing;
using System.Net;

namespace ActivityCalender.Data
{
	public class ActivityService
	{
		private static readonly string[] Summaries = new[]
		{
			"Norrcup", "Ungdomsträning", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		public Task<ILookup<string, Activity>> GetActivitiesAsync(IEnumerable<ActivityTypes> activityTypes
			, int daysHorizon
			)
		{
			var fromDate = DateTime.Now.AddDays(-10);
			if(daysHorizon == 0)
			{
				daysHorizon = 100;
			}

			var toDate = DateTime.Now.AddDays(daysHorizon);

			var webSites = GetWebSites(activityTypes);

			using (var webClient = new HttpClient())
			{
				var activities = new List<Activity>();
				foreach (var webSite in webSites)
				{
					if (webSite.Key.IsRss())
					{
						var reader = new FeedReader();
						var items = reader.RetrieveFeed(webSite.Value);
						foreach (var item in items.Where(x => x.PublishDate >= fromDate && x.PublishDate <= toDate))
						{
							activities.Add(new Activity(webSite.Key, item));
						}
					}
					else
					{
						var events = LoadCalendar(fromDate
							, toDate
							, webSite
							, webClient
							);

						activities.AddRange(events);
					}
				}

				if(!activities.Any())
				{
					activities.Add(Activity.Empty);
				}

				var sortedActivities = activities
					.OrderBy(x => x.CalendarDateTime)
					.ToLookup(x => x.CalendarDateTime.Date)
					.ToDictionary(x => x.Key, y => y.Select(z => z));

				this.AddWeekends(activityTypes, sortedActivities);

				return Task.FromResult(sortedActivities.OrderBy(x => x.Key).SelectMany(x => x.Value).ToLookup(x => x.YearMonth));
			}
		}

		private void AddWeekends(IEnumerable<ActivityTypes> activityTypes, IDictionary<DateTime, IEnumerable<Activity>> activities)
		{
			var addWeekEnds = activityTypes.Contains(ActivityTypes.SaturDayAndSundays);

			var date = activities.First().Key.AddDays(1);
			var lastDate = activities.Last().Key;

			while(date < lastDate)
			{
				if (!activities.ContainsKey(date))
				{
					if (addWeekEnds && (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) || date == DateTime.UtcNow.Date)
					{
						var newActivity = new Activity(ActivityTypes.SaturDayAndSundays, date);


						if (newActivity.IsToday)
						{
							newActivity = new Activity(ActivityTypes.Today, date);
						}

						activities[date] = new[]
						{
							newActivity
						};
					}
				}

				date = date.AddDays(1);
			}
			
		}

		private static IEnumerable<Activity> LoadCalendar(DateTime fromDate
			, DateTime toDate, KeyValuePair<ActivityTypes, string> webSite
			, HttpClient webClient
			)
		{
			try
			{
				var html = ResolveHtml(webSite.Value, webClient);

				var calendar = Ical.Net.Calendar.Load(html);

				var events = calendar
					.Events
					.Where(x => x.Start.Date >= fromDate && x.Start.Date <= toDate)
					.Select(x => new Activity(webSite.Key, x));

				return events;
			}
			catch (Exception e)
			{
				return new[] { new Activity(webSite.Key, e) };
			}
		}

		private static Dictionary<ActivityTypes, string> GetWebSites(IEnumerable<ActivityTypes> activityTypes)
		{
			var webSites = new Dictionary<ActivityTypes, string>();
			var filteredActivityTypes = new HashSet<ActivityTypes>();

			foreach (ActivityTypes activityType in activityTypes)
			{
				switch (activityType)
				{
					case ActivityTypes.Training:
					case ActivityTypes.Meetings:
					case ActivityTypes.Education:
					case ActivityTypes.Camp:
					case ActivityTypes.Other:
					case ActivityTypes.AnnualMeeting:
					case ActivityTypes.BoardMeeting:
						webSites.Add(activityType, $@"https://idrottonline.se/Calendar/ICalExport.aspx?calendarId=41602&activityTypeIds={(int)activityType}&calendarName=Aktivitetskalender");
						break;
					case ActivityTypes.Eventor:
						webSites.Add(activityType, @"https://eventor.orientering.se/Events/ExportICalendarEvents?organisations=11");
						break;
					case ActivityTypes.SaturDayAndSundays:
						break;
					case ActivityTypes.Holidays:
						//webSites.Add(activityType, $@"https://kalender.link/ical/all");
						webSites.Add(activityType, $@"https://www.officeholidays.com/ics-local-name/sweden");
						break;
					case ActivityTypes.NewsRF:
						webSites.Add(activityType, $@"https://www.svenskorientering.se/Nyheter/forbundsnytt/?rss=True");
						break;
					case ActivityTypes.NewsOrientation:
						webSites.Add(activityType, $@"https://www.svenskorientering.se/lastasidor/IONF?complete=2");
						break;
					case ActivityTypes.NewsHOK:
						webSites.Add(activityType, $@"https://hudiksvallsoknyheter.blogspot.com/feeds/posts/default");
						break;
					default:
						break;
				}
			};


			return webSites;
		}

		private static string ResolveHtml(string webSite, HttpClient webClient)
		{
			try
			{
				using (HttpResponseMessage response = webClient.GetAsync(webSite).Result)
				{
					using (HttpContent content = response.Content)
					{
						var html = content.ReadAsStringAsync().Result;

						return html;
					}
				}
			}
			catch (Exception e)
			{
				throw new WebException(webSite, e);

			}
		}

	}
}