using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;

namespace DatawedgeIntentExample3
{
    [Activity(Label = "Datawedge Intent Example 3", MainLauncher = true)]
    public class MainActivity : Activity
    {
        //  Instance used to communicate from broadcast receiver back to main activity
        public static MainActivity Instance;
        myBroadcastReceiver receiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MainActivity.Instance = this;
            receiver = new myBroadcastReceiver();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
        }

        protected override void OnResume()
        {
            base.OnResume();
            //  Register the broadcast receiver dynamically
            RegisterReceiver(receiver, new IntentFilter(Resources.GetString(Resource.String.activity_intent_filter_action)));
        }

        protected override void OnPause()
        {
            base.OnPause();
            UnregisterReceiver(receiver);
        }

        public void DisplayResult(Intent intent)
        {
            //  Output the scanned barcode on the screen.  Bear in mind older JB devices will use the legacy DW parameters on unbranded devices.
            String decodedSource = intent.GetStringExtra(Resources.GetString(Resource.String.datawedge_intent_key_source));
            String decodedData = intent.GetStringExtra(Resources.GetString(Resource.String.datawedge_intent_key_data));
            String decodedLabelType = intent.GetStringExtra(Resources.GetString(Resource.String.datawedge_intent_key_label_type));

            if (decodedSource == null)
            {
                decodedSource = intent.GetStringExtra(Resources.GetString(Resource.String.datawedge_intent_key_source_legacy));
                decodedData = intent.GetStringExtra(Resources.GetString(Resource.String.datawedge_intent_key_data_legacy));
                decodedLabelType = intent.GetStringExtra(Resources.GetString(Resource.String.datawedge_intent_key_label_type_legacy));
            }
            TextView scanSourceTxt = FindViewById<TextView>(Resource.Id.txtScanScource);
            TextView scanDataTxt = FindViewById<TextView>(Resource.Id.txtScanData);
            TextView scanLabelTypeTxt = FindViewById<TextView>(Resource.Id.txtScanDecoder);
            scanSourceTxt.Text = "Scan Source: " + decodedSource;
            scanDataTxt.Text = "Scan Data: " + decodedData;
            scanLabelTypeTxt.Text = "Scan Decoder: " + decodedLabelType;
        }
    }

    //  Broadcast receiver to receive our scanned data from Datawedge
    [BroadcastReceiver(Enabled = true)]
    public class myBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            String action = intent.Action;
            if (action.Equals(MainActivity.Instance.Resources.GetString(Resource.String.activity_intent_filter_action)))
            {
                //  A barcode has been scanned
                MainActivity.Instance.RunOnUiThread(() => MainActivity.Instance.DisplayResult(intent));
            }
        }

    }
}

