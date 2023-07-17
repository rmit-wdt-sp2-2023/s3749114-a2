namespace CustomerApplication.Utilities.Paging;

public static class Paging
{
    // Calculates the total number of pages needed to
    // display a list, given a particular page size.

    public static int CalculateTotalPages(int listCount, int pageSize) =>
        (int)Math.Ceiling((double)listCount / (double)pageSize);

    // Takes a list of objects and returns the objects that
    // would fit on a page, given a particular page size and page number. 

    public static List<T> GetPage<T>(List<T> list, int pageNumber, int pageSize) =>
        list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
}