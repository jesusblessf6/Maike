using System;
using System.ComponentModel;
using System.Reflection;

namespace Enums
{
	public class EnumHelper
	{
		public static string GetDescription<TEnum>(TEnum e)
		{
			var type = e.GetType();
			var enumName = Enum.GetName(type, e);
			if (enumName == null)
			{
				return null;
			}

			FieldInfo field = type.GetField(enumName);
			var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);

			if (attributes.Length > 0)
			{
				return attributes[0].Description;
			}

			return enumName;
		}
	}
}
