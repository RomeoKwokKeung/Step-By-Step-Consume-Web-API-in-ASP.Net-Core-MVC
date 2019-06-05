using BookApiProject.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIGUI.ViewModels
{
    public class CategoryBooksViewModel
    {
        // single category
        public CategoryDto Category { get; set; }
        // a list of book
        public IEnumerable<BookDto> Books { get; set; }
    }
}
