using FavoriteMovies.Shared;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using System.Net.Http.Json;

namespace FavoriteMovies.Client.Pages
{
    public partial class Search
    {
        [Inject]
        public HttpClient Http { get; set; } = new();

        private readonly string OMDBAPIKey = "86c39163";
        private readonly string OMDBAPIUrl = "https://www.omdbapi.com/?apikey=";

        private string searchTerm = String.Empty;
        private List<MovieSearchResultItem> OMDBMovies = new();
        private string totalResults = String.Empty;
        private string selectedPoster = String.Empty;
        private SfGrid<MovieSearchResultItem>? MoviesGrid;
        public List<MovieSearchResultItem>? SelectedMovies = new();

        private async Task SearchOMDB()
        {
            MovieSearchResult movieResult = await Http.GetFromJsonAsync<MovieSearchResult>($"{OMDBAPIUrl}{OMDBAPIKey}&s={searchTerm}");
            if (movieResult is not null)
            {
                OMDBMovies = movieResult.Search.ToList();
                totalResults = movieResult.totalResults;
            }
        }

        public async Task GetSelectedRecords(RowSelectEventArgs<MovieSearchResultItem> args)
        {
            SelectedMovies = await MoviesGrid.GetSelectedRecordsAsync();
            MovieSearchResultItem selectedMovie = args.Data;
            selectedPoster = selectedMovie.Poster;
        }
    }
}
