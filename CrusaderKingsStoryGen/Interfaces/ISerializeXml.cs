// <copyright file="ISerializeXml.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Interfaces
{
    using System.Xml;

    interface ISerializeXml
    {
        void SaveProject(XmlWriter writer);

        void LoadProject(XmlReader reader);
    }
}