using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Graphics;
using Android.Content;
using Android.Hardware;
using Android.Locations;
using System;
using System.IO;
using System.Collections.Generic;
using static Android.Views.GestureDetector;

namespace Running
{
    [ActivityAttribute(Label = "Running", MainLauncher = false)]
    public class MainActivity : Activity
    {
        Button b1, b2, b3, b4, b5;
        TextView text;
        float size;
        public static RunningView run;

        //voor als de app start
        protected override void OnCreate(Bundle b)
        {
            base.OnCreate(b);
            //initialiseer alle layouts etc.
            LinearLayout layout;
            layout = new LinearLayout(this);
            LinearLayout layout2;
            layout2 = new LinearLayout(this);
            run = new RunningView(this);
            LinearLayout.LayoutParams param;
            param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 0.25f);
            //de omlijsting van de buttons
            param.SetMargins(10, 0, 10, 20);

            text = new TextView(this);
            text.Text = "\"nuttig informatie\"";
            size = 15;

            //alle buttons op een rij
            b1 = new Button(this);
            b1.TextSize = size;
            b1.Text = "Center";
            b2 = new Button(this);
            b2.TextSize = size;
            b2.Text = "Start";
            b3 = new Button(this);
            b3.TextSize = size;
            b3.Text = "Erase";
            b4 = new Button(this);
            b4.TextSize = 15;
            b4.Text = "Stop";
            b5 = new Button(this);
            b5.TextSize = size;
            b5.Text = "Laden";
            b1.Click += B1_Click;
            b2.Click += B2_Click;
            b3.Click += B3_Click;
            b4.Click += B4_Click;
            b5.Click += Laden;

            //layout van de buttons
            layout.Orientation = Orientation.Horizontal;
            layout.AddView(b1, param);
            layout.AddView(b2, param);
            layout.AddView(b4, param);
            layout.AddView(b3, param);
            layout.AddView(b5, param);
            //totale layout
            layout2.Orientation = Orientation.Vertical;
            layout2.AddView(text);
            layout2.AddView(layout);
            layout2.AddView(run);
            //display de kaart met de buttons erboven (layout2)
            SetContentView(layout2);
        }


        //wat gebeurd er als je de kaart moet centreren
        private void B1_Click(object sender, System.EventArgs e)
        {
            run.Reset();
        }

        //wat gebeurd er als je gaat starten
        private void B2_Click(object sender, System.EventArgs e)
        {
            run.Starting();
        }

        //wat gebeurd er als je op stoppen klikt
        private void B4_Click(object sender, System.EventArgs e)
        {
            run.Stopping();
        }

