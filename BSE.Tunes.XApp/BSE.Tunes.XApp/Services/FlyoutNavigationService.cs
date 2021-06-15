using BSE.Tunes.XApp.Controls;
using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.Services
{
    public class FlyoutNavigationService : PageNavigationService, IFlyoutNavigationService
    {
        private BottomFlyoutPage _flyoutPage; 

        public FlyoutNavigationService(
            IContainerExtension container,
            IApplicationProvider applicationProvider,
            IPageBehaviorFactory pageBehaviorFactory) : base(container, applicationProvider, pageBehaviorFactory)
        {
        }

        public async Task<INavigationResult> CloseFlyoutAsync()
        {
            if (_flyoutPage != null)
            {
                await _flyoutPage.DisappearingAnimation();
            }
            /* 
             * Workaround for Prism 8.1.97
             * PopUp Page wasnt completely removed
             */
            var result = new NavigationResult();
            
            var page = GetCurrentPage();

            var poppedPage = await DoPop(page.Navigation, true, false);
            if (poppedPage != null)
            {
                PageUtilities.DestroyPage(poppedPage);

                result.Success = true;
                return result;
            }
            /*
             * End of workaround
             */

            return await GoBackInternal(null,useModalNavigation: true, animated: false);
        }

        public async Task<INavigationResult> ShowFlyoutAsync(string name, INavigationParameters parameters)
        {
            var result = new NavigationResult();
            var page = CreatePageFromSegment(name);
            if (page is BottomFlyoutPage flyoutPage)
            {
                _flyoutPage = flyoutPage;

                var useModalNavigation = true;
                var animated = false;
                var uri = UriParsingHelper.Parse(name);
                var navigationSegments = UriParsingHelper.GetUriSegments(uri);

                var nextSegment = navigationSegments.Dequeue();

                await ProcessNavigation(flyoutPage, navigationSegments, parameters, useModalNavigation, animated);

                await DoNavigateAction(page, nextSegment, flyoutPage, parameters, async () =>
                {
                    await DoPush(GetCurrentPage(), flyoutPage, useModalNavigation, animated);
                });
                
                await flyoutPage.AppearingAnimation();
                
                result.Success = true;
                return result;
            }
            return await NavigateAsync(name, parameters);
        }
    }
}
