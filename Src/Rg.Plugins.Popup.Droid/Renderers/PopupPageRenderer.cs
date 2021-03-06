using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Rg.Plugins.Popup.Droid.Impl;
using Rg.Plugins.Popup.Droid.Renderers;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PopupPage), typeof(PopupPageRenderer))]
namespace Rg.Plugins.Popup.Droid.Renderers
{
    class PopupPageRenderer : PageRenderer
    {
        private DateTime downTime;
        private Xamarin.Forms.Point downPosition;

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            Element.ForceLayout();
            base.OnLayout(changed, l, t, r, b);
        }

        protected override void OnAttachedToWindow()
        {
            ContextExtensions.HideKeyboard(Context, ((Activity)Forms.Context).Window.DecorView);
            base.OnAttachedToWindow();
        }

        protected override void OnDetachedFromWindow()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(0), () =>
            {
                ContextExtensions.HideKeyboard(Context, ((Activity)Forms.Context).Window.DecorView);
                return false;
            });
            base.OnDetachedFromWindow();
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                downTime = DateTime.UtcNow;
                downPosition = new Xamarin.Forms.Point((double)e.RawX, (double)e.RawY);
            }
            if (e.Action != MotionEventActions.Up)
                return base.DispatchTouchEvent(e);
            Android.Views.View currentFocus1 = ((Activity) Context).CurrentFocus;
            bool flag = base.DispatchTouchEvent(e);
            if (currentFocus1 is EditText)
            {
                Android.Views.View currentFocus2 = ((Activity) Context).CurrentFocus;
                if (currentFocus1 == currentFocus2 && this.downPosition.Distance(new Xamarin.Forms.Point((double)e.RawX, (double)e.RawY)) <= ContextExtensions.ToPixels(Context, 20.0) && !(DateTime.UtcNow - downTime > TimeSpan.FromMilliseconds(200.0)))
                {
                    int[] location = new int[2];
                    currentFocus1.GetLocationOnScreen(location);
                    float num1 = e.RawX + currentFocus1.Left - location[0];
                    float num2 = e.RawY + currentFocus1.Top - location[1];
                    if (!new Rectangle((double)currentFocus1.Left, (double)currentFocus1.Top, (double)currentFocus1.Width, (double)currentFocus1.Height).Contains((double)num1, (double)num2))
                    {
                        ContextExtensions.HideKeyboard(Context, currentFocus1);
                        RequestFocus();
                        currentFocus1.ClearFocus();
                    }
                }
            }
            return flag;
        }
    }
}