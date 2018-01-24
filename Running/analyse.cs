using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace Running
{
    [ActivityAttribute(Label = "Running", MainLauncher = false)]
    class AnalyseActivity : Activity
    {
        Button b1;
        Analyse analyseview;

        protected override void OnCreate(Bundle b)
        {
            base.OnCreate(b);
            analyseview = new Analyse(this);
            b1 = new Button(this);
            b1.Text = "Terug naar Mijn Routes";
            b1.Click += B1_Click;
            LinearLayout lay = new LinearLayout(this);
            lay.Orientation = Orientation.Vertical;

            lay.AddView(b1);
            lay.AddView(analyseview);

            this.SetContentView(lay);
        }

        private void B1_Click(object sender, EventArgs e)
        {
            Intent i;
            i = new Intent(this, typeof(Routes));
            StartActivity(i);
        }
    }


    public class Analyse : View
    {
        //maak een lijst met de waarden voor de tijd in hh:mm:ss
        public List<string> tijd;

        public Analyse(Context c) : base(c)
        {
            this.SetBackgroundColor(Color.Beige);
        }

        //teken de grafiek
        protected override void OnDraw(Canvas cv)
        {
            base.OnDraw(cv);
        }

        //bepaal het verschil in seconden tussen twee tijden
        public double BepaalTijd(DateTime begintijd, DateTime eindtijd)
        {
            TimeSpan ts = eindtijd - begintijd;
            double totaal = ts.TotalSeconds;
            tijd.Add((ts.ToString("HH:mm:ss")));
            return totaal;
        }

        //bepaal de afstand in meters tussen twee punten
        public float BepaalAfstand(PointF begin, PointF eind)
        {
            float dx, dy, afstand;
            dx = Math.Max(eind.X - begin.X, begin.X - eind.X);
            dy = Math.Max(eind.Y - begin.Y, begin.Y - eind.Y);
            afstand = (float)Math.Sqrt((dx * dx) + (dy * dy));
            return afstand;
        }

        //bepaal de snelheid van een stukje track in m/s
        public float BepaalSnelheid(float afstand, double tijd)
        {
            return (float)(afstand / tijd);
        }
    }
}