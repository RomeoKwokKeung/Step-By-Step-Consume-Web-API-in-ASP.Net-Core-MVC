﻿using BookApiProject.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIGUI.Components
{
    public class CategoriesList
    {
        private List<CategoryDto> _allCategories = new List<CategoryDto>();

        public CategoriesList(List<CategoryDto> allCategories)
        {
            _allCategories = allCategories;
        }
        public List<SelectListItem> GetCategoryList()
        {
            var items = new List<SelectListItem>();
            foreach (var category in _allCategories)
            {
                items.Add(new SelectListItem
                {
                    Text = category.Name,
                    Value = category.Id.ToString(),
                    Selected = false
                });
            }

            return items;
        }

        public List<SelectListItem> GetCategoryList(List<int> selectedCategories)
        {
            var items = new List<SelectListItem>();
            foreach (var category in _allCategories)
            {
                items.Add(new SelectListItem
                {
                    Text = category.Name,
                    Value = category.Id.ToString(),
                    Selected = selectedCategories.Contains(category.Id) ? true : false
                });
            }

            return items;
        }
    }
}
