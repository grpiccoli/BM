using BiblioMit.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace BiblioMit.Services
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _resourceKey;
        private readonly ResourceManager _resource;
        public LocalizedDescriptionAttribute(
            string resourceKey,
            Type resourceType)
        {
            _resource = new ResourceManager(resourceType);
            _resourceKey = resourceKey;
        }

        public override string Description
        {
            get
            {
                string displayName = _resource.GetString(_resourceKey);

                return string.IsNullOrEmpty(displayName)
                    ? string.Format("{0}", _resourceKey)
                    : displayName;
            }
        }
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return enumValue.ToString();
        }

        public static string Localize(this Enum e, string attribute, string lang)
        {
            if (lang.Contains("es"))
            {
                var rm = new ResourceManager(typeof(EnumResources));
                var name = e.GetType().Name + "_" + e + "_" + attribute;
                var resourceDisplayName = rm.GetString(name);
                return string.IsNullOrWhiteSpace(resourceDisplayName) ? GetAttr(e,attribute) : resourceDisplayName;
            }
            return GetAttr(e,attribute);
        }

        public static string GetAttr(this Enum e, string attribute)
        {
            switch (attribute)
            {
                case "Prompt":
                    {
                        return e.GetType()
                            .GetMember(e.ToString())
                          .FirstOrDefault()
                          ?.GetCustomAttribute<DisplayAttribute>(false)
                          ?.Prompt
                          ?? e.ToString();
                    }
                case "Description":
                    {
                        return e.GetType()
                            .GetMember(e.ToString())
                          .FirstOrDefault()
                          ?.GetCustomAttribute<DisplayAttribute>(false)
                          ?.Description
                          ?? e.ToString();
                    }
                case "GroupName":
                    {
                        return e.GetType()
                            .GetMember(e.ToString())
                          .FirstOrDefault()
                          ?.GetCustomAttribute<DisplayAttribute>(false)
                          ?.GroupName
                          ?? e.ToString();
                    }
                case "Name":
                default:
                    {
                        return e.GetType()
                            .GetMember(e.ToString())
                          .FirstOrDefault()
                          ?.GetCustomAttribute<DisplayAttribute>(false)
                          ?.Name
                          ?? e.ToString();
                    }
            }
        }

        public static string GetDisplayName(this Enum e, string lang)
        {
            if (lang.Contains("es"))
            {
                var rm = new ResourceManager(typeof(EnumResources));
                var name = e.GetType().Name + "_" + e;
                var resourceDisplayName = rm.GetString(name);
                return string.IsNullOrWhiteSpace(resourceDisplayName) ? string.Format("{0}", e) : resourceDisplayName;
            }
            else
            {
                return e.GetType()
                    .GetMember(e.ToString())
                  .FirstOrDefault()
                  ?.GetCustomAttribute<DisplayAttribute>(false)
                  ?.Name
                  ?? e.ToString();
            }
        }
    }
}
