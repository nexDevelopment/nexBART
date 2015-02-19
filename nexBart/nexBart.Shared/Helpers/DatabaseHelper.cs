﻿using nexBart.DataModel;
using nexBart.DataModels;
using nexBart.Models;
using SQLite;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace nexBart.Helpers
{
    class DatabaseHelper
    {
        static string dbPath;
        public static async Task CheckDB()
        {
            StorageFile file = null;
            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync("favorites.sqlite");
                dbPath = file.Path;
            }
            catch (FileNotFoundException)
            {
                MakeDB(file);
            }
        }

        private static async Task MakeDB(StorageFile file)
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("favorites.sqlite");
            file = await ApplicationData.Current.LocalFolder.GetFileAsync("favorites.sqlite");
            dbPath = file.Path;

            SQLiteAsyncConnection favDB = new SQLiteAsyncConnection(dbPath);
            await favDB.CreateTableAsync<StationData>();
        }

        public static async Task GetFavorites()
        {
            //ObservableCollection<Station>
            if (FavoritesModel.FavoriteStations != null) FavoritesModel.FavoriteStations.Clear();

            SQLiteAsyncConnection favDB = new SQLiteAsyncConnection(dbPath);
            var results = await favDB.QueryAsync<StationData>("SELECT * FROM Favorites");

            ObservableCollection<Station> tempList = new ObservableCollection<Station>();
            foreach(StationData d in results)
            {
                Station temp = new Station(d);
                temp.LinesList = await DeparturesHelper.GetDepartures(d);
                FavoritesModel.FavoriteStations.Add(temp);
            }
        }

        public static async Task AddFavorite(Station s)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbPath);
            await conn.InsertAsync(new StationData
            {
                Name = s.Name,
                Abbrv = s.abbrv
            });
        }
    }
}