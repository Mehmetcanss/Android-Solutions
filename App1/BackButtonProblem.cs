using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace App1
{
    [Activity(Label = "BackButtonProblem")]
    public class BackButtonProblem : Activity
    {

        private View firstChild;
        private int usableHeightPrevious;
        private FrameLayout.LayoutParams frameLayoutParams;
        bool keyboard_was_visible = false;
        bool back_pressed = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var main = (Android.Widget.FrameLayout)FindViewById(Android.Resource.Id.Content);
            firstChild = main.GetChildAt(0);
            frameLayoutParams = (FrameLayout.LayoutParams)firstChild.LayoutParameters;

            //add event listener for child content resize events
            main.ViewTreeObserver.GlobalLayout += (object sender, EventArgs e) =>
            {

                ChildContentResized(e);
            };
        }

        private void ChildContentResized(EventArgs e)
        {

            int usableHeightNow = computeUsableHeight();
            if (usableHeightNow != usableHeightPrevious)
            {
                int usableHeightSansKeyboard = firstChild.RootView.Height;
                int HeightDifference = usableHeightSansKeyboard - usableHeightNow;
                if (HeightDifference > (usableHeightSansKeyboard / 4))
                {
                    // keyboard probably just became visible
                    frameLayoutParams.Height = usableHeightSansKeyboard - HeightDifference;
                    keyboard_was_visible = true;

                }
                else
                {
                    /*
                      keyboard probably just became hidden, 
                      this part of the code is caught before the back button is caught
                     */
                    frameLayoutParams.Height = usableHeightSansKeyboard;
                    if (back_pressed)
                    {
                        back_pressed = false;
                        keyboard_was_visible = true;

                    }
                    else
                    {
                        keyboard_was_visible = false;
                    }

                }
                firstChild.RequestLayout();
                usableHeightPrevious = usableHeightNow;
            }
        }

        private int computeUsableHeight()
        {
            Rect r = new Rect();
            firstChild.GetWindowVisibleDisplayFrame(r);
            return (r.Bottom - r.Top);
        }

        public override void OnBackPressed()
        {

            back_pressed = false;
            if (keyboard_was_visible)
                keyboard_was_visible = false;
            else
                base.OnBackPressed();
        }
    }
}