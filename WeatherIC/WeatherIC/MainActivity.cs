using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Widget;
using System;
using System.Linq;


namespace WeatherIC
{
    [Activity(Label = "WeatherIC", MainLauncher = true, Icon = "@drawable/appicon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity, ILocationListener
    {
        // Global Variables
        LocationManager locMgr;

        TextView tvLocality, tvTemperature, tvDate;
        TextView tvHumidity, tvPressure, tvWindDirection;
        TextView tvTempMin, tvTempMax, tvSunRise, tvSunSet;
        ImageView ivCloudIcon;

        TextView tvAddress, tvLat, tvLong;        
        ImageView ivLocation, ivAddress, ivMap, ivWeather;
        
        // Forecast Components
        TextView tvDay1, tvDay1Temperature;
        TextView tvDay2, tvDay2Temperature;
        TextView tvDay3, tvDay3Temperature;
        TextView tvDay4, tvDay4Temperature;
        TextView tvDay5, tvDay5Temperature;
        ImageView ivDay1Cloud, ivDay2Cloud, ivDay3Cloud, ivDay4Cloud, ivDay5Cloud;

        RESThandler objRest;
        
        AutoCompleteTextView acLocality;        
        string[] chosenLocality = new string[] { "Auckland", "Hamilton", "Wellington", "Christchurch", "Dunedin", "Queenstown" };


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Initialising the Components
            tvDate = FindViewById<TextView>(Resource.Id.tvDate);
            tvLocality = FindViewById<TextView>(Resource.Id.tvLocality);
            ivCloudIcon = FindViewById<ImageView>(Resource.Id.ivCloudIcon);
            tvTemperature = FindViewById<TextView>(Resource.Id.tvTemperature);
            tvHumidity = FindViewById<TextView>(Resource.Id.tvHumidity);
            tvPressure = FindViewById<TextView>(Resource.Id.tvPressure);
            tvWindDirection = FindViewById<TextView>(Resource.Id.tvWindDirection);
            tvTempMin = FindViewById<TextView>(Resource.Id.tvTempMin);
            tvTempMax = FindViewById<TextView>(Resource.Id.tvTempMax);
            tvSunRise = FindViewById<TextView>(Resource.Id.tvSunRise);
            tvSunSet = FindViewById<TextView>(Resource.Id.tvSunSet);

            tvAddress = FindViewById<TextView>(Resource.Id.tvAddress);
            tvLat = FindViewById<TextView>(Resource.Id.tvLat);
            tvLong = FindViewById<TextView>(Resource.Id.tvLong);

            tvDay1 = FindViewById<TextView>(Resource.Id.tvDay1);
            ivDay1Cloud = FindViewById<ImageView>(Resource.Id.ivDay1Cloud);
            tvDay1Temperature = FindViewById<TextView>(Resource.Id.tvDay1Temperature);
            tvDay2 = FindViewById<TextView>(Resource.Id.tvDay2);
            ivDay2Cloud = FindViewById<ImageView>(Resource.Id.ivDay2Cloud);
            tvDay2Temperature = FindViewById<TextView>(Resource.Id.tvDay2Temperature);
            tvDay3 = FindViewById<TextView>(Resource.Id.tvDay3);
            ivDay3Cloud = FindViewById<ImageView>(Resource.Id.ivDay3Cloud);
            tvDay3Temperature = FindViewById<TextView>(Resource.Id.tvDay3Temperature);
            tvDay4 = FindViewById<TextView>(Resource.Id.tvDay4);
            ivDay4Cloud = FindViewById<ImageView>(Resource.Id.ivDay4Cloud);
            tvDay4Temperature = FindViewById<TextView>(Resource.Id.tvDay4Temperature);
            tvDay5 = FindViewById<TextView>(Resource.Id.tvDay5);
            ivDay5Cloud = FindViewById<ImageView>(Resource.Id.ivDay5Cloud);
            tvDay5Temperature = FindViewById<TextView>(Resource.Id.tvDay5Temperature);

            ivWeather = FindViewById<ImageView>(Resource.Id.ivWeather);
            ivWeather.Click += GetWeather;
            ivLocation = FindViewById<ImageView>(Resource.Id.ivLocation);
            ivLocation.Click += GetLocation;
            ivAddress = FindViewById<ImageView>(Resource.Id.ivAddress);
            ivAddress.Click += GetAddress;
            ivMap = FindViewById<ImageView>(Resource.Id.ivMap);
            ivMap.Click += OpenMapActivity;
                        
            chosenLocality = Resources.GetStringArray(Resource.Array.locality);
            acLocality = FindViewById<AutoCompleteTextView>(Resource.Id.acLocality);
            acLocality.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.TestListItem, chosenLocality);
            acLocality.ItemClick += acLocality_ItemClick;
            
            tvDate.Text = DateTime.Today.ToShortDateString(); // Sets the Current Date 

            // Disables the ImageViews when clicked
            ivAddress.Enabled = false; 
            ivMap.Enabled = false;
            ivWeather.Enabled = false;            
        }

        void acLocality_ItemClick(object sender, AdapterView.ItemClickEventArgs e) // Event argument to set the Latitude & Longitude based on item clicked on the AutoComplete Text
        {
            string chosenLocality = acLocality.Text; // Sets the value as that on the AutoComplete text when the item is clicked
           
            if (chosenLocality.Contains("Auckland")) // Sets the value of Latitude & Longitude as below when "Auckland" is clicked
            {
                tvLat.Text = "-36.8406";
                tvLong.Text = "174.7400";
            }

            else if (chosenLocality.Contains("Hamilton")) // Sets the value of Latitude & Longitude as below when "Hamilton" is clicked
            {
                tvLat.Text = "-37.7833";
                tvLong.Text = "175.2833";
            }

            else if (chosenLocality.Contains("Wellington")) // Sets the value of Latitude & Longitude as below when "Wellington" is clicked
            {
                tvLat.Text = "-41.2889";
                tvLong.Text = "174.7772";
            }

            else if (chosenLocality.Contains("Christchurch")) // Sets the value of Latitude & Longitude as below when "Christchurch" is clicked
            {
                tvLat.Text = "-43.5300";
                tvLong.Text = "172.6203";
            }

            else if (chosenLocality.Contains("Dunedin")) // Sets the value of Latitude & Longitude as below when "Dunedin" is clicked
            {
                tvLat.Text = "-45.8667";
                tvLong.Text = "170.5000";
            }

            else if (chosenLocality.Contains("Queenstown")) // Sets the value of Latitude & Longitude as below when "Queenstown" is clicked
            {
                tvLat.Text = "-45.0311";
                tvLong.Text = "168.6625";
            }

            // Enables the ImageViews when an item in the AutoComplete text is clicked
            ivAddress.Enabled = true;
            ivMap.Enabled = true;
            ivWeather.Enabled = true;

            ivLocation.Enabled = false; // Disables clicking the GPS button to populate current location
            tvAddress.Text = ""; // Clears out the Address texts when an item in AutoComplete text is clicked

            Toast.MakeText(this, "Location Change - Refresh WEATHER.", ToastLength.Short).Show(); // Populate to remind user to refresh by pressing Weather button in every location change           
        }

        void GetWeather(object sender, EventArgs e) // Runs to fetch the Weather (Current & 5-Day Forecast)
        {
            if (tvLat.Text != "")
            {
                LoadTodaysWeatherAsync(); // Calls the function to run the Current Weather
                LoadWeatherForecastAsync(); // Calls the function to run the 5-Day Forecast

                acLocality.Text = ""; // Clears out the AutoTextView text when button is clicked and parameters are passed on  
                ivLocation.Enabled = true; // Enables clicking the GPS button to populate current location
                acLocality.Enabled = true; // Enables clicking the AutoCompleteView to select Locality
            }
        }
        
        void GetLocation(object sender, EventArgs e) // Runs to fetch through GPS the current location and set the Latitude and Longitude coordinates
        {
            Criteria locationCriteria = new Criteria();

            locationCriteria.Accuracy = Accuracy.Coarse;
            locationCriteria.PowerRequirement = Power.Medium;

            string locationProvider = locMgr.GetBestProvider(locationCriteria, true);

            if (locationProvider != null)
            {
                locMgr.RequestLocationUpdates(locationProvider, 2000, 1, this);
                //Toast.MakeText(this, "Provider:" + locationProvider, ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "No location providers available", ToastLength.Short).Show();
            }

            // Enables the ImageViews when an current location is clicked and populates the coordinates (Latitude and Longitude)
            ivAddress.Enabled = true;
            ivMap.Enabled = true;
            ivWeather.Enabled = true;

            acLocality.Enabled = false; // Disables clicking the AutoCompleteView to select Locality
            tvAddress.Text = ""; // Clears out the Address texts when GPS button is clicked

            Toast.MakeText(this, "Location Change - Refresh WEATHER.", ToastLength.Short).Show(); // Populate to remind user to refresh by pressing Weather button in every location change           
        }

        protected override void OnResume() // Runs the function everytime user comes back to it
        {
            base.OnResume();
            // Initialize the Location Manager
            locMgr = GetSystemService(Context.LocationService) as LocationManager;
        }

        public void OnProviderEnabled(string provider) // Runs the function when provider is available
        {
            Toast.MakeText(this, "Provider Enabled", ToastLength.Short).Show();
        }
        public void OnProviderDisabled(string provider) // Runs the function when provider is unavailable
        {
            Toast.MakeText(this, "Provider Disabled", ToastLength.Short).Show();
        }
        public void OnStatusChanged(string provider, Availability status, Bundle extras) // Runs the function to prompt provider status
        {
            Toast.MakeText(this, "Provider status" + status.ToString(), ToastLength.Short).Show();
        }

        public void OnLocationChanged(Android.Locations.Location location) // Runs the function and sets the value everytime there is a change in GPS location
        {
            tvLat.Text = location.Latitude.ToString();
            tvLong.Text = location.Longitude.ToString();
        }

        protected override void OnPause() // Runs to call the function when program is in idle
        {
            base.OnPause();
            locMgr.RemoveUpdates(this);
        }

        private async void GetAddress(object sender, EventArgs e) // Runs to fetch the current GPS address and sets it to show into the tvAddress text field
        {
            var geo = new Geocoder(this);
            var addresses = await geo.GetFromLocationAsync(Convert.ToDouble(tvLat.Text), Convert.ToDouble(tvLong.Text), 1);

            if (addresses.Count > 0)
            {
                addresses.ToList().ForEach(addr => tvAddress.Text = addr.GetAddressLine(0) + "\n" + addr.GetAddressLine(1) + "\n" + addr.GetAddressLine(2));
            }
            else
            {
                Toast.MakeText(this, "No address Found", ToastLength.Short).Show();
            }
        }

        public async void LoadTodaysWeatherAsync() // Runs to call and sets it into the parameter fields the response fetch from the URL through the RESThandler using the fields found from our class Response(CurrentWeather)
        {
            objRest = new RESThandler(@"http://api.openweathermap.org/data/2.5/weather?lat=" + Convert.ToDouble(tvLat.Text) + "&lon=" + Convert.ToDouble(tvLong.Text) + "&type=accurate&mode=xml&units=metric&APPID=1f324d7043102d230f6b41b4781b2cfd");
            var Response = await objRest.ExecuteRequestAsync();
            tvLocality.Text = Response.City.Name;
            string cloud = Response.Weather.Icon;
            Koush.UrlImageViewHelper.SetUrlDrawable(ivCloudIcon, "http://openweathermap.org/img/w/" + cloud + ".png");
            tvTemperature.Text = Response.Temperature.Value + "°";
            tvTempMin.Text = Response.Temperature.Min + "°";
            tvTempMax.Text = Response.Temperature.Max + "°";            
            tvHumidity.Text = Response.Humidity.Value + "%";
            tvPressure.Text = Response.Pressure.Value + "hPa";
            tvWindDirection.Text = Response.Wind.Direction.Code;
            
            string cloud1 = Response.Weather.Icon;
            Koush.UrlImageViewHelper.SetUrlDrawable(ivDay1Cloud, "http://openweathermap.org/img/w/" + cloud1 + ".png");
            tvDay1Temperature.Text = Convert.ToString(Response.Temperature.Value + "°");

        }

        public async void LoadWeatherForecastAsync() // Runs to call and sets it into the parameter fields the response fetch from the URL through the RESThandler using the fields found from our class ResponseFC(Forecast Weather)
        {
            objRest = new RESThandler(@"http://api.openweathermap.org/data/2.5/forecast?lat=" + Convert.ToDouble(tvLat.Text) + "&lon=" + Convert.ToDouble(tvLong.Text) + "&type=accurate&mode=xml&units=metric&APPID=1f324d7043102d230f6b41b4781b2cfd");
            var ResponseFC = await objRest.ExecuteRequestAsyncFC();
            // Sunrise and Sunset for Current Weather
            tvSunRise.Text = ResponseFC.sun.rise.TimeOfDay.ToString();
            tvSunSet.Text = ResponseFC.sun.set.TimeOfDay.ToString();
            // Day 1
            tvDay1.Text = Convert.ToString(ResponseFC.forecast[1].from.DayOfWeek);            
            // Day 2
            tvDay2.Text = Convert.ToString(ResponseFC.forecast[9].from.DayOfWeek);
            string cloud2 = ResponseFC.forecast[9].symbol.var;
            Koush.UrlImageViewHelper.SetUrlDrawable(ivDay2Cloud, "http://openweathermap.org/img/w/" + cloud2 + ".png");
            Console.WriteLine("........................" + ResponseFC.forecast[9].clouds.value);  
            tvDay2Temperature.Text = Convert.ToString(ResponseFC.forecast[9].temperature.value + "°");
            // Day 3
            tvDay3.Text = Convert.ToString(ResponseFC.forecast[16].from.DayOfWeek);
            string cloud3 = ResponseFC.forecast[16].symbol.var;
            Koush.UrlImageViewHelper.SetUrlDrawable(ivDay3Cloud, "http://openweathermap.org/img/w/" + cloud3 + ".png");           
            tvDay3Temperature.Text = Convert.ToString(ResponseFC.forecast[16].temperature.value + "°");
            // Day 4
            tvDay4.Text = Convert.ToString(ResponseFC.forecast[24].from.DayOfWeek);
            string cloud4 = ResponseFC.forecast[24].symbol.var;
            Koush.UrlImageViewHelper.SetUrlDrawable(ivDay4Cloud, "http://openweathermap.org/img/w/" + cloud4 + ".png");
            tvDay4Temperature.Text = Convert.ToString(ResponseFC.forecast[24].temperature.value + "°");
            // Day 5
            tvDay5.Text = Convert.ToString(ResponseFC.forecast[32].from.DayOfWeek);
            string cloud5 = ResponseFC.forecast[32].symbol.var;
            Koush.UrlImageViewHelper.SetUrlDrawable(ivDay5Cloud, "http://openweathermap.org/img/w/" + cloud5 + ".png");
            tvDay5Temperature.Text = Convert.ToString(ResponseFC.forecast[32].temperature.value + "°");
            Console.WriteLine("**************************" + ResponseFC.location.location.latitude);
            Console.WriteLine("**************************" + ResponseFC.location.location.longitude);            
        }

        void OpenMapActivity(object sender, EventArgs e) // Runs to open Map Activity through intent and passed on our selected data fields (Latitude, Longitude & Address)
        {
            var mapactivity = new Intent(this, typeof(MapActivity));
            mapactivity.PutExtra("Latitude", tvLat.Text);
            mapactivity.PutExtra("Longitude", tvLong.Text);
            mapactivity.PutExtra("Address", tvAddress.Text);            
            StartActivity(mapactivity);
        }

    }
}

