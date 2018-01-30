using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;


namespace Running
{
    [ActivityAttribute(Label = "Running", MainLauncher = false)]
    class Routes : Activity
    {
        Button b1, share, opslaan, laden, analyze;
        public TextView txt;
        public string file1;

        protected override void OnCreate(Bundle b)
        {
            base.OnCreate(b);

            LinearLayout.LayoutParams param;
            param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 0.25f);
            //de omlijsting van de buttons
            param.SetMargins(0, 0, 0, 0);

            txt = new TextView(this);
            txt.Text = "Fake Track";

            //de buttons van deze pagina
            b1 = new Button(this);
            b1.Text = "Terug naar het hoofdmenu";
            b1.Click += B1_Click;
            share = new Button(this);
            share.Text = "Share";
            share.Click += Sharing;
            opslaan = new Button(this);
            opslaan.Text = "Route opslaan";
            opslaan.Click += Opslaan;
            laden = new Button(this);
            laden.Text = "Laden";
            laden.Click += Laden;

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

            layout.AddView(b1);
            layout.AddView(routelayout);

            this.SetContentView(layout);
        }

        //hier wordt een string gemaakt met alle coordinaten en hun tijd, die gebruikt kan worden voor share en voor save
        public static string MaakBericht()
        {
            string bericht = "";
            foreach (PuntEnTijd pt in MainActivity.run.lijst)
            {
                bericht += $"{pt.info}\n";
            }
            return bericht;
        }
    	
	    private void Sharing(object sender, EventArgs e)
        {

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
                i.PutExtra(Intent.ExtraText, MaakBericht());
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

        private void Analyze(object o, EventArgs e)
        {
            Intent i = new Intent(this, typeof(AnalyseActivity));
            StartActivity(i);
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

        public void Opslaan(object o, EventArgs ea)
        {   
            string dir1 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string dir2 = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string dir3 = System.IO.Path.Combine(dir2, "Routes");
            if (!Directory.Exists(dir3))
                Directory.CreateDirectory(dir3);
            file1 = System.IO.Path.Combine(dir3, "route.txt");
            File.WriteAllText(file1, MaakBericht());        
        }

        private void Laden(object o, EventArgs ea)

        {
            string dir1 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string dir2 = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string dir3 = System.IO.Path.Combine(dir2, "Routes");
            if (!Directory.Exists(dir3))
                Directory.CreateDirectory(dir3);
            file1 = System.IO.Path.Combine(dir3, "route.txt");
            MainActivity.run.lijst = new List<PuntEnTijd>();

            foreach (string regel in File.ReadLines(file1))
            {
                string[] woorden;
                    woorden = regel.Split(' ');
                MainActivity.run.lijst.Add(
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
    }
}