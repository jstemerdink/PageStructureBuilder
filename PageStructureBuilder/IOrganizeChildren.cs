using EPiServer.Core;

namespace PageStructureBuilder
{
    public interface IOrganizeChildren
    {
        ContentReference GetParentForPage(PageData content);
    }
}
