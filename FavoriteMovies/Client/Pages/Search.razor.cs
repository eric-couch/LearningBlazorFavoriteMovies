using FavoriteMovies.Shared;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
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
        private int page = 1;
        private List<MovieSearchResultItem> OMDBMovies = new();
        private int totalItems = 0;
        private string selectedPoster = String.Empty;
        private string? toastContent = String.Empty;
        private string? toastSuccess = "e-toast-success";

        private SfGrid<MovieSearchResultItem>? MoviesGrid;
        private SfPager? Page;
        private SfToast? ToastObj;
        public List<MovieSearchResultItem>? SelectedMovies = new();

        private async Task SearchOMDB()
        {
            MovieSearchResult movieResult = await Http.GetFromJsonAsync<MovieSearchResult>($"{OMDBAPIUrl}{OMDBAPIKey}&s={searchTerm}&page={page}");
            if (movieResult is not null)
            {
                OMDBMovies = movieResult.Search.ToList();
                totalItems = movieResult.totalResults;
            }
        }

        public async Task PageClick(PagerItemClickEventArgs args)
        {
            page = args.CurrentPage;
            await SearchOMDB();
        }

        public async Task GetSelectedRecords(RowSelectEventArgs<MovieSearchResultItem> args)
        {
            SelectedMovies = await MoviesGrid.GetSelectedRecordsAsync();
            MovieSearchResultItem selectedMovie = args.Data;
            selectedPoster = selectedMovie.Poster;
        }

        public async Task ToolbarClickHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "GridMovieAdd")
            {
                if (SelectedMovies is not null)
                {
                    foreach (var movie in SelectedMovies)
                    {
                        Movie newMovie = new() { imdbId = movie.imdbID };
                        var res = await Http.PostAsJsonAsync("api/add-movie", newMovie);
                        if (res.IsSuccessStatusCode)
                        {
                            toastContent = $"Added {movie.Title} to your favorites";
                            StateHasChanged();
                            await Task.Delay(100);
                            await ToastObj.ShowAsync();
                        } else
                        {
                            toastSuccess = "e-toast-warning";
                            toastContent = $"Failed to add movie {movie.Title} to your favorites";
                            StateHasChanged();
                            await Task.Delay(100);
                            await ToastObj.ShowAsync();
                        }
                    }
                }
            }
        }
    }
}
