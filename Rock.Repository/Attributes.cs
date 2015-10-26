using System;

namespace Rock.Repository
{
    public class SaveAsStringAttribute : Attribute
    {
    }

    public class NoTableSaveAttribute : Attribute
    {
    }

    public class NoSaveAttribute : Attribute
    {
    }

    public class SaveAsClassNameAttribute : Attribute { }

}