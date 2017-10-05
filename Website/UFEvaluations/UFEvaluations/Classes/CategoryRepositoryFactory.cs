using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class CategoryRepositoryFactory
{
    private static CategoryRepositoryFactory instance;

    private CategoryRepositorySQL sqlObject;
    private CategoryRepositoryXML xmlObject;

    private void CategoryRepository() { }

    public static CategoryRepositoryFactory Instance
    {
        get
        {
            if (instance == null)
                instance = new CategoryRepositoryFactory();

            return instance;
        }
    }

    public CategoryRepositorySQL GetCategoryRepositorySQL()
    {
        return sqlObject;
    }

    public CategoryRepositoryXML GetCategoryRepositoryXML()
    {
        return xmlObject;
    }
}