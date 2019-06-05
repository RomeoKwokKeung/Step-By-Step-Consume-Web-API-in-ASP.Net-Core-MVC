using BookApiProject.Dtos;
using BookApiProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIGUI.ViewModels
{
    public class ReviewerReviewsBooksViewModel
    {
        // single reviewer
        public ReviewerDto Reviewer { get; set; }
        // keep track of each review detail
        public IDictionary<ReviewDto, BookDto> ReviewBook { get; set; }
    }
}
