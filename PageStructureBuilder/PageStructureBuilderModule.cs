using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;

namespace PageStructureBuilder
{
    [InitializableModule]
    public class PageStructureBuilderModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();

            contentEvents.CreatingContent += ContentEventsOnCreatingContent;
            contentEvents.MovingContent += ContentEventsOnMovingContent;
        }

        private void ContentEventsOnCreatingContent(object sender, ContentEventArgs contentEventArgs)
        {
            if (!(contentEventArgs.Content is PageData))
                return;

            var parentLink = contentEventArgs.Content.ParentLink;
            var page = contentEventArgs.Content as PageData;
            parentLink = GetNewParent(parentLink as PageReference, page);

            contentEventArgs.Content.ParentLink = parentLink;
        }

        private void ContentEventsOnMovingContent(object sender, ContentEventArgs contentEventArgs)
        {
            if (!(contentEventArgs.Content is PageData))
                return;

            var parentLink = contentEventArgs.TargetLink;
            var page = GetPage(contentEventArgs.ContentLink) as PageData;
            parentLink = GetNewParent(parentLink as PageReference, page);

            if (!PageReference.IsValue(parentLink as PageReference) || contentEventArgs.TargetLink.CompareToIgnoreWorkID(parentLink)) 
                return;

            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            contentRepository.Move(page.ContentLink, parentLink, AccessLevel.NoAccess, AccessLevel.NoAccess);
        }

        private PageReference GetNewParent(PageReference originalParentLink, PageData page)
        {
            var queriedParents = new List<PageReference>();
            var organizingParent = GetChildrenOrganizer(originalParentLink);

            PageReference parentLink = originalParentLink;

            while (organizingParent != null && ListContains(queriedParents, parentLink))
            {
                queriedParents.Add(parentLink);
                var newParentLink = organizingParent.GetParentForPage(page);
                
                if (PageReference.IsValue(newParentLink))
                    parentLink = newParentLink;
                
                organizingParent = GetChildrenOrganizer(parentLink);
            }

            return parentLink;
        }

        private bool ListContains(IEnumerable<ContentReference> queriedParents, ContentReference parentLink)
        {
            return queriedParents.Count(p => p.CompareToIgnoreWorkID(parentLink)) == 0;
        }

        private IOrganizeChildren GetChildrenOrganizer(ContentReference pageLink)
        {
            if (ContentReference.IsNullOrEmpty(pageLink))
                return null;

            return GetPage(pageLink) as IOrganizeChildren;
        }

        private IContent GetPage(ContentReference contentLink)
        {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            return contentLoader.Get<IContent>(contentLink);
        }

        public void Uninitialize(InitializationEngine context)
        {
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();

            contentEvents.CreatingContent -= ContentEventsOnCreatingContent;
            contentEvents.MovingContent -= ContentEventsOnMovingContent;
        }

        public void Preload(string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
