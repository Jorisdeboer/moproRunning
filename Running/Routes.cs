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
        public static Button b1, share, analyze;
        public TextView txt;
        string bericht;

        protected override void OnCreate(Bundle b)
        {
            base.OnCreate(b);
            txt = new TextView(this);
            txt.Text = "Fake Track";

            LinearLayout.LayoutParams param;
            param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 0.25f);
            //de omlijsting van de buttons
            param.SetMargins(5, 0, 5, 0);
            bericht = "";

            //de buttons van deze pagina
            b1 = new Button(this);
            b1.Text = "Terug naar het hoofdmenu";
            b1.Click += B1_Click;
            share = new Button(this);
            share.Text = "Share";
            share.Click += Sharing;

            analyze = new Button(this);
            analyze.Text = "Analyzeer";
            analyze.Click += Analyze;

            LinearLayout layout;
            layout = new LinearLayout(this);
            layout.Orientation = Orientation.Vertical;

            LinearLayout routelayout = new LinearLayout(this);
            routelayout.Orientation = Orientation.Horizontal;

            routelayout.AddView(txt);
            routelayout.AddView(share, param);
            routelayout.AddView(analyze, param);

            layout.AddView(b1);
            layout.AddView(routelayout);

            this.SetContentView(layout);
        }

        private void Analyze(object sender, EventArgs e)
        {
            Intent i;
            i = new Intent(this, typeof(AnalyseActivity));
            StartActivity(i);
        }

        private void Sharing(object sender, EventArgs e)
        {
            //loop over de hele lijst, zodat alle elementen in het bericht komen
            foreach (PuntEnTijd pt in MainActivity.run.lijst)
            {
                bericht += $" {pt.info} \n"; 
            }

            //info om de track te kunnen sharen
            AlertDialog.Builder d;
            d = new AlertDialog.Builder(this);
            d.SetTitle("Weet je zeker dat je deze track wilt sharen?");
            d.SetPositiveButton("ja", Zenden);
            d.SetNeutralButton("Laat zien", laatZien);
            d.SetNegativeButton("nee", Niks);
            d.Show();

            //verzend de track
            void Zenden(object o, EventArgs ea)
            {
                Intent i;
                i = new Intent(Intent.ActionSend);
                i.SetType("text/plain");
                i.PutExtra(Intent.ExtraText, bericht);
                this.StartActivity(i);
            }

            //ga naar de analyse pagina
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