using BSE.Tunes.XApp.PlatformConfiguration.iOSSpecific;
using UIKit;
using Xamarin.Forms;
using BSERenderer = BSE.Tunes.XApp.iOS.Renderer;
using CellSpezific = BSE.Tunes.XApp.PlatformConfiguration.iOSSpecific.TextCell;

[assembly: ExportRenderer(typeof(Xamarin.Forms.TextCell), typeof(BSERenderer.TextCellRenderer))]
namespace BSE.Tunes.XApp.iOS.Renderer
{
    public class TextCellRenderer : Xamarin.Forms.Platform.iOS.TextCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var tvc = base.GetCell(item, reusableCell, tv);

            UpdateAccessory(item, tvc);

            return tvc;
        }
        private void UpdateAccessory(Cell item, UITableViewCell tableViewCell)
        {
            var accessor = item.GetValue(CellSpezific.AccessoryProperty);
            if (accessor != null)
            {
                if (accessor is TableViewCellAccessory cellAccessory)
                {
                    switch (cellAccessory)
                    {
                        case TableViewCellAccessory.DisclosureIndicator:
                            tableViewCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                            break;
                        default:
                            tableViewCell.Accessory = UITableViewCellAccessory.None;
                            break;
                    }
                }
            }
        }
    }
}