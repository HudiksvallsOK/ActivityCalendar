using ActivityCalendar.Data;

namespace ActivityCalendar.Core
{
	public class AvtivityTypeVm
	{
		public AvtivityTypeVm(ActivityTypes activityType
			, bool isIncluded
			, string id
			, string label
			)
		{
			this.ActivityType = activityType;
			this.IsIncluded = isIncluded;
			this.Id = id;
			this.Label = label;
		}

		public ActivityTypes ActivityType { get; }
		public bool IsIncluded { get; set; }
		public string Id { get; }
		public string Label { get; }

		public void Toogle()
		{
			this.IsIncluded = !this.IsIncluded;
		}
	}
}
