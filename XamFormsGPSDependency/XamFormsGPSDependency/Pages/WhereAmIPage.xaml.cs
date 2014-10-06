using System;
using System.Collections.Generic;
using Xamarin.Forms;
using XamFormsGPSDependency.DependencySvcTts;
using XamFormsGPSDependency.ViewModel;
using Xamarin.Forms.Maps;
using XamFormsGPSDependency.DependencySvcGps;


namespace XamFormsGPSDependency.Pages
{
    public partial class WhereAmIPage : ContentPage
    {
        LocationViewModel viewModel  = new LocationViewModel();

        public WhereAmIPage()
        {
            InitializeComponent ();
            map.MoveToRegion (MapSpan.FromCenterAndRadius (viewModel.Location, Distance.FromMiles (2000)));
            MessagingCenter.Subscribe<IGeoLocation, Position>(this, "LocationChanged", (sender, arg) =>
                {
                    this.LocationChanged();
                });
        }

        void slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (map.VisibleRegion != null)
            {
                double dblZoom = e.NewValue;
                var lat = viewModel.Location.Latitude / (Math.Pow(2, dblZoom));
                var lon = viewModel.Location.Longitude / (Math.Pow(2, dblZoom));
                map.MoveToRegion(new MapSpan(map.VisibleRegion.Center, lat, lon));
            }

        }

        void btnGo_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<ITextToSpeech>().Speak("Starting the GPS service.");
            viewModel.GetLocationCommand.Execute(null);
        } //end SetContent

        void tap_Addr(object sender, EventArgs e)
        {
            DependencyService.Get<ITextToSpeech>().Speak(viewModel.Address);
        }

        void tap_GPS(object sender, EventArgs e)
        {
            DependencyService.Get<ITextToSpeech>().Speak(viewModel.DisplayLocation);
        }

        private void LocationChanged()
        {
            DependencyService.Get<ITextToSpeech>().Speak("Your location has been found.");

            this.PlotLocation();
        }


        private void PlotLocation()
        {
            Pin pin = new Pin()
            {
                Type = PinType.Place,
                Position = viewModel.Location,
                Label = "You're Here!",
            };
            map.Pins.Add(pin);

            MapSpan ms = MapSpan.FromCenterAndRadius(viewModel.Location, Distance.FromMiles(1));
            map.MoveToRegion(ms);

        }


    }
}
