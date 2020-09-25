
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSE.Tunes.XApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlyoutButton: Button
    {
        public static readonly BindableProperty HorizontalContentAlignmentProperty =
            BindableProperty.Create(nameof(HorizontalContentAlignment), typeof(TextAlignment), typeof(FlyoutButton), TextAlignment.Start);

        public TextAlignment HorizontalContentAlignment
        {
            get { return (TextAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public FlyoutButton()
        {
            InitializeComponent();
        }
        
    }
}