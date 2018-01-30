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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            text = new TextView(this);
            text.Text = "test";

            LinearLayout layout = new LinearLayout(this);
            layout.Orientation = Orientation.Vertical;

            Button b = new Button(this);
            b.Click += B_Click;
            b.Text = "Terug naar Mijn Routes";

            layout.AddView(text);
            layout.AddView(b);

            this.SetContentView(layout);
        }

        //knop voor teruggaan naar routes
        private void B_Click(object sender, EventArgs e)
        {
            Intent i;
            i = new Intent(this, typeof(Routes));
            StartActivity(i);
        }
    }

}