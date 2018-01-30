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
    public class AnalysisDisplay : Activity
    {
        public TextView text;
        public static string deelbaarBericht;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            deelbaarBericht = "";
            text = new TextView(this);
            //deelbaarBericht moet je de highlights aan toevoegen (maxsnelheid, gelopen afstand, gelopen tijd + wat info om het 'menselijk' te maken
            text.Text = deelbaarBericht;
            LinearLayout layout = new LinearLayout(this);
            layout.Orientation = Orientation.Vertical;

            Button b = new Button(this);
            b.Click += B_Click;
            b.Text = "Terug";

            layout.AddView(text);
            layout.AddView(b);

            this.SetContentView(layout);
        }

        //knop voor teruggaan naar routes
        private void B_Click(object sender, EventArgs e)
        {
            Intent i;
            i = new Intent(this, typeof(MainActivity));
            StartActivity(i);
        }
    }

}