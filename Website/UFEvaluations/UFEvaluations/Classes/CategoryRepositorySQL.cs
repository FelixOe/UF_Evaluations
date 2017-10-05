using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class CategoryRepositorySQL : ICategoryRepository<Category>
{
    private static CategoryRepositorySQL instance;

    private CategoryRepositorySQL() { }

    public static CategoryRepositorySQL Instance
    {
        get
        {
            if (instance == null)
                instance = new CategoryRepositorySQL();

            return instance;
        }
    }

    public List<Category> listAll()
    {
        List<Category> categories = new List<Category>();

        return categories;
    }
}