        //wat gebeurd er als je wil Erasen
        private void B3_Click(object sender, System.EventArgs e)
        {
            //voor de vraag om te wissen
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Route echt wissen?");
            alert.SetNegativeButton("Nee", NietWissen);
            alert.SetPositiveButton("Ja", WelWissen);
            alert.Show();
            void NietWissen(object o, EventArgs ea)
            { }
            void WelWissen(object o, EventArgs ea)
            {
                run.Erase();
            }
        }
        //Laden van faketrack
        public void Laden(object sender, EventArgs e)
        {

            string content;
            using (StreamReader sr = new StreamReader(this.Assets.Open("faketrack.txt")))
            {
                content = sr.ReadToEnd();
            }

            using (StringReader sr = new StringReader(content))
            {
                run.lijst = new List<PuntEnTijd>();
                string regel;
                while((regel = sr.ReadLine()) != null)
                {
                    Console.WriteLine(regel);
                    string[] woorden;
                    woorden = regel.Split(' ');
                    run.lijst.Add(
                        new PuntEnTijd(
                            new PointF(float.Parse(woorden[0]), float.Parse(woorden[1])),
                            new DateTime(int.Parse(woorden[2]),
                            int.Parse(woorden[3]),
                            int.Parse(woorden[4]),
                            int.Parse(woorden[5]),
                            int.Parse(woorden[6]),
                            int.Parse(woorden[7])
                            )));
                }
            }
            run.Invalidate();
        }
    }

    public class RunningView : View, ISensorEventListener, ILocationListener, ScaleGestureDetector.IOnScaleGestureListener, IOnGestureListener
    {
        Matrix mat, mat2;
        Bitmap p, p1;
        PointF plek, centrum, route;
        public List<PuntEnTijd> lijst;
        ScaleGestureDetector det;
        GestureDetector det2;
        float Schaal, Hoek, dragx, dragy, midx, midy, spelerX, spelerY, rad;
        bool pinching = false;
        public bool start = false;
        public bool stop = false;

        //initialiseer de eigen view
        public RunningView(Context c) : base(c)

        {
            BitmapFactory.Options options;
            options = new BitmapFactory.Options();
            options.InScaled = false;

            //voor het aanraken van het scherm
            det = new ScaleGestureDetector(c, this);
            det2 = new GestureDetector(c, this);
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
            //positie wordt geupdate elke 500 ms en bij een verandering 
            lm.RequestLocationUpdates(lp, 1000, 0, this);

            //beginwaardes declareren
            centrum = new PointF(139000, 455500);
            plek = new PointF(138300, 454300);
            rad = Math.Min(p1.Height / 4, p1.Width / 4);
            Schaal = 1.25f;
        }

        //voor resetten van de view, te gebruiken bij de knop reset
        public void Reset()
        {
            centrum.X = Math.Max(plek.X, 136000 + ((this.Width / 2) / Schaal / 0.4f));
            centrum.X = Math.Min(plek.X, 142000 - ((this.Width / 2) / Schaal / 0.4f));
            centrum.Y = Math.Min(plek.Y, 458000 - ((this.Height / 2) / Schaal / 0.4f));
            centrum.Y = Math.Max(plek.Y, 453000 + ((this.Height / 2) / Schaal / 0.4f));

            this.Invalidate();
        }

        //om te starten
        public void Starting()
        {
            if (start == false)
            {
                lijst = new List<PuntEnTijd>();
                start = true;
                Routes.share.Visibility = ViewStates.Visible;
                Routes.analyze.Visibility = ViewStates.Visible;

                this.Invalidate();
            }
        }

        //om te stoppen
        public void Stopping()
        {
            if (start == true)
            {
                start = false;
                this.Invalidate();
            }
        }

        //om te erasen
        public void Erase()
        {
            lijst.Clear();
            Routes.analyze.Visibility = ViewStates.Invisible;
            Routes.share.Visibility = ViewStates.Invisible;
            this.Invalidate();
        }

        

        //tekent de kaart
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (Schaal == 0)
                Schaal = Math.Min(((float)this.Width) / this.p.Width, ((float)this.Height) / this.p.Height);

            midx = (centrum.X - 136000) * 0.4f;
            midy = -(centrum.Y - 458000) * 0.4f;

            //voor x waarde gebruiker
            float ax = plek.X - centrum.X;
            float px = ax * 0.4f;
            float sx = px * Schaal;
            spelerX = this.Width / 2 + sx;

            //voor y waarde gebruiker
            float ay = plek.Y - centrum.Y;
            float py = ay * 0.4f;
            float sy = py * Schaal;
            spelerY = this.Height / 2 - sy;

            //voor kaart zelf
            mat = new Matrix();
            mat.PostTranslate(-midx, -midy);

            //Borders voor schalen
            if (Schaal > (0.005f * this.Width))
            {
                Schaal = (0.005f * this.Width);
            }
            if (Schaal < Math.Min(((float)this.Width) / this.p.Width, ((float)this.Height) / this.p.Height))
            {
                Schaal = Math.Min(((float)this.Width) / this.p.Width, ((float)this.Height) / this.p.Height);
            }
            mat.PostScale(this.Schaal, this.Schaal);

            //Borders voor draggen
            centrum.X = Math.Max(centrum.X, 136000 + ((this.Width / 2) / Schaal / 0.4f));
            centrum.X = Math.Min(centrum.X, 142000 - ((this.Width / 2) / Schaal / 0.4f));
            centrum.Y = Math.Min(centrum.Y, 458000 - ((this.Height / 2) / Schaal / 0.4f));
            centrum.Y = Math.Max(centrum.Y, 453000 + ((this.Height / 2) / Schaal / 0.4f));

            mat.PostTranslate(this.Width / 2, this.Height / 2);

            //voor de gebruiker
            mat2 = new Matrix();
            mat2.PostTranslate(-this.p1.Width / 2, -this.p1.Height / 2);
            mat2.PostRotate(-this.Hoek);
            mat2.PostTranslate(spelerX, spelerY);

            //teken de kaart
            canvas.DrawBitmap(p, mat, new Paint());

            //teken de afgelegde track
            if (lijst != null)
            {
                for (int i = 0; i < lijst.Count; i++)
                {
                    PointF vorig, nu;
                    //zet de verf naar de juiste kleur en dikte
                    Paint verf = new Paint();
                    verf.Color = Color.Blue;
                    verf.StrokeWidth = rad;

                    //omreken van nuX
                    float ax1 = lijst[i].punt.X - centrum.X;
                    float px1 = ax1 * 0.4f;
                    float sx1 = px1 * Schaal;
                    float nuX = this.Width / 2 + sx1;
                    //omreken van nuY
                    float ay1 = lijst[i].punt.Y - centrum.Y;
                    float py1 = ay1 * 0.4f;
                    float sy1 = py1 * Schaal;
                    float nuY = this.Height / 2 - sy1;

                    if (i - 1 >= 0)
                    {
                        //omreken van vorigX
                        float ax2 = lijst[i - 1].punt.X - centrum.X;
                        float px2 = ax2 * 0.4f;
                        float sx2 = px2 * Schaal;
                        float vorigX = this.Width / 2 + sx2;
                        //omreken van vorigY
                        float ay2 = lijst[i - 1].punt.Y - centrum.Y;
                        float py2 = ay2 * 0.4f;
                        float sy2 = py2 * Schaal;
                        float vorigY = this.Height / 2 - sy2;

                        //bepaal het huidige punt, en het vorige punt
                        nu = new PointF(nuX, nuY);
                        vorig = new PointF(vorigX, vorigY);

                        canvas.DrawLine(vorig.X, vorig.Y, nu.X, nu.Y, verf);
                    }
                    else
                    {
                        canvas.DrawCircle(nuX, nuY, rad, verf);
                    }
                }
            }
            //teken de gebruiker
            canvas.DrawBitmap(p1, mat2, new Paint());
        }

        //voor bepalen van locatie
        public void OnLocationChanged(Location loc)
        {
            plek = Projectie.Geo2RD(loc);
            if (start == true)
            {
                route = new PointF(plek.X, plek.Y);
                DateTime nu = DateTime.Now;
                PuntEnTijd pt = new PuntEnTijd(route, nu);
                lijst.Add(pt);
                this.Invalidate();
            }
        }

        //voor orientation naar het noorden
        public void OnSensorChanged(SensorEvent s)
        {
            if (s.Sensor.Type == SensorType.Orientation)
                Hoek = s.Values[0];
            this.Invalidate();
        }

        //dit gebeurd op het moment dat het scherm aangeraakt wordt
        private void raakAan(object sender, TouchEventArgs e)
        {
            det.OnTouchEvent(e.Event);
            if (e.Event.Action == MotionEventActions.Pointer2Down && e.Event.Action == MotionEventActions.Pointer1Down)
            {
                pinching = true;
            }
            else if (!pinching)
            {
                det2.OnTouchEvent(e.Event);
                this.Invalidate();
            }
        }

        //voor pinch bewegingen
        public bool OnScale(ScaleGestureDetector d)
        {
            this.Schaal *= d.ScaleFactor;
            this.Invalidate();
            pinching = false;
            return true;
        }

        //voor drag bewegingen
        public bool OnScroll(MotionEvent m1, MotionEvent m2, float x, float y)
        {
            dragx = (x / Schaal) / 0.4f;
            dragy = (y / Schaal) / 0.4f;
            centrum = new PointF(centrum.X + dragx, centrum.Y - dragy);
            this.Invalidate();
            return true;
        }


        //BEGIN VAN OVERIG
        //overige methodes die we niet hoeven te gebruiken
        public bool OnScaleBegin(ScaleGestureDetector d)
        {
            return true;
        }

        public bool OnSingleTapUp(MotionEvent me)
        {
            return true;
        }

        public void OnShowPress(MotionEvent me)
        { }

        public void OnLongPress(MotionEvent me)
        { }

        public bool OnFling(MotionEvent m1, MotionEvent m2, float x, float y)
        {
            return true;
        }

        public bool OnDown(MotionEvent me)
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

    public class PuntEnTijd
    {
        public PointF punt;
        public DateTime tijd;
        public string info;

        public PuntEnTijd(PointF p, DateTime dt)
        {
            punt = new PointF(p.X, p.Y);
	    tijd = dt;
            info = $"{punt.X} {punt.Y} {tijd.ToString("yyyy MM dd HH mm ss")}";
        }
    }
}
