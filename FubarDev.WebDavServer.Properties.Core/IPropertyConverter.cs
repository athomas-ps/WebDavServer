﻿using System.Xml.Linq;

using JetBrains.Annotations;

namespace FubarDev.WebDavServer.Properties
{
    public interface IPropertyConverter<T>
    {
        [NotNull]
        T FromElement([NotNull] XElement element);

        [NotNull]
        XElement ToElement(XName name, [NotNull] T value);
    }
}
