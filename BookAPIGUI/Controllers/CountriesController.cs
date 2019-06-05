using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BookAPIGUI.Services;
using BookAPIGUI.ViewModels;
using BookApiProject.Dtos;
using BookApiProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookAPIGUI.Controllers
{
    public class CountriesController : Controller
    {
        ICountryRepositoryGUI _countryRepository;
        public CountriesController(ICountryRepositoryGUI countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public IActionResult Index()
        {
            var countries = _countryRepository.GetCountries();

            if (countries.Count() <= 0)
            {
                ViewBag.Message = "There was a problem retreving countries from the database or no country exists";
            }

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(countries);
        }

        public IActionResult GetCountryById(int countryId)
        {
            var country = _countryRepository.GetCountryById(countryId);

            if (country == null)
            {
                ModelState.AddModelError("", "Error getting a country");
                ViewBag.Message = $"There was a problem retrieving country with id {countryId} " +
                    $"From the database or no country with that id exists";
                country = new CountryDto();
            }

            var authors = _countryRepository.GetAuthorFromACountry(countryId);
            if (authors.Count() <= 0)
            {
                ViewBag.AuthorMessage = $"There was a problem retrieving authors with id {countryId} " +
                    $"From the database or no authors with that id exist";
            }

            var countryAuthorsViewModel = new CountryAuthorsViewModel
            {
                Country = country,
                Authors = authors
            };

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(countryAuthorsViewModel);
        }

        [HttpGet]
        public IActionResult CreateCountry()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCountry(Country country)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:60039/api/");
                var responseTask = client.PostAsJsonAsync("countries", country);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var newCountryTask = result.Content.ReadAsAsync<Country>();
                    newCountryTask.Wait();

                    var newCountry = newCountryTask.Result;
                    TempData["SuccessMessage"] = $"Country {newCountry.Name} was successfully created.";

                    return RedirectToAction("GetCountryById", new { countryId = newCountry.Id });
                }

                if ((int)result.StatusCode == 422)
                {
                    ModelState.AddModelError("", "Country Already Exists!");
                }
                else
                {
                    ModelState.AddModelError("", "Some kind of error. Country not created!");
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult UpdateCountry(int countryId)
        {
            var countryToUpdate = _countryRepository.GetCountryById(countryId);
            if (countryToUpdate == null)
            {
                ModelState.AddModelError("", "Error getting country!");
                countryToUpdate = new CountryDto();
            }
            return View(countryToUpdate);
        }

        [HttpPost]
        public IActionResult UpdateCountry(Country countryToUpdate)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:60039/api/");
                var responseTask = client.PutAsJsonAsync($"countries/{countryToUpdate.Id}", countryToUpdate);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Country was successfully updated.";

                    return RedirectToAction("GetCountryById", new { countryId = countryToUpdate.Id });
                }

                if ((int)result.StatusCode == 422)
                {
                    ModelState.AddModelError("", "Country Already Exists!");
                }
                else
                {
                    ModelState.AddModelError("", "Some kind of error. Country not updated!");
                }
            }

            var countryDto = _countryRepository.GetCountryById(countryToUpdate.Id);
            return View(countryDto);
        }

        [HttpGet]
        public IActionResult DeleteCountry(int countryId)
        {
            var country = _countryRepository.GetCountryById(countryId);
            if (country == null)
            {
                ModelState.AddModelError("", "Some kind of error. Country doesn't exist");
                country = new CountryDto();
            }

            return View(country);
        }

        [HttpPost]
        public IActionResult DeleteCountry(int countryId, string countryName)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:60039/api/");
                var responseTask = client.DeleteAsync($"countries/{countryId}");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Country {countryName} was successfully deleted.";

                    return RedirectToAction("Index");
                }

                if ((int)result.StatusCode == 409)
                {
                    ModelState.AddModelError("", $"Country {countryName} cannot be deleted because it is used by at least one author ");
                }
                else
                {
                    ModelState.AddModelError("", "Some kind of error. Country not deleted!");
                }
            }

            var countryDto = _countryRepository.GetCountryById(countryId);
            return View(countryDto);
        }
    }
}