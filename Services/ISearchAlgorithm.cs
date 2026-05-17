using SearchAlgorithms.Models;

namespace SearchAlgorithms.Services {
    public interface ISearchAlgorithm {
        string Name { get; }
        SearchResult FindPath(string startCity, string targetCity);
    }
}
