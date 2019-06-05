using BookAPIGUI.Models;
using BookAPIGUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIGUI.Components
{
    public class ReviewersListViewComponent : ViewComponent
    {
        IReviewerRepositoryGUI _reviewerRepository;

        public ReviewersListViewComponent(IReviewerRepositoryGUI reviewerRepository)
        {
            _reviewerRepository = reviewerRepository;
        }

        public IViewComponentResult Invoke()
        {
            var reviewers = _reviewerRepository.GetReviewers().
                                                OrderBy(r => r.LastName).
                                                Select(x => new { Id = x.Id, Value = x.FirstName + " " + x.LastName });

            var reviewersList = new ReviewerSelectList
            {
                ReviewersList = new SelectList(reviewers, "Id", "Value")
            };

            // partal view
            return View("_ReviewersList", reviewersList);
        }
    }
}
