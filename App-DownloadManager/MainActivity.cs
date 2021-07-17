using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Android.Net;

namespace App_DownloadManager
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button btn;TextView textView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            btn = (Button)FindViewById(Resource.Id.button1);
            textView = (TextView)FindViewById(Resource.Id.textView1);
            btn.Click += Btn_Click;


            if (CheckSelfPermission(Android.Manifest.Permission.WriteExternalStorage) == Android.Content.PM.Permission.Denied)
            {
                RequestPermissions(new string[] { Android.Manifest.Permission.WriteExternalStorage }, 0);
            }

            RegisterReceiver(new NetworkInternet(), new IntentFilter(ConnectivityManager.ConnectivityAction));
            RegisterReceiver(new DWR(), new IntentFilter(DownloadManager.ActionDownloadComplete));

        }

        public void SetText_Panel(string p)
        {
            textView.Text = p;
        }

        private void Btn_Click(object sender, System.EventArgs e)
        {
            string path_DWR = Android.OS.Environment.DirectoryDownloads;

            DownloadManager manager = GetSystemService(DownloadService) as DownloadManager;
            var uri = Android.Net.Uri.Parse("https://dl.vmusic.ir/2019/11/Morninglightmusic%20-%20Ten%20(2019)%20128k%20[Vmusic.ir]/01.%20Morninglightmusic%20-%20Presentation.mp3");
            DownloadManager.Request request = new DownloadManager.Request(uri);

            request.SetTitle("My File");
            request.SetDescription("Dowdloading ...");
            request.SetNotificationVisibility(DownloadVisibility.VisibleNotifyCompleted);
            request.SetVisibleInDownloadsUi(false);
            request.SetDestinationInExternalPublicDir(path_DWR, "MyFile.mp3");

            manager.Enqueue(request);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (requestCode == 0 && permissions[0]  == Android.Manifest.Permission.WriteExternalStorage)
            {
                if (grantResults[0] == Android.Content.PM.Permission.Denied)
                    Toast.MakeText(this, "Error PRS", ToastLength.Short).Show();
            }
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }


    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new string[] { DownloadManager.ActionDownloadComplete })]
    public class DWR : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == DownloadManager.ActionDownloadComplete)
            {
                var id = intent.GetLongExtra(DownloadManager.ExtraDownloadId, -1);
                Toast.MakeText(context, "Download Complete (ID) " + id, ToastLength.Short).Show();
            }
        }
    }


    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new string[] { ConnectivityManager.ConnectivityAction})]
    public class NetworkInternet : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                var main_ac = (MainActivity)context;
                if (intent.Action == ConnectivityManager.ConnectivityAction && Check(context))
                {
                    main_ac.SetText_Panel("Internet : True");
                }
                else
                {
                    main_ac.SetText_Panel("Internet : False");
                }
            }
            catch 
            {
            }
        
        }

        public bool Check(Context context)
        {
            try
            {
                ConnectivityManager manager = context.GetSystemService(Context.ConnectivityService) as ConnectivityManager;
                NetworkInfo info = manager.ActiveNetworkInfo;
                return info.IsConnected;
            }
            catch 
            {
                return false;
            }

        }
    }
}