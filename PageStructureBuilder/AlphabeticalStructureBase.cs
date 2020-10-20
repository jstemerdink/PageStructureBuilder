namespace PageStructureBuilder
{
    using System;

    using EPiServer.Core;

    /// <summary>
    /// Class AlphabeticalStructureBase.
    /// Implements the <see cref="SingleLevelStructureBase{TCharacter}" />
    /// </summary>
    /// <typeparam name="TCharacter">The type of <see cref="PageData"/> to use for the character container.</typeparam>
    /// <seealso cref="SingleLevelStructureBase{TCharacter}" />
    public abstract class AlphabeticalStructureBase<TCharacter> : SingleLevelStructureBase<TCharacter>
        where TCharacter : PageData, new()
    {
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

            return childPage.PageName[0].ToString().ToUpperInvariant();
        }
    }
}