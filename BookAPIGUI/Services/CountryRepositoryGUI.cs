using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BookApiProject.Dtos;

namespace BookAPIGUI.Services
{
    public class CountryRepositoryGUI : ICountryRepositoryGUI
    {
        public IEnumerable<AuthorDto> GetAuthorFromACountry(int countryId)
        {
            IEnumerable<AuthorDto> authors = new List<AuthorDto>();

            // calling api have to use HttpClient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:60039/api/");
                // get data
                var response = client.GetAsync($"countries/{countryId}/authors");
                response.Wait();
                // show result
                var result = response.Result;

                if (result.IsSuccessStatusCode)
                {
                    // read data
                    var readTask = result.Content.ReadAsAsync<IList<AuthorDto>>();
                    readTask.Wait();

                    authors = readTask.Result;
                }
            }

            return authors;
        }

        public IEnumerable<CountryDto> GetCountries()
        {
            IEnumerable<CountryDto> countries = new List<CountryDto>();

            // calling api have to use HttpClient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:60039/api/");
                // get data
                var response = client.GetAsync("countries");
                response.Wait();
                // show result
                var result = response.Result;

                if (result.IsSuccessStatusCode)
                {
                    // read data
                    var readTask = result.Content.ReadAsAsync<IList<CountryDto>>();
                    readTask.Wait();

                    countries = readTask.Result;
                }
            }

            return countries;
        }

        public CountryDto GetCountryById(int countryId)
        {
            CountryDto country = new CountryDto();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:60039/api/");

                var response = client.GetAsync($"countries/{countryId}");
                response.Wait();

                var result = response.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<CountryDto>();
                    readTask.Wait();

                    country = readTask.Result;
                }
            }

            return country;
        }

        public CountryDto GetCountryOfAnAuthor(int authorId)
        {
            CountryDto country = new CountryDto();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:60039/api/");

                var response = client.GetAsync($"countries/authors/{authorId}");
                response.Wait();

                var result = response.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<CountryDto>();
                    readTask.Wait();

                    country = readTask.Result;
                }
            }

            return country;
        }
    }
}
