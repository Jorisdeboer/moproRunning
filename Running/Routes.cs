using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Running
{
    [ActivityAttribute(Label = "Running", MainLauncher = false)]
    class Routes : Activity
    {
        Button b1, share;
        public TextView txt;
        public int nummer;
        public static string bericht;

        protected override void OnCreate(Bundle b)
        {
            base.OnCreate(b);
            txt = new TextView(this);
            txt.Text = "Route 1";

            LinearLayout.LayoutParams param;
            param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 0.25f);
            int schermbreedte = Resources.DisplayMetrics.WidthPixels;
            int linkerdeel = schermbreedte * (7 / 8);
            //de omlijsting van de buttons
            param.SetMargins(linkerdeel, 0, 0, 0);
            nummer = 1;
            bericht = "| coordinaten | tijd | snelheid |";

            b1 = new Button(this);
            b1.Text = "Terug naar het hoofdmenu";
            b1.Click += B1_Click;
            share = new Button(this);
            share.Text = "Share";
            share.Click += Sharing;

            LinearLayout layout;
            layout = new LinearLayout(this);
            layout.Orientation = Orientation.Vertical;

            LinearLayout routelayout;
            routelayout = new LinearLayout(this);
            routelayout.Orientation = Orientation.Horizontal;

            routelayout.AddView(txt);
            routelayout.AddView(share, param);

            layout.AddView(b1);
            layout.AddView(routelayout);

            this.SetContentView(layout);
        }

        private void Sharing(object sender, EventArgs e)
        {
            //loopje over de hele lijst, zodat alle elementen in het bericht komen
            foreach(PuntEnTijd pt in MainActivity.run.lijst)
            {
                bericht += $"\n{nummer} | {pt.info}";
                nummer++;
            }
            bericht += "\n Dit zijn de gegevens van mijn run, \n Kan jij dit verbeteren?";

            AlertDialog.Builder d;
            d = new AlertDialog.Builder(this);
            d.SetTitle("Weet je zeker dat je deze track wilt sharen?");
            d.SetPositiveButton("ja", Zenden);
            d.SetNeutralButton("Laat zien", laatZien);
            d.SetNegativeButton("nee", Niks);
            d.Show();

            void Zenden(object o, EventArgs ea)
            {
                Intent i;
                i = new Intent(Intent.ActionSend);
                i.SetType("text/plain");
                i.PutExtra(Intent.ExtraText, bericht);
                this.StartActivity(i);
            }

            void laatZien(object o, EventArgs ea)
            {
                Intent i;
                i = new Intent(this, typeof(AnalysisDisplay));
                StartActivity(i);
            }

            void Niks(object o, EventArgs ea)
            { }
        }

        private void B1_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder d;
            d = new AlertDialog.Builder(this);
            d.SetTitle("Weet je zeker dat je terug wilt naar het hoofdmenu?");
            d.SetPositiveButton("ja", Ja);
            d.SetNegativeButton("nee", Nee);
            d.Show();

            void Ja(object o, EventArgs ea)
            {
                Intent i;
                i = new Intent(this, typeof(Multiclass));
                StartActivity(i);
            }

            void Nee(object o, EventArgs ea)
            { }
        }
    }
}