using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System.IO;
using Xamarin.Controls;
using SignaturePad;
using SignaturePad.Forms;
using B2B.iOS;

[assembly: ExportRenderer(typeof(MarkingView), typeof(MarkingViewRenderer))]
namespace B2B.iOS
{
    public class MarkingViewRenderer : ViewRenderer<MarkingView, SignaturePad.Forms.SignaturePadView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<MarkingView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.SaveImageWithBackground = null;
            }
            if (e.NewElement != null)
            {
                e.NewElement.SaveImageWithBackground += NewElement_SaveImageWithBackground;
            }
        }

        string NewElement_SaveImageWithBackground()
        {
            UIGraphics.BeginImageContextWithOptions(Bounds.Size, false, 0f);

            using (var context = UIGraphics.GetCurrentContext())
            {
                Layer.RenderInContext(context);
                using (UIImage img = UIGraphics.GetImageFromCurrentImageContext())
                {
                    UIGraphics.EndImageContext();

                    string folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string filename;
                    do
                    {
                        filename = Path.Combine(folder, "Marking-" + DateTime.Now.Ticks + ".png");
                    }
                    while (File.Exists(filename));

                    img.AsPNG().Save(filename, true);

                    return filename;
                }
            }
        }
    }
}