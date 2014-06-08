using EPiServer.Core;

namespace PageStructureBuilder
{
    public abstract class PageTypeStructureBase<TContainer> : SingleLevelStructureBase<TContainer> where TContainer : PageData
    {
        protected override string GetContainerPageName(PageData childPage)
        {
            var pageType = ContentTypeRespository.Service.Load(childPage.PageTypeID);
            return pageType.Name;
        }
    }
}
