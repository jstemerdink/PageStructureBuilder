using System;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Security;

namespace PageStructureBuilder
{
    public class StructureHelper
    {
        private IContentRepository _contentRepository;
        private IContentTypeRepository _contentTypeRepository;

        public StructureHelper(IContentRepository contentRepository, IContentTypeRepository contentTypeRepository)
        {
            _contentRepository = contentRepository;
            _contentTypeRepository = contentTypeRepository;
        }

        public virtual TResult GetOrCreateChildPage<TResult>(PageReference parentLink, string pageName) where TResult : PageData
        {
            var child = GetExistingChild<TResult>(parentLink, pageName);
            if (child != null)
                return child;

            child = CreateChild<TResult>(parentLink, pageName);
            return child;
        }

        private TResult GetExistingChild<TResult>(PageReference parentLink, string pageName) where TResult : PageData
        {
            var children = _contentRepository.GetChildren<PageData>(parentLink);
            return children
                .OfType<TResult>()
                .FirstOrDefault(c => c.PageName.Equals(pageName, StringComparison.InvariantCulture));
        }

        private TResult CreateChild<TResult>(PageReference parentLink, string pageName) where TResult : PageData
        {
            TResult child;

            var resultPageType = _contentTypeRepository.Load(typeof (TResult));
            
            child = _contentRepository.GetDefault<PageData>(parentLink, resultPageType.ID) as TResult;
            child.PageName = pageName;
            
            _contentRepository.Save(child, SaveAction.Publish, AccessLevel.NoAccess);
            
            return child;
        }
    }
}
