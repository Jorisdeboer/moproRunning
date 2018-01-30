using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace Running
{
    [Activity(Label = "Running", MainLauncher = true)]
    class Multiclass : Activity
    {
        Button b1, b2;

        protected override void OnCreate(Bundle b)
        {
            base.OnCreate(b);
            TextView t1;
            t1 = new TextView(this);
            t1.Text = "Kies wat je wilt doen";

            b1 = new Button(this);
            b1.Text = "Open Map";
            b2 = new Button(this);
            b2.Text = "Afsluiten";
            b1.Click += clicked;
            b2.Click += clicked2;

            LinearLayout layout;
            layout = new LinearLayout(this);
            layout.Orientation = Orientation.Vertical;

            layout.AddView(t1);
            layout.AddView(b1);
            layout.AddView(b2);

            this.SetContentView(layout);
        }


        private void clicked(object sender, EventArgs e)
        {
            Intent i;
            i = new Intent(this, typeof(MainActivity));
            StartActivity(i);
        }

        private void clicked2(object sender, EventArgs e)
        {
            AlertDialog.Builder d;
            d = new AlertDialog.Builder(this);
            d.SetTitle("Weet je zeker dat je af wilt sluiten?");
            d.SetPositiveButton("ja", Stoppen);
            d.SetNegativeButton("nee", Doorgaan);
            d.Show();

            void Stoppen(object o, EventArgs ea)
            { this.FinishAffinity(); }

            void Doorgaan(object o, EventArgs ea)
            { }
        }
    }
}