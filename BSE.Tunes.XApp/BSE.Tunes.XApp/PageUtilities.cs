using System;
using Xamarin.Forms;

namespace BSE.Tunes.XApp
{
    public static class PageUtilities
    {
        public static bool IsCurrentPageTypeOf(Type name)
        {
            var currentPage = Prism.Common.PageUtilities.GetCurrentPage(Application.Current.MainPage);
            return name.Equals(currentPage.GetType());
        }
    }
}
