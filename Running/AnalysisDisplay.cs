using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
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
            text.Text = Routes.bericht;
            Button b = new Button(this);
            LinearLayout layout = new LinearLayout(this);
            layout.Orientation = Orientation.Vertical;
            b.Click += B_Click;
            b.Text = "Terug naar Mijn Routes";
            layout.AddView(text);
            layout.AddView(b);

            this.SetContentView(layout);
        }

        private void B_Click(object sender, EventArgs e)
        {
            Intent i;
            i = new Intent(this, typeof(Routes));
            StartActivity(i);
        }
    }
}