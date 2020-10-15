namespace PageStructureBuilder
{
    using EPiServer.Core;

    /// <summary>
    /// Class SingleLevelStructureBase.
    /// Implements the <see cref="EPiServer.Core.PageData" />
    /// Implements the <see cref="PageStructureBuilder.IOrganizeChildren" />
    /// </summary>
    /// <typeparam name="TContainer">The type of the t container.</typeparam>
    /// <seealso cref="EPiServer.Core.PageData" />
    /// <seealso cref="PageStructureBuilder.IOrganizeChildren" />
    public abstract class SingleLevelStructureBase<TContainer> : PageData, IOrganizeChildren
        where TContainer : PageData
    {
        /// <summary>
        /// Gets the parent for page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The parent <see cref="ContentReference"/>.</returns>
        /// <exception cref="EPiServer.ServiceLocation.ActivationException">if there is are errors resolving the service instance.</exception>
        public virtual ContentReference GetParentForPage(PageData page)
        {
            if (page == null)
            {
                return this.ContentLink;
            }

            if (page is TContainer)
            {
                return this.ContentLink;
            }

            if (string.IsNullOrEmpty(value: page.PageName))
            {
                return this.ContentLink;
            }

            StructureHelper structureHelper = new StructureHelper();

            TContainer container = structureHelper.GetOrCreateChildPage<TContainer>(
                parentLink: this.ContentLink,
                this.GetContainerPageName(childPage: page));

            return container.PageLink;
        }

        /// <summary>
        /// Gets the name of the container page.
        /// </summary>
        /// <param name="childPage">The child page.</param>
        /// <returns>The name of the container page.</returns>
        protected abstract string GetContainerPageName(PageData childPage);
    }
}