using learning.Data;
using learning.Models;
using Microsoft.EntityFrameworkCore;

namespace learning.Services
{

    public interface ISearch
    {
        Task<List<Product>> SearchProductsAsync(string query);
    }

    public class SearchService : ISearch
    {
        private readonly learningContext _context;

        public SearchService(learningContext context)
        {
            _context = context;
        }


        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Product>(); // return empty list if query is null or empty
            var tokens = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            IQueryable<Product> searchQuery = _context.Product;


            foreach (var token in tokens)
            {
                searchQuery = searchQuery.Where(p =>
                p.Name.Contains(token) ||
                p.Description.Contains(token)
            );
            }

            return await searchQuery.ToListAsync();
        }


    }
}
