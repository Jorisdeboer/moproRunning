using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Graphics;
using Android.Content;
using Android.Hardware;
using Android.Locations;
using System;

namespace Running
{
    [Activity(Label = "Running", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Button b1, b2, b3;
        RunningView run;
        
        //voor als de app start
        protected override void OnCreate(Bundle b)
        {
            base.OnCreate(b);
            
            LinearLayout layout;
            layout = new LinearLayout(this);
            LinearLayout layout2;
            layout2 = new LinearLayout(this);
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
            b1.Click += B1_Click;
            b2.Click += B2_Click;
            b3.Click += B3_Click;

            layout.Orientation = Orientation.Horizontal;
            layout.AddView(b1, param);
            layout.AddView(b2, param);
            layout.AddView(b3, param);

            layout2.Orientation = Orientation.Vertical;
            layout2.AddView(layout);
            layout2.AddView(run);

            SetContentView(layout2);
        }

        //wat gebeurd er als je de kaart moet centreren
        private void B1_Click(object sender, System.EventArgs e)
        {
            run.Reset();
        }

        //wat gebeurd er als je gaat starten/stoppen
        private void B2_Click(object sender, System.EventArgs e)
        {
        }

        //wat gebeurd er als je wil Erasen
        private void B3_Click(object sender, System.EventArgs e)
        {
        }
    }

    public class RunningView : View, ISensorEventListener, ILocationListener, ScaleGestureDetector.IOnScaleGestureListener
    {
        Bitmap p, p1;
        float Schaal, Hoek;
        double noord, oost;
        ScaleGestureDetector det;

        //initialiseer de eigen view
        public RunningView(Context c) : base(c)
        {
            BitmapFactory.Options options;
            options = new BitmapFactory.Options();
            options.InScaled = false;

            //voor het aanraken van het scherm
            det = new ScaleGestureDetector(c, this);
            this.Touch += raakAan;

            //laad de plaatjes
            p = BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.Utrecht, options);
            p1 = BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.character2, options);

            //laad de sensor voor kompas
            SensorManager sm = (SensorManager)c.GetSystemService(Context.SensorService);
            sm.RegisterListener(this, sm.GetDefaultSensor(SensorType.Orientation), SensorDelay.Ui);

            //voor de locatie
            LocationManager lm = (LocationManager)c.GetSystemService(Context.LocationService);
            Criteria crit = new Criteria();
            crit.Accuracy = Accuracy.Fine;
            string lp = lm.GetBestProvider(crit, true);
            lm.RequestLocationUpdates(lp, 2000, 1, this);
        }

        //voor resetten van de view, te gebruiken bij de knop reset
        public void Reset()
        {

        }
        
        //tekent de kaart
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if (Schaal == 0)
                Schaal = Math.Min(((float)this.Width) / this.p1.Width, ((float)this.Height) / this.p1.Height);

            Matrix mat = new Matrix();
            mat.PostTranslate(-this.p.Width / 2, -this.p.Height / 2);
            mat.PostScale(this.Schaal, this.Schaal);
            mat.PostTranslate(this.Width / 2, this.Height / 2);

            Matrix mat2 = new Matrix();
            mat2.PostTranslate(-this.p1.Width / 2, -this.p1.Height / 2);
            mat2.PostRotate(-this.Hoek);
            //x, y moet op locatie noorderbreedte/oosterlengte
            mat2.PostTranslate(this.Width / 2, this.Height / 2);
           
            canvas.DrawBitmap(p, mat, new Paint());
            canvas.DrawBitmap(this.p1, mat2, new Paint());
        }


        //voor orientation naar het noorden
        public void OnSensorChanged(SensorEvent s)
        {
            if(s.Sensor.Type == SensorType.Orientation)
            Hoek = s.Values[0];
            this.Invalidate();
        }

        //dit gebeurd op het moment dat het scherm aangeraakt wordt
        private void raakAan(object sender, TouchEventArgs e)
        {
            det.OnTouchEvent(e.Event);
        }

        //voor bepalen van locatie
        public void OnLocationChanged(Location loc)
        {
            noord = loc.Latitude;
            oost = loc.Longitude;

            //schrijf een string met de info over waar men zich bevindt
            string info = $"{noord} graden noorderbreedte, {oost} graden oosterlengte";
        }

        //voor pinch en drag bewegingen
        public bool OnScale(ScaleGestureDetector d)
        {
            this.Schaal *= d.ScaleFactor;
            this.Invalidate();
            return true;
        }

        //BEGIN VAN OVERIG
        //overige methodes die we niet hoeven te gebruiken
        public bool OnScaleBegin(ScaleGestureDetector d)
        {
            return true;
        }

        public void OnScaleEnd(ScaleGestureDetector d)
        { }

        public void OnProviderDisabled(string s)
        { }

        public void OnProviderEnabled(string s)
        { }

        public void OnStatusChanged(string s, Availability a, Bundle b)
        { }

        public void OnAccuracyChanged(Sensor s, SensorStatus ss)
        { }
        //EIND VAN OVERIGE METHODE, WEL LATEN STAAN

    }
}