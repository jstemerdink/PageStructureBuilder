namespace PageStructureBuilder
{
    using System;

    using EPiServer;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// Class PageTypeStructureBase.
    /// Implements the <see cref="SingleLevelStructureBase{TContainer}" />
    /// </summary>
    /// <typeparam name="TContainer">The type of the t container.</typeparam>
    /// <seealso cref="SingleLevelStructureBase{TContainer}" />
    public abstract class PageTypeStructureBase<TContainer> : SingleLevelStructureBase<TContainer>
        where TContainer : PageData
    {

        /// <summary>
        /// The content type repository
        /// </summary>
        private readonly IContentTypeRepository contentTypeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageTypeStructureBase{TContainer}" /> class.
        /// </summary>
        /// <exception cref="ActivationException">if there is are errors resolving the service instance.</exception>
        protected PageTypeStructureBase()
        {
            this.contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
        }

        /// <summary>
        /// Gets the name of the container page.
        /// </summary>
        /// <param name="childPage">The child page.</param>
        /// <returns>The name of the container page.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="childPage"/> is <see langword="null"/></exception>
        protected override string GetContainerPageName(PageData childPage)
        {
            if (childPage == null)
            {
                throw new ArgumentNullException(nameof(childPage));
            }

            ContentType pageType = this.contentTypeRepository.Load(id: childPage.ContentTypeID);
            return pageType.Name;
        }
    }
}