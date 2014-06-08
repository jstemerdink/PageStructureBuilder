using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace PageStructureBuilder
{
    public abstract class SingleLevelStructureBase<TContainer> : PageData, IOrganizeChildren where TContainer : PageData
    {
        protected Injected<IContentRepository> ContentRespository { get; set; }
        protected Injected<IContentTypeRepository> ContentTypeRespository { get; set; } 
        
        public virtual PageReference GetParentForPage(PageData page)
        {
            if (page is TContainer)
                return PageLink;

            if (string.IsNullOrEmpty(page.PageName))
                return PageLink;

            var structureHelper = new StructureHelper(ContentRespository.Service, ContentTypeRespository.Service);

            var container = structureHelper.GetOrCreateChildPage<TContainer>(PageLink, GetContainerPageName(page));
            return container.PageLink;
        }

        protected abstract string GetContainerPageName(PageData childPage);
    }
}
