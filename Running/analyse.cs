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
            b1.Text = "Terug";
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
            i = new Intent(this, typeof(MainActivity));
            StartActivity(i);
        }
    }

    public class Analyse : View
    {
        //variabelen nodig in analyse
        public List<string> tijd;
        float graphx, graphy, grafiekafstand, grafiekbreedte, stapsnelheid, staptijd;
        public static float maxafstand, maxtijd, maxsnelheid, gemsnelheid;

        PointF eerstepunt = MainActivity.run.lijst.ElementAt(0).punt;
        DateTime eerstetijd = MainActivity.run.lijst.ElementAt(0).tijd;

        public Analyse(Context c) : base(c)
        {
            this.SetBackgroundColor(Color.Beige);
            maxafstand = 0;
            this.Invalidate();
        }

        //teken de grafiek
        protected override void OnDraw(Canvas cv)
        {
            base.OnDraw(cv);
            //graphx is het beginpunt van x; graphy is het beginpunt van y
            graphx = (float)this.Width * 0.1f;
            graphy = (float)this.Height * (6f / 7f);
            //afstand is hoogte van grafiek, breedte is breedte van grafiek
            grafiekafstand = (float)this.Height*0.6f;
            grafiekbreedte = ((float)this.Width * 0.95f) - graphx;

            //array met de afstanden, tijden en snelheden
            float[] afstanden = new float[MainActivity.run.lijst.Count];
            float[] tijden = new float[MainActivity.run.lijst.Count];
            float[] snelheden = new float[MainActivity.run.lijst.Count];
            for (int i = 0; i < MainActivity.run.lijst.Count; i++)
            {
                if (i == 0)
                {
                    afstanden[i] = 0;
                    tijden[i] = 0;
                    snelheden[i] = 0;
                }

                else
                {
                    afstanden[i] = BepaalAfstand(MainActivity.run.lijst.ElementAt(i - 1).punt, MainActivity.run.lijst.ElementAt(i).punt);
                    tijden[i] = BepaalTijd(MainActivity.run.lijst.ElementAt(i - 1).tijd, MainActivity.run.lijst.ElementAt(i).tijd);
                    if (tijden[i] == 0)
                        snelheden[i] = 0;
                    else
                        snelheden[i] = (afstanden[i] / tijden[i]);
                }
            }

            //afstand in meters
            for(int j = 0; j< afstanden.Length; j++)
                maxafstand += afstanden[j];
            
            //tijd in seconden
            maxtijd = BepaalTijd(eerstetijd, MainActivity.run.lijst.ElementAt(MainActivity.run.lijst.Count-1).tijd);

            //gemsnelheid in m/s
            gemsnelheid = ((float)maxafstand / (float)maxtijd);

            //maxsnelheid in m/s
            maxsnelheid = snelheden.Max();

            //SNELHEID EN TIJD
            //achtergrond van de grafiek
            Paint verf = new Paint();
            verf.Color = Color.LightGray;
            cv.DrawRect(graphx, this.Height - grafiekafstand,grafiekbreedte, graphy, verf);

            //info over binnenlijnen - verticaal
            verf.Color = Color.Red;
            verf.StrokeWidth = 2;
            verf.TextSize = 20;
            staptijd = Math.Max((grafiekbreedte / maxtijd), (grafiekbreedte/12));
            float astijd = 0f;
            for (float i = 0; i < grafiekbreedte - staptijd; i += staptijd)
            {
                cv.DrawLine(graphx + i, graphy, graphx + i, this.Height - grafiekafstand, verf);
                verf.Color = Color.Blue;
                cv.DrawText(astijd.ToString(), graphx + i, graphy + 20, verf);
                astijd += 5f;
                verf.Color = Color.Red;
            }

            //info over binnelijnen - horizontaal
            stapsnelheid = Math.Max((grafiekafstand / maxsnelheid), (grafiekafstand/12));
            float asgetal = 0f;
            for(float i = 0; i < (this.Height - grafiekafstand + stapsnelheid); i += stapsnelheid)
            {
                cv.DrawLine(graphx, graphy - i, grafiekbreedte, graphy - i, verf);
                verf.Color = Color.Blue;
                cv.DrawText(asgetal.ToString(), graphx - 40, graphy - i, verf);
                asgetal += 5f;
                verf.Color = Color.Red;
            }

            //om de snelheid over de tijd in de grafiek weer te geven
            verf.Color = Color.Green;
            verf.StrokeWidth = 4;
            float stap = (tijden.Length/12f);
            float schaalmeter = (grafiekafstand / 60f);
            for (int i = 0; i < 10; i ++)
            {
                cv.DrawLine(graphx + (staptijd*i), graphy - ((snelheden[(int)stap*i])*schaalmeter), graphx + (staptijd*(i+1)), graphy - ((snelheden[(int)stap*(i+1)])*schaalmeter), verf);
            }
            cv.DrawLine(graphx + (staptijd * 10), graphy - ((snelheden[(int)stap * 10]) * schaalmeter), (grafiekbreedte), graphy, verf);

            verf.Color = Color.Black;
            verf.StrokeWidth = 8;
            //verticale zijlijn - snelheid
            cv.DrawLine(graphx, graphy, graphx, this.Height - grafiekafstand, verf);
            //horizontale zijlijn - tijd
            cv.DrawLine(graphx, graphy, grafiekbreedte, graphy, verf);

            //tekst lang de zijkanten
            verf.Color = Color.Blue;
            cv.DrawText("snelheid (m/s)", (graphx - 40), (this.Height - grafiekafstand - 20), verf);
            cv.DrawText("tijd (sec)", (grafiekbreedte + 20), (graphy + 20), verf);

            //tekst met informatie
            verf.Color = Color.Black;
            verf.TextSize = 40;
            float afstand = (float)(this.Height - grafiekafstand);
            float tekstpositie = (afstand / 8);
            cv.DrawText($"Duratie: {maxtijd} seconden", graphx, tekstpositie * 2, verf);
            cv.DrawText($"Gelopen afstand: {maxafstand} meter", graphx, tekstpositie * 3, verf);
            cv.DrawText($"Gemiddelde snelheid: {gemsnelheid} meter/seconde", graphx, tekstpositie * 4, verf);
            cv.DrawText($"Maximaal bereikte snelheid: {maxsnelheid} meter/seconde", graphx, tekstpositie * 5, verf);

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