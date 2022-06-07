using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;
using PreserveAttribute = Foundation.PreserveAttribute;

//[assembly: ExportRenderer(typeof(Xamarin.Forms.Image), typeof(BSE.Tunes.XApp.iOS.Renderer.ImageRenderer))]
namespace BSE.Tunes.XApp.iOS.Renderer
{
    // TODO: 09.05.2022 [Bug] iOS ImageRenderer NullReferenceException #15104 
    // https://github.com/xamarin/Xamarin.Forms/issues/15104
    //public class ImageRenderer : ViewRenderer<Xamarin.Forms.Image, FormsUIImageView>, IImageVisualElementRenderer
    //{
    //    bool _isDisposed;

    //    [Preserve(Conditional = true)]
    //    public ImageRenderer() : base()
    //    {
    //        ImageElementManager.Init(this);
    //    }

    //    protected override void Dispose(bool disposing)
    //    {
    //        if (_isDisposed)
    //            return;

    //        if (disposing)
    //        {
    //            UIImage oldUIImage;
    //            if (Control != null && (oldUIImage = Control.Image) != null)
    //            {
    //                ImageElementManager.Dispose(this);
    //                oldUIImage.Dispose();
    //            }
    //        }

    //        _isDisposed = true;

    //        base.Dispose(disposing);
    //    }

    //    protected override async void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
    //    {
    //        if (e.NewElement != null)
    //        {
    //            if (Control == null)
    //            {
    //                var imageView = new FormsUIImageView();
    //                imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
    //                imageView.ClipsToBounds = true;
    //                SetNativeControl(imageView);
    //            }

    //            await TrySetImage(e.OldElement as Image);
    //            UpdateBackground();
    //        }

    //        base.OnElementChanged(e);
    //    }

    //    protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    //    {
    //        base.OnElementPropertyChanged(sender, e);

    //        if (e.PropertyName == Image.SourceProperty.PropertyName)
    //            await TrySetImage().ConfigureAwait(false);
    //        else if (e.PropertyName == VisualElement.BackgroundProperty.PropertyName)
    //            UpdateBackground();
    //    }

    //    protected virtual async Task TrySetImage(Xamarin.Forms.Image previous = null)
    //    {
    //        // By default we'll just catch and log any exceptions thrown by SetImage so they don't bring down
    //        // the application; a custom renderer can override this method and handle exceptions from
    //        // SetImage differently if it wants to

    //        try
    //        {
    //            await SetImage(previous).ConfigureAwait(false);
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Warning(nameof(ImageRenderer), "Error loading image: {0}", ex);
    //        }
    //        finally
    //        {
    //            ((Xamarin.Forms.IImageController)Element)?.SetIsLoading(false);
    //        }
    //    }

    //    protected async Task SetImage(Xamarin.Forms.Image oldElement = null)
    //    {
    //        await ImageElementManager.SetImage(this, Element, oldElement).ConfigureAwait(false);
    //    }

    //    void IImageVisualElementRenderer.SetImage(UIImage image) => Control.Image = image;

    //    bool IImageVisualElementRenderer.IsDisposed => _isDisposed;

    //    UIImageView IImageVisualElementRenderer.GetImage() => Control;

    //    void UpdateBackground()
    //    {
    //        var parent = Control?.Superview;

    //        if (parent == null)
    //            return;

    //        parent.UpdateBackground(Element.Background);
    //    }
    //}

}