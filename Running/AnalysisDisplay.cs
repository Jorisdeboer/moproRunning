using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Graphics;

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
            deelbaarBericht = $"Ik heb {Analyse.maxafstand} gelopen.\nHier heb ik {Analyse.maxtijd} seconden over gedaan.\nMijn gemiddelde snelheid was dus {Analyse.gemsnelheid}!\n\nKan jij dit verbeteren?";
            text = new TextView(this);
            text.Text = deelbaarBericht;
            LinearLayout layout = new LinearLayout(this);
            layout.Orientation = Orientation.Vertical;
            layout.SetBackgroundColor(Color.Beige);

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