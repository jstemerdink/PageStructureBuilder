using System;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace PageStructureBuilder
{
    public abstract class DateStructureBase<TYear, TMonth, TDay> : PageData, IOrganizeChildren
        where TYear : PageData 
        where TMonth : PageData 
        where TDay : PageData
    {
        protected Injected<IContentRepository> ContentRespository { get; set; }
        protected Injected<IContentTypeRepository> ContentTypeRespository { get; set; } 
        
        public PageReference GetParentForPage(PageData page)
        {
            if (page is TYear)
                return PageLink;

            var pageDate = GetStructureDate(page);
            var structureHelper = new StructureHelper(ContentRespository.Service, ContentTypeRespository.Service);
            
            var yearPage = structureHelper.GetOrCreateChildPage<TYear>(PageLink, pageDate.Year.ToString());
            var monthPage = structureHelper.GetOrCreateChildPage<TMonth>(yearPage.PageLink, pageDate.Month.ToString());
            var dayPage = structureHelper.GetOrCreateChildPage<TDay>(monthPage.PageLink, pageDate.Day.ToString());
            
            return dayPage.PageLink;
        }

        protected virtual DateTime GetStructureDate(PageData pageToGetParentFor)
        {
            return pageToGetParentFor.Created;
        }
    }
}
