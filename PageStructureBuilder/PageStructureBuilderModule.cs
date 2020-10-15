namespace PageStructureBuilder
{
    using System.Collections.Generic;
    using System.Linq;

    using EPiServer;
    using EPiServer.Core;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.Security;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// Class PageStructureBuilderModule.
    /// Implements the <see cref="EPiServer.Framework.IInitializableModule" />
    /// </summary>
    /// <seealso cref="EPiServer.Framework.IInitializableModule" />
    [InitializableModule]
    public class PageStructureBuilderModule : IInitializableModule
    {
        /// <summary>
        /// The content events
        /// </summary>
        private IContentEvents contentEvents;

        /// <summary>
        /// The content loader
        /// </summary>
        private IContentLoader contentLoader;

        /// <summary>
        /// The content repository
        /// </summary>
        private IContentRepository contentRepository;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <remarks>Gets called as part of the EPiServer Framework initialization sequence. Note that it will be called
        /// only once per AppDomain, unless the method throws an exception. If an exception is thrown, the initialization
        /// method will be called repeatedly for each request reaching the site until the method succeeds.</remarks>
        /// <exception cref="ActivationException">if there is are errors resolving the service instance.</exception>
        public void Initialize(InitializationEngine context)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            this.contentEvents = context.Locate.Advanced.GetInstance<IContentEvents>();
            this.contentLoader = context.Locate.Advanced.GetInstance<IContentLoader>();
            this.contentRepository = context.Locate.Advanced.GetInstance<IContentRepository>();
#pragma warning restore CA1062 // Validate arguments of public methods

            this.contentEvents.CreatingContent += this.ContentEventsOnCreatingContent;
            this.contentEvents.MovingContent += this.ContentEventsOnMovingContent;
        }

        /// <summary>
        /// Resets the module into an uninitialized state.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <remarks><para>
        /// This method is usually not called when running under a web application since the web app may be shut down very
        /// abruptly, but your module should still implement it properly since it will make integration and unit testing
        /// much simpler.
        /// </para>
        /// <para>
        /// Any work done by <see cref="IInitializableModule.Initialize(InitializationEngine)" /> as well as any code executing on <see cref="InitializationEngine.InitComplete" /> should be reversed.
        /// </para></remarks>
        public void Uninitialize(InitializationEngine context)
        {
            this.contentEvents.CreatingContent -= this.ContentEventsOnCreatingContent;
            this.contentEvents.MovingContent -= this.ContentEventsOnMovingContent;
        }

        /// <summary>
        /// Check if the <paramref name="queriedParents"/> contains the <paramref name="parentLink"/>.
        /// </summary>
        /// <param name="queriedParents">The queried parents.</param>
        /// <param name="parentLink">The parent link.</param>
        /// <returns><c>true</c> if The <paramref name="queriedParents"/> contains the <paramref name="parentLink"/>, <c>false</c> otherwise.</returns>
        private static bool ListContains(IEnumerable<ContentReference> queriedParents, ContentReference parentLink)
        {
            return !queriedParents.Any(p => p.CompareToIgnoreWorkID(contentReference: parentLink));
        }

        /// <summary>
        /// On creating content.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="contentEventArgs">The <see cref="ContentEventArgs"/> instance containing the event data.</param>
        private void ContentEventsOnCreatingContent(object sender, ContentEventArgs contentEventArgs)
        {
            if (!(contentEventArgs.Content is PageData))
            {
                return;
            }

            ContentReference parentLink = contentEventArgs.Content.ParentLink;
            PageData page = contentEventArgs.Content as PageData;

            parentLink = this.GetNewParent(parentLink, page: page);

            contentEventArgs.Content.ParentLink = parentLink;
        }

        /// <summary>
        /// On moving content.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="contentEventArgs">The <see cref="ContentEventArgs"/> instance containing the event data.</param>
        private void ContentEventsOnMovingContent(object sender, ContentEventArgs contentEventArgs)
        {
            if (!(contentEventArgs.Content is PageData))
            {
                return;
            }

            ContentReference parentLink = contentEventArgs.TargetLink;
            PageData page = this.GetPage(contentLink: contentEventArgs.ContentLink) as PageData;

            if (page == null)
            {
                return;
            }

            parentLink = this.GetNewParent(parentLink, page: page);

            if (ContentReference.IsNullOrEmpty(parentLink)
                || contentEventArgs.TargetLink.CompareToIgnoreWorkID(contentReference: parentLink))
            {
                return;
            }

            this.contentRepository.Move(
                contentLink: page.ContentLink,
                destination: parentLink,
                requiredSourceAccess: AccessLevel.NoAccess,
                requiredDestinationAccess: AccessLevel.NoAccess);
        }

        /// <summary>
        /// Gets the children organizer.
        /// </summary>
        /// <param name="pageLink">The page link.</param>
        /// <returns>The <see cref="IOrganizeChildren"/> implementation.</returns>
        private IOrganizeChildren GetChildrenOrganizer(ContentReference pageLink)
        {
            if (ContentReference.IsNullOrEmpty(contentLink: pageLink))
            {
                return null;
            }

            return this.GetPage(contentLink: pageLink) as IOrganizeChildren;
        }

        /// <summary>
        /// Gets the new parent.
        /// </summary>
        /// <param name="originalParentLink">The original parent link.</param>
        /// <param name="page">The page.</param>
        /// <returns>The new parent <see cref="ContentReference"/>.</returns>
        private ContentReference GetNewParent(ContentReference originalParentLink, PageData page)
        {
            List<ContentReference> queriedParents = new List<ContentReference>();
            IOrganizeChildren organizingParent = this.GetChildrenOrganizer(pageLink: originalParentLink);

            ContentReference parentLink = originalParentLink;

            while (organizingParent != null && ListContains(queriedParents: queriedParents, parentLink: parentLink))
            {
                queriedParents.Add(item: parentLink);
                ContentReference newParentLink = organizingParent.GetParentForPage(content: page);

                if (!ContentReference.IsNullOrEmpty(newParentLink))
                {
                    parentLink = newParentLink;
                }

                organizingParent = this.GetChildrenOrganizer(pageLink: parentLink);
            }

            return parentLink;
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="contentLink">The content link.</param>
        /// <returns>The <see cref="IContent"/> instance.</returns>
        private IContent GetPage(ContentReference contentLink)
        {
            return this.contentLoader.Get<IContent>(contentLink: contentLink);
        }
    }
}