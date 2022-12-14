using System.Reflection;

namespace ActivityCalendarCore.Core
{
	public static class EnumExtensions
	{
		public static Expected? GetAttributeValue<T, Expected>(this Enum enumeration, Func<T, Expected> expression)
		where T : Attribute
		{
			var attribute =
			  enumeration
				.GetType()
				.GetMember(enumeration.ToString())
				.Where(member => member.MemberType == MemberTypes.Field)
				.FirstOrDefault()?
				.GetCustomAttributes(typeof(T), false)
				.Cast<T>()
				.SingleOrDefault();

			if (attribute == null)
			{
				return default;
			}

			return expression(attribute);
		}
	}
}
