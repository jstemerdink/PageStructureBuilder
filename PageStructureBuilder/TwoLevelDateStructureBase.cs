namespace PageStructureBuilder
{
    using System;

    using EPiServer.Core;

    /// <summary>
    /// Class TwoLevelDateStructureBase.
    /// Implements the <see cref="PageData" />
    /// Implements the <see cref="IOrganizeChildren" />
    /// </summary>
    /// <typeparam name="TYear">The type of <see cref="PageData"/> to use for the year container.</typeparam>
    /// <typeparam name="TMonth">The type of <see cref="PageData"/> to use for the month container.</typeparam>
    /// <seealso cref="PageData" />
    /// <seealso cref="IOrganizeChildren" />
    public abstract class TwoLevelDateStructureBase<TYear, TMonth> : PageData, IOrganizeChildren
        where TYear : PageData where TMonth : PageData 
    {
        /// <summary>
        /// Gets the parent for page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The <see cref="ContentReference"/> for the parent of the <paramref name="page"/>.</returns>
        /// <exception cref="EPiServer.ServiceLocation.ActivationException">if there is are errors resolving the service instance.</exception>
        public ContentReference GetParentForPage(PageData page)
        {
            if (page is TYear)
            {
                return this.ContentLink;
            }

            DateTime pageDate = this.GetStructureDate(pageToGetParentFor: page);

            StructureHelper structureHelper = new StructureHelper();

            TYear yearPage = structureHelper.GetOrCreateChildPage<TYear>(
                parentLink: this.PageLink,
                pageDate.Year.ToString(this.MasterLanguage));

            if (yearPage == null)
            {
                return this.ContentLink;
            }

            TMonth monthPage = structureHelper.GetOrCreateChildPage<TMonth>(
                parentLink: yearPage.PageLink,
                pageDate.Month.ToString(this.MasterLanguage));

            return monthPage == null ? yearPage.ContentLink : monthPage.PageLink;
        }

        /// <summary>
        /// Gets the structure date.
        /// </summary>
        /// <param name="pageToGetParentFor">The page to get parent for.</param>
        /// <returns>The date.</returns>
        protected virtual DateTime GetStructureDate(PageData pageToGetParentFor)
        {
            return pageToGetParentFor?.Created ?? DateTime.Now;
        }
    }
}