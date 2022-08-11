
using ActivityCalendarCore.Data;

namespace ActivityCalendarCore.Core
{
	public class AvtivityTypeVmFactory
	{
		public HashSet<AvtivityTypeVm> Create()
		{
			var activityTypes = new HashSet<AvtivityTypeVm>();

			var html = string.Empty;

			foreach (ActivityTypes value in typeof(ActivityTypes).GetEnumValues())
			{
				if (value == ActivityTypes.Today)
				{
					continue;
				}

				var id = value.ToString();

				var label = value.GetAttributeValue<TranslationAttribute, string>(x => x.Translation) ?? id;

				activityTypes.Add(new AvtivityTypeVm(value, !value.IsRss(), id, label));
			}

			return activityTypes;
		}
	}
}
