using BookApiProject.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIGUI.Services
{
    public interface IReviewRepositoryGUI
    {
        IEnumerable<ReviewDto> GetReviews();
        IEnumerable<ReviewDto> GetReviewsOfABook(int bookId);
        ReviewDto GetReviewById(int reviewId);
        BookDto GetBookOfAReview(int reviewId);
    }
}
