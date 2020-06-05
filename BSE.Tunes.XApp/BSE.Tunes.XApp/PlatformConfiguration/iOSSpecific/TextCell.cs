using Xamarin.Forms;

namespace BSE.Tunes.XApp.PlatformConfiguration.iOSSpecific
{
    public static class TextCell
    {
        public static BindableProperty AccessoryProperty =
            BindableProperty.CreateAttached(nameof(iOSSpecific.TableViewCellAccessory),
                                            typeof(TableViewCellAccessory),
                                            typeof(Xamarin.Forms.Cell),
                                            TableViewCellAccessory.None);

        public static void SetAccessory(BindableObject element,
                                        TableViewCellAccessory value)
            => element.SetValue(AccessoryProperty, value);

        public static TableViewCellAccessory GetAccessory(BindableObject element)
            => (TableViewCellAccessory)element.GetValue(AccessoryProperty);
    }

}
