namespace PageStructureBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EPiServer;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAccess;
    using EPiServer.Security;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// Class StructureHelper.
    /// </summary>
    public class StructureHelper
    {
        /// <summary>
        /// The content repository
        /// </summary>
        private readonly IContentRepository contentRepository;

        /// <summary>
        /// The content type repository
        /// </summary>
        private readonly IContentTypeRepository contentTypeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureHelper"/> class.
        /// </summary>
        /// <exception cref="EPiServer.ServiceLocation.ActivationException">if there is are errors resolving the service instance.</exception>
        public StructureHelper() : this(ServiceLocator.Current.GetInstance<IContentRepository>(), ServiceLocator.Current.GetInstance<IContentTypeRepository>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureHelper"/> class.
        /// </summary>
        /// <param name="contentRepository">The content repository.</param>
        /// <param name="contentTypeRepository">The content type repository.</param>
        public StructureHelper(IContentRepository contentRepository, IContentTypeRepository contentTypeRepository)
        {
            this.contentRepository = contentRepository;
            this.contentTypeRepository = contentTypeRepository;
        }

        /// <summary>
        /// Gets the or create child page.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="parentLink">The parent link.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns>The child page.</returns>
        public virtual TResult GetOrCreateChildPage<TResult>(ContentReference parentLink, string pageName)
            where TResult : PageData
        {
            TResult child = this.GetExistingChild<TResult>(parentLink: parentLink, pageName: pageName);

            if (child != null)
            {
                return child;
            }

            child = this.CreateChild<TResult>(parentLink: parentLink, pageName: pageName);
            return child;
        }

        /// <summary>
        /// Creates the child.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="parentLink">The parent link.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns>The child.</returns>
        private TResult CreateChild<TResult>(ContentReference parentLink, string pageName)
            where TResult : PageData
        {
            ContentType resultPageType = this.contentTypeRepository.Load(typeof(TResult));

            TResult child = this.contentRepository.GetDefault<PageData>(
                                parentLink: parentLink,
                                contentTypeID: resultPageType.ID) as TResult;

            child.PageName = pageName;

            this.contentRepository.Save(content: child, action: SaveAction.Publish, access: AccessLevel.NoAccess);

            return child;
        }

        /// <summary>
        /// Gets the existing child.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="parentLink">The parent link.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns>The existing child.</returns>
        private TResult GetExistingChild<TResult>(ContentReference parentLink, string pageName)
            where TResult : PageData
        {
            IEnumerable<PageData> children = this.contentRepository.GetChildren<PageData>(contentLink: parentLink);

            return children.OfType<TResult>().FirstOrDefault(
                c => c.PageName.Equals(value: pageName, comparisonType: StringComparison.InvariantCulture));
        }
    }
}