using System;
using System.Linq;
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
        float graphx, graphy, grafiekafstand, grafiekbreedte, maxafstand;
        PointF eerstepunt = MainActivity.run.lijst.ElementAt(0).punt;
        DateTime eerstetijd = MainActivity.run.lijst.ElementAt(0).tijd;
        DateTime vorig;

        public Analyse(Context c) : base(c)
        {
            this.SetBackgroundColor(Color.Beige);
            maxafstand = 0;
            vorig = eerstetijd;
            this.Invalidate();
        }

        //teken de grafiek
        protected override void OnDraw(Canvas cv)
        {
            base.OnDraw(cv);
            float maxtijd, maxsnelheid, gemsnelheid;
            
            graphx = (float)this.Width * 0.1f;
            graphy = (float)this.Height * (6f / 7f);
            grafiekafstand = (float)this.Height*0.6f;
            grafiekbreedte = ((float)this.Width * 0.95f) - graphx;

            Paint verf = new Paint();
            verf.Color = Color.LightGray;

            /*
            //AFSTAND EN TIJD -- eventueel als er extra tijd is
            //achtergrond van de eerste grafiek
            cv.DrawRect(graphx, (this.Height / 2f) - grafiekafstand, grafiekbreedte, graph1y, verf);
            
            verf.Color = Color.Black;

            //horizontale zijlijn
            cv.DrawLine(graphx, graph1y, grafiekbreedte, graph1y, verf);
            //verticale zijlijn
            cv.DrawLine(graphx, graph1y, graphx, (this.Height / 2f) - grafiekafstand, verf); */     



            //array met de afstanden tot het vorige punt
            float[] afstanden = new float[MainActivity.run.lijst.Count];
            for (int i = 0; i < MainActivity.run.lijst.Count; i++)
            {
                if (i == 0)
                    afstanden[i] = 0;

                else
                    afstanden[i] = BepaalAfstand(MainActivity.run.lijst.ElementAt(i - 1).punt, MainActivity.run.lijst.ElementAt(i).punt);
            }

            //array met snelheden
            float[] snelheden = new float[MainActivity.run.lijst.Count];
            for(int i = 0; i < MainActivity.run.lijst.Count; i++)
            {
                float afstand = 0;
                int plek = 0;
                float dt = BepaalTijd(vorig, MainActivity.run.lijst.ElementAt(i).tijd);
                afstand += afstanden[i];
                if (dt >= 5f)
                {
                    snelheden[plek] = (float)(afstand / dt);
                    vorig = MainActivity.run.lijst.ElementAt(i).tijd;
                    plek++;
                }
            }

            //afstand in meters
            for(int j = 0; j< afstanden.Length; j++)
                maxafstand += afstanden[j];
            
            //tijd in seconden
            maxtijd = -BepaalTijd(eerstetijd, MainActivity.run.lijst.ElementAt(MainActivity.run.lijst.Count).tijd);

            //gemsnelheid in m/s
            gemsnelheid = ((float)maxafstand / (float)maxtijd);

            //maxsnelheid in m/s
            maxsnelheid = snelheden.Max();

            //SNELHEID EN TIJD
            //achtergrond van de grafiek
            verf.Color = Color.LightGray;
            cv.DrawRect(graphx, this.Height - grafiekafstand,grafiekbreedte, graphy, verf);

            //info over de grafiek
            verf.Color = Color.Black;
            verf.StrokeWidth = 8;

            //horizontale zijlijn - tijd
            cv.DrawLine(graphx, graphy, grafiekbreedte, graphy, verf);
            //properties voor indicaties in de grafiek
            verf.Color = Color.DarkGray;
            verf.StrokeWidth = 1;
            verf.TextSize = 12;
            //per seconde getekend
            if (maxtijd <= 60f)
            {
                float stapgrootte = (grafiekbreedte / maxtijd);
                for (float i = 0; i < (int)maxtijd; i += stapgrootte)
                {
                    cv.DrawLine(graphx + i, graphy, graphx + i, this.Height - grafiekafstand, verf);
                    cv.DrawText(i.ToString(), graphx + i, graphy - 15, verf);
                }
            }
            //per minuut getekend
            else
            {
                int stapgrootte = (int)(grafiekbreedte / maxtijd);
                for (float i = 0; i < grafiekbreedte; i += stapgrootte)
                {
                    cv.DrawLine(graphx + i, graphy, graphx + i, this.Height - grafiekafstand, verf);
                    cv.DrawText(i.ToString(), graphx + i, graphy - 15, verf);
                }
            }

            //verticale zijlijn - snelheid
            cv.DrawLine(graphx, graphy, graphx, this.Height - grafiekafstand, verf);
            if(maxsnelheid > 0)
            {
                float stapgrootte = (grafiekafstand / maxsnelheid);
                for(float i = 0; i < maxsnelheid; i += stapgrootte)
                {
                    cv.DrawLine(graphx, graphy + i, grafiekbreedte, graphy + i, verf);
                }
            }

            //for-loop om een lijn te tekenen tussen vorige punt en nieuwe punt.
            for (int i = 0; i<afstanden.Length - 1; i++)
            {
                //cv.DrawLine(graphx, graph1y, (float)i, graph1y - afstanden[i], verf);
            }
        }

        //bepaal het verschil in seconden tussen twee tijden
        public float BepaalTijd(DateTime begintijd, DateTime eindtijd)
        {
            TimeSpan ts = eindtijd - begintijd;
            return (float)ts.TotalSeconds;
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
    }
}