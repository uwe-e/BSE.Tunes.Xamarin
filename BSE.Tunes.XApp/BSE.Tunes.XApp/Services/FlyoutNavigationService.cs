using BSE.Tunes.XApp.Controls;
using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Logging;
using Prism.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public class FlyoutNavigationService : PageNavigationService, IFlyoutNavigationService
    {
        private FlyoutPage _flyoutPage; 

        public FlyoutNavigationService(
            IContainerExtension container,
            IApplicationProvider applicationProvider,
            IPageBehaviorFactory pageBehaviorFactory,
            ILoggerFacade logger) : base(container, applicationProvider, pageBehaviorFactory, logger)
        {
        }

        public async Task<INavigationResult> CloseFlyoutAsync()
        {
            if (_flyoutPage != null)
            {
                await _flyoutPage.DisappearingAnimation();
            }
            
            return await GoBackInternal(null,useModalNavigation: true, animated: false);
        }

        public async Task<INavigationResult> ShowFlyoutAsync(string name, INavigationParameters parameters)
        {
            var result = new NavigationResult();
            var page = CreatePageFromSegment(name);
            if (page is FlyoutPage flyoutPage)
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
