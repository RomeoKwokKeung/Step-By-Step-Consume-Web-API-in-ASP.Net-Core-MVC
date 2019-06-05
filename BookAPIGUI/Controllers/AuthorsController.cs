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
    public class AuthorsController : Controller
    {
        IAuthorRepositoryGUI _authorRepository;
        ICountryRepositoryGUI _countryRepository;
        ICategoryRepositoryGUI _categoryRepository;

        public AuthorsController(IAuthorRepositoryGUI authorRepository, ICountryRepositoryGUI countryRepository, ICategoryRepositoryGUI categoryRepository)
        {
            _authorRepository = authorRepository;
            _countryRepository = countryRepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            var authors = _authorRepository.GetAuthors();

            if (authors.Count() <= 0)
            {
                ViewBag.Message = "There was a problem retreving authors from the database or no author exists";
            }

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(authors);
        }

        public IActionResult GetAuthorById(int authorId)
        {
            var author = _authorRepository.GetAuthorById(authorId);

            if (author == null)
            {
                ModelState.AddModelError("", "Some kind of error getting author");
                ViewBag.Message = $"There was a problem retrieving author with id {authorId} " +
                    $"From the database or no author with that id exists";
                author = new AuthorDto();
            }

            var country = _countryRepository.GetCountryOfAnAuthor(authorId);

            if (country == null)
            {
                ModelState.AddModelError("", "Some kind of error getting country");
                ViewBag.Message += $"There was a problem retrieving country with id {authorId} " +
                    $"From the database or no country with that id exists";
                country = new CountryDto();
            }

            var bookCategories = new Dictionary<BookDto, IEnumerable<CategoryDto>>();
            var books = _authorRepository.GetBooksByAuthor(authorId);
            if (books.Count() <= 0)
            {
                ViewBag.BookMessage = $"No books for {author.FirstName} {author.LastName} exists";
            }

            foreach (var book in books)
            {
                var categories = _categoryRepository.GetAllCategoriesOfABook(book.Id);
                bookCategories.Add(book, categories);
            }

            var authorCountryBooksCategoriesViewModel = new AuthorCountryBooksCategoriesViewModel
            {
                Author = author,
                Country = country,
                BookCategories = bookCategories
            };

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(authorCountryBooksCategoriesViewModel);
        }

        [HttpGet]
        public IActionResult CreateAuthor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAuthor(int CountryId, Author author)
        {
            using (var client = new HttpClient())
            {
                var countryDto = _countryRepository.GetCountryById(CountryId);

                if (countryDto == null || author == null)
                {
                    ModelState.AddModelError("", "Invalid country or author. Cannot create author!");
                    author = new Author();
                    return View(author);
                }

                author.Country = new Country
                {
                    Id = countryDto.Id,
                    Name = countryDto.Name,
                };

                client.BaseAddress = new Uri("http://localhost:60039/api/");
                var responseTask = client.PostAsJsonAsync("authors", author);
                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var newAuthorTask = result.Content.ReadAsAsync<Author>();
                    newAuthorTask.Wait();

                    var newAuthor = newAuthorTask.Result;
                    TempData["SuccessMessage"] = $"Author for {author.FirstName} {author.LastName} was successfully created.";
                    return RedirectToAction("GetAuthorById", new { authorId = newAuthor.Id });
                }

                ModelState.AddModelError("", "Author not created");
            }

            return View(author);
        }

        [HttpGet]
        public IActionResult UpdateAuthor(int CountryId, int authorId)
        {
            var authorDto = _authorRepository.GetAuthorById(authorId);
            var countryDto = _countryRepository.GetCountryOfAnAuthor(authorId);

            Author author = null;

            if (countryDto == null || authorDto == null)
            {
                ModelState.AddModelError("", "Invalid country or author. Cannot update author!");
                author = new Author();
            }
            else
            {
                author = new Author
                {
                    Id = authorDto.Id,
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Country = new Country
                    {
                        Id = countryDto.Id,
                        Name = countryDto.Name
                    }
                };
            }

            return View(author);
        }

        [HttpPost]
        public IActionResult UpdateAuthor(int CountryId, Author authorToUpdate)
        {
            var countryDto = _countryRepository.GetCountryById(CountryId);

            if (countryDto == null || authorToUpdate == null)
            {
                ModelState.AddModelError("", "Invalid country or author. Cannot update review!");
            }
            else
            {
                authorToUpdate.Country = new Country
                {
                    Id = countryDto.Id,
                    Name = countryDto.Name,
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:60039/api/");
                    var responseTask = client.PutAsJsonAsync($"authors/{authorToUpdate.Id}", authorToUpdate);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Author updated";
                        return RedirectToAction("GetAuthorById", new { authorId = authorToUpdate.Id });
                    }

                    ModelState.AddModelError("", "Unexpected Error. Author Not Updated");
                }
            }

            return View(authorToUpdate);
        }

        [HttpGet]
        public IActionResult DeleteAuthor(int authorId)
        {
            var authorDto = _authorRepository.GetAuthorById(authorId);

            if (authorDto == null)
            {
                ModelState.AddModelError("", "Invalid author. Cannot delete review!");
                authorDto = new AuthorDto();
            }

            return View(authorDto);
        }

        [HttpPost]
        public IActionResult DeleteAuthor(int authorId, string authorFirstName, string authorLastName)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:60039/api/");
                var responseTask = client.DeleteAsync($"authors/{authorId}");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Author {authorFirstName} {authorLastName} was successfully deleted.";

                    return RedirectToAction("Index");
                }

                if ((int)result.StatusCode == 409)
                {
                    ModelState.AddModelError("", $"Author {authorFirstName} {authorLastName} cannot be deleted because " +
                                                $"it is used by at least one book");
                }
                else
                {
                    ModelState.AddModelError("", "Some kind of error. Author not deleted!");
                }
            }

            var authorDto = _authorRepository.GetAuthorById(authorId);
            return View(authorDto);
        }
    }
}