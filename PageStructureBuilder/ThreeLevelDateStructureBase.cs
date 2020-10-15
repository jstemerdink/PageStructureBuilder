namespace PageStructureBuilder
{
    using System;

    using EPiServer.Core;

    /// <summary>
    /// Class ThreeLevelDateStructureBase.
    /// Implements the <see cref="PageData" />
    /// Implements the <see cref="IOrganizeChildren" />
    /// </summary>
    /// <typeparam name="TYear">The type of the t year.</typeparam>
    /// <typeparam name="TMonth">The type of the t month.</typeparam>
    /// <typeparam name="TDay">The type of the t day.</typeparam>
    /// <seealso cref="PageData" />
    /// <seealso cref="IOrganizeChildren" />
    public abstract class ThreeLevelDateStructureBase<TYear, TMonth, TDay> : PageData, IOrganizeChildren
        where TYear : PageData where TMonth : PageData where TDay : PageData
    {
        /// <summary>
        /// Gets the parent for page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The parent <see cref="ContentReference"/>.</returns>
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

            TMonth monthPage = structureHelper.GetOrCreateChildPage<TMonth>(
                parentLink: yearPage.PageLink,
                pageDate.Month.ToString(this.MasterLanguage));

            TDay dayPage = structureHelper.GetOrCreateChildPage<TDay>(
                parentLink: monthPage.PageLink,
                pageDate.Day.ToString(this.MasterLanguage));

            return dayPage.PageLink;
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