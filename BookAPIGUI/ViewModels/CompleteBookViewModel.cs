using BookApiProject.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIGUI.ViewModels
{
    public class CompleteBookViewModel
    {
        public BookDto Book { get; set; }
        public IEnumerable<CategoryDto> Categories { get; set; }
        public IDictionary<AuthorDto, CountryDto> AuthorsCountry { get; set; }
        public IDictionary<ReviewDto, ReviewerDto> ReviewsReviewers { get; set; }
        public decimal Rating { get; set; }
    }
}
