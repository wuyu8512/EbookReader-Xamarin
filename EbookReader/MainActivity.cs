﻿using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace EbookReader
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static Activity Activity;
        public WebView WebView;

        private readonly string[] _permissionsStorage = { Manifest.Permission.ReadExternalStorage };
        private const int RequestPermissionCode = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity = this;
            // 创建Web服务器，这里要注意一下，启动的是Asp.Net Core 2时代的产物，所以各种中间件依赖等不能装太高的版本
            KestrelWebHost.Server.CreateServer();

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // 申请读取文件权限
            RequestAllPower();

            WebView = FindViewById<WebView>(Resource.Id.webView1);
            WebView.SetWebContentsDebuggingEnabled(true);
            var webSetting = WebView.Settings;
            webSetting.JavaScriptEnabled = true;
            webSetting.DomStorageEnabled = true;
            webSetting.AllowFileAccess = true;
            webSetting.AllowFileAccessFromFileURLs = true;
            //WebView.AddJavascriptInterface(new Device(this), "device");
            //WebView.SetWebViewClient(new MyWebViewClient());
            //WebView.SetWebChromeClient(new MyWebChromeClient(this));

            //WebView.LoadUrl("http://10.0.2.2:8080");
            WebView.LoadUrl($"file:///android_asset/Web/index.html");
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void RequestAllPower()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
            {
                if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadExternalStorage))
                {
                }
                else
                {
                    ActivityCompat.RequestPermissions(this, _permissionsStorage, RequestPermissionCode);
                }
            }

            if (Build.VERSION.SdkInt > BuildVersionCodes.Q)
            {
                if (!Environment.IsExternalStorageManager)
                {
                    var callIntent = new Intent(Settings.ActionManageAllFilesAccessPermission);
                    StartActivity(callIntent);
                }
            }
        }

        // 后退事件
        public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Up)
            {
                if (WebView.CanGoBack())
                {
                    WebView.GoBack();
                    return true;
                }
            }

            return base.OnKeyUp(keyCode, e);
        }
    }
}