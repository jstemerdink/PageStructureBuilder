namespace PageStructureBuilder
{
    using EPiServer.Core;

    /// <summary>
    /// Interface IOrganizeChildren
    /// </summary>
    public interface IOrganizeChildren
    {
        /// <summary>
        /// Gets the parent for page.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The <see cref="ContentReference"/> for the parent of the <paramref name="content"/>.</returns>
        ContentReference GetParentForPage(PageData content);
    }
}