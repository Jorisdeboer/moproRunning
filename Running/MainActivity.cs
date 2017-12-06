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
        Button b1, b2, b3;

        protected override void OnCreate(Bundle b)
        {
            base.OnCreate(b);

            LinearLayout layout;
            layout = new LinearLayout(this);
            LinearLayout layout2;
            layout2 = new LinearLayout(this);
            RunningView run;
            run = new RunningView(this);
            LinearLayout.LayoutParams param;
            param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 0.25f);

            param.SetMargins(20, 0, 20, 20);
            
            b1 = new Button(this);
            b1.Text = "Center";
            b2 = new Button(this);
            b2.Text = "Start/Stop";
            b3 = new Button(this);
            b3.Text = "Erase";


            layout.Orientation = Orientation.Horizontal;
            layout.AddView(b1, param);
            layout.AddView(b2, param);
            layout.AddView(b3, param);

            layout2.Orientation = Orientation.Vertical;
            layout2.AddView(layout);
            layout2.AddView(run);

            SetContentView(layout2);
        }

    }
    
    public class RunningView : View
    {
        Bitmap p;

        public RunningView(Context c) : base(c)
        {
            BitmapFactory.Options options;
            options = new BitmapFactory.Options();
            options.InScaled = false;

            p = BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.Utrecht, options);
            
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            canvas.DrawBitmap(p, 0, 0, new Paint());
        }
    }
}

