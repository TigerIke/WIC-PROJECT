using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Widget;
using System;

namespace WeatherIC
{
    [Activity(Label = "MapActivity", Icon = "@drawable/appicon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MapActivity : Activity, IOnMapReadyCallback
    {
        // Global Variables
        private GoogleMap map;
        Button btnMap, btnSat, btnHybrid, btnTerrain;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.Map);
            
            // Initialising the Components
            btnMap = FindViewById<Button>(Resource.Id.btnMap);
            btnSat = FindViewById<Button>(Resource.Id.btnSatellite);
            btnHybrid = FindViewById<Button>(Resource.Id.btnHybrid);
            btnTerrain = FindViewById<Button>(Resource.Id.btnTerrain);

            btnMap.Click += btnMap_Click;
            btnSat.Click += btnSat_Click;
            btnHybrid.Click += btnHybrid_Click;            
            btnTerrain.Click += btnTerrain_Click;
            SetupMap();
        }

        void btnTerrain_Click(object sender, EventArgs e) // Sets the googles view "Terrain"
        {
            map.MapType = GoogleMap.MapTypeTerrain;
        }

        void btnHybrid_Click(object sender, EventArgs e) // Sets the googles view "Hybrid"
        {
            map.MapType = GoogleMap.MapTypeHybrid;
        }

        void btnSat_Click(object sender, EventArgs e) // Sets the googles view "Satellite"
        {
            map.MapType = GoogleMap.MapTypeSatellite;
        }

        void btnMap_Click(object sender, EventArgs e) // // Sets the googles view "Map"
        {
            map.MapType = GoogleMap.MapTypeNormal;
        }
        
        private void SetupMap() // Sets to view the Map when empty utilising the field passed on from Main Activity into our fragment 
        {
            if (map == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }

        }

        public void OnMapReady(GoogleMap googleMap) // Calls to set the Map through Google when ready and calls on the function adddatatomap() 
        {
            map = googleMap;
            adddatatomap();
        }

        public void adddatatomap() // Function to add data fields passed on from Main Activity (Latitude, Longitude, Address - then adds a tag to marker utilising the address field if there's any)
        {         
            MarkerOptions opt = new MarkerOptions();
            double lat = Convert.ToDouble(Intent.GetStringExtra("Latitude"));
            double lng = Convert.ToDouble(Intent.GetStringExtra("Longitude"));
            string address = Intent.GetStringExtra("Address");

            LatLng location = new LatLng(lat, lng);

            opt.SetPosition(location);
            opt.SetTitle(address);

            map.AddMarker(opt); // Adds a marker to map based on the address that was past on from our Main Activity

            // Positioning the camera to show the marker based on fields parameter as set below
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location);
            builder.Zoom(15);
            builder.Bearing(90);
            builder.Tilt(65);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            map.MoveCamera(cameraUpdate);
            // Marker window clicked event
            map.InfoWindowClick += map_InfoWindowClick;
            // Marker dragged event
            map.MarkerDragEnd += map_MarkerDragEnd;
        }

        void map_MarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e) // Sets to prompt when marker is dragged to its end
        {
            Toast.MakeText(this, "ended at " + e.Marker.Position.Latitude.ToString() + e.Marker.Position.Longitude.ToString(), ToastLength.Long).Show();
        }

        // Standard Info Window click event
        void map_InfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            Toast.MakeText(this, e.Marker.Position.Latitude.ToString() + e.Marker.Position.Longitude.ToString(), ToastLength.Long).Show();
        }

        public static void dothis() // Show through console that the program works
        {
            Console.WriteLine("I've DONE it!");
        }
        
    }

}