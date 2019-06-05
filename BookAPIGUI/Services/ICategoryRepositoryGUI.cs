using BookApiProject.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIGUI.Services
{
    public interface ICategoryRepositoryGUI
    {
        IEnumerable<CategoryDto> GetCategories();
        CategoryDto GetCategoryById(int categoryId);
        IEnumerable<BookDto> GetAllBooksForCategory(int categoryId);
        IEnumerable<CategoryDto> GetAllCategoriesOfABook(int bookId);
    }
}
