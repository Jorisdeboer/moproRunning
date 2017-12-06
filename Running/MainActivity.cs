using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Graphics;
using Android.Content;

namespace Running
{
    [Activity(Label = "Running", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle b)
        {
            base.OnCreate(b);
            LinearLayout layout;
            layout = new LinearLayout(this);
            
            SetContentView(layout);
        }

    }
    
    public class RunningView : View
    {
        public RunningView(Context c) : base(c)
        {
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
        }
    }
}

