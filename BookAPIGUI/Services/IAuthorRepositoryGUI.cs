using BookApiProject.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIGUI.Services
{
    public interface IAuthorRepositoryGUI
    {
        IEnumerable<AuthorDto> GetAuthors();
        AuthorDto GetAuthorById(int authorId);
        IEnumerable<BookDto> GetBooksByAuthor(int authorId);
        IEnumerable<AuthorDto> GetAuthorOfABook(int bookId);

    }
}
