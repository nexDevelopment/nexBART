﻿using nexBart.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace nexBart.Helpers
{
    public class WebHelper
    {
        public static async Task<List<Line>> GetPredictions(Station station)
        {
            string predictionsUrl = Requests.MakePredictionsURL(station.Abbrv);
            var client = new HttpClient();
            var response = new HttpResponseMessage();
            var xmlDoc = new XDocument();

            //Make sure to pull from network not cache everytime
            client.DefaultRequestHeaders.IfModifiedSince = System.DateTime.Now;
            try
            {
                response = await client.GetAsync(new Uri(predictionsUrl));
                response.EnsureSuccessStatusCode();
                var reader = await response.Content.ReadAsStringAsync();
                xmlDoc = XDocument.Parse(reader);
            }
            catch(Exception)
            {
                //ErrorHandler.NetworkError("Error getting predictions. Check network connection and try again.");
            }

            return XmlParser.ParsePredictions(xmlDoc);
        }

        public static async Task<List<Alert>> GetAlerts()
        {
            string advisoryURL = Requests.MakeAdvsURL();
            string elevURL = Requests.MakeElevURL();

            var client = new HttpClient();
            var response = new HttpResponseMessage();
            XDocument advisoryXml = new XDocument();
            XDocument elevatorXml = new XDocument();
            string reader;

            //Make sure to pull from network not cache everytime
            client.DefaultRequestHeaders.IfModifiedSince = System.DateTime.Now;
            try
            {
                response = await client.GetAsync(new Uri(advisoryURL));
                response.EnsureSuccessStatusCode();
                reader = await response.Content.ReadAsStringAsync();
                advisoryXml = XDocument.Parse(reader);

                response = await client.GetAsync(new Uri(elevURL));
                response.EnsureSuccessStatusCode();
                reader = await response.Content.ReadAsStringAsync();
                elevatorXml = XDocument.Parse(reader);
            } 
            catch(Exception)
            {

            }

            return await XmlParser.Alerts(advisoryXml, elevatorXml);
        }

        public static async Task<Train[]> GetTrainDetails()
        {
            return new Train[1];
        }
    }
}
